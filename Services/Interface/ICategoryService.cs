using BussinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface ICategoryService
    {
        // Queries
        IEnumerable<Category> GetAll(string? keyword = null, int page = 1, int pageSize = 20);
        Category GetById(int id);

        // Commands
        Category Create(Category category);
        Category Update(Category category);
        bool Delete(int id); // returns false if blocked by business rules
    }
}
