using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Project_pearl.Controllers
{
    [Route("order")]
    public class Order : Controller
    {

        SqlConnection con = new SqlConnection("Data Source=DESKTOP-GS79Q5E;Initial Catalog=db_pearl;Integrated Security=True;Encrypt=False;");
        SqlCommand cmd = new SqlCommand();
        public IActionResult Index()
        {
            return View();
        }
    }
}
