namespace BookSearch.Models
{
	public class SearchResult
	{
		public const int PageSize = 20;

		public List<BookSearchResult> Books { get; set; }

		public int TotalCount { get; set; }

		public int CurrentPage { get; set; }

		public string Query { get; set; }

		public List<Facet> Facets { get; set; }

		public List<string> Categories { get; set; } = [];

		public int Count => Books.Count;

		public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
		public bool HasPreviousPage => CurrentPage > 1;
		public bool HasNextPage => CurrentPage < TotalPages;

		public class BookSearchResult
		{
			public string Title { get; set; }
			public List<string> Authors { get; set; }
			public string Description { get; set; }
			public List<string> Categories { get; set; }
			public int? Year { get; set; }
			public double Rating { get; set; }
			public string CoverImage { get; set; }
		}

		public class Facet
		{
			public string Value { get; set; }
			public long Count { get; set; }
		}
	}
}
