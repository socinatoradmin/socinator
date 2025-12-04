using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.Views.AccountSetting.CustomControl;
using DominatorUIUtility.Views.ViewModel.Startup.ModuleConfig;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public class BroadcastMessagesModel : BindableBase
    {
        private string _GroupUrlInput;
        private List<string> _GroupUrlList;
        private bool _IsCheckedBySoftware;
        private bool _IsCheckedLangKeyCustomUserList;
        private bool _IsCheckedOutSideSoftware;
        private bool _IsChkGroupBlackList;
        private bool _IsChkPrivateBlackList;
        private bool _IsChkSkipBlackListedUser;

        private bool _IsChkSpintaxChecked;
        private bool _IsChkTagChecked;

        private bool _IsConnections = true;
        private bool _IsGroup;

        private bool _isSpintaxChecked;

        private ObservableCollection<ManageMessagesModel> _lstDisplayManageMessageModel =
            new ObservableCollection<ManageMessagesModel>();

        private ManageMessagesModel _manageMessagesModel = new ManageMessagesModel();
        private string _UrlInput;
        private List<string> _UrlList;

        public ObservableCollection<ManageMessagesModel> LstDisplayManageMessageModel
        {
            get => _lstDisplayManageMessageModel;
            set
            {
                if (_lstDisplayManageMessageModel != value)
                    SetProperty(ref _lstDisplayManageMessageModel, value);
            }
        }

        public ManageMessagesModel ManageMessagesModel
        {
            get => _manageMessagesModel;
            set => SetProperty(ref _manageMessagesModel, value);
        }

        public bool IsSpintaxChecked
        {
            get => _isSpintaxChecked;
            set => SetProperty(ref _isSpintaxChecked, value);
        }

        public bool IsCheckedBySoftware
        {
            get => _IsCheckedBySoftware;
            set => SetProperty(ref _IsCheckedBySoftware, value);
        }


        public bool IsCheckedOutSideSoftware
        {
            get => _IsCheckedOutSideSoftware;
            set => SetProperty(ref _IsCheckedOutSideSoftware, value);
        }


        public bool IsCheckedLangKeyCustomUserList
        {
            get => _IsCheckedLangKeyCustomUserList;
            set => SetProperty(ref _IsCheckedLangKeyCustomUserList, value);
        }


        public string UrlInput
        {
            get => _UrlInput;
            set => SetProperty(ref _UrlInput, value);
        }

        public List<string> UrlList
        {
            get => _UrlList;
            set => SetProperty(ref _UrlList, value);
        }

        public List<string> GroupUrlList
        {
            get => _GroupUrlList;
            set => SetProperty(ref _GroupUrlList, value);
        }


        public bool IsChkSkipBlackListedUser
        {
            get => _IsChkSkipBlackListedUser;
            set => SetProperty(ref _IsChkSkipBlackListedUser, value);
        }


        public bool IsChkPrivateBlackList
        {
            get => _IsChkPrivateBlackList;
            set => SetProperty(ref _IsChkPrivateBlackList, value);
        }


        public bool IsChkGroupBlackList
        {
            get => _IsChkGroupBlackList;
            set => SetProperty(ref _IsChkGroupBlackList, value);
        }

        public bool IsConnections
        {
            get => _IsConnections;
            set => SetProperty(ref _IsConnections, value);
        }

        public bool IsGroup
        {
            get => _IsGroup;
            set => SetProperty(ref _IsGroup, value);
        }

        public string GroupUrlInput
        {
            get => _GroupUrlInput;
            set => SetProperty(ref _GroupUrlInput, value);
        }
    }

    public interface IBroadcastMessagesViewModel
    {
    }

    public class BroadcastMessagesViewModel : StartupBaseViewModel, IBroadcastMessagesViewModel
    {
        public BroadcastMessagesViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.BroadcastMessages});
            NextCommand = new DelegateCommand(NavigateNext);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            AddMessagesCommand = new DelegateCommand<object>(AddMessages);
            AddQueryToMessageCommand = new DelegateCommand<object>(AddQueryToMessageControl);
            AddMultiMediaMessageCommand = new DelegateCommand<object>(AddMultiMediaMessages);

            DeleteQueryCommand = new DelegateCommand<object>(DeleteQuery);
            DeleteMultipleCommand = new DelegateCommand(DeleteMultiple);
            SaveCustomUserListCommand = new DelegateCommand<object>(SaveCustomUsers);
            SaveCustomGroupListCommand = new DelegateCommand<object>(SaveCustomGroup);
            ElementsVisibility.NetworkElementsVisibilty(this);

            if (LinkedInElementsVisibility == Visibility.Visible)
            {
                IsNonQuery = true;
                AllVisibility = Visibility.Collapsed;
                AddQueries();
            }

            if (FacebookElementsVisibility == Visibility.Visible) FaceBookIn = true;

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfMessagesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfMessagesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfMessagesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfMessagesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxMessagesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }

        public Visibility LinkedInElementsVisibility { get; set; } = Visibility.Collapsed;
        public Visibility AllVisibility { get; set; } = Visibility.Visible;
        public Visibility FacebookElementsVisibility { get; set; } = Visibility.Collapsed;
        public bool FaceBookIn { get; set; }

        #region Command

        public ICommand AddMessagesCommand { get; set; }
        public ICommand AddQueryToMessageCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand DeleteMultipleCommand { get; set; }
        public ICommand SaveCustomUserListCommand { get; set; }
        public ICommand SaveCustomGroupListCommand { get; set; }
        public ICommand AddMultiMediaMessageCommand { get; set; }

        #endregion

        #region Properties

        private bool _isSpintaxChecked;

        public bool IsSpintaxChecked
        {
            get => _isSpintaxChecked;
            set => SetProperty(ref _isSpintaxChecked, value);
        }

        private bool _IsCheckedBySoftware;
        private bool _IsCheckedOutSideSoftware;
        private bool _IsCheckedLangKeyCustomUserList;
        private bool _IsChkSkipBlackListedUser;
        private string _UrlInput;
        private bool _IsChkPrivateBlackList;
        private bool _IsChkGroupBlackList;

        private bool _IsConnections = true;
        private bool _IsGroup;
        private List<string> _UrlList;

        public bool IsCheckedBySoftware
        {
            get => _IsCheckedBySoftware;
            set => SetProperty(ref _IsCheckedBySoftware, value);
        }

        public bool IsCheckedOutSideSoftware
        {
            get => _IsCheckedOutSideSoftware;
            set => SetProperty(ref _IsCheckedOutSideSoftware, value);
        }

        public bool IsCheckedLangKeyCustomUserList
        {
            get => _IsCheckedLangKeyCustomUserList;
            set => SetProperty(ref _IsCheckedLangKeyCustomUserList, value);
        }

        public string UrlInput
        {
            get => _UrlInput;
            set => SetProperty(ref _UrlInput, value);
        }

        public List<string> UrlList
        {
            get => _UrlList;
            set => SetProperty(ref _UrlList, value);
        }

        private List<string> _GroupUrlList;

        public List<string> GroupUrlList
        {
            get => _GroupUrlList;
            set => SetProperty(ref _GroupUrlList, value);
        }

        public bool IsChkSkipBlackListedUser
        {
            get => _IsChkSkipBlackListedUser;
            set => SetProperty(ref _IsChkSkipBlackListedUser, value);
        }

        public bool IsChkPrivateBlackList
        {
            get => _IsChkPrivateBlackList;
            set => SetProperty(ref _IsChkPrivateBlackList, value);
        }

        public bool IsChkGroupBlackList
        {
            get => _IsChkGroupBlackList;
            set => SetProperty(ref _IsChkGroupBlackList, value);
        }

        public bool IsConnections
        {
            get => _IsConnections;
            set => SetProperty(ref _IsConnections, value);
        }

        public bool IsGroup
        {
            get => _IsGroup;
            set => SetProperty(ref _IsGroup, value);
        }

        private BroadcastMessagesModel _broadcastMessagesModel = new BroadcastMessagesModel();

        //public BroadcastMessagesModel BroadcastMessagesModel
        //{
        //    get
        //    {
        //        return _broadcastMessagesModel;
        //    }
        //    set
        //    {
        //        SetProperty(ref _broadcastMessagesModel, value);
        //    }
        //}
        private ObservableCollection<ManageMessagesModel> _lstDisplayManageMessageModel =
            new ObservableCollection<ManageMessagesModel>();

        public ObservableCollection<ManageMessagesModel> LstDisplayManageMessageModel
        {
            get => _lstDisplayManageMessageModel;
            set => SetProperty(ref _lstDisplayManageMessageModel, value);
        }

        private string _GroupUrlInput;

        public string GroupUrlInput
        {
            get => _GroupUrlInput;
            set => SetProperty(ref _GroupUrlInput, value);
        }

        private ManageMessagesModel _manageMessagesModel = new ManageMessagesModel();

        public ManageMessagesModel ManageMessagesModel
        {
            get => _manageMessagesModel;
            set => SetProperty(ref _manageMessagesModel, value);
        }

        private bool _isMessageAsPreview;

        public bool IsMessageAsPreview
        {
            get => _isMessageAsPreview;
            set
            {
                if (value == _isMessageAsPreview)
                    return;
                SetProperty(ref _isMessageAsPreview, value);
            }
        }

        private bool _isTagChecked;

        public bool IsTagChecked
        {
            get => _isTagChecked;
            set
            {
                if (value == _isTagChecked)
                    return;
                SetProperty(ref _isTagChecked, value);
            }
        }

        #endregion

        #region Methods

        private void AddMultiMediaMessages(object sender)
        {
            try
            {
                var messageData = sender as MessageMediaControl;

                if (messageData == null) return;

                messageData.Messages.SelectedQuery =
                    new ObservableCollection<QueryContent>(
                        messageData.Messages.LstQueries.Where(x => x.IsContentSelected));

                if (messageData.Messages.SelectedQuery.Count == 0)
                {
                    Dialog.ShowDialog("Warning", "Please select atleast one query!!");
                    return;
                }

                if (string.IsNullOrEmpty(messageData.Messages.MessagesText))
                {
                    Dialog.ShowDialog("Warning", "Please enter message text!!");
                    return;
                }

                messageData.Messages.SelectedQuery.Remove(
                    messageData.Messages.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));

                LstDisplayManageMessageModel.Add(messageData.Messages);

                messageData.Messages = new ManageMessagesModel
                {
                    LstQueries = ManageMessagesModel.LstQueries
                };

                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                messageData.Messages.LstQueries.Select(query =>
                {
                    query.IsContentSelected = false;
                    return query;
                }).ToList();

                ManageMessagesModel = messageData.Messages;

                messageData.ComboBoxQueries.ItemsSource = ManageMessagesModel.LstQueries;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddMessages(object sender)
        {
            try
            {
                var messageData = sender as MessageMediaControl;

                if (messageData == null) return;

                messageData.Messages.SelectedQuery =
                    new ObservableCollection<QueryContent>(
                        messageData.Messages.LstQueries.Where(x => x.IsContentSelected));

                if (messageData.Messages.SelectedQuery.Count == 0)
                {
                    Dialog.ShowDialog("Warning", "Please select atleast one query!!");
                    return;
                }

                if (string.IsNullOrEmpty(messageData.Messages.MessagesText))
                {
                    Dialog.ShowDialog("Warning", "Please enter message text!!");
                    return;
                }

                messageData.Messages.SelectedQuery.Remove(
                    messageData.Messages.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));

                LstDisplayManageMessageModel.Add(messageData.Messages);

                messageData.Messages = new ManageMessagesModel
                {
                    LstQueries = ManageMessagesModel.LstQueries
                };

                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                messageData.Messages.LstQueries.Select(query =>
                {
                    query.IsContentSelected = false;
                    return query;
                }).ToList();

                ManageMessagesModel = messageData.Messages;

                messageData.ComboBoxQueries.ItemsSource = ManageMessagesModel.LstQueries;

                //var messageData = sender as MessagesControl;

                //if (messageData == null) return;

                //messageData.Messages.SelectedQuery = new ObservableCollection<QueryContent>(messageData.Messages.LstQueries.Where(x => x.IsContentSelected));

                //if (messageData.Messages.SelectedQuery.Count == 0 || string.IsNullOrEmpty(messageData.Messages.MessagesText))
                //{
                //    Dialog.ShowDialog("Warning", "May be you didn't select any query or message is missing.");
                //    return;
                //}

                //if (messageData.Messages.SelectedQuery.Count == 1 &&
                //    messageData.Messages.SelectedQuery[0].Content.QueryType == "All")
                //{
                //    Dialog.ShowDialog("Warning", "May be you didn't select any query or message is missing.");
                //    return;
                //}
                //messageData.Messages.SelectedQuery.Remove(messageData.Messages.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));

                //LstDisplayManageMessageModel.Add(messageData.Messages);

                //messageData.Messages = new ManageMessagesModel
                //{
                //    LstQueries = ManageMessagesModel.LstQueries
                //};
                //messageData.Messages.LstQueries.Select(x =>
                //{
                //    x.IsContentSelected = false;
                //    return x;
                //}).ToList();

                //ManageMessagesModel = messageData.Messages;
                //messageData.ComboBoxQueries.ItemsSource = ManageMessagesModel.LstQueries;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddQueryToMessageControl(object sender)
        {
            try
            {
                var activitySetting = sender as ActivitySettingWithoutButton;
                if (!ManageMessagesModel.LstQueries.Any(x =>
                    activitySetting != null &&
                    x.Content.QueryValue == activitySetting.QueryControl.CurrentQuery.QueryValue &&
                    x.Content.QueryType == activitySetting.QueryControl.CurrentQuery.QueryType))
                {
                    if (activitySetting != null && activitySetting.QueryControl.CurrentQuery.QueryValue.Contains(","))
                    {
                        activitySetting.QueryControl.CurrentQuery.QueryValue.Split(',')
                            .Where(x => !string.IsNullOrEmpty(x.Trim())).Distinct().ForEach(query =>
                            {
                                var newquery = new QueryContent
                                {
                                    Content = new QueryInfo
                                    {
                                        QueryValue = query,
                                        QueryType = activitySetting.QueryControl.CurrentQuery.QueryType
                                    }
                                };
                                ManageMessagesModel.LstQueries.Add(newquery);
                                LstDisplayManageMessageModel.ForEach(x =>
                                {
                                    if (!x.LstQueries.Any(y =>
                                        newquery.Content.QueryType ==
                                        activitySetting.QueryControl.CurrentQuery.QueryType &&
                                        y.Content.QueryValue == newquery.Content.QueryValue))
                                        x.LstQueries.Add(newquery);
                                });
                            });
                    }
                    else if (activitySetting != null)
                    {
                        var newquery = new QueryContent
                        {
                            Content = new QueryInfo
                            {
                                QueryValue = activitySetting.QueryControl.CurrentQuery.QueryValue,
                                QueryType = activitySetting.QueryControl.CurrentQuery.QueryType
                            }
                        };
                        ManageMessagesModel.LstQueries.Add(newquery);
                        LstDisplayManageMessageModel.ForEach(x =>
                        {
                            if (!x.LstQueries.Any(y =>
                                newquery.Content.QueryType ==
                                activitySetting.QueryControl.CurrentQuery.QueryType &&
                                y.Content.QueryValue == newquery.Content.QueryValue))
                                x.LstQueries.Add(newquery);
                        });
                    }
                }

                if (ManageMessagesModel.LstQueries[0].IsContentSelected)
                    ManageMessagesModel.LstQueries.Select(x =>
                    {
                        x.IsContentSelected = true;
                        return x;
                    }).ToList();
                AddQueryCommand.Execute(sender);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeleteQuery(object sender)
        {
            try
            {
                var currentQuery = sender as QueryInfo;

                var queryToDelete = ManageMessagesModel.LstQueries.FirstOrDefault(x =>
                    currentQuery != null && x.Content.QueryValue == currentQuery.QueryValue &&
                    x.Content.QueryType == currentQuery.QueryType);

                if (SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                    SavedQueries.Remove(currentQuery);

                ManageMessagesModel.LstQueries.Remove(queryToDelete);
                foreach (var message in LstDisplayManageMessageModel.ToList())
                {
                    var queryDelete = message.SelectedQuery.FirstOrDefault(x =>
                        currentQuery != null && x.Content.QueryType == currentQuery.QueryType &&
                        x.Content.QueryValue == currentQuery.QueryValue);
                    message.SelectedQuery.Remove(queryDelete);

                    if (message.SelectedQuery.Count == 0)
                        LstDisplayManageMessageModel.Remove(message);
                }

                if (!ManageMessagesModel.LstQueries.Skip(1).Any())
                    ManageMessagesModel.LstQueries[0].IsContentSelected = false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeleteMultiple()
        {
            var selectedQuery = SavedQueries.Where(x => x.IsQuerySelected).ToList();
            try
            {
                foreach (var currentQuery in selectedQuery)
                    try
                    {
                        var queryToDelete = ManageMessagesModel.LstQueries.FirstOrDefault(x =>
                            x.Content.QueryValue == currentQuery.QueryValue
                            && x.Content.QueryType == currentQuery.QueryType);

                        if (SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                            SavedQueries.Remove(currentQuery);

                        ManageMessagesModel.LstQueries.Remove(queryToDelete);
                        foreach (var message in LstDisplayManageMessageModel.ToList())
                        {
                            message.SelectedQuery.Remove(queryToDelete);
                            if (message.SelectedQuery.Count == 0)
                                LstDisplayManageMessageModel.Remove(message);
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void SaveCustomUsers(object sender)
        {
            try
            {
                if (UrlInput.Contains("\r\n"))
                {
                    UrlList =
                        Regex.Split(UrlInput, "\r\n").Where(x => !string.IsNullOrEmpty(x.Trim())).Distinct().ToList();

                    GlobusLogHelper.log.Info("" + UrlList.Count + " profile urls saved sucessfully");
                }
                else
                {
                    UrlList = new List<string>();
                    UrlList.Add(UrlInput);
                    GlobusLogHelper.log.Info("One profile url saved sucessfully");
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void SaveCustomGroup(object sender)
        {
            try
            {
                if (GroupUrlInput.Contains("\r\n"))
                {
                    GroupUrlList =
                        Regex.Split(GroupUrlInput, "\r\n").Where(x => !string.IsNullOrEmpty(x.Trim())).Distinct()
                            .ToList();

                    GlobusLogHelper.log.Info("" + GroupUrlList.Count + " group urls saved sucessfully");
                }
                else
                {
                    GroupUrlList = new List<string> {UrlInput};
                    GlobusLogHelper.log.Info("One group url saved sucessfully");
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void AddQueries()
        {
            AddQuery("All");
            AddQuery("LangKeyBySoftware");
            AddQuery("LangKeyOutsideSoftware");
            AddQuery("LangKeyCustomUsersList");
            AddQuery("LangKeyCustomGroupUrl");
        }

        public void AddQuery(string keyResource)
        {
            try
            {
                var queryValue = keyResource.Equals("All") ? "All" : keyResource?.FromResourceDictionary();
                if (ManageMessagesModel.LstQueries.All(x => x.Content.QueryValue != queryValue))
                    ManageMessagesModel.LstQueries.Add(new QueryContent
                    {
                        Content = new QueryInfo
                        {
                            QueryValue = queryValue
                        }
                    });
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }

        #endregion
    }
}