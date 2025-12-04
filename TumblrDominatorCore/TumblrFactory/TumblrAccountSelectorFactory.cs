using CommonServiceLocator;
using DominatorHouseCore.DatabaseHandler.TumblrTables.Account;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TumblrDominatorCore.TumblrLibrary.DAL;

namespace TumblrDominatorCore.TumblrFactory
{
    public class TumblrAccountSelectorFactory : IDestinationSelectors
    {
        private static TumblrAccountSelectorFactory _instance;

        private TumblrAccountSelectorFactory()
        {
        }

        public static TumblrAccountSelectorFactory Instance
            => _instance ?? (_instance = new TumblrAccountSelectorFactory());

        public bool IsPagesOrBoardsAvailable { get; set; } = true;

        public async Task<List<AccountDetailsSelectorModel>> GetPagesDetails(string accountId, string accountName,
            List<string> alreadySelectedList)
        {
            var listPagesUrl = new List<AccountDetailsSelectorModel>();
            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            var account = accountsFileManager.GetAll().FirstOrDefault(x => x.AccountId == accountId);
            var dbAccountService = InstanceProvider.ResolveWithDominatorAccount<IDbAccountService>(account);


            #region OwnPages

            var ownPages = await dbAccountService.GetAsync<OwnBlogs>();

            ownPages.ForEach(x =>
            {
                var accountDetailsSelectorModel = new AccountDetailsSelectorModel
                {
                    AccountId = accountId,
                    AccountName = accountName,
                    DetailName = x.BlogName,
                    DetailUrl = x.BlogUrl,
                    Network = SocialNetworks.Tumblr,
                    IsSelected = alreadySelectedList.Contains(x.BlogUrl)
                };
                listPagesUrl.Add(accountDetailsSelectorModel);
            });

            #endregion

            return listPagesUrl;
        }

        public async Task<List<string>> GetPageOrBoardUrls(string accountId, string accountName)
        {
            var listPageUrl = new List<string>();

            var dataBase = new DbOperations(accountId, SocialNetworks.Tumblr, ConstantVariable.GetAccountDb);

            var pages = await dataBase.GetAsync<OwnBlogs>();

            pages?.ForEach(x => { listPageUrl.Add(x.BlogUrl); });

            return listPageUrl;
        }

        public Task<List<AccountDetailsSelectorModel>> GetGroupsDetails(string accountId, string accountName,
            List<string> alreadySelectedList)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetGroupsUrls(string accountId, string accountName)
        {
            return null;
        }

        public Task<List<string>> GetGroupUrls(string accountId, DateTime addedAfter)
        {
            return null;
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