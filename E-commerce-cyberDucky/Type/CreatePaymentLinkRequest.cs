namespace E_commerce_cyberDucky.Type
{
    public record CreatePaymentLinkRequest
    (
        string productName,
        string description,
        int price,
        string returnUrl,
        string cancelUrl
        );
}
