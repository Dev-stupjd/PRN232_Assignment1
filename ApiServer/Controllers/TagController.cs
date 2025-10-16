using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using BussinessObjects.Models;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [HttpGet]
        public IActionResult GetAllTags([FromQuery] string? keyword = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var tags = _tagService.GetAll(keyword, page, pageSize);
                return Ok(new { success = true, data = tags, count = tags.Count() });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetTag(int id)
        {
            try
            {
                var tag = _tagService.GetById(id);
                return Ok(new { success = true, data = tag });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, error = "Tag not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult CreateTag([FromBody] Tag tag)
        {
            try
            {
                var createdTag = _tagService.Create(tag);
                return Ok(new { success = true, data = createdTag });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        [HttpPut]
        public IActionResult UpdateTag([FromBody] Tag tag)
        {
            try
            {
                var updatedTag = _tagService.Update(tag);
                return Ok(new { success = true, data = updatedTag });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, error = "Tag not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTag(int id)
        {
            try
            {
                var result = _tagService.Delete(id);
                if (result)
                {
                    return Ok(new { success = true, message = "Tag deleted successfully" });
                }
                else
                {
                    return BadRequest(new { success = false, error = "Tag not found" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }
    }
}
