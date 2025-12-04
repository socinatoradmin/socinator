using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.LDModel.GrowConnection;

namespace LinkedDominatorCore.LDViewModel.GrowConnection
{
    public class BlockUserViewModel : BindableBase
    {
        private BlockUserModel _blockUserModel = new BlockUserModel();

        public BlockUserViewModel()
        {
            BlockUserModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfUsersToBlockPerJob")?.ToString(),
                ActivitiesPerHourDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfUsersToBlockPerHour")?.ToString(),
                ActivitiesPerDayDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfUsersToBlockPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName =
                    Application.Current.FindResource("LangKeyNumberOfUsersToBlockPerWeek")?.ToString(),
                IncreaseActivityDisplayName =
                    Application.Current.FindResource("LangKeyMaxUsersToBlockPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            SaveCustomUserListCommand = new BaseCommand<object>(sender => true, SaveCustomUsers);
        }

        public BlockUserModel BlockUserModel
        {
            get => _blockUserModel;
            set => SetProperty(ref _blockUserModel, value);
        }

        public BlockUserModel Model => BlockUserModel;

        private void SaveCustomUsers(object sender)
        {
            try
            {
                if (BlockUserModel.UrlInput.Contains("\r\n"))
                {
                    BlockUserModel.UrlList =
                        Regex.Split(BlockUserModel.UrlInput, "\r\n").Where(x => !string.IsNullOrEmpty(x.Trim()))
                            .Distinct().ToList();
                    GlobusLogHelper.log.Info("" + BlockUserModel.UrlList.Count + " profile urls saved sucessfully");
                }
                else
                {
                    BlockUserModel.UrlList = new List<string> {BlockUserModel.UrlInput};
                    GlobusLogHelper.log.Info("One profile url saved sucessfully");
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #region Commands

        public ICommand SaveFileFormatCommand { get; set; }
        public ICommand SaveCustomUserListCommand { get; set; }

        #endregion
    }
}