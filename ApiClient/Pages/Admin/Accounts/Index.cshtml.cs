using Microsoft.AspNetCore.Mvc.RazorPages;
using ApiClient.Services;
using ApiClient.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiClient.Pages.Admin.Accounts
{
    public class IndexModel : PageModel
    {
        private readonly IAccountApi _accountApi;
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IAccountApi accountApi, IHttpClientFactory httpClientFactory)
        {
            _accountApi = accountApi;
            _httpClientFactory = httpClientFactory;
        }

        public List<SystemAccountDto> Accounts { get; private set; } = new();
        public string? ErrorMessage { get; private set; }

        public async Task OnGet()
        {
            var res = await _accountApi.ODataListAsync("?$orderby=AccountId%20asc&$top=100");
            if (res == null)
            {
                // Try to fetch raw error details to surface the real cause
                var client = _httpClientFactory.CreateClient("Api");
                var url = "/odata/SystemAccount?$orderby=AccountId%20asc&$top=100";
                try
                {
                    var resp = await client.GetAsync(url);
                    var body = await resp.Content.ReadAsStringAsync();
                    ErrorMessage = $"Accounts request failed: {(int)resp.StatusCode} {resp.ReasonPhrase} - {body}";
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Accounts request error: {ex.Message}";
                }
                Accounts = new();
                return;
            }
            Accounts = res.Value;
        }

        public async Task<IActionResult> OnPostCreateAsync([FromForm] SystemAccountDto dto)
        {
            // Fetch current max AccountId and set next
            try
            {
                var client = _httpClientFactory.CreateClient("Api");
                var respMax = await client.GetAsync("/odata/SystemAccount?$orderby=AccountId%20desc&$top=1");
                if (respMax.IsSuccessStatusCode)
                {
                    var data = await respMax.Content.ReadFromJsonAsync<ODataListResponse<SystemAccountDto>>();
                    var maxId = data?.Value?.FirstOrDefault()?.AccountId ?? 0;
                    dto.AccountId = (short)(maxId + 1);
                }
            }
            catch
            {
                // ignore; server will still assign if not provided
            }
            var resp = await _accountApi.CreateAsync(dto);
            if (!resp.IsSuccessStatusCode)
            {
                ErrorMessage = await resp.Content.ReadAsStringAsync();
                await OnGet(); // reload list to show existing accounts alongside the error
                return Page();
            }
            return RedirectToPage();
        }

        // Admin only creates accounts per requirement; no update/delete
    }
}


