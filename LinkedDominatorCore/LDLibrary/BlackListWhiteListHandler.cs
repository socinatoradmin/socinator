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
using DominatorUIUtility;
using LinkedDominatorCore.LDModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LinkedDominatorCore.LDLibrary
{
    public class BlackListWhiteListHandler
    {
        public BlackListWhiteListHandler(LDModel.ModuleSetting moduleSetting, DominatorAccountModel dominatorAccountModel,
            ActivityType activityType)
        {
            try
            {
                ActivityType = activityType;
                DominatorAccountModel = dominatorAccountModel;
                DataBaseConnectionGlb = SocinatorInitialize.GetGlobalDatabase();

                DbWhiteListOperations =
                    new DbOperations(
                        DataBaseConnectionGlb.GetSqlConnection(SocialNetworks.LinkedIn, UserType.WhiteListedUser));
                DbBlackListOperations =
                    new DbOperations(
                        DataBaseConnectionGlb.GetSqlConnection(SocialNetworks.LinkedIn, UserType.BlackListedUser));

                DataBaseConnectionAccount = SocinatorInitialize.GetSocialLibrary(SocialNetworks.LinkedIn)
                    .GetNetworkCoreFactory().AccountDatabase;
                DbAccountoperation = new DbOperations(dominatorAccountModel.AccountId, SocialNetworks.LinkedIn,
                    ConstantVariable.GetAccountDb);
                ModuleSetting = moduleSetting;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        //second param : [CallerMemberName] string CallerMethod = null
        public List<LinkedinUser> SkipWhiteListUser(List<LinkedinUser> lstDetails)
        {
            var listSkippedWhiteListUser = new List<LinkedinUser>(lstDetails);
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

        public List<T> SkipBlackListUser<T>(List<T> lstDetails, [CallerMemberName] string callerMethod = null)
        {
            var listSkippedBlackList = new List<T>();
            try
            {
                List<string> listBlackListUsers;
                listBlackListUsers = GetBlackListUsers();

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
                        List<LinkedinUser> lstInatagramUser;
                        lstInatagramUser = lstDetails.OfType<LinkedinUser>().ToList()
                            .Where(x => !listBlackListUsers.Contains(x.Username)).ToList();

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
                    case Enums.WhitelistblacklistType.Both:
                        {
                            listWhiteListUsers = DbBlackListOperations.Get<WhiteListUser>().Select(x => x.UserName).ToList();

                            var listTempBlackListUsers = dbOperation?.Get<PrivateWhitelist>().Select(x => x.UserName).ToList() ??
                                                         DbAccountoperation.Get<PrivateWhitelist>().Select(x => x.UserName)
                                                             .ToList();
                            listWhiteListUsers.AddRange(listTempBlackListUsers);
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

        private LDModel.ModuleSetting ModuleSetting { get; }

        private DbOperations DbBlackListOperations { get; }

        private DbOperations DbWhiteListOperations { get; }

        private DbOperations DbAccountoperation { get; }

        private IDatabaseConnection DataBaseConnectionAccount { get; }

        private IGlobalDatabaseConnection DataBaseConnectionGlb { get; }

        private DominatorAccountModel DominatorAccountModel { get; }

        #endregion


    }
}
