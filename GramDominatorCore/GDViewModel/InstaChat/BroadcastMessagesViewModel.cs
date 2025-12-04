using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.GdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore;
using DominatorUIUtility.CustomControl;
using DominatorHouseCore.LogHelper;
using System.Collections.Generic;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDViewModel.Instachat
{
    public class BroadcastMessagesViewModel : BindableBase
    {
        public BroadcastMessagesViewModel()
        {
            Enum.GetValues(typeof(GdUserQuery)).Cast<GdUserQuery>().ToList().ForEach(query =>
            {
                if(!(query == GdUserQuery.ScrapeUsersToWhomWeMessaged || query == GdUserQuery.ScrapUserWhoMessagedUs))
                BroadcastMessagesModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())?.ToString());
            });
            
            BroadcastMessagesModel.ManageMessagesModel = new ManageMessagesModel()
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

            BroadcastMessagesModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfMessagesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfMessagesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfMessagesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfMessagesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxMessagesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };


            AddQueryCommand = new BaseCommand<object>((sender) => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>((sender) => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>((sender) => true, DeleteQuery);
            AddMessagesCommand = new BaseCommand<object>((sender) => true, AddMessages);
            DeleteMultipleCommand = new BaseCommand<object>((sender) => true, DeleteMultiple);
        }

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand AddMessagesCommand { get; set; }
        public ICommand DeleteMultipleCommand { get; set; }

        #endregion


        #region Command Implemented Methods

        private void AddQuery(object sender)
        {
            try
            {
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<BroadcastMessagesViewModel, BroadcastMessagesModel>;

                if (moduleSettingsUserControl == null || string.IsNullOrEmpty(moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue.Trim()) && !moduleSettingsUserControl._queryControl.QueryCollection.Any())
                    return;

                var splittedQueries = moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue.Contains(",")
                    ? moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue.Split(',').Where(x => !string.IsNullOrEmpty(x.Trim())).ToList()
                    : new List<string> { moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue };

                if (string.IsNullOrEmpty(moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue) && moduleSettingsUserControl._queryControl.QueryCollection.Count != 0)
                {
                    foreach (var queryValue in moduleSettingsUserControl._queryControl.QueryCollection)
                    {
                        if (Model.ManageMessagesModel.LstQueries.Any(x =>
                            x.Content.QueryValue == queryValue &&
                            x.Content.QueryType == moduleSettingsUserControl._queryControl.CurrentQuery.QueryType))
                            continue;
                        {
                            var addNew = new QueryContent
                            {
                                Content = new QueryInfo
                                {
                                    QueryValue = queryValue,
                                    QueryType = moduleSettingsUserControl._queryControl.CurrentQuery.QueryType
                                }
                            };
                            Model.ManageMessagesModel.LstQueries.Add(addNew);
                            Model.LstDisplayManageMessageModel.ForEach(x =>
                            {
                                if (!x.LstQueries.Any(y => addNew.Content.QueryType == moduleSettingsUserControl._queryControl.CurrentQuery.QueryType &&
                                                               y.Content.QueryValue == addNew.Content.QueryValue))
                                    x.LstQueries.Add(addNew);
                            });
                        }
                    }
                }
                else
                {
                    foreach (var queryValue in splittedQueries)
                    {
                        if (Model.ManageMessagesModel.LstQueries.Any(x =>
                            x.Content.QueryType == moduleSettingsUserControl._queryControl.CurrentQuery.QueryType &&
                            x.Content.QueryValue == queryValue)) continue;
                        {
                            var addNew = new QueryContent
                            {
                                Content = new QueryInfo
                                {
                                    QueryValue = queryValue,
                                    QueryType = moduleSettingsUserControl._queryControl.CurrentQuery.QueryType
                                }
                            };
                            Model.ManageMessagesModel.LstQueries.Add(addNew);

                            Model.LstDisplayManageMessageModel.ForEach(x =>
                            {
                                if (!x.LstQueries.Any(y => addNew.Content.QueryType == moduleSettingsUserControl._queryControl.CurrentQuery.QueryType &&
                                                            y.Content.QueryValue == addNew.Content.QueryValue))
                                x.LstQueries.Add(addNew);
                            });
                        }
                    }

                }

                moduleSettingsUserControl.AddQuery(typeof(GdUserQuery));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void CustomFilter(object sender)
        {
            try
            {
                var ModuleSettingsUserControl = sender as ModuleSettingsUserControl<BroadcastMessagesViewModel, BroadcastMessagesModel>;
                if (ModuleSettingsUserControl != null) ModuleSettingsUserControl.CustomFilter();
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

                var queryToDelete = Model.ManageMessagesModel.LstQueries.FirstOrDefault(x =>
                        currentQuery != null && (x.Content.QueryValue == currentQuery.QueryValue
                                                 && x.Content.QueryType == currentQuery.QueryType));


                if (Model.SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                    Model.SavedQueries.Remove(currentQuery);


                Model.ManageMessagesModel.LstQueries.Remove(queryToDelete);
                foreach (var message in Model.LstDisplayManageMessageModel.ToList())
                {
                    message.SelectedQuery.Remove(message.SelectedQuery.GetDeletingQuery(queryToDelete));

                    if (message.SelectedQuery.Count == 0)
                        Model.LstDisplayManageMessageModel.Remove(message);

                    message.LstQueries.Remove(message.LstQueries.GetDeletingQuery(queryToDelete));
                }
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
                //Dialog.ShowDialog(this, "Auto Reply To New Messages Input Error", "Please add query type with message(s)",
                //    MessageDialogStyle.Affirmative);
                GlobusLogHelper.log.Info("Please add query type with message(s)");
                return;
            }
            if(messageData.Messages.LstQueries.Count==1 && messageData.Messages.LstQueries[0].Content.QueryType=="All")
            {
                GlobusLogHelper.log.Info("Please add query type");
                return;
            }
            messageData.Messages.SelectedQuery.Remove(messageData.Messages.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));

            if (messageData.Messages.MessagesText != null)
            {
                if (BroadcastMessagesModel.IsChkAddMultipleComments)
                {
                    List<string> listMessages = new List<string>();

                    listMessages = messageData.Messages.MessagesText.Split('\n').ToList();

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

        private void DeleteMultiple(object sender)
        {
            var selectedQuery = Model.SavedQueries.Where(x => x.IsQuerySelected).ToList();
            try
            {
                foreach (var currentQuery in selectedQuery)
                {
                    try
                    {

                        var queryToDelete = Model.ManageMessagesModel.LstQueries.FirstOrDefault(x =>
                                x.Content.QueryValue == currentQuery.QueryValue
                                && x.Content.QueryType == currentQuery.QueryType);


                        if (Model.SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                            Model.SavedQueries.Remove(currentQuery);


                        Model.ManageMessagesModel.LstQueries.Remove(queryToDelete);
                        foreach (var message in Model.LstDisplayManageMessageModel.ToList())
                        {
                            message.SelectedQuery.Remove(message.SelectedQuery.GetDeletingQuery(queryToDelete));

                            if (message.SelectedQuery.Count == 0)
                                Model.LstDisplayManageMessageModel.Remove(message);

                            message.LstQueries.Remove(message.LstQueries.GetDeletingQuery(queryToDelete));
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        #endregion


        private BroadcastMessagesModel _broadcastMessagesModel = new BroadcastMessagesModel();

        public BroadcastMessagesModel BroadcastMessagesModel
        {
            get
            {
                return _broadcastMessagesModel;
            }
            set
            {
                if (_broadcastMessagesModel == null & _broadcastMessagesModel == value)
                    return;
                SetProperty(ref _broadcastMessagesModel, value);
            }
        }

        public BroadcastMessagesModel Model => BroadcastMessagesModel;

    }
}
