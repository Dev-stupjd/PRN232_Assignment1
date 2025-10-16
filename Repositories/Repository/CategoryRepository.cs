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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CategoryDAO _categoryDAO;

        public CategoryRepository(CategoryDAO categoryDAO)
        {
            _categoryDAO = categoryDAO;
        }
        public List<Category> GetAllCategory() => _categoryDAO.GetAllCategorys();
        public void AddCategory(Category category) => _categoryDAO.AddCategory(category);
        public void UpdateCategory(Category category) => _categoryDAO.UpdateCategory(category);
        public void DeleteCategory(Category category) => _categoryDAO.DeleteCategory(category.CategoryId);
        public Category GetCategoryByID(int id) => _categoryDAO.GetCategoryById(id);
        public bool AnyNewsInCategory(int categoryId) => _categoryDAO.AnyNewsInCategory(categoryId);
    }
}
