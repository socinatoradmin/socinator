using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace PinDominatorCore.PDModel
{
    [ProtoContract]
    public class EditPinModel : ModuleSetting, IGeneralSettings
    {
        private List<string> _listAccounts = new List<string>();


        private List<string> _listOfSelectedAccounts = new List<string>();

        private List<string> _listPins = new List<string>();

        private List<PinInfo> _listPinsDetails = new List<PinInfo>();
        private ObservableCollectionBase<PinInfo> _pinDetails = new ObservableCollectionBase<PinInfo>();

        private PinInfo _pinInfo = new PinInfo();

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
        public ObservableCollectionBase<PinInfo> PinDetails
        {
            get => _pinDetails;
            set
            {
                if (_pinDetails != null && _pinDetails == value)
                    return;
                SetProperty(ref _pinDetails, value);
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
        public PinInfo PinInfo
        {
            get => _pinInfo;
            set
            {
                if (_pinInfo != null && _pinInfo == value)
                    return;
                SetProperty(ref _pinInfo, value);
            }
        }

        [ProtoMember(4)]
        public List<string> listPins
        {
            get => _listPins;
            set
            {
                if (_listPins != null && _listPins == value)
                    return;
                SetProperty(ref _listPins, value);
            }
        }

        [ProtoMember(5)]
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

        [ProtoMember(6)]
        public List<PinInfo> listDetails
        {
            get => _listPinsDetails;
            set
            {
                if (_listPinsDetails != null && _listPinsDetails == value)
                    return;
                SetProperty(ref _listPinsDetails, value);
            }
        }
    }
}