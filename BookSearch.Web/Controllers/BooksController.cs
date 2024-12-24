using BookSearch.Global.Models;
using BookSearch.Models;
using BookSearch.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookSearch.Controllers
{
	public class BooksController : Controller
	{
		private readonly IAzureSearchService _searchService;

		public BooksController(IAzureSearchService searchService)
		{
			_searchService = searchService;
		}

		[HttpGet]
		public async Task<JsonResult> Autocomplete(string term)
		{
			if (string.IsNullOrWhiteSpace(term))
			{
				return Json(new List<string>());
			}

			var suggestions = await _searchService.AutocompleteAsync(term);
			return Json(suggestions);
		}

		[HttpGet]
		public async Task<IActionResult> Search(string query, string category, int page)
		{
			if (string.IsNullOrWhiteSpace(query))
			{
				return View();
			}

			var categories = category?.Split(',') ?? [];

			var results = await _searchService.SearchBooksAsync(query, categories, page);

			results.Query = query;
			results.CurrentPage = page;
			results.Categories = categories.ToList();

			return View(results);
		}

		[HttpPost]
		public async Task<IActionResult> AddBook(Book book)
		{
			await _searchService.AddBookAsync(book);

			return RedirectToAction(nameof(AddBook));
		}

		[HttpGet]
		public async Task<IActionResult> AddBook()
		{
			return View();
		}
	}
}
