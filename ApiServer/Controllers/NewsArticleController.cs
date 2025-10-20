using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using BussinessObjects.Models;
using System.ComponentModel.DataAnnotations;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsArticleController : ControllerBase
    {
        private readonly INewsArticleService _newsArticleService;

        public NewsArticleController(INewsArticleService newsArticleService)
        {
            _newsArticleService = newsArticleService;
        }

        // Public endpoints (no authentication required)
        [HttpGet("public")]
        public IActionResult GetActiveArticles([FromQuery] string? keyword = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var articles = _newsArticleService.GetActiveArticles(keyword, page, pageSize);
                return Ok(new { success = true, data = articles, count = articles.Count() });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        [HttpGet("public/{id}")]
        public IActionResult GetActiveArticle(string id)
        {
            try
            {
                var article = _newsArticleService.GetActiveArticleById(id);
                return Ok(new { success = true, data = article });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, error = "Active article not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        // Staff endpoints (authentication required)
        [HttpGet]
        public IActionResult GetAllArticles([FromQuery] string? keyword = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var articles = _newsArticleService.GetAll(keyword, page, pageSize);
                return Ok(new { success = true, data = articles, count = articles.Count() });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetArticle(string id)
        {
            try
            {
                var article = _newsArticleService.GetById(id);
                return Ok(new { success = true, data = article });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, error = "Article not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        [HttpGet("author/{authorId}")]
        public IActionResult GetArticlesByAuthor(short authorId, [FromQuery] string? keyword = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var articles = _newsArticleService.GetByAuthor(authorId, keyword, page, pageSize);
                return Ok(new { success = true, data = articles, count = articles.Count() });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        public class NewsArticleRequest
        {
            // Nullable so ApiController doesn't reject missing ID for create
            public string? NewsArticleId { get; set; }
            [Required]
            public string? Headline { get; set; }
            [Required]
            public string? NewsContent { get; set; }
            public string? NewsTitle { get; set; }
            public string? NewsSource { get; set; }
            public short? CategoryId { get; set; }
            public bool? NewsStatus { get; set; }
            public short? CreatedById { get; set; }
            public short? UpdatedById { get; set; }
        }

        [HttpPost]
        public IActionResult CreateArticle([FromBody] NewsArticleRequest req)
        {
            try
            {
                var article = new NewsArticle
                {
                    NewsArticleId = req.NewsArticleId ?? string.Empty, // service will generate if empty
                    NewsTitle = req.NewsTitle,
                    Headline = req.Headline ?? string.Empty,
                    NewsContent = req.NewsContent ?? string.Empty,
                    NewsSource = req.NewsSource,
                    CategoryId = req.CategoryId,
                    NewsStatus = req.NewsStatus,
                    CreatedById = req.CreatedById,
                    UpdatedById = req.UpdatedById
                };

                var createdArticle = _newsArticleService.Create(article);
                return Ok(new { success = true, data = createdArticle });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        [HttpPut]
        public IActionResult UpdateArticle([FromBody] NewsArticleRequest req)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(req.NewsArticleId))
                    return BadRequest(new { success = false, error = "NewsArticleId is required." });

                var article = new NewsArticle
                {
                    NewsArticleId = req.NewsArticleId!,
                    NewsTitle = req.NewsTitle,
                    Headline = req.Headline ?? string.Empty,
                    NewsContent = req.NewsContent ?? string.Empty,
                    NewsSource = req.NewsSource,
                    CategoryId = req.CategoryId,
                    NewsStatus = req.NewsStatus,
                    CreatedById = req.CreatedById,
                    UpdatedById = req.UpdatedById
                };

                var updatedArticle = _newsArticleService.Update(article);
                return Ok(new { success = true, data = updatedArticle });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, error = "Article not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteArticle(string id)
        {
            try
            {
                var result = _newsArticleService.Delete(id);
                if (result)
                {
                    return Ok(new { success = true, message = "Article deleted successfully" });
                }
                else
                {
                    return BadRequest(new { success = false, error = "Article not found" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        // Admin endpoints for reports
        [HttpGet("reports/date-range")]
        public IActionResult GetArticlesByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] string? keyword = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var articles = _newsArticleService.GetByDateRange(startDate, endDate, keyword, page, pageSize);
                return Ok(new { success = true, data = articles, count = articles.Count() });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }
    }
}
