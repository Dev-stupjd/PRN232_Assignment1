using BussinessObjects;
using BussinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObjects.DAO
{
    public class AccountDAO
    {
        private FunewsManagementContext _context;

        public AccountDAO(FunewsManagementContext context)
        {
            _context = context;
        }
        public SystemAccount? GetSystemAccountById(int AccountId)
        {
            return _context.SystemAccounts.FirstOrDefault(u => u.AccountId == AccountId);
        }
        public List<SystemAccount> GetAllSystemAccounts()
        {
            return _context.SystemAccounts.ToList();
        }
        public void AddSystemAccount(SystemAccount Account)
        {
            _context.SystemAccounts.Add(Account);
            _context.SaveChanges();
        }
        public void UpdateSystemAccount(SystemAccount Account)
        {
            _context.Entry<SystemAccount>(Account).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
        }
        public void DeleteSystemAccount(int AccountId)
        {
            var Account = GetSystemAccountById(AccountId);
            if (Account != null)
            {
                _context.SystemAccounts.Remove(Account);
                _context.SaveChanges();
            }
        }
    }
}
