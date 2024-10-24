using static System.Net.WebRequestMethods;

namespace E_commerce_cyberDucky.Type
{
    public record CreatePaymentLinkRequest
    (
       int orderId, 
       int userId,       
       string cancelUrl = "https://cyberducky.vercel.app/cancel-success",
       string returnUrl = "https://cyberducky.vercel.app/success"
        );
}
