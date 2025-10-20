using BussinessObjects.Models;
using Repositories.Interface;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class NewsArticleService : INewsArticleService
    {
        private readonly INewsArticleRepository _articles;
        private readonly ICategoryRepository _categories;

        public NewsArticleService(INewsArticleRepository articles, ICategoryRepository categories)
        {
            _articles = articles;
            _categories = categories;
        }

        // Public queries (no authentication required) - only active articles
        public IEnumerable<NewsArticle> GetActiveArticles(string? keyword = null, int page = 1, int pageSize = 20)
        {
            if (page <= 0 || pageSize <= 0)
                throw new ArgumentException("Invalid paging.");
            
            var data = _articles.GetActiveArticles().AsEnumerable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                data = data.Where(a =>
                    (a.NewsTitle?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (a.Headline?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (a.NewsContent?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            return data
                .OrderByDescending(a => a.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public NewsArticle GetActiveArticleById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Id is required.", nameof(id));
            
            var article = _articles.GetArticleByID(id);
            if (article == null || article.NewsStatus != true)
                throw new KeyNotFoundException("Active article not found.");
            
            return article;
        }

        // Staff queries - all articles
        public IEnumerable<NewsArticle> GetAll(string? keyword = null, int page = 1, int pageSize = 20)
        {
            if (page <= 0 || pageSize <= 0)
                throw new ArgumentException("Invalid paging.");
            
            var data = _articles.GetAllArticle().AsEnumerable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                data = data.Where(a =>
                    (a.NewsTitle?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (a.Headline?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (a.NewsContent?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            return data
                .OrderByDescending(a => a.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public NewsArticle GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Id is required.", nameof(id));
            
            var article = _articles.GetArticleByID(id);
            return article ?? throw new KeyNotFoundException("Article not found.");
        }

        public IEnumerable<NewsArticle> GetByAuthor(short authorId, string? keyword = null, int page = 1, int pageSize = 20)
        {
            if (page <= 0 || pageSize <= 0)
                throw new ArgumentException("Invalid paging.");
            
            var data = _articles.GetArticlesByAuthor(authorId).AsEnumerable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                data = data.Where(a =>
                    (a.NewsTitle?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (a.Headline?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (a.NewsContent?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            return data
                .OrderByDescending(a => a.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        // Staff commands
        public NewsArticle Create(NewsArticle article)
        {
            Validate(article, isUpdate: false);

            // Set default values
            article.CreatedDate = DateTime.Now;
            article.NewsStatus = true; // Default to active

            // Generate unique ID if not provided
            if (string.IsNullOrWhiteSpace(article.NewsArticleId))
            {
                article.NewsArticleId = GenerateArticleId();
            }

            // Check if category exists
            if (article.CategoryId.HasValue)
            {
                var category = _categories.GetCategoryByID(article.CategoryId.Value);
                if (category == null)
                    throw new ArgumentException("Category not found.");
            }

            _articles.AddArticle(article);
            return article;
        }

        public NewsArticle Update(NewsArticle article)
        {
            if (article == null)
                throw new ArgumentNullException(nameof(article));
            
            var existing = _articles.GetArticleByID(article.NewsArticleId);
            if (existing == null)
                throw new KeyNotFoundException("Article not found.");

            Validate(article, isUpdate: true);

            // Preserve original creation data
            article.CreatedDate = existing.CreatedDate;
            article.CreatedById = existing.CreatedById;
            article.ModifiedDate = DateTime.Now;

            // Check if category exists
            if (article.CategoryId.HasValue)
            {
                var category = _categories.GetCategoryByID(article.CategoryId.Value);
                if (category == null)
                    throw new ArgumentException("Category not found.");
            }

            _articles.UpdateArticle(article);
            return article;
        }

        public bool Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return false;

            var existing = _articles.GetArticleByID(id);
            if (existing == null)
                return false;

            _articles.DeleteArticle(existing);
            return true;
        }

        // Admin queries for reports
        public IEnumerable<NewsArticle> GetByDateRange(DateTime startDate, DateTime endDate, string? keyword = null, int page = 1, int pageSize = 20)
        {
            if (page <= 0 || pageSize <= 0)
                throw new ArgumentException("Invalid paging.");
            
            var data = _articles.GetArticlesByDateRange(startDate, endDate).AsEnumerable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                data = data.Where(a =>
                    (a.NewsTitle?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (a.Headline?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (a.NewsContent?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            return data
                .OrderByDescending(a => a.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        // --- helpers ---
        private static void Validate(NewsArticle article, bool isUpdate)
        {
            if (string.IsNullOrWhiteSpace(article.Headline))
                throw new ArgumentException("Headline is required.");
            if (string.IsNullOrWhiteSpace(article.NewsContent))
                throw new ArgumentException("News content is required.");
            // add more rules as needed
        }

        private static string GenerateArticleId()
        {
            // Must fit nvarchar(20): 3 (ART) + 14 (yyyyMMddHHmmss) + 3 (random) = 20
            var id = "ART" + DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(100, 999);
            return id;
        }
    }
}
