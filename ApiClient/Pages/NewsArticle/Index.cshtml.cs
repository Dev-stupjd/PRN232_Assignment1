using Microsoft.AspNetCore.Mvc.RazorPages;
using ApiClient.Services;
using ApiClient.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiClient.Pages.NewsArticle
{
    public class IndexModel : PageModel
    {
        private readonly INewsApi _newsApi;
        private readonly IAccountApi _accountApi;

        public IndexModel(INewsApi newsApi, IAccountApi accountApi)
        {
            _newsApi = newsApi;
            _accountApi = accountApi;
        }

        public List<NewsArticleVm> Articles { get; private set; } = new();

        public async Task OnGet()
        {
            var accountsTask = _accountApi.ODataListAsync("?$select=AccountId,AccountName&$top=200");
            var articlesTask = _newsApi.ODataListAsync("?$orderby=CreatedDate%20desc");

            await Task.WhenAll(accountsTask, articlesTask);

            var accounts = accountsTask.Result?.Value ?? new List<SystemAccountDto>();
            var articles = articlesTask.Result?.Value ?? new List<NewsArticleDto>();

            var authorById = accounts.ToDictionary(a => a.AccountId, a => a.AccountName ?? "");

            Articles = articles.Select(a => new NewsArticleVm
            {
                NewsArticleId = a.NewsArticleId ?? string.Empty,
                NewsTitle = a.NewsTitle,
                Headline = a.Headline,
                CreatedDate = a.CreatedDate,
                NewsStatus = a.NewsStatus,
                AuthorName = (a.CreatedById.HasValue && authorById.TryGetValue(a.CreatedById.Value, out var name)) ? name : "",
                CreatedById = a.CreatedById
            }).ToList();
        }

        public async Task<IActionResult> OnPostCreateAsync([FromForm] NewsArticleDto dto)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (short.TryParse(userId, out var id))
            {
                dto.CreatedById = id;
            }
            var resp = await _newsApi.CreateAsync(dto);
            return RedirectToPage();
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


