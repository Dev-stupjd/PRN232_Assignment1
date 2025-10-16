using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using BussinessObjects.Models;
using BussinessObjects;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("odata")]
    public class CategoryODataController : ODataController
    {
        private readonly FunewsManagementContext _context;

        public CategoryODataController(FunewsManagementContext context)
        {
            _context = context;
        }

        [EnableQuery]
        [HttpGet("Category")]
        public IActionResult Get()
        {
            return Ok(_context.Categories);
        }

        [EnableQuery]
        [HttpGet("Category({key})")]
        public IActionResult Get([FromRoute] int key)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == key);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        [HttpPost("Category")]
        public IActionResult Post([FromBody] Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Categories.Add(category);
            _context.SaveChanges();
            return Created(category);
        }

        [HttpPut("Category({key})")]
        public IActionResult Put([FromRoute] int key, [FromBody] Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingCategory = _context.Categories.FirstOrDefault(c => c.CategoryId == key);
            if (existingCategory == null)
            {
                return NotFound();
            }

            existingCategory.CategoryName = category.CategoryName;
            existingCategory.CategoryDesciption = category.CategoryDesciption;
            existingCategory.ParentCategoryId = category.ParentCategoryId;
            existingCategory.IsActive = category.IsActive;

            _context.SaveChanges();
            return Ok(existingCategory);
        }

        [HttpDelete("Category({key})")]
        public IActionResult Delete([FromRoute] int key)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == key);
            if (category == null)
            {
                return NotFound();
            }

            // Check if category has news articles (business rule)
            var hasArticles = _context.NewsArticles.Any(n => n.CategoryId == key);
            if (hasArticles)
            {
                return BadRequest("Cannot delete category - it has news articles");
            }

            _context.Categories.Remove(category);
            _context.SaveChanges();
            return NoContent();
        }
    }
}

