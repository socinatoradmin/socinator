using DominatorHouseCore.Models;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.CommonSettings;
using ProtoBuf;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FaceDominatorCore.FDModel.MessageModel
{

    public interface IAutoReplyMessageModel
    {
        /*string IncommingFilterText { get; set; }

        List<string> MessageRequestFilterText { get; set; }

        bool IsMessageAsPreview { get; set; }

        bool IsSpintaxChecked { get; set; }

        bool IsTagChecked { get; set; }

        bool IschkUniqueMessageChecked { get; set; }*/


        bool IschkUniqueMessageForUserChecked { get; set; }


    }

    public class AutoReplyMessageModel : ModuleSetting, IAutoReplyMessageModel
    {
        [ProtoMember(1)]
        public override JobConfiguration JobConfiguration { get; set; }

        [ProtoMember(2)]
        public override AutoReplyOptionModel AutoReplyOptionModel { get; set; } = new AutoReplyOptionModel();

        [ProtoMember(3)]

        public string Message { get; set; } = string.Empty;


        private string _incommingFilterText = string.Empty;
        [ProtoMember(4)]
        public string IncommingFilterText
        {
            get
            {
                return _incommingFilterText;
            }
            set
            {
                SetProperty(ref _incommingFilterText, value);
            }
        }


        [ProtoMember(5)]
        public override ObservableCollection<ManageMessagesModel> LstDisplayManageMessageModel { get; set; } = new ObservableCollection<ManageMessagesModel>();


        private List<string> _lstMessage = new List<string>();

        [ProtoMember(7)]
        public override List<string> LstMessage
        {
            get { return _lstMessage; }
            set
            {
                SetProperty(ref _lstMessage, value);
            }
        }


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


        private ManageMessagesModel _manageMessagesModel = new ManageMessagesModel();


        [ProtoMember(13)]
        public ManageMessagesModel ManageMessagesModel
        {
            get { return _manageMessagesModel; }
            set
            {
                SetProperty(ref _manageMessagesModel, value);
            }
        }



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
