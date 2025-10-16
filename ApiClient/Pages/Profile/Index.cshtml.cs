using Microsoft.AspNetCore.Mvc.RazorPages;
using ApiClient.Services;
using ApiClient.Models;

namespace ApiClient.Pages.Profile
{
    public class IndexModel : PageModel
    {
        private readonly IAccountApi _accountApi;

        public IndexModel(IAccountApi accountApi)
        {
            _accountApi = accountApi;
        }

        public SystemAccountDto? Account { get; private set; }

        public async Task OnGet()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (short.TryParse(userId, out var id))
            {
                var query = $"?$filter=AccountId eq {id}&$top=1";
                var list = await _accountApi.ODataListAsync(query);
                Account = list?.Value?.FirstOrDefault();
            }
        }
    }
}


