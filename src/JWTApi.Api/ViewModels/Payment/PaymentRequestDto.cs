namespace JWTApi.Api.ViewModels.Payment
{
    public class PaymentRequestDto
    {
        public int Amount { get; set; } // تومان
        public string Description { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
    }
}
