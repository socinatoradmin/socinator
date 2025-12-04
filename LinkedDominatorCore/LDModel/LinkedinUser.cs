using System.Collections.Generic;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.Interfaces;

namespace LinkedDominatorCore.LDModel
{
    public class LinkedinUser : IUser
    {
        public LinkedinUser()
        {
        }


        public LinkedinUser(string PublicIdentifier)
        {
            this.PublicIdentifier = PublicIdentifier;
            ProfileUrl = $"https://www.linkedin.com/in/{PublicIdentifier}";
        }

        public string PublicIdentifier { get; set; }

        public string EmailAddress { get; set; }

        public string ProfileId { get; set; }

        public string MemberId { get; set; }

        public string AuthToken { get; set; }

        public string ProfileUrl { get; set; }

        public string SalesNavigatorProfileUrl { get; set; }

        public string IsViewed { get; set; }

        public string TrackingId { get; set; }

        public long ConnectedTimeStamp { get; set; }

        public long RequestedTimeStamp { get; set; }

        public ConnectionType ConnectionType { get; set; }

        public string Occupation { get; set; }

        public string HeadlineTitle { get; set; }

        public string Industry { get; set; }

        public string CompanyName { get; set; }

        public string CurrentCompany { get; set; }

        public string SelectedSource { get; set; }

        public string Location { get; set; }

        public string DetailedUserInfo { get; set; }

        public string NumberOfSharedConnections { get; set; }

        public string InvitationId { get; set; }

        public string InvitationSharedSecret { get; set; }

        public bool? HasAnonymousProfilePicture { get; set; }
        public bool IsFollowing {  get; set; }

        public string MessageContent { get; set; }

        public string SelectedMessageFilter { get; set; }

        public string MessageThreadId { get; set; }

        public string NotificationType { get; set; }

        public string UniqueNotificationSuffix { get; set; }
        public string SessionId { get; set; }
        public string NodeId { get; set; }
        public string NodeResponse { get; set; }
        public string ShareNodeId { get; set; }
        public string PostData { get; set; }
        public string AttachmentId { get; set; }

        public string ConnectedTime { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public List<KeyValuePair<string, string>>
            attachmentDetails { get; set; } =
            new List<KeyValuePair<string, string>>();

        public List<KeyValuePair<string, string>> FileNameAndUrls { get; set; }
            = new List<KeyValuePair<string, string>>();
        public bool IsPremium { get; set; }

        public Dictionary<string, string> ActivityTypeWithNameOrId { get; set; } =
            new Dictionary<string, string>();

        public string FullName { get; set; }

        public string ProfilePicUrl { get; set; }

        #region Not Required

        public string UserId { get; set; }

        public string Username { get; set; }

        #endregion

        #region Account Info

        public string AccountUserProfileUrl { get; set; }
        public string AccountUserFullName { get; set; }

        #endregion
    }
}