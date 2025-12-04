using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore.DatabaseHandler.PdTables.Accounts;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel;
using PinDominatorCore.PDLibrary.DAL;

namespace PinDominatorCore.PDViewModel
{
    public class PrivateBlackListViewModel : BlackListViewModel, IPrivateBlickListViewModel
    {
        private readonly IDelayService _delayService;
        private readonly SocialNetworks _network;
        private List<PrivateBlacklist> _blackListUser = new List<PrivateBlacklist>();
        private IDbAccountService _dbAccountService;
        private bool _isAddingToDb;
        private bool _isNeedToStop;
        private bool _isStopLoading;
        private bool _usersSearched;

        public PrivateBlackListViewModel(SocialNetworks network)
        {
            _network = network;
            _delayService = InstanceProvider.GetInstance<IDelayService>();
            PrivateBlacklistUserModel.LstAccountModel = new ObservableCollection<DominatorAccountModel>(ServiceLocator
                .Current.GetInstance<IDominatorAccountViewModel>()
                .LstDominatorAccountModel.Where(x => x.AccountBaseModel.AccountNetwork == network));
            var selectedAccount = SocinatorInitialize.GetSocialLibrary(network)
                .GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount;
            PrivateBlacklistUserModel.SelectedAccount = PrivateBlacklistUserModel.LstAccountModel.FirstOrDefault(x =>
                x.UserName == selectedAccount && x.AccountBaseModel.AccountNetwork == network);
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
                _isAddingToDb = true;
                if (!_isNeedToStop)
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
            _isNeedToStop = false;
            _isAddingToDb = false;
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
                if (IsAllBlackListUserChecked && !_usersSearched)
                {
                    if (_isAddingToDb) _isNeedToStop = true;
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
                            _dbAccountService.Remove<PrivateBlacklist>(user => user.UserName == x.BlacklistUser);
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
            _usersSearched = false;
            _dbAccountService =
                InstanceProvider.ResolveWithDominatorAccount<DbAccountService>(PrivateBlacklistUserModel
                    .SelectedAccount);

            ThreadFactory.Instance.Start(() =>
            {
                _dbAccountService.GetPrivateBlacklist()?.ForEach(user =>
                {
                    _isAddingToDb = true;
                    if (!_isNeedToStop)
                    {
                        Application.Current.Dispatcher.Invoke(() => LstBlackListUsers.Add(
                            new PrivateBlacklistUserModel
                            {
                                BlacklistUser = user.UserName
                            }));
                        _delayService.ThreadSleep(5);
                    }
                });
                _isNeedToStop = false;
                _isAddingToDb = false;
            });
        }

        public override void Search(object sender)
        {
            LstBlackListUsers.Clear();
            IsAllBlackListUserChecked = false;
            _usersSearched = true;
            _dbAccountService =
                InstanceProvider.ResolveWithDominatorAccount<DbAccountService>(PrivateBlacklistUserModel
                    .SelectedAccount);

            ThreadFactory.Instance.Start(() =>
            {
                StopPreviousProcess();
                _isAddingToDb = true;

                var listBlacklistData =
                    _dbAccountService.Get<PrivateBlacklist>().ToList() ?? new List<PrivateBlacklist>();
                listBlacklistData.RemoveAll(x => !x.UserName.ToLower().Contains(TextSearch.ToLower()));

                listBlacklistData?.ForEach(user =>
                {
                    _isAddingToDb = true;
                    if (!_isNeedToStop && !_isStopLoading)
                    {
                        Application.Current.Dispatcher.Invoke(() => LstBlackListUsers.Add(
                            new BlacklistUserModel
                            {
                                BlacklistUser = user.UserName
                            }));
                        _delayService.ThreadSleep(5);
                    }
                });

                _isNeedToStop = false;
                _isAddingToDb = false;
            });
        }
        
        public override void StopPreviousProcess()
        {
            _isStopLoading = true;
            _delayService.ThreadSleep(200);
            LstBlackListUsers.Clear();
            _isStopLoading = false;
        }
    }
}