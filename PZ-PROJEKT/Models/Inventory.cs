using System.ComponentModel.DataAnnotations;

namespace PZ_PROJEKT.Models
{
    public class Inventory
    {
        public int Id { get; set; }
        public User User { get; set; }
        public Item Item { get; set; }
        public int Count { get; set; }
    }
}
