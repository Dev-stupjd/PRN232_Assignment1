using BussinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface INewsArticleRepository
    {
        List<NewsArticle> GetAllArticle();
        void AddArticle(NewsArticle article);
        void UpdateArticle(NewsArticle article);
        void DeleteArticle(NewsArticle article);
        NewsArticle GetArticleByID(string id);
    }
}
