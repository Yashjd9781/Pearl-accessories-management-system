using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Drawing;


namespace Project_pearl.Controllers
{
    [Route("UserDescription")]
    public class Description : Controller
    {
        SqlConnection con = new SqlConnection("Data Source=DESKTOP-GS79Q5E;Initial Catalog=db_pearl;Integrated Security=True;Encrypt=False;");
        SqlCommand cmd = new SqlCommand();

        [Route("DescriptionShow/{id}")]
        public IActionResult Index(int id)
        {
            ViewBag.Id = id;
            return View("../Home/Description");
        }

        [Route("GetProducts")]
        public IActionResult GetProducts(int id)
        {
            string html = "";
            con.Open();
            cmd = new SqlCommand("SELECT p.product_id, p.product_name, pi.image_url, MIN(v.price) AS variant_price FROM tbl_product AS p JOIN tbl_product_image AS pi ON p.product_id = pi.product_id JOIN tbl_variants AS v ON p.product_id = v.product_id WHERE p.category_id = '" + id + "' AND p.isdeleted=1 GROUP BY p.product_id, p.product_name, pi.image_url; ", con);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                html += "<div>" +
                "<a href='/UserProduct/ProductShow/" + reader["product_id"] + "'>" +
                    "<img src='/assets/img/" + reader["image_url"] + "' style='width:200 px; height:200px;'></img>" +
                    "<div>" + reader["product_name"] + "</div>" +
                    "<div>" + reader["variant_price"] + "</div>" +
                    "</a>" +
                    "</div><br>";
            }
            con.Close();
            return Content(html, "text/html");

        }

        [Route("VariantSize")]
        public string FetchSize(int id)
        {
            con.Open();
            cmd = new SqlCommand("select * from tbl_variants where product_id ='" + id + "';", con);
            SqlDataReader reader = cmd.ExecuteReader();
            string result = "";
            while (reader.Read())
            {
                int variant_id = reader.GetInt32(0);
                string size = reader.GetString(1);
                result += $"<option value={variant_id}>{size}</option>";
            }
            con.Close();
            return result;
        }


        [Route("UpdateDescription")]
        public IActionResult UpdateDescription(int id, int size)
        {
            ViewBag.vid = size;
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT v.product_id AS pid, v.image_url, p.product_name, p.description, v.price, p.status, v.weight, v.size FROM tbl_variants v INNER JOIN tbl_product p ON v.product_id = p.product_id WHERE v.product_id = @id AND v.variant_id = @size", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@size", size);

            SqlDataReader result = cmd.ExecuteReader();
            Dictionary<string, object> userData = new Dictionary<string, object>();
            while (result.Read())
            {
                userData.Add("product_id", result["pid"]);
                userData.Add("image", result["image_url"]);
                userData.Add("product_name", result["product_name"]);
                userData.Add("description", result["description"]);
                userData.Add("price", result["price"]);
                userData.Add("status", result["status"]);
                userData.Add("weight", result["weight"]);
                userData.Add("size", result["size"]);
            }

            string jsonData = JsonConvert.SerializeObject(userData);
            con.Close();
            return Content(jsonData, "appslication/json");
        }


        [Route("confirm_order_page/{id}")]
        public IActionResult ConfirmOrder(int id)
        {
            string email = HttpContext.Session.GetString("email");
            // Retrieve user ID from session
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT  tbl_variants.*, tbl_product.* FROM tbl_variants INNER JOIN  tbl_product ON tbl_product.product_id = tbl_variants.product_id WHERE tbl_variants.variant_id = '"+ id +"';", con);
            SqlDataReader result = cmd.ExecuteReader();
            if (result.Read())
            {
                ViewBag.image_url = result["image_url"];
                ViewBag.Product_name = result["Product_name"];
                ViewBag.price = result["price"];
                ViewBag.size = result["size"];
                ViewBag.stock_quantity = result["stock_quantity"];
                ViewBag.description = result["description"];
            }
            else
            {
                ViewBag.error1 = "SELECT  tbl_variants.*, tbl_product.* FROM tbl_variants INNER JOIN  tbl_product ON tbl_product.product_id = tbl_variants.product_id WHERE tbl_variants.variant_id = '" + id + "';";
            }
            con.Close();
            con.Open();
            SqlCommand cmd1 = new SqlCommand("select * from tbl_register where email='"+email+"';", con);
            SqlDataReader result1 = cmd1.ExecuteReader();
            if (result1.Read())
            {
                ViewBag.shipping_address = result1["shipping_address"];
                ViewBag.username = result1["username"];
                ViewBag.contact = result1["contact"];
            }
            else {
                ViewBag.email = email;
                ViewBag.error2 = "select * from tbl_register where username = '"+email+"' ";
            }
            con.Close();
            ViewBag.Id = id;
            return View("../Home/Order_confirmation");


        }

    }
}
 
