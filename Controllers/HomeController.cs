using System.Diagnostics;
using Azure.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Project_pearl.Models;

namespace Project_pearl.Controllers
{
    [Route("app")]
    public class HomeController : Controller
    {
        SqlConnection con = new SqlConnection("Data Source=DESKTOP-GS79Q5E;Initial Catalog=db_pearl;Integrated Security=True;Encrypt=False;");
        SqlCommand cmd = new SqlCommand();

        [Route("LoginView")]
        public IActionResult Loginview()
        {
            return View("../Home/Register");
        }
        [Route("ProfileView")]
        public IActionResult ProfileView()
        {
            return View("../Home/Profile");
        }
        [Route("Change_Password")]
        public IActionResult Change_password()
        {
            return View("../Home/Change_password");
        }
        [Route("ForgotView")]
        public IActionResult ForgotView()
        {
            return View("../Home/Email_verification");
        }
        [Route("About_us")]
        public IActionResult About_us()
        {
            return View("../Home/About_us");
        }

        [Route("Contact_us")]
        public IActionResult contact_us()
        {
            return View("../Home/Contact_us");
        }

        [Route("RegisterView")]
        public IActionResult register()
        {
            return View("../Home/Register");
        }

       [Route("~/")]
        [Route("IndexView")]
        public IActionResult Index()
        {

            return View("../Home/Index");
        }

        [Route("checkSession")]
        public String CheckSession()
        {
            String email = "";
            if (HttpContext.Session.GetString("email") != null)
            {
                email = HttpContext.Session.GetString("email").ToString();
            }
            return email;
        }

        [Route("Login")]
        public string Login(string email, string password)
        {
            string role = ""; // Initialize role
            try
            {
                // Check if email or password is empty
                if (string.IsNullOrEmpty(email))
                {
                    return "empty email";
                }

                if (string.IsNullOrEmpty(password))
                {
                    return "empty password";
                }

                con.Open();
                string query = "SELECT * FROM tbl_register WHERE email = @Email";
                string user_id =  "SELECT user_id from tbl_register where email='" +email+ "'";


                string cart_id  = "SELECT cart_id from tbl_cart where user_id='"+user_id+"'";
                ViewBag.cart_id = cart_id;

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            string hashedPasswordFromDB = dr.GetString(3); // Assuming password is stored at index 3
                            if (PasswordHasher.VerifyPassword(hashedPasswordFromDB, password))
                            {
                                string username = dr.GetString(1);
                                string emaill = dr.GetString(2);
                                HttpContext.Session.SetString("email", emaill);
                                ViewBag.email = email;
                                HttpContext.Session.SetString("username", username);
                                HttpContext.Session.SetString("user_id", user_id);
                                ViewBag.uid = user_id;
                                HttpContext.Session.SetString("cart_id", cart_id);

                                role = dr.GetString(12).ToLower(); // Assuming role is stored at index 12
                                if (role == "u")
                                {
                                   

                                    role = "user";
                                }
                                else if (role == "a")
                                {
                                    role = "admin";
                                }
                                else if (role == "s")
                                {
                                    role = "staf";
                                }
                                else
                                {
                                    role = "unknown";
                                }
                            }
                            else
                            {
                                role = "wrong password";
                            }
                        }
                        else
                        {
                            role = "not found email";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                role = "error";
            }
            finally
            {
                con.Close(); // Close the connection
            }
            return role;
        }


        [Route("sendOtp")]

        public string SendOtp(string email)
        {
            string otp = EmailService.GenerateRandomOTP(6);
            if (EmailService.SendOTP(email, otp))
            {
                return otp;
                HttpContext.Session.SetString("email", email);

            }
            else
            {
                return "error";
            }
        }

        [Route("checkEmail")]
        public string checkEmail(string email)
        {
            if (EmailService.ValidateEmail(email))
            {
                con.Open();
                cmd = new SqlCommand("SELECT * FROM tbl_register where email = '" + email + "'; ", con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    con.Close();
                    return "exists";
                }
                else
                {
                    con.Close();
                    return "send";
                }
            }
            else
            {
                con.Close();
                return "notValid";
            }
        }

        [Route("forgotpassword")]
        public string reset(string email, string newPassword)
        {
            string Email = HttpContext.Session.GetString("email");

            con.Open();
            newPassword = PasswordHasher.HashedPassword(newPassword);
            cmd = new SqlCommand("update tbl_register set password = '" + newPassword + "' where email = '" + email + "';", con);

            if (cmd.ExecuteNonQuery() > 0)
            {
             
                return "success";
            }
            else
            {
                return "error";
            }
        }
        [Route("hasing/{str}")]
        public string Hashing(string str)
        {
            return PasswordHasher.HashedPassword(str);
        }

        [Route("changepassword")]
        public string ChangePassword(string newPassword)
        {
            string Email = HttpContext.Session.GetString("email");

            con.Open();
            SqlCommand cmd = new SqlCommand("UPDATE tbl_register SET password = '" + newPassword + "' WHERE email = '" + Email + "'", con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                return "success";
            }
            else
            {
                return "error";
            }

        }
        

        [Route("logout")]
        public IActionResult Logout()
        {
            if (HttpContext.Session.GetString("email") != null)
            {
                HttpContext.Session.Remove("email");
                HttpContext.Session.Remove("username");
            }
            return View("../Home/Register");
        }
    }
}