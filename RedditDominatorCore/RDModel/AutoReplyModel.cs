using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace RedditDominatorCore.RDModel
{
    public class AutoReplyModel : ModuleSetting
    {

        private bool _isReplyToMessagesThatContainSpecificWord;

        [ProtoMember(2)]
        public bool IsReplyToMessagesThatContainSpecificWord
        {
            get { return _isReplyToMessagesThatContainSpecificWord; }
            set { SetProperty(ref _isReplyToMessagesThatContainSpecificWord, value); }
        }


        [ProtoMember(1)] public override JobConfiguration JobConfiguration { get; set; }

        private string _message;

        [ProtoMember(3)]
        public string Message
        {
            get => _message;
            set
            {
                if (_message == value) return;
                SetProperty(ref _message, value);
            }
        }

        private string _specificWord;

        [ProtoMember(4)]
        public string SpecificWord
        {
            get { return _specificWord; }
            set
            {
                if (_specificWord == value) return;
                SetProperty(ref _specificWord, value);
            }
        }

        private bool _isSkipPrivateBlackListUser;

        [ProtoMember(5)]
        public bool IsSkipPrivateBlackListUser
        {
            get { return _isSkipPrivateBlackListUser; }
            set
            {
                if (_isSkipPrivateBlackListUser == value) return;
                SetProperty(ref _isSkipPrivateBlackListUser, value);
            }
        }

        private bool _isSkipGroupBlackListUsers;

        [ProtoMember(6)]
        public bool IsSkipGroupBlackListUsers
        {
            get { return _isSkipGroupBlackListUsers; }
            set
            {
                if (_isSkipGroupBlackListUsers == value) return;
                SetProperty(ref _isSkipGroupBlackListUsers, value);
            }
        }
        private bool _IsSpintax;

        [ProtoMember(9)]
        public bool IsSpintax
        {
            get => _IsSpintax;
            set => SetProperty(ref _IsSpintax, value);
        }

        #region Manage Speed 

        /// <summary>
        ///     Slow week 150
        ///     Medium week 300
        ///     Fast week 450
        ///     SuperFast week 600
        /// </summary>
        public JobConfiguration SlowSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(15, 20),
            ActivitiesPerHour = new RangeUtilities(4, 6),
            ActivitiesPerWeek = new RangeUtilities(120, 150),
            ActivitiesPerJob = new RangeUtilities(2, 3),
            DelayBetweenJobs = new RangeUtilities(20, 30),
            DelayBetweenActivity = new RangeUtilities(40, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(30, 45),
            ActivitiesPerHour = new RangeUtilities(8, 12),
            ActivitiesPerWeek = new RangeUtilities(250, 300),
            ActivitiesPerJob = new RangeUtilities(3, 4),
            DelayBetweenJobs = new RangeUtilities(50, 80),
            DelayBetweenActivity = new RangeUtilities(40, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration FastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(60, 70),
            ActivitiesPerHour = new RangeUtilities(10, 15),
            ActivitiesPerWeek = new RangeUtilities(400, 450),
            ActivitiesPerJob = new RangeUtilities(6, 8),
            DelayBetweenJobs = new RangeUtilities(100, 150),
            DelayBetweenActivity = new RangeUtilities(40, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(80, 90),
            ActivitiesPerHour = new RangeUtilities(18, 25),
            ActivitiesPerWeek = new RangeUtilities(500, 600),
            ActivitiesPerJob = new RangeUtilities(10, 15),
            DelayBetweenJobs = new RangeUtilities(180, 220),
            DelayBetweenActivity = new RangeUtilities(40, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        #endregion
    }
}
