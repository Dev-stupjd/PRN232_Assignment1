using Microsoft.AspNetCore.Mvc.RazorPages;
using ApiClient.Services;
using ApiClient.Models;

namespace ApiClient.Pages.NewsArticle
{
    public class HistoryModel : PageModel
    {
        private readonly INewsApi _newsApi;

        public HistoryModel(INewsApi newsApi)
        {
            _newsApi = newsApi;
        }

        public List<NewsArticleDto> Articles { get; private set; } = new();

        public async Task OnGet()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (short.TryParse(userId, out var id))
            {
                // Use REST author endpoint for author history
                // GET /api/NewsArticle/author/{authorId}
                var client = HttpContext.RequestServices.GetRequiredService<IHttpClientFactory>().CreateClient("Api");
                var resp = await client.GetAsync($"/api/NewsArticle/author/{id}");
                if (resp.IsSuccessStatusCode)
                {
                    var api = await resp.Content.ReadFromJsonAsync<ApiClient.Models.ApiListResponse<NewsArticleDto>>();
                    Articles = api?.Data ?? new();
                }
            }
        }
    }
}


