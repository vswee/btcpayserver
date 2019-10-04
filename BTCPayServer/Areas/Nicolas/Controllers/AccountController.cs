using System;
using System.Threading.Tasks;
using BTCPayServer.Areas.Nicolas.Models.Account;
using BTCPayServer.Security;
using BTCPayServer.Services.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BTCPayServer.Data;
using Microsoft.AspNetCore.Authentication;
using NicolasDorier.RateLimits;
using BTCPayServer.U2F;
using BTCPayServer.U2F.Models;
using BTCPayServer.Logging;
using Microsoft.Extensions.Logging;
using BTCPayServer.Services;
using BTCPayServer.Services.Mails;
using BTCPayServer.Areas.Nicolas.Models.Partials;
using System.Linq;

namespace BTCPayServer.Areas.Nicolas.Controllers
{
    [Authorize(AuthenticationSchemes = Policies.CookieAuthentication)]
    public class AccountController : BaseController
    {
        private StoreRepository _storeRepo;
        private UserManager<ApplicationUser> _userManager;
        private readonly U2FService _u2FService;
        private readonly BTCPayServerEnvironment _btcPayServerEnvironment;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly EmailSenderFactory _emailSenderFactory;
        ILogger _logger;
        public AccountController(StoreRepository storeRepo,
            UserManager<ApplicationUser> userManager,
            U2FService u2FService,
            BTCPayServerEnvironment btcPayServerEnvironment,
            SignInManager<ApplicationUser> signInManager,
            EmailSenderFactory emailSenderFactory)
        {
            _storeRepo = storeRepo;
            _userManager = userManager;
            _u2FService = u2FService;
            _btcPayServerEnvironment = btcPayServerEnvironment;
            _signInManager = signInManager;
            _emailSenderFactory = emailSenderFactory;
            _logger = Logs.PayServer;
        }

        public IActionResult Index()
        {
            var model = new AccountIndexViewModel
            {
                UsersPartialModel = getUsersFromDatabase(_userManager, 0, 50),
                InvoicesPartialModel = new Models.Partials.InvoicesFrameViewModel()
                //StoresPartialModel = _Repo.fetchStoresAsync(GetUserId())
            };

            return View(model);
        }

        public static UsersViewModel getUsersFromDatabase(UserManager<ApplicationUser> userManager, int skip, int count)
        {
            var users = new UsersViewModel();
            users.Users = userManager.Users.Skip(skip).Take(count)
                .Select(u => new UsersViewModel.UserListItem
                {
                    Name = u.UserName,
                    Email = u.Email,
                    Id = u.Id
                }).ToList();
            users.Skip = skip;
            users.Count = count;
            users.Total = userManager.Users.Count();

            return users;
        }

        //private async Task<StoresViewModel> fetchStoresAsync(StoreRepository repo, string v)
        //{
        //    var result = new StoresViewModel();
        //    var stores = await repo.GetStoresByUserId(v);
        //    for (int i = 0; i < stores.Length; i++)
        //    {
        //        var store = stores[i];
        //        result.Stores.Add(new StoresViewModel.StoreViewModel()
        //        {
        //            Id = store.Id,
        //            Name = store.StoreName,
        //            WebSite = store.StoreWebsite,
        //            IsOwner = store.HasClaim(Policies.CanModifyStoreSettings.Key)
        //        });
        //    }
        //    return result;
        //}

        private string GetUserId()
        {
            return _userManager.GetUserId(User);
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated && string.IsNullOrEmpty(returnUrl))
                return RedirectToLocal();
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [RateLimitsFilter(ZoneLimits.Login, Scope = RateLimitsScope.RemoteAddress)]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // Require the user to have a confirmed email before they can log on.
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    if (user.RequiresEmailConfirmation && !await _userManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError(string.Empty, "You must have a confirmed email to log in.");
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }

                if (!await _userManager.IsLockedOutAsync(user) && await _u2FService.HasDevices(user.Id))
                {
                    if (await _userManager.CheckPasswordAsync(user, model.Password))
                    {
                        LoginWith2faViewModel twoFModel = null;

                        if (user.TwoFactorEnabled)
                        {
                            // we need to do an actual sign in attempt so that 2fa can function in next step
                            await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);
                            twoFModel = new LoginWith2faViewModel
                            {
                                RememberMe = model.RememberMe
                            };
                        }

                        return View("SecondaryLogin", new SecondaryLoginViewModel()
                        {
                            LoginWith2FaViewModel = twoFModel,
                            LoginWithU2FViewModel = await BuildU2FViewModel(model.RememberMe, user)
                        });
                    }
                    else
                    {
                        var incrementAccessFailedResult = await _userManager.AccessFailedAsync(user);
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return View(model);

                    }
                }

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return View("SecondaryLogin", new SecondaryLoginViewModel()
                    {
                        LoginWith2FaViewModel = new LoginWith2faViewModel()
                        {
                            RememberMe = model.RememberMe
                        }
                    });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private async Task<LoginWithU2FViewModel> BuildU2FViewModel(bool rememberMe, ApplicationUser user)
        {
            if (_btcPayServerEnvironment.IsSecure)
            {
                var u2fChallenge = await _u2FService.GenerateDeviceChallenges(user.Id,
                    Request.GetAbsoluteUriNoPathBase().ToString().TrimEnd('/'));

                return new LoginWithU2FViewModel()
                {
                    Version = u2fChallenge[0].version,
                    Challenge = u2fChallenge[0].challenge,
                    Challenges = u2fChallenge,
                    AppId = u2fChallenge[0].appId,
                    UserId = user.Id,
                    RememberMe = rememberMe
                };
            }

            return null;
        }



        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }




        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
                _emailSenderFactory.GetEmailSender().SendEmail(model.Email, "Reset Password",
                    $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");

                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}
