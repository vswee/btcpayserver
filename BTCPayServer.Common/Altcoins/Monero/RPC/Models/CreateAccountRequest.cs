using Newtonsoft.Json;

namespace BTCPayServer.Altcoins.Monero.RPC.Models
{
    public partial class CreateAccountRequest
    {
        [JsonProperty("label")] public string Label { get; set; }
    }
}
