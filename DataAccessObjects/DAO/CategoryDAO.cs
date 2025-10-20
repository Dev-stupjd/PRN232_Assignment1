using BussinessObjects;
using BussinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObjects.DAO
{
    public class CategoryDAO
    {
        private FunewsManagementContext _context;

        public CategoryDAO(FunewsManagementContext context)
        {
            _context = context;
        }
        public Category? GetCategoryById(int CategoryId)
        {
            return _context.Categories.FirstOrDefault(u => u.CategoryId == CategoryId);
        }
        public List<Category> GetAllCategorys()
        {
            return _context.Categories.ToList();
        }
        public void AddCategory(Category Category)
        {
            _context.Categories.Add(Category);
            _context.SaveChanges();
        }
        public void UpdateCategory(Category Category)
        {
            // Since we're updating a tracked entity, just save changes
            _context.SaveChanges();
        }
        public void DeleteCategory(int CategoryId)
        {
            var Category = GetCategoryById(CategoryId);
            if (Category != null)
            {
                _context.Categories.Remove(Category);
                _context.SaveChanges();
            }
        }
        public bool AnyNewsInCategory(int categoryId)
        {
            return _context.NewsArticles.Any(n => n.CategoryId == categoryId);
        }
    }
}
