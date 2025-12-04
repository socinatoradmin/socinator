using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IGroupUnJoinerViewModel
    {
    }

    public class GroupUnJoinerViewModel : StartupBaseViewModel, IGroupUnJoinerViewModel
    {
        private UnfriendOption _unfriendOption = new UnfriendOption();

        public GroupUnJoinerViewModel(IRegionManager region) : base(region)
        {
            UnfriendOptionModel = new UnfriendOption
            {
                SourceDisplayName = "LangKeyGroupUnjoinerSource".FromResourceDictionary(),
                BySoftwareDisplayName = "LangKeyGroupAddedBySoftware".FromResourceDictionary(),
                OutsideSoftwareDisplayName = "LangKeyGroupAddedOutsideSoftware".FromResourceDictionary()
            };

            IsNonQuery = true;
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.GroupUnJoiner});

            NextCommand = new DelegateCommand(GroupUnJoinerValidate);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfGroupUnjoinsPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfGroupUnjoinsPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfGroupUnjoinsPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfGroupUnjoinsPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxGroupUnjoinsPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            ListQueryType.Clear();
        }

        public UnfriendOption UnfriendOptionModel
        {
            get => _unfriendOption;
            set
            {
                if ((_unfriendOption == value) & (_unfriendOption == null))
                    return;
                SetProperty(ref _unfriendOption, value);
            }
        }

        private void GroupUnJoinerValidate()
        {
            if (!UnfriendOptionModel.IsAddedThroughSoftware && !UnfriendOptionModel.IsAddedOutsideSoftware)
            {
                Dialog.ShowDialog("Error", "Please select atleast one source.");
                return;
            }

            if (UnfriendOptionModel.IsFilterApplied && UnfriendOptionModel.DaysBefore == 0 &&
                UnfriendOptionModel.HoursBefore == 0)
            {
                Dialog.ShowDialog("Error", "Please select valid source filter.");
                return;
            }

            NavigateNext();
        }
    }
}