using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Project_pearl.Controllers
{
    [Route("UserProduct")]
    public class UserProduct : Controller
    {
        SqlConnection con = new SqlConnection("Data Source=DESKTOP-GS79Q5E;Initial Catalog=db_pearl;Integrated Security=True;Encrypt=False;");
        SqlCommand cmd = new SqlCommand();
        [Route("ProductShow/{id}")]
        public IActionResult Index(int id)
        {
            ViewBag.Id = id;
            return View("../Home/ViewProduct");
        }

        [Route("GetProducts")]
        public IActionResult GetProducts(int id)
        {
            string html = "";
            con.Open();
            cmd = new SqlCommand("SELECT p.product_id, p.product_name, MIN(v.image_url) AS image_url, MIN(v.price) AS variant_price FROM tbl_product p JOIN tbl_variants v ON p.product_id = v.product_id WHERE p.category_id = "+id+" And p.isdeleted = 1 GROUP BY p.product_id, p.product_name;", con);
            SqlDataReader reader = cmd.ExecuteReader();
            int productCount = 0;
            html += "<div style='display:grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); grid-gap: 10px; background-color: #f0f0f0; padding: 10px;'>"; // Start of grid container

            while (reader.Read())
            {
                html += "<div style='display:inline-block;text-align:center ; margin-right:10px; background-color: #fff; padding: 10px; border-radius: 5px; box-shadow: 0px 0px 5px rgba(0, 0, 0, 0.1);'>" +
                        "<a href='/UserDescription/DescriptionShow/" + reader["product_id"] + "' style='text-decoration: none; color: black;'>" +
                        "<img src='/assets/img/" + reader["image_url"] + "' style='width:200px; height:200px;' class='product-image'></img>" +
                        "<div style='font-size:16px; margin-top:5px; color:black; font-weight: bold;'>" + reader["product_name"] + "</div>" +
                        "<div style='font-size:14px; margin-top:5px; color:blue;' class='product-price'>" + reader["variant_price"] + "</div>" +
                        "</a>" +
                        "</div>";

                productCount++;

                // If four products have been added, close the current grid container and open a new one
                if (productCount % 4 == 0)
                {
                    html += "</div><br>"; // End of current grid container and add line break
                    html += "<div style='display:grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); grid-gap: 10px; background-color: #f0f0f0; padding: 10px;'>"; // Start of new grid container
                }
            }

            // Close the last grid container if it's not closed already
            if (productCount % 4 != 0)
            {
                html += "</div><br>"; // End of current grid container and add line break
            }
            con.Close();
            return Content(html, "text/html");
        }
    }
}