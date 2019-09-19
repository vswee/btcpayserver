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


using BTCPayServer.Models.InvoicingModels;
using BTCPayServer.Services.Invoices;

using BTCPayServer.Models.StoreViewModels;

namespace BTCPayServer.Controllers
{
    // NOTE: Started separate controller as illustration of reducing number of injected dependencies
    [Authorize(AuthenticationSchemes = Policies.CookieAuthentication)]
    public class NewStuffController : Controller
    {
        private UserManager<ApplicationUser> _UserManager;
        private StoreRepository _Repo;
        public NewStuffController(UserManager<ApplicationUser> userManager, StoreRepository repos)
        {
            _UserManager = userManager;
            _Repo = repos;
        }

        [Route("/Account/")]
        public IActionResult Index()
        {
            var model = new NewDashboardModel
            {
                UsersPartialModel = _UserManager.getUsersFromDatabase(0, 50),
                //StoresPartialModel = _Repo.fetchStoresAsync(GetUserId())
            };

            return View("/Views/NewStuff/AccountIndex.cshtml", model);
        }

        private async Task<StoresViewModel> fetchStoresAsync(StoreRepository repo, string v)
        {
            StoresViewModel result = new StoresViewModel();
            var stores = await repo.GetStoresByUserId(v);
            for (int i = 0; i < stores.Length; i++)
            {
                var store = stores[i];
                result.Stores.Add(new StoresViewModel.StoreViewModel()
                {
                    Id = store.Id,
                    Name = store.StoreName,
                    WebSite = store.StoreWebsite,
                    IsOwner = store.HasClaim(Policies.CanModifyStoreSettings.Key)
                });
            }
            return result;
        }

        private string GetUserId()
        {
            return _UserManager.GetUserId(User);
        }

    }

    public static class UserManagerExt
    {
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
