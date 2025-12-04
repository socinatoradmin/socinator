using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDLibrary.PuppeteerBrowser;
using FaceDominatorCore.FDResponse.AccountsResponse;
using FaceDominatorCore.FDResponse.CommonResponse;
using FaceDominatorCore.FDResponse.ScrapersResponse;
using FaceDominatorCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ThreadUtils;
using Unity;

namespace FaceDominatorCore.FDLibrary.FdProcesses
{

    public interface IFdUpdateAccountProcess
    {
        Task<int> CustomUpdateFriends(DominatorAccountModel objDominatorAccountModel, CancellationToken token, IFdBrowserManager BrowserManager);

        Task<int> StartUpdateFriends(DominatorAccountModel objDominatorAccountModel, CancellationToken token, CancellationTokenSource source = null);

        Task UpdateGroupsNew(DominatorAccountModel objDominatorAccountModel, CancellationToken token);

        Task UpdateGroupsFromBrowser(DominatorAccountModel objDominatorAccountModel
            , CancellationToken token, IFdBrowserManager browserManager);
    }

    public class FdUpdateAccountProcess : IFdUpdateAccountProcess
    {

        public DominatorAccountModel Account { get; set; }


        public CancellationToken Token { get; set; }
        public CancellationTokenSource tokenSource { get; set; }

        private IFdRequestLibrary _fdRequestLibrary;

        private IFdBrowserManager BrowserManager;

        private readonly IAccountScopeFactory _accountScopeFactory;
        private readonly IDelayService _delayService;

        private bool friendsCoutUpdated;

        public FdUpdateAccountProcess()
        {

        }

        public FdUpdateAccountProcess(DominatorAccountModel account, IAccountScopeFactory accountScopeFactory)
        {
            _accountScopeFactory = accountScopeFactory;
            Account = account ?? new DominatorAccountModel();
            var accountId = account.AccountId == null ? "0" : account.AccountId;
            _fdRequestLibrary = accountScopeFactory[accountId].Resolve<IFdRequestLibrary>();
            Token = account == null ? new CancellationToken() : account.Token;
            tokenSource = account == null ? new CancellationTokenSource() : account.CancellationSource;
            BrowserManager = _accountScopeFactory[$"{accountId}"].Resolve<IBrowserManagerFactory>().FdBrowserManager(account, Token) != null ? _accountScopeFactory[$"{accountId}"].Resolve<IBrowserManagerFactory>().FdBrowserManager(account, Token) : _accountScopeFactory[accountId].Resolve<IFdBrowserManager>();
            _delayService = InstanceProvider.GetInstance<IDelayService>();
        }

        public async Task UpdateAllUserDetails(ActionBlock<FdUpdateAccountProcess> adsActionBlock)
        {
            try
            {
                FdConstants.GetIndexGlobalFdDir();

                FdConstants.DeleteAccountDetailsBin();

                GlobusLogHelper.log.Info(Log.UpdatingDetails, Account.AccountBaseModel.AccountNetwork,
                    Account.AccountBaseModel.UserName, "LangKeyAccountDetails".FromResourceDictionary());


                ////Changed
                //_fdLoginProcess.RequestParameterInitialized(Account);

                //if (Account.IsRunProcessThroughBrowser)
                //    BrowserManager.BrowserLogin(Account, Account.Token);

                if (Account.IsRunProcessThroughBrowser)
                    BrowserManager.ChangeLanguage(Account, FdConstants.FdDefaultLanguage);
                else
                {
                    _fdRequestLibrary.GetLangugae(Account);
                    _fdRequestLibrary.ChangeLanguage(Account, FdConstants.FdDefaultLanguage);
                }

                await UpdateAccountInfo(Account, Token);

                await UpdateOwnPages(Account, Token);

                await UpdateLikeedPages(Account, Token);

                await UpdateGroupsNew(Account, Token);

                await StartUpdateFriends(Account, Token, tokenSource);
                GlobusLogHelper.log.Info(Log.DetailsUpdated, Account.AccountBaseModel.AccountNetwork,
                    Account.AccountBaseModel.UserName, "LangKeyAccountDetails".FromResourceDictionary());

                if (Account.DisplayColumnValue1 != null)
                    if (Account.DisplayColumnValue2 != null)
                        if (Account.DisplayColumnValue3 != null)
                            FdAccountUpdateFactory.Instance.AddToDailyGrowth(Account.AccountId,
                                Account.DisplayColumnValue1.Value,
                                Account.DisplayColumnValue2.Value,
                                Account.DisplayColumnValue3.Value);


            }
            catch (ArgumentException ex)
            {
                ex.DebugLog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally
            {
                Account.AccountBaseModel.NeedToCloseBrowser = true;
                await Task.Run(() => BrowserManager.CloseBrowser(Account));
            }

        }

        public async Task UpdateGroupsNew(DominatorAccountModel objDominatorAccountModel, CancellationToken token)
        {
            try
            {
                if (string.IsNullOrEmpty(_fdRequestLibrary.SessionId))
                    _fdRequestLibrary = _accountScopeFactory[objDominatorAccountModel.AccountId].Resolve<IFdRequestLibrary>();

                GlobusLogHelper.log.Info(Log.UpdatingDetails, objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                    objDominatorAccountModel.AccountBaseModel.UserName, "Groups");
                FdFunctions.FdFunctions objFdFunctions = new FdFunctions.FdFunctions(objDominatorAccountModel);
                int groupCount = 0;
                IResponseHandler objResponseHandler = null;
                if (Account.IsRunProcessThroughBrowser)
                {
                    objResponseHandler = await BrowserManager.UpdateGroupsAsync(objDominatorAccountModel, token);
                    if (!int.TryParse(objResponseHandler?.ObjFdScraperResponseParameters?.TotalCount, out groupCount))
                        groupCount = BrowserManager.GetGroupsCount(Account);
                    if (objResponseHandler?.ObjFdScraperResponseParameters?.ListGroup.Count == 0 && groupCount != 0)
                    {
                        _fdRequestLibrary.SetCoockies(objDominatorAccountModel);

                        objResponseHandler = await _fdRequestLibrary.ScrapGroupAsync(objDominatorAccountModel, null, GroupAccessType.Admin.ToString(), token);

                        var responseHandlerNonAdmin = await _fdRequestLibrary.ScrapGroupAsync(objDominatorAccountModel, null, GroupAccessType.NonAdmin.ToString(), token);

                        objResponseHandler?.ObjFdScraperResponseParameters?.ListGroup?.AddRange(responseHandlerNonAdmin?.ObjFdScraperResponseParameters?.ListGroup);
                    }

                }
                else
                {
                    objResponseHandler = await _fdRequestLibrary.ScrapGroupAsync(objDominatorAccountModel, null, GroupAccessType.Admin.ToString(), token);

                    var responseHandlerNonAdmin = await _fdRequestLibrary.ScrapGroupAsync(objDominatorAccountModel, null, GroupAccessType.NonAdmin.ToString(), token);

                    objResponseHandler.ObjFdScraperResponseParameters.ListGroup.AddRange(responseHandlerNonAdmin.ObjFdScraperResponseParameters.ListGroup);
                }
                objFdFunctions.UpdateGroupCountToAccountModel(objDominatorAccountModel, groupCount, false);
                if (objResponseHandler?.ObjFdScraperResponseParameters != null)
                {
                    await objFdFunctions.SaveGroupsDetail(objDominatorAccountModel, objResponseHandler?.ObjFdScraperResponseParameters?.ListGroup);
                    if (groupCount == objResponseHandler?.ObjFdScraperResponseParameters?.ListGroup.Count || groupCount == 0)
                        objFdFunctions.UpdateGroupCountToAccountModelNew(objDominatorAccountModel);
                    int previousCount = 0;

                    while (!string.IsNullOrEmpty(objResponseHandler?.PageletData))
                    {
                        try
                        {
                            objResponseHandler = await _fdRequestLibrary.ScrapGroupsNewAsync(objDominatorAccountModel, (GroupScraperResponseHandlerNew)objResponseHandler, token);

                            //Call the method to store Frineds detlailss in db

                            if (previousCount == objResponseHandler?.ObjFdScraperResponseParameters?.ListGroup.Count)
                                break;

                            await objFdFunctions.SaveGroupsDetail(objDominatorAccountModel, objResponseHandler?.ObjFdScraperResponseParameters?.ListGroup);
                            objFdFunctions.UpdateGroupCountToAccountModelNew(objDominatorAccountModel);

                            previousCount = objResponseHandler.ObjFdScraperResponseParameters.ListGroup.Count;
                        }
                        catch (ArgumentException ex)
                        {
                            ex.DebugLog();
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                        Task.Delay(2000).Wait(token);
                    }

                    var status = objFdFunctions.DeleteUnnecessaryGroupDetails(objResponseHandler.ObjFdScraperResponseParameters.ListGroup);
                    if (status)
                        GlobusLogHelper.log.Info(Log.CustomMessage, objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                   objDominatorAccountModel.AccountBaseModel.UserName, "Groups Status", "Unnecessary Groups Deleted...");
                    if (groupCount == objResponseHandler?.ObjFdScraperResponseParameters?.ListGroup.Count || groupCount == 0)
                        objFdFunctions.UpdateGroupCountToAccountModelNew(objDominatorAccountModel);
                }

                GlobusLogHelper.log.Info(Log.DetailsUpdated, objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                    objDominatorAccountModel.AccountBaseModel.UserName, "Groups");

            }
            catch (ArgumentException ex)
            {
                ex.DebugLog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private async Task UpdateAccountInfo(DominatorAccountModel objDominatorAccountModel, CancellationToken token)
        {
            try
            {

                GlobusLogHelper.log.Info(Log.UpdatingDetails, objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                    objDominatorAccountModel.AccountBaseModel.UserName, "AccountInfo");

                FacebookUser objFacebookUser = FdConstants.getFaceBookUserFromUrlOrIdOrUserName(objDominatorAccountModel.AccountBaseModel.UserId);

                token.ThrowIfCancellationRequested();

                IResponseHandler objResponseHandler = BrowserManager.GetFullUserDetails(Account, objFacebookUser, closeBrowser: false);


                FdFunctions.FdFunctions objFdFunctions = new FdFunctions.FdFunctions();

                //await FdFunctions.FdFunctions.DownLoadMediaFromUrlAsync(objResponseHandler.ObjFdScraperResponseParameters.FacebookUser.ProfilePicUrl,
                //$@"{FdConstants.GetPlatformBaseDirectory()}\{ConstantVariable.ApplicationName}\Fd\ProfilePic\{Account.AccountBaseModel.UserName}.jpg",
                //$@"{FdConstants.GetPlatformBaseDirectory()}\{ConstantVariable.ApplicationName}\Fd\ProfilePic");

                objFdFunctions.UpdateAccountInfoToModel(objDominatorAccountModel, objResponseHandler?.ObjFdScraperResponseParameters?.FacebookUser);

                token.ThrowIfCancellationRequested();

                GlobusLogHelper.log.Info(Log.DetailsUpdated, objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                    objDominatorAccountModel.AccountBaseModel.UserName, "AccountInfo");
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (ArgumentException ex)
            {
                ex.DebugLog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public async Task<int> StartUpdateFriends(DominatorAccountModel objDominatorAccountModel, CancellationToken token, CancellationTokenSource source = null)
        {
            try
            {
                if (string.IsNullOrEmpty(_fdRequestLibrary.SessionId))
                    _fdRequestLibrary = _accountScopeFactory[objDominatorAccountModel.AccountId]
                        .Resolve<IFdRequestLibrary>();

                List<string> lstPageId = new List<string>();
                token.ThrowIfCancellationRequested();

                FdFunctions.FdFunctions objFdFunctions = new FdFunctions.FdFunctions(objDominatorAccountModel);
                var lstPageDetails = objFdFunctions.GetPageDetails();

                if (lstPageDetails.Count != 0)
                    lstPageId = lstPageDetails.Select(x => string.IsNullOrEmpty(x.PageId) ? x.PageUrl : x.PageId).ToList();

                GlobusLogHelper.log.Info(Log.UpdatingDetails, objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                    objDominatorAccountModel.AccountBaseModel.UserName, "Friends");

                await _delayService.DelayAsync(1000);

                await UpdateFriendsCount(objDominatorAccountModel, token, tokenSource);

                token.ThrowIfCancellationRequested();

                GlobusLogHelper.log.Info(Log.DetailsUpdated, objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                    objDominatorAccountModel.AccountBaseModel.UserName, "Friends");

                Task.Delay(5000).Wait(token);

                return lstPageDetails.Count;
            }
            catch (OperationCanceledException)
            {
                return 0;
            }
            catch (ArgumentException ex)
            {
                ex.DebugLog();
                return 0;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return 0;
            }
        }


        public async Task<bool> UpdateFriendsCount(DominatorAccountModel objDominatorAccountModel, CancellationToken token, CancellationTokenSource source = null)
        {
            try
            {

                List<FacebookUser> listFacebookUser = new List<FacebookUser>();

                IResponseHandler objResponseHandler = null;

                int friendsCount = 0;
                if (Account.IsRunProcessThroughBrowser)
                    BrowserManager.SearchByFriendRequests(Account, FbEntityType.Friends);
                FdFunctions.FdFunctions objFdFunctions = new FdFunctions.FdFunctions(objDominatorAccountModel);
                friendsCount = BrowserManager.GetfriendsCount(Account);
                if (friendsCount >= 2500)
                {
                    var fdPupBrowseractivity = new FdPupBrowserRequest(objDominatorAccountModel, source ?? new CancellationTokenSource());
                    try
                    {
                        await fdPupBrowseractivity.FacebookHomePageLogin(objDominatorAccountModel, token);
                        fdPupBrowseractivity.BrowserWindow.CloseUnWantedTabs(fdPupBrowseractivity.BrowserWindow.TargetUrl, fdPupBrowseractivity.CancellationToken);
                        fdPupBrowseractivity.SearchByFriendRequests(Account, FbEntityType.Friends);
                        objResponseHandler = fdPupBrowseractivity.ScrollWindowAndGetFriends(Account,
                                    FbEntityType.Friends, 350, 0, FdConstants.AddedFriend3, FdConstants.SentFriendPaginationElement);
                        token.ThrowIfCancellationRequested();
                    }
                    catch (OperationCanceledException ex)
                    {
                        ex.DebugLog();
                    }
                    catch (AggregateException ex)
                    {
                        ex.DebugLog();
                    }
                    catch (Exception e)
                    { e.DebugLog(); }
                    finally
                    {
                        if (fdPupBrowseractivity.BrowserWindow != null)
                            fdPupBrowseractivity.BrowserWindow.ClosedBrowser();
                    }
                }
                else
                    objResponseHandler = BrowserManager.ScrollWindowAndGetFriends(Account,
                                    FbEntityType.Friends, 150, 0, FdConstants.AddedFriend3, FdConstants.SentFriendPaginationElement);
                objFdFunctions.UpdateFriendCountToAccountModelNewUI(objDominatorAccountModel, friendsCount, false);
                if (objResponseHandler?.ObjFdScraperResponseParameters != null)
                {
                    listFacebookUser.AddRange(objResponseHandler.ObjFdScraperResponseParameters.ListUser);
                    Token.ThrowIfCancellationRequested();
                    await objFdFunctions.SaveOrUpdateFriendDetailsAsync(Account, listFacebookUser);
                    if (listFacebookUser.Count >= friendsCount || friendsCount == 0)
                        objFdFunctions.UpdateFriendCountToAccountModel(Account, listFacebookUser.Count, false);
                    await objFdFunctions.SaveFriendDetailsToBin(Account, listFacebookUser);
                    objFdFunctions.DeleteRemovedFriends(Account, listFacebookUser);
                }
                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (ArgumentException ex)
            {
                ex.DebugLog();
                return false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }

        public async Task<bool> UpdateFriendsNewAsync(DominatorAccountModel objDominatorAccountModel, CancellationToken token)
        {
            try
            {
                int friendsCount = 0;
                List<FacebookUser> listFacebookUser = new List<FacebookUser>();

                FdFunctions.FdFunctions objFdFunctions = new FdFunctions.FdFunctions(objDominatorAccountModel);

                IResponseHandler objResponseHandler = null;

                if (objDominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    if(BrowserManager.BrowserWindow==null||(BrowserManager.BrowserWindow!=null&&!BrowserManager.BrowserWindow.IsLoggedIn))
                    BrowserManager.BrowserLogin(objDominatorAccountModel, objDominatorAccountModel.Token);

                    BrowserManager.SearchByFriendRequests(Account, FbEntityType.Friends);
                    friendsCount = BrowserManager.GetfriendsCount(objDominatorAccountModel);
                    objResponseHandler = BrowserManager.ScrollWindowAndGetFriends(objDominatorAccountModel,
                            FbEntityType.Friends, 500, 0, FdConstants.AddedFriend3, FdConstants.SentFriendPaginationElement);

                }
                else
                    objResponseHandler = await _fdRequestLibrary.UpdateFriendsNew(objDominatorAccountModel, null, Token, false);

                //if (objResponseHandler != null)
                //    listFacebookUser.AddRange(objResponseHandler.ObjFdScraperResponseParameters.ListUser);

                Token.ThrowIfCancellationRequested();
                objFdFunctions.UpdateFriendCountToAccountModelNewUI(objDominatorAccountModel, friendsCount, false);
                if (objResponseHandler != null && objResponseHandler?.ObjFdScraperResponseParameters != null && objResponseHandler.ObjFdScraperResponseParameters.ListUser != null)
                {
                    var unique = listFacebookUser.Except(objResponseHandler.ObjFdScraperResponseParameters.ListUser).Union(objResponseHandler.ObjFdScraperResponseParameters.ListUser.Except(listFacebookUser)).ToList();
                    listFacebookUser.AddRange(unique);
                    Token.ThrowIfCancellationRequested();
                    await objFdFunctions.SaveOrUpdateFriendDetailsAsync(objDominatorAccountModel, listFacebookUser);
                    if (listFacebookUser.Count >= friendsCount || friendsCount == 0)
                        objFdFunctions.UpdateFriendCountToAccountModel(objDominatorAccountModel, listFacebookUser.Count, false);
                    await objFdFunctions.SaveFriendDetailsToBin(objDominatorAccountModel, listFacebookUser);
                    objFdFunctions.DeleteRemovedFriends(objDominatorAccountModel, listFacebookUser);
                }

                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (ArgumentException ex)
            {
                ex.DebugLog();
                return false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }

        public async Task<bool> UpdateFriendsNewAsync(ActionBlock<FdUpdateAccountProcess> adsActionBlock)
        {

            try
            {
                await Task.Delay(TimeSpan.FromMinutes(new Random().Next(15, 20)));

                var isLastPage = false;

                int friendsCount = 0;

                bool isFriendsUpdated = false;

                List<FacebookUser> listFacebookUser = new List<FacebookUser>();

                FdFunctions.FdFunctions objFdFunctions = new FdFunctions.FdFunctions(Account);

                IResponseHandler objResponseHandler = null;

                if (Account.IsRunProcessThroughBrowser)
                {
                    BrowserManager.BrowserLogin(Account, Account.Token);

                    BrowserManager.SearchByFriendRequests(Account, FbEntityType.Friends);

                    friendsCount = BrowserManager.GetfriendsCount(Account);
                    objResponseHandler = BrowserManager.ScrollWindowAndGetFriends(Account,
                            FbEntityType.AddedFriends, 500, 0, FdConstants.AddedFriend2, FdConstants.SentFriendPaginationElement);

                    BrowserManager.CloseBrowser(Account);

                }
                else
                    objResponseHandler = await _fdRequestLibrary.UpdateFriendsNew(Account, null, Token, false);

                if (objResponseHandler != null)
                {
                    if (objResponseHandler.ObjFdScraperResponseParameters.ListUser.Count > 0)
                        isFriendsUpdated = true;

                    listFacebookUser.AddRange(objResponseHandler.ObjFdScraperResponseParameters.ListUser);

                }

                while (objResponseHandler != null && !string.IsNullOrEmpty(objResponseHandler.ObjFdScraperResponseParameters.FriendsPager?.Data))
                {
                    try
                    {
                        if (!isFriendsUpdated && objResponseHandler.ObjFdScraperResponseParameters.ListUser.Count > 0)
                            isFriendsUpdated = true;

                        Token.ThrowIfCancellationRequested();

                        objResponseHandler = await _fdRequestLibrary.UpdateFriendsNew(Account,
                            (FdFriendsInfoNewResponseHandler)objResponseHandler, Token, isLastPage);
                        // Call the method to store Frineds detlas in db

                        if (objResponseHandler == null)
                            break;

                        listFacebookUser.AddRange(objResponseHandler.ObjFdScraperResponseParameters.ListUser);

                        if (objResponseHandler.ObjFdScraperResponseParameters.FriendsPager.MaxDataKey ==
                            objResponseHandler.ObjFdScraperResponseParameters.FriendsPager.CurrentDataKey)
                            isLastPage = true;

                        if (isLastPage && objResponseHandler.ObjFdScraperResponseParameters.ListUser.Count == 0)
                            break;

                        await _delayService.DelayAsync(1000);
                    }
                    catch (ArgumentException ex)
                    {
                        ex.DebugLog();
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                }

                Token.ThrowIfCancellationRequested();
                await objFdFunctions.SaveOrUpdateFriendDetailsAsync(Account, listFacebookUser);
                if (friendsCount == listFacebookUser.Count || friendsCount == 0)
                    objFdFunctions.UpdateFriendCountToAccountModel(Account, listFacebookUser.Count, false);
                await objFdFunctions.SaveFriendDetailsToBin(Account, listFacebookUser);
                objFdFunctions.DeleteRemovedFriends(Account, listFacebookUser);

                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (ArgumentException ex)
            {
                ex.DebugLog();
                return false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }

        private async Task UpdateFriendsFromPages(DominatorAccountModel objDominatorAccountModel, CancellationToken token, List<string> lstPageId)
        {
            try
            {

                List<FacebookUser> listFacebookUser = new List<FacebookUser>();

                FdFunctions.FdFunctions objFdFunctions = new FdFunctions.FdFunctions(objDominatorAccountModel);

                IResponseHandler objResponseHandler = objDominatorAccountModel.IsRunProcessThroughBrowser
                                    ? BrowserManager.UpdateFriendsDetailsFromPages(objDominatorAccountModel, lstPageId)
                                    : await _fdRequestLibrary.UpdateFriendsFromPage(objDominatorAccountModel, null, token, lstPageId);

                Task.Delay(5000).Wait(token);

                objResponseHandler.ObjFdScraperResponseParameters.ListUser.RemoveAll(x => x.UserId == objDominatorAccountModel.AccountBaseModel.UserId);

                if (objResponseHandler != null)
                    listFacebookUser.AddRange(objResponseHandler.ObjFdScraperResponseParameters.ListUser);

                friendsCoutUpdated = objResponseHandler.ObjFdScraperResponseParameters.ListUser.Count > 0;

                objFdFunctions.DeleteRemovedFriends(objDominatorAccountModel, listFacebookUser);
                await objFdFunctions.SaveFriendDetailsNew(objDominatorAccountModel, listFacebookUser);
                objFdFunctions.UpdateFriendCountToAccountModel(objDominatorAccountModel, listFacebookUser.Count, true);
            }
            catch (ArgumentException ex)
            {
                ex.DebugLog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private async Task UpdateOwnPages(DominatorAccountModel objDominatorAccountModel, CancellationToken token)
        {

            try
            {
                FacebookUser objFacebookUser = FdConstants.getFaceBookUserFromUrlOrIdOrUserName(objDominatorAccountModel.AccountBaseModel?.UserId);
                IResponseHandler objResponseHandler = Account.IsRunProcessThroughBrowser
                                    ? BrowserManager.UpdateOwnPagesDetails(Account, objFacebookUser)
                                    : await _fdRequestLibrary.UpdateOwnPagesAsync(objDominatorAccountModel, null, token);
                FdFunctions.FdFunctions objFdFunctions = new FdFunctions.FdFunctions(objDominatorAccountModel);

                while (!string.IsNullOrEmpty(objResponseHandler.PageletData))
                {
                    objResponseHandler =
                        await _fdRequestLibrary.UpdateOwnPagesAsync(objDominatorAccountModel, (SearchOwnPageResponseHandler)objResponseHandler,
                            token);
                    // Call the method to store Frineds detlas in db

                }

                foreach (FanpageDetails objFanpageDetails in objResponseHandler.ObjFdScraperResponseParameters.ListPage)
                {
                    IResponseHandler objFanpageScraperResponseHandler = Account.IsRunProcessThroughBrowser
                        ? BrowserManager.GetFullPageDetails(Account, objFanpageDetails, false, false)
                        : _fdRequestLibrary.GetFanpageDetails(objDominatorAccountModel, objFanpageDetails);
                    objFdFunctions.SaveOwnPageDetails(objDominatorAccountModel, (FanpageScraperResponseHandler)objFanpageScraperResponseHandler);
                }

            }
            catch (ArgumentException ex)
            {
                ex.DebugLog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private async Task UpdateLikeedPages(DominatorAccountModel objDominatorAccountModel, CancellationToken token)
        {
            try
            {

                GlobusLogHelper.log.Info(Log.UpdatingDetails, objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                    objDominatorAccountModel.AccountBaseModel.UserName, "FanPages");

                if (Account.IsRunProcessThroughBrowser)
                    BrowserManager.SearchFanpageLikedByFriends(Account, FbEntityType.Fanpage, $"{FdConstants.FbHomeUrl}pages/?category=your_pages&ref=bookmarks");

                var responseHandler = Account.IsRunProcessThroughBrowser
                    ? BrowserManager.ScrollWindowAndGetData(Account, FbEntityType.Fanpage, 100, 0, FdConstants.FriendsFanpageLikes3Element)
                    : await _fdRequestLibrary.UpdateLikedPagesAsync(objDominatorAccountModel, null, token, true);

                FdFunctions.FdFunctions objFdFunctions = new FdFunctions.FdFunctions(objDominatorAccountModel);

                var lstPageId = objFdFunctions.GetPageDetails();

                await objFdFunctions.SaveLikedPageDetails(responseHandler, lstPageId);

                token.ThrowIfCancellationRequested();

                objFdFunctions.UpdatePageCountToAccountModel(objDominatorAccountModel, responseHandler.ObjFdScraperResponseParameters.ListPage.Count, true);

                while (!string.IsNullOrEmpty(responseHandler.PageletData))
                {
                    try
                    {
                        responseHandler =
                            await _fdRequestLibrary.UpdateLikedPagesAsync(objDominatorAccountModel,
                                responseHandler, token);

                        await objFdFunctions.SaveLikedPageDetails(responseHandler, lstPageId);

                        objFdFunctions.UpdatePageCountToAccountModel(objDominatorAccountModel,
                            responseHandler.ObjFdScraperResponseParameters.ListPage.Count, false);

                        token.ThrowIfCancellationRequested();
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (ArgumentException ex)
                    {
                        ex.DebugLog();
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }

                token.ThrowIfCancellationRequested();

                GlobusLogHelper.log.Info(Log.DetailsUpdated, objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                    objDominatorAccountModel.AccountBaseModel.UserName, "FanPages");
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public async Task UpdateGroupsFromBrowser(DominatorAccountModel objDominatorAccountModel
            , CancellationToken token, IFdBrowserManager browserManager)
        {
            try
            {

                if (string.IsNullOrEmpty(_fdRequestLibrary.SessionId))
                    _fdRequestLibrary = _accountScopeFactory[objDominatorAccountModel.AccountId].Resolve<IFdRequestLibrary>();

                GlobusLogHelper.log.Info(Log.UpdatingDetails, objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                    objDominatorAccountModel.AccountBaseModel.UserName, "Groups");
                FdFunctions.FdFunctions objFdFunctions = new FdFunctions.FdFunctions(objDominatorAccountModel);

                IResponseHandler objResponseHandler = null;
                if (objDominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    objResponseHandler = await browserManager.UpdateGroupsAsync(objDominatorAccountModel, token);

                    if (objResponseHandler?.ObjFdScraperResponseParameters?.ListGroup.Count <= 0)
                    {
                        objResponseHandler = await _fdRequestLibrary.ScrapGroupAsync(objDominatorAccountModel, null, GroupAccessType.Admin.ToString(), token);

                        var responseHandlerNonAdmin = await _fdRequestLibrary.ScrapGroupAsync(objDominatorAccountModel, null, GroupAccessType.NonAdmin.ToString(), token);

                        objResponseHandler.ObjFdScraperResponseParameters.ListGroup.AddRange(responseHandlerNonAdmin.ObjFdScraperResponseParameters.ListGroup);
                    }

                }
                else
                {
                    objResponseHandler = await _fdRequestLibrary.ScrapGroupAsync(objDominatorAccountModel, null, GroupAccessType.Admin.ToString(), token);

                    var responseHandlerNonAdmin = await _fdRequestLibrary.ScrapGroupAsync(objDominatorAccountModel, null, GroupAccessType.NonAdmin.ToString(), token);

                    objResponseHandler.ObjFdScraperResponseParameters.ListGroup.AddRange(responseHandlerNonAdmin.ObjFdScraperResponseParameters.ListGroup);
                }
                if (objResponseHandler?.ObjFdScraperResponseParameters != null)
                {
                    var groupsList = objResponseHandler?.ObjFdScraperResponseParameters?.ListGroup;
                    foreach (var group in groupsList)
                    {
                        if (string.IsNullOrEmpty(group.GroupType))
                        {
                            var groupInfo = browserManager.GroupDetails(group);
                        }
                    }

                    objResponseHandler.ObjFdScraperResponseParameters.ListGroup = groupsList;

                    await objFdFunctions.SaveGroupsDetail(objDominatorAccountModel, objResponseHandler.ObjFdScraperResponseParameters.ListGroup);
                    objFdFunctions.UpdateGroupCountToAccountModelNew(objDominatorAccountModel);
                    int previousCount = 0;

                    while (!string.IsNullOrEmpty(objResponseHandler.PageletData))
                    {
                        try
                        {
                            objResponseHandler = await _fdRequestLibrary.ScrapGroupsNewAsync(objDominatorAccountModel, (GroupScraperResponseHandlerNew)objResponseHandler, token);

                            //Call the method to store Frineds detlailss in db

                            if (previousCount == objResponseHandler.ObjFdScraperResponseParameters.ListGroup.Count)
                                break;

                            await objFdFunctions.SaveGroupsDetail(objDominatorAccountModel, objResponseHandler.ObjFdScraperResponseParameters.ListGroup);
                            objFdFunctions.UpdateGroupCountToAccountModelNew(objDominatorAccountModel);

                            previousCount = objResponseHandler.ObjFdScraperResponseParameters.ListGroup.Count;
                        }
                        catch (ArgumentException ex)
                        {
                            ex.DebugLog();
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                        Task.Delay(2000).Wait(token);
                    }

                    var status = objFdFunctions.DeleteUnnecessaryGroupDetails(objResponseHandler?.ObjFdScraperResponseParameters?.ListGroup);
                    if (status)
                        GlobusLogHelper.log.Info(Log.CustomMessage, objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                   objDominatorAccountModel.AccountBaseModel.UserName, "Groups Status", "Unnecessary Groups Deleted...");
                }
                objFdFunctions.UpdateGroupCountToAccountModelNew(objDominatorAccountModel);
                GlobusLogHelper.log.Info(Log.DetailsUpdated, objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                    objDominatorAccountModel.AccountBaseModel.UserName, "Groups");

            }
            catch (ArgumentException ex)
            {
                ex.DebugLog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        public async Task<int> CustomUpdateFriends(DominatorAccountModel objDominatorAccountModel, CancellationToken token, IFdBrowserManager browserManager)
        {
            try
            {

                if (string.IsNullOrEmpty(_fdRequestLibrary.SessionId))
                    _fdRequestLibrary = _accountScopeFactory[objDominatorAccountModel.AccountId]
                        .Resolve<IFdRequestLibrary>();

                List<string> lstPageId = new List<string>();
                token.ThrowIfCancellationRequested();

                FdFunctions.FdFunctions objFdFunctions = new FdFunctions.FdFunctions(objDominatorAccountModel);
                var lstPageDetails = objFdFunctions.GetPageDetails();

                if (lstPageDetails.Count != 0)
                    lstPageId = lstPageDetails.Select(x => x.PageId).ToList();

                GlobusLogHelper.log.Info(Log.UpdatingDetails, objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                    objDominatorAccountModel.AccountBaseModel.UserName, "Friends");

                await Task.Delay(1000);
                BrowserManager = browserManager;

                await UpdateFriendsNewAsync(objDominatorAccountModel, token);

                token.ThrowIfCancellationRequested();

                GlobusLogHelper.log.Info(Log.DetailsUpdated, objDominatorAccountModel.AccountBaseModel.AccountNetwork,
                    objDominatorAccountModel.AccountBaseModel.UserName, "Friends");

                return lstPageDetails.Count;
            }
            catch (OperationCanceledException)
            {
                return 0;
            }
            catch (ArgumentException ex)
            {
                ex.DebugLog();
                return 0;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return 0;
            }
        }


    }
}



//public async Task UpdateGoupsDetail(DominatorAccountModel objDominatorAccountModel, CancellationToken token)
//{
//    FdFunctions.FdFunctions objFdFunctions = new FdFunctions.FdFunctions(objDominatorAccountModel);

//    GroupDetailsResponseHandler groupDetailsResponseHandler = await ObjFdRequestLibrary.GetGroupDetails(objDominatorAccountModel, token);

//    await objFdFunctions.SaveGroupsDetail(objDominatorAccountModel, groupDetailsResponseHandler.ListGroupDetails);

//    objFdFunctions.UpdateGroupCountToAccountModelNew(objDominatorAccountModel);
//    // List<GroupDetails> groupList = groupDetailsResponseHandler.ListGroupDetails;
//}

/*
        public async Task UpdateGroupDetails(DominatorAccountModel objDominatorAccountModel, CancellationToken token)
        {
            FdFunctions.FdFunctions objFdFunctions = new FdFunctions.FdFunctions(objDominatorAccountModel);

            GroupDateDetailsResponseHandler groupDateDetailsResponseHandler = await ObjFdRequestLibrary.GetGroupDateDetails(objDominatorAccountModel, token);

            objFdFunctions.SaveGroupsDetail(objDominatorAccountModel, groupDateDetailsResponseHandler.ListGroupDetails);
        }
*/
//        DominatorAccountModel AccountModel { get;set;}
//        public GeneralModel GeneralSettingsModel { get; set; }
//        private FacebookModel AdvanceModel { get; set; }
//        public CancellationTokenSource CampaignCancellationToken { get; set; }

//private async Task UpdateGroups(DominatorAccountModel objDominatorAccountModel, CancellationToken token)
//{
//    try
//    {
//        GlobusLogHelper.log.Info(Log.UpdatingDetails, objDominatorAccountModel.AccountBaseModel.AccountNetwork, objDominatorAccountModel.AccountBaseModel.UserName, "Groups");

//        FdFunctions.FdFunctions objFdFunctions = new FdFunctions.FdFunctions(objDominatorAccountModel);

//        GroupScraperResponseHandler objResponseHandler = await ObjFdRequestLibrary.ScrapGroupsAsync(objDominatorAccountModel, null, token);

//        objFdFunctions.SaveGroupDetails(objDominatorAccountModel, objResponseHandler);

//        while (!string.IsNullOrEmpty(objResponseHandler.PaginationData))
//        {
//            try
//            {
//                objResponseHandler = await ObjFdRequestLibrary.ScrapGroupsAsync(objDominatorAccountModel, objResponseHandler, token);
//                // Call the method to store Frineds detlas in db

//                objFdFunctions.SaveGroupDetails(objDominatorAccountModel, objResponseHandler);
//            }
//            catch (Exception ex)
//            {
//               ex.DebugLog();
//            }


//        }
//        objFdFunctions.UpdateGroupCountToAccountModelNew(objDominatorAccountModel);


//    }
//    catch (Exception ex)
//    {
//        ex.DebugLog();
//    }
//}

// ReSharper disable once UnusedMethodReturnValue.Local
//private bool UpdateFriendsNewSync(DominatorAccountModel objDominatorAccountModel, CancellationToken token)
//{
//    return UpdateFriendsNew(objDominatorAccountModel, token).Result;
//}