using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

namespace Project_pearl.Controllers
{
    [Route("Product")]
    public class Product : Controller
    {

        SqlConnection con = new SqlConnection("Data Source=DESKTOP-GS79Q5E;Initial Catalog=db_pearl;Integrated Security=True;Encrypt=False;");
        SqlCommand cmd = new SqlCommand();

        [Route("ProductAddView")]
        public IActionResult Index()
        {
            return View("../Admin/Product");
        }

        [Route("ProductShow")]
        public IActionResult view()
        {
            return View("../Admin/ViewProduct");
        }

        [Route("ProductUpdateView/{id:int}")]
        public IActionResult UpdateProductView(int id)
        {
            ViewData["id"] = id;
            return View("../Admin/UpdateProduct");
        }


        [Route("dropdown/Category")]
        public IActionResult GetCategories()
        {

            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT category_id, category_name FROM tbl_category where isdeleted = 1 ", con);
            SqlDataReader reader = cmd.ExecuteReader();

            string result = "<option value=''>Select Category</option>"; // Add a default option
            while (reader.Read())
            {
                int category_id = reader.GetInt32(0);
                string category_name = reader.GetString(1);
                result += $"<option value='{category_id}'>{category_name}</option>";
            }
            con.Close();

            return Ok(result);

        }

        [Route("ProductAdd")]
        public string insert(string product_name, string description, string status, int category_id)
        {
            con.Open();

            // Check if the product name already exists
            SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM tbl_product WHERE product_name = @product_name", con);
            checkCmd.Parameters.AddWithValue("@product_name", product_name);
            int count = (int)checkCmd.ExecuteScalar();

            if (count > 0)
            {
                con.Close();
                return "exists";
            }
            else
            {
                // Product name doesn't exist, proceed with insertion
                SqlCommand cmd = new SqlCommand("INSERT INTO tbl_product (product_name, description, status, category_id) VALUES (@product_name, @description, @status, @category_id)", con);
                cmd.Parameters.AddWithValue("@product_name", product_name);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@category_id", category_id);

                int rowsAffected = cmd.ExecuteNonQuery();
                con.Close();

                if (rowsAffected > 0)
                {
                    return "success";
                }
                else
                {
                    return "error";
                }
            }
        }

        [Route("ProductView")]
        public ActionResult ProductView()
        {
            string sqlQuery = "SELECT * FROM tbl_product where isdeleted = '1' ";
            string html = "";
            con.Open();
            SqlCommand cmd = new SqlCommand(sqlQuery, con);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                html += "<tr>" +
                        "<td>" + reader["product_id"] + "</td>" +
                        "<td>" + reader["product_name"] + "</td>" +
                        "<td>" + reader["description"] + "</td>" +
                        "<td>" + reader["created_at"] + "</td>" +
                        "<td>" + reader["updated_at"] + "</td>" +
                        "<td>" + reader["status"] + "</td>" +
                        "<td><button style='background-color:#28a745; color: white; border: none; padding: 8px 16px; text-align: center; text-decoration: none; display: inline-block; border-radius: 4px; cursor: pointer;' onclick='updatepro(" + reader["product_id"] + ")'>Update</button></td>" +
                        "<td><button style='background-color:#dc3545; color: white; border: none; padding: 8px 16px; text-align: center; text-decoration: none; display: inline-block; border-radius: 4px; cursor: pointer;'  onclick='deletepro(" + reader["product_id"] + ")'>Delete</button></td>" +
                        "</tr>";

            }
            con.Close();
            return Content(html, "text/html");
        }

        [Route("ProductFetchName")]
        public IActionResult ProductFetchName(int id)
        {
            string sqlQuery = "SELECT * FROM tbl_product where product_id='" + id + "'";
            con.Open();
            SqlCommand cmd = new SqlCommand(sqlQuery, con);
            SqlDataReader result = cmd.ExecuteReader();
            Dictionary<string, object> userData = new Dictionary<string, object>();
            while (result.Read())
            {
                userData.Add("product_name", result["product_name"]);
                userData.Add("desc", result["description"]);
                userData.Add("status", result["status"]);
            }
            string jsonData = JsonConvert.SerializeObject(userData);
            return Content(jsonData, "application/json");

        }



        [Route("ProductUpdate")]
        public string updatepro(int id, string name, string description, string status)
        {
            con.Close();
            con.Open();
            cmd = new SqlCommand("update tbl_product set product_name = '" + name + "' , description = '" + description + "' , status = '" + status + "' where product_id = '" + id + "' ; ", con);
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

        [Route("ProductDelete")]
        public string deletecat(int id)
        {
            con.Close();
            con.Open();
            cmd = new SqlCommand("update tbl_product set isdeleted = '0' where product_id = '" + id + "' ; ", con);
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
    }
}