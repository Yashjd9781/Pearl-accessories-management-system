using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

namespace Project_pearl.Controllers
{
    [Route("Variant")]
    public class Variant : Controller
    {
        SqlConnection con = new SqlConnection("Data Source=DESKTOP-GS79Q5E;Initial Catalog=db_pearl;Integrated Security=True;Encrypt=False;");
        SqlCommand cmd = new SqlCommand();
        private readonly string _uploadPathUser = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Assets", "img");

        [Route("VariantAddView")]
        public IActionResult Index()
        {
            return View("../Admin/Variant");
        }


        [Route("VariantShow")]
        public IActionResult view()
        {
            return View("../Admin/ViewVariant");
        }

        [Route("VariantUpdateView/{id:int}")]
        public IActionResult UpdateProductView(int id)
        {
            ViewData["id"] = id;
            return View("../Admin/UpdateVariant");
        }

        [Route("dropdown/Product")]
        public IActionResult GetCategories()
        {

            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT product_id, product_name FROM tbl_product where isdeleted = 1 ", con);
            SqlDataReader reader = cmd.ExecuteReader();

            string result = "<option value=''>Select Product</option>"; // Add a default option
            while (reader.Read())
            {
                int product_id = reader.GetInt32(0);
                string product_name = reader.GetString(1);
                result += $"<option value='{product_id}'>{product_name}</option>";
            }
            con.Close();

            return Ok(result);

        }

        [Route("VariantAdd")]

        public string insert(string size, string weight, string price, string stock_quantity, int product_id, string image)
        {
            string result = "";
            con.Open();
            SqlCommand cmd = new SqlCommand("INSERT INTO tbl_variants (size, weight, price, stock_quantity, product_id, image_url) VALUES ('" + size + "', '" + weight + "', '" + price + "', '" + stock_quantity + "' , '" + product_id + "', '"+image+"');", con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                result = "success";
            }
            else
            {
                result = "error";
            }
            con.Close();
            return result;
        }


        [Route("VariantView")]
        public ActionResult ProductView()
        {
            string sqlQuery = "SELECT * from tbl_variants WHERE isdeleted = 1 ;";
            string html = "";
            con.Open();
            SqlCommand cmd1 = new SqlCommand(sqlQuery, con);
            SqlDataReader reader = cmd1.ExecuteReader();

            while (reader.Read())
            {
                html += "<tr>" +
                        "<td>" + reader["variant_id"] + "</td>" +
                        "<td>" + reader["size"] + "</td>" +
                        "<td>" + reader["weight"] + "</td>" +
                        "<td>" + reader["price"] + "</td>" +
                        "<td>" + reader["stock_quantity"] + "</td>" +
                        "<td> <img src='/assets/img/" + reader["image_url"] + "' alt='image' style='width:100px; height:50px;'></img></td>" +
                        "<td><button style='background-color:#28a745; color: white; border: none; padding: 8px 16px; text-align: center; text-decoration: none; display: inline-block; border-radius: 4px; cursor: pointer;' onclick='updatevar(" + reader["variant_id"] + ")'>Update</button></td>" +
                        "<td><button style='background-color:#dc3545; color: white; border: none; padding: 8px 16px; text-align: center; text-decoration: none; display: inline-block; border-radius: 4px; cursor: pointer;'  onclick='deletevar(" + reader["variant_id"] + ")'>Delete</button></td>" +
                        "</tr>";

            }
            con.Close();
            return Content(html, "text/html");
        }


        [Route("VariantFetchName")]
        public IActionResult ProductFetchName(int id)
        {
            string sqlQuery = "SELECT v.size, v.weight,v.price, v.stock_quantity,pi.image_url FROM tbl_variants v JOIN tbl_product p ON p.product_id = v.product_id JOIN (SELECT *,ROW_NUMBER() OVER (PARTITION BY product_id ORDER BY image_id) AS rn FROM tbl_product_image) pi ON pi.product_id = p.product_id AND pi.rn = 1 WHERE v.variant_id = '" + id + "';";

            con.Open();
            SqlCommand cmd = new SqlCommand(sqlQuery, con);
            SqlDataReader result = cmd.ExecuteReader();
            Dictionary<string, object> userData = new Dictionary<string, object>();
            while (result.Read())
            {
                userData.Add("size", result["size"]);
                userData.Add("weight", result["weight"]);
                userData.Add("price", result["price"]);
                userData.Add("stock_quantity", result["stock_quantity"]);
                userData.Add("image_url", result["image_url"]);
            }
            string jsonData = JsonConvert.SerializeObject(userData);
            return Content(jsonData, "application/json");

        }



        [Route("VariantUpdate")]
        public string updatevar(int id, string size, string weight, string price, string stock_quantity)
        {
            con.Close();
            con.Open();
            cmd = new SqlCommand("update tbl_variants set size = '" + size + "' , weight = '" + weight + "' , price = '" + price + "', stock_quantity = '" + stock_quantity + "' where variant_id = '" + id + "' ; ", con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                con.Close();
                return "success";
            }
            else
            {
                con.Close();
                return "error";
            }
        }

        [Route("VariantDelete")]
        public string deletevar(int id)
        {
            con.Close();
            con.Open();
            cmd = new SqlCommand("update tbl_variants set isdeleted = '0' where variant_id = '" + id + "' ; ", con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                con.Close();
                return "success";
            }
            else
            {
                con.Close();
                return "error";
            }
        }


        [HttpPost("VariantImage")]
        public async Task<IActionResult> UploadImageClub([FromForm] IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("No image file uploaded");
            }

            string fileName = image.FileName;
            string filePath = Path.Combine(_uploadPathUser, fileName);

            if (System.IO.File.Exists(filePath))
            {
                return BadRequest("An image with the same name already exists. Please rename your image and try again.");
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return Ok(new { success = true, message = "Image uploaded successfully!" });

        }
    }
}