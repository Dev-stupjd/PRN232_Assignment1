using BussinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface INewsArticleService
    {
        // Public queries (no authentication required)
        IEnumerable<NewsArticle> GetActiveArticles(string? keyword = null, int page = 1, int pageSize = 20);
        NewsArticle GetActiveArticleById(string id);

        // Staff queries
        IEnumerable<NewsArticle> GetAll(string? keyword = null, int page = 1, int pageSize = 20);
        NewsArticle GetById(string id);
        IEnumerable<NewsArticle> GetByAuthor(short authorId, string? keyword = null, int page = 1, int pageSize = 20);

        // Staff commands
        NewsArticle Create(NewsArticle article);
        NewsArticle Update(NewsArticle article);
        bool Delete(string id);

        // Admin queries for reports
        IEnumerable<NewsArticle> GetByDateRange(DateTime startDate, DateTime endDate, string? keyword = null, int page = 1, int pageSize = 20);
    }
}
