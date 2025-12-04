using DominatorHouseCore.Enums;
using DominatorHouseCore.Process;
using LinkedDominatorCore.LDLibrary.EngageProcesses;
using LinkedDominatorCore.LDLibrary.GroupProcesses;
using LinkedDominatorCore.LDLibrary.GrowConnectionProcesses;
using LinkedDominatorCore.LDLibrary.MessengerProcesses;
using LinkedDominatorCore.LDLibrary.ScraperProcesses;
using Unity;
using Unity.Extension;
using Unity.Lifetime;

namespace LinkedDominatorCore.LDLibrary
{
    public class LinkedInJobProcessUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            var socialNetwork = SocialNetworks.LinkedIn;
            Container.RegisterType<IJobProcess, CommentProcess>($"{socialNetwork}{ActivityType.Comment}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, LikeProcess>($"{socialNetwork}{ActivityType.Like}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, ShareProcess>($"{socialNetwork}{ActivityType.Share}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, GroupJoinerProcess>($"{socialNetwork}{ActivityType.GroupJoiner}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, GroupUnJoinerProcess>($"{socialNetwork}{ActivityType.GroupUnJoiner}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, AcceptConnectionRequestProcess>(
                $"{socialNetwork}{ActivityType.AcceptConnectionRequest}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, ConnectionRequestProcess>(
                $"{socialNetwork}{ActivityType.ConnectionRequest}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, ExportConnectionProcess>(
                $"{socialNetwork}{ActivityType.ExportConnection}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, CommentProcess>($"{socialNetwork}{ActivityType.Comment}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, RemoveConnectionProcess>(
                $"{socialNetwork}{ActivityType.RemoveConnections}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, WithdrawConnectionRequestProcess>(
                $"{socialNetwork}{ActivityType.WithdrawConnectionRequest}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, BlockUserProcess>($"{socialNetwork}{ActivityType.BlockUser}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, AutoReplyToNewMessageProcess>(
                $"{socialNetwork}{ActivityType.AutoReplyToNewMessage}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, BroadcastMessagesProcess>(
                $"{socialNetwork}{ActivityType.BroadcastMessages}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, SendGreetingsToConnectionsProcess>(
                $"{socialNetwork}{ActivityType.SendGreetingsToConnections}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, SendMessageToNewConnectionProcess>(
                $"{socialNetwork}{ActivityType.SendMessageToNewConnection}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, ProfileEndorsementProcess>(
                $"{socialNetwork}{ActivityType.ProfileEndorsement}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, SalesNavigatorScraperProcesses.CompanyScraperProcess>(
                $"{socialNetwork}{ActivityType.SalesNavigatorCompanyScraper}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, SalesNavigatorScraperProcesses.UserScraperProcess>(
                $"{socialNetwork}{ActivityType.SalesNavigatorUserScraper}",
                new HierarchicalLifetimeManager());

            Container.RegisterType<IJobProcess, CompanyScraperProcess>($"{socialNetwork}{ActivityType.CompanyScraper}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, UserScraperProcess>($"{socialNetwork}{ActivityType.UserScraper}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, JobScraperProcess>($"{socialNetwork}{ActivityType.JobScraper}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, DeleteConversationsProcess>(
                $"{socialNetwork}{ActivityType.DeleteConversations}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, GroupInviterProcess>($"{socialNetwork}{ActivityType.GroupInviter}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, MessageConversationProcess>(
                $"{socialNetwork}{ActivityType.AttachmnetsMessageScraper}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, FollowPageProcess>($"{socialNetwork}{ActivityType.FollowPages}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, SendPageInvitationProcess>(
                $"{socialNetwork}{ActivityType.SendPageInvitations}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, SendPageInvitationProcess>(
                $"{socialNetwork}{ActivityType.EventInviter}",
                new HierarchicalLifetimeManager());
            Container.RegisterType<IJobProcess, SendGroupMemberInvitationProcess>(
                $"{socialNetwork}{ActivityType.SendGroupInvitations}",
                new HierarchicalLifetimeManager());

        }
    }
}