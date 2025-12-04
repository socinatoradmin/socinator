using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IAcceptBoardInvitationViewModel
    {
    }

    public class AcceptBoardInvitationViewModel : StartupBaseViewModel, IAcceptBoardInvitationViewModel
    {
        public AcceptBoardInvitationViewModel(IRegionManager region) : base(region)
        {
            IsNonQuery = true;
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.AcceptBoardInvitation});
            NextCommand = new DelegateCommand(NavigateNext);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfAcceptBoardInvitationsPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfAcceptBoardInvitationsPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfAcceptBoardInvitationsPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfAcceptBoardInvitationsPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxAcceptBoardInvitationsPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }
    }
}