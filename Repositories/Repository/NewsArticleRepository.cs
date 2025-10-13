using BussinessObjects.Models;
using DataAccessObjects.DAO;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class NewsArticleRepository : INewsArticleRepository
    {
        private readonly NewsArticleDAO _newsArticleDAO;

        public NewsArticleRepository(NewsArticleDAO NewsArticleDAO)
        {
            _newsArticleDAO = NewsArticleDAO;
        }
        public List<NewsArticle> GetAllArticle() => _newsArticleDAO.GetAllNewsArticles();
        public void AddArticle(NewsArticle NewsArticle) => _newsArticleDAO.AddNewsArticle(NewsArticle);
        public void UpdateArticle(NewsArticle NewsArticle) => _newsArticleDAO.UpdateNewsArticle(NewsArticle);
        public void DeleteArticle(NewsArticle NewsArticle) => _newsArticleDAO.DeleteNewsArticle(NewsArticle.NewsArticleId);
        public NewsArticle GetArticleByID(string id) => _newsArticleDAO.GetNewsArticleById(id);
    }
}
