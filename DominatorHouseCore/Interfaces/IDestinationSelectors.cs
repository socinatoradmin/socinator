#region

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DominatorHouseCore.Models.SocioPublisher;

#endregion

namespace DominatorHouseCore.Interfaces
{
    public interface IDestinationSelectors
    {
        bool IsGroupsAvailables { get; set; }

        string DisplayAsPageOrBoards { get; set; }

        Task<List<AccountDetailsSelectorModel>> GetGroupsDetails(string accountId, string accountName,
            List<string> alreadySelectedList);

        Task<List<AccountDetailsSelectorModel>> GetFriendsDetails(string accountId, string accountName,
            List<string> alreadySelectedList);

        Task<List<AccountDetailsSelectorModel>> GetPagesDetails(string accountId, string accountName,
            List<string> alreadySelectedList);

        Task<List<string>> GetGroupsUrls(string accountId, string accountName);

        Task<List<string>> GetGroupUrls(string accountId, DateTime addedAfter);

        Task<List<string>> GetPageOrBoardUrls(string accountId, string accountName);
    }
}