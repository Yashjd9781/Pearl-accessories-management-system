using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Project_pearl.Controllers
{
    [Route("UserCategory")]
    public class UserCategory : Controller
    {
        SqlConnection con = new SqlConnection("Data Source=DESKTOP-GS79Q5E;Initial Catalog=db_pearl;Integrated Security=True;Encrypt=False;");
        SqlCommand cmd = new SqlCommand();

        [Route("CategoryShow")]
        public IActionResult Index()
        {
            return View("../Home/ViewCategory");
        }


        [Route("GetCategories")]
        public IActionResult GetCategories()
        {
            string html = "";
            con.Open();
            cmd = new SqlCommand("SELECT c.category_id, c.category_name, MIN(v.image_url) AS image_url FROM tbl_category c LEFT JOIN tbl_product p ON c.category_id = p.category_id LEFT JOIN tbl_variants v ON p.product_id = v.product_id where c.isdeleted=1 GROUP BY c.category_id, c.category_name ;", con);
            SqlDataReader reader = cmd.ExecuteReader();
            int productCount = 0;
            html += "<div style='display:grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); grid-gap: 10px; background-color: #f0f0f0; padding: 1px;'>"; // Start of grid container

            while (reader.Read())
            {
                html += "<div style='display:inline-block;text-align:center ; margin-right:10px; background-color: #fff; padding: 10px; border-radius: 5px; box-shadow: 0px 0px 5px rgba(0, 0, 0, 0.1);'>" +
                "<a href='/UserProduct/ProductShow/" + reader["category_id"] + "'>" +
                "<img src='/assets/img/" + reader["image_url"] + "' style='width:200px; height:200px; border-radius: 10px; background-color: #fff;'></img>" +
                "<div style='font-size:16px; margin-top:5px; color:black; ; padding: 5px; border-radius: 5px;' class='product-name' onmouseover=\"this.style.color='#ff6600'\" onmouseout=\"this.style.color='black'\">" + reader["category_name"] + "</div>" +

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
                html += "</div>";
            }





            con.Close();
            return Content(html, "text/html");

        }
    }

}