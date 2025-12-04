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

namespace PinDominatorCore.PDViewModel.GrowFollower
{
    public class FollowerViewModel : BindableBase
    {
        private FollowerModel _followerModel = new FollowerModel();

        public FollowerViewModel()
        {
            FollowerModel.ListQueryType.Clear();

            Enum.GetValues(typeof(PDUsersQueries)).Cast<PDUsersQueries>().ToList().ForEach(query =>
            {
                if(query!=PDUsersQueries.UsersWhoTriedPins)
                    FollowerModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())
                    ?.ToString());
            });
            // Load job configuration values
            FollowerModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfFollowsPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfFollowsPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfFollowsPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfFollowsPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxFollowsPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            UploadCommentsCommand = new BaseCommand<object>(sender => true, UploadComments);
            UploadNotesCommand = new BaseCommand<object>(sender => true, UploadNotes);
            AddPhotoCommand = new BaseCommand<object>(sender => true, AddPhoto);
        }

        // NOTE: Required alias property to make it work at runtime
        public FollowerModel Model => FollowerModel;

        public FollowerModel FollowerModel
        {
            get => _followerModel;
            set
            {
                if ((_followerModel == null) & (_followerModel == value))
                    return;
                SetProperty(ref _followerModel, value);
            }
        }

        #region Commands

        public ICommand AddQueryCommand { get; set; }
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
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<FollowerViewModel, FollowerModel>;
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
                var moduleSettingsUserControl = sender as ModuleSettingsUserControl<FollowerViewModel, FollowerModel>;

                moduleSettingsUserControl?.AddQuery(typeof(PDUsersQueries));
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
                FollowerModel.UploadComment = FollowerModel.UploadComment.Trim();
                if (string.IsNullOrEmpty(FollowerModel.UploadComment))
                    return;
                FollowerModel.LstComments = Regex.Split(FollowerModel.UploadComment, "\r\n").ToList();
                GlobusLogHelper.log.Info("LangKeyCommentsSavedSuccessfully".FromResourceDictionary());
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
                FollowerModel.Note = FollowerModel.Note.Trim();
                if (string.IsNullOrEmpty(FollowerModel.Note))
                    return;
                if (string.IsNullOrEmpty(FollowerModel.MediaPath))
                {
                    Dialog.ShowDialog(Application.Current.MainWindow, "LangKeyError".FromResourceDictionary(),
                        "LangKeyPleaseUploadPhoto".FromResourceDictionary());
                    return;
                }

                FollowerModel.LstNotes = Regex.Split(FollowerModel.Note, "\r\n").ToList();
                GlobusLogHelper.log.Info("LangKeyNotesSavedSuccessfully".FromResourceDictionary());
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
                if (opf.ShowDialog().Value) FollowerModel.MediaPath = opf.FileName;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion
    }
}