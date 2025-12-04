/*using System.Collections.Generic;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using FaceDominatorCore.FDModel.CommonSettings;

namespace FaceDominatorCore.FDModel.FriendsModel
{
    public interface IManageFriendRequestModel
    {
//        bool IsAcceptRequest { get; set; }
//        bool IsCancelReceivedRequest { get; set; }
//        bool IsCancelSentRequest { get; set; }
//        bool IsLocationFilterChecked { get; set; }
//        bool IsGenderFilterChecked { get; set; }
//        bool IsMaleChecked { get; set; }
//        bool IsFemaleChecked { get; set; }
//        List<string> ListLocation { get; set; }
    }

    public class ManageFriendRequestModel : ModuleSetting, IManageFriendRequestModel
    {
        
        
      
        [ProtoMember(1)]
        public override JobConfiguration JobConfiguration { get; set; }
     


        private bool _isAcceptRequest ;
        [ProtoMember(2)]
        public bool IsAcceptRequest
        {
            get { return _isAcceptRequest; }
            set
            {
                if (value == _isAcceptRequest)
                    return;
                SetProperty(ref _isAcceptRequest, value);
            }
        }

        private bool _isCancelReceivedRequest;
        [ProtoMember(3)]
        public bool IsCancelReceivedRequest
        {
            get { return _isCancelReceivedRequest; }
            set
            {
                if (value == _isCancelReceivedRequest)
                    return;
                SetProperty(ref _isCancelReceivedRequest, value);
            }
        }

        private bool _isCancelSentRequest;
        [ProtoMember(4)]
        public bool IsCancelSentRequest
        {
            get { return _isCancelSentRequest; }
            set
            {
                if (value == _isCancelSentRequest)
                    return;
                SetProperty(ref _isCancelSentRequest, value);
            }
        }

        private bool _isLocationFilterChecked;
        [ProtoMember(5)]
        public bool IsLocationFilterChecked
        {
            get { return _isLocationFilterChecked; }
            set
            {
                if (value == _isLocationFilterChecked)
                    return;
                SetProperty(ref _isLocationFilterChecked, value);
            }
        }

        private bool _isGenderFilterChecked;
        [ProtoMember(6)]
        public bool IsGenderFilterChecked
        {
            get { return _isGenderFilterChecked; }
            set
            {
                if (value == _isGenderFilterChecked)
                    return;
                SetProperty(ref _isGenderFilterChecked, value);
            }
        }

        private bool _isMaleChecked;
        [ProtoMember(7)]
        public bool IsMaleChecked
        {
            get { return _isMaleChecked; }
            set
            {
                if (value == _isMaleChecked)
                    return;
                SetProperty(ref _isMaleChecked, value);
            }
        }


        private bool _isFemaleChecked;
        [ProtoMember(8)]
        public bool IsFemaleChecked
        {
            get { return _isFemaleChecked; }
            set
            {
                if (value == _isFemaleChecked)
                    return;
                SetProperty(ref _isFemaleChecked, value);
            }
        }


        private List<string> _listLocation = new List<string>();
        [ProtoMember(9)]
        public List<string> ListLocation
        {
            get { return _listLocation; }
            set
            {
                if (value == _listLocation)
                    return;
                SetProperty(ref _listLocation, value);
            }
        }

        public JobConfiguration SlowSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(66, 100),
            ActivitiesPerHour = new RangeUtilities(6, 10),
            ActivitiesPerWeek = new RangeUtilities(400, 600),
            ActivitiesPerJob = new RangeUtilities(8, 12),
            DelayBetweenJobs = new RangeUtilities(81, 122),
            DelayBetweenActivity = new RangeUtilities(0, 1)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(200, 300),
            ActivitiesPerHour = new RangeUtilities(20, 30),
            ActivitiesPerWeek = new RangeUtilities(1200, 1800),
            ActivitiesPerJob = new RangeUtilities(27, 37),
            DelayBetweenJobs = new RangeUtilities(72, 108),
            DelayBetweenActivity = new RangeUtilities(0, 1)
        };

        public JobConfiguration FastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(333, 500),
            ActivitiesPerHour = new RangeUtilities(33, 50),
            ActivitiesPerWeek = new RangeUtilities(2000, 3000),
            ActivitiesPerJob = new RangeUtilities(41, 62),
            DelayBetweenJobs = new RangeUtilities(69, 103),
            DelayBetweenActivity = new RangeUtilities(0, 1)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(533, 800),
            ActivitiesPerHour = new RangeUtilities(53, 80),
            ActivitiesPerWeek = new RangeUtilities(3200, 4800),
            ActivitiesPerJob = new RangeUtilities(66, 100),
            DelayBetweenJobs = new RangeUtilities(73, 110),
            DelayBetweenActivity = new RangeUtilities(0, 1)
        };

    }
}*/