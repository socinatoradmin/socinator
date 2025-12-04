using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.PdQuery;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using PinDominatorCore.PDModel;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace PinDominatorCore.PDViewModel.PinTryCommenter
{
    public class TryViewModel : BindableBase
    {
        private TryModel _tryModel = new TryModel();

        public TryViewModel()
        {
            TryModel.ListQueryType.Clear();

            Enum.GetValues(typeof(PDPinQueries)).Cast<PDPinQueries>().ToList().ForEach(query =>
            {
                TryModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())
                    ?.ToString());
            });
            // Load job configuration values
            TryModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfTriesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfTriesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfTriesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfTriesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxFollowsPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            TryModel.ManageNoteModel.LstQueries.Add(new QueryContent
            {
                Content = new QueryInfo
                {
                    QueryType = "All",
                    QueryValue = "All"
                }
            });
            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
            DeleteMulipleCommand = new BaseCommand<object>(sender => true, DeleteMuliple);
            UploadNotesCommand = new BaseCommand<object>(sender => true, UploadNotes);
            AddPhotoCommand = new BaseCommand<object>(sender => true, AddPhoto);
        }

        // NOTE: Required alias property to make it work at runtime
        public TryModel Model => TryModel;

        public TryModel TryModel
        {
            get => _tryModel;
            set
            {
                if ((_tryModel == null) & (_tryModel == value))
                    return;
                SetProperty(ref _tryModel, value);
            }
        }

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand UploadNotesCommand { get; set; }
        public ICommand DeleteMulipleCommand { get; set; }
        public ICommand AddPhotoCommand { get; set; }

        #endregion

        #region Methods

        private void CustomFilter(object sender)
        {
            try
            {
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<TryViewModel, TryModel>;
                moduleSettingsUserControl?.CustomFilter();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddQuery(object sender)
        {
            try
            {
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<TryViewModel, TryModel>;
                if (moduleSettingsUserControl != null && !Model.ManageNoteModel.LstQueries.Any(x =>
                        x.Content.QueryValue == moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue &&
                        x.Content.QueryType == moduleSettingsUserControl._queryControl.CurrentQuery.QueryType) &&
                    !string.IsNullOrEmpty(moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue))
                {
                    if (moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue.Contains(","))
                    {
                        var queries = moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue.Split(',');
                        foreach (var query in queries)
                            Model.ManageNoteModel.LstQueries.Add(new QueryContent
                            {
                                Content = new QueryInfo
                                {
                                    QueryValue = query,
                                    QueryType = moduleSettingsUserControl._queryControl.CurrentQuery.QueryType
                                }
                            });
                    }
                    else
                    {
                        Model.ManageNoteModel.LstQueries.Add(new QueryContent
                        {
                            Content = new QueryInfo
                            {
                                QueryValue = moduleSettingsUserControl._queryControl.CurrentQuery.QueryValue.Trim(),
                                QueryType = moduleSettingsUserControl._queryControl.CurrentQuery.QueryType
                            }
                        });
                    }
                }

                if (moduleSettingsUserControl != null &&
                    moduleSettingsUserControl._queryControl.QueryCollection.Count != 0)
                    moduleSettingsUserControl._queryControl.QueryCollection.ForEach(x =>
                        Model.ManageNoteModel.LstQueries.Add(new QueryContent
                        {
                            Content = new QueryInfo
                            {
                                AddedDateTime = moduleSettingsUserControl._queryControl.CurrentQuery.AddedDateTime,
                                Id = moduleSettingsUserControl._queryControl.CurrentQuery.Id,
                                QueryPriority = moduleSettingsUserControl._queryControl.CurrentQuery.QueryPriority,
                                QueryType = moduleSettingsUserControl._queryControl.CurrentQuery.QueryType,
                                QueryTypeDisplayName = moduleSettingsUserControl._queryControl.CurrentQuery
                                    .QueryTypeDisplayName,
                                QueryValue = x
                            }
                        })
                    );

                moduleSettingsUserControl?.AddQuery(typeof(PDPinQueries));
                moduleSettingsUserControl?._queryControl.QueryCollection.Clear();
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

                var queryToDelete = Model.ManageNoteModel.LstQueries.FirstOrDefault(x =>
                    currentQuery != null && x.Content.QueryValue == currentQuery.QueryValue
                                         && x.Content.QueryType == currentQuery.QueryType);


                if (Model.SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                    Model.SavedQueries.Remove(currentQuery);


                Model.ManageNoteModel.LstQueries.Remove(queryToDelete);
                foreach (var message in Model.LstDisplayManageNoteModel.ToList())
                {
                    var query = message.SelectedQuery.FirstOrDefault(x => x.Content.QueryType.Equals(queryToDelete.Content.QueryType) &&
                    x.Content.QueryValue.Equals(queryToDelete.Content.QueryValue));
                    message.SelectedQuery.Remove(query);
                    if (message.SelectedQuery.Count == 0)
                        Model.LstDisplayManageNoteModel.Remove(message);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        private void DeleteMuliple(object sender)
        {
            var selectedQuery = Model.SavedQueries.Where(x => x.IsQuerySelected).ToList();
            try
            {
                foreach (var currentQuery in selectedQuery)
                    try
                    {
                        var queryToDelete = Model.ManageNoteModel.LstQueries.FirstOrDefault(x =>
                            x.Content.QueryValue == currentQuery.QueryValue
                            && x.Content.QueryType == currentQuery.QueryType);


                        if (Model.SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                            Model.SavedQueries.Remove(currentQuery);


                        Model.ManageNoteModel.LstQueries.Remove(queryToDelete);
                        foreach (var message in Model.LstDisplayManageNoteModel.ToList())
                        {
                            message.SelectedQuery.Remove(queryToDelete);
                            if (message.SelectedQuery.Count == 0)
                                Model.LstDisplayManageNoteModel.Remove(message);
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

        private void UploadNotes(object sender)
        {
            try
            {
                TryModel.Message = TryModel.Message.Trim();
                if (string.IsNullOrEmpty(TryModel.Message))
                    return;
                if (string.IsNullOrEmpty(TryModel.MediaPath))
                {
                    Dialog.ShowDialog(Application.Current.MainWindow, "Error",
                        "Please upload photo");
                    return;
                }

                TryModel.LstNotes = Regex.Split(TryModel.Message, "\r\n").ToList();
                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest, "", ActivityType.Try,
                    "notes Saved Successfully");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddPhoto(object sender)
        {
            try
            {
                var opf = new OpenFileDialog();
                opf.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF";
                if (opf.ShowDialog() ?? false) TryModel.MediaPath = opf.FileName;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion
    }
}