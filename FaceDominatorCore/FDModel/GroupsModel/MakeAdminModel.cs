using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.CommonSettings;
using ProtoBuf;
using System.Collections.Generic;

namespace FaceDominatorCore.FDModel.GroupsModel
{

    public interface IMakeAdminModel
    {
        List<string> ListFriendProfileUrl { get; set; }
        List<string> ListGroupUrl { get; set; }
    }
    public class MakeAdminModel : ModuleSetting, IMakeAdminModel
    {
        [ProtoMember(1)]
        public override JobConfiguration JobConfiguration { get; set; }

        [ProtoMember(2)]
        public override SelectAccountDetailsModel SelectAccountDetailsModel { get; set; } = new SelectAccountDetailsModel();

        private List<string> _listFriendProfileUrl = new List<string>();

        [ProtoMember(3)]
        public List<string> ListFriendProfileUrl
        {
            get { return _listFriendProfileUrl; }
            set
            {
                SetProperty(ref _listFriendProfileUrl, value);
            }
        }

        private List<string> _listGroupUrl = new List<string>();

        [ProtoMember(4)]
        public List<string> ListGroupUrl
        {
            get { return _listGroupUrl; }
            set
            {
                SetProperty(ref _listGroupUrl, value);
            }
        }

        private bool _isSelctDetails;

        [ProtoMember(5)]
        public bool IsSelctDetails
        {
            get { return _isSelctDetails; }
            set
            {
                SetProperty(ref _isSelctDetails, value);
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
