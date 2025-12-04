using DominatorHouseCore.Models;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.CommonSettings;
using FaceDominatorCore.FDModel.FilterModel;
using ProtoBuf;
using System.Collections.Generic;

namespace FaceDominatorCore.FDModel.LikerCommentorModel
{
    public interface IPostLikerCommentorModel
    {
        //        bool IsActionasOwnAccountChecked { get; set; }
        //
        //        bool IsActionasPageChecked { get; set; }
        //
        //        List<string> ListOwnPageUrl { get; set; }
    }

    public class PostLikerCommentorModel : ModuleSetting, IPostLikerCommentorModel
    {
        //private bool _isActionasOwnAccountChecked;
        //private bool _isActionasPageChecked;


        [ProtoMember(1)]
        public override JobConfiguration JobConfiguration { get; set; }

        [ProtoMember(2)]
        public override PostFilterModel PostFilterModel { get; set; } = new PostFilterModel();

        [ProtoMember(3)]
        public override LikerCommentorConfigModel LikerCommentorConfigModel { get; set; } = new LikerCommentorConfigModel();


        [ProtoMember(4)]
        public override PostLikeCommentorModel PostLikeCommentorModel { get; set; } = new PostLikeCommentorModel();


        /*[ProtoMember(5)]
        public bool IsActionasOwnAccountChecked
        {
            get { return _isActionasOwnAccountChecked; }
            set
            {
                if (value == _isActionasOwnAccountChecked)
                    return;
                SetProperty(ref _isActionasOwnAccountChecked, value);
            }
        }

        [ProtoMember(6)]
        public bool IsActionasPageChecked
        {
            get { return _isActionasPageChecked; }
            set
            {
                if (value == _isActionasPageChecked)
                    return;
                SetProperty(ref _isActionasPageChecked, value);
            }
        }


        private List<string> _listOwnPageUrl = new List<string>();

        [ProtoMember(7)]
        public List<string> ListOwnPageUrl
        {
            get { return _listOwnPageUrl; }
            set
            {
                if (value == _listOwnPageUrl)
                    return;
                SetProperty(ref _listOwnPageUrl, value);
            }
        }*/



        [ProtoMember(8)]
        public override List<string> PostLikeCommentOptions { get; set; } = new List<string>();


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