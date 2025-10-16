using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using BussinessObjects.Models;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public IActionResult GetAllCategories([FromQuery] string? keyword = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var categories = _categoryService.GetAll(keyword, page, pageSize);
                return Ok(new { success = true, data = categories, count = categories.Count() });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetCategory(int id)
        {
            try
            {
                var category = _categoryService.GetById(id);
                return Ok(new { success = true, data = category });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, error = "Category not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult CreateCategory([FromBody] Category category)
        {
            try
            {
                var createdCategory = _categoryService.Create(category);
                return Ok(new { success = true, data = createdCategory });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        [HttpPut]
        public IActionResult UpdateCategory([FromBody] Category category)
        {
            try
            {
                var updatedCategory = _categoryService.Update(category);
                return Ok(new { success = true, data = updatedCategory });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, error = "Category not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            try
            {
                var result = _categoryService.Delete(id);
                if (result)
                {
                    return Ok(new { success = true, message = "Category deleted successfully" });
                }
                else
                {
                    return BadRequest(new { success = false, error = "Cannot delete category - it has news articles" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }
    }
}
