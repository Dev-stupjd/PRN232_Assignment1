using BussinessObjects.Models;
using DataAccessObjects.DAO;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public  class AccountRepository : IAccountRepository
    {
        private readonly AccountDAO _accountDAO;

        public AccountRepository(AccountDAO accountDAO)
        {
            _accountDAO = accountDAO;
        }
        public List<SystemAccount> GetAllAccount() => _accountDAO.GetAllSystemAccounts();
        public void AddAccount(SystemAccount user) => _accountDAO.AddSystemAccount(user);
        public void UpdateAccount(SystemAccount user) => _accountDAO.UpdateSystemAccount(user);
        public void DeleteAccount(SystemAccount user) => _accountDAO.DeleteSystemAccount(user.AccountId);
        public SystemAccount GetAccountByID(int id) => _accountDAO.GetSystemAccountById(id);
    }
}
