using System.Net.Http.Json;
using ApiClient.Models;

namespace ApiClient.Services
{
    public interface ICategoryApi
    {
        Task<ODataListResponse<CategoryDto>?> ODataListAsync(string odataQuery, CancellationToken ct = default);
        Task<HttpResponseMessage> CreateAsync(CategoryDto dto, CancellationToken ct = default);
        Task<HttpResponseMessage> UpdateAsync(CategoryDto dto, CancellationToken ct = default);
        Task<HttpResponseMessage> DeleteAsync(int id, CancellationToken ct = default);
    }

    public interface INewsApi
    {
        Task<ODataListResponse<NewsArticleDto>?> ODataListAsync(string odataQuery, CancellationToken ct = default);
        Task<HttpResponseMessage> CreateAsync(NewsArticleDto dto, CancellationToken ct = default);
        Task<HttpResponseMessage> UpdateAsync(NewsArticleDto dto, CancellationToken ct = default);
        Task<HttpResponseMessage> DeleteAsync(string id, CancellationToken ct = default);
    }

    public interface IAccountApi
    {
        Task<ODataListResponse<SystemAccountDto>?> ODataListAsync(string odataQuery, CancellationToken ct = default);
        Task<HttpResponseMessage> CreateAsync(SystemAccountDto dto, CancellationToken ct = default);
        Task<HttpResponseMessage> UpdateAsync(SystemAccountDto dto, CancellationToken ct = default);
        Task<HttpResponseMessage> DeleteAsync(short id, CancellationToken ct = default);
    }

    public class CategoryApi : ICategoryApi
    {
        private readonly HttpClient _http;
        public CategoryApi(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("Api");
        }

        public async Task<ODataListResponse<CategoryDto>?> ODataListAsync(string odataQuery, CancellationToken ct = default)
        {
            // Try OData endpoint first, fallback to REST API if it fails
            try
            {
                var resp = await _http.GetAsync($"/odata/Category{odataQuery}", ct);
                if (resp.IsSuccessStatusCode)
                {
                    var result = await resp.Content.ReadFromJsonAsync<ODataListResponse<CategoryDto>>(cancellationToken: ct);
                    // Debug: Log that OData worked
                    System.Diagnostics.Debug.WriteLine($"OData SUCCESS: {odataQuery}");
                    return result;
                }
                else
                {
                    // Debug: Log OData failure
                    System.Diagnostics.Debug.WriteLine($"OData FAILED: {resp.StatusCode} - {odataQuery}");
                }
            }
            catch (Exception ex)
            {
                // Debug: Log OData exception
                System.Diagnostics.Debug.WriteLine($"OData EXCEPTION: {ex.Message} - {odataQuery}");
            }
            
            // Fallback to REST API
            System.Diagnostics.Debug.WriteLine($"FALLBACK to REST API: {odataQuery}");
            var restResp = await _http.GetAsync("/api/Category", ct);
            if (!restResp.IsSuccessStatusCode) return new ODataListResponse<CategoryDto> { Value = new() };
            
            var restResponse = await restResp.Content.ReadFromJsonAsync<RestApiResponse<CategoryDto>>(cancellationToken: ct);
            return new ODataListResponse<CategoryDto> { Value = restResponse?.Data ?? new() };
        }

        public Task<HttpResponseMessage> CreateAsync(CategoryDto dto, CancellationToken ct = default)
        {
            return _http.PostAsJsonAsync("/api/Category", dto, ct);
        }

        public Task<HttpResponseMessage> UpdateAsync(CategoryDto dto, CancellationToken ct = default)
        {
            return _http.PutAsJsonAsync("/api/Category", dto, ct);
        }

        public Task<HttpResponseMessage> DeleteAsync(int id, CancellationToken ct = default)
        {
            return _http.DeleteAsync($"/api/Category/{id}", ct);
        }
    }

    public class NewsApi : INewsApi
    {
        private readonly HttpClient _http;
        public NewsApi(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("Api");
        }

        public async Task<ODataListResponse<NewsArticleDto>?> ODataListAsync(string odataQuery, CancellationToken ct = default)
        {
            // Try OData endpoint first, fallback to REST API if it fails
            try
            {
                var resp = await _http.GetAsync($"/odata/NewsArticle{odataQuery}", ct);
                if (resp.IsSuccessStatusCode)
                {
                    var result = await resp.Content.ReadFromJsonAsync<ODataListResponse<NewsArticleDto>>(cancellationToken: ct);
                    System.Diagnostics.Debug.WriteLine($"News OData SUCCESS: {odataQuery}");
                    return result;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"News OData FAILED: {resp.StatusCode} - {odataQuery}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"News OData EXCEPTION: {ex.Message} - {odataQuery}");
            }
            
            // Fallback to REST API
            System.Diagnostics.Debug.WriteLine($"News FALLBACK to REST API: {odataQuery}");
            var restResp = await _http.GetAsync("/api/NewsArticle", ct);
            if (!restResp.IsSuccessStatusCode) return new ODataListResponse<NewsArticleDto> { Value = new() };
            
            var restResponse = await restResp.Content.ReadFromJsonAsync<RestApiResponse<NewsArticleDto>>(cancellationToken: ct);
            return new ODataListResponse<NewsArticleDto> { Value = restResponse?.Data ?? new() };
        }

        public Task<HttpResponseMessage> CreateAsync(NewsArticleDto dto, CancellationToken ct = default)
        {
            return _http.PostAsJsonAsync("/api/NewsArticle", dto, ct);
        }

        public Task<HttpResponseMessage> UpdateAsync(NewsArticleDto dto, CancellationToken ct = default)
        {
            return _http.PutAsJsonAsync("/api/NewsArticle", dto, ct);
        }

        public Task<HttpResponseMessage> DeleteAsync(string id, CancellationToken ct = default)
        {
            return _http.DeleteAsync($"/api/NewsArticle/{id}", ct);
        }
    }

    public class AccountApi : IAccountApi
    {
        private readonly HttpClient _http;
        public AccountApi(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("Api");
        }

        public async Task<ODataListResponse<SystemAccountDto>?> ODataListAsync(string odataQuery, CancellationToken ct = default)
        {
            // Try OData endpoint first, fallback to REST API if it fails
            try
            {
                var resp = await _http.GetAsync($"/odata/SystemAccount{odataQuery}", ct);
                if (resp.IsSuccessStatusCode)
                {
                    var result = await resp.Content.ReadFromJsonAsync<ODataListResponse<SystemAccountDto>>(cancellationToken: ct);
                    System.Diagnostics.Debug.WriteLine($"Account OData SUCCESS: {odataQuery}");
                    return result;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Account OData FAILED: {resp.StatusCode} - {odataQuery}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Account OData EXCEPTION: {ex.Message} - {odataQuery}");
            }
            
            // Fallback to REST API
            System.Diagnostics.Debug.WriteLine($"Account FALLBACK to REST API: {odataQuery}");
            var restResp = await _http.GetAsync("/api/SystemAccount", ct);
            if (!restResp.IsSuccessStatusCode) return new ODataListResponse<SystemAccountDto> { Value = new() };
            
            var restResponse = await restResp.Content.ReadFromJsonAsync<RestApiResponse<SystemAccountDto>>(cancellationToken: ct);
            return new ODataListResponse<SystemAccountDto> { Value = restResponse?.Data ?? new() };
        }

        public Task<HttpResponseMessage> CreateAsync(SystemAccountDto dto, CancellationToken ct = default)
        {
            // Server exposes OData SystemAccount controller
            return _http.PostAsJsonAsync("/odata/SystemAccount", dto, ct);
        }

        public Task<HttpResponseMessage> UpdateAsync(SystemAccountDto dto, CancellationToken ct = default)
        {
            return _http.PutAsJsonAsync($"/odata/SystemAccount({dto.AccountId})", dto, ct);
        }

        public Task<HttpResponseMessage> DeleteAsync(short id, CancellationToken ct = default)
        {
            return _http.DeleteAsync($"/odata/SystemAccount({id})", ct);
        }
    }
}


