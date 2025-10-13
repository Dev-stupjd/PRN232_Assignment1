using BussinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IAccountRepository
    {
        List<SystemAccount> GetAllAccount();
        void AddAccount(SystemAccount account);
        void UpdateAccount(SystemAccount account);
        void DeleteAccount(SystemAccount account);
        SystemAccount GetAccountByID(int id);
    }
}
