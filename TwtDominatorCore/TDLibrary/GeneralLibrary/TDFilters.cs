using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDModels;

namespace TwtDominatorCore.TDLibrary
{
    internal static class TDFilters
    {
        public class Settings
        {
            protected readonly ModuleSetting ModuleSetting;

            protected Settings(ModuleSetting settings)
            {
                ModuleSetting = settings;
            }
        }

        public class TweetFilterFunctions : Settings
        {
            public TweetFilterFunctions(ModuleSetting settings)
                : base(settings)
            {
            }

            private ITweetFilter TweetFIlter => ModuleSetting.TweetFilterModel;

            public bool IsTweetFilterActive()
            {
                return TweetFIlter.IsFilterAlreadyLiked || TweetFIlter.IsFilterAlreadyRetweeted ||
                       TweetFIlter.IsFilterSkipRetweets || TweetFIlter.IsFilterSkipTweetsContainingAtSign ||
                       TweetFIlter.IsFilterSkipTweetsContainingSpecificWords ||
                       TweetFIlter.IsFilterSkipTweetsContainNonEnglishChar || TweetFIlter.IsFilterSkipTweetsWithLinks ||
                       TweetFIlter.IsFilterSkipTweetsWithoutLinks || TweetFIlter.IsFilterTweetsHaveBetweenFavorites ||
                       TweetFIlter.IsFilterTweetsHaveBetweenRetweets || TweetFIlter.IsFilterTweetsLessThanSpecificDaysOld ||
                       TweetFIlter.IsFilterTweetsLessThanSpecificHoursOld;
            }

            #region Filter Already Liked Tweet

            public void FilterAlreadyLiked(List<TagDetails> Tweet)
            {
                if (!TweetFIlter.IsFilterAlreadyLiked)
                    return;
                Tweet.RemoveAll(x => IsAlreadyLikedTweet(x));
            }

            public bool FilterAlreadyLiked(TagDetails Tweet)
            {
                if (!TweetFIlter.IsFilterAlreadyLiked)
                    return false;
                return IsAlreadyLikedTweet(Tweet);
            }

            public static bool IsAlreadyLikedTweet(TagDetails x)
            {
                return x.IsAlreadyLiked;
            }

            #endregion

            #region Filter Already Retweeted Tweet from running account

            public void FilterAlreadyRetweeted(List<TagDetails> Tweet)
            {
                if (!TweetFIlter.IsFilterAlreadyRetweeted)
                    return;
                Tweet.RemoveAll(x => IsAlreadyRetweeted(x));
            }

            public bool FilterAlreadyRetweeted(TagDetails Tweet)
            {
                if (!TweetFIlter.IsFilterAlreadyRetweeted)
                    return false;
                return IsAlreadyRetweeted(Tweet);
            }

            private static bool IsAlreadyRetweeted(TagDetails x)
            {
                return x.IsAlreadyRetweeted;
            }

            #endregion

            #region Filter  Retweeted Tweet

            public void FilterIsRetweetedTweet(List<TagDetails> Tweet)
            {
                if (!TweetFIlter.IsFilterSkipRetweets)
                    return;
                Tweet.RemoveAll(x => IsRetweetedTweet(x));
            }

            public bool FilterIsRetweetedTweet(TagDetails Tweet)
            {
                if (!TweetFIlter.IsFilterSkipRetweets)
                    return false;
                return IsRetweetedTweet(Tweet);
            }

            private static bool IsRetweetedTweet(TagDetails x)
            {
                return x.IsRetweet;
            }

            #endregion

            #region Filter Already Reposted Tweet

            public bool FilterAlreadyReposted(TagDetails Tweet, IDbAccountService _accountService)
            {
                return IsAlreadyReposted(Tweet, _accountService);
            }

            private static bool IsAlreadyReposted(TagDetails x, IDbAccountService _accountService)
            {
                try
                {
                    var listTweetReport = _accountService.GetInteractedPosts(ActivityType.Reposter)
                        .Where(y => x.Id == y.TweetId).ToList();
                    if (listTweetReport.Count > 0)
                        return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                return x.IsAlreadyReposted;
            }

            #endregion

            #region Filter Old Tweets

            public void FilterOldTweets(List<TagDetails> Tweet)
            {
                if (TweetFIlter.IsFilterTweetsLessThanSpecificHoursOld)
                    Tweet.RemoveAll(x => IsFilterOldTweets(x));

                else if (TweetFIlter.IsFilterTweetsLessThanSpecificDaysOld)
                    Tweet.RemoveAll(x => IsFilterOldTweets(x));
            }

            public bool FilterOldTweets(TagDetails Tweet)
            {
                return IsFilterOldTweets(Tweet);
            }

            private bool IsFilterOldTweets(TagDetails x)
            {
                try
                {
                    if (TweetFIlter.IsFilterTweetsLessThanSpecificDaysOld)
                    {
                        return CompareAndReturn(x.TweetedTimeStamp, TweetFIlter.IsFilterTweetsLessThanSpecificDaysOld);
                    }else if(TweetFIlter.IsFilterTweetsLessThanSpecificHoursOld)
                        return CompareAndReturn(x.TweetedTimeStamp, false);
                    else return false;
                }
                catch { return false; }
                
            }
            private bool CompareAndReturn(int takenAt, bool DaysOld = false)
            {
                var dateProvider = InstanceProvider.GetInstance<IDateProvider>();
                var utcNow = dateProvider.UtcNow();
                var postUtc = takenAt.EpochToDateTimeUtc();
                if (DaysOld)
                {
                    if (utcNow.Year == postUtc.Year)
                    {
                        if (utcNow.Month == postUtc.Month)
                        {
                            return (utcNow.Day - postUtc.Day) >= TweetFIlter.FilterTweetsLessThanSpecificDaysOldValue;
                        }
                        else
                        {
                            var monthdiff = utcNow.Month - postUtc.Month;
                            return (monthdiff * 730) >= TweetFIlter.FilterTweetsLessThanSpecificDaysOldValue;
                        }
                    }
                    else
                    {
                        var diffYears = utcNow.Year - postUtc.Year;
                        return (diffYears * 365) >= TweetFIlter.FilterTweetsLessThanSpecificDaysOldValue;
                    }
                }
                else
                {
                    if (utcNow.Year == postUtc.Year)
                    {
                        if (utcNow.Month == postUtc.Month)
                        {
                            if(utcNow.Day == postUtc.Day)
                            {
                                return (utcNow.Hour - postUtc.Hour) >= TweetFIlter.FilterTweetsLessThanSpecificHoursOldValue;
                            }
                            else
                            {
                                var dayDiff = utcNow.Day - postUtc.Day;
                                return (dayDiff * 24) >= TweetFIlter.FilterTweetsLessThanSpecificHoursOldValue;
                            }
                        }
                        else
                        {
                            var monthdiff = utcNow.Month - postUtc.Month;
                            return (monthdiff * 730) >= TweetFIlter.FilterTweetsLessThanSpecificHoursOldValue;
                        }
                    }
                    else
                    {
                        var diffYears = utcNow.Year - postUtc.Year;
                        return (diffYears * 8766) >= TweetFIlter.FilterTweetsLessThanSpecificHoursOldValue;
                    }
                }
            }
            #endregion

            #region Filter by retweet range

            public void FilterByRetweetsRange(List<TagDetails> Tweet)
            {
                if (!ModuleSetting.TweetFilterModel.IsFilterTweetsHaveBetweenRetweets)
                    return;
                Tweet.RemoveAll(x => IsRetweetsOutOfRange(x));
            }

            public bool FilterByRetweetsRange(TagDetails Tweet)
            {
                if (!ModuleSetting.TweetFilterModel.IsFilterTweetsHaveBetweenRetweets)
                    return false;
                return IsRetweetsOutOfRange(Tweet);
            }

            private bool IsRetweetsOutOfRange(TagDetails x)
            {
                return !ModuleSetting.TweetFilterModel.FilterTweetsHaveBetweenRetweets.InRange(x.RetweetCount);
            }

            #endregion

            #region Filter by Favorites Range

            public void FilterByFavoritesRange(List<TagDetails> Tweet)
            {
                if (!TweetFIlter.IsFilterTweetsHaveBetweenFavorites)
                    return;
                Tweet.RemoveAll(x => IsFavoritesOutOfRange(x));
            }

            public bool FilterByFavoritesRange(TagDetails Tweet)
            {
                if (!TweetFIlter.IsFilterTweetsHaveBetweenFavorites)
                    return false;
                return IsFavoritesOutOfRange(Tweet);
            }

            private bool IsFavoritesOutOfRange(TagDetails x)
            {
                return !TweetFIlter.FilterTweetsHaveBetweenFavorites.InRange(x.LikeCount);
            }

            #endregion

            #region Filter By Restricted words

            public void FilterTweetByRestrictedWords(List<TagDetails> Tweet)
            {
                if (!TweetFIlter.IsFilterSkipTweetsContainingSpecificWords)
                    return;
                Tweet.RemoveAll(x => IsTweetContainsRestrictedWords(x));
            }

            public bool FilterTweetByRestrictedWords(TagDetails Tweet)
            {
                if (!TweetFIlter.IsFilterSkipTweetsContainingSpecificWords)
                    return false;
                return IsTweetContainsRestrictedWords(Tweet);
            }

            private bool IsTweetContainsRestrictedWords(TagDetails x)
            {
                return TweetFIlter.LstSkipTweetsContainingWords.Any(y => x.Caption.ToLower().Contains(y.ToLower()));
            }

            #endregion

            #region Filter Tweet with Non English Character

            public void FilterTweetsWithNonEnglishCharacter(List<TagDetails> Tweet)
            {
                if (!TweetFIlter.IsFilterSkipTweetsContainNonEnglishChar)
                    return;
                Tweet.RemoveAll(x => IsFilterTweetsWithNonEnglishCharacter(x));
            }

            public bool FilterTweetsWithNonEnglishCharacter(TagDetails Tweet)
            {
                if (!TweetFIlter.IsFilterSkipTweetsContainNonEnglishChar)
                    return false;
                return IsFilterTweetsWithNonEnglishCharacter(Tweet);
            }

            private bool IsFilterTweetsWithNonEnglishCharacter(TagDetails x)
            {
                var englishPattern = "^[a-zA-Z0-9 .,&#@!:;\"'=_+]*$";
                try
                {
                    var AfterCaption = Regex.Replace(x.Caption, "[.!@#$%^&*()?<>~`^,\\\\_+\\[\\]{}/]", "");
                    var check = Regex.IsMatch(AfterCaption, englishPattern);
                    if(!check)
                        check = Regex.IsMatch(AfterCaption, @"\p{So}|\p{Cs}\p{Cs}(\p{Cf}\p{Cs}\p{Cs})*");
                    return !check;
                }
                catch (Exception ex)
                {
                    ex.ErrorLog();
                }

                return !Regex.IsMatch(x.Caption, englishPattern);
            }

            #endregion

            #region Filter Tweet not containing Link

            public void FilterWithoutLinks(List<TagDetails> Tweet)
            {
                if (!TweetFIlter.IsFilterSkipTweetsWithoutLinks)
                    return;
                Tweet.RemoveAll(x => IsFilterWithoutLinks(x));
            }

            public bool FilterWithoutLinks(TagDetails Tweet)
            {
                if (!TweetFIlter.IsFilterSkipTweetsWithoutLinks)
                    return false;
                return IsFilterWithoutLinks(Tweet);
            }

            private bool IsFilterWithoutLinks(TagDetails x)
            {
                try
                {
                    var listUrlList = Regex.Split(x.Caption, "http").Skip(1).Select(y => "http" + y).ToList();
                    return !listUrlList.Any(y =>
                        (y.Contains("http://") || y.Contains("https://")) && !y.Contains("https://pbs.twimg.com"));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                return !(x.Caption.Contains("http://www") || x.Caption.Contains("https://www"));
            }

            #endregion

            #region Filter Tweet containing Links

            public void FilterWithLinks(List<TagDetails> Tweet)
            {
                if (!TweetFIlter.IsFilterSkipTweetsWithLinks)
                    return;
                Tweet.RemoveAll(x => IsFilterWithLinks(x));
            }

            public bool FilterWithLinks(TagDetails Tweet)
            {
                if (!TweetFIlter.IsFilterSkipTweetsWithLinks || string.IsNullOrEmpty(Tweet.Caption))
                    return false;
                return IsFilterWithLinks(Tweet);
            }

            private bool IsFilterWithLinks(TagDetails x)
            {
                try
                {
                    var listUrlList = Regex.Split(x.Caption, "http").Skip(1).Select(y => "http" + y).ToList();
                    return listUrlList.Any(y =>
                        (y.Contains("http://") || y.Contains("https://")) && !y.Contains("https://pbs.twimg.com"));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                return x.Caption.Contains("http://www") ||
                       x.Caption.Contains("https://www"); //&& !x.Caption.Contains("https://pbs.twimg.com"));
            }

            #endregion

            #region Filter Tweet containing '@'

            public void FilterTweetContainingAtSign(List<TagDetails> Tweet)
            {
                if (!TweetFIlter.IsFilterSkipTweetsContainingAtSign)
                    return;
                Tweet.RemoveAll(x => IsFilterTweetContainingAtSign(x));
            }

            public bool FilterTweetContainingAtSign(TagDetails Tweet)
            {
                if (!TweetFIlter.IsFilterSkipTweetsContainingAtSign)
                    return false;
                return IsFilterTweetContainingAtSign(Tweet);
            }

            private bool IsFilterTweetContainingAtSign(TagDetails x)
            {
                return x.Caption.Contains("@");
            }

            #endregion
        }

        public class UserFilterFunctions : Settings
        {
            public UserFilterFunctions(ModuleSetting settings)
                : base(settings)
            {
            }

            private IUserFilter UserFilter => ModuleSetting.UserFilterModel;

            public bool IsAlreadyFollowing(TwitterUser user)
            {
                return user.FollowStatus;
            }

            public bool IsUserFilterActive()
            {
                return UserFilter.IsFilterBioHasMinimumCharacters || UserFilter.IsFilterEnglishCharactersInBio ||
                       UserFilter.IsFilterFollowersRange || UserFilter.IsFilterFollowingsRange ||
                       UserFilter.IsFilterFollowRatioGreaterThan || UserFilter.IsFilterFollowRatioSmallerThan ||
                       UserFilter.IsFilterMinimumNumberOfTweets || UserFilter.IsFilterMustContainSpecificWords ||
                       UserFilter.IsFilterMustNotContainInvalidWords || UserFilter.IsFilterTweetedWithinLastDays;
            }

            #region Filter based on verification 

            public bool FilterBasedOnVerification(TwitterUser user)
            {
                if (UserFilter.IsFilterByVerification && UserFilter.IsFilterActiveVerfiedUser && user.IsVerified)
                    return true;
                // filter for skip not verified user
                if (UserFilter.IsFilterByVerification && UserFilter.IsFilterActiveWhoAreNotVerified && !user.IsVerified)
                    return true;
                return false;
            }

            #endregion

            #region Filter Muted User

            public bool FilterIfUserMuted(TwitterUser user)
            {
                if (!UserFilter.IsFilterMutedUser)
                    return false;
                return IsUserMuted(user);
            }

            private bool IsUserMuted(TwitterUser x)
            {
                return x.IsMuted;
            }

            #endregion

            #region Filter Users not having profile image

            public bool FilterByProfileImage(TwitterUser user)
            {
                if (!UserFilter.IsFilterProfileImage)
                    return false;
                return DoesUserNotHavingProfileImage(user);
            }

            private bool DoesUserNotHavingProfileImage(TwitterUser x)
            {
                return !x.HasProfilePic;
            }

            #endregion

            #region Filter Private User

            public bool FilterIfPrivateUser(TwitterUser user)
            {
                if (!UserFilter.IsFilterPrivateUser)
                    return false;
                return IsUserhavingPrivateProfile(user);
            }

            private bool IsUserhavingPrivateProfile(TwitterUser x)
            {
                return x.IsPrivate;
            }

            #endregion

            #region Filter If user is not following back to this account

            public bool FilterIfUserIsNotFollowing(TwitterUser user)
            {
                if (!UserFilter.IsFilterUserIsNotFollowingThisAccount)
                    return false;
                return IsUserNotFollowingBack(user);
            }

            private bool IsUserNotFollowingBack(TwitterUser x)
            {
                return x.FollowBackStatus;
            }

            #endregion

            #region Filter non english character from bio

            public bool FilterIfUserHaveNonEnglishCharacterInBio(TwitterUser user)
            {
                if (!UserFilter.IsFilterEnglishCharactersInBio)
                    return false;
                return IsUserHaveNonEnglishCharacterInBio(user);
            }

            private bool IsUserHaveNonEnglishCharacterInBio(TwitterUser x)
            {
                return !Regex.IsMatch(x.UserBio, "^[a-zA-Z0-9 .,&#@!:;\"'=_+\n]*$");
            }

            #endregion

            #region Filter user by tweets count

            public bool FilterByTweetsCount(TwitterUser user)
            {
                if (!UserFilter.IsFilterMinimumNumberOfTweets)
                    return false;
                return IsUserNotHavingMinimumTweetCount(user);
            }

            private bool IsUserNotHavingMinimumTweetCount(TwitterUser x)
            {
                return UserFilter.FilterMinimumNumberOfTweetsValue > x.TweetsCount;
            }

            #endregion

            #region Filter user by followings count

            public bool FilterByFollowingsCount(TwitterUser user)
            {
                if (!UserFilter.IsFilterFollowingsRange)
                    return false;
                return IsUserFollowingCountOutOfRange(user);
            }

            private bool IsUserFollowingCountOutOfRange(TwitterUser x)
            {
                return !UserFilter.FilterFollowingsRange.InRange(x.FollowingsCount);
            }

            #endregion

            #region Filter user by followers count

            public bool FilterByFollowersCount(TwitterUser user)
            {
                if (!UserFilter.IsFilterFollowersRange)
                    return false;
                return IsUserFollowersCountOutOfRange(user);
            }

            private bool IsUserFollowersCountOutOfRange(TwitterUser x)
            {
                return !UserFilter.FilterFollowersRange.InRange(x.FollowersCount);
            }

            #endregion

            #region Filter if follow ratio is greater than given value

            public bool FilterByFollowRatioGreaterThan(TwitterUser user)
            {
                if (!UserFilter.IsFilterFollowRatioGreaterThan)
                    return false;
                return !IsFollowRatioGreaterThan(user);
            }

            private bool IsFollowRatioGreaterThan(TwitterUser x)
            {
                return x.FollowingsCount == 0 || x.FollowersCount / (double)x.FollowingsCount >
                       UserFilter.FilterFollowRatioGreaterThanValue;
            }

            #endregion

            #region Filter if follow ratio smaller than

            public bool FilterByFollowRatioSmallerThan(TwitterUser user)
            {
                if (!UserFilter.IsFilterFollowRatioSmallerThan)
                    return false;
                return !IsFollowRatioSmallerThan(user);
            }

            private bool IsFollowRatioSmallerThan(TwitterUser x)
            {
                return x.FollowingsCount != 0 && x.FollowersCount / (double)x.FollowingsCount <
                       UserFilter.FilterFollowRatioSmallerThanValue;
            }

            #endregion

            #region Filter by Bio Character Length

            public bool FilterByBioCharacterLength(TwitterUser user)
            {
                if (!UserFilter.IsFilterBioHasMinimumCharacters)
                    return false;
                return IsBioLengthNotHavingMinimumValue(user);
            }

            private bool IsBioLengthNotHavingMinimumValue(TwitterUser x)
            {
                return x.UserBio.Length < UserFilter.FilterMinimumCharactersValue;
            }

            #endregion

            #region Filter if bio contain Invalid words

            public bool FilterIfBioContainInvalidWords(TwitterUser user)
            {
                if (!UserFilter.IsFilterMustNotContainInvalidWords)
                    return false;
                return IsBioContainInvalidWords(user);
            }

            private bool IsBioContainInvalidWords(TwitterUser x)
            {
                return UserFilter.LstInvalidWords.Any(y =>
                    x.UserBio.ToLower().Contains(y.ToLower()) || x.Username.ToLower().Contains(y.ToLower()) ||
                    x.FullName.ToLower().Contains(y.ToLower()));
            }

            #endregion

            #region Filter if bio not contained specific words

            public bool FilterIfBioNotContainSpecificWords(TwitterUser user)
            {
                if (!UserFilter.IsFilterMustContainSpecificWords)
                    return false;
                return IsBioNotContainedSpecificWords(user);
            }

            private bool IsBioNotContainedSpecificWords(TwitterUser x)
            {
                // bool check= !this.UserFilter.LstSpecificWords.Any<string>((Func<string, bool>)(y => x.UserBio.ToLower().Contains(y.ToLower()) || x.Username.ToLower().Contains(y.ToLower()) || x.FullName.ToLower().Contains(y.ToLower())));
                return !UserFilter.LstSpecificWords.Any(y =>
                    x.UserBio.ToLower().Contains(y.ToLower()) || x.Username.ToLower().Contains(y.ToLower()) ||
                    x.FullName.ToLower().Contains(y.ToLower()));
            }

            #endregion

            #region Filter by last tweet age

            public bool FilterByLastTweetAge(TwitterUser user)
            {
                if (!UserFilter.IsFilterTweetedWithinLastDays)
                    return false;
                return IsTweetOlderThanGivenValue(user);
            }

            private bool IsTweetOlderThanGivenValue(TwitterUser x)
            {
                var tweets = x.ListTag;
                var lastTweetCountList = new List<int>();
                foreach (var tweet in tweets)
                {
                    var currentDate = DateTime.Now;
                    var tweetDate = tweet.DateTime;
                    var LastTweetedDayCount = (currentDate.Date - tweetDate.Date).Days;
                    lastTweetCountList.Add(LastTweetedDayCount);
                }
                bool isTweetOlder = true;
                if (lastTweetCountList.Any(y => y <= UserFilter.FilterTweetedWithinTheLastValue))
                {
                    isTweetOlder = false;
                }
                return isTweetOlder;
            }

            #endregion
        }
    }
}