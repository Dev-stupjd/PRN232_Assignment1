using BussinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IAccountService
    {
        // Queries
        IEnumerable<SystemAccount> GetAll(string? keyword = null, int page = 1, int pageSize = 20);
        SystemAccount GetById(int id);

        // Commands
        SystemAccount Create(SystemAccount account);
        SystemAccount Update(SystemAccount account);
        bool Delete(int id); // returns false if blocked by business rules

        // Auth (optional for Assignment 1)
        SystemAccount? Authenticate(string email, string password);
    }
}
