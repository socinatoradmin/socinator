using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.TdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using Microsoft.Win32;
using TwtDominatorCore.TDModels;

namespace TwtDominatorCore.TDViewModel.TwtBlaster
{
    public class TweetToViewModel : BindableBase
    {
        private TweetToModel _TweetToModel = new TweetToModel();

        private string formats = "Image Files |*.jpg;*.jpeg;*.png;*.gif|Videos Files |*.mov;*.mp4| All files (*.*)|*.*";
        public List<string> TDSupportedVideoFormat = new List<string> {"mov", "mp4"};

        public TweetToViewModel()
        {
            TweetToModel.ListQueryType.Clear();

            Enum.GetValues(typeof(TdUserInteractionQueryEnum)).Cast<TdUserInteractionQueryEnum>().ToList().ForEach(
                query =>
                {
                    TweetToModel.ListQueryType.Add(Application.Current.FindResource(query.GetDescriptionAttr())
                        .ToString());
                });

            //RetweetModel.ListQueryType = Enum.GetNames(typeof(DominatorHouseCore.Enums.TdQuery.TdTweetInteractionQueryEnum)).ToList();


            // Load job configuration values
            TweetToModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfTweetToPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfTweetToPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfTweetToPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfTweetToPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxTweetToPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            #region  commands

            SelectMedia = new BaseCommand<object>(sender => true, SelectMediaFileExecute);
            RemoveFilePath = new BaseCommand<object>(sender => true, RemoveFilePathExecute);
            RemoveSelectedMedia = new BaseCommand<object>(sender => true, RemoveSelectedMediaExecute);


            AddQueryCommand = new BaseCommand<object>(sender => true, AddQuery);
            CustomFilterCommand = new BaseCommand<object>(sender => true, CustomFilter);
            DeleteQueryCommand = new BaseCommand<object>(sender => true, DeleteQuery);
            DeleteMulipleCommand = new BaseCommand<object>(sender => true, DeleteMuliple);
            SplitCommentInputToListCommand = new BaseCommand<object>(sender => true, SplitCommentInputToListExecute);

            #endregion
        }

        public TweetToModel TweetToModel
        {
            get => _TweetToModel;
            set => SetProperty(ref _TweetToModel, value);
        }

        public TweetToModel Model => TweetToModel;


        private void SplitCommentInputToListExecute(object obj)
        {
            try
            {
                var CustomUsers = TweetToModel.UploadedCommentInput;
                TweetToModel.Unfollower.ListCustomUsers = Regex.Split(CustomUsers, "\r\n").ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #region  ICommands

        public ICommand SelectMedia { get; set; }
        public ICommand RemoveFilePath { get; set; }
        public ICommand RemoveSelectedMedia { get; set; }
        public ICommand AddQueryCommand { get; set; }
        public ICommand CustomFilterCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand AddMessagesCommand { get; set; }
        public ICommand DeleteMulipleCommand { get; set; }
        public ICommand SplitCommentInputToListCommand { get; set; }

        #endregion

        #region Methods

        private void SelectMediaFileExecute(object obj)
        {
            try
            {
                var opf = new OpenFileDialog();
                opf.Multiselect = true;

                if (opf.ShowDialog().Equals(true))
                {
                    var videoFile = 0;
                    foreach (var file in opf.FileNames)
                        try
                        {
                            var Extension = Path.GetExtension(file);

                            if (TweetToModel.MediaList.Contains(file))
                                continue;
                            Extension = Extension.Replace(".", "").ToLower();

                            #region video file

                            if (TDSupportedVideoFormat.Contains(Extension))
                            {
                                videoFile++;
                                if (videoFile > 1 || TweetToModel.MediaList.Count > 0)
                                {
                                    MessageBox.Show("Only one video to be selected.");
                                    break;
                                }

                                TweetToModel.VideoFilePath = file;
                                TweetToModel.VideoFilePathVisibility = Visibility.Visible;
                            }

                            #endregion

                            #region gif file

                            else if (Extension.Equals("gif"))
                            {
                                if (TweetToModel.MediaList.Count > 0)
                                {
                                    MessageBox.Show("You cannot upload more than one GIFs.");
                                    break;
                                }

                                TweetToModel.MediaList.Add(file);
                            }

                            #endregion

                            #region image file

                            else if (Extension.Equals("jpg") || Extension.Equals("png") || Extension.Equals("jpeg"))
                            {
                                if (TweetToModel.MediaList.Any(vid =>
                                    vid.Contains("gif") || vid.Contains("mov") || vid.Contains("mp4")))
                                {
                                    MessageBox.Show("You selected multiple type media.");
                                    break;
                                }

                                if (TweetToModel.MediaList.Count >= 1)
                                {
                                    if (TweetToModel.MediaList.Count >= 4)
                                    {
                                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                                        {
                                            Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                                                "You selected more than 4 images.");
                                        }));

                                        break;
                                    }

                                    //TweetToModel.MediaList.RemoveAt(0);
                                    TweetToModel.MediaList.Add(file);
                                    // MessageBox.Show("You selected more than 4 images.");
                                    continue;
                                }

                                TweetToModel.MediaList.Add(file);
                            }

                            #endregion
                        }
                        catch (Exception Ex)
                        {
                            Ex.DebugLog();
                        }

                    if (TweetToModel.MediaList.Count > 0)
                        TweetToModel.MediaListVisibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void RemoveFilePathExecute(object obj)
        {
            try
            {
                TweetToModel.VideoFilePath = "";
                TweetToModel.VideoFilePathVisibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void RemoveSelectedMediaExecute(object obj)
        {
            try
            {
                var data = ((FrameworkElement) obj).DataContext.ToString();
                TweetToModel.MediaList.Remove(data);
                if (TweetToModel.MediaList.Count <= 0)
                    TweetToModel.VideoFilePathVisibility = Visibility.Visible;
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
                var ModuleSettingsUserControl = sender as ModuleSettingsUserControl<ReposterViewModel, ReposterModel>;
                ModuleSettingsUserControl.CustomFilter();
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
                var ModuleSettingsUserControl = sender as ModuleSettingsUserControl<TweetToViewModel, TweetToModel>;
                ModuleSettingsUserControl.AddQuery(typeof(TdUserInteractionQueryEnum));
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
                if (Model.SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                    Model.SavedQueries.Remove(currentQuery);
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
                        if (Model.SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                            Model.SavedQueries.Remove(currentQuery);
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

        #endregion
    }
}