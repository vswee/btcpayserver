using BTCPayServer.Models.ServerViewModels;
using BTCPayServer.Models.StoreViewModels;
using BTCPayServer.Services.Stores;
using BTCPayServer.Controllers;

namespace BTCPayServer.Areas.Nicolas.Models.Account
{
    public class AccountIndexViewModel
    {
        public UsersViewModel UsersPartialModel { get; set; }
        public StoresViewModel StoresPartialModel { get; set; }
    }

}

