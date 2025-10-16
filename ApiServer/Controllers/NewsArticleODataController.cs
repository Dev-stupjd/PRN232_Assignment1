using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using BussinessObjects.Models;
using BussinessObjects;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("odata")]
    public class NewsArticleODataController : ODataController
    {
        private readonly FunewsManagementContext _context;

        public NewsArticleODataController(FunewsManagementContext context)
        {
            _context = context;
        }

        [EnableQuery]
        [HttpGet("NewsArticle")]
        public IActionResult Get()
        {
            return Ok(_context.NewsArticles);
        }

        [EnableQuery]
        [HttpGet("NewsArticle({key})")]
        public IActionResult Get([FromRoute] string key)
        {
            var article = _context.NewsArticles.FirstOrDefault(a => a.NewsArticleId == key);
            if (article == null)
            {
                return NotFound();
            }
            return Ok(article);
        }

        [EnableQuery]
        [HttpGet("NewsArticle/Active")]
        public IActionResult GetActive()
        {
            return Ok(_context.NewsArticles.Where(a => a.NewsStatus == true));
        }

        [HttpPost("NewsArticle")]
        public IActionResult Post([FromBody] NewsArticle article)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Set default values
            article.CreatedDate = DateTime.Now;
            article.NewsStatus = true;

            // Generate unique ID if not provided
            if (string.IsNullOrWhiteSpace(article.NewsArticleId))
            {
                article.NewsArticleId = GenerateArticleId();
            }

            _context.NewsArticles.Add(article);
            _context.SaveChanges();
            return Created(article);
        }

        [HttpPut("NewsArticle({key})")]
        public IActionResult Put([FromRoute] string key, [FromBody] NewsArticle article)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingArticle = _context.NewsArticles.FirstOrDefault(a => a.NewsArticleId == key);
            if (existingArticle == null)
            {
                return NotFound();
            }

            // Preserve original creation data
            article.CreatedDate = existingArticle.CreatedDate;
            article.CreatedById = existingArticle.CreatedById;
            article.ModifiedDate = DateTime.Now;

            existingArticle.NewsTitle = article.NewsTitle;
            existingArticle.Headline = article.Headline;
            existingArticle.NewsContent = article.NewsContent;
            existingArticle.NewsSource = article.NewsSource;
            existingArticle.CategoryId = article.CategoryId;
            existingArticle.NewsStatus = article.NewsStatus;
            existingArticle.UpdatedById = article.UpdatedById;
            existingArticle.ModifiedDate = article.ModifiedDate;

            _context.SaveChanges();
            return Ok(existingArticle);
        }

        [HttpDelete("NewsArticle({key})")]
        public IActionResult Delete([FromRoute] string key)
        {
            var article = _context.NewsArticles.FirstOrDefault(a => a.NewsArticleId == key);
            if (article == null)
            {
                return NotFound();
            }

            _context.NewsArticles.Remove(article);
            _context.SaveChanges();
            return NoContent();
        }

        private static string GenerateArticleId()
        {
            return "ART" + DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 9999);
        }
    }
}

