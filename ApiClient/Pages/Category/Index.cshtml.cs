using Microsoft.AspNetCore.Mvc.RazorPages;
using ApiClient.Services;
using ApiClient.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiClient.Pages.Category
{
    public class IndexModel : PageModel
    {
        private readonly ICategoryApi _categoryApi;

        public IndexModel(ICategoryApi categoryApi)
        {
            _categoryApi = categoryApi;
        }

        public List<CategoryDto> Categories { get; private set; } = new();
        public string? SearchQuery { get; private set; }

        public async Task OnGet(string? q = null)
        {
            SearchQuery = q;
            
            // Build OData query
            var odataQuery = BuildODataQuery(q);
            
            // Debug: Log the OData query
            TempData["DebugInfo"] = $"OData Query: {odataQuery}";
            
            var list = await _categoryApi.ODataListAsync(odataQuery);
            if (list?.Value != null)
            {
                // Sort by category name in alphabetical order
                Categories = list.Value.OrderBy(c => c.CategoryName).ToList();
                
                // If we have a search term but got all results, apply client-side filtering
                if (!string.IsNullOrWhiteSpace(q) && Categories.Count > 5) // Assume if we get more than 5 results, OData filtering didn't work
                {
                    var searchTerm = q.Trim().ToLower();
                    Categories = Categories.Where(c => 
                        (c.CategoryName?.ToLower().Contains(searchTerm) == true) ||
                        (c.CategoryDesciption?.ToLower().Contains(searchTerm) == true)
                    ).OrderBy(c => c.CategoryName).ToList();
                    
                    TempData["DebugInfo"] += $" | Client-side filtered to {Categories.Count} categories";
                }
                else
                {
                    TempData["DebugInfo"] += $" | Results: {Categories.Count} categories";
                }
            }
        }

        private string BuildODataQuery(string? searchTerm)
        {
            var queryParts = new List<string>();
            
            // Add search filter if provided
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var encodedTerm = Uri.EscapeDataString(searchTerm.Trim());
                // Search in CategoryName and CategoryDesciption
                var filter = $"$filter=contains(tolower(CategoryName),tolower('{encodedTerm}')) or contains(tolower(CategoryDesciption),tolower('{encodedTerm}'))";
                queryParts.Add(filter);
            }
            
            // Add ordering
            queryParts.Add("$orderby=CategoryName asc");
            
            // Add top limit for performance
            queryParts.Add("$top=100");
            
            return queryParts.Count > 0 ? "?" + string.Join("&", queryParts) : "";
        }

        public async Task<IActionResult> OnPostCreateAsync([FromForm] CategoryDto dto)
        {
            var resp = await _categoryApi.CreateAsync(dto);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync([FromForm] CategoryDto dto)
        {
            try
            {
                // Debug: Log the DTO values
                TempData["DebugInfo"] = $"Edit DTO: ID={dto.CategoryId}, Name='{dto.CategoryName}', Desc='{dto.CategoryDesciption}', Active={dto.IsActive}";
                
                var resp = await _categoryApi.UpdateAsync(dto);
                if (resp.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Category updated successfully!";
                    return RedirectToPage();
                }
                else
                {
                    // Log the error for debugging
                    var errorContent = await resp.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Update failed: {resp.StatusCode} - {errorContent}";
                    return RedirectToPage();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Update error: {ex.Message}";
                return RedirectToPage();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int categoryId)
        {
            var resp = await _categoryApi.DeleteAsync(categoryId);
            return RedirectToPage();
        }
    }
}


