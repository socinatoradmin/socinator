#region

using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;

#endregion

namespace DominatorHouseCore.FileManagers
{
    public interface IAccountsFileManager
    {
        void UpdateAccounts(IList<DominatorAccountModel> libraryAccounts);
        bool Edit(DominatorAccountModel account);
        List<DominatorAccountModel> GetAll();
        List<DominatorAccountModel> GetAll(SocialNetworks network);

        List<DominatorAccountModel> GetAllAccounts(List<string> neededAccountList,
            SocialNetworks socialNetwork);

        bool Add(DominatorAccountModel account);

        void Delete(Func<DominatorAccountModel, bool> match);

        DominatorAccountModel GetAccountById(string accountId);

        DominatorAccountModel GetAccount(string userName, SocialNetworks networks);

        IEnumerable<string> GetUsers();

        IEnumerable<string> GetUsers(SocialNetworks networks);
        List<DominatorAccountModel> GetAll(List<string> neededAccountList);
    }

    public class AccountsFileManager : IAccountsFileManager
    {
        private readonly IAccountsCacheService _accountsCacheService;

        public AccountsFileManager(IAccountsCacheService accountsCacheService)
        {
            _accountsCacheService = accountsCacheService;
        }

        // Update account entries and save to AccountDetails.bin        
        public void UpdateAccounts(IList<DominatorAccountModel> libraryAccounts)
        {
            _accountsCacheService.UpsertAccounts(libraryAccounts.ToArray());
        }

        // alias
        public bool Edit(DominatorAccountModel account)
        {
            return SaveAccount(account);
        }

        public List<DominatorAccountModel> GetAll()
        {
            return _accountsCacheService.GetAccountDetails()?.ToList();
        }

        public List<DominatorAccountModel> GetAll(SocialNetworks network)
        {
            return _accountsCacheService.GetAccountDetails().Where(a => a.AccountBaseModel.AccountNetwork == network)
                .ToList();
        }

        public List<DominatorAccountModel> GetAll(List<string> neededAccountList)
        {
            return _accountsCacheService.GetAccountDetails()
                .Where(a => neededAccountList.Contains(a.AccountBaseModel.UserName)).ToList();
        }

        public List<DominatorAccountModel> GetAllAccounts(List<string> neededAccountList,
            SocialNetworks socialNetwork)
        {
            var Accounts = _accountsCacheService.GetAccountDetails()
                .Where(a => neededAccountList.Contains(a.AccountBaseModel.UserName))
                .ToList();
            return Accounts.FindAll(x => x.AccountBaseModel.AccountNetwork == socialNetwork);
        }

        // backward compatibility for TD, PD
        public bool Add(DominatorAccountModel account)
        {
            return _accountsCacheService.UpsertAccounts(account);
        }

        public void Delete(Func<DominatorAccountModel, bool> match)
        {
            var accs = GetAll();
            _accountsCacheService.Delete(accs.Where(match).ToArray());
        }

        public DominatorAccountModel GetAccountById(string accountId)

        {
            var accounts = GetAll();
            var result = accounts.FirstOrDefault(x => x.AccountBaseModel.AccountId == accountId);
            return result;
        }

        public DominatorAccountModel GetAccount(string userName, SocialNetworks networks)
        {
            var accounts = GetAll();
            var result = accounts.FirstOrDefault(x =>
                x.AccountBaseModel.UserName == userName && x.AccountBaseModel.AccountNetwork == networks);
            return result;
        }

        public IEnumerable<string> GetUsers()
        {
            var accounts = GetAll();
            var result = accounts.Select(x => x.AccountBaseModel.UserName);
            return result;
        }

        public IEnumerable<string> GetUsers(SocialNetworks networks)
        {
            var accounts = GetAll();
            var result = accounts.Where(x => x.AccountBaseModel.AccountNetwork == networks)
                .Select(x => x.AccountBaseModel.UserName);
            return result;
        }

        // Saves one account by looking for it in list of all accounts.
        // Use Edit() in consumer code
        private bool SaveAccount(DominatorAccountModel account)
        {
            var savedStatus = _accountsCacheService.UpsertAccounts(account);
            return savedStatus;
        }
    }
}