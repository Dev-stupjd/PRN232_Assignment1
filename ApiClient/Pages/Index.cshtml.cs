using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ApiClient.Services;
using ApiClient.Models;

namespace ApiClient.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IAccountApi _accountApi;

        public IndexModel(ILogger<IndexModel> logger, IAccountApi accountApi)
        {
            _logger = logger;
            _accountApi = accountApi;
        }

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string? ErrorMessage { get; private set; }

        public IActionResult OnGet()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToPage("/Profile/Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Email and Password are required.";
                return Page();
            }

            var safeEmail = Email.Replace("'", "''");
            var safePassword = Password.Replace("'", "''");
            var filter = $"AccountEmail eq '{safeEmail}' and AccountPassword eq '{safePassword}'";
            var query = $"?$filter={Uri.EscapeDataString(filter)}&$top=1";

            var result = await _accountApi.ODataListAsync(query);
            var account = result?.Value?.FirstOrDefault();
            if (account == null)
            {
                ErrorMessage = "Invalid email or password.";
                return Page();
            }

            HttpContext.Session.SetString("UserId", account.AccountId.ToString());
            HttpContext.Session.SetString("UserName", account.AccountName ?? string.Empty);
            HttpContext.Session.SetString("UserEmail", account.AccountEmail ?? string.Empty);
            HttpContext.Session.SetString("UserRole", (account.AccountRole ?? 0).ToString());

            return RedirectToPage("/Profile/Index");
        }
    }
}
