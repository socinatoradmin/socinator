using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDEnums;
using RedditDominatorCore.RDModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RedditDominatorCore.RDLibrary
{
    public class BlackListWhitelistHandler
    {
        public BlackListWhitelistHandler(ModuleSetting moduleSetting, DominatorAccountModel dominatorAccountModel,
            ActivityType activityType)
        {
            try
            {
                ActivityType = activityType;
                DominatorAccountModel = dominatorAccountModel;
                DataBaseConnectionGlb = SocinatorInitialize.GetGlobalDatabase();

                DbWhiteListOperations =
                    new DbOperations(
                        DataBaseConnectionGlb.GetSqlConnection(SocialNetworks.Reddit, UserType.WhiteListedUser));
                DbBlackListOperations =
                    new DbOperations(
                        DataBaseConnectionGlb.GetSqlConnection(SocialNetworks.Reddit, UserType.BlackListedUser));

                DataBaseConnectionAccount = SocinatorInitialize.GetSocialLibrary(SocialNetworks.Reddit)
                    .GetNetworkCoreFactory().AccountDatabase;
                DbAccountoperation = new DbOperations(dominatorAccountModel.AccountId, SocialNetworks.Reddit,
                    ConstantVariable.GetAccountDb);
                ModuleSetting = moduleSetting;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        public List<RedditUser> SkipWhiteListUser(List<RedditUser> lstDetails)
        {
            var listSkippedWhiteListUser = new List<RedditUser>(lstDetails);
            try
            {
                var listWhiteListUsers = new List<string>();
                if (ModuleSetting.ManageBlackWhiteListModel.IsSkipGroupWhiteList)
                    listWhiteListUsers = GetWhiteListUsers(Enums.WhitelistblacklistType.Group);

                if (ModuleSetting.ManageBlackWhiteListModel.IsSkipPrivateWhiteList)
                    listWhiteListUsers.AddRange(GetWhiteListUsers(Enums.WhitelistblacklistType.Private));

                foreach (var username in lstDetails)
                    if (listWhiteListUsers.Contains(username.Username))
                        listSkippedWhiteListUser.Remove(username);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return listSkippedWhiteListUser;
        }
        public List<string> GetBlackListUsers(DbOperations dbOperation = null)
        {
            var listBlackListUsers = new List<string>();

            try
            {
                listBlackListUsers = DbBlackListOperations.Get<BlackListUser>().Select(x => x.UserName).ToList();

                var listTempBlackListUsers = dbOperation?.Get<PrivateBlacklist>().Select(x => x.UserName).ToList() ??
                                             DbAccountoperation.Get<PrivateBlacklist>().Select(x => x.UserName)
                                                 .ToList();
                listBlackListUsers.AddRange(listTempBlackListUsers);

                listBlackListUsers = listBlackListUsers.Distinct().ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return listBlackListUsers;
        }

        public List<string> GetWhiteListUsers(Enums.WhitelistblacklistType whitelistblacklistType,
            DbOperations dbOperation = null)
        {
            var listWhiteListUsers = new List<string>();
            try
            {
                switch (whitelistblacklistType)
                {
                    case Enums.WhitelistblacklistType.Group:
                        listWhiteListUsers =
                            DbWhiteListOperations.Get<WhiteListUser>().Select(x => x.UserName).ToList();
                        break;
                    case Enums.WhitelistblacklistType.Private:
                        {
                            var listTempWhiteListUsers =
                                DbAccountoperation.Get<PrivateWhitelist>().Select(x => x.UserName).ToList();
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
            var success = !(string.IsNullOrEmpty(userId.Trim()) && string.IsNullOrEmpty(username.Trim()));

            try
            {
                if (ModuleSetting.ManageBlackWhiteListModel.IsAddToGroupBlackList)
                {
                    var whitelistusers = GetWhiteListUsers(Enums.WhitelistblacklistType.Group);
                    var listUsers = DbBlackListOperations.Get<BlackListUser>().Select(x => x.UserName).ToList();

                    if (!listUsers.Contains(username) && !whitelistusers.Contains(username))
                    {
                        DbBlackListOperations.Add(new BlackListUser
                        {
                            UserId = userId,
                            UserName = username,
                            AddedDateTime = DateTime.Now
                        });

                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                            ActivityType, $"{username} Successfully Added to Group BlackList");
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                            ActivityType, $"{username} Already Added to Group BlackList/Whitelist");
                        success = false;
                    }
                }

                if (ModuleSetting.ManageBlackWhiteListModel.IsAddToPrivateBlackList)
                {
                    var blacklistusers = GetWhiteListUsers(Enums.WhitelistblacklistType.Private);

                    if (!blacklistusers.Contains(username) && !DbAccountoperation.Get<PrivateBlacklist>()
                            .Select(x => x.UserName).ToList().Contains(username))
                    {
                        DbAccountoperation.Add(new PrivateBlacklist
                        {
                            UserId = userId,
                            UserName = username,
                            InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                        });
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                            ActivityType, $"{username} Successfully Added to Private BlackList");
                    }
                    else
                    {
                        success = false;
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                            ActivityType, $"{username} Already Added to Private BlackList/Whitelist");
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

        #region Properties

        private ActivityType ActivityType { get; }

        private ModuleSetting ModuleSetting { get; }

        private DbOperations DbBlackListOperations { get; }

        private DbOperations DbWhiteListOperations { get; }

        private DbOperations DbAccountoperation { get; }

        private IDatabaseConnection DataBaseConnectionAccount { get; }

        private IGlobalDatabaseConnection DataBaseConnectionGlb { get; }

        private DominatorAccountModel DominatorAccountModel { get; }

        #endregion
    }
}