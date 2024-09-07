using System.ComponentModel.DataAnnotations;

namespace PZ_PROJEKT.Models
{
	public class Item
	{
		public int Id { get; set; }
		public string Name { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        public float Price { get; set; }
		public string ImagePath { get; set; }
	}
}
