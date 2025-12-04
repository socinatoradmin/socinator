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
using LinkedDominatorCore.LDModel.Profilling;

namespace LinkedDominatorCore.LDViewModel.Profilling
{
    public class ProfileEndorsementViewModel : BindableBase
    {
        private ProfileEndorsementModel _ProfileEndorsementModel = new ProfileEndorsementModel();

        public ProfileEndorsementViewModel()
        {
            ProfileEndorsementModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = Application.Current
                    .FindResource("LangKeyNumberOfProfileEndorsementsPerJob")?.ToString(),
                ActivitiesPerHourDisplayName = Application.Current
                    .FindResource("LangKeyNumberOfProfileEndorsementsPerHour")?.ToString(),
                ActivitiesPerDayDisplayName = Application.Current
                    .FindResource("LangKeyNumberOfProfileEndorsementsPerDay")?.ToString(),
                ActivitiesPerWeekDisplayName = Application.Current
                    .FindResource("LangKeyNumberOfProfileEndorsementsPerWeek")?.ToString(),
                IncreaseActivityDisplayName =
                    Application.Current.FindResource("LangKeyMaxProfileEndorsementsPerDay")?.ToString(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            SaveCustomUserListCommand = new BaseCommand<object>(sender => true, SaveCustomUsers);
        }

        public ProfileEndorsementModel ProfileEndorsementModel
        {
            get => _ProfileEndorsementModel;
            set
            {
                if ((_ProfileEndorsementModel == null) & (_ProfileEndorsementModel == value))
                    return;
                SetProperty(ref _ProfileEndorsementModel, value);
            }
        }

        public ProfileEndorsementModel Model => ProfileEndorsementModel;

        public ICommand SaveCustomUserListCommand { get; set; }

        private void SaveCustomUsers(object sender)
        {
            try
            {
                if (ProfileEndorsementModel.UrlInput.Contains("\r\n"))
                {
                    ProfileEndorsementModel.UrlList =
                        Regex.Split(ProfileEndorsementModel.UrlInput, "\r\n").ToList();
                    GlobusLogHelper.log.Info("" + ProfileEndorsementModel.UrlList.Count +
                                             " profile urls saved sucessfully");
                }
                else
                {
                    try
                    {
                        ProfileEndorsementModel.UrlList = new List<string>() { ProfileEndorsementModel.UrlInput };
                        GlobusLogHelper.log.Info("One profile url saved sucessfully");
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
    }
}