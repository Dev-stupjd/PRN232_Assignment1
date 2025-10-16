using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using BussinessObjects.Models;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public TestController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("accounts")]
        public IActionResult GetAllAccounts([FromQuery] string? keyword = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var accounts = _accountService.GetAll(keyword, page, pageSize);
                return Ok(new { success = true, data = accounts, count = accounts.Count() });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        [HttpGet("accounts/{id}")]
        public IActionResult GetAccount(int id)
        {
            try
            {
                var account = _accountService.GetById(id);
                return Ok(new { success = true, data = account });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, error = "Account not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        [HttpPost("accounts")]
        public IActionResult CreateAccount([FromBody] SystemAccount account)
        {
            try
            {
                var createdAccount = _accountService.Create(account);
                return Ok(new { success = true, data = createdAccount });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        [HttpPut("accounts")]
        public IActionResult UpdateAccount([FromBody] SystemAccount account)
        {
            try
            {
                var updatedAccount = _accountService.Update(account);
                return Ok(new { success = true, data = updatedAccount });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, error = "Account not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        [HttpDelete("accounts/{id}")]
        public IActionResult DeleteAccount(int id)
        {
            try
            {
                var result = _accountService.Delete(id);
                if (result)
                {
                    return Ok(new { success = true, message = "Account deleted successfully" });
                }
                else
                {
                    return BadRequest(new { success = false, error = "Cannot delete account - it has created news articles" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        [HttpPost("accounts/authenticate")]
        public IActionResult Authenticate([FromBody] LoginRequest request)
        {
            try
            {
                var account = _accountService.Authenticate(request.Email, request.Password);
                if (account != null)
                {
                    return Ok(new { success = true, data = account });
                }
                else
                {
                    return Unauthorized(new { success = false, error = "Invalid credentials" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { 
                success = true, 
                message = "FU News Management API is working!",
                timestamp = DateTime.Now,
                version = "1.0.0",
                endpoints = new {
                    accounts = new[] {
                        "GET /api/test/accounts - Get all accounts",
                        "GET /api/test/accounts/{id} - Get account by ID", 
                        "POST /api/test/accounts - Create account",
                        "PUT /api/test/accounts - Update account",
                        "DELETE /api/test/accounts/{id} - Delete account",
                        "POST /api/test/accounts/authenticate - Login"
                    },
                    categories = new[] {
                        "GET /api/category - Get all categories",
                        "GET /api/category/{id} - Get category by ID",
                        "POST /api/category - Create category",
                        "PUT /api/category - Update category",
                        "DELETE /api/category/{id} - Delete category"
                    },
                    newsArticles = new[] {
                        "GET /api/newsarticle/public - Get active articles (public)",
                        "GET /api/newsarticle/public/{id} - Get active article by ID (public)",
                        "GET /api/newsarticle - Get all articles (staff)",
                        "GET /api/newsarticle/{id} - Get article by ID (staff)",
                        "GET /api/newsarticle/author/{authorId} - Get articles by author (staff)",
                        "POST /api/newsarticle - Create article (staff)",
                        "PUT /api/newsarticle - Update article (staff)",
                        "DELETE /api/newsarticle/{id} - Delete article (staff)"
                    },
                    tags = new[] {
                        "GET /api/tag - Get all tags",
                        "GET /api/tag/{id} - Get tag by ID",
                        "POST /api/tag - Create tag",
                        "PUT /api/tag - Update tag",
                        "DELETE /api/tag/{id} - Delete tag"
                    },
                    reports = new[] {
                        "GET /api/report/news-statistics - Get news statistics by date range (admin)",
                        "GET /api/report/category-statistics - Get category statistics (admin)",
                        "GET /api/report/author-statistics - Get author statistics (admin)",
                        "GET /api/report/overall-statistics - Get overall statistics (admin)"
                    }
                }
            });
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
