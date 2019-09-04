using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BTCPayServer.Models;
using BTCPayServer.Models.AccountViewModels;
using BTCPayServer.Services;
using BTCPayServer.Services.Mails;
using BTCPayServer.Services.Stores;
using BTCPayServer.Logging;
using BTCPayServer.Security;
using System.Globalization;
using BTCPayServer.Services.U2F;
using BTCPayServer.Services.U2F.Models;
using Newtonsoft.Json;
using NicolasDorier.RateLimits;
using BTCPayServer.Models.ServerViewModels;
using BTCPayServer.Models.NewStuff;

namespace BTCPayServer.Controllers
{
    [Authorize(AuthenticationSchemes = Policies.CookieAuthentication)]
    [Route("[controller]/[action]")]
    public class AccountDashboardController : Controller
    {
        private UserManager<ApplicationUser> _UserManager;
        public AccountDashboardController(UserManager<ApplicationUser> userManager)
        {
            _UserManager = userManager;
        }

        [Route("/Account/")]
        public IActionResult Index()
        {
            var model = new NewDashboardModel
            {
                UsersPartialModel = _UserManager.getUsersFromDatabase(0, 50)
            };

            return View("/Views/NewStuff/AccountIndex.cshtml", model);
        }



    }

    public static class UserManagerExt {

        public static UsersViewModel getUsersFromDatabase(this UserManager<ApplicationUser> userManager, int skip, int count)
        {
            var users = new UsersViewModel();
            users.Users = userManager.Users.Skip(skip).Take(count)
                .Select(u => new UsersViewModel.UserViewModel
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
    }
}