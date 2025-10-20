using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using ApiClient.Models;
using ApiClient.Services;

namespace ApiClient.Pages.Admin.Reports
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _factory;
        private readonly IAccountApi _accountApi;

        public IndexModel(IHttpClientFactory factory, IAccountApi accountApi)
        {
            _factory = factory;
            _accountApi = accountApi;
        }

        [BindProperty]
        public DateTime StartDate { get; set; } = DateTime.UtcNow.AddDays(-7);
        [BindProperty]
        public DateTime EndDate { get; set; } = DateTime.UtcNow;

        public List<NewsArticleVm> Articles { get; private set; } = new();
        public string? ErrorMessage { get; private set; }

        public void OnGet()
        {
        }

        public async Task OnPostAsync()
        {
            var client = _factory.CreateClient("Api");

            // Validate input dates
            if (EndDate < StartDate)
            {
                ErrorMessage = "End date must be greater than or equal to start date.";
                return;
            }

            // Normalize to UTC and format as ISO-8601 with 'Z' (OData v4 DateTimeOffset literal)
            var startUtc = DateTime.SpecifyKind(StartDate, DateTimeKind.Local).ToUniversalTime();
            var endUtc = DateTime.SpecifyKind(EndDate, DateTimeKind.Local).ToUniversalTime();

            // Make end inclusive to the second to avoid missing items created within the same minute
            if (endUtc.Second == 0 && endUtc.Millisecond == 0)
            {
                endUtc = endUtc.AddSeconds(59);
            }

            string startIso = startUtc.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");
            string endIso = endUtc.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");

            // Build OData query: CreatedDate ge 2024-12-01T00:00:00Z and CreatedDate le 2024-12-31T23:59:59Z
            var filterExpr = $"CreatedDate ge {startIso} and CreatedDate le {endIso}";
            var odata = $"/odata/NewsArticle?$filter={Uri.EscapeDataString(filterExpr)}&$orderby=NewsTitle%20asc";

            try
            {
                var resp = await client.GetAsync(odata);
                if (!resp.IsSuccessStatusCode)
                {
                    var content = await resp.Content.ReadAsStringAsync();
                    ErrorMessage = $"Report query failed ({(int)resp.StatusCode} {resp.StatusCode}). URL: {odata}. Server says: {content}";
                    return;
                }

                var data = await resp.Content.ReadFromJsonAsync<ODataListResponse<NewsArticleDto>>();
                var articles = data?.Value ?? new List<NewsArticleDto>();

                // Fetch author information
                var accountsTask = _accountApi.ODataListAsync("?$select=AccountId,AccountName&$top=200");
                await accountsTask;
                var accounts = accountsTask.Result?.Value ?? new List<SystemAccountDto>();
                var authorById = accounts.ToDictionary(a => a.AccountId, a => a.AccountName ?? "");

                // Create view models with author names
                Articles = articles.Select(a => new NewsArticleVm
                {
                    NewsArticleId = a.NewsArticleId ?? string.Empty,
                    NewsTitle = a.NewsTitle,
                    Headline = a.Headline,
                    CreatedDate = a.CreatedDate,
                    NewsStatus = a.NewsStatus,
                    AuthorName = (a.CreatedById.HasValue && authorById.TryGetValue(a.CreatedById.Value, out var name)) ? name : "Unknown",
                    CreatedById = a.CreatedById
                }).OrderBy(a => a.NewsTitle).ToList();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Unexpected error calling API. URL: {odata}. Error: {ex.Message}";
            }
        }
    }

    public class NewsArticleVm
    {
        public string NewsArticleId { get; set; } = string.Empty;
        public string? NewsTitle { get; set; }
        public string? Headline { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? NewsStatus { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public short? CreatedById { get; set; }
    }
}


