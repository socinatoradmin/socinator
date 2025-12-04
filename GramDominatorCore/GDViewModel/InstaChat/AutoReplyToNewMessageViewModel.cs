using System;
using System.Collections.ObjectModel;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorUIUtility.CustomControl;
using DominatorHouseCore;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Enums.GdQuery;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDViewModel.Instachat
{
    public class AutoReplyToNewMessageViewModel : BindableBase
    {
        public AutoReplyToNewMessageViewModel()
        {
            AutoReplyToNewMessageModel.ManageMessagesModel = new ManageMessagesModel()
            {
                LstQueries = new ObservableCollection<QueryContent>()
                {
                    new QueryContent()
                    {
                        Content = new QueryInfo
                        {
                            QueryType = "All",
                            QueryValue = "All"
                        }
                    }
                }
            };

            AutoReplyToNewMessageModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfMessagesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfMessagesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfMessagesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfMessagesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxMessagesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            AddMessagesCommand = new BaseCommand<object>((sender) => true, AddMessages);
            InputSaveCommand = new BaseCommand<object>((sender) => true, SaveInput);
        }

        #region Commands

        public ICommand AddMessagesCommand { get; set; }

        public ICommand InputSaveCommand { get; set; }

        #endregion
        
        #region Command Implemented Methods

        private void SaveInput(object sender)
        {
            try
            {
                List<string> lstSpecificWords = Regex.Split(Model.SpecificWord, "\n").Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim()).ToList();
                Model.LstMessage = lstSpecificWords;

                int count = Model.ManageMessagesModel.LstQueries.Count;

                while (count > 1)
                {
                    var Content = Model.ManageMessagesModel.LstQueries[count - 1].Content;

                    if (Content.QueryValue != "All")
                    {
                        Model.ManageMessagesModel.LstQueries.RemoveAt(count - 1);
                    }
                    count--;
                }

                lstSpecificWords.ForEach(x =>
                {
                    if (Model.ManageMessagesModel.LstQueries.All(y => y.Content.QueryValue != x))
                    {
                        Model.ManageMessagesModel.LstQueries.Add(new QueryContent
                        {
                            Content = new QueryInfo
                            {
                                QueryValue = x
                            }

                        });
                    }
                });

                GlobusLogHelper.log.Info($"{lstSpecificWords.Count} specific word{(lstSpecificWords.Count > 1 ? "s" : "")} saved and added to query sucessfully!");
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

            messageData.Messages.SelectedQuery = new ObservableCollection<QueryContent>(messageData.Messages.LstQueries.Where(x => x.IsContentSelected));

            if (messageData.Messages.SelectedQuery.Count == 0)
            {
                GlobusLogHelper.log.Info("Please add query type with message(s)");
                return;
            }
            messageData.Messages.SelectedQuery.Remove(messageData.Messages.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));

            if (messageData.Messages.MessagesText != null)
            {
                if(AutoReplyToNewMessageModel.IsChkAddMultipleComments)
                {
                    List<string> listMessages = messageData.Messages.MessagesText.Split('\n').ToList();
                    listMessages = listMessages.Where(x => !string.IsNullOrEmpty(x.Trim())).Select(y => y.Trim()).ToList();

                    listMessages.ForEach(message =>
                    {
                        try
                        {
                            bool isContain = false;
                            Model.LstDisplayManageMessageModel.ForEach(lstMessage =>
                            {
                                if (lstMessage.MessagesText.ToLower().Equals(message.ToLower()))
                                    isContain = lstMessage.SelectedQuery.Any(x => messageData.Messages.SelectedQuery.Contains(x));
                            });
                            if (!isContain)
                            {
                                var medias = messageData?.Messages?.Medias ?? new ObservableCollection<MessageMediaInfo>();
                                var mediaPath = messageData?.Messages?.Medias != null ? messageData.Messages?.Medias.GetRandomItem().MediaPath : string.Empty;
                                Model.LstDisplayManageMessageModel.Add(new ManageMessagesModel()
                                {
                                    MessagesText = message,
                                    SelectedQuery = messageData.Messages.SelectedQuery,
                                    MessageId = messageData.Messages.MessageId,
                                    LstQueries = messageData.Messages.LstQueries,
                                    MediaPath = mediaPath,
                                    Medias = medias
                                });
                            }       
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
                    Model.LstDisplayManageMessageModel.Add(messageData.Messages);
                }
               
            }
            else
                Model.LstDisplayManageMessageModel.Add(messageData.Messages);

            messageData.Messages = new ManageMessagesModel
            {
                LstQueries = Model.ManageMessagesModel.LstQueries
            };

            messageData.Messages.LstQueries.Select(query => { query.IsContentSelected = false; return query; }).ToList();

            Model.ManageMessagesModel = messageData.Messages;

            messageData.ComboBoxQueries.ItemsSource = Model.ManageMessagesModel.LstQueries;

        }

        //private void AddMessages(object sender)
        //{
        //    var messageData = sender as MessageMediaControl;

        //    if (messageData?.Messages.MessagesText == null) return;

        //    messageData.Messages.SelectedQuery = new ObservableCollection<QueryContent>(messageData.Messages.LstQueries.Where(x => x.IsContentSelected));

        //    if (messageData.Messages.SelectedQuery.Count == 0)
        //    {
        //        GlobusLogHelper.log.Info("Please add query type with message(s)");
        //        return;
        //    }

        //    messageData.Messages.SelectedQuery.Remove(messageData.Messages.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));

        //    Model.LstDisplayManageMessageModel.Add(messageData.Messages);

        //    messageData.Messages = new ManageMessagesModel
        //    {
        //        LstQueries = Model.ManageMessagesModel.LstQueries
        //    };

        //    messageData.Messages.LstQueries.Select(query => { query.IsContentSelected = false; return query; }).ToList();

        //    Model.ManageMessagesModel = messageData.Messages;

        //    messageData.ComboBoxQueries.ItemsSource = Model.ManageMessagesModel.LstQueries;

        //}
        #endregion
       
        private AutoReplyToNewMessageModel _autoReplyToNewMessageModel = new AutoReplyToNewMessageModel();

        public AutoReplyToNewMessageModel AutoReplyToNewMessageModel
        {
            get
            {
                return _autoReplyToNewMessageModel;
            }
            set
            {
                if (_autoReplyToNewMessageModel == null & _autoReplyToNewMessageModel == value)
                    return;
                SetProperty(ref _autoReplyToNewMessageModel, value);
            }
        }

        public AutoReplyToNewMessageModel Model => AutoReplyToNewMessageModel;
    }
}
