using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using PinDominatorCore.PDModel;

namespace PinDominatorCore.PDViewModel.Boards
{
    public class AcceptBoardInvitationViewModel : BindableBase
    {
        private AcceptBoardInvitationModel _acceptBoardInvitationModel = new AcceptBoardInvitationModel();

        public AcceptBoardInvitationViewModel()
        {
            AcceptBoardInvitationModel.JobConfiguration = new JobConfiguration
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

        public AcceptBoardInvitationModel Model => AcceptBoardInvitationModel;

        public AcceptBoardInvitationModel AcceptBoardInvitationModel
        {
            get => _acceptBoardInvitationModel;
            set
            {
                if ((_acceptBoardInvitationModel == null) & (_acceptBoardInvitationModel == value))
                    return;
                SetProperty(ref _acceptBoardInvitationModel, value);
            }
        }
    }
}