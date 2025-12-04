using DominatorHouseCore.Models;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.CommonSettings;
using FaceDominatorCore.FDModel.FilterModel;
using ProtoBuf;
using static FaceDominatorCore.FDLibrary.FdClassLibrary.MangeBlacklist;

namespace FaceDominatorCore.FDModel.FriendsModel
{

    public interface IUnfriendModel
    {
        //        bool IsChkStopFriendToolWhenReachChecked { get; set; }
        //
        //        bool IsChkEnableAutoSendRequestWithdrawChecked { get; set; }
        //
        //
        //
        //        RangeUtilities StopFollowToolWhenReach { get; set; }
    }

    public class UnfriendModel : ModuleSetting, IUnfriendModel
    {

        [ProtoMember(1)]
        public override JobConfiguration JobConfiguration { get; set; }


        //        [ProtoMember(2)]
        //        public override FdGenderFilterModel GenderFilterModel { get; set; }= new FdGenderFilterModel();

        // [ProtoMember(3)]
        //  public override UnfriendOption UnfriendOptionModel { get; set; } = new UnfriendOption();


        private UnfriendOption _unfriendOptionModel = new UnfriendOption();
        [ProtoMember(3)]
        public UnfriendOption UnfriendOptionModel
        {
            get { return _unfriendOptionModel; }
            set
            {
                SetProperty(ref _unfriendOptionModel, value);
            }
        }



        private bool _isChkStopFriendToolWhenReachChecked;


        [ProtoMember(4)]
        public bool IsChkStopFriendToolWhenReachChecked
        {
            get
            {
                return _isChkStopFriendToolWhenReachChecked;
            }

            set
            {
                SetProperty(ref _isChkStopFriendToolWhenReachChecked, value);
            }
        }

        private bool _isChkEnableAutoSendRequestWithdrawChecked;

        [ProtoMember(5)]
        public bool IsChkEnableAutoSendRequestWithdrawChecked
        {
            get
            {
                return _isChkEnableAutoSendRequestWithdrawChecked;
            }

            set
            {
                SetProperty(ref _isChkEnableAutoSendRequestWithdrawChecked, value);
            }
        }


        private RangeUtilities _stopFollowToolWhenReach = new RangeUtilities();

        [ProtoMember(6)]

        public RangeUtilities StopFollowToolWhenReach
        {
            get
            {
                return _stopFollowToolWhenReach;
            }

            set
            {
                SetProperty(ref _stopFollowToolWhenReach, value);
            }
        }


        [ProtoMember(7)]
        public override FdGenderAndLocationFilterModel GenderAndLocationFilter { get; set; } = new FdGenderAndLocationFilterModel();


        [ProtoMember(11)]
        public override SkipBlacklist SkipBlacklist { get; set; } = new SkipBlacklist();

        [ProtoMember(12)]
        public override ManageBlackWhiteListModel ManageBlackWhiteListModel { get; set; } = new ManageBlackWhiteListModel();
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
