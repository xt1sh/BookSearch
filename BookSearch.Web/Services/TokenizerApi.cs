namespace BookSearch.Services
{
	public class TokenizerApi
	{
		private readonly HttpClient _httpClient;

		public TokenizerApi(IHttpClientFactory httpClientFactory)
		{
			_httpClient = httpClientFactory.CreateClient();
		}
	}
}
