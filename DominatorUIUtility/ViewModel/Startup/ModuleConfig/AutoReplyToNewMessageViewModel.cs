using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.Views.ViewModel.Startup.ModuleConfig;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IAutoReplyToNewMessageViewModel : IFacebookModel
    {
    }

    public class AutoReplyToNewMessageViewModel : StartupBaseViewModel, IAutoReplyToNewMessageViewModel
    {
        private AutoReplyOptionModel _autoReplyOptionModel = new AutoReplyOptionModel();

        private bool _isChkAutoReplayGroupBlacklist;

        private bool _isChkAutoReplayPrivateBlacklist;

        private bool _IsChkMakeCaptionAsSpinText;

        private bool _isReplyToAllMessages﻿﻿Checked;

        private bool _isReplyToConnectedPeople﻿﻿﻿﻿Checked;
        private bool _isReplyToMessagesThatContainSpecificWord﻿Checked;
        private bool _isReplyToPendingMessages﻿﻿Checked;
        private List<string> _lstMessage = new List<string>();

        private List<string> _lstMultiMessageForUserHasNotReplied = new List<string>();
        private List<string> _lstMultiMessageForUserHasReplied = new List<string>();

        private string _message;
        private string _specificWord;

        public AutoReplyToNewMessageViewModel(IRegionManager region) : base(region)
        {
            ManageMessagesModel.LstQueries.Add(new QueryContent
                {Content = new QueryInfo {QueryType = "All", QueryValue = "All"}});
            IsNonQuery = true;
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.AutoReplyToNewMessage});

            ElementsVisibility.NetworkElementsVisibilty(this);

            if (FacebookElementsVisibility == Visibility.Visible)
                AllMessagesVisibility = Visibility.Collapsed;
            if (QuoraElementsVisibility == Visibility.Visible)
                MessagesVisibility = Visibility.Collapsed;
            if (LinkedInElementsVisibility == Visibility.Visible)
                IsLinkedIn = true;
            if (FacebookElementsVisibility == Visibility.Visible)
                IsFacebook = true;

            NextCommand = new DelegateCommand(AutoReplyToNewMessageValidation);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            AddMessagesCommand = new DelegateCommand<object>(AddMessages);
            InputSaveCommand = new DelegateCommand<object>(SaveInput);
            CheckedChangedCommand = new BaseCommand<object>(sender => true, CheckedChangedExecute);
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

            AutoReplyOptionModel = new AutoReplyOptionModel
            {
                BySoftwareDisplayName =
                    "LangKeyReplyToNewPendingMessagesReplyToMessageRequests".FromResourceDictionary(),
                OutsideSoftwareDisplayName = "LangKeyReplyToConnectedPeopleReplyToThePeopleWhoAreConnectedInMessanger"
                    .FromResourceDictionary()
            };
        }

        public Visibility QuoraElementsVisibility { get; set; } = Visibility.Collapsed;
        public Visibility AllMessagesVisibility { get; set; } = Visibility.Visible;
        public Visibility MessagesVisibility { get; set; } = Visibility.Visible;
        public Visibility LinkedInElementsVisibility { get; set; } = Visibility.Collapsed;
        public bool IsLinkedIn { get; set; }
        public bool IsFacebook { get; set; }
        public ICommand CheckedChangedCommand { get; set; }
        public ICommand AddMessagesCommand { get; set; }
        public ICommand InputSaveCommand { get; set; }

        public List<string> LstMessage
        {
            get => _lstMessage;
            set
            {
                if (_lstMessage == value)
                    return;
                SetProperty(ref _lstMessage, value);
            }
        }

        public bool IsReplyToMessagesThatContainSpecificWord﻿Checked
        {
            get => _isReplyToMessagesThatContainSpecificWord﻿Checked;
            set
            {
                if (_isReplyToMessagesThatContainSpecificWord﻿Checked == value)
                    return;
                SetProperty(ref _isReplyToMessagesThatContainSpecificWord﻿Checked, value);
                FbMethod();
                if (!_isReplyToMessagesThatContainSpecificWord﻿Checked)
                {
                    SpecificWord = string.Empty;
                    var count = ManageMessagesModel.LstQueries.Count;
                    while (count > 1)
                    {
                        var Content = ManageMessagesModel.LstQueries[count - 1].Content;
                        if (Content.QueryValue != "Default" &&
                            Content.QueryValue != "LangKeyReplyToAllMessages"?.FromResourceDictionary())
                            ManageMessagesModel.LstQueries.RemoveAt(count - 1);
                        count--;
                    }
                }
            }
        }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public bool IsReplyToPendingMessages﻿﻿Checked
        {
            get => _isReplyToPendingMessages﻿﻿Checked;
            set
            {
                if (_isReplyToPendingMessages﻿﻿Checked == value)
                    return;
                SetProperty(ref _isReplyToPendingMessages﻿﻿Checked, value);
                if (_isReplyToPendingMessages﻿﻿Checked)
                {
                    FbMethod();
                    AddQuery("LangKeyReplyToNewPendingMessagesReplyOnlyMessageSentByUsersThatDontFollowYourAccount");
                }
                else
                {
                    RemoveQuery("LangKeyReplyToNewPendingMessagesReplyOnlyMessageSentByUsersThatDontFollowYourAccount");
                }
            }
        }

        public bool IsReplyToConnectedPeople﻿﻿﻿﻿Checked
        {
            get => _isReplyToConnectedPeople﻿﻿Checked;
            set
            {
                if (_isReplyToConnectedPeople﻿﻿Checked == value)
                    return;
                SetProperty(ref _isReplyToConnectedPeople﻿﻿Checked, value);
                if (_isReplyToConnectedPeople﻿﻿Checked == true)
                    FbMethod();
            }
        }

        public bool IsReplyToAllMessagesChecked
        {
            get => _isReplyToAllMessages﻿﻿Checked;
            set
            {
                SetProperty(ref _isReplyToAllMessages﻿﻿Checked, value);
                if (IsReplyToAllMessagesChecked)
                    AddQuery("LangKeyReplyToAllMessages");

                else
                    RemoveQuery("LangKeyReplyToAllMessages");
            }
        }

        public string SpecificWord
        {
            get => _specificWord;
            set => SetProperty(ref _specificWord, value);
        }

        public bool IsChkMakeCaptionAsSpinText
        {
            get => _IsChkMakeCaptionAsSpinText;
            set
            {
                if (_IsChkMakeCaptionAsSpinText == value)
                    return;
                SetProperty(ref _IsChkMakeCaptionAsSpinText, value);
            }
        }

        public bool IsChkAutoReplyPrivateBlacklist
        {
            get => _isChkAutoReplayPrivateBlacklist;
            set
            {
                if (_isChkAutoReplayPrivateBlacklist == value)
                    return;
                SetProperty(ref _isChkAutoReplayPrivateBlacklist, value);
            }
        }

        public bool IsChkAutoReplyGroupBlacklist
        {
            get => _isChkAutoReplayGroupBlacklist;
            set
            {
                if (_isChkAutoReplayGroupBlacklist == value)
                    return;
                SetProperty(ref _isChkAutoReplayGroupBlacklist, value);
            }
        }

        public List<string> LstMultiMessageForUserHasNotReplied
        {
            get => _lstMultiMessageForUserHasNotReplied;

            set
            {
                if (_lstMultiMessageForUserHasNotReplied != value)
                    SetProperty(ref _lstMultiMessageForUserHasNotReplied, value);
            }
        }

        public List<string> LstMultiMessageForUserHasReplied
        {
            get => _lstMultiMessageForUserHasReplied;

            set
            {
                if (_lstMultiMessageForUserHasReplied != value)
                    SetProperty(ref _lstMultiMessageForUserHasReplied, value);
            }
        }

        public ObservableCollection<ManageMessagesModel> LstDisplayManageMessageModel { get; set; } =
            new ObservableCollection<ManageMessagesModel>();

        public ManageMessagesModel ManageMessagesModel { get; set; } = new ManageMessagesModel();

        public AutoReplyOptionModel AutoReplyOptionModel
        {
            get => _autoReplyOptionModel;
            set
            {
                if ((_autoReplyOptionModel == value) & (_autoReplyOptionModel == null))
                    return;
                SetProperty(ref _autoReplyOptionModel, value);
            }
        }

        public Visibility FacebookElementsVisibility { get; set; } = Visibility.Collapsed;

        private void SaveInput(object sender)
        {
            try
            {
                var lstSpecificWords = Regex.Split(SpecificWord.Trim(), "\n").Where(x => !string.IsNullOrEmpty(x))
                    .Select(x => x.Trim()).ToList();
                LstMessage = lstSpecificWords;

                var count = ManageMessagesModel.LstQueries.Count;


                if (FacebookElementsVisibility != Visibility.Visible)
                {
                    while (count > 1)
                    {
                        var Content = ManageMessagesModel.LstQueries[count - 1].Content;

                        if (Content.QueryValue != "All" &&
                            Content.QueryValue != "LangKeyReplyToAllMessages"?.FromResourceDictionary() &&
                            Content.QueryValue !=
                            "LangKeyReplyToNewPendingMessagesReplyOnlyMessageSentByUsersThatDontFollowYourAccount"
                                .FromResourceDictionary()) ManageMessagesModel.LstQueries.RemoveAt(count - 1);
                        count--;
                    }

                    lstSpecificWords.ForEach(x =>
                    {
                        if (ManageMessagesModel.LstQueries.All(y => y.Content.QueryValue != x))
                            ManageMessagesModel.LstQueries.Add(new QueryContent
                            {
                                Content = new QueryInfo
                                {
                                    QueryValue = x
                                }
                            });
                    });
                }


                GlobusLogHelper.log.Info(
                    $"{lstSpecificWords.Count} specific word{(lstSpecificWords.Count > 1 ? "s" : "")} saved and added to query sucessfully!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddMessages(object sender)
        {
            var messageData = sender as MessageMediaControl;

            if (messageData?.Messages.MessagesText == null) return;

            messageData.Messages.SelectedQuery =
                new ObservableCollection<QueryContent>(messageData.Messages.LstQueries.Where(x => x.IsContentSelected));

            if (messageData.Messages.SelectedQuery.Count == 0)
            {
                GlobusLogHelper.log.Info("Please add query type with message(s)");
                return;
            }

            messageData.Messages.SelectedQuery.Remove(
                messageData.Messages.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));

            if (messageData.Messages.MessagesText != null)
            {
                var listMessages = messageData.Messages.MessagesText.Split('\n').ToList();
                listMessages = listMessages.Where(x => !string.IsNullOrEmpty(x.Trim())).Select(y => y.Trim()).ToList();

                listMessages.ForEach(message =>
                {
                    try
                    {
                        var isContain = false;
                        LstDisplayManageMessageModel.ForEach(lstMessage =>
                        {
                            if (lstMessage.MessagesText.ToLower().Equals(message.ToLower()))
                                isContain = lstMessage.SelectedQuery.Any(x =>
                                    messageData.Messages.SelectedQuery.Contains(x));
                        });
                        if (!isContain)
                            LstDisplayManageMessageModel.Add(new ManageMessagesModel
                            {
                                MessagesText = message, SelectedQuery = messageData.Messages.SelectedQuery,
                                MessageId = messageData.Messages.MessageId, LstQueries = messageData.Messages.LstQueries
                            });
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    //AddToList(messageData.Messages, message);
                });
            }
            else
            {
                LstDisplayManageMessageModel.Add(messageData.Messages);
            }

            messageData.Messages = new ManageMessagesModel
            {
                LstQueries = ManageMessagesModel.LstQueries
            };

            messageData.Messages.LstQueries.Select(query =>
            {
                query.IsContentSelected = false;
                return query;
            }).ToList();

            ManageMessagesModel = messageData.Messages;

            messageData.ComboBoxQueries.ItemsSource = ManageMessagesModel.LstQueries;
        }

        private void FbMethod()
        {
            AutoReplyOptionModel.IsMessageRequestChecked = IsReplyToPendingMessagesChecked;
            AutoReplyOptionModel.IsFilterByIncommingMessageText = IsReplyToMessagesThatContainSpecificWord﻿Checked;
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
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void RemoveQuery(string keyResource)
        {
            try
            {
                var queryValue = keyResource.Equals("All") ? "All" : keyResource?.FromResourceDictionary();
                ManageMessagesModel.LstQueries.Remove(
                    ManageMessagesModel.LstQueries.FirstOrDefault(x => x.Content.QueryValue == queryValue));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void AutoReplyToNewMessageValidation()
        {
            if (IsReplyToMessagesThatContainSpecificWord﻿Checked && SpecificWord == string.Empty)
            {
                Dialog.ShowDialog("Error", "Please add atleast on keyword");
                return;
            }

            if (FacebookElementsVisibility == Visibility.Visible
                && !AutoReplyOptionModel.IsMessageRequestChecked && !AutoReplyOptionModel.IsFriendsMessageChecked)
            {
                Dialog.ShowDialog("Error", "Please Check atleast One mesaage type");
                return;
            }

            if (FacebookElementsVisibility != Visibility.Visible
                && !IsReplyToPendingMessages﻿﻿Checked && !IsReplyToAllMessagesChecked)
            {
                Dialog.ShowDialog("Error", "Please Check atleast One mesaage type");
                return;
            }

            var account = InstanceProvider.GetInstance<ISelectActivityViewModel>().SelectAccount;

            if (account.AccountBaseModel.AccountNetwork != SocialNetworks.Quora)
            {
                if (LstDisplayManageMessageModel.Count == 0)
                {
                    Dialog.ShowDialog("Error", "Please add atleast One Message");
                    return;
                }
            }
            else if (account.AccountBaseModel.AccountNetwork == SocialNetworks.Quora && string.IsNullOrEmpty(Message))
            {
                Dialog.ShowDialog("Error", "Please type some message.");
                return;
            }

            NavigateNext();
        }

        private void CheckedChangedExecute(object obj)
        {
            if (AutoReplyOptionModel.IsMessageRequestChecked &&
                ManageMessagesModel.LstQueries.All(x => x.Content.QueryType != "Reply to Message Requests"))
                AddMessagesToModel("Reply to Message Requests", string.Empty);
            else if (!AutoReplyOptionModel.IsMessageRequestChecked)
                RemoveMessagesToModel("Reply to Message Requests", string.Empty);

            if (AutoReplyOptionModel.IsFriendsMessageChecked &&
                ManageMessagesModel.LstQueries.All(x => x.Content.QueryType != "Reply to Connected Friends"))
                AddMessagesToModel("Reply to Connected Friends", string.Empty);
            else if (!AutoReplyOptionModel.IsFriendsMessageChecked)
                RemoveMessagesToModel("Reply to Connected Friends", string.Empty);

            if (AutoReplyOptionModel.IsReplyToPageMessagesChecked &&
                ManageMessagesModel.LstQueries.All(x => x.Content.QueryType != "Reply To Page Messages"))
                AddMessagesToModel("Reply To Page Messages", string.Empty);
            else if (!AutoReplyOptionModel.IsReplyToPageMessagesChecked)
                RemoveMessagesToModel("Reply To Page Messages", string.Empty);

            if (!AutoReplyOptionModel.IsReplyToPageMessagesChecked)
                AutoReplyOptionModel.OwnPages = string.Empty;
        }

        public void AddMessagesToModel(string queryType, string queryValue)
        {
            ManageMessagesModel.LstQueries.Add(new QueryContent {Content = new QueryInfo {QueryType = queryType}});
        }

        public void RemoveMessagesToModel(string queryType, string queryValue)
        {
            var queryToDelete = ManageMessagesModel.LstQueries.FirstOrDefault(x => x.Content.QueryType == queryType);
            ManageMessagesModel.LstQueries.Remove(queryToDelete);
            ManageMessagesModel.LstQueries.Remove(queryToDelete);
            foreach (var message in LstDisplayManageMessageModel.ToList())
            {
                var query = message.SelectedQuery.FirstOrDefault(x =>
                    queryToDelete != null && x.Content.Id == queryToDelete.Content.Id);
                message.SelectedQuery.Remove(query);
                if (message.SelectedQuery.Count == 0)
                    LstDisplayManageMessageModel.Remove(message);
            }
        }
    }
}