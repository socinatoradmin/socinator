using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IMakeAdminViewModel
    {
        SelectAccountDetailsModel SelectAccountDetailsModel { get; set; }
        bool IsSelctDetails { get; set; }
    }

    public class MakeAdminViewModel : StartupBaseViewModel, IMakeAdminViewModel
    {
        private bool _isSelctDetails;

        private SelectAccountDetailsModel _selectAccountDetailsModel = new SelectAccountDetailsModel();

        public MakeAdminViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig {Model = this, ActivityType = ActivityType.MakeAdmin});
            IsNonQuery = true;
            NextCommand = new DelegateCommand(MakeAdminValidate);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyMakeAdminToNumberOfProfilesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyMakeAdminToNumberOfProfilesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyMakeAdminToNumberOfProfilesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyMakeAdminToMaxProfilesPerDay".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMakeAdminToNumberOfProfilesPerWeek".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            ListQueryType.Clear();
        }

        public SelectAccountDetailsModel SelectAccountDetailsModel
        {
            get => _selectAccountDetailsModel;
            set
            {
                if ((_selectAccountDetailsModel == value) & (_selectAccountDetailsModel == null))
                    return;
                SetProperty(ref _selectAccountDetailsModel, value);
            }
        }

        public bool IsSelctDetails
        {
            get => _isSelctDetails;
            set
            {
                if (_isSelctDetails == value)
                    return;
                SetProperty(ref _isSelctDetails, value);
            }
        }

        private void MakeAdminValidate()
        {
            var selectAccountDetailsControl = new SelectAccountDetailsModel();

            if (selectAccountDetailsControl.GetGroupInviterDetails(SelectAccountDetailsModel).GroupInviterDetails
                    .Count == 0)
            {
                Dialog.ShowDialog("Error", "Please select atleast one Make Admin details.");
                return;
            }

            NavigateNext();
        }
    }
}