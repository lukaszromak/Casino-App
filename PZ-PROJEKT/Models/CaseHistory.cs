using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace PZ_PROJEKT.Models
{
    public class CaseHistory
    {
        public int Id { get; set; }
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public User? User { get; set; }
        public Case Case { get; set; }
        public float Price { get; set; }
        public DateTime OpenedTime {  get; set; } = DateTime.Now;
    }
}
