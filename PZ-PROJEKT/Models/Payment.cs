using System.ComponentModel.DataAnnotations.Schema;

namespace PZ_PROJEKT.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public User User { get; set; }
        public float Amount {  get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime PaymentTime { get; set; } = DateTime.Now;
    }
}
