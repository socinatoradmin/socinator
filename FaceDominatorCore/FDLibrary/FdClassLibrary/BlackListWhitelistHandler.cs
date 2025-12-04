using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.CommonSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FaceDominatorCore.FDLibrary.FdClassLibrary
{


    public class BlackListWhitelistHandler
    {
        private readonly ModuleSetting _moduleSetting;
        // private DbOperations dbOperations;
        /*
                private DbContext dbContext;
        */

        // private ModuleSetting moduleSetting;
        private readonly DbOperations _dbBlackListOperations;
        private readonly DbOperations _dbAccountoperation;

        private readonly DbOperations _dbWhiteListOperations;

        private readonly ActivityType _activityType;

        /*
        private SQLiteConnection dbAccountContext;
*/

        private readonly DominatorAccountModel _dominatorAccountModel;


        /*
                public BlackListWhitelistHandler(ModuleSetting moduleSetting)
                {
                    _moduleSetting = moduleSetting;
                    var dataBaseConnectionGlb = SocinatorInitialize.GetGlobalDatabase();
                    var dbBlackListContext = dataBaseConnectionGlb.GetSqlConnection(SocialNetworks.Facebook, UserType.BlackListedUser);
                    var dbWhiteListContext = dataBaseConnectionGlb.GetSqlConnection(SocialNetworks.Facebook, UserType.WhiteListedUser);
                    _dbWhiteListOperations = new DbOperations(dbWhiteListContext);
                    _dbBlackListOperations = new DbOperations(dbBlackListContext);
                }
        */
        public BlackListWhitelistHandler(ModuleSetting moduleSetting, DominatorAccountModel dominatorAccountModel,
            ActivityType activityType)
        {
            try
            {
                _dominatorAccountModel = dominatorAccountModel;
                var dataBaseConnectionGlb = SocinatorInitialize.GetGlobalDatabase();
                _activityType = activityType;
                var dbBlackListContext = dataBaseConnectionGlb.GetSqlConnection(SocialNetworks.Facebook, UserType.BlackListedUser);
                _dbBlackListOperations = new DbOperations(dbBlackListContext);

                var dbWhiteListContext = dataBaseConnectionGlb.GetSqlConnection(SocialNetworks.Facebook, UserType.WhiteListedUser);
                _dbWhiteListOperations = new DbOperations(dbWhiteListContext);

                //SocinatorInitialize.GetSocialLibrary(SocialNetworks.Facebook).GetNetworkCoreFactory().AccountDatabase;
                _dbAccountoperation = new DbOperations(dominatorAccountModel.AccountId, SocialNetworks.Facebook, ConstantVariable.GetAccountDb);
                _moduleSetting = moduleSetting;

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public List<T> SkipWhiteListUsers<T>(List<T> lstDetails, [CallerMemberName] string callerMethod = null)
        {
            List<T> listSkippedWhiteListUser = new List<T>();
            try
            {
                //var DataBaseConnectionGlb = SocinatorInitialize.GetGlobalDatabase();
                //var dbContext = DataBaseConnectionGlb.GetDbContext(SocialNetworks.Twitter, UserType.BlackListedUser);
                //var dbOperations = new DbOperations(dbContext);
                List<string> whiteListUsers;
                switch (callerMethod)
#pragma warning disable 1522
                {
                    case "StartProcessForMediaCommenters":
                        //{
                        //    List<TagDetails> tagDetails = new List<TagDetails>();
                        //    listWhiteListUsers = getWhiteListUsers();
                        //    tagDetails = lstDetails.OfType<TagDetails>().ToList().Where(x => !listWhiteListUsers.Contains(x.Username)).ToList();

                        //    tagDetails.ForEach(tag =>
                        //    {
                        //        listSkippedWhiteListUser.Add((T)Convert.ChangeType(tag, typeof(T)));
                        //    });
                        //}
                        break;
                    default:
                        List<FacebookUser> tagDetails;
                        whiteListUsers = GetWhiteListUsers();
                        tagDetails = lstDetails.OfType<FacebookUser>().ToList().Where(x => !whiteListUsers.Contains(x.UserId)).ToList();

                        tagDetails.ForEach(tag =>
                        {
                            listSkippedWhiteListUser.Add((T)Convert.ChangeType(tag, typeof(T)));
                        });
                        break;
                }
#pragma warning restore 1522
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Debug($"Error  : {callerMethod}");
                ex.DebugLog();
            }
            return listSkippedWhiteListUser;
        }

        public List<T> SkipBlackListUsers<T>(List<T> lstDetails, [CallerMemberName] string callerMethod = null)
        {
            List<T> listSkippedBlackList = new List<T>();
            try
            {

                List<string> listBlackListUsers;

                switch (callerMethod)
                {

                    case "StartProcessForMediaCommenters":
                        //{
                        //    List<TagDetails> tagDetails = new List<TagDetails>();
                        //    listWhiteListUsers = getWhiteListUsers();
                        //    tagDetails = lstDetails.OfType<TagDetails>().ToList().Where(x => !listWhiteListUsers.Contains(x.Username)).ToList();

                        //    tagDetails.ForEach(tag =>
                        //    {
                        //        listSkippedWhiteListUser.Add((T)Convert.ChangeType(tag, typeof(T)));
                        //    });
                        //}
                        break;
                    default:
                        List<FacebookUser> tagDetails;
                        listBlackListUsers = GetBlackListUsers();
                        tagDetails = lstDetails.OfType<FacebookUser>().ToList().Where(x => !listBlackListUsers.Any(y => y.Contains(x.UserId))).ToList();
                        tagDetails = lstDetails.OfType<FacebookUser>().ToList().Where(x => !listBlackListUsers.Any(y => y.Contains(x.ProfileId))).ToList();

                        tagDetails.ForEach(tag =>
                        {
                            listSkippedBlackList.Add((T)Convert.ChangeType(tag, typeof(T)));
                        });
                        break;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return listSkippedBlackList;
        }

        public List<string> GetBlackListUsers(DbOperations dbOperation = null)
        {
            List<string> listBlackListUsers = new List<string>();
            try
            {
                if (_moduleSetting.SkipBlacklist.IsSkipGroupBlackListUsers)
                    listBlackListUsers = _dbBlackListOperations.GetSingleColumn<BlackListUser>(x => x.UserName).ToList();

                if (_moduleSetting.SkipBlacklist.IsSkipPrivateBlackListUser)
                {
                    // if check inside it have been null
                    var listTempBlackListUsers = dbOperation == null ? _dbAccountoperation.GetSingleColumn<PrivateBlacklist>(x => x.UserName).ToList() : dbOperation.GetSingleColumn<PrivateBlacklist>(x => x.UserName).ToList();

                    listBlackListUsers.AddRange(listTempBlackListUsers);
                    listBlackListUsers = listBlackListUsers.Distinct().ToList();
                }


            }
            catch (Exception ex) { ex.DebugLog(); }
            return listBlackListUsers;
        }


        public List<string> GetWhiteListUsers(DbOperations dbOperation = null)
        {
            List<string> listWhiteListUsers = new List<string>();
            try
            {

                //dbWhiteListOperations.Add<WhiteListUser>(new WhiteListUser()
                //{
                //    UserId = "23164",
                //    UserName = "SuperSportTV",
                //    AddedDateTime = DateTime.Now
                //});

                if (_moduleSetting.ManageBlackWhiteListModel.IsUseGroupWhiteList)
                    listWhiteListUsers = _dbWhiteListOperations.Get<WhiteListUser>().Select(x => x.UserName).ToList();

                if (_moduleSetting.ManageBlackWhiteListModel.IsUsePrivateWhiteList)
                {
                    var listTempWhiteListUsers = _dbAccountoperation.Get<PrivateWhitelist>().Select(x => x.UserName).ToList();

                    listWhiteListUsers.AddRange(listTempWhiteListUsers);

                    listWhiteListUsers = listWhiteListUsers.Distinct().ToList();
                }
            }
            catch (Exception ex) { ex.DebugLog(); }
            return listWhiteListUsers;
        }


        public bool AddToBlackList(string userId, string username)
        {
            bool success = !(string.IsNullOrEmpty(userId.Trim()) && string.IsNullOrEmpty(username.Trim()));

            try
            {

                if (_moduleSetting.ManageBlackWhiteListModel.IsAddToGroupBlackList)
                {
                    List<string> listUsers = _dbBlackListOperations.Get<BlackListUser>().Select(x => x.UserName).ToList();
                    if (!listUsers.Contains(username))
                    {
                        _dbBlackListOperations.Add(new BlackListUser()
                        {
                            UserId = userId,
                            UserName = username,
                            AddedDateTime = DateTime.Now
                        });

                        GlobusLogHelper.log.Info(Log.CustomMessage, _dominatorAccountModel.AccountBaseModel.AccountNetwork, _dominatorAccountModel.UserName, _activityType, $"{username} Successfully Added to Group BlackList");
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, _dominatorAccountModel.AccountBaseModel.AccountNetwork, _dominatorAccountModel.UserName, _activityType, $"{username} Already Added to Group BlackList");
                        success = false;
                    }


                }

                if (_moduleSetting.ManageBlackWhiteListModel.IsAddToPrivateBlackList)
                {
                    if (!_dbAccountoperation.Get<PrivateBlacklist>().Select(x => x.UserName).ToList().Contains(username))
                    {
                        _dbAccountoperation.Add(new PrivateBlacklist()
                        {
                            UserId = userId,
                            UserName = username,
                            InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                        });
                        GlobusLogHelper.log.Info(Log.CustomMessage, _dominatorAccountModel.AccountBaseModel.AccountNetwork, _dominatorAccountModel.UserName, _activityType, $"{username} Successfully Added to Private BlackList");
                    }
                    else
                    {
                        success = false;
                        GlobusLogHelper.log.Info(Log.CustomMessage, _dominatorAccountModel.AccountBaseModel.AccountNetwork, _dominatorAccountModel.UserName, _activityType, $"{username} Already Added to Private BlackList");
                    }
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
                success = false;
            }

            return success;
        }

        /*
                public bool AddToWhiteList(string username, string userId)
                {
                    bool success = !(string.IsNullOrEmpty(userId.Trim()) && string.IsNullOrEmpty(username.Trim()));

                    try
                    {

                        if (_moduleSetting.ManageBlackWhiteListModel.IsUseGroupWhiteList)
                        {

                            if (!(_dbWhiteListOperations.Get<WhiteListUser>().Select(x => x.UserName).ToList().Contains(username)))
                            {
                                _dbWhiteListOperations.Add(new WhiteListUser()
                                {
                                    UserId = userId,
                                    UserName = username,
                                    AddedDateTime = DateTime.Now
                                });
                                GlobusLogHelper.log.Info(Log.CustomMessage, _dominatorAccountModel.AccountBaseModel.AccountNetwork, _dominatorAccountModel.UserName,_activityType, $"{username} Successfully Added to Group BlackList");
                            }
                            else
                            {
                                success = false;
                                GlobusLogHelper.log.Info(Log.CustomMessage, _dominatorAccountModel.AccountBaseModel.AccountNetwork, _dominatorAccountModel.UserName,_activityType, $"{username} Already Added to Group BlackList");
                            }


                        }

                        if (_moduleSetting.ManageBlackWhiteListModel.IsAddToPrivateBlackList)
                        {

                            if (!(_dbAccountoperation.Get<PrivateWhitelist>().Select(x => x.UserName).ToList().Contains(username)))
                            {
                                _dbAccountoperation.Add(new PrivateWhitelist()
                                {
                                    UserId = userId,
                                    UserName = username,
                                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                                });
                                GlobusLogHelper.log.Info(Log.CustomMessage, _dominatorAccountModel.AccountBaseModel.AccountNetwork, _dominatorAccountModel.UserName,_activityType, $"{username} Successfully Added to Private Whitelist");
                            }
                            else
                            {
                                success = false;
                                GlobusLogHelper.log.Info(Log.CustomMessage, _dominatorAccountModel.AccountBaseModel.AccountNetwork, _dominatorAccountModel.UserName,_activityType, $"{username} Already Added to Private Whitelist");
                            }


                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                        success = false;
                    }


                    return success;
                }
        */

    }
}
