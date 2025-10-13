using BussinessObjects;
using BussinessObjects.Models;
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
            return _context.NewsArticles.FirstOrDefault(u => u.NewsArticleId == NewsArticleId);
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
            _context.Entry<NewsArticle>(NewsArticle).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
        }
        public void DeleteNewsArticle(string NewsArticleId)
        {
            var NewsArticle = GetNewsArticleById(NewsArticleId);
            if (NewsArticle != null)
            {
                _context.NewsArticles.Remove(NewsArticle);
                _context.SaveChanges();
            }
        }
    }
}
