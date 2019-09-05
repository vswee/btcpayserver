using BTCPayServer.Models.ServerViewModels;
using BTCPayServer.Models.StoreViewModels;
using BTCPayServer.Services.Stores;
using BTCPayServer.Controllers;

namespace BTCPayServer.Models.NewStuff
{
    public class NewDashboardModel
    {
        public UsersViewModel UsersPartialModel { get; set; }
        public StoresViewModel StoresPartialModel { get; set; }
    }

}

