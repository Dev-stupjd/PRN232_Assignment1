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
        public string? SearchQuery { get; private set; }

        public async Task OnGet(string? q = null)
        {
            SearchQuery = q;
            
            // Build OData query for articles
            var articlesQuery = BuildODataQuery(q);
            
            var accountsTask = _accountApi.ODataListAsync("?$select=AccountId,AccountName&$top=200");
            var articlesTask = _newsApi.ODataListAsync(articlesQuery);

            await Task.WhenAll(accountsTask, articlesTask);

            var accounts = accountsTask.Result?.Value ?? new List<SystemAccountDto>();
            var articles = articlesTask.Result?.Value ?? new List<NewsArticleDto>();

            // Debug: Log what we got
            TempData["DebugInfo"] = $"Accounts: {accounts.Count}, Articles: {articles.Count}";
            if (accounts.Count > 0)
            {
                TempData["DebugInfo"] += $" | First Account: {accounts[0].AccountName} (ID: {accounts[0].AccountId})";
            }
            if (articles.Count > 0)
            {
                TempData["DebugInfo"] += $" | First Article CreatedById: {articles[0].CreatedById}";
            }

            var authorById = accounts.ToDictionary(a => a.AccountId, a => a.AccountName ?? "");

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
            
            // If we have a search term but got many results, apply client-side filtering
            if (!string.IsNullOrWhiteSpace(q) && Articles.Count > 10) // Assume if we get more than 10 results, OData filtering didn't work
            {
                var searchTerm = q.Trim().ToLower();
                Articles = Articles.Where(a => 
                    (a.NewsTitle?.ToLower().Contains(searchTerm) == true)
                    ).OrderBy(a => a.NewsTitle).ToList();
                
                TempData["DebugInfo"] += $" | Client-side filtered to {Articles.Count} articles";
            }
            else
            {
                TempData["DebugInfo"] += $" | Mapped Authors: {Articles.Count(a => !string.IsNullOrEmpty(a.AuthorName))}";
            }
        }

        private string BuildODataQuery(string? searchTerm)
        {
            var queryParts = new List<string>();
            
            // Add search filter if provided
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var encodedTerm = Uri.EscapeDataString(searchTerm.Trim());
                // Search in NewsTitle
                var filter = $"$filter=contains(tolower(NewsTitle),tolower('{encodedTerm}'))";
                queryParts.Add(filter);
            }
            
            // Add ordering
            queryParts.Add("$orderby=NewsTitle asc");
            
            // Add top limit for performance
            queryParts.Add("$top=100");
            
            return queryParts.Count > 0 ? "?" + string.Join("&", queryParts) : "?$orderby=NewsTitle asc&$top=100";
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


