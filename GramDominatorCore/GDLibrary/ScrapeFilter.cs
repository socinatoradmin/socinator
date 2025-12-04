using GramDominatorCore.GDEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GramDominatorCore.Interface;
using System.Text.RegularExpressions;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using MediaType = DominatorHouseCore.Enums.MediaType;

namespace GramDominatorCore.GDLibrary
{
    public static class ScrapeFilter
    {
        public class Image : Settings
        {
            #region Constructor

            public Image(ModuleSetting settings)
                 : base(settings)
            {

            }

            #endregion

            #region Properties

            // private IFilterPost PostFilter => (IFilterPost)ModuleSetting.PostFilterModel;

            // private IScrapingSettings ScrapingFilter => (IScrapingSettings)ModuleSetting;

            #endregion

            #region Media Filter Methods

            #region Filter liked images
            //public static void LikedImages(List<InstagramPost> images)
            //{
            //    images.RemoveAll(IsLiked);
            //}

            //public static bool LikedImages(InstagramPost image)
            //{
            //    return IsLiked(image);
            //}

            //private static bool IsLiked(InstagramPost x)
            //{
            //    return x.HasLiked;
            //}
            #endregion

            #region Filter images with Ads
            //public void FilterAds(List<InstagramPost> images)
            //{
            //    if (!ModuleSetting.PostFilterModel.IgnoreAdPost)
            //        return;
            //    images.RemoveAll(IsHavingAdd);
            //}

            //public bool FilterAds(InstagramPost image)
            //{
            //    if (!ModuleSetting.PostFilterModel.IgnoreAdPost)
            //        return false;
            //    return IsHavingAdd(image);
            //}

            //private static bool IsHavingAdd(InstagramPost x)
            //{
            //    return x.IsAd;
            //}
            #endregion

            #region Filter images with caption

            public bool FilterByPostCaption(InstagramPost instagramPost)
            {
                if (!ModuleSetting.PostFilterModel.FilterByBlacklistedWhitelistedWordsInCaption)
                    return false;
                return FilterByBlacklistedWhitelistedWordsInCaption(instagramPost);
            }
            private bool FilterByBlacklistedWhitelistedWordsInCaption(InstagramPost instagramPost)
            {
                if (ModuleSetting.PostFilterModel.FilterRestrictedPostCaptionList)
                    return IsImageCaptionHavingRestrictedCaption(instagramPost);

                if (ModuleSetting.PostFilterModel.FilterAcceptedPostCaptionList)
                    return IsImageCaptionHavingWhiteListCaption(instagramPost);

                return false;
            }

            public bool IsImageCaptionHavingRestrictedCaption(InstagramPost x)
            {
                if (string.IsNullOrEmpty(x.Caption))
                    x.Caption = "";
                return ModuleSetting.PostFilterModel.RestrictedPostCaptionList.Any(y => x.Caption.ToLower().Contains(y.ToLower()));
            }
            public List<InstagramPost> IsImageCaptionHavingRestrictedCaption(List<InstagramPost> images)
            {
                images.RemoveAll(x=>!string.IsNullOrEmpty(x.Caption) && ModuleSetting.PostFilterModel.RestrictedPostCaptionList.Any(y => x.Caption.ToLower().Contains(y.ToLower())));
                return images;
            }
            public bool IsImageCaptionHavingWhiteListCaption(InstagramPost x)
            {
                if (string.IsNullOrEmpty(x.Caption))
                    x.Caption = "";
                return !ModuleSetting.PostFilterModel.AcceptedPostCaptionList.Any(y => x.Caption.ToLower().Contains(y.ToLower()));
            }
            public List<InstagramPost> IsImageCaptionHavingWhiteListCaption(List<InstagramPost> images)
            {
                images.RemoveAll(x=>!string.IsNullOrEmpty(x.Caption) &&!ModuleSetting.PostFilterModel.AcceptedPostCaptionList.Any(y => x.Caption.ToLower().Contains(y.ToLower())));
                return images;
            }
            #endregion
            #region Filter with number of comments count

            public bool FilterComments(InstagramPost image)
            {
                if (!ModuleSetting.PostFilterModel.FilterComments)
                    return false;
                return IsCommentsCountInRange(image);
            }
            private bool IsCommentsCountInRange(InstagramPost x)
            {
                return !ModuleSetting.PostFilterModel.CommentsCountRange.InRange(x.CommentCount);
            }
            #endregion

            #region Filter with number of likes count
            public bool FilterLikes(InstagramPost image)
            {
                if (!ModuleSetting.PostFilterModel.FilterLikes)
                    return false;
               return IsLikesCountInRange(image);  
            }
            private bool IsLikesCountInRange(InstagramPost x)
            {
                return !ModuleSetting.PostFilterModel.LikesCountRange.InRange(x.LikeCount);
            }
            #endregion

            #region Filter with images post age
            

            public bool FilterPostAge(InstagramPost image)
            {
                if (!ModuleSetting.PostFilterModel.FilterPostAge)
                    return false;
                return IsImagePostOverAged(image);
            }
            public List<InstagramPost> FilterPostAge(List<InstagramPost> images)
            {
                if (ModuleSetting.PostFilterModel.FilterPostAge)
                    IsImagePostOverAged(images);
                return images;
            }
            private bool IsImagePostOverAged(InstagramPost x)
            {
                return CompareAndReturn(x, ModuleSetting.PostFilterModel.FilterBeforePostAge);
            }

            private bool CompareAndReturn(InstagramPost post, bool IsBeforeAge = false)
            {
                var dateProvider = InstanceProvider.GetInstance<IDateProvider>();
                var utcNow = dateProvider.UtcNow();
                var postUtc = post.TakenAt.EpochToDateTimeUtc();
                if(utcNow.Year == postUtc.Year)
                {
                    if(utcNow.Month == postUtc.Month)
                    {
                        var diffDay = utcNow.Day - postUtc.Day;
                        return IsBeforeAge ? diffDay < ModuleSetting.PostFilterModel.MaxPostAge : ModuleSetting.PostFilterModel.FilterLastPostAge ? diffDay > ModuleSetting.PostFilterModel.MaxLastPostAge:false;
                    }
                    else
                    {
                        var totalDays = (utcNow-postUtc).TotalDays;
                        return IsBeforeAge ? totalDays < ModuleSetting.PostFilterModel.MaxPostAge : ModuleSetting.PostFilterModel.FilterLastPostAge ? totalDays > ModuleSetting.PostFilterModel.MaxLastPostAge : false;
                    }
                }
                else
                {
                    var totalDays = (utcNow - postUtc).TotalDays;
                    return IsBeforeAge ? totalDays < ModuleSetting.PostFilterModel.MaxPostAge : ModuleSetting.PostFilterModel.FilterLastPostAge ? totalDays > ModuleSetting.PostFilterModel.MaxLastPostAge:false;
                }
            }

            private List<InstagramPost> IsImagePostOverAged(List<InstagramPost> images)
            {
                List<InstagramPost> posts = new List<InstagramPost>();
                images.ForEach(x=> {
                    bool check= (DateTime.UtcNow - x.TakenAt.EpochToDateTimeUtc()).TotalDays > ModuleSetting.PostFilterModel.MaxLastPostAge;

                    if (!check)
                        posts.Add(x);
                });
                images.RemoveAll(x=>posts.Any(y=>y.Code==x.Code));
                return images;
            }
            #endregion

            #region Filter with image post type
            //public void FilterPostType(List<InstagramPost> images)
            //{
            //    if (!ModuleSetting.PostFilterModel.PostCategory.FilterPostCategory)
            //        return;
            //    images.RemoveAll(IsImagePostTypeIgnored);
            //}

            public bool FilterPostType(InstagramPost image)
            {
                if (!ModuleSetting.PostFilterModel.PostCategory.FilterPostCategory)
                    return false;
                return IsImagePostTypeIgnored(image);
            }
            public bool IsImagePostTypeIgnored(InstagramPost image)
            {
                if (ModuleSetting.PostFilterModel.PostCategory.IgnorePostImages &&
                    image.MediaType == MediaType.Image)
                    return true;
                if (ModuleSetting.PostFilterModel.PostCategory.IgnorePostVideos &&
                    image.MediaType == MediaType.Video)
                    return true;
                if (ModuleSetting.PostFilterModel.PostCategory.IgnorePostAlbums &&
                    image.MediaType == MediaType.Album)
                    return true;

                return false;
            }
            #endregion

            #region Filter caption length

            public bool FilterCaptionLength(InstagramPost image)
            {
                if (!ModuleSetting.PostFilterModel.FilterCharsLenghInCaption)
                    return false;
                return IsImageCaptionLengthLesserThanMinimumValue(image);
            }
            public bool IsImageCaptionLengthLesserThanMinimumValue(InstagramPost instaPost)
            {
                if (instaPost.Caption != null)
                    return instaPost.Caption.Length < ModuleSetting.PostFilterModel.MinimumPostCaptionChars;
                return ModuleSetting.PostFilterModel.MinimumPostCaptionChars > 0;
            }
            #endregion

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

        [Localizable(false)]
        public class User : Settings
        {
            private List<UsernameInfoIgResponseHandler> detailedInfo;

            public User(ModuleSetting settings)
              : base(settings)
            {

            }

            public List<UsernameInfoIgResponseHandler> DetailedInfo
            {
                private get
                {
                    if (detailedInfo == null)
                        throw new ArgumentException("DetailedInfo needs to be set to be able to use this filter");
                    return detailedInfo;
                }
                set
                {
                    detailedInfo = value;
                }
            }

            private IUserFilter UserFilter => ModuleSetting.UserFilterModel;

            #region filter for bio blacklist

            public bool FilterBioRestrictedWords(UsernameInfoIgResponseHandler user)
            {
                if (!UserFilter.FilterBioRestrictedWords)
                    return false;
                return IsBioRestrictedWords(user);
            }
            public bool FilterBioRestrictedWords(InstagramUserDetails user)
            {
                if (!UserFilter.FilterBioRestrictedWords)
                    return false;
                return IsBioRestrictedWords(user);
            }
            public List<InstagramUser> FilterAllBioRestrictedWords(List<InstagramUser> user)
            {
                if (UserFilter.FilterBioRestrictedWords)
                    user.RemoveAll(x => UserFilter.LstInvalidWord.Any(z => x.UserDetails.Biography.ToLower().Contains(z.ToLower())));

                return user;
            }
            public List<InstagramPost> FilterAllPostBioRestrictedWords(List<InstagramPost> postUser)
            {
                if (UserFilter.FilterBioRestrictedWords)
                    postUser.RemoveAll(x => UserFilter.LstInvalidWord.Any(z => x.User.UserDetails.Biography.ToLower().Contains(z.ToLower())));

                return postUser;
            }
            public bool FilterBioNotRestrictedWords(UsernameInfoIgResponseHandler user)
            {
                if (!UserFilter.FilterBioNotRestrictedWords)
                    return false;
                return IsBioNotRestrictedWords(user);
            }
            public List<InstagramUser> FilterAllBioNotRestrictedWords(List<InstagramUser> user)
            {
                if (UserFilter.FilterBioNotRestrictedWords)
                {
                    user.RemoveAll(x => !UserFilter.LstvalidWord.Any(z => x.UserDetails.Biography.ToLower().Contains(z.ToLower())));
                    //!UserFilter.LstvalidWord.Any(y => user.Biography.ToLower().Contains(y.ToLower()))
                }
                return user;
            }

            public List<InstagramPost> FilterAllPostBioNotRestrictedWords(List<InstagramPost> postUser)
            {
                if (UserFilter.FilterBioNotRestrictedWords)
                {
                    postUser.RemoveAll(x => !UserFilter.LstvalidWord.Any(z => x.User.UserDetails.Biography.ToLower().Contains(z.ToLower())));
                    //!UserFilter.LstvalidWord.Any(y => user.Biography.ToLower().Contains(y.ToLower()))
                }
                return postUser;
            }

            public bool IsBioRestrictedWords(UsernameInfoIgResponseHandler user)
            {
                //if (user.Biography.Length < this.UserFilter.MinimumCharacterInBio || this.UserFilter.LstInvalidWord.Any<string>((Func<string, bool>)(y => user.Biography.ToLower().Contains(y.ToLower()))))
                if (UserFilter.LstInvalidWord.Any(y => user.Biography.ToLower().Contains(y.ToLower())))
                    return true;
                return false;
            }
            public bool IsBioRestrictedWords(InstagramUserDetails user)
            {
                //if (user.Biography.Length < this.UserFilter.MinimumCharacterInBio || this.UserFilter.LstInvalidWord.Any<string>((Func<string, bool>)(y => user.Biography.ToLower().Contains(y.ToLower()))))
                if (UserFilter.LstInvalidWord.Any(y => user.Biography.ToLower().Contains(y.ToLower())))
                    return true;
                return false;
            }
            public bool IsBioNotRestrictedWords(UsernameInfoIgResponseHandler user)
            {
                //if (user.Biography.Length < this.UserFilter.MinimumCharacterInBio || this.UserFilter.LstInvalidWord.Any<string>((Func<string, bool>)(y => user.Biography.ToLower().Contains(y.ToLower()))))
                if (!UserFilter.LstvalidWord.Any(y => user.Biography.ToLower().Contains(y.ToLower())))
                    return true;
                return false;
            }


            public bool FilterBioRestrictedWordsLength(UsernameInfoIgResponseHandler user)
            {
                if (!UserFilter.FilterMinimumCharacterInBio)
                    return false;
                return IsBioRestrictedWordsLength(user);
            }
            public bool FilterBioRestrictedWordsLength(InstagramUserDetails user)
            {
                if (!UserFilter.FilterMinimumCharacterInBio)
                    return false;
                return IsBioRestrictedWordsLength(user);
            }
            public List<InstagramUser> FilterAllBioRestrictedWordsLength(List<InstagramUser> user)
            {
                if (UserFilter.FilterMinimumCharacterInBio)
                    user.RemoveAll(x => x.UserDetails.Biography.Length < UserFilter.MinimumCharacterInBio);

                return user;
            }
            public List<InstagramPost> FilterAllPostBioRestrictedWordsLength(List<InstagramPost> postUser)
            {
                if (UserFilter.FilterMinimumCharacterInBio)
                    postUser.RemoveAll(x => x.User.UserDetails.Biography.Length < UserFilter.MinimumCharacterInBio);

                return postUser;
            }
            public bool IsBioRestrictedWordsLength(UsernameInfoIgResponseHandler user)
            {
                if (user.Biography.Length < UserFilter.MinimumCharacterInBio)
                    return true;
                return false;
            }
            public bool IsBioRestrictedWordsLength(InstagramUserDetails user)
            {
                if (user.Biography.Length < UserFilter.MinimumCharacterInBio)
                    return true;
                return false;
            }


            #endregion

            #region FilterIsVerified
            public bool FilterIsVerified(InstagramUser user)
            {
                if(ModuleSetting.UserFilterModel.VerifiedAccount)
                {
                    if (ModuleSetting.UserFilterModel.IgnoreVerifiedUser)
                    {
                        return user.IsVerified ? true : user.UserDetails.IsVerified ? true:false;//IsVerifiedIgnoreUser(user);
                    }
                    else if(ModuleSetting.UserFilterModel.IsVerifiedAccount)
                    {
                      return  user.IsVerified ? false : user.UserDetails.IsVerified ? false : true;
                    }
                }
                return false;
            }
            public List<InstagramUser> FilterIsVerifiedAll(List<InstagramUser> user)
            {
                if (ModuleSetting.UserFilterModel.IgnoreVerifiedUser)
                    user.RemoveAll(x => x.IsVerified == true);
                return user;
            }

            public List<InstagramPost> FilterPostUserIsVerifiedAll(List<InstagramPost> postUser)
            {
                if (ModuleSetting.UserFilterModel.IgnoreVerifiedUser)
                    postUser.RemoveAll(x => x.User.IsVerified == true);
                return postUser;
            }

            public bool IsVerifiedUser(InstagramUser user)
            {
                return user.IsVerified;
            }

            public bool IsVerifiedIgnoreUser(InstagramUser user)
            {
                return !user.IsVerified;
            }

            #endregion

            #region Filter for blacklist users
            public bool FilterBlacklistedUsers(InstagramUser user, IEnumerable<BlacklistedUser> privateBlacklist, IEnumerable<BlacklistedUser> groupBlacklist)
            {
                if (UserFilter.RestrictedProfilelist.FilterBlacklist)
                    return IsProfileBlackList(privateBlacklist, user);
                if (UserFilter.RestrictedGrouplist.FilterBlacklist)
                    return IsGroupBlackList(groupBlacklist, user);
                return false;
            }
            public List<InstagramUser> FilterAllBlackListedUser(List<InstagramUser> user, IEnumerable<BlacklistedUser> privateBlacklist, IEnumerable<BlacklistedUser> groupBlacklist)
            {
                if (UserFilter.RestrictedProfilelist.FilterBlacklist)
                    return IsAllProfileBlackList(privateBlacklist, user);
                if (UserFilter.RestrictedGrouplist.FilterBlacklist)
                    return IsAllGroupBlackList(groupBlacklist, user);
                return user;
            }

            public List<InstagramPost> FilterAllPostUserBlackListed(List<InstagramPost> postUser, IEnumerable<BlacklistedUser> privateBlacklist, IEnumerable<BlacklistedUser> groupBlacklist)
            {
                if (UserFilter.RestrictedProfilelist.FilterBlacklist)
                    return IsAllPostUserProfileBlackList(privateBlacklist, postUser);
                if (UserFilter.RestrictedGrouplist.FilterBlacklist)
                    return IsAllPostUserGroupBlackList(groupBlacklist, postUser);
                return postUser;
            }

            private bool IsProfileBlackList(IEnumerable<BlacklistedUser> privateBlacklist, InstagramUser x)
            {
                return privateBlacklist.Any(y =>
                {
                    if (y.Pk == null)
                        return y.Username == x.Username;
                    return y.Pk == x.Pk;
                });
            }
            private List<InstagramUser> IsAllProfileBlackList(IEnumerable<BlacklistedUser> privateBlacklist, List<InstagramUser> user)
            {
                user.RemoveAll(x => privateBlacklist.Any(y =>
                {
                    if (y.Pk == null)
                        return y.Username == x.Username;
                    return y.Pk == x.Pk;
                }));
                return user;
            }
            private List<InstagramPost> IsAllPostUserProfileBlackList(IEnumerable<BlacklistedUser> privateBlacklist, List<InstagramPost> postUser)
            {
                postUser.RemoveAll(x => privateBlacklist.Any(y =>
                {
                    if (y.Pk == null)
                        return y.Username == x.User.Username;
                    return y.Pk == x.User.Pk;
                }));
                return postUser;
            }
            private bool IsGroupBlackList(IEnumerable<BlacklistedUser> groupBlacklist, InstagramUser x)
            {
                return groupBlacklist.Any(y =>
                {
                    if (y.Pk == null)
                        return y.Username == x.Username;
                    return y.Pk == x.Pk;
                });
            }
            private List<InstagramUser> IsAllGroupBlackList(IEnumerable<BlacklistedUser> groupBlacklist, List<InstagramUser> user)
            {
                user.RemoveAll(x => groupBlacklist.Any(y =>
                {
                    if (y.Pk == null)
                        return y.Username == x.Username;
                    return y.Pk == x.Pk;
                }));
                return user;
            }

            private List<InstagramPost> IsAllPostUserGroupBlackList(IEnumerable<BlacklistedUser> groupBlacklist, List<InstagramPost> postUser)
            {
                postUser.RemoveAll(x => groupBlacklist.Any(y =>
                {
                    if (y.Pk == null)
                        return y.Username == x.User.Username;
                    return y.Pk == x.User.Pk;
                }));
                return postUser;
            }

            #endregion

            #region Fitler with follower count is in range
            public void FilterFollowers(List<InstagramUser> users)
            {
                if (UserFilter.FilterFollowersCount)
                {
                    for (int index = DetailedInfo.Count - 1; index >= 0; --index)
                    {
                        UsernameInfoIgResponseHandler usernameInfoIgResponseHandler = DetailedInfo[index];
                        if (!IsFollowerCountIsInRange(usernameInfoIgResponseHandler))
                        {
                            users.Remove(usernameInfoIgResponseHandler);
                            DetailedInfo.RemoveAt(index);
                        }
                    }
                }
            }

            public List<InstagramUser> FilterAllFollowers(List<InstagramUser> users)
            {
                if (UserFilter.FilterFollowersCount)
                    users.RemoveAll(x => !UserFilter.FollowersCount.InRange(x.UserDetails.FollowerCount));
                return users;
            }

            public List<InstagramPost> FilterAllPostFollowers(List<InstagramPost> users)
            {
                if (UserFilter.FilterFollowersCount)
                    users.RemoveAll(x => !UserFilter.FollowersCount.InRange(x.User.UserDetails.FollowerCount));
                return users;
            }
            public bool FilterFollowers(UsernameInfoIgResponseHandler user)
            {
                if (!UserFilter.FilterFollowersCount)
                {
                    return false;
                }
                return IsFollowerCountIsInRange(user);
            }
            public bool FilterFollowers(InstagramUserDetails user)
            {
                if (!UserFilter.FilterFollowersCount)
                    return false;

                return IsFollowerCountIsInRange(user);
            }
            private bool IsFollowerCountIsInRange(UsernameInfoIgResponseHandler UsernameInfoIgResponseHandler)
            {
                return !UserFilter.FollowersCount.InRange(UsernameInfoIgResponseHandler.FollowerCount);
            }
            private bool IsFollowerCountIsInRange(InstagramUserDetails User)
            {
                return !UserFilter.FollowersCount.InRange(User.FollowerCount);
            }
            #endregion

            #region Filter with following counts is in range

            public bool FilterFollowings(UsernameInfoIgResponseHandler UsernameInfoIgResponseHandler)
            {
                if (!UserFilter.FilterFollowingsCount)
                    return false;
                return IsFollowingCountsIsInRange(UsernameInfoIgResponseHandler);
            }

            public bool FilterFollowings(InstagramUserDetails user)
            {
                if (!UserFilter.FilterFollowingsCount)
                    return false;
                return IsFollowingCountsIsInRange(user);
            }
            public List<InstagramUser> FilterAllFollowings(List<InstagramUser> users)
            {
                if (UserFilter.FilterFollowingsCount)
                    users.RemoveAll(x => !UserFilter.FollowingsCount.InRange(x.UserDetails.FollowingCount));
                return users;
            }
            public List<InstagramPost> FilterAllPostFollowings(List<InstagramPost> postUsers)
            {
                if (UserFilter.FilterFollowingsCount)
                    postUsers.RemoveAll(x => !UserFilter.FollowingsCount.InRange(x.User.UserDetails.FollowingCount));
                return postUsers;
            }
            private bool IsFollowingCountsIsInRange(UsernameInfoIgResponseHandler UsernameInfoIgResponseHandler)
            {
                return !UserFilter.FollowingsCount.InRange(UsernameInfoIgResponseHandler.FollowingCount);
            }

            private bool IsFollowingCountsIsInRange(InstagramUserDetails user)
            {
                return !UserFilter.FollowingsCount.InRange(user.FollowingCount);
            }
            #endregion

            #region Filter with user gender

            public bool FilterGender(InstagramUser user)
            {
                if (!UserFilter.GenderFilters.IsFilterByGender)
                    return false;
                return IsUserGenderIgnored(user);
            }


            public bool IsUserGenderIgnored(InstagramUser user)
            {
                if (UserFilter.GenderFilters.IgnoreMalesUser && user.Gender == InstagramGender.Male)
                    return true;
                if (UserFilter.GenderFilters.IgnoreFemalesUser && user.Gender == InstagramGender.Female)
                    return true;
                if (UserFilter.GenderFilters.IgnoreOthersUser && user.Gender == InstagramGender.Unknown)
                    return true;
                return false;
            }

            public List<InstagramUser> FilterAllUserGender(List<InstagramUser> user)
            {
                if (UserFilter.GenderFilters.IsFilterByGender)
                    IsAllUserGenderIgnored(user);
                return user;
            }
            public List<InstagramUser> IsAllUserGenderIgnored(List<InstagramUser> user)
            {
                if (UserFilter.GenderFilters.IgnoreMalesUser)
                {
                    user.RemoveAll(x => x.Gender == InstagramGender.Male);
                    return user;
                }
                if (UserFilter.GenderFilters.IgnoreFemalesUser)
                {
                    user.RemoveAll(x => x.Gender == InstagramGender.Female);
                    return user;
                }
                if (UserFilter.GenderFilters.IgnoreOthersUser)
                {
                    user.RemoveAll(x => x.Gender == InstagramGender.Unknown);
                    return user;
                }
                return user;
            }

            public List<InstagramPost> IsAllPostUserGenderIgnored(List<InstagramPost> postUser)
            {
                if (UserFilter.GenderFilters.IgnoreMalesUser)
                {
                    postUser.RemoveAll(x => x.User.Gender == InstagramGender.Male);
                    return postUser;
                }
                if (UserFilter.GenderFilters.IgnoreFemalesUser)
                {
                    postUser.RemoveAll(x => x.User.Gender == InstagramGender.Female);
                    return postUser;
                }
                if (UserFilter.GenderFilters.IgnoreOthersUser)
                {
                    postUser.RemoveAll(x => x.User.Gender == InstagramGender.Unknown);
                    return postUser;
                }
                return postUser;
            }

            public List<InstagramPost> FilterAllPostUserGender(List<InstagramPost> postUser)
            {
                if (UserFilter.GenderFilters.IsFilterByGender)
                    IsAllPostUserGenderIgnored(postUser);
                return postUser;
            }
            #endregion

            #region Filter for inactive users
            //public void FilterInactiveUsers(List<UserInfoWithFeed> userInfoWithFeeds)
            //{
            //    if (!UnfollowSettings.filterInactiveUsers)
            //        return;
            //    for (int index = userInfoWithFeeds.Count - 1; index >= 0; --index)
            //    {
            //        UserInfoWithFeed userInfoWithFeed = userInfoWithFeeds[index];
            //        if (CheckIfInactiveUser(userInfoWithFeed))
            //            userInfoWithFeeds.RemoveAt(index);
            //    }
            //}
            //public bool FilterInactiveUsers(UserInfoWithFeed userInfoWithFeeds)
            //{
            //    if (!UnfollowSettings.filterInactiveUsers)
            //        return false;
            //    return CheckIfInactiveUser(userInfoWithFeeds);
            //}

            //private bool CheckIfInactiveUser(UserInfoWithFeed userInfoWithFeed)
            //{
            //    if (!UnfollowSettings.filterInactiveUsers)
            //        return false;
            //    return userInfoWithFeed.Feed.Count != 0 && (DateTime.UtcNow - userInfoWithFeed.Feed[0].TakenAt.EpochToDateTimeUtc()).TotalDays <= UnfollowSettings.MaxDaysSinceLastPost;
            //}
            #endregion

            #region FilterMaxDaysSinceLastPost
            //public void FilterMaxDaysSinceLastPost(List<UserInfoWithFeed> userInfoWithFeeds)
            //{
            //    if (!UserFilter.FilterPostedInRecentDays)
            //        return;
            //    for (int index = userInfoWithFeeds.Count - 1; index >= 0; --index)
            //    {
            //        UserInfoWithFeed userInfoWithFeed = userInfoWithFeeds[index];
            //        if (CheckForMaxDaysSinceLastPosted(userInfoWithFeed))
            //            userInfoWithFeeds.RemoveAt(index);
            //    }
            //}


            public bool FilterMaxDaysSinceLastPost(UserInfoWithFeed userInfoWithFeeds)
            {
                if (!UserFilter.FilterPostedInRecentDays)
                    return false;
                return CheckForMaxDaysSinceLastPosted(userInfoWithFeeds);
            }


            private bool CheckForMaxDaysSinceLastPosted(UserInfoWithFeed userInfoWithFeed)
            {
                return userInfoWithFeed.Feed.Count == 0 || (DateTime.UtcNow - userInfoWithFeed.Feed.FirstOrDefault().TakenAt.EpochToDateTimeUtc()).TotalDays > UserFilter.PostedInRecentDays;
            }
            #endregion

            #region FilterNotPostedWithinSomeDays
            //public void FilterNotPostedWithinSomeDays(List<UserInfoWithFeed> userInfoWithFeeds)
            //{
            //    if (!UserFilter.FilterNotPostedInRecentdDays)
            //        return;
            //    for (int index = userInfoWithFeeds.Count - 1; index >= 0; --index)
            //    {
            //        UserInfoWithFeed userInfoWithFeed = userInfoWithFeeds[index];
            //        if (CheckForNotPostedWithinSomeDays(userInfoWithFeed))
            //            userInfoWithFeeds.RemoveAt(index);
            //    }
            //}


            public bool FilterNotPostedWithinSomeDays(UserInfoWithFeed userInfoWithFeeds)
            {
                if (!UserFilter.FilterNotPostedInRecentdDays)
                    return false;
                return CheckForNotPostedWithinSomeDays(userInfoWithFeeds);
            }


            private bool CheckForNotPostedWithinSomeDays(UserInfoWithFeed userInfoWithFeed)
            {
                return userInfoWithFeed.Feed.Count == 0 || (DateTime.UtcNow - userInfoWithFeed.Feed[0].TakenAt.EpochToDateTimeUtc()).TotalDays < UserFilter.NotPostedInRecentDays;
            }
            #endregion

            #region FilterPostCaptionWithSpecificWords
            public bool FilterPostCaptionWithSpecificWords(UserInfoWithFeed userInfoWithFeeds)
            {
                if (!UserFilter.FilterInvaildWord)
                    return false;
                return IsPostCaptionNotHavingSpecificWords(userInfoWithFeeds);
            }


            private bool IsPostCaptionNotHavingSpecificWords(UserInfoWithFeed userInfoWithFeed)
            {
                foreach (var postCaption in userInfoWithFeed.Feed.Select(x => x.Caption))
                {
                    try
                    {
                        foreach (var eachGivenCaption in ModuleSetting.UserFilterModel.LstPostCaption)
                        {
                            if (postCaption.Contains(eachGivenCaption))
                            {
                                return false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
                return true;
            }
            #endregion

            #region Fiter follower/following ratio  is lesser than maximum follow ratio set for filter
            /// <summary>
            /// Fiter follower/following ratio  is lesser than maximum follow ratio set for filter
            /// </summary>
            /// <param name="users"></param>

            public bool FilterMaxFollowRatio(UsernameInfoIgResponseHandler UsernameInfoIgResponseHandler)
            {
                if (!UserFilter.FilterMaximumFollowRatio)
                    return false;
                return !CheckForMaximumFollowRatio(UsernameInfoIgResponseHandler);
            }
            public bool FilterMaxFollowRatio(InstagramUserDetails user)
            {
                if (!UserFilter.FilterMaximumFollowRatio)
                    return false;
                return !CheckForMaximumFollowRatio(user);
            }
            public List<InstagramUser> FilterAllMaxFollowRatio(List<InstagramUser> User)
            {
                if (UserFilter.FilterMaximumFollowRatio)
                    User.RemoveAll(x => x.UserDetails.FollowingCount != 0 && !(x.UserDetails.FollowerCount / (double)x.UserDetails.FollowingCount > UserFilter.MaximumFollowRatio));
                return User;
            }
            public List<InstagramPost> FilterAllMaxPostFollowRatio(List<InstagramPost> postUser)
            {
                if (UserFilter.FilterMaximumFollowRatio)
                    postUser.RemoveAll(x => x.User.UserDetails.FollowingCount != 0 && !(x.User.UserDetails.FollowerCount / (double)x.User.UserDetails.FollowingCount > UserFilter.MaximumFollowRatio));
                return postUser;
            }
            /// <summary>
            /// check if follower/following ratio  is lesser than maximum follow ratio set for filter
            /// </summary>
            /// <param name="UsernameInfoIgResponseHandler"></param>
            /// <returns></returns>
            private bool CheckForMaximumFollowRatio(UsernameInfoIgResponseHandler UsernameInfoIgResponseHandler)
            {
                // var s= UsernameInfoIgResponseHandler.FollowingCount != 0 && UsernameInfoIgResponseHandler.FollowerCount / (double)UsernameInfoIgResponseHandler.FollowingCount > UserFilter.MaximumFollowRatio; 
                return UsernameInfoIgResponseHandler.FollowingCount != 0 && UsernameInfoIgResponseHandler.FollowerCount / (double)UsernameInfoIgResponseHandler.FollowingCount > UserFilter.MaximumFollowRatio;
            }
            private bool CheckForMaximumFollowRatio(InstagramUserDetails user)
            {
                // var s= UsernameInfoIgResponseHandler.FollowingCount != 0 && UsernameInfoIgResponseHandler.FollowerCount / (double)UsernameInfoIgResponseHandler.FollowingCount > UserFilter.MaximumFollowRatio; 
                return user.FollowingCount != 0 && user.FollowerCount / (double)user.FollowingCount > UserFilter.MaximumFollowRatio;
            }
            #endregion

            #region Filter for minimum follower/following ratio
            /// <summary>
            /// check if follower/following ratio  is greater than minimum follow ratio set for filter
            ///filter for list of users
            /// </summary>
            /// <param name="users"></param>

            public bool FilterMinFollowRatio(UsernameInfoIgResponseHandler UsernameInfoIgResponseHandler)
            {
                if (!UserFilter.FilterMinimumFollowRatio)
                    return false;
                return !CheckForMinimumFollowRatio(UsernameInfoIgResponseHandler);
            }
            public bool FilterMinFollowRatio(InstagramUserDetails user)
            {
                if (!UserFilter.FilterMinimumFollowRatio)
                    return false;
                return !CheckForMinimumFollowRatio(user);
            }
            public List<InstagramUser> FilterAllMinFollowRatio(List<InstagramUser> Users)
            {
                //var check = Users[0].UserDetails.FollowerCount / (double)Users[0].UserDetails.FollowingCount < UserFilter.MinimumFollowRatio;
                if (UserFilter.FilterMinimumFollowRatio)
                    Users.RemoveAll(x => x.UserDetails.FollowingCount != 0 && !(x.UserDetails.FollowerCount / (double)x.UserDetails.FollowingCount < UserFilter.MinimumFollowRatio));
                return Users;
            }

            public List<InstagramPost> FilterAllMinPostFollowRatio(List<InstagramPost> postUsers)
            {
                // var check = postUsers[0].User.UserDetails.FollowerCount / (double)postUsers[0].User.UserDetails.FollowingCount < UserFilter.MinimumFollowRatio;
                if (UserFilter.FilterMinimumFollowRatio)
                    postUsers.RemoveAll(x => x.User.UserDetails.FollowingCount != 0 && !(x.User.UserDetails.FollowerCount / (double)x.User.UserDetails.FollowingCount < UserFilter.MinimumFollowRatio));
                return postUsers;
            }
            private bool CheckForMinimumFollowRatio(UsernameInfoIgResponseHandler usernameInfoIgResponseHandler)
            {
                // var check=  usernameInfoIgResponseHandler.FollowingCount != 0 && usernameInfoIgResponseHandler.FollowerCount / (double)usernameInfoIgResponseHandler.FollowingCount < UserFilter.MinimumFollowRatio;
                return usernameInfoIgResponseHandler.FollowingCount != 0 && usernameInfoIgResponseHandler.FollowerCount / (double)usernameInfoIgResponseHandler.FollowingCount < UserFilter.MinimumFollowRatio;
            }
            private bool CheckForMinimumFollowRatio(InstagramUserDetails user)
            {
                // var check=  usernameInfoIgResponseHandler.FollowingCount != 0 && usernameInfoIgResponseHandler.FollowerCount / (double)usernameInfoIgResponseHandler.FollowingCount < UserFilter.MinimumFollowRatio;
                return user.FollowingCount != 0 && user.FollowerCount / (double)user.FollowingCount < UserFilter.MinimumFollowRatio;
            }
            #endregion

            #region Fiter follower/following ratio  is in range follow ratio range set for filter
            /// <summary>
            /// Fiter follower/following ratio  is lesser than maximum follow ratio set for filter
            /// </summary>
            /// <param name="users"></param>

            public bool FilterFollowRatioRange(UsernameInfoIgResponseHandler UsernameInfoIgResponseHandler)
            {
                if (!UserFilter.FilterSpecificFollowRatio)
                    return false;
                return CheckForRangeFollowRatio(UsernameInfoIgResponseHandler);
            }
            public bool FilterFollowRatioRange(InstagramUserDetails user)
            {
                if (!UserFilter.FilterSpecificFollowRatio)
                    return false;
                return CheckForRangeFollowRatio(user);
            }
            public List<InstagramUser> FilterAllFollowRatioRange(List<InstagramUser> Users)
            {
                if (UserFilter.FilterSpecificFollowRatio)
                {
                    Users.RemoveAll(x => x.UserDetails.FollowingCount != 0 && (x.UserDetails.FollowerCount / (double)x.UserDetails.FollowingCount < ModuleSetting.UserFilterModel.SpecificFollowRatio.StartValue
                    || x.UserDetails.FollowerCount / (double)x.UserDetails.FollowingCount < ModuleSetting.UserFilterModel.SpecificFollowRatio.EndValue));
                }
                return Users;
            }

            public List<InstagramPost> FilterAllPostFollowRatioRange(List<InstagramPost> postUsers)
            {
                if (UserFilter.FilterSpecificFollowRatio)
                {
                    postUsers.RemoveAll(x => x.User.UserDetails.FollowingCount != 0 && (x.User.UserDetails.FollowerCount / (double)x.User.UserDetails.FollowingCount < ModuleSetting.UserFilterModel.SpecificFollowRatio.StartValue
                    || x.User.UserDetails.FollowerCount / (double)x.User.UserDetails.FollowingCount < ModuleSetting.UserFilterModel.SpecificFollowRatio.EndValue));
                }
                return postUsers;
            }
            /// <summary>
            /// check if follower/following ratio  is lesser than maximum follow ratio set for filter
            /// </summary>
            /// <param name="UsernameInfoIgResponseHandler"></param>
            /// <returns></returns>
            private bool CheckForRangeFollowRatio(UsernameInfoIgResponseHandler UsernameInfoIgResponseHandler)
            {
                double followRatio = UsernameInfoIgResponseHandler.FollowerCount /
                    (double)UsernameInfoIgResponseHandler.FollowingCount;
                return UsernameInfoIgResponseHandler.FollowingCount != 0 && (followRatio < ModuleSetting.UserFilterModel.SpecificFollowRatio.StartValue || followRatio > ModuleSetting.UserFilterModel.SpecificFollowRatio.EndValue);
            }
            private bool CheckForRangeFollowRatio(InstagramUserDetails user)
            {
                double followRatio = user.FollowerCount / (double)user.FollowingCount;
                return user.FollowingCount != 0 && (followRatio < ModuleSetting.UserFilterModel.SpecificFollowRatio.StartValue || followRatio > ModuleSetting.UserFilterModel.SpecificFollowRatio.EndValue);
            }
            #endregion

            #region Filter Post Count
            /// <summary>
            /// Filter post Count
            /// </summary>
            /// <param name="users"></param>
            public List<InstagramUser> FilterPosts(List<InstagramUser> users)
            {
                if (UserFilter.FilterPostCounts)
                    users.RemoveAll(x => !UserFilter.PostCounts.InRange(x.UserDetails.MediaCount));

                return users;
            }
            public List<InstagramPost> FilterPostUserPosts(List<InstagramPost> postUsers)
            {
                if (UserFilter.FilterPostCounts)
                    postUsers.RemoveAll(x => !UserFilter.PostCounts.InRange(x.User.UserDetails.MediaCount));

                return postUsers;
            }
            public bool FilterPosts(UsernameInfoIgResponseHandler usernameInfoIgResponseHandler)
            {
                if (UserFilter.FilterPostCounts)
                    return !UserFilter.PostCounts.InRange(usernameInfoIgResponseHandler.MediaCount);
                else
                    return IsPostCountInRange(usernameInfoIgResponseHandler);
            }
            public bool FilterPosts(InstagramUserDetails user)
            {
                if (UserFilter.FilterPostCounts)
                    return !UserFilter.PostCounts.InRange(user.MediaCount);
                else
                    return IsPostCountInRange(user);
            }

            /// <summary>
            /// Check if post count is in range
            /// </summary>
            /// <param name="UsernameInfoIgResponseHandler"></param>
            /// <returns></returns>
            private bool IsPostCountInRange(UsernameInfoIgResponseHandler usernameInfoIgResponseHandler)
            {
                if (UserFilter.FilterPostCounts)
                    return UserFilter.PostCounts.InRange(usernameInfoIgResponseHandler.MediaCount);
                else
                    return false;
            }
            private bool IsPostCountInRange(InstagramUserDetails user)
            {
                if (UserFilter.FilterPostCounts)
                    return UserFilter.PostCounts.InRange(user.MediaCount);
                else
                    return false;
            }
            #endregion

            #region Filter Private Users
            /// <summary>
            /// Filter Private Users
            /// </summary>
            /// <param name="users"></param>

            public bool FilterPrivateUsers(InstagramUser user)
            {
                if (!ModuleSetting.UserFilterModel.IgnorePrivateUser)
                    return false;
                return IsPrivateUser(user);
            }
            public List<InstagramUser> FilterAllPrivateUser(List<InstagramUser> instagramUser)
            {
                if (ModuleSetting.UserFilterModel.IgnorePrivateUser)
                {
                    try
                    {
                        instagramUser.RemoveAll(x =>x.UserDetails.IsPrivate);
                    }
                    catch {
                        try
                        {
                            instagramUser.RemoveAll(x => x.IsPrivate);
                        }
                        catch { }
                    }
                    
                }
                return instagramUser;
            }
            public List<InstagramPost> FilterAllPostUserPrivate(List<InstagramPost> postUser)
            {
                if (ModuleSetting.UserFilterModel.IgnorePrivateUser)
                    postUser.RemoveAll(x => x.User.IsPrivate == true);
                return postUser;
            }
            public bool IsPrivateUser(InstagramUser x)
            {
                return x.IsPrivate || x.UserDetails.IsPrivate;
            }
            #endregion

            #region FilterProfileUser
            public bool FilterProfileUsers(InstagramUser x)
            {
                if (!UserFilter.IgnoreNoProfilePicUsers)
                    return false;
                return IsAnonymousProfilePicture(x);
            }

            //overloading method 
            public bool FilterProfileUsers(InstagramUserDetails x)
            {
                if (!UserFilter.IgnoreNoProfilePicUsers)
                    return false;
                return IsAnonymousProfilePicture(x);
            }

            public List<InstagramUser> FilterAllProfileUsers(List<InstagramUser> users)
            {
                if (UserFilter.IgnoreNoProfilePicUsers)
                    users.RemoveAll(x => IsAnonymousProfilePicture(x));
                return users;
            }

            public List<InstagramPost> FilterAllPostuserProfile(List<InstagramPost> postUsers)
            {
                if (UserFilter.IgnoreNoProfilePicUsers)
                    postUsers.RemoveAll(x => x.User.HasAnonymousProfilePicture.GetValueOrDefault() && x.User.HasAnonymousProfilePicture.HasValue);
                return postUsers;
            }
            #endregion

            #region IsAnonymousProfilePicture
            public bool IsAnonymousProfilePicture(InstagramUser x)
            {
                if (x.ProfilePicUrl != null && x.ProfilePicUrl.Contains("44884218_345707102882519_2446069589734326272_n.jpg"))
                    x.HasAnonymousProfilePicture = true;
                else
                    x.HasAnonymousProfilePicture = false;
                if (!x.HasAnonymousProfilePicture.GetValueOrDefault())
                    return false;
                return x.HasAnonymousProfilePicture.HasValue;
            }

            //overloading method 
            public bool IsAnonymousProfilePicture(InstagramUserDetails x)
            {
                if (!x.HasAnonymousProfilePicture.GetValueOrDefault())
                    return false;
                return x.HasAnonymousProfilePicture.HasValue;
            }
            #endregion

            #region check if uer info details is required to scrape for filteration for list of users          
            /// <summary>
            /// Check if user info details is required to scrape for filteration
            /// For perticular user
            /// </summary>
            /// <param name="users"></param>
            /// <returns></returns>
            public bool ShouldGetDetailedInfo()
            {
                return UserFilter.FilterFollowersCount || UserFilter.FilterFollowingsCount ||
                       UserFilter.IgnoreNonEnglishUser || UserFilter.FilterPostCounts ||
                       UserFilter.FilterBioRestrictedWords || UserFilter.FilterMinimumCharacterInBio ||
                       UserFilter.FilterMaximumFollowRatio ||
                       UserFilter.FilterInvaildWord || UserFilter.FilterMinimumFollowRatio ||
                       UserFilter.IgnoreNoProfilePicUsers || UserFilter.FilterPostedInRecentDays ||
                       UserFilter.FilterNotPostedInRecentdDays || UserFilter.FilterBioNotRestrictedWords||ModuleSetting.FollowOnlyBusinessAccounts;
            }

            #endregion

            #region Filter Business Users
            /// <summary>
            /// Filter Business Users
            /// </summary>
            /// <param name="users"></param>

            public bool FilterBusinessUsers(InstagramUser user)
            {
                if (!ModuleSetting.UserFilterModel.IgnoreBusinessUser)
                    return false;
                return IsBusinessUser(user);
            }

            public List<InstagramUser> FilterAllBusinessUsers(List<InstagramUser> user)
            {
                if (ModuleSetting.UserFilterModel.IgnoreBusinessUser)
                    user.RemoveAll(x => x.IsBusiness == true);
                return user;
            }
            public List<InstagramPost> FilterAllPostUserBusiness(List<InstagramPost> postUser)
            {
                if (ModuleSetting.UserFilterModel.IgnoreBusinessUser)
                    postUser.RemoveAll(x => x.User.IsBusiness == true);
                return postUser;
            }
            private bool IsBusinessUser(InstagramUser x)
            {
                return x.IsBusiness;
            }
            #endregion

            #region Filter Non-English Users
            /// <summary>
            /// Filter Non-English Users
            /// </summary>
            /// <param name="users"></param>

            public bool FilterNonEnglishUsers(UsernameInfoIgResponseHandler userInfo)
            {
                if (!ModuleSetting.UserFilterModel.IgnoreNonEnglishUser)
                    return false;

                if (!string.IsNullOrEmpty(userInfo.Biography))
                    return IsNonEnglishUser(userInfo);

                return true;
            }
            public bool FilterNonEnglishUsers(InstagramUserDetails userInfo)
            {
                if (!ModuleSetting.UserFilterModel.IgnoreNonEnglishUser)
                    return false;

                if (!string.IsNullOrEmpty(userInfo.Biography))
                    return IsNonEnglishUser(userInfo);

                return true;
            }
            private bool IsNonEnglishUser(UsernameInfoIgResponseHandler userInfo)
            {
                // Removes Alphabets, Letters and white spaces from a text string
                string filterBiography = Regex.Replace(userInfo.Biography, @"[A-Za-z0-9\s+]", "");
                // Removes Emoji's from a text string
                filterBiography = Regex.Replace(filterBiography, @"\p{Cs}", "");

                #region Removes special characters from a text string
                string[] chars = new[] { "\\n", "?", ",", ".", "/", "!", "@", "#", "$", "%", "^", "&", "*", "'", "\"", ";", "_", "-", "(", ")", ":", "|", "[", "]", "♡", "•" };
                foreach (var character in chars)
                {
                    if (filterBiography.Contains(character))
                    {
                        filterBiography = filterBiography.Replace(character, "");
                    }
                }
                #endregion

                #region Commented (Detects 3518 Emoji characters)
                //var EmojiPattern = @"(?:\uD83D(?:[\uDC76\uDC66\uDC67](?:\uD83C[\uDFFB-\uDFFF])?|\uDC68(?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92]|\u2708\uFE0F?|\uD83E[\uDDB0-\uDDB3]))?)|\u200D(?:\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D(?:\uDC69\u200D\uD83D(?:\uDC66(?:\u200D\uD83D\uDC66)?|\uDC67(?:\u200D\uD83D[\uDC66\uDC67])?)|\uDC68\u200D\uD83D(?:\uDC66(?:\u200D\uD83D\uDC66)?|\uDC67(?:\u200D\uD83D[\uDC66\uDC67])?)|\uDC66(?:\u200D\uD83D\uDC66)?|\uDC67(?:\u200D\uD83D[\uDC66\uDC67])?|[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92])|\u2708\uFE0F?|\uD83E[\uDDB0-\uDDB3]|\u2764(?:\uFE0F\u200D\uD83D(?:\uDC8B\u200D\uD83D\uDC68|\uDC68)|\u200D\uD83D(?:\uDC8B\u200D\uD83D\uDC68|\uDC68)))))?|\uDC69(?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92]|\u2708\uFE0F?|\uD83E[\uDDB0-\uDDB3]))?)|\u200D(?:\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D(?:\uDC69\u200D\uD83D(?:\uDC66(?:\u200D\uD83D\uDC66)?|\uDC67(?:\u200D\uD83D[\uDC66\uDC67])?)|\uDC66(?:\u200D\uD83D\uDC66)?|\uDC67(?:\u200D\uD83D[\uDC66\uDC67])?|[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92])|\u2708\uFE0F?|\uD83E[\uDDB0-\uDDB3]|\u2764(?:\uFE0F\u200D\uD83D(?:\uDC8B\u200D\uD83D[\uDC68\uDC69]|[\uDC68\uDC69])|\u200D\uD83D(?:\uDC8B\u200D\uD83D[\uDC68\uDC69]|[\uDC68\uDC69])))))?|[\uDC74\uDC75](?:\uD83C[\uDFFB-\uDFFF])?|\uDC6E(?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|\uDD75(?:(?:\uFE0F(?:\u200D(?:[\u2642\u2640]\uFE0F?))?|\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDC82\uDC77](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|\uDC78(?:\uD83C[\uDFFB-\uDFFF])?|\uDC73(?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|\uDC72(?:\uD83C[\uDFFB-\uDFFF])?|\uDC71(?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDC70\uDC7C](?:\uD83C[\uDFFB-\uDFFF])?|[\uDE4D\uDE4E\uDE45\uDE46\uDC81\uDE4B\uDE47\uDC86\uDC87\uDEB6](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDC83\uDD7A](?:\uD83C[\uDFFB-\uDFFF])?|\uDC6F(?:\u200D(?:[\u2642\u2640]\uFE0F?))?|[\uDEC0\uDECC](?:\uD83C[\uDFFB-\uDFFF])?|\uDD74(?:(?:\uD83C[\uDFFB-\uDFFF]|\uFE0F))?|\uDDE3\uFE0F?|[\uDEA3\uDEB4\uDEB5](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDCAA\uDC48\uDC49\uDC46\uDD95\uDC47\uDD96](?:\uD83C[\uDFFB-\uDFFF])?|\uDD90(?:(?:\uD83C[\uDFFB-\uDFFF]|\uFE0F))?|[\uDC4C-\uDC4E\uDC4A\uDC4B\uDC4F\uDC50\uDE4C\uDE4F\uDC85\uDC42\uDC43](?:\uD83C[\uDFFB-\uDFFF])?|\uDC41(?:(?:\uFE0F(?:\u200D\uD83D\uDDE8\uFE0F?)?|\u200D\uD83D\uDDE8\uFE0F?))?|[\uDDE8\uDDEF\uDD73\uDD76\uDECD\uDC3F\uDD4A\uDD77\uDD78\uDDFA\uDEE3\uDEE4\uDEE2\uDEF3\uDEE5\uDEE9\uDEF0\uDECE\uDD70\uDD79\uDDBC\uDDA5\uDDA8\uDDB1\uDDB2\uDCFD\uDD6F\uDDDE\uDDF3\uDD8B\uDD8A\uDD8C\uDD8D\uDDC2\uDDD2\uDDD3\uDD87\uDDC3\uDDC4\uDDD1\uDDDD\uDEE0\uDDE1\uDEE1\uDDDC\uDECF\uDECB\uDD49]\uFE0F?|[\uDE00-\uDE06\uDE09-\uDE0B\uDE0E\uDE0D\uDE18\uDE17\uDE19\uDE1A\uDE42\uDE10\uDE11\uDE36\uDE44\uDE0F\uDE23\uDE25\uDE2E\uDE2F\uDE2A\uDE2B\uDE34\uDE0C\uDE1B-\uDE1D\uDE12-\uDE15\uDE43\uDE32\uDE41\uDE16\uDE1E\uDE1F\uDE24\uDE22\uDE2D\uDE26-\uDE29\uDE2C\uDE30\uDE31\uDE33\uDE35\uDE21\uDE20\uDE37\uDE07\uDE08\uDC7F\uDC79\uDC7A\uDC80\uDC7B\uDC7D\uDC7E\uDCA9\uDE3A\uDE38\uDE39\uDE3B-\uDE3D\uDE40\uDE3F\uDE3E\uDE48-\uDE4A\uDC64\uDC65\uDC6B-\uDC6D\uDC8F\uDC91\uDC6A\uDC63\uDC40\uDC45\uDC44\uDC8B\uDC98\uDC93-\uDC97\uDC99-\uDC9C\uDDA4\uDC9D-\uDC9F\uDC8C\uDCA4\uDCA2\uDCA3\uDCA5\uDCA6\uDCA8\uDCAB-\uDCAD\uDC53-\uDC62\uDC51\uDC52\uDCFF\uDC84\uDC8D\uDC8E\uDC35\uDC12\uDC36\uDC15\uDC29\uDC3A\uDC31\uDC08\uDC2F\uDC05\uDC06\uDC34\uDC0E\uDC2E\uDC02-\uDC04\uDC37\uDC16\uDC17\uDC3D\uDC0F\uDC11\uDC10\uDC2A\uDC2B\uDC18\uDC2D\uDC01\uDC00\uDC39\uDC30\uDC07\uDC3B\uDC28\uDC3C\uDC3E\uDC14\uDC13\uDC23-\uDC27\uDC38\uDC0A\uDC22\uDC0D\uDC32\uDC09\uDC33\uDC0B\uDC2C\uDC1F-\uDC21\uDC19\uDC1A\uDC0C\uDC1B-\uDC1E\uDC90\uDCAE\uDD2A\uDDFE\uDDFB\uDC92\uDDFC\uDDFD\uDD4C\uDD4D\uDD4B\uDC88\uDE82-\uDE8A\uDE9D\uDE9E\uDE8B-\uDE8E\uDE90-\uDE9C\uDEB2\uDEF4\uDEF9\uDEF5\uDE8F\uDEA8\uDEA5\uDEA6\uDED1\uDEA7\uDEF6\uDEA4\uDEA2\uDEEB\uDEEC\uDCBA\uDE81\uDE9F-\uDEA1\uDE80\uDEF8\uDD5B\uDD67\uDD50\uDD5C\uDD51\uDD5D\uDD52\uDD5E\uDD53\uDD5F\uDD54\uDD60\uDD55\uDD61\uDD56\uDD62\uDD57\uDD63\uDD58\uDD64\uDD59\uDD65\uDD5A\uDD66\uDD25\uDCA7\uDEF7\uDD2E\uDD07-\uDD0A\uDCE2\uDCE3\uDCEF\uDD14\uDD15\uDCFB\uDCF1\uDCF2\uDCDE-\uDCE0\uDD0B\uDD0C\uDCBB\uDCBD-\uDCC0\uDCFA\uDCF7-\uDCF9\uDCFC\uDD0D\uDD0E\uDCA1\uDD26\uDCD4-\uDCDA\uDCD3\uDCD2\uDCC3\uDCDC\uDCC4\uDCF0\uDCD1\uDD16\uDCB0\uDCB4-\uDCB8\uDCB3\uDCB9\uDCB1\uDCB2\uDCE7-\uDCE9\uDCE4-\uDCE6\uDCEB\uDCEA\uDCEC-\uDCEE\uDCDD\uDCBC\uDCC1\uDCC2\uDCC5-\uDCD0\uDD12\uDD13\uDD0F-\uDD11\uDD28\uDD2B\uDD27\uDD29\uDD17\uDD2C\uDD2D\uDCE1\uDC89\uDC8A\uDEAA\uDEBD\uDEBF\uDEC1\uDED2\uDEAC\uDDFF\uDEAE\uDEB0\uDEB9-\uDEBC\uDEBE\uDEC2-\uDEC5\uDEB8\uDEAB\uDEB3\uDEAD\uDEAF\uDEB1\uDEB7\uDCF5\uDD1E\uDD03\uDD04\uDD19-\uDD1D\uDED0\uDD4E\uDD2F\uDD00-\uDD02\uDD3C\uDD3D\uDD05\uDD06\uDCF6\uDCF3\uDCF4\uDD31\uDCDB\uDD30\uDD1F\uDCAF\uDD20-\uDD24\uDD36-\uDD3B\uDCA0\uDD18\uDD32-\uDD35\uDEA9])|\uD83E(?:[\uDDD2\uDDD1\uDDD3](?:\uD83C[\uDFFB-\uDFFF])?|[\uDDB8\uDDB9](?:\u200D(?:[\u2640\u2642]\uFE0F?))?|[\uDD34\uDDD5\uDDD4\uDD35\uDD30\uDD31\uDD36](?:\uD83C[\uDFFB-\uDFFF])?|[\uDDD9-\uDDDD](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2640\u2642]\uFE0F?))?)|\u200D(?:[\u2640\u2642]\uFE0F?)))?|[\uDDDE\uDDDF](?:\u200D(?:[\u2640\u2642]\uFE0F?))?|[\uDD26\uDD37](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDDD6-\uDDD8](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2640\u2642]\uFE0F?))?)|\u200D(?:[\u2640\u2642]\uFE0F?)))?|\uDD38(?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|\uDD3C(?:\u200D(?:[\u2642\u2640]\uFE0F?))?|[\uDD3D\uDD3E\uDD39](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDD33\uDDB5\uDDB6\uDD1E\uDD18\uDD19\uDD1B\uDD1C\uDD1A\uDD1F\uDD32](?:\uD83C[\uDFFB-\uDFFF])?|[\uDD23\uDD70\uDD17\uDD29\uDD14\uDD28\uDD10\uDD24\uDD11\uDD2F\uDD75\uDD76\uDD2A\uDD2C\uDD12\uDD15\uDD22\uDD2E\uDD27\uDD20\uDD21\uDD73\uDD74\uDD7A\uDD25\uDD2B\uDD2D\uDDD0\uDD13\uDD16\uDD3A\uDD1D\uDDB0-\uDDB3\uDDE0\uDDB4\uDDB7\uDDE1\uDD7D\uDD7C\uDDE3-\uDDE6\uDD7E\uDD7F\uDDE2\uDD8D\uDD8A\uDD9D\uDD81\uDD84\uDD93\uDD8C\uDD99\uDD92\uDD8F\uDD9B\uDD94\uDD87\uDD98\uDDA1\uDD83\uDD85\uDD86\uDDA2\uDD89\uDD9A\uDD9C\uDD8E\uDD95\uDD96\uDD88\uDD80\uDD9E\uDD90\uDD91\uDD8B\uDD97\uDD82\uDD9F\uDDA0\uDD40\uDD6D\uDD5D\uDD65\uDD51\uDD54\uDD55\uDD52\uDD6C\uDD66\uDD5C\uDD50\uDD56\uDD68\uDD6F\uDD5E\uDDC0\uDD69\uDD53\uDD6A\uDD59\uDD5A\uDD58\uDD63\uDD57\uDDC2\uDD6B\uDD6E\uDD5F-\uDD61\uDDC1\uDD67\uDD5B\uDD42\uDD43\uDD64\uDD62\uDD44\uDDED\uDDF1\uDDF3\uDDE8\uDDE7\uDD47-\uDD49\uDD4E\uDD4F\uDD4D\uDD4A\uDD4B\uDD45\uDD4C\uDDFF\uDDE9\uDDF8\uDD41\uDDEE\uDDFE\uDDF0\uDDF2\uDDEA-\uDDEC\uDDEF\uDDF4-\uDDF7\uDDF9-\uDDFD])|[\u263A\u2639\u2620]\uFE0F?|\uD83C(?:\uDF85(?:\uD83C[\uDFFB-\uDFFF])?|\uDFC3(?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDFC7\uDFC2](?:\uD83C[\uDFFB-\uDFFF])?|\uDFCC(?:(?:\uFE0F(?:\u200D(?:[\u2642\u2640]\uFE0F?))?|\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDFC4\uDFCA](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|\uDFCB(?:(?:\uFE0F(?:\u200D(?:[\u2642\u2640]\uFE0F?))?|\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDFCE\uDFCD\uDFF5\uDF36\uDF7D\uDFD4-\uDFD6\uDFDC-\uDFDF\uDFDB\uDFD7\uDFD8\uDFDA\uDFD9\uDF21\uDF24-\uDF2C\uDF97\uDF9F\uDF96\uDF99-\uDF9B\uDF9E\uDFF7\uDD70\uDD71\uDD7E\uDD7F\uDE02\uDE37]\uFE0F?|\uDFF4(?:(?:\u200D\u2620\uFE0F?|\uDB40\uDC67\uDB40\uDC62\uDB40(?:\uDC65\uDB40\uDC6E\uDB40\uDC67\uDB40\uDC7F|\uDC73\uDB40\uDC63\uDB40\uDC74\uDB40\uDC7F|\uDC77\uDB40\uDC6C\uDB40\uDC73\uDB40\uDC7F)))?|\uDFF3(?:(?:\uFE0F(?:\u200D\uD83C\uDF08)?|\u200D\uD83C\uDF08))?|\uDDE6\uD83C[\uDDE8-\uDDEC\uDDEE\uDDF1\uDDF2\uDDF4\uDDF6-\uDDFA\uDDFC\uDDFD\uDDFF]|\uDDE7\uD83C[\uDDE6\uDDE7\uDDE9-\uDDEF\uDDF1-\uDDF4\uDDF6-\uDDF9\uDDFB\uDDFC\uDDFE\uDDFF]|\uDDE8\uD83C[\uDDE6\uDDE8\uDDE9\uDDEB-\uDDEE\uDDF0-\uDDF5\uDDF7\uDDFA-\uDDFF]|\uDDE9\uD83C[\uDDEA\uDDEC\uDDEF\uDDF0\uDDF2\uDDF4\uDDFF]|\uDDEA\uD83C[\uDDE6\uDDE8\uDDEA\uDDEC\uDDED\uDDF7-\uDDFA]|\uDDEB\uD83C[\uDDEE-\uDDF0\uDDF2\uDDF4\uDDF7]|\uDDEC\uD83C[\uDDE6\uDDE7\uDDE9-\uDDEE\uDDF1-\uDDF3\uDDF5-\uDDFA\uDDFC\uDDFE]|\uDDED\uD83C[\uDDF0\uDDF2\uDDF3\uDDF7\uDDF9\uDDFA]|\uDDEE\uD83C[\uDDE8-\uDDEA\uDDF1-\uDDF4\uDDF6-\uDDF9]|\uDDEF\uD83C[\uDDEA\uDDF2\uDDF4\uDDF5]|\uDDF0\uD83C[\uDDEA\uDDEC-\uDDEE\uDDF2\uDDF3\uDDF5\uDDF7\uDDFC\uDDFE\uDDFF]|\uDDF1\uD83C[\uDDE6-\uDDE8\uDDEE\uDDF0\uDDF7-\uDDFB\uDDFE]|\uDDF2\uD83C[\uDDE6\uDDE8-\uDDED\uDDF0-\uDDFF]|\uDDF3\uD83C[\uDDE6\uDDE8\uDDEA-\uDDEC\uDDEE\uDDF1\uDDF4\uDDF5\uDDF7\uDDFA\uDDFF]|\uDDF4\uD83C\uDDF2|\uDDF5\uD83C[\uDDE6\uDDEA-\uDDED\uDDF0-\uDDF3\uDDF7-\uDDF9\uDDFC\uDDFE]|\uDDF6\uD83C\uDDE6|\uDDF7\uD83C[\uDDEA\uDDF4\uDDF8\uDDFA\uDDFC]|\uDDF8\uD83C[\uDDE6-\uDDEA\uDDEC-\uDDF4\uDDF7-\uDDF9\uDDFB\uDDFD-\uDDFF]|\uDDF9\uD83C[\uDDE6\uDDE8\uDDE9\uDDEB-\uDDED\uDDEF-\uDDF4\uDDF7\uDDF9\uDDFB\uDDFC\uDDFF]|\uDDFA\uD83C[\uDDE6\uDDEC\uDDF2\uDDF3\uDDF8\uDDFE\uDDFF]|\uDDFB\uD83C[\uDDE6\uDDE8\uDDEA\uDDEC\uDDEE\uDDF3\uDDFA]|\uDDFC\uD83C[\uDDEB\uDDF8]|\uDDFD\uD83C\uDDF0|\uDDFE\uD83C[\uDDEA\uDDF9]|\uDDFF\uD83C[\uDDE6\uDDF2\uDDFC]|[\uDFFB-\uDFFF\uDF92\uDFA9\uDF93\uDF38-\uDF3C\uDF37\uDF31-\uDF35\uDF3E-\uDF43\uDF47-\uDF53\uDF45\uDF46\uDF3D\uDF44\uDF30\uDF5E\uDF56\uDF57\uDF54\uDF5F\uDF55\uDF2D-\uDF2F\uDF73\uDF72\uDF7F\uDF71\uDF58-\uDF5D\uDF60\uDF62-\uDF65\uDF61\uDF66-\uDF6A\uDF82\uDF70\uDF6B-\uDF6F\uDF7C\uDF75\uDF76\uDF7E\uDF77-\uDF7B\uDF74\uDFFA\uDF0D-\uDF10\uDF0B\uDFE0-\uDFE6\uDFE8-\uDFED\uDFEF\uDFF0\uDF01\uDF03-\uDF07\uDF09\uDF0C\uDFA0-\uDFA2\uDFAA\uDF11-\uDF20\uDF00\uDF08\uDF02\uDF0A\uDF83\uDF84\uDF86-\uDF8B\uDF8D-\uDF91\uDF80\uDF81\uDFAB\uDFC6\uDFC5\uDFC0\uDFD0\uDFC8\uDFC9\uDFBE\uDFB3\uDFCF\uDFD1-\uDFD3\uDFF8\uDFA3\uDFBD\uDFBF\uDFAF\uDFB1\uDFAE\uDFB0\uDFB2\uDCCF\uDC04\uDFB4\uDFAD\uDFA8\uDFBC\uDFB5\uDFB6\uDFA4\uDFA7\uDFB7-\uDFBB\uDFA5\uDFAC\uDFEE\uDFF9\uDFE7\uDFA6\uDD8E\uDD91-\uDD9A\uDE01\uDE36\uDE2F\uDE50\uDE39\uDE1A\uDE32\uDE51\uDE38\uDE34\uDE33\uDE3A\uDE35\uDFC1\uDF8C])|\u26F7\uFE0F?|\u26F9(?:(?:\uFE0F(?:\u200D(?:[\u2642\u2640]\uFE0F?))?|\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\u261D\u270C](?:(?:\uD83C[\uDFFB-\uDFFF]|\uFE0F))?|[\u270B\u270A](?:\uD83C[\uDFFB-\uDFFF])?|\u270D(?:(?:\uD83C[\uDFFB-\uDFFF]|\uFE0F))?|[\u2764\u2763\u26D1\u2618\u26F0\u26E9\u2668\u26F4\u2708\u23F1\u23F2\u2600\u2601\u26C8\u2602\u26F1\u2744\u2603\u2604\u26F8\u2660\u2665\u2666\u2663\u260E\u2328\u2709\u270F\u2712\u2702\u26CF\u2692\u2694\u2699\u2696\u26D3\u2697\u26B0\u26B1\u26A0\u2622\u2623\u2B06\u2197\u27A1\u2198\u2B07\u2199\u2B05\u2196\u2195\u2194\u21A9\u21AA\u2934\u2935\u269B\u267E\u2721\u2638\u262F\u271D\u2626\u262A\u262E\u25B6\u23ED\u23EF\u25C0\u23EE\u23F8-\u23FA\u23CF\u2640\u2642\u2695\u267B\u269C\u2611\u2714\u2716\u303D\u2733\u2734\u2747\u203C\u2049\u3030\u00A9\u00AE\u2122]\uFE0F?|[\u0023\u002A\u0030-\u0039](?:\uFE0F\u20E3|\u20E3)|[\u2139\u24C2\u3297\u3299\u25AA\u25AB\u25FB\u25FC]\uFE0F?|[\u2615\u26EA\u26F2\u26FA\u26FD\u2693\u26F5\u231B\u23F3\u231A\u23F0\u2B50\u26C5\u2614\u26A1\u26C4\u2728\u26BD\u26BE\u26F3\u267F\u26D4\u2648-\u2653\u26CE\u23E9-\u23EC\u2B55\u2705\u274C\u274E\u2795-\u2797\u27B0\u27BF\u2753-\u2755\u2757\u25FD\u25FE\u2B1B\u2B1C\u26AA\u26AB])";
                //filterBiography = Regex.Replace(filterBiography, EmojiPattern, ""); 
                #endregion

                // After removing alphabets, letters and emojis 
                // if string is empty then it returns "English" 
                // otherwise "Non-english".
                if ((userInfo.Biography.Length > filterBiography.Length) && (filterBiography == string.Empty))
                    return false;
                else
                    return true;
            }
            private bool IsNonEnglishUser(InstagramUserDetails userInfo)
            {
                // Removes Alphabets, Letters and white spaces from a text string
                string filterBiography = Regex.Replace(userInfo.Biography, @"[A-Za-z0-9\s+]", "");
                // Removes Emoji's from a text string
                filterBiography = Regex.Replace(filterBiography, @"\p{Cs}", "");

                #region Removes special characters from a text string
                string[] chars = new[] { "\\n", "?", ",", ".", "/", "!", "@", "#", "$", "%", "^", "&", "*", "'", "\"", ";", "_", "-", "(", ")", ":", "|", "[", "]", "♡", "•" };
                foreach (var character in chars)
                {
                    if (filterBiography.Contains(character))
                    {
                        filterBiography = filterBiography.Replace(character, "");
                    }
                }
                #endregion

                #region Commented (Detects 3518 Emoji characters)
                //var EmojiPattern = @"(?:\uD83D(?:[\uDC76\uDC66\uDC67](?:\uD83C[\uDFFB-\uDFFF])?|\uDC68(?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92]|\u2708\uFE0F?|\uD83E[\uDDB0-\uDDB3]))?)|\u200D(?:\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D(?:\uDC69\u200D\uD83D(?:\uDC66(?:\u200D\uD83D\uDC66)?|\uDC67(?:\u200D\uD83D[\uDC66\uDC67])?)|\uDC68\u200D\uD83D(?:\uDC66(?:\u200D\uD83D\uDC66)?|\uDC67(?:\u200D\uD83D[\uDC66\uDC67])?)|\uDC66(?:\u200D\uD83D\uDC66)?|\uDC67(?:\u200D\uD83D[\uDC66\uDC67])?|[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92])|\u2708\uFE0F?|\uD83E[\uDDB0-\uDDB3]|\u2764(?:\uFE0F\u200D\uD83D(?:\uDC8B\u200D\uD83D\uDC68|\uDC68)|\u200D\uD83D(?:\uDC8B\u200D\uD83D\uDC68|\uDC68)))))?|\uDC69(?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92]|\u2708\uFE0F?|\uD83E[\uDDB0-\uDDB3]))?)|\u200D(?:\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D(?:\uDC69\u200D\uD83D(?:\uDC66(?:\u200D\uD83D\uDC66)?|\uDC67(?:\u200D\uD83D[\uDC66\uDC67])?)|\uDC66(?:\u200D\uD83D\uDC66)?|\uDC67(?:\u200D\uD83D[\uDC66\uDC67])?|[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92])|\u2708\uFE0F?|\uD83E[\uDDB0-\uDDB3]|\u2764(?:\uFE0F\u200D\uD83D(?:\uDC8B\u200D\uD83D[\uDC68\uDC69]|[\uDC68\uDC69])|\u200D\uD83D(?:\uDC8B\u200D\uD83D[\uDC68\uDC69]|[\uDC68\uDC69])))))?|[\uDC74\uDC75](?:\uD83C[\uDFFB-\uDFFF])?|\uDC6E(?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|\uDD75(?:(?:\uFE0F(?:\u200D(?:[\u2642\u2640]\uFE0F?))?|\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDC82\uDC77](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|\uDC78(?:\uD83C[\uDFFB-\uDFFF])?|\uDC73(?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|\uDC72(?:\uD83C[\uDFFB-\uDFFF])?|\uDC71(?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDC70\uDC7C](?:\uD83C[\uDFFB-\uDFFF])?|[\uDE4D\uDE4E\uDE45\uDE46\uDC81\uDE4B\uDE47\uDC86\uDC87\uDEB6](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDC83\uDD7A](?:\uD83C[\uDFFB-\uDFFF])?|\uDC6F(?:\u200D(?:[\u2642\u2640]\uFE0F?))?|[\uDEC0\uDECC](?:\uD83C[\uDFFB-\uDFFF])?|\uDD74(?:(?:\uD83C[\uDFFB-\uDFFF]|\uFE0F))?|\uDDE3\uFE0F?|[\uDEA3\uDEB4\uDEB5](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDCAA\uDC48\uDC49\uDC46\uDD95\uDC47\uDD96](?:\uD83C[\uDFFB-\uDFFF])?|\uDD90(?:(?:\uD83C[\uDFFB-\uDFFF]|\uFE0F))?|[\uDC4C-\uDC4E\uDC4A\uDC4B\uDC4F\uDC50\uDE4C\uDE4F\uDC85\uDC42\uDC43](?:\uD83C[\uDFFB-\uDFFF])?|\uDC41(?:(?:\uFE0F(?:\u200D\uD83D\uDDE8\uFE0F?)?|\u200D\uD83D\uDDE8\uFE0F?))?|[\uDDE8\uDDEF\uDD73\uDD76\uDECD\uDC3F\uDD4A\uDD77\uDD78\uDDFA\uDEE3\uDEE4\uDEE2\uDEF3\uDEE5\uDEE9\uDEF0\uDECE\uDD70\uDD79\uDDBC\uDDA5\uDDA8\uDDB1\uDDB2\uDCFD\uDD6F\uDDDE\uDDF3\uDD8B\uDD8A\uDD8C\uDD8D\uDDC2\uDDD2\uDDD3\uDD87\uDDC3\uDDC4\uDDD1\uDDDD\uDEE0\uDDE1\uDEE1\uDDDC\uDECF\uDECB\uDD49]\uFE0F?|[\uDE00-\uDE06\uDE09-\uDE0B\uDE0E\uDE0D\uDE18\uDE17\uDE19\uDE1A\uDE42\uDE10\uDE11\uDE36\uDE44\uDE0F\uDE23\uDE25\uDE2E\uDE2F\uDE2A\uDE2B\uDE34\uDE0C\uDE1B-\uDE1D\uDE12-\uDE15\uDE43\uDE32\uDE41\uDE16\uDE1E\uDE1F\uDE24\uDE22\uDE2D\uDE26-\uDE29\uDE2C\uDE30\uDE31\uDE33\uDE35\uDE21\uDE20\uDE37\uDE07\uDE08\uDC7F\uDC79\uDC7A\uDC80\uDC7B\uDC7D\uDC7E\uDCA9\uDE3A\uDE38\uDE39\uDE3B-\uDE3D\uDE40\uDE3F\uDE3E\uDE48-\uDE4A\uDC64\uDC65\uDC6B-\uDC6D\uDC8F\uDC91\uDC6A\uDC63\uDC40\uDC45\uDC44\uDC8B\uDC98\uDC93-\uDC97\uDC99-\uDC9C\uDDA4\uDC9D-\uDC9F\uDC8C\uDCA4\uDCA2\uDCA3\uDCA5\uDCA6\uDCA8\uDCAB-\uDCAD\uDC53-\uDC62\uDC51\uDC52\uDCFF\uDC84\uDC8D\uDC8E\uDC35\uDC12\uDC36\uDC15\uDC29\uDC3A\uDC31\uDC08\uDC2F\uDC05\uDC06\uDC34\uDC0E\uDC2E\uDC02-\uDC04\uDC37\uDC16\uDC17\uDC3D\uDC0F\uDC11\uDC10\uDC2A\uDC2B\uDC18\uDC2D\uDC01\uDC00\uDC39\uDC30\uDC07\uDC3B\uDC28\uDC3C\uDC3E\uDC14\uDC13\uDC23-\uDC27\uDC38\uDC0A\uDC22\uDC0D\uDC32\uDC09\uDC33\uDC0B\uDC2C\uDC1F-\uDC21\uDC19\uDC1A\uDC0C\uDC1B-\uDC1E\uDC90\uDCAE\uDD2A\uDDFE\uDDFB\uDC92\uDDFC\uDDFD\uDD4C\uDD4D\uDD4B\uDC88\uDE82-\uDE8A\uDE9D\uDE9E\uDE8B-\uDE8E\uDE90-\uDE9C\uDEB2\uDEF4\uDEF9\uDEF5\uDE8F\uDEA8\uDEA5\uDEA6\uDED1\uDEA7\uDEF6\uDEA4\uDEA2\uDEEB\uDEEC\uDCBA\uDE81\uDE9F-\uDEA1\uDE80\uDEF8\uDD5B\uDD67\uDD50\uDD5C\uDD51\uDD5D\uDD52\uDD5E\uDD53\uDD5F\uDD54\uDD60\uDD55\uDD61\uDD56\uDD62\uDD57\uDD63\uDD58\uDD64\uDD59\uDD65\uDD5A\uDD66\uDD25\uDCA7\uDEF7\uDD2E\uDD07-\uDD0A\uDCE2\uDCE3\uDCEF\uDD14\uDD15\uDCFB\uDCF1\uDCF2\uDCDE-\uDCE0\uDD0B\uDD0C\uDCBB\uDCBD-\uDCC0\uDCFA\uDCF7-\uDCF9\uDCFC\uDD0D\uDD0E\uDCA1\uDD26\uDCD4-\uDCDA\uDCD3\uDCD2\uDCC3\uDCDC\uDCC4\uDCF0\uDCD1\uDD16\uDCB0\uDCB4-\uDCB8\uDCB3\uDCB9\uDCB1\uDCB2\uDCE7-\uDCE9\uDCE4-\uDCE6\uDCEB\uDCEA\uDCEC-\uDCEE\uDCDD\uDCBC\uDCC1\uDCC2\uDCC5-\uDCD0\uDD12\uDD13\uDD0F-\uDD11\uDD28\uDD2B\uDD27\uDD29\uDD17\uDD2C\uDD2D\uDCE1\uDC89\uDC8A\uDEAA\uDEBD\uDEBF\uDEC1\uDED2\uDEAC\uDDFF\uDEAE\uDEB0\uDEB9-\uDEBC\uDEBE\uDEC2-\uDEC5\uDEB8\uDEAB\uDEB3\uDEAD\uDEAF\uDEB1\uDEB7\uDCF5\uDD1E\uDD03\uDD04\uDD19-\uDD1D\uDED0\uDD4E\uDD2F\uDD00-\uDD02\uDD3C\uDD3D\uDD05\uDD06\uDCF6\uDCF3\uDCF4\uDD31\uDCDB\uDD30\uDD1F\uDCAF\uDD20-\uDD24\uDD36-\uDD3B\uDCA0\uDD18\uDD32-\uDD35\uDEA9])|\uD83E(?:[\uDDD2\uDDD1\uDDD3](?:\uD83C[\uDFFB-\uDFFF])?|[\uDDB8\uDDB9](?:\u200D(?:[\u2640\u2642]\uFE0F?))?|[\uDD34\uDDD5\uDDD4\uDD35\uDD30\uDD31\uDD36](?:\uD83C[\uDFFB-\uDFFF])?|[\uDDD9-\uDDDD](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2640\u2642]\uFE0F?))?)|\u200D(?:[\u2640\u2642]\uFE0F?)))?|[\uDDDE\uDDDF](?:\u200D(?:[\u2640\u2642]\uFE0F?))?|[\uDD26\uDD37](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDDD6-\uDDD8](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2640\u2642]\uFE0F?))?)|\u200D(?:[\u2640\u2642]\uFE0F?)))?|\uDD38(?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|\uDD3C(?:\u200D(?:[\u2642\u2640]\uFE0F?))?|[\uDD3D\uDD3E\uDD39](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDD33\uDDB5\uDDB6\uDD1E\uDD18\uDD19\uDD1B\uDD1C\uDD1A\uDD1F\uDD32](?:\uD83C[\uDFFB-\uDFFF])?|[\uDD23\uDD70\uDD17\uDD29\uDD14\uDD28\uDD10\uDD24\uDD11\uDD2F\uDD75\uDD76\uDD2A\uDD2C\uDD12\uDD15\uDD22\uDD2E\uDD27\uDD20\uDD21\uDD73\uDD74\uDD7A\uDD25\uDD2B\uDD2D\uDDD0\uDD13\uDD16\uDD3A\uDD1D\uDDB0-\uDDB3\uDDE0\uDDB4\uDDB7\uDDE1\uDD7D\uDD7C\uDDE3-\uDDE6\uDD7E\uDD7F\uDDE2\uDD8D\uDD8A\uDD9D\uDD81\uDD84\uDD93\uDD8C\uDD99\uDD92\uDD8F\uDD9B\uDD94\uDD87\uDD98\uDDA1\uDD83\uDD85\uDD86\uDDA2\uDD89\uDD9A\uDD9C\uDD8E\uDD95\uDD96\uDD88\uDD80\uDD9E\uDD90\uDD91\uDD8B\uDD97\uDD82\uDD9F\uDDA0\uDD40\uDD6D\uDD5D\uDD65\uDD51\uDD54\uDD55\uDD52\uDD6C\uDD66\uDD5C\uDD50\uDD56\uDD68\uDD6F\uDD5E\uDDC0\uDD69\uDD53\uDD6A\uDD59\uDD5A\uDD58\uDD63\uDD57\uDDC2\uDD6B\uDD6E\uDD5F-\uDD61\uDDC1\uDD67\uDD5B\uDD42\uDD43\uDD64\uDD62\uDD44\uDDED\uDDF1\uDDF3\uDDE8\uDDE7\uDD47-\uDD49\uDD4E\uDD4F\uDD4D\uDD4A\uDD4B\uDD45\uDD4C\uDDFF\uDDE9\uDDF8\uDD41\uDDEE\uDDFE\uDDF0\uDDF2\uDDEA-\uDDEC\uDDEF\uDDF4-\uDDF7\uDDF9-\uDDFD])|[\u263A\u2639\u2620]\uFE0F?|\uD83C(?:\uDF85(?:\uD83C[\uDFFB-\uDFFF])?|\uDFC3(?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDFC7\uDFC2](?:\uD83C[\uDFFB-\uDFFF])?|\uDFCC(?:(?:\uFE0F(?:\u200D(?:[\u2642\u2640]\uFE0F?))?|\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDFC4\uDFCA](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|\uDFCB(?:(?:\uFE0F(?:\u200D(?:[\u2642\u2640]\uFE0F?))?|\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDFCE\uDFCD\uDFF5\uDF36\uDF7D\uDFD4-\uDFD6\uDFDC-\uDFDF\uDFDB\uDFD7\uDFD8\uDFDA\uDFD9\uDF21\uDF24-\uDF2C\uDF97\uDF9F\uDF96\uDF99-\uDF9B\uDF9E\uDFF7\uDD70\uDD71\uDD7E\uDD7F\uDE02\uDE37]\uFE0F?|\uDFF4(?:(?:\u200D\u2620\uFE0F?|\uDB40\uDC67\uDB40\uDC62\uDB40(?:\uDC65\uDB40\uDC6E\uDB40\uDC67\uDB40\uDC7F|\uDC73\uDB40\uDC63\uDB40\uDC74\uDB40\uDC7F|\uDC77\uDB40\uDC6C\uDB40\uDC73\uDB40\uDC7F)))?|\uDFF3(?:(?:\uFE0F(?:\u200D\uD83C\uDF08)?|\u200D\uD83C\uDF08))?|\uDDE6\uD83C[\uDDE8-\uDDEC\uDDEE\uDDF1\uDDF2\uDDF4\uDDF6-\uDDFA\uDDFC\uDDFD\uDDFF]|\uDDE7\uD83C[\uDDE6\uDDE7\uDDE9-\uDDEF\uDDF1-\uDDF4\uDDF6-\uDDF9\uDDFB\uDDFC\uDDFE\uDDFF]|\uDDE8\uD83C[\uDDE6\uDDE8\uDDE9\uDDEB-\uDDEE\uDDF0-\uDDF5\uDDF7\uDDFA-\uDDFF]|\uDDE9\uD83C[\uDDEA\uDDEC\uDDEF\uDDF0\uDDF2\uDDF4\uDDFF]|\uDDEA\uD83C[\uDDE6\uDDE8\uDDEA\uDDEC\uDDED\uDDF7-\uDDFA]|\uDDEB\uD83C[\uDDEE-\uDDF0\uDDF2\uDDF4\uDDF7]|\uDDEC\uD83C[\uDDE6\uDDE7\uDDE9-\uDDEE\uDDF1-\uDDF3\uDDF5-\uDDFA\uDDFC\uDDFE]|\uDDED\uD83C[\uDDF0\uDDF2\uDDF3\uDDF7\uDDF9\uDDFA]|\uDDEE\uD83C[\uDDE8-\uDDEA\uDDF1-\uDDF4\uDDF6-\uDDF9]|\uDDEF\uD83C[\uDDEA\uDDF2\uDDF4\uDDF5]|\uDDF0\uD83C[\uDDEA\uDDEC-\uDDEE\uDDF2\uDDF3\uDDF5\uDDF7\uDDFC\uDDFE\uDDFF]|\uDDF1\uD83C[\uDDE6-\uDDE8\uDDEE\uDDF0\uDDF7-\uDDFB\uDDFE]|\uDDF2\uD83C[\uDDE6\uDDE8-\uDDED\uDDF0-\uDDFF]|\uDDF3\uD83C[\uDDE6\uDDE8\uDDEA-\uDDEC\uDDEE\uDDF1\uDDF4\uDDF5\uDDF7\uDDFA\uDDFF]|\uDDF4\uD83C\uDDF2|\uDDF5\uD83C[\uDDE6\uDDEA-\uDDED\uDDF0-\uDDF3\uDDF7-\uDDF9\uDDFC\uDDFE]|\uDDF6\uD83C\uDDE6|\uDDF7\uD83C[\uDDEA\uDDF4\uDDF8\uDDFA\uDDFC]|\uDDF8\uD83C[\uDDE6-\uDDEA\uDDEC-\uDDF4\uDDF7-\uDDF9\uDDFB\uDDFD-\uDDFF]|\uDDF9\uD83C[\uDDE6\uDDE8\uDDE9\uDDEB-\uDDED\uDDEF-\uDDF4\uDDF7\uDDF9\uDDFB\uDDFC\uDDFF]|\uDDFA\uD83C[\uDDE6\uDDEC\uDDF2\uDDF3\uDDF8\uDDFE\uDDFF]|\uDDFB\uD83C[\uDDE6\uDDE8\uDDEA\uDDEC\uDDEE\uDDF3\uDDFA]|\uDDFC\uD83C[\uDDEB\uDDF8]|\uDDFD\uD83C\uDDF0|\uDDFE\uD83C[\uDDEA\uDDF9]|\uDDFF\uD83C[\uDDE6\uDDF2\uDDFC]|[\uDFFB-\uDFFF\uDF92\uDFA9\uDF93\uDF38-\uDF3C\uDF37\uDF31-\uDF35\uDF3E-\uDF43\uDF47-\uDF53\uDF45\uDF46\uDF3D\uDF44\uDF30\uDF5E\uDF56\uDF57\uDF54\uDF5F\uDF55\uDF2D-\uDF2F\uDF73\uDF72\uDF7F\uDF71\uDF58-\uDF5D\uDF60\uDF62-\uDF65\uDF61\uDF66-\uDF6A\uDF82\uDF70\uDF6B-\uDF6F\uDF7C\uDF75\uDF76\uDF7E\uDF77-\uDF7B\uDF74\uDFFA\uDF0D-\uDF10\uDF0B\uDFE0-\uDFE6\uDFE8-\uDFED\uDFEF\uDFF0\uDF01\uDF03-\uDF07\uDF09\uDF0C\uDFA0-\uDFA2\uDFAA\uDF11-\uDF20\uDF00\uDF08\uDF02\uDF0A\uDF83\uDF84\uDF86-\uDF8B\uDF8D-\uDF91\uDF80\uDF81\uDFAB\uDFC6\uDFC5\uDFC0\uDFD0\uDFC8\uDFC9\uDFBE\uDFB3\uDFCF\uDFD1-\uDFD3\uDFF8\uDFA3\uDFBD\uDFBF\uDFAF\uDFB1\uDFAE\uDFB0\uDFB2\uDCCF\uDC04\uDFB4\uDFAD\uDFA8\uDFBC\uDFB5\uDFB6\uDFA4\uDFA7\uDFB7-\uDFBB\uDFA5\uDFAC\uDFEE\uDFF9\uDFE7\uDFA6\uDD8E\uDD91-\uDD9A\uDE01\uDE36\uDE2F\uDE50\uDE39\uDE1A\uDE32\uDE51\uDE38\uDE34\uDE33\uDE3A\uDE35\uDFC1\uDF8C])|\u26F7\uFE0F?|\u26F9(?:(?:\uFE0F(?:\u200D(?:[\u2642\u2640]\uFE0F?))?|\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\u261D\u270C](?:(?:\uD83C[\uDFFB-\uDFFF]|\uFE0F))?|[\u270B\u270A](?:\uD83C[\uDFFB-\uDFFF])?|\u270D(?:(?:\uD83C[\uDFFB-\uDFFF]|\uFE0F))?|[\u2764\u2763\u26D1\u2618\u26F0\u26E9\u2668\u26F4\u2708\u23F1\u23F2\u2600\u2601\u26C8\u2602\u26F1\u2744\u2603\u2604\u26F8\u2660\u2665\u2666\u2663\u260E\u2328\u2709\u270F\u2712\u2702\u26CF\u2692\u2694\u2699\u2696\u26D3\u2697\u26B0\u26B1\u26A0\u2622\u2623\u2B06\u2197\u27A1\u2198\u2B07\u2199\u2B05\u2196\u2195\u2194\u21A9\u21AA\u2934\u2935\u269B\u267E\u2721\u2638\u262F\u271D\u2626\u262A\u262E\u25B6\u23ED\u23EF\u25C0\u23EE\u23F8-\u23FA\u23CF\u2640\u2642\u2695\u267B\u269C\u2611\u2714\u2716\u303D\u2733\u2734\u2747\u203C\u2049\u3030\u00A9\u00AE\u2122]\uFE0F?|[\u0023\u002A\u0030-\u0039](?:\uFE0F\u20E3|\u20E3)|[\u2139\u24C2\u3297\u3299\u25AA\u25AB\u25FB\u25FC]\uFE0F?|[\u2615\u26EA\u26F2\u26FA\u26FD\u2693\u26F5\u231B\u23F3\u231A\u23F0\u2B50\u26C5\u2614\u26A1\u26C4\u2728\u26BD\u26BE\u26F3\u267F\u26D4\u2648-\u2653\u26CE\u23E9-\u23EC\u2B55\u2705\u274C\u274E\u2795-\u2797\u27B0\u27BF\u2753-\u2755\u2757\u25FD\u25FE\u2B1B\u2B1C\u26AA\u26AB])";
                //filterBiography = Regex.Replace(filterBiography, EmojiPattern, ""); 
                #endregion

                // After removing alphabets, letters and emojis 
                // if string is empty then it returns "English" 
                // otherwise "Non-english".
                if ((userInfo.Biography.Length > filterBiography.Length) && (filterBiography == string.Empty))
                    return false;
                else
                    return true;
            }


            public List<InstagramUser> FilterAllNonEnglishUsers(List<InstagramUser> userInfo)
            {
                if (ModuleSetting.UserFilterModel.IgnoreNonEnglishUser)
                    return IsAllNonEnglishUser(userInfo);
                return userInfo;
            }
            private List<InstagramUser> IsAllNonEnglishUser(List<InstagramUser> userInfo)
            {
                try
                {
                    List<InstagramUser> lstUser = new List<InstagramUser>();
                    foreach (var key in userInfo)
                    {
                        if (string.IsNullOrEmpty(key.UserDetails.Biography))
                            continue;
                        string filterBiography = Regex.Replace(key.UserDetails.Biography, @"[A-Za-z0-9\s+]", "");
                        // Removes Emoji's from a text string
                        filterBiography = Regex.Replace(filterBiography, @"\p{Cs}", "");
                        string[] chars = new[] { "\\n", "?", ",", ".", "/", "!", "@", "#", "$", "%", "^", "&", "*", "'", "\"", ";", "_", "-", "(", ")", ":", "|", "[", "]", "♡", "•" };
                        foreach (var character in chars)
                        {
                            if (filterBiography.Contains(character))
                            {
                                filterBiography = filterBiography.Replace(character, "");
                            }
                        }
                        if ((key.UserDetails.Biography.Length > filterBiography.Length) && (filterBiography == string.Empty))
                        {}
                        else
                           lstUser.Add(key);                       
                    }
                    userInfo.RemoveAll(x=>lstUser.Any(y=>y.Username==x.Username));
                }
                catch (Exception)
                {

                }
                return userInfo;
            }
            #endregion


        }
    }
}