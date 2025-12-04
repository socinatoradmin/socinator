using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
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

namespace PinDominatorCore.PDViewModel.PinPoster
{
    public class RePinViewModel : BindableBase
    {
        private RePinModel _rePinModel = new RePinModel();

        public RePinViewModel()
        {
            if (RePinModel.ListQueryType.Count == 0)
            {
                RePinModel.ListQueryType.Add(Application.Current
                    .FindResource(PDPinQueries.Keywords.GetDescriptionAttr())?.ToString());
                RePinModel.ListQueryType.Add(Application.Current
                    .FindResource(PDPinQueries.CustomBoard.GetDescriptionAttr())?.ToString());
                RePinModel.ListQueryType.Add(Application.Current
                    .FindResource(PDPinQueries.CustomPin.GetDescriptionAttr())?.ToString());
                RePinModel.ListQueryType.Add(Application.Current
                    .FindResource(PDPinQueries.Customusers.GetDescriptionAttr())?.ToString());
                RePinModel.ListQueryType.Add(Application.Current
                    .FindResource(PDPinQueries.SocinatorPublisherCampaign.GetDescriptionAttr())?.ToString());

            }

            RePinModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfRepostPerJob")?.ToString(),
                ActivitiesPerHourDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfRepostPerHour")?.ToString(),
                ActivitiesPerDayDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfRepostPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfRepostPerWeek")?.ToString(),
                IncreaseActivityDisplayName = Application.Current.FindResource("LangKeyMaxRepostPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            UploadCommentsCommand = new BaseCommand<object>(sender => true, UploadComments);
            UploadNotesCommand = new BaseCommand<object>(sender => true, UploadNotes);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
            AddPhotoCommand = new BaseCommand<object>(sender => true, AddPhoto);
        }

        public RePinModel Model => RePinModel;

        public RePinModel RePinModel
        {
            get => _rePinModel;
            set
            {
                if ((_rePinModel == null) & (_rePinModel == value))
                    return;
                SetProperty(ref _rePinModel, value);
            }
        }

        #region Commands

        public ICommand AddQueryCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand UploadCommentsCommand { get; set; }
        public ICommand UploadNotesCommand { get; set; }
        public ICommand AddPhotoCommand { get; set; }

        #endregion

        #region Methods

        private void CustomFilter(object sender)
        {
            try
            {
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<RePinViewModel, RePinModel>;
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
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<RePinViewModel, RePinModel>;

                moduleSettingsUserControl?.AddQuery(typeof(PDPinQueries), Model.ListQueryType);
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
                if (currentQuery != null)
                {
                    var content = $"{currentQuery.QueryType} [{currentQuery.QueryValue}]";
                    if (Model.SavedQueries.Any(x => x.Id == currentQuery.Id))
                        Model.SavedQueries.Remove(currentQuery);
                    RePinModel.AccountPagesBoardsPair.ForEach(x =>
                    {
                        x.LstofPinsToRepin.Value.RemoveAll(y => y.Content == content);
                    });
                }

                RePinModel.AccountPagesBoardsPair.RemoveAll(x =>
                    x.LstofPinsToRepin.Value.Count == 1 && x.LstofPinsToRepin.Value.GetRandomItem().Content == "All");

                RePinModel.AccountPagesBoardsPair.RemoveAll(x =>
                    x.LstofPinsToRepin.Value.All(y => y.IsContentSelected == false));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void UploadComments(object sender)
        {
            try
            {
                RePinModel.Comment = RePinModel.Comment.Trim();
                if (string.IsNullOrEmpty(RePinModel.Comment))
                    return;
                RePinModel.LstComments = Regex.Split(RePinModel.Comment, "\r\n").ToList();
                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest, "", ActivityType.Repin,
                    "Comments Saved Successfully");
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
                RePinModel.Note = RePinModel.Note.Trim();
                if (string.IsNullOrEmpty(RePinModel.Note))
                    return;
                if (string.IsNullOrEmpty(RePinModel.MediaPath))
                {
                    Dialog.ShowDialog(Application.Current.MainWindow, "Error",
                        "Please upload photo");
                    return;
                }

                RePinModel.LstNotes = Regex.Split(RePinModel.Note, "\r\n").ToList();
                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest, "", ActivityType.Repin,
                    "Notes Saved Successfully");
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
                if (opf.ShowDialog() ?? false) RePinModel.MediaPath = opf.FileName;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion
    }
}