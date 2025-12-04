using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.LDModel.Filters;
using ProtoBuf;
using System.Collections.ObjectModel;

namespace LinkedDominatorCore.LDModel.Messenger
{
    [ProtoContract]
    public class SendMessageToNewConnectionModel : ModuleSetting, IGeneralSettings
    {
        private bool _IsChkGroupBlackList;

        private bool _IsChkPrivateBlackList;

        private bool _IsChkSkipBlackListedUser;
        
        private RangeUtilities _delayBetweenViewProfileBeforeMessage = new RangeUtilities(15, 30);

        private List<string> _LstMessage;


        private string _Message;

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
        public bool IsCheckedConnectedBefore
        {
            get => _IsCheckedConnectedBefore;
            set
            {
                if (value == _IsCheckedConnectedBefore) return;
                SetProperty(ref _IsCheckedConnectedBefore, value);
            }
        }

        [ProtoMember(2)]
        public int Days
        {
            get => _Days;
            set
            {
                if (value == _Days) return;
                SetProperty(ref _Days, value);
            }
        }

        [ProtoMember(3)]
        public int Hours
        {
            get => _Hours;
            set
            {
                if (value == _Hours) return;
                SetProperty(ref _Hours, value);
            }
        }

        [ProtoMember(5)] public override LDUserFilterModel LDUserFilterModel { get; set; } = new LDUserFilterModel();
        [ProtoMember(6)] public OtherConfigModel OtherConfigModel { get; set; } = new OtherConfigModel();

        [ProtoMember(7)]
        public bool IsChkSpintaxChecked
        {
            get => _IsChkSpintaxChecked;

            set
            {
                if (value == _IsChkSpintaxChecked)
                    return;
                SetProperty(ref _IsChkSpintaxChecked, value);
            }
        }

        [ProtoMember(8)]
        public bool IsChkTagChecked
        {
            get => _IsChkTagChecked;

            set
            {
                if (value == _IsChkTagChecked)
                    return;
                SetProperty(ref _IsChkTagChecked, value);
            }
        }

        [ProtoMember(9)]
        public string Message
        {
            get => _Message;
            set
            {
                if (_Message == value)
                    return;
                SetProperty(ref _Message, value);
            }
        }

        [ProtoMember(10)]
        public List<string> LstMessage
        {
            get => _LstMessage;
            set
            {
                if (_LstMessage == value)
                    return;
                SetProperty(ref _LstMessage, value);
            }
        }

        [ProtoMember(11)]
        public bool IsChkSkipBlackListedUser
        {
            get => _IsChkSkipBlackListedUser;
            set
            {
                if (value == _IsChkSkipBlackListedUser)
                    return;
                SetProperty(ref _IsChkSkipBlackListedUser, value);
            }
        }

        [ProtoMember(12)]
        public bool IsChkPrivateBlackList
        {
            get => _IsChkPrivateBlackList;
            set
            {
                if (value == _IsChkPrivateBlackList)
                    return;
                SetProperty(ref _IsChkPrivateBlackList, value);
            }
        }

        [ProtoMember(13)]
        public bool IsChkGroupBlackList
        {
            get => _IsChkGroupBlackList;
            set
            {
                if (value == _IsChkGroupBlackList)
                    return;
                SetProperty(ref _IsChkGroupBlackList, value);
            }
        }       

        [ProtoMember(14)]
        public RangeUtilities DelayBetweenViewProfileBeforeMessage
        {
            get => _delayBetweenViewProfileBeforeMessage;

            set
            {
                if (value == _delayBetweenViewProfileBeforeMessage)
                    return;
                SetProperty(ref _delayBetweenViewProfileBeforeMessage, value);
            }
        }

        [ProtoMember(15)]
        public bool IsStopSendMessageOnFailed
        {
            get => _IsStopSendMessageOnFailed;
            set => SetProperty(ref _IsStopSendMessageOnFailed, value);
        }

        [ProtoMember(16)]
        public int StopSendMessageOnCount
        {
            get => _StopSendMessageOnCount;
            set => SetProperty(ref _StopSendMessageOnCount, value);
        }        
        [ProtoMember(17)]
        public bool IsSkipUserAlreadyRecievedMessageFromOutsideSoftware
        {
            get => _isSkipUserAlreadyRecievedMessageFromOutsideSoftware;
            set => SetProperty(ref _isSkipUserAlreadyRecievedMessageFromOutsideSoftware, value);
        }
        [ProtoMember(18)]
        public ObservableCollection<ManageMessagesModel> LstDisplayManageMessagesModel { get; set; } =
           new ObservableCollection<ManageMessagesModel>();

        public ManageMessagesModel ManageMessagesModel { get; set; } = new ManageMessagesModel();

        public ObservableCollection<ManageMessagesModel> LstManageMessagesModel { get; set; } =
            new ObservableCollection<ManageMessagesModel>();
        [ProtoMember(4)] JobConfiguration IGeneralSettings.JobConfiguration { get; set; } = new JobConfiguration();



        #region MyRegion

        private bool _IsCheckedConnectedBefore;
        private int _Days;
        private int _Hours;
        private bool _IsChkSpintaxChecked;
        private bool _IsChkTagChecked;
        private bool _IsStopSendMessageOnFailed;
        private int _StopSendMessageOnCount = 3;
        private bool _isSkipUserAlreadyRecievedMessageFromOutsideSoftware;
        #endregion
    }
}