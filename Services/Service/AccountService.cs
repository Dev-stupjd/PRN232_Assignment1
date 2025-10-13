using BussinessObjects.Models;
using Repositories.Interface;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    // Minimal article usage check to satisfy Assignment 1 rule
    public interface INewsArticleRepositoryLite
    {
        bool AnyCreatedBy(short accountId); // or int, match your model
    }

    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _repo;
        private readonly INewsArticleRepositoryLite _articles; // for delete guard; inject a stub if not ready

        public AccountService(IAccountRepository repo, INewsArticleRepositoryLite articles)
        {
            _repo = repo;
            _articles = articles;
        }

        // READ ALL with simple search + paging
        public IEnumerable<SystemAccount> GetAll(string? keyword = null, int page = 1, int pageSize = 20)
        {
            if (page <= 0 || pageSize <= 0)
                throw new ArgumentException("Invalid paging.");
            var data = _repo.GetAllAccount().AsEnumerable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                data = data.Where(a =>
                    (a.AccountName?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (a.AccountEmail?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            return data
                .OrderByDescending(a => a.AccountId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public SystemAccount GetById(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be positive.", nameof(id));
            var acc = _repo.GetAccountByID(id);
            return acc ?? throw new KeyNotFoundException("Account not found.");
        }

        // CREATE with basic validation
        public SystemAccount Create(SystemAccount account)
        {
            Validate(account, isUpdate: false);

            // (optional) enforce unique email
            var exists = _repo.GetAllAccount()
                              .Any(a => a.AccountEmail!.Equals(account.AccountEmail, StringComparison.OrdinalIgnoreCase));
            if (exists)
                throw new InvalidOperationException("Email already exists.");

            _repo.AddAccount(account);
            return account;
        }

        // UPDATE with basic validation
        public SystemAccount Update(SystemAccount account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));
            if (_repo.GetAccountByID(account.AccountId) is null)
                throw new KeyNotFoundException("Account not found.");

            Validate(account, isUpdate: true);

            _repo.UpdateAccount(account);
            return account;
        }

        // DELETE respecting assignment rule: cannot delete if the account created any articles
        public bool Delete(int id)
        {
            var existing = _repo.GetAccountByID(id);
            if (existing is null)
                return false;

            // Assignment rule (Admin feature): block deletion if the account has created news
            // “the delete action will delete an account in the case this account is not belong to any news articles …”
            if (_articles.AnyCreatedBy((short)existing.AccountId))
                return false;

            _repo.DeleteAccount(existing);
            return true;
        }

        // AUTH (simple sample; you’ll likely replace with hashed passwords)
        public SystemAccount? Authenticate(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            return _repo.GetAllAccount()
                        .FirstOrDefault(a =>
                            a.AccountEmail!.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase) &&
                            a.AccountPassword == password);
        }

        // --- helpers ---
        private static void Validate(SystemAccount account, bool isUpdate)
        {
            if (string.IsNullOrWhiteSpace(account.AccountEmail))
                throw new ArgumentException("Email is required.");
            if (string.IsNullOrWhiteSpace(account.AccountName))
                throw new ArgumentException("Name is required.");
            if (!isUpdate && string.IsNullOrWhiteSpace(account.AccountPassword))
                throw new ArgumentException("Password is required.");
            // add more rules (lengths, role ranges, etc.) as your schema demands
        }
    }
}
