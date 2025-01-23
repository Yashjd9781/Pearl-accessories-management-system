using Microsoft.AspNetCore.Mvc;

namespace Project_pearl.Controllers
{
    [Route("staff")]
    public class StaffController : Controller
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
            return View("../Staff/Staff");
        }
    }
}
