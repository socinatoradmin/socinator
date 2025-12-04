using CommonServiceLocator;
using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FaceDominatorUI.FdCoreLibrary
{
    public class FdAccountSelectorFactory : IDestinationSelectors
    {
        private static FdAccountSelectorFactory _instance;

        private FdAccountSelectorFactory()
        {
        }

        public static FdAccountSelectorFactory Instance
            => _instance ?? (_instance = new FdAccountSelectorFactory());

        public bool IsPagesOrBoardsAvailable { get; set; } = true;

        public async Task<List<AccountDetailsSelectorModel>> GetGroupsDetails(string accountId, string accountName,
            List<string> alreadySelectedList)
        {
            int count = 0;
            var listGroupUrl = new List<AccountDetailsSelectorModel>();
        RunTry: try
            {
                var objDbAccountService =
                    InstanceProvider.ResolveAccountDbOperations(accountId, SocialNetworks.Facebook);

                var groups = await objDbAccountService.GetAsync<OwnGroups>();

                groups.ForEach(x =>
                {
                    var accountDetailsSelectorModel = new AccountDetailsSelectorModel
                    {
                        AccountId = accountId,
                        AccountName = accountName,
                        DetailName = x.GroupName,
                        DetailUrl = x.GroupUrl,
                        IsSelected = alreadySelectedList.Contains(x.GroupUrl),
                        IsFanpage = false,
                        IsJoinedGroup = x.GroupType == "NonAdmin" ? true : false,
                        IsOwnGroup = x.GroupType == "Admin" ? true : false,
                        IsGroup = true
                    };
                    listGroupUrl.Add(accountDetailsSelectorModel);
                });
            }
            catch (Exception)
            {
                count++;
                listGroupUrl.Clear();
                if (count <= 5)
                    goto RunTry;
            }

            return listGroupUrl;
        }

        public async Task<List<AccountDetailsSelectorModel>> GetPagesDetails(string accountId, string accountName,
            List<string> alreadySelectedList)
        {
            var listPagesUrl = new List<AccountDetailsSelectorModel>();

            // DbAccountService objDbAccountService = new DbAccountService(accountId);
            var objDbAccountService =
                InstanceProvider.ResolveAccountDbOperations(accountId, SocialNetworks.Facebook);

            #region OwnPages

            var ownPages = await objDbAccountService.GetAsync<OwnPages>();
            ownPages.ForEach(x =>
            {
                var accountDetailsSelectorModel = new AccountDetailsSelectorModel
                {
                    AccountId = accountId,
                    AccountName = accountName,
                    DetailName = x.PageName,
                    DetailUrl = x.PageUrl,
                    IsSelected = alreadySelectedList.Contains(x.PageUrl),
                    IsOwnPage = true,
                    IsFanpage = true
                };
                listPagesUrl.Add(accountDetailsSelectorModel);
            });

            #endregion

            #region LikedPages

            var likedPages = await objDbAccountService.GetAsync<LikedPages>();

            likedPages.ForEach(x =>
            {
                var accountDetailsSelectorModel = new AccountDetailsSelectorModel
                {
                    AccountId = accountId,
                    AccountName = accountName,
                    DetailName = x.PageName,
                    DetailUrl = x.PageUrl,
                    IsSelected = alreadySelectedList.Contains(x.PageUrl),
                    IsLikePage = true,
                    IsFanpage = true
                };
                listPagesUrl.Add(accountDetailsSelectorModel);
            });

            #endregion

            return listPagesUrl;
        }

        public async Task<List<string>> GetGroupsUrls(string accountId, string accountName)
        {
            var listGroupUrl = new List<string>();

            // DbAccountService objDbAccountService = new DbAccountService(accountId);

            var objDbAccountService =
                InstanceProvider.ResolveAccountDbOperations(accountId, SocialNetworks.Facebook);

            var groups = await objDbAccountService.GetAsync<OwnGroups>();

            groups?.ForEach(x => { listGroupUrl.Add(x.GroupUrl); });

            return listGroupUrl;
        }

        public async Task<List<string>> GetPageOrBoardUrls(string accountId, string accountName)
        {
            var listPageUrl = new List<string>();
            //DbAccountService objDbAccountService = new DbAccountService(accountId);

            var objDbAccountService =
                InstanceProvider.ResolveAccountDbOperations(accountId, SocialNetworks.Facebook);

            var pages = await objDbAccountService.GetAsync<OwnPages>();

            pages?.ForEach(x => { listPageUrl.Add(x.PageUrl); });

            var likedPages = await objDbAccountService.GetAsync<LikedPages>();

            likedPages?.ForEach(x => { listPageUrl.Add(x.PageUrl); });

            return listPageUrl;
        }

        public async Task<List<string>> GetGroupUrls(string accountId, DateTime addedAfter)
        {
            var listGroupUrl = new List<string>();
            //DbAccountService objDbAccountService = new DbAccountService(accountId);

            var objDbAccountService =
                InstanceProvider.ResolveAccountDbOperations(accountId, SocialNetworks.Facebook);

            var groups = await objDbAccountService.GetAsync<OwnGroups>(x => x.InteractionDate > addedAfter);

            groups?.ForEach(x => { listGroupUrl.Add(x.GroupUrl); });

            return listGroupUrl;
        }

        public async Task<List<AccountDetailsSelectorModel>> GetFriendsDetails(string accountId, string accountName,
            List<string> alreadySelectedList)
        {
            var listFriendUrls = new List<AccountDetailsSelectorModel>();
            // DbAccountService objDbAccountService = new DbAccountService(accountId);
            var objDbAccountService =
                InstanceProvider.ResolveAccountDbOperations(accountId, SocialNetworks.Facebook);

            var friends = await objDbAccountService.GetAsync<Friends>();

            friends.ForEach(x =>
            {
                var accountDetailsSelectorModel = new AccountDetailsSelectorModel
                {
                    AccountId = accountId,
                    AccountName = accountName,
                    DetailName = x.FullName,
                    DetailUrl = $"{FdConstants.FbHomeUrl}{x.FriendId}",
                    IsSelected = alreadySelectedList.Contains($"{FdConstants.FbHomeUrl}{x.FriendId}"),
                    IsFanpage = false
                };
                listFriendUrls.Add(accountDetailsSelectorModel);
            });

            return listFriendUrls;
        }

        public bool IsGroupsAvailables { get; set; } = true;

        public string DisplayAsPageOrBoards { get; set; } = "Page";
    }
}