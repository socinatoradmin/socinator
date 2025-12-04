using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.TdTables.Accounts;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDModels;

namespace TwtDominatorCore.TDLibrary
{
    public interface IBlackWhiteListHandler
    {
        List<T> SkipWhiteListUsers<T>(List<T> lstDetails);
        List<T> SkipBlackListUsers<T>(List<T> lstDetails);
        List<string> GetWhiteListUsers();
        void AddToBlackList(string userId, string username);
    }

    public class BlackWhiteListHandlerScoped : BlackWhiteListHandler
    {
        public BlackWhiteListHandlerScoped(IProcessScopeModel processScopeModel,
            IDbGlobalService globalService, IDbAccountServiceScoped accountService)
            : base(processScopeModel.GetActivitySettingsAs<ModuleSetting>(), processScopeModel.Account, globalService,
                accountService)
        {
        }
    }

    public class BlackWhiteListHandler : IBlackWhiteListHandler
    {
        private readonly IDbAccountService _accountService;
        private readonly DominatorAccountModel _dominatorAccountModel;
        private readonly IDbGlobalService _globalService;
        private readonly ModuleSetting _moduleSetting;

        [Obsolete("resolve this class through IoC")]
        public BlackWhiteListHandler(ModuleSetting moduleSetting, DominatorAccountModel DominatorAccountModel,
            IDbGlobalService globalService, IDbAccountService accountService)
        {
            _globalService = globalService;
            _accountService = accountService;
            _moduleSetting = moduleSetting;
            _dominatorAccountModel = DominatorAccountModel;
        }

        public List<T> SkipWhiteListUsers<T>(List<T> lstDetails)
        {
            var listSkippedWhiteListUser = new List<T>();
            var type = typeof(T).Name;
            var listWhiteListUsers = new List<string>();

            switch (type)
            {
                case "TagDetails":
                {
                    listWhiteListUsers = GetWhiteListUsers();
                    var tagDetails = lstDetails.OfType<TagDetails>().ToList()
                        .Where(x => !listWhiteListUsers.Contains(x.Username.ToLower())).ToList();

                    tagDetails.ForEach(tag =>
                    {
                        listSkippedWhiteListUser.Add((T) Convert.ChangeType(tag, typeof(T)));
                    });
                }
                    break;

                case "TwitterUser":
                {
                    if (_moduleSetting.ManageBlackWhiteListModel.IsSkipWhiteListUsers)
                        listWhiteListUsers = GetWhiteListUsers();

                    var tagDetails = lstDetails.OfType<TwitterUser>().ToList()
                        .Where(x => !listWhiteListUsers.Contains(x.Username.ToLower())).ToList();

                    tagDetails.ForEach(tag =>
                    {
                        listSkippedWhiteListUser.Add((T) Convert.ChangeType(tag, typeof(T)));
                    });
                }
                    break;

                case "String":
                {
                    List<string> tagDetails;

                    if (_moduleSetting.ManageBlackWhiteListModel.IsSkipWhiteListUsers)
                        listWhiteListUsers = GetWhiteListUsers();

                    tagDetails = lstDetails.OfType<string>().ToList()
                        .Where(x => !listWhiteListUsers.Contains(x.ToLower())).ToList();

                    tagDetails.ForEach(tag =>
                    {
                        listSkippedWhiteListUser.Add((T) Convert.ChangeType(tag, typeof(T)));
                    });
                }
                    break;
            }

            return listSkippedWhiteListUser;
        }

        public List<T> SkipBlackListUsers<T>(List<T> lstDetails)
        {
            var listSkippedBlackList = new List<T>();
            try
            {
                var type = typeof(T).Name;
                var listBlackListUsers = new List<string>();

                switch (type)
                {
                    case "TagDetails":
                    {
                        if (_moduleSetting.SkipBlacklist.IsSkipBlackListUsers)
                            listBlackListUsers = GetBlackListUsers();

                        var tagDetails = lstDetails.OfType<TagDetails>().ToList()
                            .Where(x => !listBlackListUsers.Contains(x.Username.ToLower())).ToList();
                        tagDetails.ForEach(tag =>
                        {
                            listSkippedBlackList.Add((T) Convert.ChangeType(tag, typeof(T)));
                        });
                    }

                        break;

                    case "TwitterUser":
                    {
                        if (_moduleSetting.SkipBlacklist.IsSkipBlackListUsers)
                            listBlackListUsers = GetBlackListUsers();

                        var tagDetails = lstDetails.OfType<TwitterUser>().ToList()
                            .Where(x => !listBlackListUsers.Contains(x.Username)).ToList();

                        tagDetails.ForEach(tag =>
                        {
                            listSkippedBlackList.Add((T) Convert.ChangeType(tag, typeof(T)));
                        });
                    }
                        break;

                    case "String":
                    {
                        if (_moduleSetting.SkipBlacklist.IsSkipBlackListUsers)
                            listBlackListUsers = GetBlackListUsers();

                        var tagDetails = lstDetails.OfType<string>().ToList()
                            .Where(x => !listBlackListUsers.Contains(x.ToLower())).ToList();

                        tagDetails.ForEach(tag =>
                        {
                            listSkippedBlackList.Add((T) Convert.ChangeType(tag, typeof(T)));
                        });
                    }
                        break;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return listSkippedBlackList;
        }

        public List<string> GetWhiteListUsers()
        {
            var listWhiteListUsers = new List<string>();
            if (_moduleSetting.ManageBlackWhiteListModel.IsUseGroupWhiteList)
                listWhiteListUsers = _globalService.GetAllWhiteListUsers().Select(x => x.UserName).ToList();

            if (_moduleSetting.ManageBlackWhiteListModel.IsUsePrivateWhiteList)
            {
                var listTempWhiteListUsers =
                    _accountService.GetPrivateWhitelistUsers().Select(x => x.UserName).ToList();
                listWhiteListUsers.AddRange(listTempWhiteListUsers);
            }


            listWhiteListUsers = listWhiteListUsers.Select(x => x.ToLower().Trim()).Distinct().ToList();

            return listWhiteListUsers.ToList();
        }

        public void AddToBlackList(string userId, string username)
        {
            if (_moduleSetting.ManageBlackWhiteListModel.IsAddToGroupBlackList)
            {
                // here we using global service to get all users not this class methods
                var listUsers = _globalService.GetAllBlackListUsers().Select(x => x.UserName).ToList();
                if (!listUsers.Contains(username))
                {
                    _globalService.Add(new BlackListUser
                    {
                        UserId = userId,
                        UserName = username,
                        AddedDateTime = DateTime.Now
                    });

                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        _dominatorAccountModel.AccountBaseModel.AccountNetwork, _dominatorAccountModel.UserName, "",
                        string.Format("LangKeyUserSuccessfullyAddedToBlackList".FromResourceDictionary(), username,
                            "LangKeyGroupBlacklist".FromResourceDictionary()));
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        _dominatorAccountModel.AccountBaseModel.AccountNetwork, _dominatorAccountModel.UserName, "",
                        string.Format("LangKeyUserAlreadyAddedToBlackList".FromResourceDictionary(), username,
                            "LangKeyGroupBlacklist".FromResourceDictionary()));
                }
            }

            if (_moduleSetting.ManageBlackWhiteListModel.IsAddToPrivateBlackList)
                if (!_accountService.GetPrivateBlacklistUsers().Select(x => x.UserName).ToList().Contains(username))
                {
                    _accountService.Add(new PrivateBlacklist
                    {
                        UserId = userId,
                        UserName = username,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                    });
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        _dominatorAccountModel.AccountBaseModel.AccountNetwork, _dominatorAccountModel.UserName, "",
                        string.Format("LangKeyUserSuccessfullyAddedToBlackList".FromResourceDictionary(), username,
                            "LangKeyPrivateBlackList".FromResourceDictionary()));
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        _dominatorAccountModel.AccountBaseModel.AccountNetwork, _dominatorAccountModel.UserName, "",
                        string.Format("LangKeyUserAlreadyAddedToBlackList".FromResourceDictionary(), username,
                            "LangKeyPrivateBlackList".FromResourceDictionary()));
                }
        }

        public List<string> GetBlackListUsers()
        {
            var listBlackListUsers = new List<string>();
            if (_moduleSetting.SkipBlacklist.IsSkipGroupBlackListUsers)
                listBlackListUsers = _globalService.GetAllBlackListUsers().Select(x => x.UserName.Trim()).ToList();

            if (_moduleSetting.SkipBlacklist.IsSkipPrivateBlackListUser)
            {
                // if check inside it have been null
                var listTempBlackListUsers =
                    _accountService.GetPrivateBlacklistUsers().Select(x => x.UserName).ToList();

                listBlackListUsers.AddRange(listTempBlackListUsers);
                listBlackListUsers = listBlackListUsers.Select(x => x.Trim()).Distinct().ToList();
            }

            return listBlackListUsers;
        }
    }
}