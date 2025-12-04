using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models.SocioPublisher;

namespace TwtDominatorCore.TDFactories
{
    public class TdAccountSelectorFactory : IDestinationSelectors
    {
        private static TdAccountSelectorFactory _instance;

        private TdAccountSelectorFactory()
        {
        }

        public static TdAccountSelectorFactory Instance => _instance ?? (_instance = new TdAccountSelectorFactory());

        public bool IsPagesOrBoardsAvailable { get; set; } = false;

        public async Task<List<AccountDetailsSelectorModel>> GetGroupsDetails(string accountId, string accountName,
            List<string> alreadySelectedList)
        {
            throw new NotImplementedException();
        }

        public async Task<List<AccountDetailsSelectorModel>> GetPagesDetails(string accountId, string accountName,
            List<string> alreadySelectedList)
        {
            throw new NotImplementedException();
        }

        public async Task<List<string>> GetGroupsUrls(string accountId, string accountName)
        {
            throw new NotImplementedException();
        }

        public async Task<List<string>> GetPageOrBoardUrls(string accountId, string accountName)
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

        public bool IsGroupsAvailables { get; set; } = false;

        public string DisplayAsPageOrBoards { get; set; } = "Empty";
    }
}