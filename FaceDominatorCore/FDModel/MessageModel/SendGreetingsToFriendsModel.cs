using DominatorHouseCore.Models;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.CommonSettings;
using FaceDominatorCore.FDModel.FilterModel;
using ProtoBuf;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FaceDominatorCore.FDModel.MessageModel
{
    public interface ISendGreetingsToFriendsModel
    {
        /*bool IsMessageAsPreview { get; set; }

        bool IsSpintaxChecked { get; set; }

        bool IsTagChecked { get; set; }

        bool IschkUniqueMessageChecked { get; set; }

        bool IschkUniqueMessageForUserChecked { get; set; }

        bool IsFilterApplied { get; set; }

        RangeUtilities DaysBefore { get; set; }*/

    }

    public class SendGreetingsToFriendsModel : ModuleSetting, ISendGreetingsToFriendsModel
    {
        [ProtoMember(1)]
        public override JobConfiguration JobConfiguration { get; set; }

        private bool _isSendBirthDayGreetings = true;

        [ProtoMember(2)]
        public bool IsSendBirthDayGreetings
        {
            get
            {
                return _isSendBirthDayGreetings;
            }

            set
            {
                SetProperty(ref _isSendBirthDayGreetings, value);
            }
        }

        [ProtoMember(3)]

        public string Message { get; set; } = string.Empty;



        [ProtoMember(5)]
        public override ObservableCollection<ManageMessagesModel> LstDisplayManageMessageModel { get; set; } = new ObservableCollection<ManageMessagesModel>();


        [ProtoMember(6)]
        public ManageMessagesModel ManageMessagesModel { get; set; } = new ManageMessagesModel();



        private bool _isMessageAsPreview;

        [ProtoMember(8)]
        public bool IsMessageAsPreview
        {
            get { return _isMessageAsPreview; }
            set
            {
                SetProperty(ref _isMessageAsPreview, value);
            }
        }


        private bool _isSpintaxChecked;

        [ProtoMember(9)]
        public bool IsSpintaxChecked
        {
            get { return _isSpintaxChecked; }
            set
            {
                SetProperty(ref _isSpintaxChecked, value);
            }
        }

        private bool _isTagChecked;

        [ProtoMember(10)]
        public bool IsTagChecked
        {
            get { return _isTagChecked; }
            set
            {
                SetProperty(ref _isTagChecked, value);
            }
        }

        private bool _ischkUniqueMessageChecked;

        [ProtoMember(11)]
        public bool IschkUniqueMessageChecked
        {
            get { return _ischkUniqueMessageChecked; }
            set
            {
                SetProperty(ref _ischkUniqueMessageChecked, value);
            }
        }

        private bool _ischkUniqueMessageForUserChecked;

        [ProtoMember(12)]
        public bool IschkUniqueMessageForUserChecked
        {
            get { return _ischkUniqueMessageForUserChecked; }
            set
            {
                SetProperty(ref _ischkUniqueMessageForUserChecked, value);
            }
        }

        private RangeUtilities _daysBefore = new RangeUtilities(1, 2);

        [ProtoMember(13)]
        public RangeUtilities DaysBefore
        {
            get { return _daysBefore; }

            set
            {
                SetProperty(ref _daysBefore, value);
            }
        }


        private RangeUtilities _userAge = new RangeUtilities(20, 60);

        [ProtoMember(14)]
        public RangeUtilities UserAge
        {
            get
            {
                return _userAge;
            }

            set
            {
                SetProperty(ref _userAge, value);
            }
        }

        private bool _isFilterByAge = true;

        [ProtoMember(15)]
        public bool IsFilterByAge
        {
            get
            {
                return _isFilterByAge;
            }

            set
            {
                SetProperty(ref _isFilterByAge, value);
            }
        }


        private bool _isFilterByDays = true;

        [ProtoMember(16)]
        public bool IsFilterByDays
        {
            get
            {
                return _isFilterByDays;
            }

            set
            {
                SetProperty(ref _isFilterByDays, value);
            }
        }


        private bool _isPostToOwnWallChecked;

        [ProtoMember(17)]
        public bool IsPostToOwnWallChecked
        {
            get
            {
                return _isPostToOwnWallChecked;
            }

            set
            {
                SetProperty(ref _isPostToOwnWallChecked, value);
            }
        }


        private string _postDetailsText = string.Empty;

        [ProtoMember(18)]
        public string PostDetailsText
        {
            get
            {
                return _postDetailsText;
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                    ListPostDetails = new List<string>();

                SetProperty(ref _postDetailsText, value);
            }
        }

        private List<string> _listPostDetails = new List<string>();

        [ProtoMember(19)]
        public List<string> ListPostDetails
        {
            get
            {
                return _listPostDetails;
            }

            set
            {
                SetProperty(ref _listPostDetails, value);
            }
        }

        private bool _isSpintaxForPostChechked;

        [ProtoMember(20)]
        public bool IsSpintaxForPostChechked
        {
            get
            {
                return _isSpintaxForPostChechked;
            }

            set
            {
                SetProperty(ref _isSpintaxForPostChechked, value);
            }
        }


        private bool _isTagForPostChecked;

        [ProtoMember(21)]
        public bool IsTagForPostChecked
        {
            get
            {
                return _isTagForPostChecked;
            }

            set
            {
                SetProperty(ref _isTagForPostChecked, value);
            }
        }

        private FbMultiMediaModel _fbMultiMediaModel = new FbMultiMediaModel();
        [ProtoMember(22)]
        public FbMultiMediaModel FbMultiMediaModel
        {
            get { return _fbMultiMediaModel; }
            set
            {
                SetProperty(ref _fbMultiMediaModel, value);
            }
        }


        private bool _ischkUniqueRequestChecked = false;

        public bool IschkUniqueRequestChecked
        {
            get
            {
                return _ischkUniqueMessageChecked;
            }
            set
            {
                SetProperty(ref _ischkUniqueMessageChecked, value);
            }
        }

        [ProtoMember(23)]
        public override FdGenderAndLocationFilterModel GenderAndLocationFilter { get; set; }
            = new FdGenderAndLocationFilterModel();


        // ReSharper disable once UnusedMember.Global
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

        // ReSharper disable once UnusedMember.Global
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

        // ReSharper disable once UnusedMember.Global
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


        // ReSharper disable once IdentifierTypo
        // ReSharper disable once UnusedMember.Global
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
    }
}
