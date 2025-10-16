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

        public async Task OnGet()
        {
            var list = await _categoryApi.ODataListAsync("?$orderby=CategoryName%20asc");
            if (list?.Value != null)
            {
                Categories = list.Value;
            }
        }

        public async Task<IActionResult> OnPostCreateAsync([FromForm] CategoryDto dto)
        {
            var resp = await _categoryApi.CreateAsync(dto);
            return RedirectToPage();
        }
    }
}


