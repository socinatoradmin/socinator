using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CommonServiceLocator;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel;
using Dialog = DominatorHouseCore.Utility.Dialog;
using ToasterNotification = DominatorHouseCore.Utility.ToasterNotification;
using GramDominatorCore.GDLibrary.DAL;

namespace GramDominatorCore.GDViewModel
{
    public class PrivateBlackListViewModel : BlackListViewModel, IPrivateBlickListViewModel
    {
        private IDbAccountService _dbAccountService;
        List<PrivateBlacklist> _blackListUser = new List<PrivateBlacklist>();
        private SocialNetworks _network;
        bool IsNeedToStop;
        bool IsAddingToDB;

        bool IsStopLoading;
        bool UsersSearched;

        public PrivateBlackListViewModel(SocialNetworks network)
        {
            _network = network;
            PrivateBlacklistUserModel.LstAccountModel = new ObservableCollection<DominatorAccountModel>(InstanceProvider.GetInstance<IDominatorAccountViewModel>()
                .LstDominatorAccountModel.Where(x => x.AccountBaseModel.AccountNetwork == network));
            var selectedAccount = SocinatorInitialize.GetSocialLibrary(network)
                .GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount;
            PrivateBlacklistUserModel.SelectedAccount = PrivateBlacklistUserModel.LstAccountModel.FirstOrDefault(x => x.UserName == selectedAccount && x.AccountBaseModel.AccountNetwork == network);
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
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    _blackListUser.Add(new PrivateBlacklist()
                                    {
                                        UserName = userName
                                    });
                                    LstBlackListUsers.Add(new PrivateBlacklistUserModel
                                    {
                                        BlacklistUser = userName
                                    });
                                }
                                );
                            }
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, _network,
                                userName, UserType.BlackListedUser,
                                $"{userName} already added to Blacklist/Whitelist. Click on refresh button to view updated list.");
                        }
                    }
                }
                else return;
            });
            IsNeedToStop = false;
            IsAddingToDB = false;
            _blackListUser = _blackListUser.GroupBy(x => x.UserName).Select(y => y.First()).ToList();

            _dbAccountService.AddRange(_blackListUser);

            if (_blackListUser.Count > 0)
                ToasterNotification.ShowSuccess(
                    $"Successfully added {_blackListUser.Count} distinct user{ (_blackListUser.Count > 1 ? "s" : "")}. Click on refresh button to view updated list");
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
                    selectedUser.ForEach(x =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            LstBlackListUsers.Remove(x);
                            _dbAccountService.Remove<PrivateBlacklist>(user => user.UserName == x.BlacklistUser);
                        });
                        Thread.Sleep(50);
                    });
            });
        }

        public override void Refresh(object sender)
        {
            IsAllBlackListUserChecked = false;
            LstBlackListUsers.Clear();
            UsersSearched = false;
            _dbAccountService = InstanceProvider.ResolveWithDominatorAccount<DbAccountService>(PrivateBlacklistUserModel.SelectedAccount);

            ThreadFactory.Instance.Start(() =>
            {
                _dbAccountService.GetPrivateBlacklist()?.ForEach(user =>
                {
                    IsAddingToDB = true;
                    if (!IsNeedToStop)
                    {
                        Application.Current.Dispatcher.Invoke(() => LstBlackListUsers.Add(
                            new PrivateBlacklistUserModel
                            {
                                BlacklistUser = user.UserName
                            }));
                        Thread.Sleep(5);
                    }
                    else return;
                });
                IsNeedToStop = false;
                IsAddingToDB = false;
            });
        }

        public override void Search(object sender)
        {
            LstBlackListUsers.Clear();
            IsAllBlackListUserChecked = false;
            UsersSearched = true;
            _dbAccountService = InstanceProvider.ResolveWithDominatorAccount<DbAccountService>(PrivateBlacklistUserModel.SelectedAccount);

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
                        Application.Current.Dispatcher.Invoke(() => LstBlackListUsers.Add(
                            new BlacklistUserModel
                            {
                                BlacklistUser = user.UserName
                            }));
                        Thread.Sleep(5);
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
            Thread.Sleep(200);
            LstBlackListUsers.Clear();
            IsStopLoading = false;
        }
    }
}
