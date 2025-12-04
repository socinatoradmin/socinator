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
    public interface IPrivateWhiteListViewModel
    {
    }

    public class WhiteListViewModel : BindableBase
    {
        public WhiteListViewModel()
        {
            AddToWhiteListCommand = new BaseCommand<object>(sender => true, AddToWhiteList);
            ClearCommand = new BaseCommand<object>(sender => true, ClearUser);
            RefreshCommand = new BaseCommand<object>(sender => true, Refresh);
            SelectCommand = new BaseCommand<object>(sender => true, Select);
            DeleteCommand = new BaseCommand<object>(sender => true, Delete);
            TextSearchCommand = new BaseCommand<object>(sender => true, Search);
            ImportCommand = new DelegateCommand(ImportUser);
            ExportCommand = new DelegateCommand(ExportUser);
            BindingOperations.EnableCollectionSynchronization(LstWhiteListUsers, _lock);
        }

        public void InitializeData()
        {
            DataBaseConnectionGlb = SocinatorInitialize.GetGlobalDatabase();
            DbOperations =
                new DbOperations(DataBaseConnectionGlb.GetSqlConnection(SocinatorInitialize.ActiveSocialNetwork,
                    UserType.WhiteListedUser));

            BlackListDbOperations =
                new DbOperations(DataBaseConnectionGlb.GetSqlConnection(SocinatorInitialize.ActiveSocialNetwork,
                    UserType.BlackListedUser));

            ThreadFactory.Instance.Start(() =>
            {
                DbOperations.Get<WhiteListUser>()?.ForEach(user =>
                {
                    IsAddingToDB = true;
                    if (!IsNeedToStop && !IsStopLoading)
                    {
                        Application.Current.Dispatcher.Invoke(() => LstWhiteListUsers.Add(
                            new WhitelistUserModel
                            {
                                WhitelistUser = user.UserName
                            }));
                        Thread.Sleep(5);
                    }
                });
                IsNeedToStop = false;
                IsAddingToDB = false;
            });
        }

        public virtual void AddToWhiteList(object sender)
        {
            if (string.IsNullOrEmpty(WhitelistUser.Trim()))
            {
                GlobusLogHelper.log.Info("LangKeyErrorEnterUsernameToWhitelist".FromResourceDictionary());
                return;
            }

            Task.Factory.StartNew(() =>
            {
                var lstuser = WhitelistUser.Split('\n');
                WhitelistUser = string.Empty;
                _whiteListUser = new List<WhiteListUser>();
                AddToDB(lstuser?.ToList());
            });
        }

        public virtual void AddToDB(List<string> lstuser)
        {
            var lstBlackListUser = BlackListDbOperations?.Get<BlackListUser>();
            lstuser?.ForEach(user =>
            {
                IsAddingToDB = true;
                if (!IsNeedToStop)
                {
                    var userName = user.Trim();
                    if (!string.IsNullOrEmpty(userName))
                    {
                        if (LstWhiteListUsers.All(x =>
                                string.Compare(x.WhitelistUser, userName,
                                    StringComparison.InvariantCultureIgnoreCase) != 0)
                            && lstBlackListUser.All(x =>
                                string.Compare(x.UserName, userName, StringComparison.InvariantCultureIgnoreCase) != 0))
                            Application.Current.Dispatcher.Invoke(() =>
                                {
                                    _whiteListUser.Add(new WhiteListUser
                                    {
                                        UserName = userName
                                    });
                                }
                            );
                        else
                            GlobusLogHelper.log.Info(Log.CustomMessage, SocinatorInitialize.ActiveSocialNetwork,
                                userName, UserType.WhiteListedUser,
                                $"{userName} {"LangKeyAlreadyAddedToBlacklistWhitelist".FromResourceDictionary()}");
                    }
                }
            });
            IsNeedToStop = false;
            IsAddingToDB = false;
            // Remove duplicates
            _whiteListUser = _whiteListUser.GroupBy(x => x.UserName).Select(y => y.First()).ToList();

            DbOperations.AddRange(_whiteListUser);

            if (_whiteListUser.Count > 0)
                ToasterNotification.ShowSuccess(
                    $"Successfully added {_whiteListUser.Count} distinct user{(_whiteListUser.Count > 1 ? "s" : "")}. {"LangKeyClickRefreshToViewUpdatedList".FromResourceDictionary()}");
            else if (_whiteListUser.Count > 0)
                ToasterNotification.ShowError("LangKeyNoDistinctUsersFound".FromResourceDictionary());
        }

        private void ClearUser(object sender)
        {
            WhitelistUser = string.Empty;
        }

        public virtual void Refresh(object sender)
        {
            LstWhiteListUsers.Clear();
            UsersSearched = false;
            ThreadFactory.Instance.Start(() =>
            {
                StopPreviousProcess();
                DbOperations.Get<WhiteListUser>()?.ForEach(user =>
                {
                    IsAddingToDB = true;
                    if (!IsNeedToStop && !IsStopLoading)
                    {
                        Application.Current.Dispatcher.Invoke(() => LstWhiteListUsers.Add(
                            new WhitelistUserModel
                            {
                                WhitelistUser = user.UserName
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
            IsAllWhiteListUserChecked = false;
            LstWhiteListUsers.Clear();
            UsersSearched = true;
            ThreadFactory.Instance.Start(() =>
            {
                StopPreviousProcess();
                IsAddingToDB = true;

                var listWhiteListData = DbOperations.Get<WhiteListUser>()?.ToList();
                listWhiteListData.RemoveAll(x => !x.UserName.ToLower().Contains(TextSearch.ToLower()));

                listWhiteListData?.ForEach(user =>
                {
                    IsAddingToDB = true;
                    if (!IsNeedToStop)
                    {
                        Application.Current.Dispatcher.Invoke(() => LstWhiteListUsers.Add(
                            new WhitelistUserModel
                            {
                                WhitelistUser = user.UserName
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
            LstWhiteListUsers.Select(x =>
            {
                x.IsWhiteListUserChecked = isChecked;
                return x;
            }).ToList();
        }

        private void Select(object sender)
        {
            if (LstWhiteListUsers.All(x => x.IsWhiteListUserChecked))
            {
                IsAllWhiteListUserChecked = true;
            }

            else
            {
                if (IsAllWhiteListUserChecked && !UsersSearched)
                    IsUnCheckedFromUser = true;
                IsAllWhiteListUserChecked = false;
            }
        }

        public virtual void Delete(object sender)
        {
            var selectedUser = LstWhiteListUsers.Where(x => x.IsWhiteListUserChecked).ToList();
            if (selectedUser.Count == 0)
            {
                Dialog.ShowDialog("LangKeyAlert".FromResourceDictionary(),
                    "LangKeySelectAtLeastOneUser".FromResourceDictionary());
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
                        DbOperations.RemoveAll<WhiteListUser>();
                    });
                    IsAllWhiteListUserChecked = false;
                }
                else
                {
                    selectedUser.ForEach(x =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            LstWhiteListUsers.Remove(x);
                            DbOperations.Remove<WhiteListUser>(user => user.UserName == x.WhitelistUser);
                        });
                        Thread.Sleep(50);
                    });
                }
            });
        }

        private void ExportUser()
        {
            var selectedUsers = LstWhiteListUsers?.Where(x => x.IsWhiteListUserChecked);
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
                $"{exportPath}\\{SocinatorInitialize.ActiveSocialNetwork}_WhiteList {ConstantVariable.DateasFileName}.csv";

            selectedUsers.ForEach(user =>
            {
                try
                {
                    using (var streamWriter = new StreamWriter(filename, true))
                    {
                        streamWriter.WriteLine(user.WhitelistUser);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            });
            Dialog.ShowDialog("LangKeyExportWhiteListUser".FromResourceDictionary(),
                string.Format("LangKeyWhiteListedUserSuccessfullyExportedTo".FromResourceDictionary(), filename));
        }

        private void ImportUser()
        {
            StopPreviousProcess();
            _whiteListUser = new List<WhiteListUser>();
            var lstUser = FileUtilities.FileBrowseAndReader();
            if (lstUser?.Count != 0)
                Task.Factory.StartNew(() => { AddToDB(lstUser); });
        }

        public virtual void StopPreviousProcess()
        {
            IsStopLoading = true;
            Thread.Sleep(200);
            LstWhiteListUsers.Clear();
            IsStopLoading = false;
        }

        #region Commands

        public ICommand AddToWhiteListCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand SelectCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand ImportCommand { get; set; }
        public ICommand ExportCommand { get; set; }
        public ICommand TextSearchCommand { get; set; }

        #endregion

        #region Properties

        private bool IsNeedToStop;
        private bool IsAddingToDB;
        private bool IsStopLoading;

        private static readonly object _lock = new object();
        private IGlobalDatabaseConnection DataBaseConnectionGlb { get; set; }

        private DbOperations DbOperations { get; set; }

        private DbOperations BlackListDbOperations { get; set; }

        private bool IsUnCheckedFromUser { get; set; }

        private bool UsersSearched { get; set; }

        private WhitelistUserModel _whitelistUserModel = new WhitelistUserModel();

        public WhitelistUserModel WhitelistUserModel
        {
            get => _whitelistUserModel;
            set
            {
                if (_whitelistUserModel == value)
                    return;
                _whitelistUserModel = value;
                SetProperty(ref _whitelistUserModel, value);
            }
        }

        private PrivateWhitelistUserModel _privateWhitelistUserModel = new PrivateWhitelistUserModel();

        public PrivateWhitelistUserModel PrivateWhitelistUserModel
        {
            get => _privateWhitelistUserModel;
            set => SetProperty(ref _privateWhitelistUserModel, value);
        }

        private bool _isAllWhiteistUserChecked;

        public bool IsAllWhiteListUserChecked
        {
            get => _isAllWhiteistUserChecked;
            set
            {
                if (value == _isAllWhiteistUserChecked)
                    return;
                SetProperty(ref _isAllWhiteistUserChecked, value);
                SelectAll(_isAllWhiteistUserChecked);
                if (IsUnCheckedFromUser)
                    IsUnCheckedFromUser = false;
            }
        }

        private string _whitelistUser = string.Empty;

        public string WhitelistUser
        {
            get => _whitelistUser;
            set
            {
                if (value == _whitelistUser)
                    return;
                SetProperty(ref _whitelistUser, value);
            }
        }

        private List<WhiteListUser> _whiteListUser = new List<WhiteListUser>();

        private ObservableCollection<WhitelistUserModel> _lstWhiteListUsers =
            new ObservableCollection<WhitelistUserModel>();

        public ObservableCollection<WhitelistUserModel> LstWhiteListUsers
        {
            get => _lstWhiteListUsers;
            set
            {
                if (value == _lstWhiteListUsers)
                    return;
                SetProperty(ref _lstWhiteListUsers, value);
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