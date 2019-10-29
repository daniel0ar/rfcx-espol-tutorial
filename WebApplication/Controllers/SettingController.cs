using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace WebApplication {

    public class SettingController : Controller {
        public IActionResult Index() {
            return View();
        }
    }
    
}