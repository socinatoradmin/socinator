using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using GramDominatorCore.GDLibrary;
using ProtoBuf;
using DominatorHouseCore.Utility;
using System.Collections.ObjectModel;

namespace GramDominatorCore.GDModel
{
    public interface ISendMessageToNewFollower
    {
        bool IsCheckedSendMessageToNewFollowers { get; set; }

        string TextMessage { get; set; }

        List<string> LstMessage { get; set; }
    }

    [ProtoContract]
    public class SendMessageToFollowerModel : ModuleSetting, ISendMessageToNewFollower, IGeneralSettings
    {
        [ProtoMember(1)]
        JobConfiguration IGeneralSettings.JobConfiguration { get; set; }
        [ProtoMember(2)]
        public override UserFilterModel UserFilterModel { get; set; } = new UserFilterModel();
        //[ProtoMember(3)]
        //public OtherConfigModel OtherConfigModel { get; set; } = new OtherConfigModel();

        #region Set Job Configuration speed
        public JobConfiguration SlowSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(13, 20),
            ActivitiesPerHour = new RangeUtilities(1, 2),
            ActivitiesPerWeek = new RangeUtilities(80, 120),
            ActivitiesPerJob = new RangeUtilities(1, 2),
            DelayBetweenJobs = new RangeUtilities(88, 132),
            DelayBetweenActivity = new RangeUtilities(30, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(33, 50),
            ActivitiesPerHour = new RangeUtilities(3, 5),
            ActivitiesPerWeek = new RangeUtilities(200, 300),
            ActivitiesPerJob = new RangeUtilities(4, 6),
            DelayBetweenJobs = new RangeUtilities(87, 130),
            DelayBetweenActivity = new RangeUtilities(25, 50),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration FastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(53, 80),
            ActivitiesPerHour = new RangeUtilities(5, 8),
            ActivitiesPerWeek = new RangeUtilities(320, 480),
            ActivitiesPerJob = new RangeUtilities(6, 10),
            DelayBetweenJobs = new RangeUtilities(86, 130),
            DelayBetweenActivity = new RangeUtilities(15, 30),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(66, 100),
            ActivitiesPerHour = new RangeUtilities(6, 10),
            ActivitiesPerWeek = new RangeUtilities(400, 600),
            ActivitiesPerJob = new RangeUtilities(8, 12),
            DelayBetweenJobs = new RangeUtilities(87, 131),
            DelayBetweenActivity = new RangeUtilities(8, 15),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };
        #endregion

        #region ISendMessageToNewFollower
        private bool _isCheckedSendMessageToNewFollowers = true;
        [ProtoMember(4)]
        public bool IsCheckedSendMessageToNewFollowers
        {
            get
            {
                return _isCheckedSendMessageToNewFollowers;
            }
            set
            {
                if (_isCheckedSendMessageToNewFollowers == value)
                    return;
                SetProperty(ref _isCheckedSendMessageToNewFollowers, value);
            }
        }


        private string _textMessage = string.Empty;
        [ProtoMember(5)]
        public string TextMessage
        {
            get
            {
                return _textMessage;
            }
            set
            {
                if (_textMessage == value)
                    return;
                SetProperty(ref _textMessage, value);
            }
        }

        private List<string> _lstMessage = new List<string>();
        [ProtoMember(6)]
        public List<string> LstMessage
        {
            get
            {
                return _lstMessage;
            }
            set
            {
                if (_lstMessage == value)
                    return;
                SetProperty(ref _lstMessage, value);
            }
        }

        [ProtoMember(7)]
        public override MangeBlacklist.SkipBlacklist SkipBlacklist { get; set; } = new MangeBlacklist.SkipBlacklist();

        [ProtoMember(8)]
        public ObservableCollection<ManageMessagesModel> LstDisplayManageMessageModel { get; set; } = new ObservableCollection<ManageMessagesModel>();

        public ManageMessagesModel ManageMessagesModel { get; set; } = new ManageMessagesModel();

        [ProtoMember(9)]
        private string _mediaPath = string.Empty;
        public string MediaPath
        {
            get { return _mediaPath; }
            set
            {
                if (value == _mediaPath)
                    return;
                SetProperty(ref _mediaPath, value);
            }
        }
        private ObservableCollection<MessageMediaInfo> mediaInfos = new ObservableCollection<MessageMediaInfo>();
        [ProtoMember(10)]
        public ObservableCollection<MessageMediaInfo> Medias
        {
            get => mediaInfos;
            set { SetProperty(ref mediaInfos, value); }
        }
        #endregion
    }
}
