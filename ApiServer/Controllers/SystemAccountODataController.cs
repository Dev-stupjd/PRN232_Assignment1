using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using BussinessObjects.Models;
using BussinessObjects;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("odata")]
    public class SystemAccountODataController : ODataController
    {
        private readonly FunewsManagementContext _context;

        public SystemAccountODataController(FunewsManagementContext context)
        {
            _context = context;
        }

        [EnableQuery]
        [HttpGet("SystemAccount")]
        public IActionResult Get()
        {
            return Ok(_context.SystemAccounts);
        }

        [EnableQuery]
        [HttpGet("SystemAccount({key})")]
        public IActionResult Get([FromRoute] short key)
        {
            var account = _context.SystemAccounts.FirstOrDefault(a => a.AccountId == key);
            if (account == null)
            {
                return NotFound();
            }
            return Ok(account);
        }

        [HttpPost("SystemAccount")]
        public IActionResult Post([FromBody] SystemAccount account)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.SystemAccounts.Add(account);
            _context.SaveChanges();
            return Created(account);
        }

        [HttpPut("SystemAccount({key})")]
        public IActionResult Put([FromRoute] short key, [FromBody] SystemAccount account)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingAccount = _context.SystemAccounts.FirstOrDefault(a => a.AccountId == key);
            if (existingAccount == null)
            {
                return NotFound();
            }

            existingAccount.AccountName = account.AccountName;
            existingAccount.AccountEmail = account.AccountEmail;
            existingAccount.AccountRole = account.AccountRole;
            existingAccount.AccountPassword = account.AccountPassword;

            _context.SaveChanges();
            return Ok(existingAccount);
        }

        [HttpDelete("SystemAccount({key})")]
        public IActionResult Delete([FromRoute] short key)
        {
            var account = _context.SystemAccounts.FirstOrDefault(a => a.AccountId == key);
            if (account == null)
            {
                return NotFound();
            }

            // Check if account has created news articles (business rule)
            var hasArticles = _context.NewsArticles.Any(n => n.CreatedById == key);
            if (hasArticles)
            {
                return BadRequest("Cannot delete account - it has created news articles");
            }

            _context.SystemAccounts.Remove(account);
            _context.SaveChanges();
            return NoContent();
        }
    }
}

