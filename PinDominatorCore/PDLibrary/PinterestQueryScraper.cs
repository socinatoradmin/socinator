using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Enums.PdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using PinDominatorCore.PDEnums;
using PinDominatorCore.PDLibrary.Processors;
using PinDominatorCore.PDLibrary.Processors.AccountCreator;
using PinDominatorCore.PDLibrary.Processors.Board;
using PinDominatorCore.PDLibrary.Processors.Pin;
using PinDominatorCore.PDLibrary.Processors.User;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using System;
using System.Collections.Generic;
using System.Threading;
using Unity;

namespace PinDominatorCore.PDLibrary.Process
{
    public class PinterestQueryScraper : QueryScraper
    {
        public PinterestQueryScraper(IPdJobProcess jobProcess,
            Dictionary<string, Action<QueryInfo>> scrapeWithQueriesActionTable,
            Dictionary<string, Action> scrapeWithoutQueriesActionTable)
            : base(jobProcess, scrapeWithQueriesActionTable, scrapeWithoutQueriesActionTable)
        {
        }
    }

    public interface IPinterestScraperActionTables : IScraperActionTables
    {
    }

    public class PinterestScraperActionTables : IPinterestScraperActionTables
    {
        private readonly IPdJobProcess _jobProcess;
        private readonly IUnityContainer _unityContainer;

        public PinterestScraperActionTables(IPdJobProcess pdJobProcess, IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
            _jobProcess = pdJobProcess;
            var activityType = pdJobProcess.ActivityType;
            var scrapeWith = activityType.GetPdElementByActivityType();

            ScrapeWithQueriesActionTable = new Dictionary<string, Action<QueryInfo>>();

            switch (scrapeWith)
            {
                case PdElements.Users:
                    ScrapeWithQueriesActionTable.Add(
                        $"{activityType}{EnumUtility.GetQueryFromEnum(PDUsersQueries.Keywords)}",
                        StartProcessToPerformUserQueriesForKeyword);
                    ScrapeWithQueriesActionTable.Add(
                        $"{activityType}{EnumUtility.GetQueryFromEnum(PDUsersQueries.Customusers)}",
                        StartProcessToPerformUserQueriesForCustomUsers);
                    ScrapeWithQueriesActionTable.Add(
                        $"{activityType}{EnumUtility.GetQueryFromEnum(PDUsersQueries.SomeonesFollowers)}",
                        StartProcessToPerformUserQueriesForSomeonesFollowers);
                    ScrapeWithQueriesActionTable.Add(
                        $"{activityType}{EnumUtility.GetQueryFromEnum(PDUsersQueries.SomeonesFollowings)}",
                        StartProcessToPerformUserQueriesForSomeonesFollowings);
                    ScrapeWithQueriesActionTable.Add(
                        $"{activityType}{EnumUtility.GetQueryFromEnum(PDUsersQueries.FollowersOfSomeonesFollowers)}",
                        StartProcessToPerformUserQueriesForFollowersOfSomeonesFollowers);
                    ScrapeWithQueriesActionTable.Add(
                        $"{activityType}{EnumUtility.GetQueryFromEnum(PDUsersQueries.FollowersOfSomeonesFollowings)}",
                        StartProcessToPerformUserQueriesForFollowersOfSomeonesFollowings);
                    ScrapeWithQueriesActionTable.Add(
                        $"{activityType}{EnumUtility.GetQueryFromEnum(PDUsersQueries.UsersWhoTriedPins)}",
                        StartProcessToPerformUserQueriesForUsersWhoTriedPins);
                    //ScrapeWithQueriesActionTable.Add(
                    //    $"{activityType}{EnumUtility.GetQueryFromEnum(PDUsersQueries.BoardFollowers)}",
                    //    StartProcessToPerformUserQueriesForBoardFollowers);
                    ScrapeWithQueriesActionTable.Add(
                        $"{activityType}{EnumUtility.GetQueryFromEnum(PDUsersQueries.BoardsbyKeywords)}",
                        StartProcessToPerformBoardQueriesForBoardsbyKeywords);
                    ScrapeWithQueriesActionTable.Add(
                        $"{activityType}{EnumUtility.GetQueryFromEnum(PDUsersQueries.CustomBoard)}",
                        StartProcessToPerformBoardQueriesForCustomBoard);

                    break;
                case PdElements.Pin:
                    ScrapeWithQueriesActionTable.Add(
                        $"{activityType}{EnumUtility.GetQueryFromEnum(PDPinQueries.Keywords)}",
                        StartProcessToPerformPinQueriesForKeyword);
                    ScrapeWithQueriesActionTable.Add(
                        $"{activityType}{EnumUtility.GetQueryFromEnum(PDPinQueries.CustomPin)}",
                        StartProcessToPerformPinQueriesForCustomPin);
                    ScrapeWithQueriesActionTable.Add(
                        $"{activityType}{EnumUtility.GetQueryFromEnum(PDUsersQueries.Customusers)}",
                        StartProcessToPerformPinQueriesForCustomUser);
                    ScrapeWithQueriesActionTable.Add(
                        $"{activityType}{EnumUtility.GetQueryFromEnum(PDPinQueries.OwnFollowers)}",
                        StartProcessToPerformPinQueriesForOwnFollowers);
                    ScrapeWithQueriesActionTable.Add(
                        $"{activityType}{EnumUtility.GetQueryFromEnum(PDPinQueries.OwnFollowings)}",
                        StartProcessToPerformPinQueriesForOwnFollowings);
                    ScrapeWithQueriesActionTable.Add(
                        $"{activityType}{EnumUtility.GetQueryFromEnum(PDPinQueries.Newsfeed)}",
                        StartProcessToPerformPinQueriesForNewsFeedPins);
                    ScrapeWithQueriesActionTable.Add(
                        $"{activityType}{EnumUtility.GetQueryFromEnum(PDPinQueries.SocinatorPublisherCampaign)}",
                        StartProcessToPerformPinQueriesForSocinatorPublisherCampaign);
                    ScrapeWithQueriesActionTable.Add(
                        $"{activityType}{EnumUtility.GetQueryFromEnum(PDUsersQueries.CustomBoard)}",
                        StartProcessToPerformPinQueriesForCustomBoard);
                    break;

                case PdElements.Board:
                    ScrapeWithQueriesActionTable.Add(
                        $"{activityType}{EnumUtility.GetQueryFromEnum(PDUsersQueries.Keywords)}",
                        StartProcessToPerformBoardQueriesForBoardsbyKeywords);
                    ScrapeWithQueriesActionTable.Add(
                        $"{activityType}{EnumUtility.GetQueryFromEnum(PDUsersQueries.CustomBoard)}",
                        StartProcessToPerformBoardQueriesForCustomBoard);
                    ScrapeWithQueriesActionTable.Add(
                        $"{activityType}{EnumUtility.GetQueryFromEnum(PDUsersQueries.Customusers)}",
                        StartProcessToPerformBoardQueriesForCustomUsers);
                    break;
            }

            ScrapeWithoutQueriesActionTable = new Dictionary<string, Action>
            {
                {"Unfollow", StarProcessForUnfollow},
                {"CreateBoard", StartProcessForCreateBoard},
                {"FollowBack", StartProcessForFollowBack},
                {"AutoReplyToNewMessage", StartProcessForAutoReplyToNewMessage},
                {"SendMessageToFollower", StartProcessForSendMessageToFollower},
                {"DeletePin", StartProcessForDeletePin},
                {"EditPin", StartProcessForEditPin},
                {"AcceptBoardInvitation", StartProcessForAcceptBoardInvitations},
                {"SendBoardInvitation", StartProcessForSendBoardInvitations},
                {"CreateAccount", StartProcessToCreateAccount}
            };
        }


        public CancellationToken CancellationToken { get; set; }

        public Dictionary<string, Action<QueryInfo>> ScrapeWithQueriesActionTable { get; set; }
        public Dictionary<string, Action> ScrapeWithoutQueriesActionTable { get; set; }

        #region Database instance    

        public List<Func<PinterestUser, bool>> FilterUserActionList { get; set; } =
            new List<Func<PinterestUser, bool>>();

        #endregion

        #region Start Process For User Queries


        private void StartProcessToPerformUserQueriesForKeyword(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<KeywordUserProcessor>();
            processor.Start(queryInfo);
        }

        private void StartProcessToPerformUserQueriesForCustomUsers(QueryInfo queryInfo)
        {
            _jobProcess.DominatorAccountModel.IsNeedToSchedule = false;
            IQueryProcessor processor = _unityContainer.Resolve<CustomUserProcessor>();
            processor.Start(queryInfo);
        }

        private void StartProcessToPerformUserQueriesForSomeonesFollowers(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<SomeonesFollowersUserProcessor>();
            processor.Start(queryInfo);
        }

        private void StartProcessToPerformUserQueriesForSomeonesFollowings(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<SomeonesFollowingsUserProcessor>();
            processor.Start(queryInfo);
        }

        private void StartProcessToPerformUserQueriesForFollowersOfSomeonesFollowers(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<FollowersOfSomeonesFollowersUserProcessor>();
            processor.Start(queryInfo);
        }

        private void StartProcessToPerformUserQueriesForFollowersOfSomeonesFollowings(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<FollowersOfSomeonesFollowingsUserProcessor>();
            processor.Start(queryInfo);
        }

        private void StartProcessToPerformUserQueriesForUsersWhoTriedPins(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<UsersWhoTriedPinsUserProcessor>();
            processor.Start(queryInfo);
        }

        private void StartProcessToPerformUserQueriesForBoardFollowers(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<BoardFollowersUserProcessor>();
            processor.Start(queryInfo);
        }

        #endregion

        #region Start Process For Pin Queries

        private void StartProcessToPerformPinQueriesForKeyword(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<KeywordPinProcessor>();
            processor.Start(queryInfo);
        }

        private void StartProcessToPerformPinQueriesForCustomPin(QueryInfo queryInfo)
        {
            _jobProcess.DominatorAccountModel.IsNeedToSchedule = false;
            IQueryProcessor processor = _unityContainer.Resolve<CustomPinProcessor>();
            processor.Start(queryInfo);
        }

        private void StartProcessToPerformPinQueriesForCustomUser(QueryInfo queryInfo)
        {
            _jobProcess.DominatorAccountModel.IsNeedToSchedule = false;
            IQueryProcessor processor = _unityContainer.Resolve<CustomUserPinProcessor>();
            processor.Start(queryInfo);
        }

        private void StartProcessToPerformPinQueriesForOwnFollowers(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<OwnFollowersPinProcessor>();
            processor.Start(queryInfo);
        }

        private void StartProcessToPerformPinQueriesForOwnFollowings(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<OwnFollowingsPinProcessor>();
            processor.Start(queryInfo);
        }

        private void StartProcessToPerformPinQueriesForNewsFeedPins(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<NewsFeedPinProcessor>();
            processor.Start(queryInfo);
        }

        private void StartProcessToPerformPinQueriesForSocinatorPublisherCampaign(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<SocinatorPublisherCampaignPinProcessor>();
            processor.Start(queryInfo);
        }

        private void StartProcessToPerformPinQueriesForCustomBoard(QueryInfo queryInfo)
        {
            _jobProcess.DominatorAccountModel.IsNeedToSchedule = false;
            IQueryProcessor processor = _unityContainer.Resolve<CustomBoardPinProcessor>();
            processor.Start(queryInfo);
        }

        #endregion

        #region Start Process For Board Queries

        private void StartProcessToPerformBoardQueriesForBoardsbyKeywords(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<BoardsbyKeywordsBoardProcessor>();
            processor.Start(queryInfo);
        }

        private void StartProcessToPerformBoardQueriesForCustomBoard(QueryInfo queryInfo)
        {
            _jobProcess.DominatorAccountModel.IsNeedToSchedule = false;
            IQueryProcessor processor = _unityContainer.Resolve<CustomBoardProcessor>();
            processor.Start(queryInfo);
        }

        private void StartProcessToPerformBoardQueriesForCustomUsers(QueryInfo queryInfo)
        {
            _jobProcess.DominatorAccountModel.IsNeedToSchedule = false;
            IQueryProcessor processor = _unityContainer.Resolve<CustomUserBoardProcessor>();
            processor.Start(queryInfo);
        }

        #endregion

        #region Start Process For Without Queries

        private void StarProcessForUnfollow()
        {
            IQueryProcessor processor = _unityContainer.Resolve<UnfollowProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }

        private void StartProcessForCreateBoard()
        {
            IQueryProcessor processor = _unityContainer.Resolve<CreateBoardProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }

        private void StartProcessForFollowBack()
        {
            IQueryProcessor processor = _unityContainer.Resolve<FollowBackProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }

        private void StartProcessForAutoReplyToNewMessage()
        {
            IQueryProcessor processor = _unityContainer.Resolve<AutoReplyToNewMessageProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }

        private void StartProcessForSendMessageToFollower()
        {
            IQueryProcessor processor = _unityContainer.Resolve<SendMessageToFollowerProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }

        private void StartProcessForDeletePin()
        {
            IQueryProcessor processor = _unityContainer.Resolve<DeletePinProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }

        private void StartProcessForEditPin()
        {
            IQueryProcessor processor = _unityContainer.Resolve<EditPinProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }

        private void StartProcessForAcceptBoardInvitations()
        {
            IQueryProcessor processor = _unityContainer.Resolve<AcceptBoardInvitationsProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }

        private void StartProcessForSendBoardInvitations()
        {
            IQueryProcessor processor = _unityContainer.Resolve<SendBoardInvitationsProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }

        private void StartProcessToCreateAccount()
        {
            IQueryProcessor processor = _unityContainer.Resolve<BaseAccountCreatorProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }
        #endregion
    }
}