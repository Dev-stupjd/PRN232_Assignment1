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
            var resp = await _http.GetAsync($"/odata/Category{odataQuery}", ct);
            if (!resp.IsSuccessStatusCode) return new ODataListResponse<CategoryDto> { Value = new() };
            return await resp.Content.ReadFromJsonAsync<ODataListResponse<CategoryDto>>(cancellationToken: ct);
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
            var resp = await _http.GetAsync($"/odata/NewsArticle{odataQuery}", ct);
            if (!resp.IsSuccessStatusCode) return new ODataListResponse<NewsArticleDto> { Value = new() };
            return await resp.Content.ReadFromJsonAsync<ODataListResponse<NewsArticleDto>>(cancellationToken: ct);
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
            var resp = await _http.GetAsync($"/odata/SystemAccount{odataQuery}", ct);
            if (!resp.IsSuccessStatusCode) return null; // let caller surface the failure
            return await resp.Content.ReadFromJsonAsync<ODataListResponse<SystemAccountDto>>(cancellationToken: ct);
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


