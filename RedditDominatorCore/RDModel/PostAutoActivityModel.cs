using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace RedditDominatorCore.RDModel
{
    public class PostAutoActivityModel : ModuleSetting, IGeneralSettings
    {
        private bool _IsChkUpvote;
        private bool _IsChkDownvote;
        private bool _IsChkUpvoteDownvoteComment;
        private bool _IsChkRemoveVote;
        private bool _IsChkRemoveVoteOfComment;
        private bool _IsChkFollowPostOwner;
        private bool _IsChkJoinPostCommunity;
        private bool _IsCheckVisitAndScroll;
        public bool IsChkUpvote { get => _IsChkUpvote; set => SetProperty(ref _IsChkUpvote, value); }
        public bool IsChkDownvote { get => _IsChkDownvote; set => SetProperty(ref _IsChkDownvote, value); }
        public bool IsChkUpvoteDownvoteComment { get => _IsChkUpvoteDownvoteComment; set => SetProperty(ref _IsChkUpvoteDownvoteComment, value); }
        public bool IsChkRemoveVote { get => _IsChkRemoveVote; set => SetProperty(ref _IsChkRemoveVote, value); }
        public bool IsChkRemoveVoteOfComment { get => _IsChkRemoveVoteOfComment; set => SetProperty(ref _IsChkRemoveVoteOfComment, value); }
        public bool IsChkFollowPostOwner { get => _IsChkFollowPostOwner; set => SetProperty(ref _IsChkFollowPostOwner, value); }
        public bool IsChkJoinPostCommunity { get => _IsChkJoinPostCommunity; set => SetProperty(ref _IsChkJoinPostCommunity, value); }
        public bool IsCheckVisitAndScroll { get => _IsCheckVisitAndScroll; set => SetProperty(ref _IsCheckVisitAndScroll, value); }

        public JobConfiguration FastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(350, 550),
            ActivitiesPerHour = new RangeUtilities(40, 60),
            ActivitiesPerWeek = new RangeUtilities(2200, 3000),
            ActivitiesPerJob = new RangeUtilities(40, 50),
            DelayBetweenJobs = new RangeUtilities(30, 60),
            DelayBetweenActivity = new RangeUtilities(30, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(200, 300),
            ActivitiesPerHour = new RangeUtilities(30, 50),
            ActivitiesPerWeek = new RangeUtilities(1200, 1800),
            ActivitiesPerJob = new RangeUtilities(25, 35),
            DelayBetweenJobs = new RangeUtilities(50, 70),
            DelayBetweenActivity = new RangeUtilities(25, 40),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration SlowSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(100, 200),
            ActivitiesPerHour = new RangeUtilities(25, 40),
            ActivitiesPerWeek = new RangeUtilities(500, 700),
            ActivitiesPerJob = new RangeUtilities(15, 25),
            DelayBetweenJobs = new RangeUtilities(40, 60),
            DelayBetweenActivity = new RangeUtilities(30, 40),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration SuperfastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(668, 1000),
            ActivitiesPerHour = new RangeUtilities(80, 100),
            ActivitiesPerWeek = new RangeUtilities(4000, 6000),
            ActivitiesPerJob = new RangeUtilities(50, 70),
            DelayBetweenJobs = new RangeUtilities(40, 74),
            DelayBetweenActivity = new RangeUtilities(20, 35),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };
    }
}
