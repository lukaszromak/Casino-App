using System.ComponentModel.DataAnnotations;

namespace PZ_PROJEKT.Models
{
	public class User
	{
		public int Id { get; set; }
		public string SteamId { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        public float Balance { get; set; }
	}
}
