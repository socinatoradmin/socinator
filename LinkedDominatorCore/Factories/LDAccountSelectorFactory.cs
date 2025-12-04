using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.LDLibrary.DAL;

namespace LinkedDominatorCore.Factories
{
    // ReSharper disable once InconsistentNaming
    public class LDAccountSelectorFactory : IDestinationSelectors
    {
        private static LDAccountSelectorFactory _instance;

        public IDbAccountService DbAccountService;

        private LDAccountSelectorFactory()
        {
        }

        public static LDAccountSelectorFactory Instance
            => _instance ?? (_instance = new LDAccountSelectorFactory());

        public bool IsPagesOrBoardsAvailable { get; set; } = false;


        public async Task<List<AccountDetailsSelectorModel>> GetGroupsDetails(string accountId, string accountName,
            List<string> alreadySelectedList)
        {
            DbAccountService = new DbAccountService(new DominatorAccountModel {AccountId = accountId});
            var listGroupUrl = new List<AccountDetailsSelectorModel>();
            var groups = await DbAccountService.GetAsync<Groups>();

            groups.ForEach(x =>
            {
                var accountDetailsSelectorModel = new AccountDetailsSelectorModel
                {
                    AccountId = accountId,
                    AccountName = accountName,
                    DetailName = x.GroupName,
                    DetailUrl = x.GroupUrl,
                    IsSelected = alreadySelectedList.Contains(x.GroupUrl)
                };
                listGroupUrl.Add(accountDetailsSelectorModel);
            });

            return listGroupUrl;
        }

        public async Task<List<string>> GetGroupsUrls(string accountId, string accountName)
        {
            var listGroupUrl = new List<string>();

            var dataBase = new DbOperations(accountId, SocialNetworks.LinkedIn, ConstantVariable.GetAccountDb);

            var groups = await dataBase.GetAsync<Groups>();

            groups?.ForEach(x => { listGroupUrl.Add(x.GroupUrl); });

            return listGroupUrl;
        }

        public async Task<List<AccountDetailsSelectorModel>> GetPagesDetails(string accountId, string accountName,
            List<string> alreadySelectedList)
        {
            DbAccountService = new DbAccountService(new DominatorAccountModel {AccountId = accountId});
            var listPageURL = new List<AccountDetailsSelectorModel>();
            var pages = await DbAccountService.GetAsync<Pages>();

            pages.ForEach(x =>
            {
                var accountDetailsSelectorModel = new AccountDetailsSelectorModel
                {
                    AccountId = accountId,
                    AccountName = accountName,
                    DetailName = x.PageName,
                    DetailUrl = x.PageUrl,
                    IsSelected = alreadySelectedList.Contains(x.PageUrl)
                };
                listPageURL.Add(accountDetailsSelectorModel);
            });

            return listPageURL;
        }

        public async Task<List<string>> GetPageOrBoardUrls(string accountId, string accountName)
        {
            var listPageUrl = new List<string>();

            var dataBase = new DbOperations(accountId, SocialNetworks.LinkedIn, ConstantVariable.GetAccountDb);

            var pages = await dataBase.GetAsync<Pages>();

            pages?.ForEach(x => { listPageUrl.Add(x.PageUrl); });

            return listPageUrl;
        }

        public async Task<List<string>> GetGroupUrls(string accountId, DateTime addedAfter)
        {
            var listGroupUrl = new List<string>();

            var groups = await DbAccountService.GetAsync<Groups>();

            groups?.Where(x => x.InteractionTimeStamp > addedAfter.GetCurrentEpochTime())
                .ForEach(x => { listGroupUrl.Add(x.GroupUrl); });

            return listGroupUrl;
        }

        public Task<List<AccountDetailsSelectorModel>> GetFriendsDetails(string accountId, string accountName,
            List<string> alreadySelectedList)
        {
            throw new NotImplementedException();
        }

        public bool IsGroupsAvailables { get; set; } = true;

        public string DisplayAsPageOrBoards { get; set; } = "Page";
    }
}