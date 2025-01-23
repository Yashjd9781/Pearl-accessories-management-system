using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Project_pearl.Controllers
{
    [Route("Profile")]

    public class Profile : Controller
    {
        SqlConnection con = new SqlConnection("Data Source=DESKTOP-GS79Q5E;Initial Catalog=db_pearl;Integrated Security=True;Encrypt=False;");
        SqlCommand cmd = new SqlCommand();
        [Route("ProfileView")]
        public IActionResult profile()
        {
            return View("../Home/Profile");
        }

        [Route("ChangePassword")]
        public IActionResult change()
        {
            return View("../Home/ChangePassword");
        }
        [Route("UpdateProfile")]

        public string update(string fname, string lname, string contact,DateTime dob, string address, string shipping_address, string gender)
        {
            string email = HttpContext.Session.GetString("email").ToString();
          


            con.Open();
            cmd = new SqlCommand("update tbl_register set fname= '" + fname + "', lname = '" + lname + "',   contact = '" + contact + "',dob = '"+ dob.ToString("yyyy-MM-dd") + "',  address = '" + address + "',shipping_address = '" + shipping_address + "', gender = '" + gender + "' where email = '" + email + "' ", con);
         

            if (cmd.ExecuteNonQuery() > 0)
            {
                con.Close();
                return "success";
            }
            else
            {
                return "error";
            }
        }


        [Route("fetchUser")]
        public IActionResult index()
        {
            string email = HttpContext.Session.GetString("email").ToString();
            //string username = HttpContext.Session.GetString("username").ToString();

            con.Open();
            cmd = new SqlCommand("select * from tbl_register where email = '"+ email +"'", con);
            SqlDataReader result = cmd.ExecuteReader();
            Dictionary<string, object> userData = new Dictionary<string, object>();
            while (result.Read())
            {
               
                userData.Add("fname", result["fname"]);
                userData.Add("lname", result["lname"]);
                userData.Add("contact", result["contact"]);
                userData.Add("email", result["email"]);
                userData.Add("dob", result["dob"]);
                userData.Add("shipping_address", result["shipping_address"]);
                userData.Add("address", result["address"]);
                userData.Add("gender" , result["gender"]);
            }
            string jsonData = JsonConvert.SerializeObject(userData);
            return Content(jsonData, "application/json");
        }

        [Route("resetPassword")]
        public string reset(string oldpass, string newpassword)
        {
            string response = "";
            string email = HttpContext.Session.GetString("email").ToString();

            con.Open();
            cmd = new SqlCommand("SELECT * FROM tbl_register WHERE email='" + email + "';", con);
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                string oldPasswordFromDB = reader.GetString(3);
                con.Close();

                if (PasswordHasher.VerifyPassword(oldPasswordFromDB, oldpass))
                {
                    string hashedNewPassword = PasswordHasher.HashedPassword(newpassword);
                    con.Open();
                    cmd = new SqlCommand("UPDATE tbl_register SET Password='" + hashedNewPassword + "' where  email='" + email + "';", con);
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        con.Close();
                        response = "success";
                    }
                    else
                    {
                        con.Close();
                        response = "error";
                    }
                }
                else
                {
                    response = "old"; // Password provided does not match the old password
                }
            }
            else
            {
                con.Close();
                response = "email"; // Email not found in the database
            }
            return response;
        }


    }
}









