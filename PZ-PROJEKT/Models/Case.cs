namespace PZ_PROJEKT.Models
{ 
	public class Case
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string ImagePath { get; set; }
		public float Price { get; set; }
		public List<CaseItem> CaseItems { get; set; }
		public int TotalWeight { get; set; }
		public int TimesOpened { get; set; }
		public User Creator { get; set; }
	}
}
