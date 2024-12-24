using System.ComponentModel.DataAnnotations;

namespace BookSearch.Global.Models
{
	public class Book
	{
		[Key]
		public string Id { get; set; }

		public string Title { get; set; }

		public List<string> Authors { get; set; }

		public string Description { get; set; }

		public List<string> Categories { get; set; }

		public int Year { get; set; }

		public double Rating { get; set; }

		public string CoverImage { get; set; }
	}
}
