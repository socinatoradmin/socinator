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
    public class PrivateWhiteListViewModel : WhiteListViewModel, IPrivateWhiteListViewModel
    {
        private readonly IDelayService _delayService;
        private IDbAccountService _dbAccountService;
        private readonly SocialNetworks _network;
        private List<PrivateWhitelist> _whiteListUser = new List<PrivateWhitelist>();
        private bool IsAddingToDB;
        private bool IsNeedToStop;

        private bool IsStopLoading;
        private bool UsersSearched;

        public PrivateWhiteListViewModel(SocialNetworks network)
        {
            _network = network;
            PrivateWhitelistUserModel.LstAccountModel = new ObservableCollection<DominatorAccountModel>(ServiceLocator
                .Current.GetInstance<IDominatorAccountViewModel>()
                .LstDominatorAccountModel.Where(x => x.AccountBaseModel.AccountNetwork == network));
            var selectedAccount = SocinatorInitialize.GetSocialLibrary(network)
                .GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount;
            PrivateWhitelistUserModel.SelectedAccount = PrivateWhitelistUserModel.LstAccountModel.FirstOrDefault(x =>
                x.UserName == selectedAccount && x.AccountBaseModel.AccountNetwork == network);
            _delayService = InstanceProvider.GetInstance<IDelayService>();
        }

        public override void AddToWhiteList(object sender)
        {
            if (string.IsNullOrEmpty(WhitelistUser.Trim()))
            {
                GlobusLogHelper.log.Info("Error:- Please enter an username to add to the Blacklist.");
                return;
            }

            Task.Factory.StartNew(() =>
            {
                var lstUser = WhitelistUser.Split('\n');
                WhitelistUser = string.Empty;
                _whiteListUser = new List<PrivateWhitelist>();

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
                                    _whiteListUser.Add(new PrivateWhitelist
                                    {
                                        UserName = userName
                                    });
                                    LstWhiteListUsers.Add(new PrivateWhitelistUserModel
                                    {
                                        WhitelistUser = userName
                                    });
                                }
                            );
                        else
                            GlobusLogHelper.log.Info(Log.CustomMessage, _network,
                                userName, UserType.WhiteListedUser,
                                userName, UserType.BlackListedUser,
                                $"{userName} already added to Blacklist/Whitelist. Click on refresh button to view updated list.");
                    }
                }
            });
            IsNeedToStop = false;
            IsAddingToDB = false;
            _whiteListUser = _whiteListUser.GroupBy(x => x.UserName).Select(y => y.First()).ToList();

            _dbAccountService.AddRange(_whiteListUser);

            if (_whiteListUser.Count > 0)
                ToasterNotification.ShowSuccess(
                    $"Successfully added {_whiteListUser.Count} distinct user{(_whiteListUser.Count > 1 ? "s" : "")}. Click on refresh button to view updated list");
        }

        public override void Delete(object sender)
        {
            var selectedUser = LstWhiteListUsers.Where(x => x.IsWhiteListUserChecked).ToList();
            if (selectedUser.Count == 0)
            {
                Dialog.ShowDialog("Alert", "Please select atleast on user");
                return;
            }

            Task.Factory.StartNew(() =>
            {
                if (IsAllWhiteListUserChecked && !UsersSearched)
                {
                    if (IsAddingToDB) IsNeedToStop = true;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        LstWhiteListUsers.Clear();
                        _dbAccountService.RemoveAll<PrivateWhitelist>();
                    });
                }
                else
                {
                    selectedUser.ForEach(x =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            LstWhiteListUsers.Remove(x);
                            _dbAccountService.RemoveMatch<PrivateWhitelist>(user => user.UserName == x.WhitelistUser);
                        });

                        _delayService.ThreadSleep(50);
                    });
                }
            });
        }

        public override void Refresh(object sender)
        {
            IsAllWhiteListUserChecked = false;
            LstWhiteListUsers.Clear();
            _dbAccountService =
                InstanceProvider.ResolveWithDominatorAccount<DbAccountService>(PrivateWhitelistUserModel
                    .SelectedAccount);

            ThreadFactory.Instance.Start(() =>
            {
                _dbAccountService.GetPrivateWhitelistUsers()?.ForEach(user =>
                {
                    IsAddingToDB = true;


                    if (!IsNeedToStop && !IsStopLoading)

                    {
                        Application.Current.Dispatcher.Invoke(() => LstWhiteListUsers.Add(
                            new PrivateWhitelistUserModel
                            {
                                WhitelistUser = user.UserName
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