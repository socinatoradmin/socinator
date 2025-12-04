using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IUnjoinViewModel
    {
    }

    public class UnjoinViewModel : StartupBaseViewModel, IUnjoinViewModel
    {
        public UnjoinViewModel(IRegionManager region) : base(region)
        {
            //ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.Unjoin });

            //NextCommand = new DelegateCommand(NevigateNext);
            //PreviousCommand = new DelegateCommand(NevigatePrevious);
            //LoadedCommand = new DelegateCommand<string>(OnLoad);
            //JobConfiguration = new JobConfiguration
            //{
            //    ActivitiesPerJobDisplayName = "LangKeyNumberOfFollowsPerJob".FromResourceDictionary(),
            //    ActivitiesPerHourDisplayName = "LangKeyNumberOfFollowsPerHour".FromResourceDictionary(),
            //    ActivitiesPerDayDisplayName = "LangKeyNumberOfFollowsPerDay".FromResourceDictionary(),
            //    ActivitiesPerWeekDisplayName = "LangKeyNumberOfFollowsPerWeek".FromResourceDictionary(),
            //    IncreaseActivityDisplayName = "LangKeyMaxFollowsPerDay".FromResourceDictionary(),
            //    RunningTime = RunningTimes.DayWiseRunningTimes
            //};
            //ListQueryType.Clear();
        }
    }
}