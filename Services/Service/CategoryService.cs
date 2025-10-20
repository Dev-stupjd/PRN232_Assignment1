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
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categories;

        public CategoryService(ICategoryRepository categories)
        {
            _categories = categories;
        }

        // READ ALL with simple search + paging
        public IEnumerable<Category> GetAll(string? keyword = null, int page = 1, int pageSize = 20)
        {
            if (page <= 0 || pageSize <= 0)
                throw new ArgumentException("Invalid paging.");
            var data = _categories.GetAllCategory().AsEnumerable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                data = data.Where(c =>
                    (c.CategoryName?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (c.CategoryDesciption?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            return data
                .OrderByDescending(c => c.CategoryId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public Category GetById(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be positive.", nameof(id));
            var category = _categories.GetCategoryByID(id);
            return category ?? throw new KeyNotFoundException("Category not found.");
        }

        // CREATE with basic validation
        public Category Create(Category category)
        {
            Validate(category, isUpdate: false);

            // (optional) enforce unique category name
            var exists = _categories.GetAllCategory()
                              .Any(c => c.CategoryName!.Equals(category.CategoryName, StringComparison.OrdinalIgnoreCase));
            if (exists)
                throw new InvalidOperationException("Category name already exists.");

            _categories.AddCategory(category);
            return category;
        }

        // UPDATE with basic validation
        public Category Update(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));
            
            // Check if category exists without tracking
            var existingCategory = _categories.GetCategoryByID(category.CategoryId);
            if (existingCategory is null)
                throw new KeyNotFoundException("Category not found.");

            Validate(category, isUpdate: true);

            // Update the existing tracked entity instead of passing the new one
            existingCategory.CategoryName = category.CategoryName;
            existingCategory.CategoryDesciption = category.CategoryDesciption;
            existingCategory.ParentCategoryId = category.ParentCategoryId;
            existingCategory.IsActive = category.IsActive;

            _categories.UpdateCategory(existingCategory);
            return existingCategory;
        }

        // DELETE respecting assignment rule: cannot delete if category has news articles
        public bool Delete(int id)
        {
            var existing = _categories.GetCategoryByID(id);
            if (existing is null)
                return false;

            // Assignment rule: block deletion if the category has news articles
            if (_categories.AnyNewsInCategory(id))
                return false;

            _categories.DeleteCategory(existing);
            return true;
        }

        // --- helpers ---
        private static void Validate(Category category, bool isUpdate)
        {
            if (string.IsNullOrWhiteSpace(category.CategoryName))
                throw new ArgumentException("Category name is required.");
            if (string.IsNullOrWhiteSpace(category.CategoryDesciption))
                throw new ArgumentException("Category description is required.");
            // add more rules as needed
        }
    }
}
