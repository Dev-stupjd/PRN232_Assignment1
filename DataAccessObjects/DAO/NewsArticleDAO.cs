using BussinessObjects;
using BussinessObjects.Models;
using Microsoft.EntityFrameworkCore; // For AsNoTracking, Include, ExecuteSqlRaw, transactions
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObjects.DAO
{
    public class NewsArticleDAO
    {
        private FunewsManagementContext _context;

        public NewsArticleDAO(FunewsManagementContext context)
        {
            _context = context;
        }
        public NewsArticle? GetNewsArticleById(string NewsArticleId)
        {
            // Use AsNoTracking to avoid tracking conflicts when the returned entity
            // is only needed for read/comparison purposes in an update flow
            return _context.NewsArticles
                .AsNoTracking()
                .FirstOrDefault(u => u.NewsArticleId == NewsArticleId);
        }
        public List<NewsArticle> GetAllNewsArticles()
        {
            return _context.NewsArticles.ToList();
        }
        public void AddNewsArticle(NewsArticle NewsArticle)
        {
            _context.NewsArticles.Add(NewsArticle);
            _context.SaveChanges();
        }
        public void UpdateNewsArticle(NewsArticle NewsArticle)
        {
            // If an instance with the same key is already tracked, detach it to avoid
            // the EF Core tracking conflict when attaching the incoming instance
            var local = _context.NewsArticles.Local.FirstOrDefault(e => e.NewsArticleId == NewsArticle.NewsArticleId);
            if (local != null)
            {
                _context.Entry(local).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }

            _context.Attach(NewsArticle);
            _context.Entry(NewsArticle).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
        }
        public void DeleteNewsArticle(string NewsArticleId)
        {
            using var tx = _context.Database.BeginTransaction();

            // 1) Hard delete all join rows first to satisfy FK
            _context.Database.ExecuteSqlRaw(
                "DELETE FROM [dbo].[NewsTag] WHERE [NewsArticleID] = {0}", NewsArticleId);

            // 2) Ensure no tracked instances linger
            var local = _context.NewsArticles.Local.FirstOrDefault(e => e.NewsArticleId == NewsArticleId);
            if (local != null)
            {
                _context.Entry(local).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }

            // 3) Attach a stub entity and delete without extra roundtrip
            var stub = new NewsArticle { NewsArticleId = NewsArticleId };
            _context.Attach(stub);
            _context.NewsArticles.Remove(stub);
            _context.SaveChanges();

            tx.Commit();
        }
        public List<NewsArticle> GetActiveArticles()
        {
            return _context.NewsArticles.Where(n => n.NewsStatus == true).ToList();
        }
        public List<NewsArticle> GetArticlesByAuthor(short authorId)
        {
            return _context.NewsArticles.Where(n => n.CreatedById == authorId).ToList();
        }
        public List<NewsArticle> GetArticlesByDateRange(DateTime startDate, DateTime endDate)
        {
            return _context.NewsArticles
                .Where(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate)
                .OrderByDescending(n => n.CreatedDate)
                .ToList();
        }
    }
}
