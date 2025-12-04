using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonServiceLocator;
using DominatorHouseCore.DatabaseHandler.PdTables.Accounts;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models.SocioPublisher;
using PinDominatorCore.PDLibrary.DAL;
using DominatorHouseCore.Utility;

namespace PinDominatorCore.PDFactories
{
    public class PdAccountSelectorFactory : IDestinationSelectors
    {
        private static PdAccountSelectorFactory _instance;

        private PdAccountSelectorFactory()
        {
        }

        public static PdAccountSelectorFactory Instance
            => _instance ?? (_instance = new PdAccountSelectorFactory());

        public async Task<List<AccountDetailsSelectorModel>> GetPagesDetails(string accountId, string accountName,
            List<string> alreadySelectedList)
        {
            var listBoardUrl = new List<AccountDetailsSelectorModel>();

            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            var account = accountsFileManager.GetAll().FirstOrDefault(x => x.AccountId == accountId);
            var dbAccountService = InstanceProvider.ResolveWithDominatorAccount<IDbAccountService>(account);

            var boards = await dbAccountService.GetAsync<OwnBoards>();

            boards.ForEach(x =>
            {
                var accountDetailsSelectorModel = new AccountDetailsSelectorModel
                {
                    AccountId = accountId,
                    AccountName = accountName,
                    DetailName = x.BoardName,
                    DetailUrl = x.BoardUrl,
                    DetailSection = x.BoardSectionCount > 0 ? $"0/{x.BoardSectionCount.ToString()}" : "LangKeyNA".FromResourceDictionary(),
                    DetailSectionValue=x.BoardSections,
                    IsSectionAvailable = x.BoardSectionCount > 0,
                    IsSelected = alreadySelectedList.Contains(x.BoardUrl)
                };
                listBoardUrl.Add(accountDetailsSelectorModel);
            });

            return listBoardUrl;
        }

        public async Task<List<string>> GetPageOrBoardUrls(string accountId, string accountName)
        {
            var listBoardUrl = new List<string>();

            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            var account = accountsFileManager.GetAll().FirstOrDefault(x => x.AccountId == accountId);
            var dbAccountService = InstanceProvider.ResolveWithDominatorAccount<IDbAccountService>(account);

            var boards = await dbAccountService.GetAsync<OwnBoards>();

            boards?.ForEach(x => { listBoardUrl.Add(x.BoardUrl); });

            return listBoardUrl;
        }

        public string DisplayAsPageOrBoards { get; set; } = "LangKeyBoard".FromResourceDictionary();

        public bool IsGroupsAvailables { get; set; } = false;

        public bool IsPagesOrBoardsAvailable { get; set; } = true;


        public Task<List<string>> GetGroupsUrls(string accountId, string accountName)
        {
            throw new NotImplementedException();
        }

        public Task<List<AccountDetailsSelectorModel>> GetGroupsDetails(string accountId, string accountName,
            List<string> alreadySelectedList)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetGroupUrls(string accountId, DateTime addedAfter)
        {
            throw new NotImplementedException();
        }

        public Task<List<AccountDetailsSelectorModel>> GetFriendsDetails(string accountId, string accountName,
            List<string> alreadySelectedList)
        {
            throw new NotImplementedException();
        }
    }
}