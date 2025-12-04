using RedditDominatorCore.RDModel;
using System;

namespace RedditDominatorCore.RDLibrary
{
    internal static class ScrapeFilter
    {
        public class Post : Settings
        {
            public Post(ModuleSetting settings) : base(settings)
            {
            }

            public bool IsFilterApplied()
            {
                return ModuleSetting.PostFilterModel.FilterArchived || ModuleSetting.PostFilterModel.FilterBlankPost ||
                       ModuleSetting.PostFilterModel.FilterCommentsCount ||
                       ModuleSetting.PostFilterModel.FilterCrossPostsCount ||
                       ModuleSetting.PostFilterModel.FilterGoldCounts || ModuleSetting.PostFilterModel.FilterHidden ||
                       ModuleSetting.PostFilterModel.FilterLocked ||
                       ModuleSetting.PostFilterModel.FilterOriginalContent || ModuleSetting.PostFilterModel.FilterPinned
                       || ModuleSetting.PostFilterModel.FilterRoadblock || ModuleSetting.PostFilterModel.FilterScore
                       || ModuleSetting.PostFilterModel.FilterSponsored ||
                       ModuleSetting.PostFilterModel.FilterViewCount || ModuleSetting.PostFilterModel.IsCheckedFlairFilter;
            }

            public bool ApplyFilters(RedditPost post)
            {
                if (FilterBlankPost(post))
                    return false;
                if (FilterCommentsCount(post))
                    return false;
                if (FilterCrossPostsCount(post))
                    return false;
                if (FilterGoldCounts(post))
                    return false;
                if (FilterScore(post))
                    return false;
                if (FilterViewCount(post))
                    return false;
                if (FilterHidden(post))
                    return false;
                if (FilterLocked(post))
                    return false;
                if (FilterOriginalContent(post))
                    return false;
                if (FilterArchived(post))
                    return false;
                //if (FilterPinned(post))
                //    return false;
                if (FilterRoadblock(post))
                    return false;
                if (FilterSponsored(post))
                    return false;
                if (FilterFlair(post))
                    return false;
                return true;
            }

            public bool FilterFlair(RedditPost post)
            {
                if (ModuleSetting.PostFilterModel.IsCheckedFlairFilter)
                    return IsMatchingWithFlairFilter(post);
                return false;
            }

            private bool IsMatchingWithFlairFilter(RedditPost post)
            {
                try
                {
                    if (ModuleSetting.PostFilterModel.IsCheckedCustomFlair)
                        return !post.FlairText.ToLower().Contains(ModuleSetting.PostFilterModel.CustomFlairText?.Trim()?.ToLower());
                    string FlairText = ModuleSetting.PostFilterModel.IsCheckedGeneralDiscourse ? "General Discourse" :
                        ModuleSetting.PostFilterModel.IsCheckedBlastFromPast ? "BlastFromPast" :
                        ModuleSetting.PostFilterModel.IsCheckedDiscuss ? "Discuss" :
                        ModuleSetting.PostFilterModel.IsCheckedDiscussion ? "Discussion" :
                        ModuleSetting.PostFilterModel.IsCheckedHighlights ? "Highlights" :
                        ModuleSetting.PostFilterModel.IsCheckedImage ? "Image" :
                        ModuleSetting.PostFilterModel.IsCheckedImUnoriginal ? "Im Unoriginal" :
                        ModuleSetting.PostFilterModel.IsCheckedPhoto ? "Photo" :
                        ModuleSetting.PostFilterModel.IsCheckedStats ? "Stats" :
                        ModuleSetting.PostFilterModel.IsCheckedVideo ? "Video" :
                        ModuleSetting.PostFilterModel.IsCheckedWedding ? "Wedding" : "";
                    if (!string.IsNullOrEmpty(FlairText) && !string.IsNullOrEmpty(post.FlairText))
                        return !post.FlairText.Contains(FlairText);
                    return true;
                }
                catch (Exception) { return true; }
            }

            #region FilterArchived

            public bool FilterArchived(RedditPost post)
            {
                if (ModuleSetting.PostFilterModel.FilterArchived)
                    return IsArchived(post);
                return false;
            }

            public bool IsArchived(RedditPost post)
            {
                return !post.IsArchived;
            }

            #endregion

            #region FilterBlankPost

            public bool FilterBlankPost(RedditPost post)
            {
                if (ModuleSetting.PostFilterModel.FilterBlankPost)
                    return IsBlankPost(post);
                return false;
            }

            public bool IsBlankPost(RedditPost post)
            {
                return !post.IsBlank;
            }

            #endregion

            #region FilterCommentsCount

            public bool FilterCommentsCount(RedditPost post)
            {
                if (ModuleSetting.PostFilterModel.FilterCommentsCount)
                    return IsCommentsCountsIsInRange(post);
                return false;
            }

            public bool IsCommentsCountsIsInRange(RedditPost post)
            {
                return !ModuleSetting.PostFilterModel.CommentsCount.InRange(post.NumComments);
            }

            #endregion

            #region FilterCrossPostsCount

            public bool FilterCrossPostsCount(RedditPost post)
            {
                if (ModuleSetting.PostFilterModel.FilterCrossPostsCount)
                    return IsCrossPostsCountsIsInRange(post);
                return false;
            }

            public bool IsCrossPostsCountsIsInRange(RedditPost post)
            {
                return !ModuleSetting.PostFilterModel.CrossPostsCount.InRange(post.NumCrossposts);
            }

            #endregion

            #region FilterGoldCounts

            public bool FilterGoldCounts(RedditPost post)
            {
                if (ModuleSetting.PostFilterModel.FilterGoldCounts)
                    return IsGoldCountIsInRange(post);
                return false;
            }

            public bool IsGoldCountIsInRange(RedditPost post)
            {
                return !ModuleSetting.PostFilterModel.GoldCounts.InRange(post.GoldCount);
            }

            #endregion

            #region FilterScore

            public bool FilterScore(RedditPost post)
            {
                if (ModuleSetting.PostFilterModel.FilterScore)
                    return IsScoreInRange(post);
                return false;
            }

            public bool IsScoreInRange(RedditPost post)
            {
                return !ModuleSetting.PostFilterModel.Score.InRange(post.Score);
            }

            #endregion

            #region FilterViewCount

            public bool FilterViewCount(RedditPost post)
            {
                if (ModuleSetting.PostFilterModel.FilterViewCount)
                    return IsViewCountInRange(post);
                return false;
            }

            public bool IsViewCountInRange(RedditPost post)
            {
                return !ModuleSetting.PostFilterModel.ViewCount.InRange(post.Score);
            }

            #endregion

            #region FilterHidden

            public bool FilterHidden(RedditPost post)
            {
                if (ModuleSetting.PostFilterModel.FilterHidden)
                    return IsHidden(post);
                return false;
            }

            public bool IsHidden(RedditPost post)
            {
                return !post.Hidden;
            }

            #endregion

            #region FilterLocked

            public bool FilterLocked(RedditPost post)
            {
                if (ModuleSetting.PostFilterModel.FilterLocked)
                    return IsLocked(post);
                return false;
            }

            public bool IsLocked(RedditPost post)
            {
                return !post.IsLocked;
            }

            #endregion

            #region FilterOriginalContent

            public bool FilterOriginalContent(RedditPost post)
            {
                if (ModuleSetting.PostFilterModel.FilterOriginalContent)
                    return IsOriginalContent(post);
                return false;
            }

            public bool IsOriginalContent(RedditPost post)
            {
                return !post.IsOriginalContent;
            }

            #endregion

            #region FilterPinned (Removed)

            //public bool FilterPinned(RedditPost post)
            //{
            //    if (!this.ModuleSetting.PostFilterModel.FilterPinned)
            //        return false;
            //    return IsPinned(post);
            //}

            //public bool IsPinned(RedditPost post)
            //{
            //    return post.isPinned;
            //}

            #endregion

            #region FilterRoadblock

            public bool FilterRoadblock(RedditPost post)
            {
                if (ModuleSetting.PostFilterModel.FilterRoadblock)
                    return IsRoadBlock(post);
                return false;
            }

            public bool IsRoadBlock(RedditPost post)
            {
                return !post.IsRoadblock;
            }

            #endregion

            #region FilterSponsored

            public bool FilterSponsored(RedditPost post)
            {
                if (ModuleSetting.PostFilterModel.FilterSponsored)
                    return IsSponsored(post);
                return false;
            }

            public bool IsSponsored(RedditPost post)
            {
                return post.IsSponsored;
            }

            #endregion
        }

        public class Settings
        {
            protected readonly ModuleSetting ModuleSetting;

            protected Settings(ModuleSetting settings)
            {
                ModuleSetting = settings;
            }
        }

        public class User : Settings
        {
            public User(ModuleSetting settings) : base(settings)
            {
            }

            public bool IsFilterApplied()
            {
                return ModuleSetting.UserFilterModel.FilterEmployees ||
                       ModuleSetting.UserFilterModel.FilterModerators ||
                       ModuleSetting.UserFilterModel.FilterGoldUsers || ModuleSetting.UserFilterModel.FilterNsfwUsers ||
                       ModuleSetting.UserFilterModel.FilterAlreadyFollowed ||
                       ModuleSetting.UserFilterModel.FilterCommentKarmaCounts;
            }

            public bool AppplyFilters(RedditUser user)
            {
                return FilterEmployees(user) ? false :
                    FilterGoldUsers(user) ? false :
                    FilterModerators(user) ? false :
                    FilterNsfwUsers(user) ? false :
                    FilterCommentKarmaCounts(user) ? false : true;
            }

            #region FilterEmployees

            public bool FilterEmployees(RedditUser user)
            {
                if (ModuleSetting.UserFilterModel.FilterEmployees)
                    return IsEmployee(user);
                return false;
            }

            public bool IsEmployee(RedditUser user)
            {
                return user.IsEmployee;
            }

            #endregion

            #region FilterGoldUsers

            public bool FilterGoldUsers(RedditUser user)
            {
                if (ModuleSetting.UserFilterModel.FilterGoldUsers)
                    return IsGoldUser(user);
                return false;
            }

            public bool IsGoldUser(RedditUser user)
            {
                return user.IsGold;
            }

            #endregion

            #region FilterModerators

            public bool FilterModerators(RedditUser user)
            {
                if (ModuleSetting.UserFilterModel.FilterModerators)
                    return IsModerator(user);
                return false;
            }

            public bool IsModerator(RedditUser user)
            {
                return user.IsMod;
            }

            #endregion

            #region FilterNsfwUsers

            public bool FilterNsfwUsers(RedditUser user)
            {
                if (ModuleSetting.UserFilterModel.FilterNsfwUsers)
                    return IsNsfwUser(user);
                return false;
            }

            public bool IsNsfwUser(RedditUser user)
            {
                return user.IsNsfw;
            }

            #endregion

            #region FilterCommentKarmaCounts

            public bool FilterCommentKarmaCounts(RedditUser user)
            {
                if (ModuleSetting.UserFilterModel.FilterCommentKarmaCounts)
                    return IsCommentKarmaCountsIsInRange(user);
                return false;
            }

            public bool IsCommentKarmaCountsIsInRange(RedditUser user)
            {
                return !ModuleSetting.UserFilterModel.CommentKarmaCounts.InRange(user.CommentKarma);
            }

            #endregion
        }

        public class Community : Settings
        {
            public Community(ModuleSetting settings) : base(settings)
            {
            }

            public bool IsFilterApplied()
            {
                return ModuleSetting.CommunityFiltersModel.FilterNsfwCommmunities ||
                       ModuleSetting.CommunityFiltersModel.FilterallOriginalContentCommmunities ||
                       ModuleSetting.CommunityFiltersModel.FilteremojisEnabledCommmunities ||
                       ModuleSetting.CommunityFiltersModel.FilterisQuarantinedCommmunities ||
                       ModuleSetting.CommunityFiltersModel.FilteroriginalContentTagEnabledCommmunities ||
                       ModuleSetting.CommunityFiltersModel.FilterMediaCommmunities ||
                       ModuleSetting.CommunityFiltersModel.FilterSubscribersCounts ||
                       ModuleSetting.CommunityFiltersModel.FilteruserIsSubscriberCommmunities ||
                       ModuleSetting.CommunityFiltersModel.FilterwlsCounts;
            }

            public bool AppplyFilters(SubRedditModel community)
            {
                if (FilterNsfwCommmunities(community))
                    return false;
                if (FilterallOriginalContentCommmunities(community))
                    return false;
                if (FilteremojisEnabledCommmunities(community))
                    return false;
                if (FilterisQuarantinedCommmunities(community))
                    return false;
                if (FilteroriginalContentTagEnabledCommmunities(community))
                    return false;
                if (FilterSubscribersCounts(community))
                    return false;
                if (FilteruserIsSubscriberCommmunities(community))
                    return false;
                if (FilterwlsCounts(community))
                    return false;
                if (FilterNotShowCommunities(community))
                    return false;
                return !FiltershowMediaCommmunities(community);
            }
            #region FilterNotShowMedia
            private bool FilterNotShowCommunities(SubRedditModel community)
            {
                return ModuleSetting.CommunityFiltersModel.FilterMediaCommmunities &&
                    ModuleSetting.CommunityFiltersModel.FilterNotshowMediaCommmunities && IsNotShowMedia(community);
            }

            private bool IsNotShowMedia(SubRedditModel community)
            {
                return community.ShowMedia;
            }
            #endregion
            #region FilterNsfwCommmunities

            public bool FilterNsfwCommmunities(SubRedditModel community)
            {
                return ModuleSetting.CommunityFiltersModel.FilterNsfwCommmunities && IsNsfw(community);
            }

            public bool IsNsfw(SubRedditModel community)
            {
                return !community.IsNsfw;
            }

            #endregion

            #region FilterallOriginalContentCommmunities

            public bool FilterallOriginalContentCommmunities(SubRedditModel community)
            {
                return ModuleSetting.CommunityFiltersModel.FilterallOriginalContentCommmunities &&
                       AllOriginalContent(community);
            }

            public bool AllOriginalContent(SubRedditModel community)
            {
                return !community.AllOriginalContent;
            }

            #endregion

            #region FilteremojisEnabledCommmunities

            public bool FilteremojisEnabledCommmunities(SubRedditModel community)
            {
                return ModuleSetting.CommunityFiltersModel.FilteremojisEnabledCommmunities && EmojisEnabled(community);
            }

            public bool EmojisEnabled(SubRedditModel community)
            {
                return !community.EmojisEnabled;
            }

            #endregion

            #region FilterisQuarantinedCommmunities

            public bool FilterisQuarantinedCommmunities(SubRedditModel community)
            {
                return ModuleSetting.CommunityFiltersModel.FilterisQuarantinedCommmunities && IsQuarantined(community);
            }

            public bool IsQuarantined(SubRedditModel community)
            {
                return !community.IsQuarantined;
            }

            #endregion

            #region FilteroriginalContentTagEnabledCommmunities

            public bool FilteroriginalContentTagEnabledCommmunities(SubRedditModel community)
            {
                return ModuleSetting.CommunityFiltersModel.FilteroriginalContentTagEnabledCommmunities &&
                       OriginalContentTagEnabled(community);
            }

            public bool OriginalContentTagEnabled(SubRedditModel community)
            {
                return !community.OriginalContentTagEnabled;
            }

            #endregion

            #region FilteruserIsSubscriberCommmunities

            public bool FilteruserIsSubscriberCommmunities(SubRedditModel community)
            {
                return ModuleSetting.CommunityFiltersModel.FilteruserIsSubscriberCommmunities &&
                       UserIsSubscriber(community);
            }

            public bool UserIsSubscriber(SubRedditModel community)
            {
                return !community.UserIsSubscriber;
            }

            #endregion

            #region FiltershowMediaCommmunities

            public bool FiltershowMediaCommmunities(SubRedditModel community)
            {
                return ModuleSetting.CommunityFiltersModel.FilterMediaCommmunities &&
                    ModuleSetting.CommunityFiltersModel.FiltershowMediaCommmunities && ShowMedia(community);
            }

            public bool ShowMedia(SubRedditModel community)
            {
                return !community.ShowMedia;
            }

            #endregion

            #region FilterSubscribersCounts

            public bool FilterSubscribersCounts(SubRedditModel community)
            {
                return ModuleSetting.CommunityFiltersModel.FilterSubscribersCounts && Subscribers(community);
            }

            public bool Subscribers(SubRedditModel community)
            {
                return !ModuleSetting.CommunityFiltersModel.SubscribersCounts.InRange(community.Subscribers);
            }

            #endregion

            #region FilterwlsCounts

            public bool FilterwlsCounts(SubRedditModel community)
            {
                return ModuleSetting.CommunityFiltersModel.FilterwlsCounts && Wls(community);
            }

            public bool Wls(SubRedditModel community)
            {
                return !ModuleSetting.CommunityFiltersModel.WlsCounts.InRange(community.Wls);
            }

            #endregion
        }
    }
}