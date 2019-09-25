using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BTCPayServer.Areas.Nicolas.Models.Account;
using BTCPayServer.Models;
using BTCPayServer.Models.ServerViewModels;
using BTCPayServer.Models.StoreViewModels;
using BTCPayServer.Security;
using BTCPayServer.Services.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BTCPayServer.CommonExtenders;

namespace BTCPayServer.Areas.Nicolas.Controllers
{
    [Area("Nicolas")]
    [Authorize(AuthenticationSchemes = Policies.CookieAuthentication)]
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> _UserManager;
        private StoreRepository _Repo;
        public AccountController(UserManager<ApplicationUser> userManager, StoreRepository repos)
        {
            _UserManager = userManager;
            _Repo = repos;
        }

        [Route("/Account/")]
        public IActionResult Index()
        {
            var model = new AccountIndexViewModel
            {
                UsersPartialModel = _UserManager.getUsersFromDatabase(0, 50),
                //StoresPartialModel = _Repo.fetchStoresAsync(GetUserId())
            };

            return View("/Views/NewStuff/AccountIndex.cshtml", model);
        }

        private async Task<StoresViewModel> fetchStoresAsync(StoreRepository repo, string v)
        {
            var result = new StoresViewModel();
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
}
