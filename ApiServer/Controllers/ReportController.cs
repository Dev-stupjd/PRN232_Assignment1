using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using BussinessObjects.Models;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("news-statistics")]
        public IActionResult GetNewsStatistics([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] string? keyword = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var articles = _reportService.GetNewsStatisticsByDateRange(startDate, endDate, keyword, page, pageSize);
                return Ok(new { success = true, data = articles, count = articles.Count() });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        [HttpGet("category-statistics")]
        public IActionResult GetCategoryStatistics([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var statistics = _reportService.GetCategoryStatistics(startDate, endDate);
                return Ok(new { success = true, data = statistics });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        [HttpGet("author-statistics")]
        public IActionResult GetAuthorStatistics([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var statistics = _reportService.GetAuthorStatistics(startDate, endDate);
                return Ok(new { success = true, data = statistics });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        [HttpGet("overall-statistics")]
        public IActionResult GetOverallStatistics([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var statistics = _reportService.GetOverallStatistics(startDate, endDate);
                return Ok(new { success = true, data = statistics });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }
    }
}
