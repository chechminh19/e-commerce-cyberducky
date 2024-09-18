using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Application.Utils
{
    public class SendMail
    {
        
        public static async Task<bool> SendConfirmationEmail(
            string toEmail,
            string confirmationLink
        )
        {
            var userName = "CyberDuckyShop";
            var emailFrom = "chechminh2001@gmail.com";
            var password = "beos ohye iaah iflf";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(userName, emailFrom));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = "Confirmation your email to login";
            message.Body = new TextPart("html")
            {
                Text =
                    @"
        <html>
            <head>
                <style>
                    body {
                        display: flex;
                        justify-content: center;
                        align-items: center;
                        height: 100vh;
                        margin: 0;
                        font-family: Arial, sans-serif;
                    }
                    .content {
                        text-align: center;
                    }
                    .button {
                        display: inline-block;
                        padding: 10px 20px;
                        background-color: #000;
                        color: #ffffff;
                        text-decoration: none;
                        border-radius: 5px;
                        font-size: 16px;
                    }
                </style>
            </head>
            <body>
                <div class='content'>
                    <p>Please click the button below to confirm your email to register:</p>                    
                      <a class='button' href='"
                    + confirmationLink
                    + "'>Confirm Email</a>"
                    + @"
                </div>
            </body>
        </html>
    "
            };
            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                //authenticate account email
                client.Authenticate(emailFrom, password);

                try
                {
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                    return true;
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }
    }
}
