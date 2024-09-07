namespace PZ_PROJEKT.Models.Request
{
    public class TransactionItem
    {
        public int Id { get; set; }
        public Item Item { get; set; }
        public int Count { get; set; }
    }
}
