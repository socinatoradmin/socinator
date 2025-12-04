using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;

namespace DominatorUIUtility.ViewModel
{
    public interface IPrivateBlickListViewModel
    {
    }

    public class BlackListViewModel : BindableBase
    {
        public BlackListViewModel()
        {
            AddToBlackListCommand = new BaseCommand<object>(sender => true, AddToBlackList);
            ClearCommand = new BaseCommand<object>(sender => true, ClearUser);
            RefreshCommand = new BaseCommand<object>(sender => true, Refresh);
            SelectCommand = new BaseCommand<object>(sender => true, Select);
            DeleteCommand = new BaseCommand<object>(sender => true, Delete);
            TextSearchCommand = new BaseCommand<object>(sender => true, Search);
            ImportCommand = new DelegateCommand(ImportUser);
            ExportCommand = new DelegateCommand(ExportUser);
            BindingOperations.EnableCollectionSynchronization(LstBlackListUsers, _lock);
        }

        public void InitializeData()
        {
            DataBaseConnectionGlb = SocinatorInitialize.GetGlobalDatabase();
            DbOperations =
                new DbOperations(DataBaseConnectionGlb.GetSqlConnection(SocinatorInitialize.ActiveSocialNetwork,
                    UserType.BlackListedUser));

            WhiteListDbOperations =
                new DbOperations(DataBaseConnectionGlb.GetSqlConnection(SocinatorInitialize.ActiveSocialNetwork,
                    UserType.WhiteListedUser));

            Task.Factory.StartNew(() =>
            {
                DbOperations.Get<BlackListUser>()?.ForEach(user =>
                {
                    IsAddingToDB = true;
                    if (!IsNeedToStop)
                    {
                        Application.Current.Dispatcher.Invoke(() => LstBlackListUsers.Add(
                            new BlacklistUserModel
                            {
                                BlacklistUser = user.UserName
                            }));

                        Thread.Sleep(5);
                    }
                });
                IsNeedToStop = false;
                IsAddingToDB = false;
            });
        }

        public virtual void AddToBlackList(object sender)
        {
            if (string.IsNullOrEmpty(BlacklistUser.Trim()))
            {
                GlobusLogHelper.log.Info("LangKeyErrorEnterUsernameToBlacklist".FromResourceDictionary());
                return;
            }

            Task.Factory.StartNew(() =>
            {
                var lstUser = BlacklistUser.Split('\n');
                BlacklistUser = string.Empty;
                _blackListUser = new List<BlackListUser>();
                AddToDB(lstUser.ToList());
            });
        }

        public virtual void AddToDB(List<string> lstuser)
        {
            var lstBlackListUser = DbOperations.Get<BlackListUser>();
            var lstWhitelistUser = WhiteListDbOperations.Get<WhiteListUser>();

            lstuser.ForEach(user =>
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
                                    _blackListUser.Add(new BlackListUser
                                    {
                                        UserName = userName
                                    });
                                }
                            );
                        else
                            GlobusLogHelper.log.Info(Log.CustomMessage, SocinatorInitialize.ActiveSocialNetwork,
                                userName, UserType.BlackListedUser,
                                $"{userName} {"LangKeyAlreadyAddedToBlacklistWhitelist".FromResourceDictionary()}");
                    }
                }
            });
            IsNeedToStop = false;
            IsAddingToDB = false;
            // Remove duplicates
            _blackListUser = _blackListUser.GroupBy(x => x.UserName).Select(y => y.First()).ToList();

            DbOperations.AddRange(_blackListUser);

            if (_blackListUser.Count > 0)
                ToasterNotification.ShowSuccess(
                    $"Successfully added {_blackListUser.Count} distinct user{(_blackListUser.Count > 1 ? "s" : "")}. {"LangKeyClickRefreshToViewUpdatedList".FromResourceDictionary()}");
        }

        private void ClearUser(object sender)
        {
            BlacklistUser = string.Empty;
        }

        public virtual void Refresh(object sender)
        {
            LstBlackListUsers.Clear();
            UsersSearched = false;
            ThreadFactory.Instance.Start(() =>
            {
                StopPreviousProcess();
                DbOperations.Get<BlackListUser>()?.ForEach(user =>
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
                });
                IsNeedToStop = false;
                IsAddingToDB = false;
            });
        }

        public virtual void Search(object sender)
        {
            LstBlackListUsers.Clear();
            IsAllBlackListUserChecked = false;
            UsersSearched = true;
            ThreadFactory.Instance.Start(() =>
            {
                StopPreviousProcess();
                IsAddingToDB = true;

                var listBlacklistData = DbOperations.Get<BlackListUser>() ?? new List<BlackListUser>();
                listBlacklistData?.RemoveAll(x => !x.UserName.ToLower().Contains(TextSearch.ToLower()));

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
                });

                IsNeedToStop = false;
                IsAddingToDB = false;
            });
        }

        private void SelectAll(bool isChecked)
        {
            if (IsUnCheckedFromUser)
                return;
            LstBlackListUsers.Select(x =>
            {
                x.IsBlackListUserChecked = isChecked;
                return x;
            }).ToList();
        }

        private void Select(object sender)
        {
            if (LstBlackListUsers.All(x => x.IsBlackListUserChecked))
            {
                IsAllBlackListUserChecked = true;
            }

            else
            {
                if (IsAllBlackListUserChecked && !UsersSearched)
                    IsUnCheckedFromUser = true;
                IsAllBlackListUserChecked = false;
            }
        }

        public virtual void Delete(object sender)
        {
            var selectedUser = LstBlackListUsers.Where(x => x.IsBlackListUserChecked).ToList();
            if (selectedUser.Count == 0)
            {
                Dialog.ShowDialog("LangKeyAlert".FromResourceDictionary(),
                    "LangKeySelectAtLeastOneUser".FromResourceDictionary());
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
                        DbOperations.RemoveAll<BlackListUser>();
                    });
                    IsAllBlackListUserChecked = false;
                }
                else
                {
                    selectedUser.ForEach(x =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            LstBlackListUsers.Remove(x);
                            DbOperations.Remove<BlackListUser>(user => user.UserName == x.BlacklistUser);
                        });
                        Thread.Sleep(2);
                    });
                }
            });
        }

        private void ExportUser()
        {
            var selectedUsers = LstBlackListUsers?.Where(x => x.IsBlackListUserChecked);
            if (selectedUsers?.Count() == 0)
            {
                Dialog.ShowDialog("LangKeyAlert".FromResourceDictionary(),
                    "LangKeySelectAtLeastOneUser".FromResourceDictionary());
                return;
            }

            var exportPath = FileUtilities.GetExportPath();

            if (string.IsNullOrEmpty(exportPath))
                return;


            var filename =
                $"{exportPath}\\{SocinatorInitialize.ActiveSocialNetwork}_BlackList {ConstantVariable.DateasFileName}.csv";

            selectedUsers.ForEach(user =>
            {
                try
                {
                    using (var streamWriter = new StreamWriter(filename, true))
                    {
                        streamWriter.WriteLine(user.BlacklistUser);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            });
            Dialog.ShowDialog("LangKeyExportBlackListUser".FromResourceDictionary(),
                $"{"LangKeyBlackListedUserSuccessfullyExportedTo".FromResourceDictionary()} [ {filename} ]");
        }

        private void ImportUser()
        {
            _blackListUser = new List<BlackListUser>();

            var lstUser = FileUtilities.FileBrowseAndReader();
            if (lstUser?.Count != 0)
                AddToDB(lstUser);
        }

        public virtual void StopPreviousProcess()
        {
            IsStopLoading = true;
            Thread.Sleep(200);
            LstBlackListUsers.Clear();
            IsStopLoading = false;
        }

        #region Commands

        public ICommand AddToBlackListCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand SelectCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand ImportCommand { get; set; }
        public ICommand ExportCommand { get; set; }
        public ICommand TextSearchCommand { get; set; }

        #endregion

        #region Properties

        private static readonly object _lock = new object();
        private IGlobalDatabaseConnection DataBaseConnectionGlb { get; set; }

        private DbOperations DbOperations { get; set; }

        private DbOperations WhiteListDbOperations { get; set; }

        private bool IsUnCheckedFromUser { get; set; }

        private bool UsersSearched { get; set; }

        private PrivateBlacklistUserModel _privateBlacklistUserModel = new PrivateBlacklistUserModel();

        public PrivateBlacklistUserModel PrivateBlacklistUserModel
        {
            get => _privateBlacklistUserModel;
            set
            {
                if (_privateBlacklistUserModel == value)
                    return;
                _privateBlacklistUserModel = value;
                SetProperty(ref _privateBlacklistUserModel, value);
            }
        }

        private BlacklistUserModel _blacklistUserModel = new BlacklistUserModel();

        public BlacklistUserModel BlacklistUserModel
        {
            get => _blacklistUserModel;
            set
            {
                if (_blacklistUserModel == value)
                    return;
                _blacklistUserModel = value;
                SetProperty(ref _blacklistUserModel, value);
            }
        }

        private bool _isAllBlackListUserChecked;

        public bool IsAllBlackListUserChecked
        {
            get => _isAllBlackListUserChecked;
            set
            {
                if (value == _isAllBlackListUserChecked)
                    return;
                SetProperty(ref _isAllBlackListUserChecked, value);

                SelectAll(_isAllBlackListUserChecked);
                if (IsUnCheckedFromUser)
                    IsUnCheckedFromUser = false;
            }
        }

        private string _blacklistUser = string.Empty;
        private bool IsNeedToStop;
        private bool IsAddingToDB;
        private bool IsStopLoading;

        public string BlacklistUser
        {
            get => _blacklistUser;
            set
            {
                if (value == _blacklistUser)
                    return;
                SetProperty(ref _blacklistUser, value);
            }
        }

        private List<BlackListUser> _blackListUser = new List<BlackListUser>();

        private ObservableCollection<BlacklistUserModel> _lstBlackListUsers =
            new ObservableCollection<BlacklistUserModel>();

        public ObservableCollection<BlacklistUserModel> LstBlackListUsers
        {
            get => _lstBlackListUsers;
            set
            {
                if (value == _lstBlackListUsers)
                    return;
                SetProperty(ref _lstBlackListUsers, value);
            }
        }

        private string _textSearch = string.Empty;

        public string TextSearch
        {
            get => _textSearch;
            set
            {
                if (value == _textSearch)
                    return;
                SetProperty(ref _textSearch, value);
            }
        }

        #endregion
    }
}