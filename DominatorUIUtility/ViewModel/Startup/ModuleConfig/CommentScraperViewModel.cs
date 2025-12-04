using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface ICommentScraperViewModel
    {
    }

    public class CommentScraperViewModel : StartupBaseViewModel, ICommentScraperViewModel
    {
        public CommentScraperViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.CommentScraper});

            NextCommand = new DelegateCommand(NavigateNext);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyScrapNumberOfCommentsPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyScrapNumberOfCommentsPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyScrapNumberOfCommentsPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyScrapNumberOfCommentsPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyScrapMaxCommentsPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            ListQueryType.Clear();
        }
    }
}