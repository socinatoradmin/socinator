using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IProfileEndorsementViewModel
    {
        bool IsCheckedBySoftware { get; set; }
        bool IsCheckedOutSideSoftware { get; set; }
        bool IsCheckedLangKeyCustomUserList { get; set; }

        string UrlInput { get; set; }
        List<string> UrlList { get; set; }
        ICommand SaveCustomUserListCommand { get; set; }
        int NumberOfSkillsToBeEndorsed { get; set; }
    }

    public class ProfileEndorsementViewModel : StartupBaseViewModel, IProfileEndorsementViewModel
    {
        private bool _IsCheckedBySoftware;

        private bool _IsCheckedLangKeyCustomUserList;
        private bool _IsCheckedOutSideSoftware;
        private int _NumberOfSkillsToBeEndorsed;
        private string _UrlInput;
        private List<string> _UrlList;

        public ProfileEndorsementViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.ProfileEndorsement});
            NextCommand = new DelegateCommand(NavigateNext);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            IsNonQuery = true;
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfProfileEndorsementsPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfProfileEndorsementsPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfProfileEndorsementsPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfProfileEndorsementsPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxProfileEndorsementsPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }

        public int NumberOfSkillsToBeEndorsed
        {
            get => _NumberOfSkillsToBeEndorsed;
            set => SetProperty(ref _NumberOfSkillsToBeEndorsed, value);
        }


        public bool IsCheckedBySoftware
        {
            get => _IsCheckedBySoftware;
            set => SetProperty(ref _IsCheckedBySoftware, value);
        }


        public bool IsCheckedLangKeyCustomUserList
        {
            get => _IsCheckedLangKeyCustomUserList;
            set => SetProperty(ref _IsCheckedLangKeyCustomUserList, value);
        }

        public bool IsCheckedOutSideSoftware
        {
            get => _IsCheckedOutSideSoftware;
            set => SetProperty(ref _IsCheckedOutSideSoftware, value);
        }

        public ICommand SaveCustomUserListCommand { get; set; }

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

        private void SaveCustomUsers(object sender)
        {
            try
            {
                if (UrlInput.Contains("\r\n"))
                {
                    UrlList = Regex.Split(UrlInput, "\r\n").ToList();
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
    }
}