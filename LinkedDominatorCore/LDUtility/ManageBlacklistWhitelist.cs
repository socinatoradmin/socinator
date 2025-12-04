using System;
using System.Collections.Generic;
using System.Linq;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.LDLibrary;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDUtility;

namespace LinkedDominatorCore.LDModel.LDUtility
{
    public class ManageBlacklistWhitelist
    {
        /// <summary>
        ///     we may use in future if we added new feature in IsAlreadyContains
        /// </summary>
        public enum BlackListType
        {
            PrivateBlackList,
            GroupBlackList
        }

        private DbOperations DbBlackListOperations { get; }
        private readonly IDbOperations _blackListOperations;
        private readonly IDbAccountService _dbAccountService;
        private readonly IDelayService _delayService;
        private readonly IDbOperations _whiteListOperations;

        public ManageBlacklistWhitelist(IDbAccountService dbAccountService, IDelayService delayService)
        {
            _delayService = delayService;
            try
            {
                _dbAccountService = dbAccountService;
                var dataBaseConnectionGlb = SocinatorInitialize.GetGlobalDatabase();
                _blackListOperations = new DbOperations(dataBaseConnectionGlb.GetSqlConnection(
                    SocialNetworks.LinkedIn,
                    UserType.BlackListedUser));
                DbBlackListOperations =
                    new DbOperations(
                        dataBaseConnectionGlb.GetSqlConnection(SocialNetworks.LinkedIn, UserType.BlackListedUser));

                _whiteListOperations = new DbOperations(dataBaseConnectionGlb.GetSqlConnection(
                    SocialNetworks.LinkedIn,
                    UserType.WhiteListedUser));
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }

        public void AddToPrivateBlackList(DominatorAccountModel dominatorAccountModel, LinkedinUser objLinkedinUser)
        {
            try
            {
                objLinkedinUser.PublicIdentifier = GetPublicIdentifier(objLinkedinUser);
                if (IsAlreadyContains(objLinkedinUser, _dbAccountService))
                    return;
                _dbAccountService.Add(new PrivateBlacklist
                {
                    UserId = objLinkedinUser.ProfileId,
                    UserName = string.IsNullOrEmpty(objLinkedinUser.PublicIdentifier)
                        ? objLinkedinUser.EmailAddress
                        : objLinkedinUser.PublicIdentifier,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                });
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        public void AddToGroupBlackList(LinkedinUser objLinkedinUser)
        {
            try
            {
                // we may use dictionary here taking profile id as key and public identifier as value

                objLinkedinUser.PublicIdentifier = GetPublicIdentifier(objLinkedinUser);
                if (IsAlreadyContains(objLinkedinUser, null))
                    return;
                _blackListOperations.Add(new BlackListUser
                {
                    UserId = objLinkedinUser.ProfileId,
                    UserName = string.IsNullOrEmpty(objLinkedinUser.PublicIdentifier)
                        ? objLinkedinUser.EmailAddress
                        : objLinkedinUser.PublicIdentifier,
                    AddedDateTime = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        public void AddToGroupWhitelist(LinkedinUser objLinkedinUser)
        {
            try
            {
                var whiteListUsers = _whiteListOperations.Get<WhiteListUser>().Select(x => x.UserId).ToList();
                if (whiteListUsers.Contains(objLinkedinUser.ProfileId))
                    return;
                _whiteListOperations.Add(new WhiteListUser
                {
                    UserId = objLinkedinUser.ProfileId,
                    UserName = objLinkedinUser.PublicIdentifier,
                    AddedDateTime = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        public List<PrivateBlacklist> GetPrivateBlackListedUsers(DbOperations dbOperation = null)
        {
            var listBlackListUsers = new List<PrivateBlacklist>();
            try
            {
                var listTempBlackListUsers = dbOperation?.Get<PrivateBlacklist>().ToList() ??
                                             _dbAccountService.Get<PrivateBlacklist>().ToList();
                listBlackListUsers.AddRange(listTempBlackListUsers);

                listBlackListUsers = listBlackListUsers.Distinct().ToList();
                return listBlackListUsers;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public List<BlackListUser> GetGroupBlackListedUsers()
        {
            var listBlackListUsers = new List<BlackListUser>();
            try
            {
                listBlackListUsers = _blackListOperations.Get<BlackListUser>().ToList();
                listBlackListUsers = listBlackListUsers.Distinct().ToList();
                return listBlackListUsers;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public List<PrivateWhitelist> GetPrivateWhitelistedUsers(IDbAccountService dbAccountService)
        {
            try
            {
                return dbAccountService.Get<PrivateWhitelist>().ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public List<WhiteListUser> GetGroupWhitelistedUsers()
        {
            try
            {
                return _whiteListOperations.Get<WhiteListUser>().ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        private string GetPublicIdentifier(LinkedinUser objLinkedinUser)
        {
            var publicIdentifier = objLinkedinUser.PublicIdentifier;
            try
            {
                if (string.IsNullOrEmpty(objLinkedinUser.EmailAddress) &&
                    string.IsNullOrEmpty(objLinkedinUser.PublicIdentifier))
                    publicIdentifier =
                        Utils.GetBetween(objLinkedinUser.ProfileUrl + "$$", "/in", "$$")
                            .Replace("/", "");
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }

            return publicIdentifier;
        }

        /// <summary>
        ///     this is a special case her we are checking using dbAccountService
        /// </summary>
        /// <param name="objLinkedinUser"></param>
        /// <param name="dbAccountService"></param>
        /// <returns></returns>
        private bool IsAlreadyContains(LinkedinUser objLinkedinUser, IDbAccountService dbAccountService)
        {
            bool isContains;
            try
            {
                var listUsers = dbAccountService == null
                    ? _blackListOperations.Get<BlackListUser>().Select(x => new { x.UserId, x.UserName }).ToList()
                    : _dbAccountService.Get<PrivateBlacklist>().Select(x => new { x.UserId, x.UserName }).ToList();

                //checking with userId if it contains
                if (!string.IsNullOrEmpty(objLinkedinUser.ProfileId) &&
                    listUsers.Any(x => x.UserId == objLinkedinUser.ProfileId))
                    return true;
                // checking with username 
                isContains = listUsers.Any(x => x.UserName.Equals(objLinkedinUser.PublicIdentifier));
            }
            catch (Exception)
            {
                return false;
            }

            return isContains;
        }

        public bool FilterBlackListedUser(string publicIdentifier, bool isChkPrivateBlackList, bool isChkGroupBlackList)
        {
            try
            {
                var isBlackListed = false;
                if (string.IsNullOrEmpty(publicIdentifier))
                    return isBlackListed;
                if (publicIdentifier.Contains("http:") || publicIdentifier.Contains("https:"))
                    publicIdentifier = Utils.GetBetween(publicIdentifier + "**", "/in/", "**").Trim('/');

                if (isChkPrivateBlackList)
                {
                    try
                    {
                        isBlackListed = GetPrivateBlackListedUsers().Any(privateBlackListedUser =>
                                privateBlackListedUser.UserName == publicIdentifier ||
                                privateBlackListedUser.UserId == publicIdentifier ||
                                privateBlackListedUser.UserName.Contains(publicIdentifier));
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    if (isBlackListed)
                        return true;
                }

                if (!isChkGroupBlackList)
                    return false;
                isBlackListed = GetGroupBlackListedUsers().Any(groupBlackListedUser =>
                    groupBlackListedUser.UserName == publicIdentifier ||
                    groupBlackListedUser.UserId == publicIdentifier);
                return isBlackListed;
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }

        public bool FilterSalesBlackListedUser(ILdFunctions ldFunctions, LinkedinUser linkedinUser,
            bool isChkPrivateBlackList,
            bool isChkGroupBlackList)
        {
            if (!isChkPrivateBlackList && !isChkGroupBlackList)
                return false;
            // here we getting user PublicIdentifier of profile that don't have it
            // since in sales navigator we don't PublicIdentifier and we use it to skip blacklisted users
            // and user only know PublicIdentifier not profileId
            if (string.IsNullOrEmpty(linkedinUser.PublicIdentifier))
                linkedinUser.PublicIdentifier =
                    LdDataHelper.GetInstance.GetPublicIdentifierFromSalesProfileUrl(ldFunctions,
                        linkedinUser.ProfileUrl);
            _delayService.ThreadSleep(new Random().Next(2000, 5000));
            return FilterBlackListedUser(linkedinUser.PublicIdentifier,
                isChkPrivateBlackList, isChkGroupBlackList);
        }
    }
}