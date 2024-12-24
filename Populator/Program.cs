using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using BookSearch.Global.Models;
using CsvHelper;
using System.Globalization;

const string endpoint = "https://mp41-search-systems.search.windows.net";
const string indexName = "books";
const string apiKey = "";

using var reader = new StreamReader(@"C:\Users\azyla\Downloads\books_1.Best_Books_Ever.csv");
using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

var records = new List<Book>();
csv.Read();
csv.ReadHeader();

var id = 1;

while (csv.Read())
{
	if (!int.TryParse(csv.GetField(14)?.Split(' ').Last(), out var year) && !int.TryParse(csv.GetField(14)?.Split('/').Last(), out year))
	{
		continue;
	}

	if (year < 100)
	{
		year += year > 24 ? 1900 : 2000;
	}

	var record = new Book
	{
		Id = $"{id++}",
		Title = csv.GetField(1),
		Authors = csv.GetField(3)?.Split(',').ToList() ?? [],
		Description = csv.GetField(5),
		Rating = csv.GetField<double>(4),
		Categories = csv.GetField(8)?.Replace("[", "").Replace("]", "").Replace("'", "").Split(',').Select(x => x.Trim()).ToList() ?? [],
		Year = year,
		CoverImage = csv.GetField(21),
	};

	records.Add(record);
}

var serviceClient = new SearchClient(
			new Uri(endpoint),
			indexName,
			new AzureKeyCredential(apiKey)
		);

var bulk = new List<Book>();

for (int i = 0; i < records.Count; i++)
{
	bulk.Add(records[i]);

	if (i != 0 && i % 1000 == 0)
	{
		var batch = IndexDocumentsBatch.Upload(bulk);
		var result = await serviceClient.IndexDocumentsAsync(batch);

		bulk.Clear();
	}
}

if (bulk.Count > 0)
{
	var batch = IndexDocumentsBatch.Upload(bulk);
	var result = await serviceClient.IndexDocumentsAsync(batch);
}

Console.ReadLine();
