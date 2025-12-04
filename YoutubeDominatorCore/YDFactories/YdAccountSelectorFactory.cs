using DominatorHouseCore.DatabaseHandler.YdTables.Accounts;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Process.ExecutionCounters;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YoutubeDominatorCore.YoutubeLibrary.DAL;

namespace YoutubeDominatorCore.YDFactories
{
    public class YdAccountSelectorFactory : IDestinationSelectors
    {
        private static YdAccountSelectorFactory _instance;
        private IEntityCountersManager entityCountersManager;

        public static YdAccountSelectorFactory Instance
            => _instance ?? (_instance = new YdAccountSelectorFactory());

        public bool IsPagesOrBoardsAvailable { get; set; } = true;

        public string DisplayAsPageOrBoards { get; set; } = "LangKeyChannel".FromResourceDictionary();

        public bool IsGroupsAvailables { get; set; } = false;

        public Task<List<AccountDetailsSelectorModel>> GetFriendsDetails(string accountId, string accountName,
            List<string> alreadySelectedList)
        {
            throw new NotImplementedException();
        }

        public Task<List<AccountDetailsSelectorModel>> GetGroupsDetails(string accountId, string accountName,
            List<string> alreadySelectedList)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetGroupsUrls(string accountId, string accountName)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetGroupUrls(string accountId, DateTime addedAfter)
        {
            throw new NotImplementedException();
        }

        public async Task<List<string>> GetPageOrBoardUrls(string accountId, string accountName)
        {
            var listChannelUrl = new List<string>();

            IDbAccountService dbAccountService = new DbAccountService(entityCountersManager,
                new DominatorAccountModel { AccountId = accountId });

            var boards = await dbAccountService.GetAsync<OwnChannels>();

            boards?.ForEach(x => { listChannelUrl.Add(x.ChannelName); });

            return listChannelUrl;
        }

        public async Task<List<AccountDetailsSelectorModel>> GetPagesDetails(string accountId, string accountName,
            List<string> alreadySelectedList)
        {
            var listChannelUrl = new List<AccountDetailsSelectorModel>();

            IDbAccountService dbAccountService = new DbAccountService(entityCountersManager,
                new DominatorAccountModel { AccountId = accountId });

            var boards = await dbAccountService.GetAsync<OwnChannels>();

            boards.ForEach(x =>
            {
                var accountDetailsSelectorModel = new AccountDetailsSelectorModel
                {
                    AccountId = accountId,
                    AccountName = accountName,
                    DetailUrl = x.PageId ?? "",
                    DetailName = x.ChannelName,
                    IsSelected = x.IsSelected
                };
                listChannelUrl.Add(accountDetailsSelectorModel);
            });

            return listChannelUrl;
        }
    }
}