using DominatorUIUtility.Views.AccountSetting;
using DominatorUIUtility.Views.AccountSetting.Activity;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace DominatorUIUtility.Module
{
    public class UiModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<SelectActivity>();
            containerRegistry.RegisterForNavigation<Follow>();
            containerRegistry.RegisterForNavigation<Unfollow>();
            containerRegistry.RegisterForNavigation<Like>();
            containerRegistry.RegisterForNavigation<Comment>();

            containerRegistry.RegisterForNavigation<Unlike>();
            containerRegistry.RegisterForNavigation<DeleteComment>();
            containerRegistry.RegisterForNavigation<Post>();
            containerRegistry.RegisterForNavigation<DeletePost>();
            containerRegistry.RegisterForNavigation<UserScraper>();
            containerRegistry.RegisterForNavigation<DownloadScraper>();
            containerRegistry.RegisterForNavigation<Reposter>();
            containerRegistry.RegisterForNavigation<Retweet>();
            containerRegistry.RegisterForNavigation<QuestionsScraper>();
            containerRegistry.RegisterForNavigation<AnswersScraper>();
            containerRegistry.RegisterForNavigation<VoteAnswers>();
            containerRegistry.RegisterForNavigation<DownvoteAnswers>();
            containerRegistry.RegisterForNavigation<ReportQuestions>();
            containerRegistry.RegisterForNavigation<ReportAnswers>();
            containerRegistry.RegisterForNavigation<ReportUsers>();
            containerRegistry.RegisterForNavigation<BoardScraper>();
            containerRegistry.RegisterForNavigation<PinScraper>();
            containerRegistry.RegisterForNavigation<SendFriendRequest>();
            containerRegistry.RegisterForNavigation<WithdrawSentRequest>();
            containerRegistry.RegisterForNavigation<Unfriend>();
            containerRegistry.RegisterForNavigation<GroupScraper>();
            containerRegistry.RegisterForNavigation<FanpageScraper>();
            containerRegistry.RegisterForNavigation<CommentScraper>();
            containerRegistry.RegisterForNavigation<PostScraper>();
            containerRegistry.RegisterForNavigation<GroupJoiner>();
            containerRegistry.RegisterForNavigation<GroupUnJoiner>();
            containerRegistry.RegisterForNavigation<GroupInviter>();
            containerRegistry.RegisterForNavigation<PageInviter>();
            containerRegistry.RegisterForNavigation<EventInviter>();
            containerRegistry.RegisterForNavigation<GroupCreator>();
            containerRegistry.RegisterForNavigation<EventCreator>();
            containerRegistry.RegisterForNavigation<Tweet>();
            containerRegistry.RegisterForNavigation<ProfileScraper>();
            containerRegistry.RegisterForNavigation<DownvoteQuestions>();
            containerRegistry.RegisterForNavigation<UpvoteAnswers>();
            containerRegistry.RegisterForNavigation<Join>();
            containerRegistry.RegisterForNavigation<Unjoin>();
            containerRegistry.RegisterForNavigation<PostLikerCommentor>();
            containerRegistry.RegisterForNavigation<FanpageLiker>();
            containerRegistry.RegisterForNavigation<WebpageLikerCommentor>();
            containerRegistry.RegisterForNavigation<TweetScraper>();
            containerRegistry.RegisterForNavigation<MakeAdmin>();
            containerRegistry.RegisterForNavigation<ConnectionRequest>();
            containerRegistry.RegisterForNavigation<Subscribe>();
            containerRegistry.RegisterForNavigation<Share>();
            containerRegistry.RegisterForNavigation<UnSubscribe>();
            containerRegistry.RegisterForNavigation<ViewIncreaser>();
            containerRegistry.RegisterForNavigation<BlockFollower>();
            containerRegistry.RegisterForNavigation<LikeComment>();
            containerRegistry.RegisterForNavigation<HashtagsScraper>();
            containerRegistry.RegisterForNavigation<CreateBoard>();
            containerRegistry.RegisterForNavigation<FollowBack>();
            containerRegistry.RegisterForNavigation<DeleteTweet>();
            containerRegistry.RegisterForNavigation<Mute>();
            containerRegistry.RegisterForNavigation<CommunityScraper>();
            containerRegistry.RegisterForNavigation<JobScraper>();


            containerRegistry.RegisterForNavigation<CompanyScraper>();
            containerRegistry.RegisterForNavigation<GroupMemberScraper>();
            containerRegistry.RegisterForNavigation<SalesNavigatorUserScraper>();
            containerRegistry.RegisterForNavigation<ChannelScraper>();
            containerRegistry.RegisterForNavigation<UnSubscribe>();
            containerRegistry.RegisterForNavigation<BroadcastMessages>();
            containerRegistry.RegisterForNavigation<SendMessageToFollower>();
            containerRegistry.RegisterForNavigation<AutoReplyToNewMessage>();
            containerRegistry.RegisterForNavigation<AcceptConnectionRequest>();
            containerRegistry.RegisterForNavigation<RemoveConnections>();
            containerRegistry.RegisterForNavigation<ProfileEndorsement>();
            containerRegistry.RegisterForNavigation<ReplyToComment>();
            containerRegistry.RegisterForNavigation<Reblog>();
            containerRegistry.RegisterForNavigation<Try>();
            containerRegistry.RegisterForNavigation<SendMessageToNewConnection>();
            containerRegistry.RegisterForNavigation<UrlScraper>();
            containerRegistry.RegisterForNavigation<SendGreetingsToConnections>();
            containerRegistry.RegisterForNavigation<Reply>();
            containerRegistry.RegisterForNavigation<Delete>();
            containerRegistry.RegisterForNavigation<IncommingFriendRequest>();
            containerRegistry.RegisterForNavigation<Downvote>();
            containerRegistry.RegisterForNavigation<Upvote>();
            containerRegistry.RegisterForNavigation<DeletePin>();
            containerRegistry.RegisterForNavigation<Repin>();
            containerRegistry.RegisterForNavigation<WithdrawConnectionRequest>();
            containerRegistry.RegisterForNavigation<RemoveVote>();
            containerRegistry.RegisterForNavigation<ExportConnection>();
            containerRegistry.RegisterForNavigation<PostLiker>();
            containerRegistry.RegisterForNavigation<PostCommentor>();
            containerRegistry.RegisterForNavigation<RemoveVoteComment>();
            containerRegistry.RegisterForNavigation<UpvoteComment>();
            containerRegistry.RegisterForNavigation<DownvoteComment>();
            containerRegistry.RegisterForNavigation<Dislike>();
            containerRegistry.RegisterForNavigation<AnswerOnQuestions>();
            containerRegistry.RegisterForNavigation<WelcomeTweet>();
            containerRegistry.RegisterForNavigation<SalesNavigatorCompanyScraper>();
            containerRegistry.RegisterForNavigation<TweetTo>();
            containerRegistry.RegisterForNavigation<SendMessageToNewFriends>();
            containerRegistry.RegisterForNavigation<WatchPartyInviter>();
            containerRegistry.RegisterForNavigation<MarketPlaceScraper>();
            containerRegistry.RegisterForNavigation<SendGreetingsToFriends>();
            containerRegistry.RegisterForNavigation<WebPostLikeComment>();
            containerRegistry.RegisterForNavigation<EditPin>();
            containerRegistry.RegisterForNavigation<AcceptBoardInvitation>();
            containerRegistry.RegisterForNavigation<SendBoardInvitation>();
            containerRegistry.RegisterForNavigation<BlockUser>();
            containerRegistry.RegisterForNavigation<MessageToFanpages>();
            containerRegistry.RegisterForNavigation<MessageToPlaces>();
            containerRegistry.RegisterForNavigation<PlaceScraper>();
            containerRegistry.RegisterForNavigation<EditComment>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("StartupRegion", typeof(SelectActivity));
        }
    }
}