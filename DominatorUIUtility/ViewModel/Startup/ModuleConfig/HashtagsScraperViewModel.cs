using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IHashtagsScraperViewModel
    {
        string Keyword { get; set; }
    }

    public class HashtagsScraperViewModel : StartupBaseViewModel, IHashtagsScraperViewModel
    {
        private string _Keyword;

        public HashtagsScraperViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.HashtagsScraper});
            IsNonQuery = true;
            NextCommand = new DelegateCommand(ValidateAndNevigate);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfHashtagsPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfHashtagsPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfHashtagsPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfHashtagsPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxHashtagsPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            ListQueryType.Clear();
        }

        public string Keyword
        {
            get => _Keyword;

            set
            {
                if (_Keyword == value)
                    return;
                SetProperty(ref _Keyword, value);
            }
        }

        private void ValidateAndNevigate()
        {
            if (string.IsNullOrEmpty(Keyword))
            {
                Dialog.ShowDialog("Input Error", "Please enter atleast one keyword");
                return;
            }

            NavigateNext();
        }
    }
}