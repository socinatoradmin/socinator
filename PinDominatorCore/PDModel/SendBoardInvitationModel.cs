using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace PinDominatorCore.PDModel
{
    public class SendBoardInvitationModel : ModuleSetting
    {
        private ObservableCollectionBase<BoardCollaboratorInfo> _boardCollaboratorDetails =
            new ObservableCollectionBase<BoardCollaboratorInfo>();

        private BoardCollaboratorInfo _boardCollaboratorInfo = new BoardCollaboratorInfo();


        private List<string> _listAccounts = new List<string>();

        private List<BoardCollaboratorInfo> _listBoardCollaboratorDetails = new List<BoardCollaboratorInfo>();

        private List<string> _listBoardCollaborators = new List<string>();

        private List<string> _listOfSelectedAccounts = new List<string>();

        public JobConfiguration SlowSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(10, 15),
            ActivitiesPerHour = new RangeUtilities(1, 2),
            ActivitiesPerWeek = new RangeUtilities(60, 90),
            ActivitiesPerJob = new RangeUtilities(1, 2),
            DelayBetweenJobs = new RangeUtilities(88, 133),
            DelayBetweenActivity = new RangeUtilities(30, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(27, 40),
            ActivitiesPerHour = new RangeUtilities(3, 4),
            ActivitiesPerWeek = new RangeUtilities(160, 240),
            ActivitiesPerJob = new RangeUtilities(3, 5),
            DelayBetweenJobs = new RangeUtilities(87, 131),
            DelayBetweenActivity = new RangeUtilities(23, 45),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration FastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(53, 80),
            ActivitiesPerHour = new RangeUtilities(5, 8),
            ActivitiesPerWeek = new RangeUtilities(320, 480),
            ActivitiesPerJob = new RangeUtilities(7, 10),
            DelayBetweenJobs = new RangeUtilities(87, 130),
            DelayBetweenActivity = new RangeUtilities(15, 30),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration SuperfastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(67, 100),
            ActivitiesPerHour = new RangeUtilities(7, 10),
            ActivitiesPerWeek = new RangeUtilities(400, 600),
            ActivitiesPerJob = new RangeUtilities(8, 12),
            DelayBetweenJobs = new RangeUtilities(88, 132),
            DelayBetweenActivity = new RangeUtilities(8, 15),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        [ProtoMember(1)]
        public ObservableCollectionBase<BoardCollaboratorInfo> BoardCollaboratorDetails
        {
            get => _boardCollaboratorDetails;
            set
            {
                if (_boardCollaboratorDetails != null && _boardCollaboratorDetails == value)
                    return;
                SetProperty(ref _boardCollaboratorDetails, value);
            }
        }

        [ProtoMember(2)]
        public List<string> ListAccounts
        {
            get => _listAccounts;
            set
            {
                if (_listAccounts != null && _listAccounts == value)
                    return;
                SetProperty(ref _listAccounts, value);
            }
        }

        [ProtoMember(3)]
        public BoardCollaboratorInfo BoardCollaboratorInfo
        {
            get => _boardCollaboratorInfo;
            set
            {
                if (_boardCollaboratorInfo != null && _boardCollaboratorInfo == value)
                    return;
                SetProperty(ref _boardCollaboratorInfo, value);
            }
        }

        [ProtoMember(4)]
        public List<string> listOfSelectedAccounts
        {
            get => _listOfSelectedAccounts;
            set
            {
                if (_listOfSelectedAccounts != null && _listOfSelectedAccounts == value)
                    return;
                SetProperty(ref _listOfSelectedAccounts, value);
            }
        }

        [ProtoMember(5)]
        public List<BoardCollaboratorInfo> ListBoardCollaboratorDetails
        {
            get => _listBoardCollaboratorDetails;
            set
            {
                if (_listBoardCollaboratorDetails != null && _listBoardCollaboratorDetails == value)
                    return;
                SetProperty(ref _listBoardCollaboratorDetails, value);
            }
        }

        [ProtoMember(4)]
        public List<string> listBoardCollaborators
        {
            get => _listBoardCollaborators;
            set
            {
                if (_listBoardCollaborators != null && _listBoardCollaborators == value)
                    return;
                SetProperty(ref _listBoardCollaborators, value);
            }
        }
    }
}