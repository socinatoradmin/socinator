using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.CommonSettings;
using FaceDominatorCore.FDModel.FilterModel;
using ProtoBuf;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FaceDominatorCore.FDModel.MessageModel
{
    public interface IMessageToPlacesModel
    {

    }

    public class MessageToPlacesModel : ModuleSetting, IMessageToPlacesModel
    {

        [ProtoMember(1)]
        public override ObservableCollection<QueryInfo> SavedQueries { get; set; } = new ObservableCollection<QueryInfo>();


        [ProtoMember(2)]
        public override JobConfiguration JobConfiguration { get; set; }


        public List<string> ListQueryType { get; set; } = new List<string>();




        [ProtoMember(26)]
        public override FdPlaceFilterModel FdPlaceFilterModel { get; set; } = new FdPlaceFilterModel();



        private bool _isTagChecked;

        [ProtoMember(28)]
        public bool IsTagChecked
        {
            get
            {
                return _isTagChecked;
            }
            set
            {
                SetProperty(ref _isTagChecked, value);
            }
        }

        private bool _isSpintaxChecked;

        [ProtoMember(29)]
        public bool IsSpintaxChecked
        {
            get
            {
                return _isSpintaxChecked;
            }
            set
            {
                SetProperty(ref _isSpintaxChecked, value);
            }
        }

        private bool _isMessageAsPreview;

        [ProtoMember(29)]
        public bool IsMessageAsPreview
        {
            get
            {
                return _isMessageAsPreview;
            }
            set
            {
                SetProperty(ref _isMessageAsPreview, value);
            }
        }

        [ProtoMember(30)]
        public override ObservableCollection<ManageMessagesModel> LstDisplayManageMessageModel { get; set; } = new ObservableCollection<ManageMessagesModel>();


        [ProtoMember(31)]
        public ManageMessagesModel ManageMessagesModel { get; set; } = new ManageMessagesModel();

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