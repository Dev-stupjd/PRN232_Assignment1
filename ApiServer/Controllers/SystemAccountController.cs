using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using BussinessObjects.Models;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SystemAccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public SystemAccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
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

        [HttpGet("{id}")]
        public IActionResult GetAccount(short id)
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

        [HttpPost]
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

        [HttpPut]
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

        [HttpDelete("{id}")]
        public IActionResult DeleteAccount(short id)
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
                    return BadRequest(new { success = false, error = "Cannot delete account - it has news articles" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }
    }
}
