using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Project_pearl.Controllers
{
    [Route("Category")]
    public class Category : Controller
    {
        SqlConnection con = new SqlConnection("Data Source=DESKTOP-GS79Q5E;Initial Catalog=db_pearl;Integrated Security=True;Encrypt=False;");
        SqlCommand cmd = new SqlCommand();

        [Route("CategoryAdd")]
        public IActionResult CategoryAdd()
        {
            return View("../Admin/category");
        }

        [Route("Categoryvv")]
        public IActionResult Categoryvv()
        {
            return View("../Admin/ViewCategory");
        }

        [Route("CategoryUpdateView/{id:int}")]
        public IActionResult UpdateCategoryView(int id)
        {
            ViewData["id"] = id;
            return View("../Admin/UpdateCategory");
        }

        [Route("CategoryView")]
        public ActionResult CategoryView()
        {
            string sqlQuery = "SELECT * FROM tbl_category where isdeleted = '1'";
            string html = "";
            con.Open();
            SqlCommand cmd = new SqlCommand(sqlQuery, con);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                html += "<tr>" +
                        "<td>" + reader["category_id"] + "</td>" +
                        "<td>" + reader["category_name"] + "</td>" +
                         "<td><button style='background-color:#28a745; color: white; border: none; padding: 8px 16px; text-align: center; text-decoration: none; display: inline-block; border-radius: 4px; cursor: pointer;' onclick='updatecat(" + reader["category_id"] + ")'>Update</button></td>" +
                         "<td><button style='background-color: #dc3545; color: white; border: none; padding: 8px 16px; text-align: center; text-decoration: none; display: inline-block; border-radius: 4px; cursor: pointer;' onclick='deletecat(" + reader["category_id"] + ")'>Delete</button></td>" +
                         "</tr>";

            }
            con.Close();
            return Content(html, "text/html");
        }

        [Route("CategoryFetchName")]
        public String CategoryFetchName(int id)
        {
            string sqlQuery = "SELECT * FROM tbl_category where category_id='" + id + "'";
            con.Open();
            SqlCommand cmd = new SqlCommand(sqlQuery, con);
            SqlDataReader reader = cmd.ExecuteReader();
            string name;
            if (reader.Read())
            {
                name = reader["Category_name"].ToString();
            }
            else
            {
                name = "error";
            }
            con.Close();
            return name;
        }


        [Route("CategoryDelete")]
        public string deletecat(int id)
        {
            con.Close();
            con.Open();
            cmd = new SqlCommand("update tbl_category set isdeleted = '0' where category_id = '" + id + "' ; ", con);
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

        [Route("CategoryUpdate")]
        public string updatecat(int id, string name)
        {
            con.Close();
            con.Open();
            cmd = new SqlCommand("update tbl_category set category_name = '" + name + "' where category_id = '" + id + "' ; ", con);
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


        [Route("insertcat")]
        public string insert(string category_name)
        {
            con.Close();
            con.Open();

            // Check if the category name already exists
            cmd = new SqlCommand("SELECT COUNT(*) FROM tbl_category WHERE category_name = @category_name", con);
            cmd.Parameters.AddWithValue("@category_name", category_name);
            int count = (int)cmd.ExecuteScalar();

            if (count > 0)
            {
                // Category name already exists, return an error message
                con.Close();
                return "exists";
            }
            else
            {
                // Category name doesn't exist, proceed with insertion
                cmd = new SqlCommand("INSERT INTO tbl_category (category_name, isdeleted) VALUES (@category_name, '1')", con);
                cmd.Parameters.AddWithValue("@category_name", category_name);

                if (cmd.ExecuteNonQuery() > 0)
                {
                    con.Close();
                    return "Success";
                }
                else
                {
                    return "Error";
                }
            }
        }

    }
}