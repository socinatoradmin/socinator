using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.AccountSelectorModel;
using FaceDominatorCore.FDRequest;
using FaceDominatorCore.FDResponse.AccountsResponse;
using FaceDominatorCore.FDResponse.ScrapersResponse;
using FaceDominatorCore.Interface;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ThreadUtils;

//using DominatorHouseCore.DatabaseHandler.Utility;

namespace FaceDominatorCore.FDLibrary.FdFunctions
{
    public interface IFdFunctions { }

    public class FdFunctions : IFdFunctions
    {

        DbAccountService DbOperation { get; }

        public static readonly Object FriendsLock = new object();

        public static readonly Object UnFriendsLock = new object();

        public static readonly Object AddFriendsLock = new object();

        public static readonly object TagFriendsLock = new object();

        public static Dictionary<string, string> DictDuplicateFriends = new Dictionary<string, string>();
        private readonly IDelayService _delayService;

        public FdFunctions()
        {
            _delayService = InstanceProvider.GetInstance<IDelayService>();
        }

        public FdFunctions(DominatorAccountModel objDominatorAccountModel)
        {
            _delayService = InstanceProvider.GetInstance<IDelayService>();
            DbOperation = new DbAccountService(objDominatorAccountModel);
        }
        public void ChangeAccountStatus
            (DominatorAccountModel account, FdLoginResponseHandler objFdLoginResponseHandler, IHttpHelper _httpHelper)
        {
            try
            {
                if (account.IsUserLoggedIn)
                {
                    account.AccountBaseModel.Status = AccountStatus.Success;
                    account.Cookies = _httpHelper.GetRequestParameter().Cookies;
                    account.AccountBaseModel.UserId = objFdLoginResponseHandler.UserId;
                    account.SessionId = objFdLoginResponseHandler.FbDtsg;
                }
                else if (objFdLoginResponseHandler.FbErrorDetails.FacebookErrors == FacebookErrors.CheckPoint)
                {
                    account.AccountBaseModel.Status = AccountStatus.NeedsVerification;
                    account.Cookies = _httpHelper.GetRequestParameter().Cookies;
                    account.AccountBaseModel.UserId = objFdLoginResponseHandler.UserId;
                    account.SessionId = objFdLoginResponseHandler.FbDtsg;
                }

                else if (objFdLoginResponseHandler.FbErrorDetails.FacebookErrors == FacebookErrors.AccountDisbled)
                {
                    account.AccountBaseModel.Status = AccountStatus.PermanentlyBlocked;
                    account.Cookies = _httpHelper.GetRequestParameter().Cookies;
                    account.AccountBaseModel.UserId = objFdLoginResponseHandler.UserId;
                    account.SessionId = objFdLoginResponseHandler.FbDtsg;
                }

                else if (objFdLoginResponseHandler.FbErrorDetails.FacebookErrors == FacebookErrors.InvalidLogin)
                {
                    account.AccountBaseModel.Status = AccountStatus.InvalidCredentials;
                    account.Cookies = _httpHelper.GetRequestParameter().Cookies;
                    account.AccountBaseModel.UserId = objFdLoginResponseHandler.UserId;
                    account.SessionId = objFdLoginResponseHandler.FbDtsg;
                }

                else if (objFdLoginResponseHandler.FbErrorDetails.FacebookErrors == FacebookErrors.ProxyNotWorking)
                {
                    account.AccountBaseModel.Status = AccountStatus.ProxyNotWorking;
                    account.Cookies = _httpHelper.GetRequestParameter().Cookies;
                    account.AccountBaseModel.UserId = objFdLoginResponseHandler.UserId;
                    account.SessionId = objFdLoginResponseHandler.FbDtsg;
                }

                SocinatorAccountBuilder.Instance(account.AccountBaseModel.AccountId)
                    .AddOrUpdateDominatorAccountBase(account.AccountBaseModel)
                    .AddOrUpdateCookies(account.Cookies)
                    .SaveToBinFile();

                var globalDb = InstanceProvider.GetInstance<IDbGlobalService>();

                globalDb.UpdateCookies(account);

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            // AccountsFileManager.Edit(account);
        }

        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCD012EFGH456ab01432cde345fgh012ijk3874lmn5415op95624qrst7256uvw7845xyzIJKLM789NOPQ789RST0123UVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string GetPublisherSessionId()
        {
            return $"{RandomString(8)}-{RandomString(4)}-{RandomString(4)}-{RandomString(4)}-{(RandomString(12)).ReplaceAt(0, 1, "d")}";
        }




        public void ChangeAccountCookies(DominatorAccountModel account, IHttpHelper _httpHelper)
        {
            account.Cookies = _httpHelper.GetRequestParameter().Cookies;
            SocinatorAccountBuilder.Instance(account.AccountBaseModel.AccountId)
                .AddOrUpdateDominatorAccountBase(account.AccountBaseModel)
                .AddOrUpdateCookies(account.Cookies)
                .SaveToBinFile();
        }

        public static DateTime GetDateTimeFromSymbols(string str)
        {
            DateTime date = DateTime.Now;
            var tempVal = 0;

            if (!string.IsNullOrEmpty(str))
            {
                if (str.Contains("h"))
                {
                    tempVal = int.Parse(GetIntegerOnlyString(str));
                    date -= TimeSpan.FromHours(tempVal);
                }
                else if (str.Contains("d"))
                {
                    tempVal = int.Parse(GetIntegerOnlyString(str));
                    date -= TimeSpan.FromDays(tempVal);
                }
                else if (str.Contains("w"))
                {
                    tempVal = int.Parse(GetIntegerOnlyString(str));
                    tempVal *= 7;
                    date -= TimeSpan.FromDays(tempVal);
                }
                else if (str.Contains("y"))
                {
                    tempVal = int.Parse(GetIntegerOnlyString(str));
                    tempVal *= 365;
                    date -= TimeSpan.FromDays(tempVal);
                }
            }
            return date;
        }

        public static string ToCamelCase(string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Length > 1)
            {
                return Char.ToLowerInvariant(str[0]) + str.Substring(1);
            }
            return str;
        }

        public FdJsonElement GetJsonElementsForLogin(DominatorAccountModel dominatorAccountModel, FdLoginResponseHandler objFdLoginResponseHandler)
        {
            FdJsonElement objFdJsonElement = new FdJsonElement()
            {
                Lsd = objFdLoginResponseHandler.LoginParameters.Lsd,
                Email = Uri.EscapeDataString(dominatorAccountModel.AccountBaseModel.UserName),
                Password = Uri.EscapeDataString(dominatorAccountModel.AccountBaseModel.Password),
                Timezone = objFdLoginResponseHandler.LoginParameters.Timezone,
                Lgndim = objFdLoginResponseHandler.LoginParameters.Lgndim,
                Lgnrnd = objFdLoginResponseHandler.LoginParameters.Lgnrnd,
                Lgnjs = objFdLoginResponseHandler.LoginParameters.Lgnjs,
                AbTestData = objFdLoginResponseHandler.LoginParameters.AbTestData,
                Locale = objFdLoginResponseHandler.LoginParameters.Locale,
                LoginSource = objFdLoginResponseHandler.LoginParameters.LoginSource,
                PrefillContactPoint = Uri.EscapeDataString(dominatorAccountModel.AccountBaseModel.UserName),
                PrefillSource = objFdLoginResponseHandler.LoginParameters.PrefillSource,
                PrefillType = objFdLoginResponseHandler.LoginParameters.PrefillType,
                Skstamp = objFdLoginResponseHandler.LoginParameters.Skstamp
            };
            return objFdJsonElement;
        }

        public async Task SaveOrUpdateFriendDetailsAsync(DominatorAccountModel objDominatorAccountModel, List<FacebookUser> listFacebookUser)
        {
            try
            {
                var savedGroups = DbOperation.Get<Friends>();
                var friendsToUpdate = savedGroups.Where(x => listFacebookUser.Any(y => y.UserId == x.FriendId)).ToList();

                var friendsToAdd = new List<Friends>();

                foreach (FacebookUser user in listFacebookUser)
                {

                    try
                    {

                        DateTime friendShipDate;
                        var isCorrectDate = DateTime.TryParse(user.InteractionDate, out friendShipDate);

                        if (!isCorrectDate)
                            friendShipDate = DateTime.Now;

                        var currentFriend = friendsToUpdate.FirstOrDefault(x => x.FriendId == user.UserId);

                        if (currentFriend == null)
                        {
                            friendsToAdd.Add(new Friends()
                            {
                                FriendId = user.UserId,
                                FullName = user.Familyname,
                                ProfileUrl = user.ProfileUrl,
                                Location = user.Currentcity,
                                DetailedUserInfo = JsonConvert.SerializeObject(user),
                                InteractionDate = friendShipDate
                            });
                        }
                        else
                        {
                            friendsToUpdate.RemoveAll(x => x.FriendId == user.UserId);
                            currentFriend.FullName = user.Familyname;
                            currentFriend.ProfileUrl = user.ScrapedProfileUrl;
                            currentFriend.InteractionDate = friendShipDate;
                            friendsToUpdate.Add(currentFriend);
                        }

                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }

                try
                {
                    DbOperation.AddRange(friendsToAdd);
                }
                catch (Exception ex)
                {
                    await _delayService.DelayAsync(1000);
                    DbOperation.AddRange(friendsToAdd);
                    ex.DebugLog();
                }

                try
                {
                    DbOperation.UpdateRange(friendsToUpdate);
                }
                catch (Exception ex)
                {
                    await _delayService.DelayAsync(1000);
                    //  DbOperation.UpdateRange(friendsToUpdate);
                    ex.DebugLog();
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }


        }

        public static string GetContactNo(string decodedResponse)
        {
            var phoneNoRegex = @"(([+][(]?[0-9]{1,3}[)]?)|([(]?[0-9]{4}[)]?))\s*[)]?[-\s\.]?[(]?[0-9]{1,3}[)]?([-\s\.]?[0-9]{3})([-\s\.]?[0-9]{3,4})";

            return FdRegexUtility.FirstMatchExtractor(decodedResponse, phoneNoRegex);
        }


        public void RemoveFriendFromBinAfterUnfriend(string friendId, DominatorAccountModel objDominatorAccountModel)
        {
            lock (UnFriendsLock)
            {
                var binFileHelper = InstanceProvider.GetInstance<IBinFileHelper>();

                List<FbFriendDetails> facebookEntityList = binFileHelper.GetFacebookEntity<FbFriendDetails>();

                facebookEntityList.RemoveAll(x => x.AccountName == objDominatorAccountModel.UserName && x.FriendId == friendId);

                binFileHelper.SaveFacebookEntity(facebookEntityList, ConstantVariable.GetFacebookDetailsConfigFile());
            }
        }

        public async Task SaveFriendDetailsNew(DominatorAccountModel objDominatorAccountModel, List<FacebookUser> listFacebookuser)
        {
            try
            {

                var listFriends = DbOperation.Get<Friends>();

                var isSuccess = true;

                listFacebookuser = listFacebookuser.Where(x => listFriends.All(y => y.FriendId != x.UserId)).ToList();

                List<Friends> lstFriends = new List<Friends>();

                if (listFacebookuser.Count > 0)
                {
                    try
                    {
                        lstFriends = (from user in listFacebookuser

                                      select new Friends
                                      {
                                          FriendId = user.UserId,
                                          FullName = user.Familyname,
                                          ProfileUrl = user.ProfileUrl,
                                          Location = user.Currentcity,
                                          DetailedUserInfo = JsonConvert.SerializeObject(user),
                                          InteractionDate = DateTime.Now
                                      }).ToList();

                        if (DbOperation.AddRange(lstFriends))
                        {
                            isSuccess = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                        isSuccess = false;
                    }

                    if (!isSuccess)
                    {
                        if (lstFriends.Count == 0)
                        {
                            lstFriends = (from user in listFacebookuser
                                          select new Friends
                                          {
                                              FriendId = user.UserId,
                                              FullName = user.Familyname,
                                              ProfileUrl = user.ProfileUrl,
                                              Location = user.Currentcity,
                                              DetailedUserInfo = JsonConvert.SerializeObject(user),
                                              InteractionDate = DateTime.Now
                                          }).ToList();
                        }
                        foreach (var friend in lstFriends)
                        {
                            DbOperation.Add(friend);
                            await _delayService.DelayAsync(15);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public List<FbFriendDetails> GetMutualFriendBetweenAccounts(DominatorAccountModel accountModel)
        {
            var binFileHelper = InstanceProvider.GetInstance<IBinFileHelper>();

            List<FbFriendDetails> facebookEntityList = binFileHelper.GetFacebookEntity<FbFriendDetails>();

            List<string> lstFriends = facebookEntityList.Select(x => x.FriendId).ToList();

            lstFriends = lstFriends.GroupBy(x => x)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key).ToList();

            facebookEntityList = facebookEntityList.Where(x => lstFriends.Any(y => y == x.FriendId)).ToList();

            List<FbFriendDetails> duplicateFriendWithCurrentAccount = facebookEntityList
                .Where(x => x.AccountName == accountModel.AccountBaseModel.UserName).ToList();

            duplicateFriendWithCurrentAccount.ForEach(x =>
            {
                var listAllCommonDuplicates =
                    facebookEntityList.Where(y => y.FriendId == x.FriendId && y.AccountName != x.AccountName).ToList();

                AddDuplicateData(listAllCommonDuplicates);
            });

            return duplicateFriendWithCurrentAccount;

        }

        private void AddDuplicateData(List<FbFriendDetails> listAllCommonDuplicates)
        {
            lock (FriendsLock)
            {
                Random random = new Random();

                FbFriendDetails friend = listAllCommonDuplicates.OrderBy(x => random.Next()).Take(1).FirstOrDefault();

                if (friend != null && !DictDuplicateFriends.ContainsKey(friend.FriendId))
                {
                    DictDuplicateFriends.Add(friend.FriendId, friend.AccountName);
                }
            }
        }



        public async Task SaveFriendDetailsToBin(DominatorAccountModel objDominatorAccountModel, List<FacebookUser> listFacebookuser)
        {
            try
            {

                if (listFacebookuser.Count > 0)
                {
                    List<FbFriendDetails> lstFriends = (from user in listFacebookuser
                                                        select new FbFriendDetails
                                                        {
                                                            FriendId = user.UserId,
                                                            FullName = user.Familyname,
                                                            ProfileUrl = user.ProfileUrl,
                                                            InteractionDate = user.InteractionDate,
                                                            AccountName = objDominatorAccountModel.UserName
                                                        }).ToList();


                    lock (AddFriendsLock)
                    {
                        var binFileHelper = InstanceProvider.GetInstance<IBinFileHelper>();

                        List<FbFriendDetails> facebookEntityList = binFileHelper.GetFacebookEntity<FbFriendDetails>();

                        facebookEntityList.RemoveAll(x => x.AccountName == objDominatorAccountModel.UserName);

                        facebookEntityList.AddRange(lstFriends);

                        binFileHelper.SaveFacebookEntity(facebookEntityList, ConstantVariable.GetFacebookDetailsConfigFile());

                    }
                }


            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        internal void DeleteRemovedFriends(DominatorAccountModel objDominatorAccountModel, List<FacebookUser> listFacebookUser)
        {
            try
            {
                var listFriends = DbOperation.Get<Friends>();
                //DbOperation.RemoveAll<Friends>();
                var friendsToRemove = listFriends.Where(x => listFacebookUser.All(y => y.UserId != x.FriendId)).ToList();

                friendsToRemove.ForEach(x =>
                {
                    DbOperation.Remove(x);
                });

                //UpdateFriendCountToAccountModelNew(objDominatorAccountModel, 0, false);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void SaveMarketPlaceDetails(AccountMarketplaceDetailsHandler marketplaceDetails)
        {
            var marketplace = marketplaceDetails.LocationDetails.LocationId != 0 ? new MarketplaceDetails()
            {
                LocationName = marketplaceDetails.LocationDetails.Name,
                FbLocationName = marketplaceDetails.LocationDetails.FbLocationName,
                FbLocationCurrency = marketplaceDetails.Currency,
                FbLocationId = marketplaceDetails.LocationDetails.LocationId,
                IsMarketPlaceAvailable = true
            }
            : new MarketplaceDetails() { IsMarketPlaceAvailable = false };
            if (DbOperation.Get<MarketplaceDetails>().FirstOrDefault() == null)
                DbOperation.Add(marketplace);
            else
                DbOperation.Update(marketplace);
        }

        /*
                public static void WriteStringToTextfile(string content, string filepath)
                {
                    try
                    {
                        using (StreamWriter writer = new StreamWriter(filepath))
                        {
                            writer.Write(content);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }

                }
        */

        /*
                internal void DeleteRemovedGroups(DominatorAccountModel objDominatorAccountModel, List<GroupDetails> listFacebookUser)
                {
                    try
                    {
                        var listFriends = DbOperation.GetSingleColumn<OwnGroups>(x=> x.GroupId);


                        foreach (string groupId in listFriends)
                        {

                            if (listFacebookUser.FirstOrDefault(x => x.GroupId == groupId) != null)
                                continue;
                            DbOperation.Remove<OwnGroups>(x => x.GroupId == groupId);

                        }

                        UpdateGroupCountToAccountModel(objDominatorAccountModel, 0, false);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
        */

        internal void RemoveFriendAfterUnfriend(string friendId)
        {
            Friends friend = DbOperation.Get<Friends>(x => friendId != "" && x.FriendId == friendId).FirstOrDefault();
            if (friend != null)
                DbOperation.Remove(friend);
        }


        internal void RemoveGroupAfterUnjoin(string groupId)
        {
            OwnGroups group = DbOperation.Get<OwnGroups>(x => groupId != "" && x.GroupId == groupId).FirstOrDefault();
            if (group != null)
                DbOperation.Remove(group);
        }


        //Generic Method
        internal async Task SaveGroupsDetail(DominatorAccountModel objDominatorAccountModel, List<GroupDetails> listOfGroup)
        {
            try
            {
                var savedGroups = DbOperation.Get<OwnGroups>();

                var groupsToUpdate = savedGroups?.Where(x => listOfGroup.Any(y => y?.GroupId == x?.GroupId))?.ToList();

                var groupsToAdd = new List<OwnGroups>();

                foreach (GroupDetails group in listOfGroup)
                {

                    try
                    {
                        var currentGroup = groupsToUpdate.FirstOrDefault(x => x?.GroupId == group?.GroupId);

                        if (currentGroup == null)
                        {
                            groupsToAdd.Add(new OwnGroups()
                            {
                                GroupName = group.GroupName,
                                GroupUrl = group.GroupUrl,
                                GroupId = group.GroupId,
                                GroupType = group.GroupType,
                                InteractionDate = group.JoinDate
                            });
                        }
                        else
                        {
                            groupsToUpdate.RemoveAll(x => x.GroupId == group.GroupId);
                            currentGroup.GroupName = group.GroupName;
                            currentGroup.GroupUrl = group.GroupUrl;
                            currentGroup.GroupId = group.GroupId;
                            currentGroup.GroupType = group.GroupType;
                            currentGroup.InteractionDate = group.JoinDate;
                            groupsToUpdate.Add(currentGroup);
                        }

                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }

                try
                {
                    DbOperation.AddRange(groupsToAdd);
                }
                catch (Exception ex)
                {
                    await _delayService.DelayAsync(1000);
                    ex.DebugLog();
                    DbOperation.AddRange(groupsToAdd);
                }

                try
                {
                    DbOperation.UpdateRange(groupsToUpdate);
                }
                catch (Exception ex)
                {
                    await _delayService.DelayAsync(1000);
                    ex.DebugLog();
                    DbOperation.UpdateRange(groupsToUpdate);
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        //public static void Writestringtotextfile(string pth, string data)
        //{

        //    try
        //    {
        //        using (StreamWriter writer = File.AppendText(pth))
        //        {
        //            writer.WriteLine(data);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }
        //}

        //internal void SaveGroupDetails(DominatorAccountModel objDominatorAccountModel, GroupScraperResponseHandler objResponseHandler)
        //{
        //    try
        //    {
        //        var listFriends = DbOperation.Get<OwnGroups>();

        //        foreach (GroupDetails group in objResponseHandler.ObjFdScraperResponseParameters.ListGroup)
        //        {

        //            try
        //            {
        //                if (listFriends.FirstOrDefault(x => x.GroupId == group.GroupId && x.GroupUrl != group.GroupUrl) != null)
        //                {
        //                    var updateGroup = DbOperation.Get<OwnGroups>(x => x.GroupId == group.GroupId).FirstOrDefault();

        //                    if (updateGroup != null)
        //                    {
        //                        updateGroup.GroupName = group.GroupName;
        //                        updateGroup.GroupUrl = group.GroupUrl;
        //                        updateGroup.GroupId = group.GroupId;
        //                        updateGroup.InteractionDate = group.JoinDate;

        //                        DbOperation.Update(updateGroup);
        //                    }
        //                }

        //                else if (listFriends.FirstOrDefault(x => x.GroupId == group.GroupId) == null)
        //                {
        //                    var groupDetails = new OwnGroups()
        //                    {
        //                        GroupName = group.GroupName,
        //                        GroupUrl = group.GroupUrl,
        //                        GroupId = group.GroupId,
        //                        InteractionDate = group.JoinDate
        //                    };
        //                    DbOperation.Add(groupDetails);
        //                }

        //                _delayService.ThreadSleep(50);
        //            }
        //            catch (Exception ex)
        //            {
        //                ex.DebugLog();
        //                _delayService.ThreadSleep(1000);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }
        //}

        /*
                internal void SaveGroupDetails(DominatorAccountModel objDominatorAccountModel, GroupScraperResponseHandlerNew objResponseHandler)
                {
                    try
                    {
                        var listFriends = DbOperation.Get<OwnGroups>();

                        foreach (GroupDetails group in objResponseHandler.ListGroupDetails)
                        {

                            if (listFriends.FirstOrDefault(x => x.GroupId == group.GroupId && x.GroupUrl != group.GroupUrl) != null)
                            {
                                var updateGroup = DbOperation.Get<OwnGroups>(x => x.GroupId == group.GroupId).FirstOrDefault();

                                if (updateGroup != null)
                                {
                                    updateGroup.GroupName = group.GroupName;
                                    updateGroup.GroupUrl = group.GroupUrl;
                                    updateGroup.GroupId = group.GroupId;
                                    updateGroup.InteractionDate = group.JoinDate;

                                    DbOperation.Update(updateGroup);
                                }
                            }
                            else if (listFriends.FirstOrDefault(x => x.GroupId == group.GroupId) == null)
                            {
                                var groupDetails = new OwnGroups()
                                {
                                    GroupName = group.GroupName,
                                    GroupUrl = group.GroupUrl,
                                    GroupId = group.GroupId,
                                    InteractionDate = group.JoinDate
                                };
                                DbOperation.Add(groupDetails);
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
        */

        internal void SaveOwnPageDetails(DominatorAccountModel objDominatorAccountModel, FanpageScraperResponseHandler objResponseHandler)
        {
            try
            {
                var pages = new OwnPages()
                {
                    PageName = objResponseHandler?.ObjFdScraperResponseParameters?.FanpageDetails?.FanPageName,
                    PageUrl = objResponseHandler?.ObjFdScraperResponseParameters?.FanpageDetails?.FanPageUrl,
                    PageId = objResponseHandler?.ObjFdScraperResponseParameters?.FanpageDetails?.FanPageID,
                    InteractionDate = DateTime.Now
                };

                var savedPage = DbOperation.Get<OwnPages>(x => x.PageId == objResponseHandler.ObjFdScraperResponseParameters.FanpageDetails.FanPageID).FirstOrDefault();

                if (savedPage == null)
                    DbOperation.Add(pages);
                else
                {
                    savedPage.PageName = pages.PageName;
                    DbOperation.Update(savedPage);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        /*
                internal bool GetAllGroupDetails(DominatorAccountModel objDominatorAccountModel)
                {
                    try
                    {


                        IDatabaseConnection databaseConnection = SocinatorInitialize
                            .GetSocialLibrary(objDominatorAccountModel.AccountBaseModel.AccountNetwork).GetNetworkCoreFactory()
                            .AccountDatabase;



                        //DataBaseConnection dataBase = DataBaseHandler.GetDataBaseConnectionInstance(accountUserId, SocialNetworks.Facebook);

                        List<OwnGroups> listGroupDetails = DbOperation.Get<OwnGroups>().ToList();

                        if (listGroupDetails.Count > 0)
                        {
                            return true;
                        }
                        else
                            return false;

                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                    return false;
                }
        */

        internal async Task SaveLikedPageDetails
            (IResponseHandler objResponseHandler, List<LikedPages> lstOwnPageId)
        {
            try
            {
                lstOwnPageId.Clear();
                DbOperation.RemoveAll<LikedPages>();

                var listFanpageDetails = objResponseHandler?.ObjFdScraperResponseParameters?.ListPage;

                listFanpageDetails = listFanpageDetails.Where(x => lstOwnPageId.All(y => y.PageId != x.FanPageID)).ToList();

                var pagesToUpdate = lstOwnPageId.Where(y => objResponseHandler.ObjFdScraperResponseParameters.ListPage.Any
                        (x => y.PageId == x.FanPageID && y.PageName != x.FanPageName)).ToList();

                if (listFanpageDetails.Count > 0)
                {
                    try
                    {
                        List<LikedPages> lstPages = (from page in listFanpageDetails
                                                     select new LikedPages
                                                     {
                                                         PageName = page.FanPageName,
                                                         PageUrl = page.FanPageUrl,
                                                         PageId = page.FanPageID,
                                                         InteractionDate = DateTime.Now
                                                     }).ToList();

                        DbOperation.AddRange(lstPages);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }


                if (pagesToUpdate.Count > 0)
                {
                    foreach (var page in pagesToUpdate)
                    {
                        page.PageName = objResponseHandler.ObjFdScraperResponseParameters.ListPage
                            .FirstOrDefault(y => y.FanPageID == page.PageId).FanPageName;
                    }

                    DbOperation.UpdateRange<LikedPages>(pagesToUpdate);
                }

                await _delayService.DelayAsync(50);

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        internal List<LikedPages> GetPageDetails()
        {
            try
            {

                var likedPageDetails = DbOperation.Get<LikedPages>();

                return likedPageDetails.ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return null;
        }

        internal void UpdatePageCountToAccountModel(DominatorAccountModel objDominatorAccountModel, int count, bool isFirstPage)
        {
            int originalcount = DbOperation.Count<LikedPages>();

            int ownPageCount = DbOperation.Count<OwnPages>();

            objDominatorAccountModel.DisplayColumnValue3 = originalcount + ownPageCount;

            SocinatorAccountBuilder.Instance(objDominatorAccountModel.AccountBaseModel.AccountId)
                .AddOrUpdateDisplayColumn3(objDominatorAccountModel.DisplayColumnValue3)
                .SaveToBinFile();
        }

        internal void UpdateAccountInfoToModel(DominatorAccountModel account, FacebookUser objFacebookUser)
        {
            try
            {
                account.AccountBaseModel.UserFullName = objFacebookUser.Familyname;
                account.AccountBaseModel.ProfilePictureUrl = objFacebookUser.ProfilePicUrl;

                if (!account.ExtraParameters.ContainsKey("UserDetails"))
                    account.ExtraParameters.Add("UserDetails", JsonConvert.SerializeObject(objFacebookUser));
                else
                    account.ExtraParameters["UserDetails"] = JsonConvert.SerializeObject(objFacebookUser);
                //AccountsFileManager.Edit(account);

                SocinatorAccountBuilder.Instance(account.AccountBaseModel.AccountId)
                    .AddOrUpdateDominatorAccountBase(account.AccountBaseModel)
                    .AddOrUpdateExtraParameter(account.ExtraParameters)
                    .SaveToBinFile();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        /// <summary>
        /// Extract integer only value from string
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>

        public static string GetIntegerOnlyString(string data)
        {
            if (data == null || data.Contains("null"))
                return "0";

            if (!data.Any(char.IsNumber))
                return "0";

            return Regex.Replace(data, "[^0-9]+", string.Empty);
        }

        public static KeyValuePair<int, int> GetScreenResolution()
        {
            int height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            int width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            return new KeyValuePair<int, int>(width, height);
        }

        public static string GetDouleOnlyString(string data)
        {
            if (data == null || data.Contains("null"))
                return "0";

            if (!data.Any(char.IsNumber))
                return "0";

            var pattern = @".*?([-]{0,1} *\d+.\d+)";

            var matches = Regex.Matches(data, pattern);

            if (matches.Count == 0)
            {
                return GetIntegerOnlyString(data);
            }

            return matches.Count > 0 ? matches[0].Groups[1].ToString() : "0";
        }

        internal void ChangeSessionId(DominatorAccountModel account, FdLoginResponseHandler objFdLoginResponseHandler)
        {
            account.SessionId = objFdLoginResponseHandler.FbDtsg;

            SocinatorAccountBuilder.Instance(account.AccountBaseModel.AccountId)
                .AddOrUpdateLoginStatus(account.IsUserLoggedIn)
                .AddOrUpdateDominatorAccountBase(account.AccountBaseModel)
                .SaveToBinFile();

            // AccountsFileManager.Edit(account);
        }

        /// <summary>
        /// Decodes Html and Unicode response in Utf8 
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>

        public static string GetDecodedResponse(string response)
        {
            string decodedResponse;
            try
            {
                decodedResponse = Regex.Replace(response, "\\\\([^u])", "\\\\$1").Replace("\\", "");
                decodedResponse = WebUtility.HtmlDecode(decodedResponse).Replace("u003C", "<").Replace("u00252C", ",");

            }
            catch (Exception ex)
            {
                ex.DebugLog();
                decodedResponse = response;
                decodedResponse = WebUtility.HtmlDecode(decodedResponse).Replace("u003C", "<").Replace("u00252C", ",");
            }
            return decodedResponse.Replace("<!--", string.Empty).Replace("--!>", string.Empty);
        }

        public static string GetPrtialDecodedResponse(string response)
        {
            var decodedResponse = Regex.Unescape(response).Replace("\\", "");
            decodedResponse = WebUtility.HtmlDecode(decodedResponse).Replace("\\u003C\\", "<").Replace("\\u003C", "<").Replace("u00252C", ",").Replace("<//", "<");
            return decodedResponse.Replace("<!--", string.Empty).Replace("--!>", string.Empty);
        }

        public static string GetNewPrtialDecodedResponse(string response, bool ispostProcess = false)
        {
            string decodedResponse = string.Empty;

            try
            {
                if (response.StartsWith("<!DOCTYPE html>"))
                    return GetDecodedResponse(response);
                else
                {
                    decodedResponse = Regex.Unescape(response).Replace("\\", "");
                    decodedResponse = Regex.Replace(decodedResponse, "\\\\([^u])", "\\\\$1");
                    decodedResponse = WebUtility.HtmlDecode(decodedResponse).Replace("\\u003C\\", "<").Replace("\\u003C", "<").Replace("u00252C", ",").Replace("<//", "<");
                    return decodedResponse.Replace("<!--", string.Empty).Replace("--!>", string.Empty);
                }
            }
            catch (Exception ex)
            {
                if (ispostProcess == false)
                    ex.DebugLog();
                else
                    Console.WriteLine(ex.StackTrace);

                return GetDecodedResponse(response);

            }

        }

        public static string GetHtmlDecodedResponse(string response)
        {
            return WebUtility.HtmlDecode(response);
        }


        public static string GetUnicodeDecodedResponse(string response)
        {
            var response2 = Regex.Unescape(Regex.Replace(response, "\\\\([^u])", "\\\\$1")).Replace("u00", "%").Replace("\\u00", "%");
            return response2;

        }

        public List<string> GetInnerHtmlListFromNodeCollection(HtmlNodeCollection objHtmlNodeCollection)
        {
            List<string> objList = new List<string>();

            if (objHtmlNodeCollection != null)
            {
                try
                {
                    foreach (HtmlNode objNode in objHtmlNodeCollection)
                    {
                        try
                        {
                            objList.Add(objNode.InnerHtml);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                return objList;
            }
            else
                return new List<string>();
        }



        public List<string> GetInnerTextListFromNodeCollection(HtmlNodeCollection objHtmlNodeCollection)
        {
            List<string> objList = new List<string>();

            if (objHtmlNodeCollection != null)
            {
                try
                {
                    foreach (HtmlNode objNode in objHtmlNodeCollection)
                    {
                        try
                        {
                            objList.Add(objNode.InnerText);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                return objList;
            }
            else
                return new List<string>();
        }

        public List<string> GetOuterHtmlListFromNodeCollection(HtmlNodeCollection objHtmlNodeCollection)
        {
            if (objHtmlNodeCollection != null)
            {
                List<string> objList = new List<string>();

                foreach (HtmlNode objNode in objHtmlNodeCollection)
                {
                    try
                    {
                        objList.Add(objNode.OuterHtml);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }

                return objList;
            }
            else
                return null;
        }



        public void UpdateFriendCountToAccountModel(DominatorAccountModel account, int count, bool isFirstPage)
        {
            try
            {
                var listFriends = DbOperation.Get<Friends>();
                account.DisplayColumnValue1 = listFriends.Count;

                SocinatorAccountBuilder.Instance(account.AccountBaseModel.AccountId)
                    .AddOrUpdateLoginStatus(account.IsUserLoggedIn)
                    .AddOrUpdateDisplayColumn1(account.DisplayColumnValue1)
                    .AddOrUpdateDominatorAccountBase(account.AccountBaseModel)
                    .SaveToBinFile();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void UpdateFriendCountToAccountModelNew(DominatorAccountModel account)
        {
            try
            {
                int originalcount = DbOperation.Count<Friends>();

                account.DisplayColumnValue1 = originalcount;

                SocinatorAccountBuilder.Instance(account.AccountBaseModel.AccountId)
                    .AddOrUpdateLoginStatus(account.IsUserLoggedIn)
                    .AddOrUpdateDisplayColumn1(account.DisplayColumnValue1)
                    .AddOrUpdateDominatorAccountBase(account.AccountBaseModel)
                    .SaveToBinFile();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        public void UpdateFriendCountToAccountModelNewUI(DominatorAccountModel account, int count, bool isFirstPage)
        {
            try
            {
                account.DisplayColumnValue1 = count;

                SocinatorAccountBuilder.Instance(account.AccountBaseModel.AccountId)
                    .AddOrUpdateLoginStatus(account.IsUserLoggedIn)
                    .AddOrUpdateDisplayColumn1(account.DisplayColumnValue1)
                    .AddOrUpdateDominatorAccountBase(account.AccountBaseModel)
                    .SaveToBinFile();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        public void UpdateGroupCountToAccountModel(DominatorAccountModel account, int count, bool isFirstPage)
        {
            account.DisplayColumnValue2 = count;

            SocinatorAccountBuilder.Instance(account.AccountBaseModel.AccountId)
                .AddOrUpdateLoginStatus(account.IsUserLoggedIn)
                .AddOrUpdateDisplayColumn2(account.DisplayColumnValue2)
                .AddOrUpdateDominatorAccountBase(account.AccountBaseModel)
                .SaveToBinFile();
        }

        public void UpdateGroupCountToAccountModelNew(DominatorAccountModel account)
        {
            int originalcount = DbOperation.Count<OwnGroups>();

            account.DisplayColumnValue2 = originalcount;

            SocinatorAccountBuilder.Instance(account.AccountBaseModel.AccountId)
                .AddOrUpdateLoginStatus(account.IsUserLoggedIn)
                .AddOrUpdateDisplayColumn2(account.DisplayColumnValue2)
                .AddOrUpdateDominatorAccountBase(account.AccountBaseModel)
                .SaveToBinFile();
        }

        public bool DeleteUnnecessaryGroupDetails(List<GroupDetails> listGroupDetails)
        {
            var isdeleted = false;
            try
            {
                var listGroups = DbOperation.Get<OwnGroups>();
                if (listGroupDetails.Count > 0)
                {
                    foreach (OwnGroups group in listGroups)
                    {
                        if (listGroupDetails.FirstOrDefault(x => x.GroupId == group?.GroupId) == null)
                        {
                            DbOperation.Remove<OwnGroups>(x => x.GroupId == group.GroupId);
                            isdeleted = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.DebugLog();
            }
            return isdeleted;
        }

        public static IEnumerable<TTsource> RandomShuffle<TTsource>(IEnumerable<TTsource> source)
        {
            return source.Select(t => new
            {
                Index = Guid.NewGuid(),
                Value = t
            })
                .OrderBy(p => p.Index)
                .Select(p => p.Value);
        }

        internal static bool CheckUrlValid(string navigationUrl)
        {
            Uri uriResult;

            return Uri.TryCreate(navigationUrl, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

        }

        public string GetBetween(string data, string startString, string endString)
        {
            var matches = Regex.Matches(data, $"{startString}(.*?){endString}", RegexOptions.Singleline);

            return matches.Count > 0 ? matches[0].Groups[1].ToString() : string.Empty;
        }

        public static string GetRandomHexNumber(int digits)
        {
            Random random = new Random();
            byte[] buffer = new byte[digits / 2];
            random.NextBytes(buffer);
            string result = String.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (digits % 2 == 0)
                return result;
            return result + random.Next(16).ToString("X");
        }

        internal static string ReversegetBetween(string strSource, string strEnd, string strStart)
        {
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                var reverseStart = strSource.LastIndexOf(strStart, StringComparison.Ordinal);
                var reverseEnd = strSource.LastIndexOf(strEnd, StringComparison.Ordinal);

                return strSource.Substring(reverseStart + 1, reverseEnd - reverseStart - 1);
            }
            else
            {
                return "";
            }
        }

        /*
                public static  void DownLoadVideoFromUrl(string url, string filePath)
                {
                    try
                    {
                        DirectoryUtilities.CreateDirectory(FdConstants.DownloadFolderPath);

                        using (var webClient = new WebClient())
                        {
                             webClient.DownloadFile(new Uri(url), filePath);
                        }
                    }
                    catch (ArgumentException e)
                    {
                        e.DebugLog(e.Message);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog(ex.Message);
                    }
                }
        */

        public static async Task DownLoadMediaFromUrlAsync(string url, string filePath,
                        string folderPath)
        {
            try
            {
                DirectoryUtilities.CreateDirectory(folderPath);

                using (var webClient = new WebClient())
                {
                    await webClient.DownloadFileTaskAsync(new Uri(url), filePath);
                }
            }
            catch (ArgumentException e)
            {
                e.DebugLog(e.Message);
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }
        }

        /*
                public List<string> GetRandomFriends(DominatorAccountModel account, int count)
                {

                    List<string> listFriendId = DbOperation.GetSingleColumn<Friends>(x=> x.FriendId).ToList();

                    Random random = new Random();

                    listFriendId = listFriendId.OrderBy(x => random.Next()).Take(count).ToList();

                    return listFriendId;

                }
        */

        public static string GetEntityName(string response)
        {
            var entityName = string.Empty;
            var entityNameSplit = Regex.Split(response, "pageTitle");

            if (entityNameSplit.Length > 1)
            {
                entityName = FdRegexUtility.FirstMatchExtractor(entityNameSplit[1], FdConstants.EntityNameRegex);
            }
            return entityName;
        }

        internal static bool GetValidUrl(ref string link)
        {
            var newPattern = "(http|ftp|https)://[\\w-]+(\\.[\\w-]+)+([\\w.,@?^=%&:/~+#-]*[\\w@?^=%&/~+#-])?";

            var match2 = Regex.Matches(link, newPattern, RegexOptions.Singleline);

            var pattern = "^(((ht|f)tp(s?))\\://)?(www.|[a-zA-Z].)[a-zA-Z0-9\\-\\.]+\\.(com|edu|gov|mil|net|org|biz|info|name|museum|us|ca|uk)(\\:[0-9]+)*(/($|[a-zA-Z0-9\\.\\,\\;\\?\'\\\\\\+&amp;%\\$#\\=~_\\-]+))*$";

            foreach (var matchd in match2)
            {
                var match = Regex.Matches(matchd.ToString(), pattern, RegexOptions.Singleline);

                if (match.Count > 0)
                {
                    link = match[0].ToString();
                    return true;
                }
            }
            return false;
        }



        public static bool CheckValidUrl(string link)
        {
            var newPattern = "(http|ftp|https)://[\\w-]+(\\.[\\w-]+)+([\\w.,@?^=%&:/~+#-]*[\\w@?^=%&/~+#-])?";

            var match2 = Regex.Matches(link, newPattern, RegexOptions.Singleline);

            var pattern = "^(((ht|f)tp(s?))\\://)?(www.|[a-zA-Z].)[a-zA-Z0-9\\-\\.]+\\.(com|edu|gov|mil|net|org|biz|info|name|museum|us|ca|uk|int|in)(\\:[0-9]+)*(/($|[a-zA-Z0-9\\.\\,\\;\\?\'\\\\\\+&amp;%\\$#\\=~_\\-]+))*$";

            foreach (var matchd in match2)
            {
                var match = Regex.Matches(matchd.ToString(), pattern, RegexOptions.Singleline);

                if (match.Count > 0)
                {
                    link = match[0].ToString();
                    return true;
                }
            }
            if (match2.Count == 0
                && Regex.Matches(link, pattern, RegexOptions.Singleline).Count >= 1)
                return true;
            return false;
        }

        public static bool CheckValidMail(string link)
        {
            var pattern = "^[a-zA-Z0-9+_.-]+@[a-zA-Z0-9.-]+$";
            var match = Regex.Matches(link, pattern, RegexOptions.Singleline);
            if (match.Count > 0)
                return true;
            return false;

        }


        public static string CheckValidWebsite(string decodedResponse)
        {
            var webaddressRegex = @"[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";

            return FdRegexUtility.FirstMatchExtractor(decodedResponse, webaddressRegex);
        }

        public List<string> GetRandomFriends(DominatorAccountModel account, int noOfTags,
            DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.FacebookModel advanceSettingsModel)
        {
            lock (TagFriendsLock)
            {
                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();

                if (noOfTags > 100)
                    noOfTags = 100;

                var listAdvanceModel = genericFileManager.GetModuleDetails<DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.FacebookModel>
                            (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Facebook));

                var currentCampaignModel = listAdvanceModel.FirstOrDefault
                                (x => x.CampaignId == advanceSettingsModel.CampaignId);

                List<string> listFriendId = DbOperation.GetSingleColumn<Friends>(x => x.FriendId).ToList();

                Random random = new Random();

                if (advanceSettingsModel.IsTagUniqueFriends)
                {
                    listFriendId = listFriendId.Where(y => currentCampaignModel != null && !currentCampaignModel.AlreadyTaggedUsers.Contains(y))
                                    .OrderBy(x => random.Next()).Take(noOfTags).ToList();

                    listAdvanceModel.Remove(currentCampaignModel);

                    if (currentCampaignModel != null)
                    {
                        currentCampaignModel.AlreadyTaggedUsers.AddRange(listFriendId);

                        listAdvanceModel.Add(currentCampaignModel);
                    }

                    genericFileManager.UpdateAdvancedSettingDetails(listAdvanceModel, @"C:\Users\GLB_266\AppData\Local\Socinator\Other\PublisherOtherConfig\Facebook.bin");

                }

                else
                {
                    listFriendId = listFriendId.OrderBy(x => random.Next()).Take(noOfTags).ToList();

                }


                return listFriendId;
            }
        }

        public void RemoveTaggedFriendsOnError(DominatorAccountModel account, List<string> taggedFriends,
            DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.FacebookModel advanceSettingsModel)
        {
            lock (this)
            {
                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();

                var listAdvanceModel = genericFileManager.GetModuleDetails<DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.FacebookModel>
                            (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Facebook));

                var currentCampaignModel = listAdvanceModel.FirstOrDefault
                              (x => x.CampaignId == advanceSettingsModel.CampaignId);


                listAdvanceModel.Remove(currentCampaignModel);
                taggedFriends.ForEach(x =>
                {
                    currentCampaignModel?.AlreadyTaggedUsers.Remove(x);
                });

                listAdvanceModel.Add(currentCampaignModel);

                genericFileManager.UpdateAdvancedSettingDetails(listAdvanceModel, @"C:\Users\GLB_266\AppData\Local\Socinator\Other\PublisherOtherConfig\Facebook.bin");


            }
        }

        public List<string> TagSpecificFriend(DominatorAccountModel account, int noOfTags,
            DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.FacebookModel advanceSettingsModel)
        {
            List<KeyValuePair<string, string>> selectedFriends = advanceSettingsModel.SelectFriendsDetailsModel.AccountFriendsPair.Where(x => x.Key == account.AccountId).ToList();

            List<string> customFriendsList = advanceSettingsModel.ListCustomTaggedUser.Where(x => advanceSettingsModel.SelectFriendsDetailsModel.AccountFriendsPair.All(y => y.Value != x)).ToList();

            customFriendsList.ForEach(x =>
            {
                if (DbOperation.Any<Friends>(z => z.ProfileUrl == x))
                {
                    selectedFriends.Add(new KeyValuePair<string, string>(account.AccountId, x));
                    advanceSettingsModel.SelectFriendsDetailsModel.AccountFriendsPair.Add(new KeyValuePair<string, string>(account.AccountId, x));
                }
            });

            customFriendsList = advanceSettingsModel.ListCustomTaggedUser.Where(x => advanceSettingsModel.SelectFriendsDetailsModel.AccountFriendsPair.All(y => y.Value != x)).ToList();

            return customFriendsList;
        }

        public List<string> MentionSpecificFriendFroPost(DominatorAccountModel account, int noOfTags,
            DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.FacebookModel advanceSettingsModel)
        {
            List<KeyValuePair<string, string>> selectedFriends = advanceSettingsModel.SelectFriendsDetailsModelForMention.AccountFriendsPair.Where(x => x.Key == account.AccountId).ToList();

            List<string> customFriendsList = advanceSettingsModel.ListCustomMentionUser.Where(x => advanceSettingsModel.SelectFriendsDetailsModelForMention.AccountFriendsPair.All(y => y.Value != x)).ToList();

            customFriendsList.ForEach(x =>
            {
                if (DbOperation.Any<Friends>(z => z.ProfileUrl == x))
                {
                    selectedFriends.Add(new KeyValuePair<string, string>(account.AccountId, x));
                    advanceSettingsModel.SelectFriendsDetailsModelForMention.AccountFriendsPair.Add(new KeyValuePair<string, string>(account.AccountId, x));
                }
            });

            customFriendsList = advanceSettingsModel.ListCustomMentionUser.Where(x => advanceSettingsModel.SelectFriendsDetailsModelForMention.AccountFriendsPair.All(y => y.Value != x)).ToList();

            return customFriendsList;
        }

        public List<string> MentionFriends(DominatorAccountModel account, SelectOptionModel objSelectOptionModel)
        {
            List<KeyValuePair<string, string>> selectedFriends = objSelectOptionModel.SelectAccountDetailsModel.AccountFriendsPair.Where(x => x.Key == account.AccountId).ToList();

            List<string> customFriendsList = objSelectOptionModel.ListCustomDetailsUrl.Where(x => objSelectOptionModel.SelectAccountDetailsModel.AccountFriendsPair.All(y => y.Value != x)).ToList();

            customFriendsList.ForEach(x =>
            {
                if (DbOperation.Any<Friends>(z => z.ProfileUrl == x))
                {
                    selectedFriends.Add(new KeyValuePair<string, string>(account.AccountId, x));
                    objSelectOptionModel.SelectAccountDetailsModel.AccountFriendsPair.Add(new KeyValuePair<string, string>(account.AccountId, x));
                }
            });

            customFriendsList = objSelectOptionModel.ListCustomDetailsUrl.Where(x => objSelectOptionModel.SelectAccountDetailsModel.AccountFriendsPair.All(y => y.Value != x)).ToList();

            return customFriendsList;
        }

        public List<string> GetRandomPageActor(DominatorAccountModel account, int noOfTags,
            DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.FacebookModel advanceSettingsModel)
        {
            List<KeyValuePair<string, string>> selectedPages = advanceSettingsModel.SelectPageDetailsModel.AccountPagesBoardsPair.Where(x => x.Key == account.AccountId).ToList();

            var ownpages = DbOperation.Get<OwnPages>();

            selectedPages.ForEach(x =>
            {
                if (ownpages.All(z => z.PageUrl != x.Value))
                {
                    advanceSettingsModel.SelectPageDetailsModel.AccountPagesBoardsPair.Remove(new KeyValuePair<string, string>(account.AccountId, x.Value));
                }
            });

            selectedPages = advanceSettingsModel.SelectPageDetailsModel.AccountPagesBoardsPair.Where(x => x.Key == account.AccountId).ToList();

            List<string> customPageList = advanceSettingsModel.ListCustomPageUrl.Where(x => advanceSettingsModel.SelectPageDetailsModel.AccountPagesBoardsPair.All(y => y.Value != x)).ToList();

            customPageList.ForEach(x =>
            {
                if (ownpages.Any(z => z.PageUrl == x) && !string.IsNullOrEmpty(x))
                {
                    selectedPages.Add(new KeyValuePair<string, string>(account.AccountId, x));
                    advanceSettingsModel.SelectPageDetailsModel.AccountPagesBoardsPair.Add(new KeyValuePair<string, string>(account.AccountId, x));
                }
            });

            customPageList = advanceSettingsModel.ListCustomPageUrl.Where(x => advanceSettingsModel.SelectPageDetailsModel.AccountPagesBoardsPair.All(y => y.Value != x)).ToList();

            customPageList.Remove("");

            return customPageList;
        }

        public static bool IsIntegerOnly(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;

            if (!input.Any(char.IsNumber))
                return false;

            var match = Regex.Match(input, "^[0-9]+$");
            return match.Success;
        }

        public static bool IsPhoneNo(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            var match = Regex.Match(Regex.Replace(input, " ", ""), @"^[+]?(\d{1,2})?[\s.-]?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$");
            return match.Success;
        }

        public string GetFriendName(string friendId)
        {
            return DbOperation.GetSingle<Friends>(x => x.FriendId == friendId).FullName;
        }

        public string GetPageName(string pageUrl)
        {
            return DbOperation.GetSingle<OwnPages>(x => x.PageUrl == pageUrl)?.PageName;
        }
        public string GetPageId(string pageUrl)
        {
            return DbOperation.GetSingle<OwnPages>(x => x.PageUrl == pageUrl)?.PageId;
        }

        public List<Friends> GetFriendDetails(List<string> listFacebookUser)
        {
            listFacebookUser.RemoveAll(x => string.IsNullOrEmpty(x));

            var friendList = DbOperation.GetUserDetails<Friends>().Result;

            var updatedFriends = friendList.Where(z => listFacebookUser.Any(d => z.DetailedUserInfo.Contains(d) || d.Contains(z.FriendId))).ToList();

            return updatedFriends;
        }

        public bool IsFriend(FacebookUser friend)
        {
            var listFriends = DbOperation.Get<Friends>();
            if (listFriends.Any(x => x.FriendId == friend.UserId || x.FriendId == friend.ProfileId))
                return true;
            return false;
        }



        public static string SymboleToCount(string countValue)
        {
            var countString = "0";
            try
            {
                countValue = countValue == null ? string.Empty : countValue;
                var doubleOnlyString = GetDouleOnlyString(countValue);
                Double MultipleCount = 0;
                countString = countValue.ToLower().Contains("k") && Double.TryParse(doubleOnlyString, out MultipleCount)
                       ? (MultipleCount * 1000).ToString()
                       : countValue.ToLower().Contains("m") && Double.TryParse(doubleOnlyString, out MultipleCount)
                       ? (MultipleCount * 1000000).ToString()
                       : FdFunctions.IsIntegerOnly(doubleOnlyString)
                       ? doubleOnlyString
                       : countString;
            }
            catch (Exception ex) { ex.DebugLog(); }
            return countString;

        }
    }

    public class FdRegexUtility
    {

        public static string FirstMatchExtractor(string decodedResponse, Regex regex)
        {
            var match = regex.Matches(decodedResponse);
            return match.Count > 0 ? match[0].Groups[1].ToString() : string.Empty;
        }

        public static string FirstMatchExtractor(string decodedResponse, string pattern)
        {
            var match = Regex.Matches(decodedResponse, pattern, RegexOptions.Singleline);
            return match.Count > 0 && match[0].Groups.Count > 1
                ? match[0].Groups[1].ToString() : match.Count > 0 && match[0].Groups.Count == 1 ?
                match[0].Groups[0].ToString() : string.Empty;
        }

        public static string GetNthMatch(string value, string pattern, int matchCount)
        {
            try
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                return Regex.Matches(value, pattern, RegexOptions.Singleline)[matchCount].Groups[1].ToString() == null
                    ? string.Empty
                    : Regex.Matches(value, pattern, RegexOptions.Singleline)[matchCount].Groups[1].ToString().Trim();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return string.Empty;
            }

        }

        public static int GetMatchCount(string decodedResponse, string pattern)
        {
            var match = Regex.Matches(decodedResponse, pattern, RegexOptions.Singleline);
            return match.Count;
        }

    }


    public class FdHtmlParseUtility : IDisposable
    {
        public string NotFound = "Not Found";

        private HtmlDocument _htmlDoc = new HtmlDocument();

        private bool _disposed;

        public string GetInnerHtmlFromTagName(string pageSource, string tagName, string attributeName,
            string attributeValue)
        {

            try
            {
                if (!string.IsNullOrEmpty(pageSource))
                    _htmlDoc.LoadHtml(pageSource);

                return !string.IsNullOrEmpty(attributeName) ?
                    _htmlDoc.DocumentNode.SelectSingleNode($"//{tagName}[@{attributeName}='{attributeValue}']") != null
                    ? _htmlDoc.DocumentNode.SelectSingleNode($"//{tagName}[@{attributeName}='{attributeValue}']").InnerHtml
                    : _htmlDoc.DocumentNode.SelectSingleNode($"//{tagName}").InnerHtml : string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public string GetInnerHtmlFromPartialTagName(string pageSource, string tagName, string attributeName,
            string attributeValue)
        {
            try
            {
                _htmlDoc = new HtmlDocument();
                if (!string.IsNullOrEmpty(pageSource))
                    _htmlDoc.LoadHtml(pageSource);

                return _htmlDoc.DocumentNode.SelectSingleNode($"//{tagName}[starts-with(@{attributeName}, '{attributeValue}')]")?.InnerHtml;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }


        public string GetInnerHtmlFromPartialEndTagName(string pageSource, string tagName, string attributeName,
            string attributeValue)
        {
            try
            {
                if (!string.IsNullOrEmpty(pageSource))
                    _htmlDoc.LoadHtml(pageSource);

                return _htmlDoc.DocumentNode.SelectSingleNode($"//{tagName}[ends-with(@{attributeName}, '{attributeValue}')]").InnerHtml;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public string GetInnerTextFromPartialTagName(string pageSource, string tagName, string attributeName,
            string attributeValue)
        {
            try
            {
                if (!string.IsNullOrEmpty(pageSource))
                    _htmlDoc.LoadHtml(pageSource);

                return _htmlDoc.DocumentNode.SelectSingleNode($"//{tagName}[starts-with(@{attributeName}, '{attributeValue}')]")?.InnerText ?? "";
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        public string GetInnerTextFromPartialTagNameContains(string pageSource, string tagName, string attributeName,
            string attributeValue)
        {
            try
            {
                _htmlDoc = _htmlDoc ?? new HtmlDocument();
                if (!string.IsNullOrEmpty(pageSource))
                    _htmlDoc.LoadHtml(pageSource);

                return _htmlDoc.DocumentNode.SelectSingleNode($"//{tagName}[contains(@{attributeName},'{attributeValue}')]")?.InnerText ?? "";
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public string GetOuterHtmlFromPartialTagName(string pageSource, string tagName, string attributeName,
           string attributeValue)
        {
            try
            {
                _htmlDoc = _htmlDoc ?? new HtmlDocument();
                if (!string.IsNullOrEmpty(pageSource))
                    _htmlDoc.LoadHtml(pageSource);

                return _htmlDoc.DocumentNode.SelectSingleNode($"//{tagName}[starts-with(@{attributeName}, '{attributeValue}')]")?.OuterHtml;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public string GetOuterHtmlFromPartialTagNameContains(string pageSource, string tagName, string attributeName,
            string attributeValue)
        {
            try
            {
                if (!string.IsNullOrEmpty(pageSource))
                    _htmlDoc.LoadHtml(pageSource);

                return _htmlDoc.DocumentNode.SelectSingleNode($"//{tagName}[contains(@{attributeName},'{attributeValue}')]")?.OuterHtml ?? "";
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public List<string> GetListInnerHtmlFromTagName(string pageSource, string tagName, string attributeName,
            string attributeValue)
        {
            try
            {
                if (!string.IsNullOrEmpty(pageSource))
                    _htmlDoc.LoadHtml(pageSource);

                var lstInnerhtml = new List<string>();
                _htmlDoc.DocumentNode.SelectNodes($"//{tagName}[@{attributeName}='{attributeValue}']")
                    .ForEach(x => { lstInnerhtml.Add(x.InnerHtml.ToString()); });
                return lstInnerhtml;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }


        public string GetOuterHtmlFromTagName(string pageSource, string tagName, string attributeName,
            string attributeValue)
        {
            try
            {
                if (!string.IsNullOrEmpty(pageSource))
                    _htmlDoc.LoadHtml(pageSource);

                return !string.IsNullOrEmpty(attributeName)
                    ? _htmlDoc.DocumentNode.SelectSingleNode($"//{tagName}[@{attributeName}='{attributeValue}']").OuterHtml
                    : _htmlDoc.DocumentNode.SelectSingleNode($"//{tagName}").OuterHtml;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public List<string> GetListInnerHtmlFromPartialTagName(string pageSource, string tagName, string attributeName,
            string attributeValue)
        {
            try
            {
                if (!string.IsNullOrEmpty(pageSource))
                    _htmlDoc.LoadHtml(pageSource);

                var lstInnerhtml = new List<string>();
                _htmlDoc.DocumentNode
                    .SelectNodes($"//{tagName}[starts-with(@{attributeName}, '{attributeValue}')]")
                    .ForEach(x => { lstInnerhtml.Add(x.InnerHtml.ToString()); });
                return lstInnerhtml;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public List<string> GetListInnerTextFromPartialTagName(string pageSource, string tagName, string attributeName,
            string attributeValue)
        {
            try
            {
                if (!string.IsNullOrEmpty(pageSource))
                    _htmlDoc.LoadHtml(pageSource);

                var lstInnerhtml = new List<string>();
                _htmlDoc.DocumentNode
                    .SelectNodes($"//{tagName}[starts-with(@{attributeName}, '{attributeValue}')]")
                    .ForEach(x => { lstInnerhtml.Add(x.InnerText.ToString()); });
                return lstInnerhtml;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }
        public List<string> GetListInnerTextFromPartialTagNameContains(string pageSource, string tagName, string attributeName,
            string attributeValue)
        {
            try
            {
                if (!string.IsNullOrEmpty(pageSource))
                    _htmlDoc.LoadHtml(pageSource);

                var lstInnerhtml = new List<string>();
                _htmlDoc.DocumentNode
                    .SelectNodes($"//{tagName}[contains(@{attributeName}, '{attributeValue}')]")
                    .ForEach(x => { lstInnerhtml.Add(x.InnerText.ToString()); });
                return lstInnerhtml;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public List<string> GetListOuterHtmlFromPartialTagName(string pageSource, string tagName, string attributeName,
            string attributeValue)
        {
            try
            {
                if (!string.IsNullOrEmpty(pageSource))
                    _htmlDoc.LoadHtml(pageSource);

                var lstInnerhtml = new List<string>();
                if (string.IsNullOrEmpty(attributeName))
                    _htmlDoc.DocumentNode.SelectNodes($"//{tagName}")
                                            .ForEach(x => { lstInnerhtml.Add(x.OuterHtml.ToString()); });
                else
                    _htmlDoc.DocumentNode
                        .SelectNodes($"//{tagName}[starts-with(@{attributeName}, '{attributeValue}')]")
                        .ForEach(x => { lstInnerhtml.Add(x.OuterHtml.ToString()); });

                return lstInnerhtml;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public List<string> GetListOuterHtmlFromPartialTagNameContains(string pageSource, string tagName, string attributeName,
            string attributeValue)
        {

            try
            {
                if (!string.IsNullOrEmpty(pageSource))
                    _htmlDoc.LoadHtml(pageSource);

                var lstInnerhtml = new List<string>();
                if (string.IsNullOrEmpty(attributeName))
                    _htmlDoc.DocumentNode.SelectNodes($"//{tagName}")
                                            .ForEach(x => { lstInnerhtml.Add(x.OuterHtml.ToString()); });
                else
                    _htmlDoc.DocumentNode
                        .SelectNodes($"//{tagName}[contains(@{attributeName}, '{attributeValue}')]")
                        .ForEach(x => { lstInnerhtml.Add(x.OuterHtml.ToString()); });

                return lstInnerhtml;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public string GetInnerTextFromTagName(string pageSource, string tagName, string attributeName, string attributeValue)
        {

            try
            {
                if (!string.IsNullOrEmpty(pageSource))
                    _htmlDoc.LoadHtml(pageSource);

                return !string.IsNullOrEmpty(attributeName) ?
                    _htmlDoc.DocumentNode.SelectSingleNode($"//{tagName}[@{attributeName}='{attributeValue}']").InnerText
                    : _htmlDoc.DocumentNode.SelectSingleNode($"//{tagName}").InnerText;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }



        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                    NotFound = NotFound != null ? null : NotFound;
                    _htmlDoc = _htmlDoc != null ? null : _htmlDoc;
                }

                // Dispose unmanaged managed resources.

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

