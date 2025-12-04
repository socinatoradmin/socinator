using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LinkedDominatorCore.LDModel
{
    public class LdJsonElements
    {
        [JsonProperty(PropertyName = "miniProfile")]
        public LdJsonElements MiniProfile { get; set; }

        [JsonProperty(PropertyName = "communityId")]
        public string CommunityId { get; set; }

        [JsonProperty(PropertyName = "Challenge_id")]
        public string ChallengeId { get; set; }

        [JsonProperty(PropertyName = "threadId")]
        public string ThreadId { get; set; }

        #region LinkedDominator JsonElements For login

        [JsonProperty(PropertyName = "session_key")]
        public string SessionKey { get; set; }

        [JsonProperty(PropertyName = "session_password")]
        public string SessionPassword { get; set; }

        [JsonProperty(PropertyName = "isJsEnabled")]
        public string IsJsEnabled { get; set; }

        [JsonProperty(PropertyName = "loginCsrfParam")]
        public string LoginCsrfParam { get; set; }

        [JsonProperty(PropertyName = "login_result")]
        public string LoginResult { get; set; }


        [JsonProperty(PropertyName = "challenge_url")]
        public string ChallengeUrl { get; set; }

        #endregion

        #region for Mobile Login

        [JsonProperty(PropertyName = "JSESSIONID")]
        public string Jsessionid { get; set; }

        [JsonProperty(PropertyName = "bcookie")]
        public string Bcookie { get; set; }

        [JsonProperty(PropertyName = "bscookie")]
        public string Bscookie { get; set; }

        [JsonProperty(PropertyName = "lang")] public string Lang { get; set; }

        [JsonProperty(PropertyName = "lidc")] public string Lidc { get; set; }

        [JsonProperty(PropertyName = "sl")] public string Sl { get; set; }

        [JsonProperty(PropertyName = "li_rm")]
        public string LIRM { get; set; }
        [JsonProperty(PropertyName = "rememberMeOptIn")]
        public bool RememberMe { get; set; }
        [JsonProperty(PropertyName = "visit")] public string Visit { get; set; }

        [JsonProperty(PropertyName = "client_enabled_features")]
        public string ClientEnabledFeatures { get; set; }

        [JsonProperty(PropertyName = "fp_data")]
        public string FpData { get; set; }
        #endregion

        #region For ConnectionRequest

        [JsonProperty(PropertyName = "trackingId")]
        public string TrackingId { get; set; }

        [JsonProperty(PropertyName = "invitee")]
        public string Invitee { get; set; }

        [JsonProperty(PropertyName = "profileId")]
        public string ProfileId { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        #endregion

        #region engage module

        #region like

        [JsonProperty(PropertyName = "actor")] public LdJsonElements Actor { get; set; }

        [JsonProperty(PropertyName = "com.linkedin.voyager.feed.MemberActor")]
        public LdJsonElements ComLinkedinVoyagerFeedMemberActor { get; set; }

        [JsonProperty(PropertyName = "actorType")]
        public string ActorType { get; set; }

        [JsonProperty(PropertyName = "profileRoute")]
        public string ProfileRoute { get; set; }

        [JsonProperty(PropertyName = "emberEntityName")]
        public string EmberEntityName { get; set; }

        [JsonProperty(PropertyName = "objectUrn")]
        public string ObjectUrn { get; set; }

        [JsonProperty(PropertyName = "entityUrn")]
        public string EntityUrn { get; set; }

        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "occupation")]
        public string Occupation { get; set; }

        [JsonProperty(PropertyName = "publicIdentifier")]
        public string PublicIdentifier { get; set; }

        [JsonProperty(PropertyName = "picture")]
        public LdJsonElements Picture { get; set; }

        [JsonProperty(PropertyName = "com.linkedin.common.VectorImage")]
        public LdJsonElements ComLinkedInCommonVectorImage { get; set; }

        [JsonProperty(PropertyName = "id")] public string Id { get; set; }

        [JsonProperty(PropertyName = "rootUrl")]
        public string RootUrl { get; set; }

        [JsonProperty(PropertyName = "artifacts")]
        public List<LdJsonElements> Artifacts { get; set; }

        [JsonProperty(PropertyName = "fileIdentifyingUrlPathSegment")]
        public string FileIdentifyingUrlPathSegment { get; set; }

        [JsonProperty(PropertyName = "height")]
        public int? Height { get; set; }

        [JsonProperty(PropertyName = "width")] public int? Width { get; set; }

        #endregion

        #region share

        //externalAudienceProviders
        [JsonProperty(PropertyName = "externalAudienceProviders")]
        public List<LdJsonElements> ExternalAudienceProviders { get; set; }

        [JsonProperty(PropertyName = "visibleToConnectionsOnly")]
        public bool? VisibleToConnectionsOnly { get; set; }

        [JsonProperty(PropertyName = "commentary")]
        public LdJsonElements Commentary { get; set; }

        [JsonProperty(PropertyName = "attributes")]
        public List<LdJsonElements> Attributes { get; set; }

        [JsonProperty(PropertyName = "text")] public string Text { get; set; }
        
        [JsonProperty(PropertyName = "commentsDisabled")]
        public bool? CommentsDisabled { get; set; }

        [JsonProperty(PropertyName = "rootUrn")]
        public string RootUrn { get; set; }

        [JsonProperty(PropertyName = "parentUrn")]
        public string ParentUrn { get; set; }

        #endregion

        #endregion

        #region withDraw

        //inviteActionType

        [JsonProperty(PropertyName = "inviteActionType")]
        public string InviteActionType { get; set; }

        [JsonProperty(PropertyName = "inviteActionData")]
        public List<LdJsonElements> InviteActionData { get; set; }

        [JsonProperty(PropertyName = "validationToken")]
        public string ValidationToken { get; set; }

        [JsonProperty(PropertyName = "genericInvitation")]
        public bool? GenericInvitation { get; set; }

        #endregion

        #region message

        [JsonProperty(PropertyName = "keyVersion")]
        public string KeyVersion { get; set; }

        [JsonProperty(PropertyName = "conversationCreate")]
        public LdJsonElements ConversationCreate { get; set; }

        [JsonProperty(PropertyName = "eventCreate")]
        public LdJsonElements EventCreate { get; set; }

        [JsonProperty(PropertyName = "originToken")]
        public string OriginToken { get; set; }

        [JsonProperty(PropertyName = "value")] public LdJsonElements Value { get; set; }

        [JsonProperty(PropertyName = "com.linkedin.voyager.messaging.create.MessageCreate")]
        public LdJsonElements ComLinkedinMessageCreate { get; set; }

        [JsonProperty(PropertyName = "body")] public string Body { get; set; }

        [JsonProperty(PropertyName = "attachments")]
        public List<string> Attachments { get; set; }

        [JsonProperty(PropertyName = "attributedBody")]
        public LdJsonElements AttributedBody { get; set; }


        [JsonProperty(PropertyName = "customContent")]
        public LdJsonElements CustomContent { get; set; }

        [JsonProperty(PropertyName = "string")]
        public string String { get; set; }

        [JsonProperty(PropertyName = "recipients")]
        public List<string> Recipients { get; set; }

        [JsonProperty(PropertyName = "subtype")]
        public string Subtype { get; set; }

        [JsonProperty(PropertyName = "originalId")]
        public string OriginalId { get; set; }

        [JsonProperty(PropertyName = "name")] public string Name { get; set; }


        [JsonProperty(PropertyName = "byteSize")]
        public long? ByteSize { get; set; }

        [JsonProperty(PropertyName = "mediaType")]
        public string MediaType { get; set; }

        [JsonProperty(PropertyName = "reference")]
        public LdJsonElements Reference { get; set; }

        [JsonProperty(PropertyName = "jsonElements")]
        public LdJsonElements[] JsonElements { get; set; }

        #endregion

        [JsonProperty (PropertyName = "media")]
        public JArray Media { get; set; }

        [JsonProperty(PropertyName = "containerEntity")]
        public string ContainerEntity { get; set; }

        [JsonProperty(PropertyName = "origin")]
        public string origin { get; set; }

        [JsonProperty(PropertyName = "organizationActor")]
        public string OrganizationActor { get; set; }
    }
    public class MediaPostJsonElement
    {
        [JsonProperty(PropertyName = "jsonElements")]
        public MediaPostJsonElement[] JsonElements { get; set; }
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "originalId")]
        public string OriginalId { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "byteSize")]
        public long? ByteSize { get; set; }
        [JsonProperty(PropertyName = "mediaType")]
        public string MediaType { get; set; }
        [JsonProperty(PropertyName = "reference")]
        public MediaPostJsonElement Reference { get; set; }
        [JsonProperty(PropertyName = "string")]
        public string String { get; set; }
    }
}