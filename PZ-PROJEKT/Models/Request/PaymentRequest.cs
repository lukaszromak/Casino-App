using System.ComponentModel.DataAnnotations;

namespace PZ_PROJEKT.Models.Request
{
    public class PaymentRequest
    {
        public int PaymentMethodId { get; set; }
        [Range(0, 1_000_000)]
        public float Amount { get; set; }
    }
}
