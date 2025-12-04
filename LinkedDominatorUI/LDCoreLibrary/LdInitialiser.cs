using DominatorHouseCore.Enums;
using LinkedDominatorCore.Utility;
using LinkedDominatorUI.Utility.Engage;
using LinkedDominatorUI.Utility.Group;
using LinkedDominatorUI.Utility.GrowConnectionBaseFactories;
using LinkedDominatorUI.Utility.GrowConnectionReports;
using LinkedDominatorUI.Utility.MessengerBaseFactories;
using LinkedDominatorUI.Utility.Profilling;
using LinkedDominatorUI.Utility.SalesNavigatorScraper;
using LinkedDominatorUI.Utility.Scraper;

namespace LinkedDominatorUI.LDCoreLibrary
{
    public class LdInitialiser
    {
        public static void RegisterModules()
        {
            #region GrowConnection

            LinkedInInitialize.LdModulesRegister(ActivityType.ConnectionRequest, new ConnectionRequestBaseFactory());
            LinkedInInitialize.LdModulesRegister(ActivityType.AcceptConnectionRequest,
                new AcceptConnectionRequestBaseFactory());
            LinkedInInitialize.LdModulesRegister(ActivityType.RemoveConnections, new RemoveConnectionsBaseFactory());
            LinkedInInitialize.LdModulesRegister(ActivityType.WithdrawConnectionRequest,
                new WithdrawConnectionRequestBaseFactory());
            LinkedInInitialize.LdModulesRegister(ActivityType.ExportConnection, new ExportConnectionBaseFactory());
            LinkedInInitialize.LdModulesRegister(ActivityType.FollowPages, new FollowPageBaseFactory());
            LinkedInInitialize.LdModulesRegister(ActivityType.SendPageInvitations, new SendPageInvitationBaseFactory());
            LinkedInInitialize.LdModulesRegister(ActivityType.EventInviter, new SendEventInvitationBaseFactory());
            LinkedInInitialize.LdModulesRegister(ActivityType.SendGroupInvitations, new SendGroupMemberInvitationBaseFactory());
            LinkedInInitialize.LdModulesRegister(ActivityType.BlockUser, new BlockUserBaseFactory());

            #endregion

            #region Messenger

            LinkedInInitialize.LdModulesRegister(ActivityType.BroadcastMessages, new BroadcastMessagesBaseFactory());
            LinkedInInitialize.LdModulesRegister(ActivityType.AutoReplyToNewMessage,
                new AutoReplyToNewMessageBaseFactory());
            LinkedInInitialize.LdModulesRegister(ActivityType.SendMessageToNewConnection,
                new SendMessageToNewConnectionBaseFactory());
            LinkedInInitialize.LdModulesRegister(ActivityType.SendGreetingsToConnections,
                new SendGreetingsToConnectionsBaseFactory());
            LinkedInInitialize.LdModulesRegister(ActivityType.DeleteConversations, new DeleteConversationBaseFactory());

            #endregion

            #region Engage

            LinkedInInitialize.LdModulesRegister(ActivityType.Like, new LikeBaseFactory());
            LinkedInInitialize.LdModulesRegister(ActivityType.Comment, new CommentBaseFactory());
            LinkedInInitialize.LdModulesRegister(ActivityType.Share, new ShareBaseFactory());

            #endregion

            #region Group

            LinkedInInitialize.LdModulesRegister(ActivityType.GroupJoiner, new GroupJoinerBaseFactory());
            LinkedInInitialize.LdModulesRegister(ActivityType.GroupUnJoiner, new GroupUnJoinerBaseFactory());
            LinkedInInitialize.LdModulesRegister(ActivityType.GroupInviter, new GroupInviterBaseFactory());

            #endregion

            #region Scraper

            LinkedInInitialize.LdModulesRegister(ActivityType.UserScraper, new UserScraperBaseFactory());
            LinkedInInitialize.LdModulesRegister(ActivityType.SalesNavigatorUserScraper, new SalesUserScraperFactory());
            LinkedInInitialize.LdModulesRegister(ActivityType.SalesNavigatorCompanyScraper,
                new SalesNavigatorCompanyScraperFactory());
            LinkedInInitialize.LdModulesRegister(ActivityType.JobScraper, new JobScraperBaseFactory());
            LinkedInInitialize.LdModulesRegister(ActivityType.CompanyScraper, new CompanyScraperBaseFactory());
            LinkedInInitialize.LdModulesRegister(ActivityType.AttachmnetsMessageScraper,
                new AttachmentsMessageScraperBaseFactory());

            #endregion

            LinkedInInitialize.LdModulesRegister(ActivityType.ProfileEndorsement, new ProfileEndorsementBaseFactory());
        }
    }
}