using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace PinDominatorCore.PDModel
{
    [ProtoContract]
    public class BoardModel : ModuleSetting, IGeneralSettings
    {
        private ObservableCollectionBase<BoardInfo> _boardDetails = new ObservableCollectionBase<BoardInfo>();


        private BoardInfo _boardInfo = new BoardInfo();
        private List<string> _listBoards = new List<string>();

        private List<BoardInfo> _listBoardsDetails = new List<BoardInfo>();
        private List<string> _listCategory = new List<string>();

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
        public override ObservableCollectionBase<BoardInfo> BoardDetails
        {
            get => _boardDetails;
            set
            {
                if (_boardDetails != null && _boardDetails == value)
                    return;
                SetProperty(ref _boardDetails, value);
            }
        }

        [ProtoMember(2)]
        public List<string> ListCategory
        {
            get => _listCategory;
            set
            {
                if (_listCategory != null && _listCategory == value)
                    return;
                SetProperty(ref _listCategory, value);
            }
        }

        [ProtoMember(3)]
        public BoardInfo BoardInfo
        {
            get => _boardInfo;
            set
            {
                if (_boardInfo != null && _boardInfo == value)
                    return;
                SetProperty(ref _boardInfo, value);
            }
        }

        [ProtoMember(4)]
        public List<string> listBoards
        {
            get => _listBoards;
            set
            {
                if (_listBoards != null && _listBoards == value)
                    return;
                SetProperty(ref _listBoards, value);
            }
        }

        [ProtoMember(5)]
        public List<BoardInfo> listBoardsDetails
        {
            get => _listBoardsDetails;
            set
            {
                if (_listBoardsDetails != null && _listBoardsDetails == value)
                    return;
                SetProperty(ref _listBoardsDetails, value);
            }
        }

        private bool _uniqueBoardWithCampaign;

        public bool UniqueBoardWithCampaign
        {
            get { return _uniqueBoardWithCampaign; }
            set
            {
                if (_uniqueBoardWithCampaign == value)
                    return;
                SetProperty(ref _uniqueBoardWithCampaign, value);
            }
        }

    }
}