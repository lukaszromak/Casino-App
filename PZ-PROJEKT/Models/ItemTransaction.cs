using PZ_PROJEKT.Models.Request;

namespace PZ_PROJEKT.Models
{
    public class ItemTransaction
    {
        public int Id { get; set; }
        public List<TransactionItem> Items { get; set; }
        public User User { get; set; }
        public TRANSACTION_TYPES TransactionType { get; set; }
        public DateTime TransactionTime { get; set; } = DateTime.Now;
        public float Total;
    }
    public enum TRANSACTION_TYPES {
        SOLD = 0,
        BOUGHT = 1
    }
}
