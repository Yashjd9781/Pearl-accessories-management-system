using Microsoft.AspNetCore.Components;
using System;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Project_pearl.Controllers
{
    public class EmailService
    {
        public static bool SendOTP(string email, string otp)
        {
            try
            {
                // Configure the SMTP client
                using (SmtpClient client = new SmtpClient("smtp.gmail.com"))
                {
                    client.Port = 587; // Update the port if required
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential("21bmiit074@gmail.com", "vpblcqldldrjkyxl");
                    client.EnableSsl = true;

                    // Create the email message
                    using (MailMessage message = new MailMessage())
                    {
                        message.From = new MailAddress("21bmiit074@gmail.com");
                        message.To.Add(email);
                        message.Subject = "One-Time Password (OTP)";

                        // HTML body for the email
                        string htmlBody = $@"
                            <html>
                            <head>
                                <style>
                                    .container {{
                                        padding: 20px;
                                        border: 1px solid #ddd;
                                        border-radius: 5px;
                                        background-color: #f9f9f9;
                                        font-family: Arial, sans-serif;
                                    }}
                                    .otp {{
                                        font-size: 20px;
                                        font-weight: bold;
                                        color: #007bff;
                                    }}
                                </style>
                            </head>
                            <body>
                                <div class='container'>
                                    <p>Dear User,</p>
                                    <p>Your One-Time Password (OTP) is:</p>
                                    <p class='otp'>{otp}</p>
                                    <p>Please use this OTP to proceed with your action.</p>
                                    <p>Regards,<br/>GameZone Team</p>
                                </div>
                            </body>
                            </html>
                        ";
                        message.Body = htmlBody;
                        message.IsBodyHtml = true;

                        // Send the email
                        client.Send(message);
                    }
                }

                // Email sent successfully
                return true;
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine("Error sending email: " + ex.Message);
                // Email sending failed
                return false;
            }
        }

        public static string GenerateRandomOTP(int iOTPLength)
        {
            string[] saAllowedCharacters = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
            string sOTP = String.Empty;
            Random rand = new Random();
            for (int i = 0; i < iOTPLength; i++)
            {
                sOTP += saAllowedCharacters[rand.Next(0, saAllowedCharacters.Length)];
            }
            return sOTP;
        }

        public static bool ValidateEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            return Regex.IsMatch(email, pattern);
        }
    }

}