using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorMigration.GramDominator.Models
{
    public class Campaign
    {
        public bool _DoUniqueOperationInPostComment { get; set; }
        public bool _DoUniqueOperationInMention { get; set; }
        public bool _DoUniqueOperation { get; set; }
        public bool _FilterInPostMustContainCaption { get; set; }
        public string campaignName { get; set; }
        public bool FilterByBlackListedUser { get; set; }
        public object[] lst_MustContainPostCaption { get; set; }
        public object[] ListBlackListedUser { get; set; }
        public bool FilterByWhiteListedUser { get; set; }
        public object[] ListWhiteListedUser { get; set; }
        public int AccountId { get; set; }
        public int CampaignId { get; set; }
        public int IsCancelEditVisibility { get; set; }
        public int runNowVisibility { get; set; }
        public string BtnCampaignName { get; set; }
        public string[] list_SelectedAccounts { get; set; }
        public string SelectedAccount { get; set; }
        public string TxtUploadedData { get; set; }
        public bool IgnoreUserAlreadyLiked { get; set; }
        public bool LikePerUsers { get; set; }
        public int MaxLikePerUsers { get; set; }
        public int MinLikePerUsers { get; set; }
        public int ThreadCount { get; set; }
        public string MentionuserRange { get; set; }
        public bool IsMention { get; set; }
        public bool Is_mentionLoaduser { get; set; }
        public bool Is_mentionFollowing { get; set; }
        public bool chkLikeBetween { get; set; }
        public bool chkFilterBydate { get; set; }
        public bool chkFilterByLikes { get; set; }
        public bool chkFilterByComments { get; set; }
        public bool SearchByKeyword { get; set; }
        public bool SearchByHashtagUsers { get; set; }
        public bool SearchByHashtagPosts { get; set; }
        public bool SearchByFollowers { get; set; }
        public bool SearchByFollowings { get; set; }
        public bool CustomList { get; set; }
        public bool SearchBylocationUsers { get; set; }
        public bool SearchByLikerUser { get; set; }
        public bool SearchByCommenteruser { get; set; }
        public bool CustomListUsers { get; set; }
        public bool CustomListPhotos { get; set; }
        public bool SearchByOwnFollowers { get; set; }
        public bool SearchByOwnFollowings { get; set; }
        public bool SearchBylocationPosts { get; set; }
        public bool IsLike { get; set; }
        public bool DoUniqueOperationInPostComment { get; set; }
        public bool DoUniqueOperationInMention { get; set; }
        public bool DoUniqueOperation { get; set; }
        public bool FilterInPostMustContainCaption { get; set; }
        public bool Iscomment { get; set; }
        public bool IsFollow { get; set; }
        public bool IsUnlike { get; set; }
        public object[] ListComments { get; set; }
        public bool IsDirectMessage { get; set; }
        public bool IsPost { get; set; }
        public bool IsUnfollow { get; set; }
        public object[] ListMessages { get; set; }
        public int AccountCount { get; set; }
        public string[] ListUsernameForMention { get; set; }
        public bool FilterPostedsDate { get; set; }
        public int PostsOlderThan { get; set; }
        public bool FilterByLikes { get; set; }
        public int MinLikes { get; set; }
        public bool FilterByComments { get; set; }
        public int Mincomments { get; set; }
        public int Maxcomments { get; set; }
        public int MaxLikes { get; set; }
        public bool FilterIsMustNotContainWordChecked { get; set; }
        public object TxtFilterWord { get; set; }
        public int IncreaseMaxActionsPerDayLimit { get; set; }
        public bool IsStopDate { get; set; }
        public int IncreaseMaxActionsPerDay { get; set; }
        public bool IsLoginFail { get; set; }
        public bool IsRunDaily { get; set; }
        public bool IsRunOnSeletedDays { get; set; }
        public bool IsStartNow { get; set; }
        public object[] ListSelectedDays { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }
        public DateTime StartTimeMin { get; set; }
        public bool IsRunOnSunday { get; set; }
        public bool IsRunOnMonday { get; set; }
        public bool IsRunOnTuesday { get; set; }
        public bool IsRunOnWednesday { get; set; }
        public bool IsRunOnThursday { get; set; }
        public bool IsRunOnFriday { get; set; }
        public bool IsRunOnSaturday { get; set; }
        public int MinDelayBetweenJobs { get; set; }
        public int MaxDelayBetweenJobs { get; set; }
        public int MinActionPerJob { get; set; }
        public int MaxActionPerJob { get; set; }
        public int MinActionsPerDay { get; set; }
        public int MaxActionsPerDay { get; set; }
        public int ActionsPerformedInJob { get; set; }
        public int ActionsPerformedToday { get; set; }
        public int MinDelayCount { get; set; }
        public int MaxDelayCount { get; set; }
        public bool InSufficientSource { get; set; }
        public bool IsStopTime { get; set; }
        public DateTime StartDateOnly { get; set; }
        public DateTime StartTimeOnly { get; set; }
        public DateTime StopDateOnly { get; set; }
        public DateTime StopTimeOnly { get; set; }
        public bool IsCustomCompleted { get; set; }
    }

    public class CampaignDetails
    {
        public long Id { get; set; }
        public long? CampaignsId { get; set; }
        public long? AccountId { get; set; }
        public Campaign CampaignJSON { get; set; }
        public long? Status { get; set; }
        public long? RunningForCampaignId { get; set; }
        public DateTime? ScheduledTime { get; set; }

        public virtual GramDominatorAccount GramDominatorAccount { get; set; }
        public string Name { get; set; }
        public string ModuleType { get; set; }
    }

    public class GramDominatorAccount
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string EmailPassword { get; set; }
        public string ProxyId { get; set; }
        public string PostCount { get; set; }
        public string FollowingCount { get; set; }
        public string FollowerCount { get; set; }
        public string WebCookie { get; set; }
        public string MobileCookie { get; set; }
        public string AppId { get; set; }
        public string DevUid { get; set; }
        public string WebUserAgent { get; set; }
        public string MobileUserAgent { get; set; }
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
        public string Authorized { get; set; }
        public string Health { get; set; }
        public string Status { get; set; }
        public string GroupName { get; set; }
        public string DateAdded { get; set; }
    }
}
