using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.TdTables.Accounts;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using YoutubeDominatorCore.YoutubeLibrary.DAL;
using YoutubeDominatorCore.YoutubeModels;

namespace YoutubeDominatorCore.YDUtility
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
            : base(processScopeModel.GetActivitySettingsAs<YdModuleSetting>(), processScopeModel.Account, globalService,
                accountService)
        {
        }
    }

    public class BlackWhiteListHandler : IBlackWhiteListHandler
    {
        private readonly IDbAccountService _accountService;
        private readonly DominatorAccountModel _dominatorAccountModel;
        private readonly IDbGlobalService _globalService;
        private readonly YdModuleSetting _moduleSetting;

        [Obsolete("resolve this class through IoC")]
        public BlackWhiteListHandler(YdModuleSetting moduleSetting, DominatorAccountModel dominatorAccountModel,
            IDbGlobalService globalService, IDbAccountService accountService)
        {
            _globalService = globalService;
            _accountService = accountService;
            _moduleSetting = moduleSetting;
            _dominatorAccountModel = dominatorAccountModel;
        }

        public List<T> SkipWhiteListUsers<T>(List<T> lstDetails)
        {
            var listSkippedWhiteListUser = new List<T>();
            var type = typeof(T).Name;
            var listWhiteListUsers = new List<string>();

            switch (type)
            {
                case "String":
                    {
                        listWhiteListUsers = GetWhiteListUsers();
                        var tagDetails = lstDetails.OfType<string>().ToList()
                            .Where(x => !listWhiteListUsers.Any(y => y.Contains(x.ToLower()))).ToList();

                        tagDetails.ForEach(tag =>
                        {
                            listSkippedWhiteListUser.Add((T)Convert.ChangeType(tag, typeof(T)));
                        });
                    }
                    break;

                case "YoutubePost":
                    {
                        listWhiteListUsers = GetWhiteListUsers();
                        var tagDetails = lstDetails.OfType<YoutubePost>().ToList()
                            .Where(x => !(listWhiteListUsers.Any(y => y.Contains(x.ChannelId.ToLower())) ||
                                          listWhiteListUsers.Any(y => y.Contains(x.ChannelUsername.ToLower())))).ToList();

                        tagDetails.ForEach(tag =>
                        {
                            listSkippedWhiteListUser.Add((T)Convert.ChangeType(tag, typeof(T)));
                        });
                    }
                    break;

                case "YoutubeChannel":
                    {
                        if (_moduleSetting.ManageBlackWhiteListModel.IsSkipWhiteListUsers)
                            listWhiteListUsers = GetWhiteListUsers();

                        var tagDetails = lstDetails.OfType<YoutubeChannel>().ToList()
                            .Where(x => !(listWhiteListUsers.Any(y => y.Contains(x.ChannelId.ToLower())) ||
                                          listWhiteListUsers.Any(y => y.Contains(x.ChannelUsername.ToLower())))).ToList();

                        tagDetails.ForEach(tag =>
                        {
                            listSkippedWhiteListUser.Add((T)Convert.ChangeType(tag, typeof(T)));
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
                    case "String":

                        {
                            if (_moduleSetting.SkipBlacklist.IsSkipBlackListUsers)
                                listBlackListUsers = GetBlackListUsers();

                            var tagDetails = lstDetails.OfType<string>().ToList()
                                .Where(x => !listBlackListUsers.Any(y => y.ToLower() == x.ToLower())).ToList();
                            tagDetails.ForEach(tag =>
                            {
                                listSkippedBlackList.Add((T)Convert.ChangeType(tag, typeof(T)));
                            });
                        }

                        break;

                    case "YoutubePost":

                        {
                            if (_moduleSetting.SkipBlacklist.IsSkipBlackListUsers)
                                listBlackListUsers = GetBlackListUsers();

                            var tagDetails = lstDetails.OfType<YoutubePost>().ToList()
                                .Where(x => !(listBlackListUsers.Any(y => y.ToLower() == x.ChannelId.ToLower()) ||
                                              listBlackListUsers.Any(y => y.ToLower() == x.ChannelUsername.ToLower())))
                                .ToList();
                            tagDetails.ForEach(tag =>
                            {
                                listSkippedBlackList.Add((T)Convert.ChangeType(tag, typeof(T)));
                            });
                        }

                        break;

                    case "YoutubeChannel":
                        {
                            if (_moduleSetting.SkipBlacklist.IsSkipBlackListUsers)
                                listBlackListUsers = GetBlackListUsers();

                            var tagDetails = lstDetails.OfType<YoutubeChannel>().ToList()
                                .Where(x => !(listBlackListUsers.Any(y => y.ToLower() == x.ChannelId.ToLower()) ||
                                              listBlackListUsers.Any(y => y.ToLower() == x.ChannelUsername.ToLower())))
                                .ToList();

                            tagDetails.ForEach(tag =>
                            {
                                listSkippedBlackList.Add((T)Convert.ChangeType(tag, typeof(T)));
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
                var listTempWhiteListUsers = _accountService.GetPrivateWhitelist().Select(x => x.UserName).ToList();
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
                    _globalService.AddSingle(new BlackListUser
                    {
                        UserId = userId,
                        UserName = username,
                        AddedDateTime = DateTime.Now
                    });

                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        _dominatorAccountModel.AccountBaseModel.AccountNetwork, _dominatorAccountModel.UserName, "",
                        $"{username} Successfully Added to Group BlackList");
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        _dominatorAccountModel.AccountBaseModel.AccountNetwork, _dominatorAccountModel.UserName, "",
                        $"{username} Already Added to Group BlackList");
                }
            }

            if (_moduleSetting.ManageBlackWhiteListModel.IsAddToPrivateBlackList)
                if (!_accountService.GetPrivateBlacklist().Select(x => x.UserName).ToList().Contains(username))
                {
                    _accountService.Add(new PrivateBlacklist
                    {
                        UserId = userId,
                        UserName = username,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                    });
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        _dominatorAccountModel.AccountBaseModel.AccountNetwork, _dominatorAccountModel.UserName, "",
                        $"{username} Successfully Added to Private BlackList");
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        _dominatorAccountModel.AccountBaseModel.AccountNetwork, _dominatorAccountModel.UserName, "",
                        $"{username} Already Added to Private BlackList");
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
                    _accountService.GetPrivateBlacklist().Select(x => x.UserName).ToList();

                listBlackListUsers.AddRange(listTempBlackListUsers);
                listBlackListUsers = listBlackListUsers.Select(x => x.ToLower().Trim()).Distinct().ToList();
            }

            return listBlackListUsers;
        }
    }
}