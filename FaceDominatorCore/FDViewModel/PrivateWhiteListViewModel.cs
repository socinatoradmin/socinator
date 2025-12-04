using CommonServiceLocator;
using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel;
using FaceDominatorCore.FDLibrary.DAL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ThreadUtils;

namespace FaceDominatorCore.FDViewModel
{
    public class PrivateWhiteListViewModel : WhiteListViewModel, IPrivateWhiteListViewModel
    {
        private IDbAccountService _dbAccountService;
        List<PrivateWhitelist> _whiteListUser = new List<PrivateWhitelist>();
        private SocialNetworks _network;
        bool IsNeedToStop;
        bool IsAddingToDB;
        bool IsStopLoading;
        bool UsersSearched;
        private readonly IDelayService _delayService;

        public PrivateWhiteListViewModel(SocialNetworks network)
        {
            _network = network;
            _delayService = InstanceProvider.GetInstance<IDelayService>();
            PrivateWhitelistUserModel.LstAccountModel = new ObservableCollection<DominatorAccountModel>(InstanceProvider.GetInstance<IDominatorAccountViewModel>()
                .LstDominatorAccountModel.Where(x => x.AccountBaseModel.AccountNetwork == network));
            var selectedAccount = SocinatorInitialize.GetSocialLibrary(network)
                .GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount;
            PrivateWhitelistUserModel.SelectedAccount = PrivateWhitelistUserModel.LstAccountModel.FirstOrDefault(x => x.UserName == selectedAccount && x.AccountBaseModel.AccountNetwork == network);
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

                AddToDB(lstUser?.ToList());
            });
        }

        public override void AddToDB(List<string> lstuser)
        {
            var lstBlackListUser = _dbAccountService.GetPrivateBlacklist();
            var lstWhitelistUser = _dbAccountService.GetPrivateWhitelist();

            lstuser?.ForEach(user =>
            {
                IsAddingToDB = true;
                if (!IsNeedToStop)
                {
                    var userName = user.Trim();
                    if (!string.IsNullOrEmpty(userName))
                    {
                        if (lstBlackListUser.All(x => string.Compare(x.UserName, userName, StringComparison.InvariantCultureIgnoreCase) != 0)
                            && lstWhitelistUser.All(x => string.Compare(x.UserName, userName, StringComparison.InvariantCultureIgnoreCase) != 0))
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                _whiteListUser.Add(new PrivateWhitelist()
                                {
                                    UserName = userName
                                });
                                LstWhiteListUsers.Add(new PrivateWhitelistUserModel
                                {

                                    WhitelistUser = userName
                                });
                            }
                            );
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, _network, userName, UserType.WhiteListedUser,
                                $"{userName} already added to Blacklist/Whitelist. Click on refresh button to view updated list.");
                        }
                    }
                }
                else return;
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
                Dialog.ShowDialog("Alert", "Please select atleast one user");
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
                    IsAllWhiteListUserChecked = false;
                }
                else
                    selectedUser.ForEach(x =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            LstWhiteListUsers.Remove(x);
                            _dbAccountService.RemoveAllMatches<PrivateWhitelist>(user => user.UserName == x.WhitelistUser);
                        });
                        _delayService.ThreadSleep(50);
                    });
            });
        }
        public override void Refresh(object sender)
        {
            IsAllWhiteListUserChecked = false;
            LstWhiteListUsers.Clear();
            _dbAccountService = InstanceProvider.ResolveWithDominatorAccount<DbAccountService>(PrivateWhitelistUserModel.SelectedAccount);

            ThreadFactory.Instance.Start(() =>
            {
                StopPreviousProcess();
                _dbAccountService.GetPrivateWhitelist()?.ForEach(user =>
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
                    else return;
                });
                IsNeedToStop = false;
                IsAddingToDB = false;
            });
        }

        public override void Search(object sender)
        {
            LstWhiteListUsers.Clear();
            IsAllWhiteListUserChecked = false;
            _dbAccountService = InstanceProvider.ResolveWithDominatorAccount<DbAccountService>(PrivateWhitelistUserModel.SelectedAccount);

            ThreadFactory.Instance.Start(() =>
            {
                StopPreviousProcess();
                IsAddingToDB = true;

                var listBlacklistData = _dbAccountService.Get<PrivateBlacklist>().ToList() ?? new List<PrivateBlacklist>();
                listBlacklistData.RemoveAll(x => !x.UserName.ToLower().Contains(TextSearch.ToLower()));

                listBlacklistData?.ForEach(user =>
                {
                    IsAddingToDB = true;
                    if (!IsNeedToStop && !IsStopLoading)
                    {
                        Application.Current.Dispatcher.Invoke(() => LstWhiteListUsers.Add(
                            new WhitelistUserModel
                            {
                                WhitelistUser = user.UserName
                            }));
                        _delayService.ThreadSleep(5);
                    }
                    else return;
                });

                IsNeedToStop = false;
                IsAddingToDB = false;
            });
        }

        public override void StopPreviousProcess()
        {
            IsStopLoading = true;
            _delayService.ThreadSleep(200);
            LstWhiteListUsers.Clear();
            IsStopLoading = false;
        }
    }
}
