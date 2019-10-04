using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BTCPayServer.Areas.Nicolas.Controllers
{
    [Area("Nicolas")]
    public class BaseController : Controller
    {
        public override RedirectToActionResult RedirectToAction(string actionName)
        {
            return base.RedirectToAction(actionName, new { area = "Nicolas" });
        }

        public override RedirectToActionResult RedirectToAction(string actionName, string controllerName)
        {
            return base.RedirectToAction(actionName, controllerName, new { area = "Nicolas" });
        }

        protected IActionResult RedirectToLocal(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}
