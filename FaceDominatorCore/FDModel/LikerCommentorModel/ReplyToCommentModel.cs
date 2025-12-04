using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.AccountSelectorModel;
using FaceDominatorCore.FDModel.CommonSettings;
using ProtoBuf;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FaceDominatorCore.FDModel.LikerCommentorModel
{

    /*public interface IReplyToCommentModel
    {
        bool IsActionasOwnAccountChecked { get; set; }

        bool IsActionasPageChecked { get; set; }

        List<string> ListOwnPageUrl { get; set; }

        string OwnPageUrl { get; set; }

        bool IsSpintaxChecked { get; set; }
    }*/

    public class ReplyToCommentModel : ModuleSetting, ICommentLikerModule
    {
        private bool _isActionasOwnAccountChecked = true;
        private bool _isActionasPageChecked;
        private string _ownPageUrl;


        [ProtoMember(1)]
        public override JobConfiguration JobConfiguration { get; set; }

        [ProtoMember(2)]
        public override ObservableCollection<QueryInfo> SavedQueries { get; set; } = new ObservableCollection<QueryInfo>();

        [ProtoMember(3)]
        public override ObservableCollection<ManageCommentModel> LstManageCommentModel { get; set; } = new ObservableCollection<ManageCommentModel>();

        private ManageCommentModel _manageCommentsModel = new ManageCommentModel();

        [ProtoMember(4)]
        public ManageCommentModel ManageCommentsModel
        {
            get { return _manageCommentsModel; }
            set
            {
                SetProperty(ref _manageCommentsModel, value);
            }
        }

        [ProtoMember(5)]
        public override OtherConfigModel OtherConfigModel { get; set; } = new OtherConfigModel();

        public List<string> ListQueryType { get; set; } = new List<string>();



        [ProtoMember(6)]
        public bool IsActionasOwnAccountChecked
        {
            get { return _isActionasOwnAccountChecked; }
            set
            {
                SetProperty(ref _isActionasOwnAccountChecked, value);
            }
        }

        [ProtoMember(7)]
        public bool IsActionasPageChecked
        {
            get { return _isActionasPageChecked; }
            set
            {
                SetProperty(ref _isActionasPageChecked, value);
                if (!value)
                {
                    OwnPageUrl = string.Empty;
                    ListOwnPageUrl.Clear();
                }
            }
        }


        private List<string> _listOwnPageUrl = new List<string>();

        [ProtoMember(8)]
        public List<string> ListOwnPageUrl
        {
            get { return _listOwnPageUrl; }
            set
            {
                SetProperty(ref _listOwnPageUrl, value);
            }
        }

        [ProtoMember(9)]
        // ReSharper disable once UnusedMember.Global
        public string OwnPageUrl
        {
            get { return _ownPageUrl; }
            set
            {
                SetProperty(ref _ownPageUrl, value);
            }
        }

        private SelectOptionModel _selectOptionModel = new SelectOptionModel();

        [ProtoMember(10)]
        public SelectOptionModel SelectOptionModel
        {
            get { return _selectOptionModel; }
            set
            {
                SetProperty(ref _selectOptionModel, value);
            }
        }

        private RangeUtilities _noOfUserMention = new RangeUtilities(0, 5);

        [ProtoMember(11)]
        public RangeUtilities NoOfUserMention
        {
            get { return _noOfUserMention; }
            set
            {
                SetProperty(ref _noOfUserMention, value);
            }
        }

        private bool _isMentionUsersChecked;

        [ProtoMember(12)]
        public bool IsMentionUsersChecked
        {
            get { return _isMentionUsersChecked; }
            set
            {
                SetProperty(ref _isMentionUsersChecked, value);
            }
        }


        private bool _isSpintaxChecked;

        [ProtoMember(12)]
        public bool IsSpintaxChecked
        {
            get { return _isSpintaxChecked; }
            set
            {
                SetProperty(ref _isSpintaxChecked, value);
            }
        }
        private bool _isMentionCommentedUserChecked;

        [ProtoMember(12)]
        public bool IsMentionCommentedUserChecked
        {
            get { return _isMentionCommentedUserChecked; }
            set
            {
                SetProperty(ref _isMentionCommentedUserChecked, value);
            }
        }

        private bool _isSkipCommentsOfSameUser;

        [ProtoMember(13)]
        public bool IsSkipCommentsOfSameUser
        {
            get { return _isSkipCommentsOfSameUser; }
            set
            {
                SetProperty(ref _isSkipCommentsOfSameUser, value);
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
