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
    public class ReportService : IReportService
    {
        private readonly INewsArticleRepository _articles;
        private readonly ICategoryRepository _categories;
        private readonly IAccountRepository _accounts;

        public ReportService(INewsArticleRepository articles, ICategoryRepository categories, IAccountRepository accounts)
        {
            _articles = articles;
            _categories = categories;
            _accounts = accounts;
        }

        // Get news statistics by date range (sorted in descending order by created date)
        public IEnumerable<NewsArticle> GetNewsStatisticsByDateRange(DateTime startDate, DateTime endDate, string? keyword = null, int page = 1, int pageSize = 20)
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

        // Get category statistics
        public IEnumerable<object> GetCategoryStatistics(DateTime startDate, DateTime endDate)
        {
            var articles = _articles.GetArticlesByDateRange(startDate, endDate);
            var categories = _categories.GetAllCategory();

            return articles
                .Where(a => a.CategoryId.HasValue)
                .GroupBy(a => a.CategoryId)
                .Select(g => new
                {
                    CategoryId = g.Key,
                    CategoryName = categories.FirstOrDefault(c => c.CategoryId == g.Key)?.CategoryName ?? "Unknown",
                    ArticleCount = g.Count(),
                    ActiveArticles = g.Count(a => a.NewsStatus == true),
                    InactiveArticles = g.Count(a => a.NewsStatus == false)
                })
                .OrderByDescending(x => x.ArticleCount)
                .ToList();
        }

        // Get author statistics
        public IEnumerable<object> GetAuthorStatistics(DateTime startDate, DateTime endDate)
        {
            var articles = _articles.GetArticlesByDateRange(startDate, endDate);
            var accounts = _accounts.GetAllAccount();

            return articles
                .Where(a => a.CreatedById.HasValue)
                .GroupBy(a => a.CreatedById)
                .Select(g => new
                {
                    AuthorId = g.Key,
                    AuthorName = accounts.FirstOrDefault(a => a.AccountId == g.Key)?.AccountName ?? "Unknown",
                    AuthorEmail = accounts.FirstOrDefault(a => a.AccountId == g.Key)?.AccountEmail ?? "Unknown",
                    ArticleCount = g.Count(),
                    ActiveArticles = g.Count(a => a.NewsStatus == true),
                    InactiveArticles = g.Count(a => a.NewsStatus == false)
                })
                .OrderByDescending(x => x.ArticleCount)
                .ToList();
        }

        // Get overall statistics
        public object GetOverallStatistics(DateTime startDate, DateTime endDate)
        {
            var articles = _articles.GetArticlesByDateRange(startDate, endDate);
            var totalArticles = articles.Count();
            var activeArticles = articles.Count(a => a.NewsStatus == true);
            var inactiveArticles = articles.Count(a => a.NewsStatus == false);

            return new
            {
                DateRange = new { StartDate = startDate, EndDate = endDate },
                TotalArticles = totalArticles,
                ActiveArticles = activeArticles,
                InactiveArticles = inactiveArticles,
                ActivePercentage = totalArticles > 0 ? Math.Round((double)activeArticles / totalArticles * 100, 2) : 0,
                UniqueAuthors = articles.Where(a => a.CreatedById.HasValue).Select(a => a.CreatedById).Distinct().Count(),
                UniqueCategories = articles.Where(a => a.CategoryId.HasValue).Select(a => a.CategoryId).Distinct().Count(),
                AverageArticlesPerDay = totalArticles > 0 ? Math.Round((double)totalArticles / (endDate - startDate).TotalDays, 2) : 0
            };
        }
    }
}
