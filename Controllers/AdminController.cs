using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;


namespace Project_pearl.Controllers
{

    [Route("admin")]
    public class AdminController : Controller
    {
        [Route("header")]
        public IActionResult header()
        {
            return View("../Admin/admin_header");
        }
        [Route("AdminProfileView")]
        public IActionResult profile()
        {
            return View("../Admin/Admin_Profile");
        }

        [Route("ChangePassword")]
        public IActionResult change()
        {
            return View("../Admin/Admin_changepassword");
        }
        [Route("AdminView")]
        public IActionResult admin()
        {
            return View("../Admin/admin");
        }
       

    }
}
