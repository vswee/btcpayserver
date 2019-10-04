using BTCPayServer.U2F.Models;

namespace BTCPayServer.Areas.Nicolas.Models.Account
{
    public class SecondaryLoginViewModel
    {
        public LoginWith2faViewModel LoginWith2FaViewModel { get; set; }
        public LoginWithU2FViewModel LoginWithU2FViewModel { get; set; }
    }
}
