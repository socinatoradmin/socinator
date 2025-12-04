using DominatorHouseCore.DatabaseHandler.RdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedditDominatorCore.RDFactories
{
    public class RdAccountSelectorFactory : IDestinationSelectors
    {
        private static RdAccountSelectorFactory _instance;

        private RdAccountSelectorFactory()
        {
        }

        public static RdAccountSelectorFactory Instance
            => _instance ?? (_instance = new RdAccountSelectorFactory());

        public bool IsPagesOrBoardsAvailable { get; set; } = false;

        public async Task<List<AccountDetailsSelectorModel>> GetGroupsDetails(string accountId, string accountName,
            List<string> alreadySelectedList)
        {
            var listGroupUrl = new List<AccountDetailsSelectorModel>();

            var dbOperartion = new DbOperations(accountId, SocialNetworks.Reddit, ConstantVariable.GetAccountDb);

            var groups = await dbOperartion.GetAsync<OwnCommunities>();

            groups.ForEach(x =>
            {
                var accountDetailsSelectorModel = new AccountDetailsSelectorModel
                {
                    AccountId = accountId,
                    AccountName = accountName,
                    DetailName = x.Name,
                    DetailUrl = x.Url,
                    IsSelected = alreadySelectedList.Contains(x.Url)
                };
                listGroupUrl.Add(accountDetailsSelectorModel);
            });

            return listGroupUrl;
        }

        public async Task<List<AccountDetailsSelectorModel>> GetPagesDetails(string accountId, string accountName,
            List<string> alreadySelectedList)
        {
            var listPagesUrl = new List<AccountDetailsSelectorModel>();

            return listPagesUrl;
        }

        public async Task<List<string>> GetGroupsUrls(string accountId, string accountName)
        {
            var listGroupUrl = new List<string>();

            var dataBase = new DbOperations(accountId, SocialNetworks.Facebook, ConstantVariable.GetAccountDb);

            var groups = await dataBase.GetAsync<OwnCommunities>();

            groups?.ForEach(x => { listGroupUrl.Add(x.Url); });

            return listGroupUrl;
        }

        public Task<List<string>> GetGroupUrls(string accountId, DateTime addedAfter)
        {
            return null;
        }

        public async Task<List<string>> GetPageOrBoardUrls(string accountId, string accountName)
        {
            var listPageUrl = new List<string>();
            return listPageUrl;
        }

        public async Task<List<AccountDetailsSelectorModel>> GetFriendsDetails(string accountId, string accountName,
            List<string> alreadySelectedList)
        {
            return new List<AccountDetailsSelectorModel>();
        }

        public bool IsGroupsAvailables { get; set; } = true;

        public string DisplayAsPageOrBoards { get; set; } = "Communities";
    }
}