using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;

namespace Project_pearl.Controllers
{
    [Route("UserRegister")]
    public class RegisterController : Controller
    {
        SqlConnection con = new SqlConnection("Data Source=DESKTOP-GS79Q5E;Initial Catalog=db_pearl;Integrated Security=True;Encrypt=False;");
        SqlCommand cmd = new SqlCommand();
        [Route("Register")]
        public string Register(string username, string password, string email)
        {
            if (EmailService.ValidateEmail(email))
            {

                con.Open();
                cmd = new SqlCommand("SELECT * FROM tbl_register WHERE email ='" + email + "';", con);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    con.Close();
                    return "exist";
                }
                else
                {
                    con.Close();
                    password = PasswordHasher.HashedPassword(password);
                    con.Open();
                    cmd = new SqlCommand("insert into tbl_register(username, email, Password,role) values ('" + username + "', '" + email + "', '" + password + "','u');", con);
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        using (SqlCommand cartCmd = new SqlCommand("INSERT INTO tbl_cart (user_id) SELECT user_id FROM tbl_register WHERE email = @Email", con))
                        {
                            cartCmd.Parameters.AddWithValue("@Email", email);
                            cartCmd.ExecuteNonQuery();
                        }
                        con.Close();
                        return "success";
                    }
                    else
                    {
                        return "error";
                    }
                }
            }
            else {
                return "not valid";
            }
        }

        [Route("StaffRegister")]
        public string StaffRegister(string username, string password, string email)
        {
            if (EmailService.ValidateEmail(email))
            {

                con.Open();
                cmd = new SqlCommand("SELECT * FROM tbl_register WHERE email ='" + email + "';", con);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    con.Close();
                    return "exist";
                }
                else
                {
                    con.Close();
                    password = PasswordHasher.HashedPassword(password);
                    con.Open();
                    cmd = new SqlCommand("insert into tbl_register(username, email, Password,role) values ('" + username + "', '" + email + "', '" + password + "','s');", con);
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        using (SqlCommand cartCmd = new SqlCommand("INSERT INTO tbl_cart (user_id) SELECT user_id FROM tbl_register WHERE email = @Email", con))
                        {
                            cartCmd.Parameters.AddWithValue("@Email", email);
                            cartCmd.ExecuteNonQuery();
                        }
                        con.Close();
                        return "success";
                    }
                    else
                    {
                        return "error";
                    }
                }
            }
            else
            {
                return "not valid";
            }
        }
    }
}