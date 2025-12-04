using DominatorHouseCore.Models;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.CommonSettings;
using FaceDominatorCore.FDModel.FilterModel;
using ProtoBuf;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FaceDominatorCore.FDModel.LikerCommentorModel
{
    public interface ICommentLikerModule
    {
        bool IsActionasOwnAccountChecked { get; set; }

        bool IsActionasPageChecked { get; set; }

        List<string> ListOwnPageUrl { get; set; }

        bool IsSkipCommentsOfSameUser { get; set; }

        //        string OwnPageUrl { get; set; }
    }

    public class CommentLikerModule : ModuleSetting, ICommentLikerModule
    {
#pragma warning disable 169
        private RangeUtilities _commentedBeforeDays;
#pragma warning restore 169
#pragma warning disable 169
        private bool _isCommentDateBetweenChecked;
#pragma warning restore 169
#pragma warning disable 169
        private JobConfiguration _jobConfiguration;
#pragma warning restore 169
        //        private List<string> _listPostUrl = new List<string>();

        private bool _isActionasOwnAccountChecked = true;
        private bool _isActionasPageChecked;
        private string _ownPageUrl;



        [ProtoMember(1)]
        public override JobConfiguration JobConfiguration { get; set; }

        [ProtoMember(2)]

        public override FdCommentFilterModel CommentFilterModel { get; set; } = new FdCommentFilterModel();


        [ProtoMember(3)]
        public override ObservableCollection<QueryInfo> SavedQueries { get; set; } = new ObservableCollection<QueryInfo>();

        [ProtoMember(4)]
        public bool IsActionasOwnAccountChecked
        {
            get { return _isActionasOwnAccountChecked; }
            set
            {
                SetProperty(ref _isActionasOwnAccountChecked, value);
            }
        }

        [ProtoMember(5)]
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

        [ProtoMember(6)]
        public List<string> ListOwnPageUrl
        {
            get { return _listOwnPageUrl; }
            set
            {
                SetProperty(ref _listOwnPageUrl, value);
            }
        }

        [ProtoMember(7)]
        // ReSharper disable once UnusedMember.Global
        public string OwnPageUrl
        {
            get { return _ownPageUrl; }
            set
            {
                SetProperty(ref _ownPageUrl, value);
            }
        }

        public List<string> ListQueryType { get; set; } = new List<string>();

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


        [ProtoMember(4)]
        public override LikerCommentorConfigModel LikerCommentorConfigModel { get; set; } = new LikerCommentorConfigModel();
        public bool IsSkipCommentsOfSameUser { get; set; }
    }
}
