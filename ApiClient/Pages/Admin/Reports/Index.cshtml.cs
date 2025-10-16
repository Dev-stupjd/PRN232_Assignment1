using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using ApiClient.Models;

namespace ApiClient.Pages.Admin.Reports
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _factory;

        public IndexModel(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        [BindProperty]
        public DateTime StartDate { get; set; } = DateTime.UtcNow.AddDays(-7);
        [BindProperty]
        public DateTime EndDate { get; set; } = DateTime.UtcNow;

        public List<NewsArticleDto> Articles { get; private set; } = new();
        public string? ErrorMessage { get; private set; }

        public void OnGet()
        {
        }

        public async Task OnPostAsync()
        {
            var client = _factory.CreateClient("Api");
            // Normalize to UTC and use ISO-8601 with 'Z' to satisfy OData datetime literal expectations
            var startUtc = DateTime.SpecifyKind(StartDate, DateTimeKind.Local).ToUniversalTime();
            var endUtc = DateTime.SpecifyKind(EndDate, DateTimeKind.Local).ToUniversalTime();
            var filterRaw = $"CreatedDate ge {startUtc:o} and CreatedDate le {endUtc:o}";
            var filter = Uri.EscapeDataString(filterRaw);
            var odata = $"/odata/NewsArticle?$filter={filter}&$orderby=CreatedDate%20desc";
            var resp = await client.GetAsync(odata);
            if (!resp.IsSuccessStatusCode)
            {
                ErrorMessage = await resp.Content.ReadAsStringAsync();
                return;
            }
            var data = await resp.Content.ReadFromJsonAsync<ODataListResponse<NewsArticleDto>>();
            Articles = data?.Value ?? new();
        }
    }
}


