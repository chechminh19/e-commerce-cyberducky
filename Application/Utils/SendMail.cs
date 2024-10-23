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
using Application.ViewModels;

namespace Application.Utils
{
    public class SendMail
    {

        public static async Task<bool> SendOrderPaymentSuccessEmail(ShowOrderSuccessEmailDTO orderEmailDto, string toEmail)
        {
            var userName = "ZodiacJewerly";
            var emailFrom = "chechminh2001@gmail.com";
            var password = "beos ohye iaah iflf";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(userName, emailFrom));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = "Order Payment Successful";

            var orderItemsHtml = string.Join("", orderEmailDto.OrderItems.Select(item => $@"
        <tr>
            <td>{item.ProductName}</td>
            <td>{item.Quantity}</td>
            <td>{item.Price:C}</td>
            <td>{item.TotalPrice:C}</td>
        </tr>
    "));

            message.Body = new TextPart("html")
            {
                Text = $@"
<html>
    <head>
        <style>
            body {{
                font-family: Arial, sans-serif;
                margin: 0;
                padding: 0;
                display: flex;
                justify-content: center;
                align-items: center;
                height: 100vh;
            }}
            .container {{
                width: 80%;
                margin: auto;
            }}
            .content {{
                text-align: center;
            }}
            table {{
                width: 100%;
                border-collapse: collapse;
            }}
            th, td {{
                padding: 10px;
                border: 1px solid #ddd;
                text-align: left;
            }}
            th {{
                background-color: #f2f2f2;
            }}
        </style>
    </head>
    <body>
        <div class='container'>
            <div class='content'>
                <h1>Thank you for your purchase, {orderEmailDto.UserName}!</h1>
                <p>Your payment for order ID {orderEmailDto.OrderId} has been confirmed successfully on {orderEmailDto.PaymentDate:MMMM dd, yyyy}.</p>
                <h2>Order Details</h2>
                <table>
                    <thead>
                        <tr>
                            <th>Product Name</th>
                            <th>Quantity</th>
                            <th>Price</th>
                            <th>Total</th>
                        </tr>
                    </thead>
                    <tbody>
                        {orderItemsHtml}
                    </tbody>
                    <tfoot>
                        <tr>
                            <td colspan='3' style='text-align:right'><strong>Total Price:</strong></td>
                            <td>{orderEmailDto.TotalPrice:C}</td>
                        </tr>
                    </tfoot>
                </table>
            </div>
        </div>
    </body>
</html>"
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
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
