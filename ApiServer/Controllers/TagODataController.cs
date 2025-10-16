using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using BussinessObjects.Models;
using BussinessObjects;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("odata")]
    public class TagODataController : ODataController
    {
        private readonly FunewsManagementContext _context;

        public TagODataController(FunewsManagementContext context)
        {
            _context = context;
        }

        [EnableQuery]
        [HttpGet("Tag")]
        public IActionResult Get()
        {
            return Ok(_context.Tags);
        }

        [EnableQuery]
        [HttpGet("Tag({key})")]
        public IActionResult Get([FromRoute] int key)
        {
            var tag = _context.Tags.FirstOrDefault(t => t.TagId == key);
            if (tag == null)
            {
                return NotFound();
            }
            return Ok(tag);
        }

        [HttpPost("Tag")]
        public IActionResult Post([FromBody] Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Tags.Add(tag);
            _context.SaveChanges();
            return Created(tag);
        }

        [HttpPut("Tag({key})")]
        public IActionResult Put([FromRoute] int key, [FromBody] Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingTag = _context.Tags.FirstOrDefault(t => t.TagId == key);
            if (existingTag == null)
            {
                return NotFound();
            }

            existingTag.TagName = tag.TagName;
            existingTag.Note = tag.Note;

            _context.SaveChanges();
            return Ok(existingTag);
        }

        [HttpDelete("Tag({key})")]
        public IActionResult Delete([FromRoute] int key)
        {
            var tag = _context.Tags.FirstOrDefault(t => t.TagId == key);
            if (tag == null)
            {
                return NotFound();
            }

            _context.Tags.Remove(tag);
            _context.SaveChanges();
            return NoContent();
        }
    }
}

