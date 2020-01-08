using Microsoft.AspNetCore.Mvc;

namespace Monitoras.Web.Controllers {
    public class HomeController : SecureController {
        public IActionResult Index () {
            System.Console.WriteLine ("Hola!");
            return View ();
        }

        public IActionResult Error () {
            return View ();
        }
    }
}