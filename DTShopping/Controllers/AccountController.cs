using System.Threading.Tasks;
using System.Web.Mvc;
using DTShopping.Models;
using DTShopping.Repository;

namespace DTShopping.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private APIRepository _APIManager;

        public AccountController()
        {           
        }

        public AccountController(APIRepository Manager)
        {
            _APIManager = Manager;            
        }
        

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]        
        public async Task<ActionResult> Login(UserDetails User)
        {
            var result = await _APIManager.Login(User);

            return Json(result,JsonRequestBehavior.AllowGet);
        }        

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {            
            return View(model);
        }
        
        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

                
      
    }
}