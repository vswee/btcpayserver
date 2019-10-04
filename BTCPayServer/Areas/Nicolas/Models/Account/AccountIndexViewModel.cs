using BTCPayServer.Areas.Nicolas.Models.Partials;

namespace BTCPayServer.Areas.Nicolas.Models.Account
{
    public class AccountIndexViewModel
    {
        public UsersViewModel UsersPartialModel { get; set; }
        //public StoresViewModel StoresPartialModel { get; set; }
        public InvoicesFrameViewModel InvoicesPartialModel { get; set; }
    }

}

