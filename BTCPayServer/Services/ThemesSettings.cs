using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BTCPayServer.Services
{
    public class ThemeSettings
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [MaxLength(500)]
        [Display(Name = "Custom bootstrap CSS file")]
        public string BootstrapCssUri { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [MaxLength(500)]
        [Display(Name = "Custom Creative Start CSS file")]
        public string CreativeStartCssUri { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [MaxLength(500)]
        [Display(Name = "Custom SB Admin 2 CSS file")]
        public string SBAdmin2CssUri { get; set; }
    }
}
