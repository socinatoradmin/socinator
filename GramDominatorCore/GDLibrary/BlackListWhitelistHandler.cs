using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using GramDominatorCore.GDEnums;
using GramDominatorCore.GDModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ConstantVariable = DominatorHouseCore.Utility.ConstantVariable;
using DateTimeUtilities = DominatorHouseCore.Utility.DateTimeUtilities;
using Log = DominatorHouseCore.Utility.Log;

namespace GramDominatorCore.GDLibrary
{
    public class BlackListWhitelistHandler
    {
        #region Properties
        private ActivityType ActivityType { get; }

        private ModuleSetting ModuleSetting { get;  }

        private DbOperations DbBlackListOperations { get; }

        private DbOperations DbWhiteListOperations { get; }

        private DbOperations DbAccountoperation { get;}

        private IGlobalDatabaseConnection DataBaseConnectionGlb { get;  }

        private DominatorAccountModel DominatorAccountModel { get; }
        #endregion

        #region Constructors

        public BlackListWhitelistHandler(ModuleSetting moduleSetting, DominatorAccountModel dominatorAccountModel, ActivityType activityType)
        {
            try
            {
                ActivityType = activityType;
                DominatorAccountModel = dominatorAccountModel;
                DataBaseConnectionGlb = SocinatorInitialize.GetGlobalDatabase();

                DbWhiteListOperations = new DbOperations(DataBaseConnectionGlb.GetSqlConnection(SocialNetworks.Instagram, UserType.WhiteListedUser));
                DbBlackListOperations = new DbOperations(DataBaseConnectionGlb.GetSqlConnection(SocialNetworks.Instagram, UserType.BlackListedUser));

                DbAccountoperation = new DbOperations(dominatorAccountModel.AccountId, SocialNetworks.Instagram, ConstantVariable.GetAccountDb);
                ModuleSetting = moduleSetting;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion
        //second param : [CallerMemberName] string CallerMethod = null
        public List<InstagramUser> SkipWhiteListUser(List<InstagramUser> lstDetails )
        {
            List<InstagramUser> listSkippedWhiteListUser = new List<InstagramUser>(lstDetails);
            try
            {             
                List<string> listWhiteListUsers = new List<string>();
                if (ModuleSetting.ManageBlackWhiteListModel.IsSkipGroupWhiteList)
                {
                    listWhiteListUsers = GetWhiteListUsers(Enums.WhitelistblacklistType.Group);
                }

                if (ModuleSetting.ManageBlackWhiteListModel.IsSkipPrivateWhiteList)
                {
                    listWhiteListUsers.AddRange(GetWhiteListUsers(Enums.WhitelistblacklistType.Private));
                }

                foreach (InstagramUser username in lstDetails)
                {
                    if (listWhiteListUsers.Contains(username.Username))
                    { 
                        listSkippedWhiteListUser.Remove(username);
                    }
                }
             
            }
            catch (Exception ex)
            {
               // GlobusLogHelper.log.Debug($"Error  : {CallerMethod}");
                ex.DebugLog();
            }
            return listSkippedWhiteListUser;
        }

        public List<T> SkipBlackListUser<T>(List<T> lstDetails, InstagramUser singleInstaUser = null,[CallerMemberName] string callerMethod = null)
        {
            List<T> listSkippedBlackList = new List<T>();
            try
            {
                List<string> listBlackListUsers;
                listBlackListUsers = GetBlackListUsers();

                if(singleInstaUser != null)
                {
                    callerMethod = "singleInstaUser";
                }

                switch(callerMethod)
                {
                    case "StartProcessForMediaCommenters":
                        { }
                        break;
                    case "singleInstaUser":
                        {
                             bool isAvailableBlackListUser=singleInstaUser != null && listBlackListUsers.Contains(singleInstaUser.Username);
                            if(!isAvailableBlackListUser)
                            {
                                listSkippedBlackList.Add((T)Convert.ChangeType(singleInstaUser, typeof(T)));
                            }
                                                      
                        }
                        break;
                    default:
                        List<InstagramUser> lstInatagramUser;
                        lstInatagramUser = lstDetails.OfType<InstagramUser>().ToList().Where(x => !listBlackListUsers.Contains(x.Username)).ToList();

                        lstInatagramUser.ForEach(tag =>
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
                listBlackListUsers = DbBlackListOperations.Get<BlackListUser>().Select(x => x.UserName).ToList();
                
                var listTempBlackListUsers = dbOperation?.Get<PrivateBlacklist>().Select(x => x.UserName).ToList() ?? DbAccountoperation.Get<PrivateBlacklist>().Select(x => x.UserName).ToList();
                listBlackListUsers.AddRange(listTempBlackListUsers);

                listBlackListUsers = listBlackListUsers.Distinct().ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return listBlackListUsers;
        }

        public List<string> GetWhiteListUsers(Enums.WhitelistblacklistType whitelistblacklistType, DbOperations dbOperation = null)
        {
            List<string> listWhiteListUsers = new List<string>();
            try
            {
                switch (whitelistblacklistType)
                {
                    case Enums.WhitelistblacklistType.Group:
                        listWhiteListUsers = DbWhiteListOperations.Get<WhiteListUser>().Select(x => x.UserName).ToList();
                        break;
                    case Enums.WhitelistblacklistType.Private:
                        {
                            var listTempWhiteListUsers = DbAccountoperation.Get<PrivateWhitelist>().Select(x => x.UserName).ToList();
                            listWhiteListUsers.AddRange(listTempWhiteListUsers);
                            break;
                        }
                }

                listWhiteListUsers = listWhiteListUsers.Distinct().ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return listWhiteListUsers;
        }

        public bool AddToBlackList(string userId, string username)
        {
            bool success = !(string.IsNullOrEmpty(userId.Trim()) && string.IsNullOrEmpty(username.Trim()));

            try
            {
                if (ModuleSetting.ManageBlackWhiteListModel.IsAddToGroupBlackList)
                {
                    List<string> whitelistusers = GetWhiteListUsers(Enums.WhitelistblacklistType.Group);
                    List<string> listUsers = DbBlackListOperations.Get<BlackListUser>().Select(x => x.UserName).ToList();
                   
                    if (!listUsers.Contains(username) && !whitelistusers.Contains(username))
                    {
                        DbBlackListOperations.Add(new BlackListUser()
                        {
                            UserId = userId,
                            UserName = username,
                            AddedDateTime = DateTime.Now
                        });

                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType, $"{username} Successfully Added to Group BlackList");
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType, $"{username} Already Added to Group BlackList/Whitelist");
                        success = false;
                    }
                }

                if (ModuleSetting.ManageBlackWhiteListModel.IsAddToPrivateBlackList)
                {
                    List<string> blacklistusers = GetWhiteListUsers(Enums.WhitelistblacklistType.Private);

                    if (!blacklistusers.Contains(username) && !DbAccountoperation.Get<PrivateBlacklist>().Select(x => x.UserName).ToList().Contains(username))
                    {
                        DbAccountoperation.Add(new PrivateBlacklist()
                        {
                            UserId = userId,
                            UserName = username,
                            InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                        });
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType, $"{username} Successfully Added to Private BlackList");
                    }
                    else
                    {
                        success = false;
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName, ActivityType, $"{username} Already Added to Private BlackList/Whitelist");
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
    }
}