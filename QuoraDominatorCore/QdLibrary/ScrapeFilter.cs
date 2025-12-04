using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.Interface;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.Response;

namespace QuoraDominatorCore.QdLibrary
{
    internal static class ScrapeFilter
    {
        public class Settings
        {
            public readonly ModuleSetting ModuleSetting;
            public readonly IProcessScopeModel ProcessScope;
            public Settings(ModuleSetting settings, IProcessScopeModel processScopeModel)
            {
                ModuleSetting = settings;
                ProcessScope = processScopeModel;
            }
        }

        [Localizable(false)]
        public class User : Settings
        {
            private List<UserInfoResponseHandler> _detailedInfo;

            public User(ModuleSetting settings, IProcessScopeModel processScopeModel)
                : base(settings, processScopeModel)
            {
            }

            public User(ModuleSetting settings, DominatorAccountModel dominatorAccountModel, IProcessScopeModel processScopeModel)
                : base(settings, processScopeModel)
            {
                DbAccountService =
                    InstanceProvider.ResolveWithDominatorAccount<IDbAccountService>(dominatorAccountModel);
            }

            private IDbAccountService DbAccountService { get; }

            public List<UserInfoResponseHandler> DetailedInfo
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

            private IUserFilter CustomUserFilter => ModuleSetting.CustomUserFilterModel;

            private AnswerFilterModel CustomAnswerFilter => ModuleSetting.CustomAnswerFilterModel;

            private QuestionFilterModel CustomQuestionFilter => ModuleSetting.CustomQuestionFilterModel;

            private IQuestionFilter QuestionFilter => ModuleSetting.QuestionFilterModel;

            private IAnswerFilter AnswerFilter => ModuleSetting.AnswerFilterModel;

            private IUnFollower UnFollowFilterFilter => ProcessScope.GetActivitySettingsAs<UnfollowerModel>();
           // ModuleSetting.UnFollowFilterModel;

            #region FilterIsVerified

            public bool IsVerifiedUser(QuoraUser user)
            {
                return user.IsVerified;
            }

            #endregion

            #region Filter AnswerCount

            public bool IsAnswerCountIsInRange(UserInfoResponseHandler userNameResponseHandler)
            {
                if (CustomUserFilter.FilterAnswersCount)
                    return !CustomUserFilter.AnswersCount.InRange(userNameResponseHandler.UserAnswerCount);
                if (UserFilter.FilterAnswersCount)
                    return !UserFilter.AnswersCount.InRange(userNameResponseHandler.UserAnswerCount);
                return false;
            }

            #endregion

            #region Filter QuestionCount

            public bool IsQuestionCountIsInRange(UserInfoResponseHandler usernameresponsehandler)
            {
                if (CustomUserFilter.FilterQuestionsCount)
                    return !CustomUserFilter.QuestionsCount.InRange(usernameresponsehandler.UserQuestionsCount);
                if (UserFilter.FilterQuestionsCount)
                    return !UserFilter.QuestionsCount.InRange(usernameresponsehandler.UserQuestionsCount);
                return false;
            }

            #endregion

            #region Filter PostCount

            public bool IsPostCountIsInRange(UserInfoResponseHandler usernameresponsehandler)
            {
                if (CustomUserFilter.FilterPostsCounts)
                    return !CustomUserFilter.PostCounts.InRange(usernameresponsehandler.UserPostsCount);
                if (UserFilter.FilterPostsCounts)
                    return !UserFilter.PostCounts.InRange(usernameresponsehandler.UserPostsCount);
                return false;
            }

            #endregion

            #region Filter BlogCount

            public bool IsBlogCountIsInRange(UserInfoResponseHandler usernameresponsehandler)
            {
                if (CustomUserFilter.FilterBlogsCounts)
                    return !CustomUserFilter.BlogsCounts.InRange(usernameresponsehandler.UserBlogsCount);
                if (UserFilter.FilterBlogsCounts)
                    return !UserFilter.BlogsCounts.InRange(usernameresponsehandler.UserBlogsCount);
                return false;
            }

            #endregion

            #region Filter TopicCount

            public bool IsTopicCountIsInRange(UserInfoResponseHandler usernameresponsehandler)
            {
                if (CustomUserFilter.FilterTopicsCounts)
                    return !CustomUserFilter.TopicsCounts.InRange(usernameresponsehandler.UserTopicsCount);
                if (UserFilter.FilterTopicsCounts)
                    return !UserFilter.TopicsCounts.InRange(usernameresponsehandler.UserTopicsCount);
                return false;
            }

            #endregion

            #region Filter EditCount

            public bool IsEditCountIsInRange(UserInfoResponseHandler usernameresponsehandler)
            {
                if (CustomUserFilter.FilterEditsCounts)
                    return !CustomUserFilter.EditsCounts.InRange(usernameresponsehandler.UserEditsCount);
                if (UserFilter.FilterEditsCounts)
                    return !UserFilter.EditsCounts.InRange(usernameresponsehandler.UserEditsCount);
                return false;
            }

            #endregion

            #region Filter TotalAnswerView

            public bool IsAnswerViewCountIsInRange(UserInfoResponseHandler userNameResponseHandler)
            {
                if (CustomUserFilter.FilterAnswersCount)
                    return !CustomUserFilter.AnswerViewsCounts.InRange(userNameResponseHandler.TotalAnswerView);
                if (UserFilter.FilterAnswerViewsCounts)
                    return !UserFilter.AnswerViewsCounts.InRange(userNameResponseHandler.TotalAnswerView);
                return false;
            }

            #endregion

            public bool CheckForRangeFollowRatio(UserInfoResponseHandler userInfoResponseHandler)
            {
                if (CustomUserFilter.FilterSpecificFollowRatio)
                    return !CustomUserFilter.SpecificFollowRatio.InRange(userInfoResponseHandler.FollowRatio);
                if (UserFilter.FilterSpecificFollowRatio)
                    return !UserFilter.SpecificFollowRatio.InRange(userInfoResponseHandler.FollowRatio);
                return false;
            }

            #region FilterIgnoreFollower

            public bool FilterIgnoreFollower(UserInfoResponseHandler userInfoResponseHandler)
            {
                if (CustomUserFilter.IgnoreCurrentUsersFollowers)
                    return userInfoResponseHandler.FollowedBack == 1;
                if (UserFilter.IgnoreCurrentUsersFollowers &&
                    userInfoResponseHandler.FollowedBack == 1)
                    return true;
                return false;
            }

            #endregion

            public bool FilterLastWriteAnswer(UserInfoResponseHandler userInfoResponseHandler)
            {
                try
                {
                    if (CustomUserFilter.FilterAnsweredInRecentDays)
                        return !(CustomUserFilter.AnsweredInRecentDays.StartValue >= userInfoResponseHandler.AnsweredLast);
                    if (UserFilter.FilterAnsweredInRecentDays)
                        return !((userInfoResponseHandler.AnsweredLast==0)?false:(UserFilter.AnsweredInRecentDays.StartValue > 365 && userInfoResponseHandler.AnsweredLast > 365) ? (UserFilter.AnsweredInRecentDays.StartValue / 365 >= userInfoResponseHandler.AnsweredLast / 365) : (UserFilter.AnsweredInRecentDays.StartValue > 30 && userInfoResponseHandler.AnsweredLast > 30) ? (UserFilter.AnsweredInRecentDays.StartValue / 30 >= userInfoResponseHandler.AnsweredLast / 30) : (UserFilter.AnsweredInRecentDays.StartValue >= userInfoResponseHandler.AnsweredLast));
                }
                catch (Exception) { }
                return false;
            }

            #region

            public bool FilterMinimumCharacterInBio(UserInfoResponseHandler quorauser)
            {
                if (CustomUserFilter.FilterMinimumCharacterInBio &&
                    (string.IsNullOrEmpty(quorauser.Biography) ||
                     UserFilter.MinimumCharacterInBio > quorauser.Biography?.Length))
                    return true;
                if (UserFilter.FilterMinimumCharacterInBio &&
                    (string.IsNullOrEmpty(quorauser.Biography) ||
                     UserFilter.MinimumCharacterInBio > quorauser.Biography?.Length))
                    return true;
                return false;
            }

            #endregion

            #region UnFollow FollowedBySoftware

            public bool FilterFollowedBySoftware(string accountid, UserInfoResponseHandler quorauser)
            {
                if (UnFollowFilterFilter.IsChkPeopleFollowedBySoftwareChecked)
                    try
                    {
                        if (DbAccountService.GetInteractedUsers(ActivityType.Follow)
                                .Count(x => x.InteractedUsername == quorauser.Username) == 0)
                            return true;
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                return false;
            }

            #endregion

            #region UnFollow FollowedOutSideSoftware

            public bool FilterFollowedOutsideSoftware(string accountid, UserInfoResponseHandler quorauser)
            {
                if (UnFollowFilterFilter.IsChkPeopleFollowedOutsideSoftwareChecked)
                    if (DbAccountService.GetInteractedUsers(ActivityType.Follow)
                            .Count(x => x.InteractedUsername == quorauser.Username) != 0)
                        return true;
                return false;
            }

            #endregion

            #region////if user select both the option

            public bool FilterFollowedBySoftwareOrOutside(string accountid, UserInfoResponseHandler quorauser)
            {
                if (UnFollowFilterFilter.IsChkPeopleFollowedBySoftwareChecked &&
                    !UnFollowFilterFilter.IsChkPeopleFollowedOutsideSoftwareChecked)
                {
                    if (DbAccountService.GetInteractedUsers(ActivityType.Follow)
                            .Count(x => x.InteractedUsername == quorauser.Username) == 0)
                        return true;
                }
                else if (!UnFollowFilterFilter.IsChkPeopleFollowedBySoftwareChecked &&
                         UnFollowFilterFilter.IsChkPeopleFollowedOutsideSoftwareChecked)
                {
                    if (DbAccountService.GetInteractedUsers(ActivityType.Follow)
                            .Count(x => x.InteractedUsername == quorauser.Username) != 0)
                        return true;
                }

                return false;
            }

            #endregion

            #region Filter FollowedBack and not both check

            public bool FilterFollowedBackOrNot(UserInfoResponseHandler userInfoResponseHandler)
            {
                if (UnFollowFilterFilter.IsWhoFollowBackChecked && !UnFollowFilterFilter.IsWhoDoNotFollowBackChecked)
                    return userInfoResponseHandler.FollowedBack == 0;
                if (!UnFollowFilterFilter.IsWhoFollowBackChecked && UnFollowFilterFilter.IsWhoDoNotFollowBackChecked)
                    return userInfoResponseHandler.FollowedBack == 1;
                return false;
            }

            #endregion

            #region FilterFollowedBackForUnfollow

            public bool FilterFollowedBack(UserInfoResponseHandler userInfoResponseHandler)
            {
                if (UnFollowFilterFilter.IsWhoFollowBackChecked)
                    if (userInfoResponseHandler.FollowedBack == 0)
                        return true;
                return false;
            }

            #endregion

            #region FilterNotFollowedBackForUnfollow

            public bool FilterNotFollowedBack(UserInfoResponseHandler userInfoResponseHandler)
            {
                if (UnFollowFilterFilter.IsWhoDoNotFollowBackChecked)
                {
                    if (userInfoResponseHandler.FollowedBack == 1)
                        return true;
                    return false;
                }

                return false;
            }

            #endregion

            #region FilterNotFollowedBackForUnfollow

            public bool FilterLastAnswer(UserInfoResponseHandler userInfoResponseHandler,
                DominatorAccountModel account) //how many hours ago has followed?
            {
                if (UnFollowFilterFilter.IsUserFollowedBeforeChecked)
                    try
                    {
                        var result = DbAccountService?.GetInteractedUsers(ActivityType.Follow)
                            .FirstOrDefault(x => x.InteractedUsername == userInfoResponseHandler.Username)?.Date;

                        if (result != null)
                        {
                            var followedhour = result.Value.EpochToDateTimeUtc();

                            var totalHourAferFollow = (int) (DateTime.UtcNow - followedhour).TotalHours;
                            var totalHours = UnFollowFilterFilter.FollowedBeforeDay * 24 +
                                             UnFollowFilterFilter.FollowedBeforeHour;
                            if (totalHourAferFollow <= totalHours)
                                return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                return false;
            }

            #endregion


            public bool IsPrivateUser(QuoraUser x)
            {
                return x.IsPrivate;
            }


            #region filter for bio blacklist

            public void FilterBioRestrictedWords(List<QuoraUser> users)
            {
                if (!UserFilter.FilterBioRestrictedWords)
                    return;
                for (var index = DetailedInfo.Count - 1; index >= 0; --index)
                {
                    var userInfoResponseHandler = DetailedInfo[index];
                    if (!IsBioRestrictedWords(userInfoResponseHandler)) DetailedInfo.RemoveAt(index);
                }
            }

            public bool FilterBioRestrictedWords(UserInfoResponseHandler user)
            {
                if (CustomUserFilter.FilterBioRestrictedWords)
                    return CustomUserFilter.BioRestrictedWords.Any(y => user.Biography.ToLower().Contains(y.ToLower()));
                if (UserFilter.FilterBioRestrictedWords)
                    if (UserFilter.BioRestrictedWords.Any(y => user.Biography.ToLower().Contains(y.ToLower())))
                        return true;
                return false;
            }

            public bool IsBioRestrictedWords(UserInfoResponseHandler user)
            {
                if (user.Biography.Length < UserFilter.MinimumCharacterInBio ||
                    UserFilter.BioRestrictedWords.Any(y => user.Biography.ToLower().Contains(y.ToLower())))
                    return true;
                return false;
            }


            public bool FilterBioRestrictedWordsLength(UserInfoResponseHandler user)
            {
                if (!UserFilter.FilterMinimumCharacterInBio)
                    return false;
                return IsBioRestrictedWordsLength(user);
            }

            public bool IsBioRestrictedWordsLength(UserInfoResponseHandler user)
            {
                if (user.Biography.Length < UserFilter.MinimumCharacterInBio)
                    return true;
                return false;
            }

            #endregion

            #region Filter for username blacklist

            public bool FilterUserNameRestrictedWords(UserInfoResponseHandler user)
            {
                if (CustomUserFilter.FilterUserHasInvalidWord)
                    return UserFilter.UserNameRestrictedWords.Any(y => user.Username.ToLower().Contains(y.ToLower()));
                if (UserFilter.FilterUserHasInvalidWord)
                    return UserFilter.UserNameRestrictedWords.Any(y => user.Username.ToLower().Contains(y.ToLower()));
                return false;
            }

            public bool IsUserNameRestrictedWords(UserInfoResponseHandler user)
            {
                if (UserFilter.UserNameRestrictedWords.Any(y => user.Username.ToLower().Contains(y.ToLower())))
                    return true;
                return false;
            }

            #endregion

            #region Filter for Locationblacklist

            public bool FilterLocationRestrictedWords(UserInfoResponseHandler user)
            {
                if (CustomUserFilter.FilterBlacklistedLivesIn)
                    return CustomUserFilter.BlacklistedLivesInPlaces.Any(y =>
                        user.Location.ToLower().Contains(y.ToLower()));
                if (UserFilter.FilterBlacklistedLivesIn)
                    return UserFilter.BlacklistedLivesInPlaces.Any(y => user.Location.ToLower().Contains(y.ToLower()));
                return false;
            }

            public bool IsUserLocationRestrictedWords(UserInfoResponseHandler user)
            {
                if (UserFilter.BlacklistedLivesInPlaces.Any(y => user.Location.ToLower().Contains(y.ToLower())))
                    return true;
                return false;
            }

            #endregion

            #region Filter for studyplace blacklist

            public bool FilterStudyPlaceRestrictedWords(UserInfoResponseHandler user)
            {
                if (CustomUserFilter.FilterBlacklistedStudiedPlaces)
                    return CustomUserFilter.BlacklistedStudiedPlaces.Any(y =>
                        user.StudiedAt.ToLower().Contains(y.ToLower()));
                if (UserFilter.FilterBlacklistedStudiedPlaces)
                    return UserFilter.BlacklistedStudiedPlaces.Any(y => user.StudiedAt.ToLower().Contains(y.ToLower()));
                return false;
            }

            public bool IsUserStudyPlaceRestrictedWords(UserInfoResponseHandler user)
            {
                if (UserFilter.BlacklistedStudiedPlaces.Any(y => user.StudiedAt.ToLower().Contains(y.ToLower())))
                    return true;
                return false;
            }

            #endregion

            #region Filter for workplace

            public bool FilterWorkPlaceRestrictedWords(UserInfoResponseHandler user)
            {
                if (CustomUserFilter.FilterBlacklistedWorkPlaces)
                    return CustomUserFilter.BlacklistedWorkPlaces.Any(y => user.WorkAt.ToLower().Contains(y.ToLower()));
                if (UserFilter.FilterBlacklistedWorkPlaces)
                    return UserFilter.BlacklistedWorkPlaces.Any(y => user.WorkAt.ToLower().Contains(y.ToLower()));
                return false;
            }

            public bool IsUserWorkPlaceRestrictedWords(UserInfoResponseHandler user)
            {
                if (UserFilter.BlacklistedWorkPlaces.Any(y => user.Location.ToLower().Contains(y.ToLower())))
                    return true;
                return false;
            }

            #endregion

            #region Filter for blacklist users

            public void FilterBlacklistedUsers(List<QuoraUser> users, IEnumerable<BlacklistedUser> privateBlacklist,
                IEnumerable<BlacklistedUser> groupBlacklist)
            {
                if (UserFilter.RestrictedProfileList.FilterBlacklist)
                    users.RemoveAll(x => IsProfileBlackList(privateBlacklist, x));

                if (UserFilter.RestrictedGrouplist.FilterBlacklist)
                    users.RemoveAll(x => IsGroupBlackList(groupBlacklist, x));
            }

            public bool FilterBlacklistedUsers(QuoraUser user, IEnumerable<BlacklistedUser> privateBlacklist,
                IEnumerable<BlacklistedUser> groupBlacklist)
            {
                if (UserFilter.RestrictedProfileList.FilterBlacklist)
                    return IsProfileBlackList(privateBlacklist, user);
                if (UserFilter.RestrictedGrouplist.FilterBlacklist)
                    return IsGroupBlackList(groupBlacklist, user);
                return false;
            }

            private bool IsProfileBlackList(IEnumerable<BlacklistedUser> privateBlacklist, QuoraUser x)
            {
                return privateBlacklist.Any(y =>
                {
                    if (y.Pk == null)
                        return y.Username == x.Username;
                    return y.Pk == x.Pk;
                });
            }

            private bool IsGroupBlackList(IEnumerable<BlacklistedUser> groupBlacklist, QuoraUser x)
            {
                return groupBlacklist.Any(y =>
                {
                    if (y.Pk == null)
                        return y.Username == x.Username;
                    return y.Pk == x.Pk;
                });
            }

            #endregion

            #region Fitler with follower count is in range

            public void FilterFollowers(List<QuoraUser> users)
            {
                if (UserFilter.FilterFollowersCount)
                    for (var index = DetailedInfo.Count - 1; index >= 0; --index)
                    {
                        var userInfoResponseHandler = DetailedInfo[index];
                        if (!IsFollowerCountIsInRange(userInfoResponseHandler))
                        {
                            users.Remove(userInfoResponseHandler);
                            DetailedInfo.RemoveAt(index);
                        }
                    }
            }

            public bool FilterFollowers(QuoraUser quorauser)
            {
                if (CustomUserFilter.FilterFollowersCount)
                    return !CustomUserFilter.FollowersCount.InRange(quorauser.FollowerCount);
                if (UserFilter.FilterFollowersCount)
                    return !UserFilter.FollowersCount.InRange(quorauser.FollowerCount);
                return false;
            }

            private bool
                IsFollowerCountIsInRange(
                    QuoraUser quorauser) //UsernameInfoIgResponseHandler UsernameInfoIgResponseHandler
            {
                return UserFilter.FollowersCount.InRange(quorauser.FollowerCount);
            }

            #endregion

            #region Filter with following counts is in range

            public void FilterFollowings(List<QuoraUser> users)
            {
                if (!UserFilter.FilterFollowingsCount)
                    return;
                for (var index = DetailedInfo.Count - 1; index >= 0; --index)
                {
                    var userInfoResponseHandler = DetailedInfo[index];
                    if (!IsFollowingCountsIsInRange(userInfoResponseHandler))
                    {
                        users.Remove(userInfoResponseHandler);
                        DetailedInfo.RemoveAt(index);
                    }
                }
            }

            public bool FilterFollowings(QuoraUser quorauser)
            {
                if (CustomUserFilter.FilterFollowingsCount)
                    return !CustomUserFilter.FollowingsCount.InRange(quorauser.FollowingCount);
                if (UserFilter.FilterFollowingsCount)
                    return !UserFilter.FollowingsCount.InRange(quorauser.FollowingCount);
                return false;
            }

            private bool IsFollowingCountsIsInRange(QuoraUser quorauser)
            {
                return UserFilter.FollowingsCount.InRange(quorauser.FollowingCount);
            }

            #endregion

            #region Filter with user gender

            public void FilterGender(List<QuoraUser> users)
            {
                if (!UserFilter.GenderFilters.IsFilterByGender)
                    return;
                users.RemoveAll(IsUserGenderIgnored);
            }

            public bool FilterGender(QuoraUser user)
            {
                if (!UserFilter.GenderFilters.IsFilterByGender)
                    return false;
                return IsUserGenderIgnored(user);
            }

            public bool IsUserGenderIgnored(QuoraUser user)
            {
                if (!UserFilter.GenderFilters.IsFilterByGender)
                    return false;
                if (UserFilter.GenderFilters.IgnoreMalesUser && user.Gender == Gender.Male)
                    return true;
                if (UserFilter.GenderFilters.IgnoreFemalesUser && user.Gender == Gender.Female)
                    return true;
                if (UserFilter.GenderFilters.IgnoreOthersUser && user.Gender == Gender.Unknown)
                    return true;
                return false;
            }

            #endregion

            #region FilterQuestion

            #region FilterFollowCount

            public bool FilterQuestionFollower(QuestionDetailsResponseHandler questionresponsehandler)
            {
                if (CustomQuestionFilter.FilterQuestionFollowers)
                    return !CustomQuestionFilter.QuestionFollowersCount.InRange(questionresponsehandler.FollowCount);
                if (QuestionFilter.FilterQuestionFollowers)
                    return !QuestionFilter.QuestionFollowersCount.InRange(questionresponsehandler.FollowCount);
                return false;
            }

            #endregion

            #region FilterViewCount

            public bool FilterQuestionView(QuestionDetailsResponseHandler questionresponsehandler)
            {
                if (CustomQuestionFilter.FilterViewsCount)
                    return !CustomQuestionFilter.ViewsCount.InRange(questionresponsehandler.ViewCount);
                if (QuestionFilter.FilterViewsCount)
                    return !QuestionFilter.ViewsCount.InRange(questionresponsehandler.ViewCount);
                return false;
            }

            #endregion

            #region FilterCommentCount

            public bool FilterQuestionComment(QuestionDetailsResponseHandler questionresponsehandler)
            {
                if (CustomQuestionFilter.FilterCommentsCount)
                    return !CustomQuestionFilter.CommentsCount.InRange(questionresponsehandler.CommentCount);
                if (QuestionFilter.FilterCommentsCount)
                    return !QuestionFilter.CommentsCount.InRange(questionresponsehandler.CommentCount);
                return false;
            }

            #endregion

            #region FilterAnswerCount

            public bool FilterQuestionAnswer(QuestionDetailsResponseHandler questionresponsehandler)
            {
                if (CustomQuestionFilter.FilterAnswersCount)
                    return !CustomQuestionFilter.AnswersCount.InRange(questionresponsehandler.AnswerCount);
                if (QuestionFilter.FilterAnswersCount)
                    return !QuestionFilter.AnswersCount.InRange(questionresponsehandler.AnswerCount);
                return false;
            }

            #endregion

            #region FilterAnswerCount

            //public bool FilterQuestionAskDay(QuestionDetailsResponseHandler questionresponsehandler)
            //{
            //    if (CustomQuestionFilter.FilterAskedInSpacesCount)
            //        return !CustomQuestionFilter.AskedInSpacesCount.InRange(questionresponsehandler.LastAskDayCount);
            //    if (QuestionFilter.FilterAskedInSpacesCount)
            //        return !QuestionFilter.AskedInSpacesCount.InRange(questionresponsehandler.LastAskDayCount);
            //    return false;
            //}

            #endregion

            #region FilterQuestionAskSpaces

            public bool FilterQuestionAskSpaces(QuestionDetailsResponseHandler questionresponsehandler)
            {
                if (CustomQuestionFilter.FilterAskedInSpacesCount)
                    return !CustomQuestionFilter.AskedInSpacesCount.InRange(questionresponsehandler.LastAskDayCount);
                if (QuestionFilter.FilterAskedInSpacesCount)
                    return !QuestionFilter.AskedInSpacesCount.InRange(questionresponsehandler.AskedInSpacesCount);
                return false;
            }

            #endregion

            #region FilterAnswerCount

            public bool FilterQuestionLocked(QuestionDetailsResponseHandler questionresponsehandler)
            {
                if (CustomQuestionFilter.FilterLockedStatus)
                    return !questionresponsehandler.QuestionLocked;
                if (QuestionFilter.FilterLockedStatus && questionresponsehandler.QuestionLocked)
                    return true;
                return false;
            }

            #endregion

            #endregion

            #region FilterAnswer

            #region FilterViewCount

            public bool FilterAnswerView(AnswerDetailsResponseHandler answerResponseHandler)
            {
                if (CustomAnswerFilter.FilterViewsCount)
                    return !CustomAnswerFilter.ViewsCount.InRange(answerResponseHandler.AnswerView);
                if (AnswerFilter.FilterViewsCount)
                    return !AnswerFilter.ViewsCount.InRange(answerResponseHandler.AnswerView);
                return false;
            }

            #endregion

            #region FilterAnswerDay

            public bool FilterAnswerDay(AnswerDetailsResponseHandler answerResponseHandler)
            {
                if (CustomAnswerFilter.FilterAnsweredDays)
                    return !CustomAnswerFilter.AnsweredInLastDays.InRange(answerResponseHandler.AnsweredLast);
                if (AnswerFilter.FilterAnsweredDays)
                    return !AnswerFilter.AnsweredInLastDays.InRange(answerResponseHandler.AnsweredLast);
                return false;
            }

            #endregion

            #region FilterUpvoteCount

            public bool FilterAnswerCount(AnswerDetailsResponseHandler answerResponseHandler)
            {
                if (CustomAnswerFilter.FilterUpvoteCounts)
                    return !CustomAnswerFilter.UpvoteCount.InRange(answerResponseHandler.UpvoteCount);
                if (AnswerFilter.FilterUpvoteCounts)
                    return !AnswerFilter.UpvoteCount.InRange(answerResponseHandler.UpvoteCount);
                return false;
            }

            #endregion

            #endregion


            #region Filter if user has not a profile picture

            /// <summary>
            ///     Filter if user has not a profile picture
            /// </summary>
            /// <param name="users"></param>
            public void FilterProfileUsers(List<QuoraUser> users)
            {
                users.RemoveAll(IsAnonymousProfilePicture);
            }

            public bool FilterProfileUsers(QuoraUser x)
            {
                if (!UserFilter.IgnoreNoProfilePicUsers)
                    return false;
                return IsAnonymousProfilePicture(x);
            }

            public bool IsAnonymousProfilePicture(QuoraUser x)
            {
                if (x.HasAnonymousProfilePicture != null && UserFilter.IgnoreNoProfilePicUsers &&
                    !x.HasAnonymousProfilePicture.Value)
                    return true;
                return false;
            }


            public void FilterProfileUsersAdvanced(List<QuoraUser> users)
            {
                if ((!UserFilter.IgnoreNoProfilePicUsers ? 0 :
                        users.Any(x => !x.HasAnonymousProfilePicture.HasValue) ? 1 : 0) == 0)
                    return;
                for (var index = DetailedInfo.Count - 1; index >= 0; --index)
                {
                    var userInfoResponseHandler = DetailedInfo[index];

                    if (IsAnonymousProfilePicture(userInfoResponseHandler))
                    {
                        users.Remove(userInfoResponseHandler);
                        DetailedInfo.RemoveAt(index);
                    }
                }
            }

            #endregion


            #region Filter with white list users

            /// <summary>
            ///     Filter with white list users
            /// </summary>
            /// <param name="users"></param>
            /// <param name="whitelistUsers"></param>
            public void FilterWhitelist(List<QuoraUser> users, IEnumerable<BlacklistedUser> whitelistUsers)
            {
                users.RemoveAll(x => IsWhiteListUser(whitelistUsers, x));
            }

            public bool FilterWhitelist(QuoraUser quoraUser, IEnumerable<BlacklistedUser> whitelistUsers)
            {
                return IsWhiteListUser(whitelistUsers, quoraUser);
            }

            /// <summary>
            ///     check if user is whitelisted
            /// </summary>
            /// <param name="whitelistUsers"></param>
            /// <param name="instaUser"></param>
            /// <returns></returns>
            private bool IsWhiteListUser(IEnumerable<BlacklistedUser> whitelistUsers, QuoraUser instaUser)
            {
                return whitelistUsers.Contains(instaUser);
            }

            #endregion
        }
    }
}