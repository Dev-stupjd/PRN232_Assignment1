using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using ApiClient.Services;
using ApiClient.Models;

namespace ApiClient.Pages.NewsArticle
{
    public class DetailsModel : PageModel
    {
        private readonly INewsApi _newsApi;
        private readonly IAccountApi _accountApi;
        private readonly ICategoryApi _categoryApi;

        public DetailsModel(INewsApi newsApi, IAccountApi accountApi, ICategoryApi categoryApi)
        {
            _newsApi = newsApi;
            _accountApi = accountApi;
            _categoryApi = categoryApi;
        }

        public NewsArticleDto? Article { get; private set; }
        public string? AuthorName { get; private set; }
        public string? CategoryName { get; private set; }
        public string? ReturnUrl { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id, string? returnUrl = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            ReturnUrl = returnUrl;

            try
            {
                // Fetch the specific article using OData filter
                var articleQuery = $"?$filter=NewsArticleId eq '{Uri.EscapeDataString(id)}'";
                var articleResponse = await _newsApi.ODataListAsync(articleQuery);
                
                var articles = articleResponse?.Value ?? new List<NewsArticleDto>();
                Article = articles.FirstOrDefault();

                if (Article == null)
                {
                    return NotFound();
                }

                // Fetch author information if available
                if (Article.CreatedById.HasValue)
                {
                    var authorQuery = $"?$filter=AccountId eq {Article.CreatedById.Value}";
                    var authorResponse = await _accountApi.ODataListAsync(authorQuery);
                    var authors = authorResponse?.Value ?? new List<SystemAccountDto>();
                    var author = authors.FirstOrDefault();
                    AuthorName = author?.AccountName ?? "Unknown";
                }

                // Fetch category information if available
                if (Article.CategoryId.HasValue)
                {
                    var categoryQuery = $"?$filter=CategoryId eq {Article.CategoryId.Value}";
                    var categoryResponse = await _categoryApi.ODataListAsync(categoryQuery);
                    var categories = categoryResponse?.Value ?? new List<CategoryDto>();
                    var category = categories.FirstOrDefault();
                    CategoryName = category?.CategoryName ?? $"Category ID: {Article.CategoryId}";
                }

                return Page();
            }
            catch (Exception ex)
            {
                // Log the exception (in a real application, you'd use proper logging)
                System.Diagnostics.Debug.WriteLine($"Error fetching article details: {ex.Message}");
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostEditAsync([FromForm] NewsArticleDto dto)
        {
            if (string.IsNullOrEmpty(dto.NewsArticleId))
            {
                TempData["ErrorMessage"] = "Article ID is required.";
                return RedirectToPage(new { id = dto.NewsArticleId });
            }

            try
            {
                var userId = HttpContext.Session.GetString("UserId");
                if (short.TryParse(userId, out var id))
                {
                    dto.UpdatedById = id;
                    dto.ModifiedDate = DateTime.UtcNow;
                }

                var response = await _newsApi.UpdateAsync(dto);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Article updated successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update article.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating article: {ex.Message}";
            }

            return RedirectToPage(new { id = dto.NewsArticleId });
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
                    return RedirectToPage("/NewsArticle");
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete article.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting article: {ex.Message}";
            }

            return RedirectToPage(new { id = newsArticleId });
        }
    }
}
