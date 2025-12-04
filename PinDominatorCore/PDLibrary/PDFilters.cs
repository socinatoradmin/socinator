using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DominatorHouseCore.Enums;
using PinDominatorCore.Interface;
using PinDominatorCore.PDModel;
using PinDominatorCore.Response;

namespace PinDominatorCore.PDLibrary
{
    internal static class PdFilters
    {
        public class Settings
        {
            protected readonly ModuleSetting ModuleSetting;

            protected Settings(ModuleSetting settings)
            {
                ModuleSetting = settings;
            }
        }

        public class PinFilterFunctions : Settings
        {
            public PinFilterFunctions(ModuleSetting settings)
                : base(settings)
            {
            }

            private IPostFilter PostFilterModel => ModuleSetting.PostFilterModel;

            public bool IsImagePostOverAged(PinterestPin x)
            {
                if (string.IsNullOrEmpty(x.PublishDate))
                    return true;
                if (ModuleSetting.PostFilterModel.IsPostedBefore)
                    return (DateTime.UtcNow - Convert.ToDateTime(x.PublishDate)).TotalDays
                        < ModuleSetting.PostFilterModel.PostedBeforeDays;

                return (DateTime.UtcNow - Convert.ToDateTime(x.PublishDate)).TotalDays >
                       ModuleSetting.PostFilterModel.PostedInLastDays;
            }

            public bool IsCommentsCountInRange(PinterestPin x)
            {
                return !ModuleSetting.PostFilterModel.CommentsCountRange.InRange(x.CommentCount);
            }

            public bool IsTryCountInRange(PinterestPin x)
            {
                return !ModuleSetting.PostFilterModel.TriedCountRange.InRange(x.NoOfTried);
            }

            public bool IsPostTypeIgnored(PinterestPin x)
            {
                if (PostFilterModel.PostCategory.IgnorePostImages)
                    return x.MediaType == MediaType.Image;
                if (PostFilterModel.PostCategory.IgnorePostVideos)
                    return x.MediaType == MediaType.Video;
                return false;
            }

            public bool IsRestrictedPost(PinterestPin pin)
            {
                foreach (var caption in PostFilterModel.RestrictedPostCaptionList)
                {
                    var isFilter = pin.Description.Contains(caption);
                    if (isFilter)
                        return isFilter;
                }

                return false;
            }

            public bool IsAcceptedPost(PinterestPin pin)
            {
                foreach (var caption in PostFilterModel.AcceptedPostCaptionList)
                {
                    var isFilter = !pin.Description.Contains(caption);
                    if (isFilter)
                        return isFilter;
                }

                return false;
            }
        }

        public class UserFilterFunctions : Settings
        {
            private List<UserNameInfoPtResponseHandler> _detailedInfo;

            public UserFilterFunctions(ModuleSetting settings)
                : base(settings)
            {
            }

            public List<UserNameInfoPtResponseHandler> DetailedInfo
            {
                private get
                {
                    if (_detailedInfo == null)
                        throw new ArgumentException("DetailedInfo needs to be set to be able to use this filter");
                    return _detailedInfo;
                }
                set { _detailedInfo = value; }
            }

            private IUserFilter UserFilter => ModuleSetting.UserFilterModel;

            public bool IsPinCountInRange(PinterestUser pinterestUser)
            {
                return !UserFilter.PinCounts.InRange(pinterestUser.PinsCount);
            }

            public bool FilterByBioCharacterLength(PinterestUser user)
            {
                return user.UserBio.Length < UserFilter.MinimumCharacterInBio;
            }

            public bool IsFollowingsCountInRange(PinterestUser pinterestUser)
            {
                return !UserFilter.FollowingsCount.InRange(pinterestUser.FollowingsCount);
            }

            public bool IsFollowersCountInRange(PinterestUser pinterestUser)
            {
                return !UserFilter.FollowersCount.InRange(pinterestUser.FollowersCount);
            }

            public bool IsBioContainRestrictedWords(PinterestUser user)
            {
                if (UserFilter.LstInvalidWord.Any(y => user.UserBio.Contains(y)))
                    return true;
                return false;
            }

            public bool CheckForMinimumFollowRatio(PinterestUser user)
            {
                return !(user.FollowingsCount != 0 &&
                         user.FollowersCount / (double)user.FollowingsCount < UserFilter.MinimumFollowRatio);
            }

            public bool CheckForMaximumFollowRatio(PinterestUser user)
            {
                return !(user.FollowingsCount != 0 &&
                         user.FollowersCount / (double)user.FollowingsCount > UserFilter.MaximumFollowRatio);
            }

            public bool CheckFollowRatioInSpecificRange(PinterestUser user)
            {
                return user.FollowingsCount != 0
                       && ModuleSetting.UserFilterModel.SpecificFollowRatio.InRange(
                           (int)(user.FollowersCount / (double)user.FollowingsCount));
            }

            public bool IsNonEnglishUser(PinterestUser user)
            {
                // Removes Alphabets, Letters and white spaces from a text string
                var filterBiography = Regex.Replace(user.UserBio, @"[A-Za-z0-9\s+]", "");
                // Removes Emoji's from a text string
                filterBiography = Regex.Replace(filterBiography, @"\p{Cs}", "");

                #region Removes special characters from a text string

                string[] chars =
                {
                    "\\n", "?", ",", ".", "/", "!", "@", "#", "$", "%", "^", "&", "*", "'", "\"", ";", "_", "-", "(",
                    ")", ":", "|", "[", "]", "♡", "•"
                };
                foreach (var t in chars)
                    if (filterBiography.Contains(t))
                        filterBiography = filterBiography.Replace(t, "");

                #endregion

                // After removing alphabets, letters and emojis 
                // if string is empty then it returns "English" 
                // otherwise "Non-english".
                if (user.UserBio.Length > filterBiography.Length && filterBiography == string.Empty)
                    return false;
                return true;
            }

            public bool IsAnonymousProfilePicture(PinterestUser x)
            {
                return !x.HasProfilePic;
            }
        }
    }
}