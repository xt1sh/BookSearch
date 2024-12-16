using Azure.Search.Documents;
using Azure;
using Microsoft.Extensions.Options;
using BookSearch.Configs;
using BookSearch.Models;

namespace BookSearch.Services
{
	public interface IAzureSearchService
	{
		Task<List<string>> AutocompleteAsync(string term);
		Task<SearchResult> SearchBooksAsync(string query, int page);
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
			var options = new SuggestOptions { UseFuzzyMatching = true, OrderBy = { "search.score() desc" } };
			var response = await _searchClient.SuggestAsync<SearchResult.BookSearchResult>(term, "default_suggester", options);
			return response == null ? [] : response.Value.Results.Select(r => r.Text).ToList();
		}

		public async Task<SearchResult> SearchBooksAsync(string query, int page)
		{
			var options = new SearchOptions
			{
				Filter = null,
				IncludeTotalCount = true,
				Size = SearchResult.PageSize,
				Skip = page * SearchResult.PageSize,
				OrderBy = { "search.score() desc" }
			};
			var response = await _searchClient.SearchAsync<SearchResult.BookSearchResult>(query, options);

			var results = response.Value.GetResults().ToList();

			var books = results.Select(r => r.Document).ToList();

			var searchResult = new SearchResult()
			{
				Books = books,
				TotalCount = (int)response.Value.TotalCount,
			};

			return  searchResult;
		}
	}
}
