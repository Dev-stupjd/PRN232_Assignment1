using BussinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface ICategoryRepository
    {
        List<Category> GetAllCategory();
        void AddCategory(Category category);
        void UpdateCategory(Category category);
        void DeleteCategory(Category category);
        Category GetCategoryByID(int id);
        bool AnyNewsInCategory(int categoryId);
    }
}
