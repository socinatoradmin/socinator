using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace PinDominatorCore.PDModel
{
    [ProtoContract]
    public class DeletePinModel : ModuleSetting
    {
        private bool _isdeteleAllPins = true;

        private bool _isdeteleAllPinsFromBoard;

        private ObservableCollection<Boards> _lstBoardsDetails = new ObservableCollection<Boards>();

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
        public bool IsDeteleAllPins
        {
            get => _isdeteleAllPins;
            set
            {
                if (_isdeteleAllPins == value)
                    return;
                SetProperty(ref _isdeteleAllPins, value);
            }
        }

        [ProtoMember(2)]
        public bool IsDeteleAllPinsFromBoard
        {
            get => _isdeteleAllPinsFromBoard;
            set
            {
                if (_isdeteleAllPinsFromBoard == value)
                    return;
                SetProperty(ref _isdeteleAllPinsFromBoard, value);
            }
        }

        public ObservableCollection<Boards> LstBoardsDetails
        {
            get => _lstBoardsDetails;
            set
            {
                if (_lstBoardsDetails == value)
                    return;
                SetProperty(ref _lstBoardsDetails, value);
            }
        }
    }

    public class Boards : BindableBase
    {
        private string _account;

        private List<Board> _lstBoards = new List<Board>();

        public string Account
        {
            get => _account;
            set
            {
                if (_account == value)
                    return;
                SetProperty(ref _account, value);
            }
        }

        public List<Board> LstBoards
        {
            get => _lstBoards;
            set
            {
                if (_lstBoards == value)
                    return;
                SetProperty(ref _lstBoards, value);
            }
        }
    }

    public class Board : BindableBase
    {
        private string _boardName;

        private string _boardUrl;
        private bool _isCheckBoard;

        public bool IsCheckBoard
        {
            get => _isCheckBoard;
            set
            {
                if (_isCheckBoard == value)
                    return;
                SetProperty(ref _isCheckBoard, value);
            }
        }

        public string BoardName
        {
            get => _boardName;
            set
            {
                if (_boardName == value)
                    return;
                SetProperty(ref _boardName, value);
            }
        }

        public string BoardUrl
        {
            get => _boardUrl;
            set
            {
                if (_boardUrl == value)
                    return;
                SetProperty(ref _boardUrl, value);
            }
        }
    }
}