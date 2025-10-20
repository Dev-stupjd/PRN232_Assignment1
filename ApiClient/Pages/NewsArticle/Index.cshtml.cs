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
        private readonly ICategoryApi _categoryApi;

        public IndexModel(INewsApi newsApi, IAccountApi accountApi, ICategoryApi categoryApi)
        {
            _newsApi = newsApi;
            _accountApi = accountApi;
            _categoryApi = categoryApi;
        }

        public List<NewsArticleVm> Articles { get; private set; } = new();
        public List<CategoryDto> Categories { get; private set; } = new();
        public string? SearchQuery { get; private set; }
        public string SortField { get; private set; } = "title"; // title|created|category
        public string SortDir { get; private set; } = "asc"; // asc|desc

        public async Task OnGet(string? q = null, string? sort = null, string? dir = null)
        {
            SearchQuery = q;
            if (!string.IsNullOrWhiteSpace(sort)) SortField = sort.ToLower();
            if (!string.IsNullOrWhiteSpace(dir)) SortDir = dir.ToLower();
            
            // Build OData query for articles
            var articlesQuery = BuildODataQuery(q, SortField, SortDir);
            
            var accountsTask = _accountApi.ODataListAsync("?$select=AccountId,AccountName&$top=200");
            var categoriesTask = _categoryApi.ODataListAsync("?$select=CategoryId,CategoryName&$top=200");
            var articlesTask = _newsApi.ODataListAsync(articlesQuery);

            await Task.WhenAll(accountsTask, categoriesTask, articlesTask);

            var accounts = accountsTask.Result?.Value ?? new List<SystemAccountDto>();
            Categories = categoriesTask.Result?.Value ?? new List<CategoryDto>();
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
            var categoryById = Categories.ToDictionary(c => c.CategoryId, c => c.CategoryName ?? "");

            Articles = articles.Select(a => new NewsArticleVm
            {
                NewsArticleId = a.NewsArticleId ?? string.Empty,
                NewsTitle = a.NewsTitle,
                Headline = a.Headline,
                CreatedDate = a.CreatedDate,
                NewsContent = a.NewsContent,
                NewsSource = a.NewsSource,
                NewsStatus = a.NewsStatus,
                AuthorName = (a.CreatedById.HasValue && authorById.TryGetValue(a.CreatedById.Value, out var name)) ? name : "Unknown",
                CreatedById = a.CreatedById,
                CategoryId = a.CategoryId,
                CategoryName = (a.CategoryId.HasValue && categoryById.TryGetValue(a.CategoryId.Value, out var cname)) ? cname : null
            }).OrderBy(a => a.NewsTitle).ToList();
            
            // If we have a search term but got many results, apply client-side filtering
            if (!string.IsNullOrWhiteSpace(q) && Articles.Count > 10) // Assume if we get more than 10 results, OData filtering didn't work
            {
                var searchTerm = q.Trim().ToLower();
                Articles = Articles.Where(a => 
                    (a.NewsTitle?.ToLower().Contains(searchTerm) == true)
                    ).ToList();

                // Apply client-side sort if we filtered locally
                Articles = SortArticles(Articles, SortField, SortDir);
                
                TempData["DebugInfo"] += $" | Client-side filtered to {Articles.Count} articles";
            }
            else
            {
                TempData["DebugInfo"] += $" | Mapped Authors: {Articles.Count(a => !string.IsNullOrEmpty(a.AuthorName))}";
            }

            // Always apply client-side sort for display, especially for category name
            Articles = SortArticles(Articles, SortField, SortDir);
        }

        private string BuildODataQuery(string? searchTerm, string sortField, string sortDir)
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
            var dir = sortDir == "desc" ? "desc" : "asc";
            var orderField = sortField switch
            {
                "created" => "CreatedDate",
                "category" => "CategoryId",
                _ => "NewsTitle"
            };
            queryParts.Add($"$orderby={orderField} {dir}");
            
            // Add top limit for performance
            queryParts.Add("$top=100");
            
            return queryParts.Count > 0 ? "?" + string.Join("&", queryParts) : "?$orderby=NewsTitle asc&$top=100";
        }

        private static List<NewsArticleVm> SortArticles(List<NewsArticleVm> items, string sortField, string sortDir)
        {
            var asc = sortDir != "desc";
            return sortField switch
            {
                "created" => (asc ? items.OrderBy(a => a.CreatedDate) : items.OrderByDescending(a => a.CreatedDate)).ToList(),
                "category" => (asc ? items.OrderBy(a => a.CategoryName) : items.OrderByDescending(a => a.CategoryName)).ToList(),
                _ => (asc ? items.OrderBy(a => a.NewsTitle) : items.OrderByDescending(a => a.NewsTitle)).ToList()
            };
        }

        public async Task<IActionResult> OnPostCreateAsync([FromForm] NewsArticleDto dto)
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");
                if (short.TryParse(userId, out var id))
                {
                    dto.CreatedById = id;
                }
                
                // Let server generate ID; ensure we don't exceed DB length
                dto.NewsArticleId = null;
                
                // Validate required fields
                if (string.IsNullOrWhiteSpace(dto.Headline))
                {
                    TempData["ErrorMessage"] = "Headline is required.";
                    return RedirectToPage();
                }
                if (string.IsNullOrWhiteSpace(dto.NewsContent))
                {
                    TempData["ErrorMessage"] = "News content is required.";
                    return RedirectToPage();
                }
                
                var resp = await _newsApi.CreateAsync(dto);
                
                if (resp.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Article created successfully.";
                }
                else
                {
                    var errorContent = await resp.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Failed to create article. Status: {resp.StatusCode}. Error: {errorContent}";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating article: {ex.Message}";
            }
            
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync([FromForm] NewsArticleDto dto)
        {
            if (string.IsNullOrEmpty(dto.NewsArticleId))
            {
                TempData["ErrorMessage"] = "Article ID is required.";
                return RedirectToPage();
            }

            try
            {
                var userId = HttpContext.Session.GetString("UserId");
                if (short.TryParse(userId, out var id))
                {
                    dto.UpdatedById = id;
                    dto.ModifiedDate = DateTime.UtcNow;
                }

                // Validate required fields
                if (string.IsNullOrWhiteSpace(dto.Headline))
                {
                    TempData["ErrorMessage"] = "Headline is required.";
                    return RedirectToPage();
                }
                if (string.IsNullOrWhiteSpace(dto.NewsContent))
                {
                    TempData["ErrorMessage"] = "News content is required.";
                    return RedirectToPage();
                }

                var response = await _newsApi.UpdateAsync(dto);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Article updated successfully.";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Failed to update article. Status: {response.StatusCode}. Error: {errorContent}";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating article: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync([FromForm] string newsArticleId)
        {
            if (string.IsNullOrEmpty(newsArticleId))
            {
                TempData["ErrorMessage"] = "Article ID is required.";
                return RedirectToPage();
            }

            try
            {
                var response = await _newsApi.DeleteAsync(newsArticleId);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Article deleted successfully.";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Failed to delete article. Status: {response.StatusCode}. Error: {errorContent}";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting article: {ex.Message}";
            }

            return RedirectToPage();
        }
    }

    public class NewsArticleVm
    {
        public string NewsArticleId { get; set; } = string.Empty;
        public string? NewsTitle { get; set; }
        public string? Headline { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? NewsContent { get; set; }
        public string? NewsSource { get; set; }
        public bool? NewsStatus { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public short? CreatedById { get; set; }
        public short? CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }
}


