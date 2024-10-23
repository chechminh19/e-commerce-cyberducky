namespace E_commerce_cyberDucky.Type
{
    public record CreatePaymentLinkRequest
    (
       int orderId, string cancelUrl, string successUrl, int userId
        );
}
