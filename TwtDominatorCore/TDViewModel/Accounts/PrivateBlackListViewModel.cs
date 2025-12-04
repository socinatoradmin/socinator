using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore.DatabaseHandler.TdTables.Accounts;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;

namespace TwtDominatorCore.TDViewModel.ViewModel
{
    public class PrivateBlackListViewModel : BlackListViewModel, IPrivateBlickListViewModel
    {
        private readonly IDelayService _delayService;
        private List<PrivateBlacklist> _blackListUser = new List<PrivateBlacklist>();
        private IDbAccountService _dbAccountService;
        private readonly SocialNetworks _network;
        private bool IsAddingToDB;
        private bool IsNeedToStop;

        private bool IsStopLoading;
        private bool UsersSearched;

        public PrivateBlackListViewModel(SocialNetworks network)
        {
            _network = network;
            PrivateBlacklistUserModel.LstAccountModel = new ObservableCollection<DominatorAccountModel>(ServiceLocator
                .Current.GetInstance<IDominatorAccountViewModel>()
                .LstDominatorAccountModel.Where(x => x.AccountBaseModel.AccountNetwork == network));
            var selectedAccount = SocinatorInitialize.GetSocialLibrary(network)
                .GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount;
            PrivateBlacklistUserModel.SelectedAccount = PrivateBlacklistUserModel.LstAccountModel.FirstOrDefault(x =>
                x.UserName == selectedAccount && x.AccountBaseModel.AccountNetwork == network);
            _delayService = InstanceProvider.GetInstance<IDelayService>();
        }

        public override void AddToBlackList(object sender)
        {
            if (string.IsNullOrEmpty(BlacklistUser.Trim()))
            {
                GlobusLogHelper.log.Info("Error:- Please enter an username to add to the Blacklist.");
                return;
            }

            Task.Factory.StartNew(() =>
            {
                var lstUser = BlacklistUser.Split('\n');
                BlacklistUser = string.Empty;
                _blackListUser = new List<PrivateBlacklist>();
                AddToDB(lstUser.ToList());
            });
        }

        public override void AddToDB(List<string> lstuser)
        {
            var lstBlackListUser = _dbAccountService.GetPrivateBlacklistUsers();
            var lstWhitelistUser = _dbAccountService.GetPrivateWhitelistUsers();

            lstuser?.ForEach(user =>
            {
                IsAddingToDB = true;
                if (!IsNeedToStop)
                {
                    var userName = user.Trim();
                    if (!string.IsNullOrEmpty(userName))
                    {
                        if (lstBlackListUser.All(x =>
                                string.Compare(x.UserName, userName, StringComparison.InvariantCultureIgnoreCase) != 0)
                            && lstWhitelistUser.All(x =>
                                string.Compare(x.UserName, userName, StringComparison.InvariantCultureIgnoreCase) != 0))
                            Application.Current.Dispatcher.Invoke(() =>
                                {
                                    _blackListUser.Add(new PrivateBlacklist
                                    {
                                        UserName = userName
                                    });
                                    LstBlackListUsers.Add(new PrivateBlacklistUserModel
                                    {
                                        BlacklistUser = userName
                                    });
                                }
                            );
                        else
                            GlobusLogHelper.log.Info(Log.CustomMessage, _network,
                                userName, UserType.BlackListedUser,
                                $"{userName} already added to Blacklist/Whitelist. Click on refresh button to view updated list.");
                    }
                }
            });
            IsNeedToStop = false;
            IsAddingToDB = false;
            _blackListUser = _blackListUser.GroupBy(x => x.UserName).Select(y => y.First()).ToList();

            _dbAccountService.AddRange(_blackListUser);

            if (_blackListUser.Count > 0)
                ToasterNotification.ShowSuccess(
                    $"Successfully added {_blackListUser.Count} distinct user{(_blackListUser.Count > 1 ? "s" : "")}. Click on refresh button to view updated list");
        }

        public override void Delete(object sender)
        {
            var selectedUser = LstBlackListUsers.Where(x => x.IsBlackListUserChecked).ToList();
            if (selectedUser.Count == 0)
            {
                Dialog.ShowDialog("Alert", "Please select atleast on user");
                return;
            }

            Task.Factory.StartNew(() =>
            {
                if (IsAllBlackListUserChecked && !UsersSearched)
                {
                    if (IsAddingToDB) IsNeedToStop = true;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        LstBlackListUsers.Clear();
                        _dbAccountService.RemoveAll<PrivateBlacklist>();
                    });
                }
                else
                {
                    selectedUser.ForEach(x =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            LstBlackListUsers.Remove(x);
                            _dbAccountService.RemoveMatch<PrivateBlacklist>(user => user.UserName == x.BlacklistUser);
                        });
                        _delayService.ThreadSleep(50);
                    });
                }
            });
        }

        public override void Refresh(object sender)
        {
            IsAllBlackListUserChecked = false;
            LstBlackListUsers.Clear();
            UsersSearched = false;
            _dbAccountService =
                InstanceProvider.ResolveWithDominatorAccount<DbAccountService>(PrivateBlacklistUserModel
                    .SelectedAccount);
            ThreadFactory.Instance.Start(() =>
            {
                _dbAccountService.GetPrivateBlacklistUsers()?.ForEach(user =>
                {
                    IsAddingToDB = true;
                    if (!IsNeedToStop)
                    {
                        Application.Current.Dispatcher.Invoke(() => LstBlackListUsers.Add(
                            new PrivateBlacklistUserModel
                            {
                                BlacklistUser = user.UserName
                            }));
                        _delayService.ThreadSleep(5);
                    }
                });
                IsNeedToStop = false;
                IsAddingToDB = false;
            });
        }
    }
}