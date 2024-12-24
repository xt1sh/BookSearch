using Azure.Search.Documents;
using BookSearch.Global.Models;
using BookSearch.Models;

namespace BookSearch.Services
{
	public interface IAzureSearchService
	{
		Task AddBookAsync(Book book);
		Task<List<string>> AutocompleteAsync(string term);
		Task<SearchResult> SearchBooksAsync(string query, string[] category, int page);
	}

	public class AzureSearchService : IAzureSearchService
	{
		private readonly SearchClient _searchClient;

		public AzureSearchService(SearchClient searchClient)
		{
			_searchClient = searchClient;
		}

		public async Task<List<string>> AutocompleteAsync(string term)
		{
			if (term.Length > 50)
			{
				return [];
			}
			var options = new SuggestOptions { UseFuzzyMatching = true, OrderBy = { "search.score() desc" } };
			var response = await _searchClient.SuggestAsync<SearchResult.BookSearchResult>(term, "default_suggester", options);
			return response == null ? [] : response.Value.Results.Select(r => r.Text).ToList();
		}

		public async Task<SearchResult> SearchBooksAsync(string query, string[] categories, int page)
		{
			var options = new SearchOptions
			{
				Filter = null,
				OrderBy = { "search.score() desc" },
				IncludeTotalCount = true,
				Size = SearchResult.PageSize,
				Skip = page * SearchResult.PageSize,
				Facets = { "Categories,count:10" }
			};

			if (categories.Length != 0)
			{
				options.Filter = string.Join(" and ", categories.Select(c => $"Categories/any(c: c eq '{c}')"));
			}

			var response = await _searchClient.SearchAsync<SearchResult.BookSearchResult>(query, options);

			var results = response.Value.GetResults().ToList();

			var books = results.Select(r => r.Document).ToList();

			var searchResult = new SearchResult()
			{
				Books = books,
				TotalCount = (int)response.Value.TotalCount,
				Facets = response?.Value?.Facets["Categories"]?.Select(f => new SearchResult.Facet
				{
					Value = f.Value?.ToString(),
					Count = f.Count.GetValueOrDefault()
				}).ToList()
			};

			return  searchResult;
		}

		public async Task AddBookAsync(Book book)
		{
			book.Id = Guid.NewGuid().ToString();

			await _searchClient.UploadDocumentsAsync([book]);
		}
	}
}
