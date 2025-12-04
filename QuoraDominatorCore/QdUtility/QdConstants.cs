using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuoraDominatorCore.Enums;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace QuoraDominatorCore.QdUtility
{
    public class QdConstants
    {
        #region API's.
        public static string GetLocationUrl => "https://app.multiloginapp.com/WhatIsMyIP";

        public static string SaveAdsUrlDev => "https://quora.dev.poweradspy.com/insertads";

        public static string SaveAdsUrlMain => "https://qapi.poweradspy.com/insertads";

        public static string SaveUserUrlMain => "https://qapi.poweradspy.com/quora_user_data";

        public static string SaveUserDetailsInDev => "https://quora.dev.poweradspy.com/quora_user_data";

        public static string GetLocationApiUrl(string ip) => $"https://api.db-ip.com/v2/17f5666a35e4e5bad778c00e3dbe2016559917ff/{ip}";
        public static string GetLocationApiUrlFree(string ip) => $"http://api.db-ip.com/v2/free/{ip}";
        public static string GetMessageAPI => $"{HomePageUrl}/graphql/gql_para_POST?q=MessageThreadListQuery";
        public static string GetBroadCastMessageAPI(string ThreadId) =>
            string.IsNullOrEmpty(ThreadId) ? $"{HomePageUrl}/graphql/gql_POST?q=UserMessageModal_threadCreate_Mutation" 
            : $"{HomePageUrl}/graphql/gql_POST?q=ThreadReplyComposer_threadReplyAdd_Mutation";
        public static string GetAllMessageAPI => $"{HomePageUrl}/graphql/gql_para_POST?q=LatestMessagesLoaderReloadableQuery";
        public static string GetSharePostAPI(string domain) => $"https://{domain}/graphql/gql_POST?q=QuoraShareComposerWrapper_postAdd_Mutation";
        public static string GetTimeStamp => DateTime.Now.Ticks.ToString();
        public static string GetContentType => "application/json";
        public static string HomePageUrl => "https://www.quora.com";
        public static string MessageUrl => $"{HomePageUrl}/messages";
        public static string GetUnfollowUserAPI => $"{HomePageUrl}/graphql/gql_POST?q=UserFollow_userUnfollow_Mutation";
        public static string GetSearchQueryAPI => $"{HomePageUrl}/graphql/gql_para_POST?q=SearchResultsListQuery";
        public static string QuestionDetailAPI => $"{HomePageUrl}/graphql/gql_para_POST?q=QuestionCollapsedAnswerLoaderQuery";
        public static string UserActivityDetailsAPI(UserActivityType activityType,string OrderType="MostRecent",string Domain= "www.quora.com") =>
            activityType == UserActivityType.Profile ?
            $"https://{Domain}/graphql/gql_para_POST?q=UserProfileCombinedListQuery" :
            activityType==UserActivityType.Followers?
            $"https://{Domain}/graphql/gql_para_POST?q=UserProfileFollowers_ProfileTopics_Query" :
            activityType==UserActivityType.Followings?
            $"https://{Domain}/graphql/gql_para_POST?q=UserProfileFollowingSpaces_ProfileTopics_Query" :
            activityType==UserActivityType.Questions?
            $"https://{Domain}/graphql/gql_para_POST?q=UserProfileQuestionsList_Questions_Query" :
            activityType==UserActivityType.Answers?
             OrderType.Contains("MostRecent")? $"https://{Domain}/graphql/gql_para_POST?q=UserProfileAnswersMostRecent_RecentAnswers_Query" : $"https://{Domain}/graphql/gql_para_POST?q=UserProfileAnswersMostViewed_RecentAnswers_Query" :
            activityType==UserActivityType.Posts?
            $"https://{Domain}/graphql/gql_para_POST?q=UserProfilePostsList_Posts_Query" :
            activityType==UserActivityType.Edits?
            $"https://{Domain}/graphql/gql_para_POST?q=UserProfileEditsQuery" :
            activityType==UserActivityType.Activity?
            $"https://{Domain}/graphql/gql_para_POST?q=UserProfileActivity_feedStories_Query":
            activityType==UserActivityType.ProfileFollowers?
            OrderType.Contains("MostRecent") ? $"https://{Domain}/graphql/gql_para_POST?q=UserProfileFollowersModalQuery" :
            $"https://{Domain}/graphql/gql_para_POST?q=UserProfileFollowers_ProfileTopics_Query" :
            activityType==UserActivityType.ProfileFollowings?
            OrderType.Contains("MostRecent")? $"https://{Domain}/graphql/gql_para_POST?q=UserProfileFollowersModalQuery" :
            $"https://{Domain}/graphql/gql_para_POST?q=UserProfileFollowingPeople_ProfileTopics_Query":
            activityType==UserActivityType.AnswersOfQuestion?
            $"https://{Domain}/graphql/gql_para_POST?q=QuestionPagedListPaginationQuery":
            activityType==UserActivityType.AnswerDetail?
            $"https://{Domain}/graphql/gql_para_POST?q=AnswerPageFooterLoaderQuery":
            activityType==UserActivityType.TopicAnswer?
            OrderType.Contains("MostRecent")? $"https://{Domain}/graphql/gql_para_POST?q=TopicWriteMultifeed_Query" 
            : $"https://{Domain}/graphql/gql_para_POST?q=MultifeedQuery":
            activityType==UserActivityType.AnswerUpvoter?
            OrderType.Contains("MostRecent")? $"https://{Domain}/graphql/gql_para_POST?q=UpvoterListModalQuery" 
            : $"https://{Domain}/graphql/gql_para_POST?q=UpvoterPagedListQuery":
            activityType==UserActivityType.AnswerCommentor || activityType==UserActivityType.QuestionCommentor?
            OrderType.Contains("MostRecent")&& UserActivityType.AnswerCommentor != activityType? $"https://{Domain}/graphql/gql_para_POST?q=CommentableCommentAreaLoaderInnerQuery" 
            : $"https://{Domain}/graphql/gql_para_POST?q=AllCommentsListQuery"
            : string.Empty;

        public static string CreateQuestionAPI => $"{HomePageUrl}/graphql/gql_POST?q=askQuestionStepUtils_questionCreate_Mutation";
        public static string MediaUploadAPI => "https://upload.quora.com/_/imgupload/upload_POST";
        public static string CreatePostAPI => $"{HomePageUrl}/graphql/gql_POST?q=draftUtils_postDraftSave_Mutation";
        public static string FetchUploadPostUrlAPI => $"{HomePageUrl}/graphql/gql_POST?q=AskQuestionStepShareLinkWrapper_postAdd_Mutation";
        public static string AnswerOnQuestionAPI => $"{HomePageUrl}/graphql/gql_POST?q=AnswerEditorMutation_answerCreate_Mutation";
        public static string GetReportAPI => $"{HomePageUrl}/graphql/gql_POST?q=ReportModalInner_reportAdd_Mutation";
        public static string ReportModelAPI => $"{HomePageUrl}/graphql/gql_para_POST?q=ReportModalQuery";
        public static string LoginAPI => $"{HomePageUrl}/graphql/gql_POST?q=LoginForm_loginDo_Mutation";
        #endregion

        #region PostData.
        public static string GetSharePostBody(string CredentialId, string objectId) =>
            $"{{\"queryName\":\"QuoraShareComposerWrapper_postAdd_Mutation\",\"variables\":{{\"tribeId\":0,\"title\":\"{{\\\"sections\\\":[{{\\\"type\\\":\\\"plain\\\",\\\"spans\\\":[{{\\\"text\\\":\\\"\\\",\\\"modifiers\\\":{{}}}}],\\\"indent\\\":0,\\\"is_rtl\\\":false,\\\"quoted\\\":false}}]}}\",\"content\":\"{{\\\"sections\\\":[]}}\",\"shouldQueue\":false,\"embeddedContentType\":\"post\",\"embeddedOid\":{objectId},\"fromSuggestions\":false,\"accessOption\":null,\"credentialId\":{CredentialId}}},\"extensions\":{{\"hash\":\"18094f9f4cedaf9cc96f22d585fd11df25b1f9dfb31a5b61d3db10de640f616e\"}}}}";
        public static string GetBroadCastMessagePostBody(string ThreadId,string UserId,string message,string numOfKnownMessages) =>
            string.IsNullOrEmpty(ThreadId) ? $"{{\"queryName\":\"UserMessageModal_threadCreate_Mutation\",\"variables\":{{\"recipientUid\":{UserId},\"content\":\"{QdUtilities.GetJsonPostBody(message)}\"}},\"extensions\":{{\"hash\":\"6e52e64ad47417185d6e9ed1111ea6e5c12352e76b99947bf0aa9f6993f6e5c6\"}}}}" :
                $"{{\"queryName\":\"ThreadReplyComposer_threadReplyAdd_Mutation\",\"variables\":{{\"threadId\":{ThreadId},\"content\":\"{QdUtilities.GetJsonPostBody(message)}\",\"numMessagesKnownOnClient\":{numOfKnownMessages}}},\"extensions\":{{\"hash\":\"feacd27a3306b0771333814fc85fe43b31f3180bd827fc70676f62a02111a6bb\"}}}}";

        public static string GetReportPostBody(string Reason,string ReportTableID,string Description,string TargetType="user") =>
            $"{{\"queryName\":\"ReportModalInner_reportAdd_Mutation\",\"variables\":{{\"reason\":\"{Reason}\",\"targetType\":\"{TargetType}\",\"reportableId\":{ReportTableID},\"comment\":{GetReportComment(Description)}}},\"extensions\":{{\"hash\":\"2ed8e9c14e48c8751454d6420da5b79bc84942311d8abd8e7112c6067a410c2b\"}}}}";

        private static object GetReportComment(string description) =>
            string.IsNullOrEmpty(description) ?"null": $"\"{{\\\"sections\\\":[{{\\\"type\\\":\\\"plain\\\",\\\"indent\\\":0,\\\"quoted\\\":false,\\\"is_rtl\\\":false,\\\"spans\\\":[{{\\\"modifiers\\\":{{}},\\\"text\\\":\\\"{description}\\\"}}]}}],\\\"caret\\\":{{\\\"start\\\":{{\\\"spanIdx\\\":0,\\\"sectionIdx\\\":0,\\\"offset\\\":{description.Length+1}}},\\\"end\\\":{{\\\"spanIdx\\\":0,\\\"sectionIdx\\\":0,\\\"offset\\\":{description.Length+1}}}}}}}\"";

        public static string ReportModelPostBody(string AuthorId) =>
            $"{{\"queryName\":\"ReportModalQuery\",\"variables\":{{\"id\":\"{AuthorId}\"}},\"extensions\":{{\"hash\":\"2faed79d39a1c2a547f509a6934c1b9c0f606a334d8446c62227a1c984b58eb5\"}}}}";
        public static string AnswerOnQuestionBody(string QuestionId, string Content) =>
            $"{{\"queryName\":\"AnswerEditorMutation_answerCreate_Mutation\",\"variables\":{{\"qid\":{QuestionId},\"content\":\"{Content}\",\"tribeId\":null,\"attachDisclaimer\":false,\"businessUid\":null,\"businessCustomAction\":{{\"objectiveType\":2,\"cta\":0}},\"accessOption\":null,\"captcha\":null}},\"extensions\":{{\"hash\":\"93127e7c752522a0114a1ada8438cbb85e3dc4b528efb58452df1745b7fd80a6\"}}}}";
        public static string GetMessagePostData(int paginationCount = 0) => $"{{\"queryName\":\"MessageThreadListQuery\",\"variables\":{{\"first\":10,\"after\":\"{paginationCount}\"}},\"extensions\":{{\"hash\":\"b7135008b813631b502d4231a7c10ca44dcc1eddbc5bbd6329d34e8185197506\"}}}}";
        public static string GetAllMessagePostData(string ThreadId) => $"{{\"queryName\":\"LatestMessagesLoaderReloadableQuery\",\"variables\":{{\"threadId\":{ThreadId},\"numMessagesKnownOnClient\":0}},\"extensions\":{{\"hash\":\"f3ab8817a9d8207025a7974fc98e7ca588607c2e17e23bdfbdf425763e870597\"}}}}";
        public static string GetPostDataForSearchQuery(SearchQueryType queryType, string QueryValue,int paginationCount=0)
        {
            var QueryType = queryType==SearchQueryType.Topics?"topic":queryType==SearchQueryType.Questions?"question":queryType==SearchQueryType.Answers?"answer":queryType==SearchQueryType.Posts?"post":queryType==SearchQueryType.Profiles?"profile":"tribe";
            paginationCount = paginationCount == 0 && QueryType == "topic" ? -1 : paginationCount;
            return $"{{\"queryName\":\"SearchResultsListQuery\",\"variables\":{{\"query\":\"{QueryValue}\",\"disableSpellCheck\":null,\"resultType\":\"{QueryType}\",\"author\":null,\"time\":\"all_times\",\"first\":10,\"after\":\"{paginationCount}\",\"tribeId\":null}},\"extensions\":{{\"hash\":\"5dbfd77783830a656aa50b46b56b289f16107737c8327b1ae8a0808ea3fb0e34\"}}}}";
        }
        public static string GetPostDataForVotings(VoteQueryType voteType,int oid,PostQueryType postType)
        {
            return voteType == VoteQueryType.UpVote && postType == PostQueryType.Post ?
                $"{{\"queryName\":\"voteUtils_voteChange_Mutation\",\"variables\":{{\"oid\":{oid},\"voteType\":\"upvote\",\"entityType\":\"post\",\"feedStoryHash\":null}},\"extensions\":{{\"hash\":\"1ed80dcdea807f14a363cced5cabf07136288210cf20e4662bb746504e725a09\"}}}}"
                : voteType == VoteQueryType.DownVote && postType == PostQueryType.Post ?
                $"{{\"queryName\":\"voteUtils_voteChange_Mutation\",\"variables\":{{\"oid\":{oid},\"voteType\":\"downvote\",\"entityType\":\"post\",\"feedStoryHash\":null}},\"extensions\":{{\"hash\":\"1ed80dcdea807f14a363cced5cabf07136288210cf20e4662bb746504e725a09\"}}}}"
                : voteType == VoteQueryType.UpVote && postType == PostQueryType.Answer ?
                $"{{\"queryName\":\"voteUtils_voteChange_Mutation\",\"variables\":{{\"oid\":{oid},\"voteType\":\"upvote\",\"entityType\":\"answer\",\"feedStoryHash\":null}},\"extensions\":{{\"hash\":\"1ed80dcdea807f14a363cced5cabf07136288210cf20e4662bb746504e725a09\"}}}}"
                : voteType == VoteQueryType.DownVote && postType == PostQueryType.Answer ?
                $"{{\"queryName\":\"voteUtils_voteChange_Mutation\",\"variables\":{{\"oid\":{oid},\"voteType\":\"downvote\",\"entityType\":\"answer\",\"feedStoryHash\":null}},\"extensions\":{{\"hash\":\"1ed80dcdea807f14a363cced5cabf07136288210cf20e4662bb746504e725a09\"}}}}"
                : string.Empty;
        }
        public static string UserActivityDetailsPostBody(UserActivityType activityType, string UserID, int PaginationCount=0, string Order = "MostRecent",string Id="",string MultifeedAfter="",string PageData="",string Hash = "")
            => activityType == UserActivityType.Questions ?
            $"{{\"queryName\":\"UserProfileQuestionsList_Questions_Query\",\"variables\":{{\"uid\":{UserID},\"first\":3,\"after\":\"{PaginationCount}\"}},\"extensions\":{{\"hash\":\"5ed5fda37c26b9d5da54a76c7a4fc93954df75e83fb46fa5e0d7aba02bcf996d\"}}}}"
            : activityType == UserActivityType.Posts ?
            $"{{\"queryName\":\"UserProfilePostsList_Posts_Query\",\"variables\":{{\"uid\":{UserID},\"order\":\"{Order}\",\"first\":10,\"after\":{PaginationCount}}},\"extensions\":{{\"hash\":\"16c5c261a2fb396dd447045f25331ad57c6a0c49a76ac8fee6437c05e65642d8\"}}}}" :
            activityType == UserActivityType.Profile ?
            $"{{\"queryName\":\"UserProfileCombinedListQuery\",\"variables\":{{\"uid\":{UserID},\"order\":{Order},\"first\":3,\"after\":{PaginationCount}}},\"extensions\":{{\"hash\":\"3140b8739726901d0a7f357fc077f32d35fc072804aa047fde1db615dc628817\"}}}}" :
            activityType==UserActivityType.Answers?
            Order.Contains("MostRecent") ? $"{{\"queryName\":\"UserProfileAnswersMostRecent_RecentAnswers_Query\",\"variables\":{{\"uid\":{UserID},\"first\":3,\"after\":{PaginationCount},\"answerFilterTid\":null}},\"extensions\":{{\"hash\":\"3d7e4ac912bc665dc90f68c58cf3af176ad45aeeb339e44cb808baca6efd91d2\"}}}}" :$"{{\"queryName\":\"UserProfileAnswersMostViewed_RecentAnswers_Query\",\"variables\":{{\"uid\":{UserID},\"first\":3,\"after\":{PaginationCount},\"answerFilterTid\":null}},\"extensions\":{{\"hash\":\"3d7e4ac912bc665dc90f68c58cf3af176ad45aeeb339e44cb808baca6efd91d2\"}}}}" :
            activityType==UserActivityType.Followers?
            $"{{\"queryName\":\"UserProfileFollowers_ProfileTopics_Query\",\"variables\":{{\"uid\":{UserID},\"first\":10,\"after\":{PaginationCount}}},\"extensions\":{{\"hash\":\"9435b6a6719299f6f9bc6f8adc6a8afacca261d08f1a734ac528f01255329a9e\"}}}}" :
            activityType==UserActivityType.Followings?
            $"{{\"queryName\":\"UserProfileFollowingSpaces_ProfileTopics_Query\",\"variables\":{{\"uid\":{UserID},\"first\":10,\"after\":{PaginationCount}}},\"extensions\":{{\"hash\":\"0b4c9f8854d322a657052b24251c06f531b6b59f30f67ac03ec6105e4923f6a6\"}}}}" :
            activityType==UserActivityType.Edits?
            $"{{\"queryName\":\"UserProfileEditsQuery\",\"variables\":{{\"uid\":{UserID},\"first\":10,\"after\":\"{PaginationCount}\"}},\"extensions\":{{\"hash\":\"ccfc4c5ce483da14906f055e57ffd8af61571a1bd76fb91b081f85cd78b62c5b\"}}}}" :
            activityType==UserActivityType.Activity?
            $"{{\"queryName\":\"UserProfileActivity_feedStories_Query\",\"variables\":{{\"uid\":{UserID},\"first\":3,\"after\":{PaginationCount}}},\"extensions\":{{\"hash\":\"3c6ef906303f8dd04e5c18d9b90b98f390ef61f6a76a449e8c156a2ca1478021\"}}}}" :
            activityType==UserActivityType.ProfileFollowers?
            Order.Contains("MostRecent") ? $"{{\"queryName\":\"UserProfileFollowersModalQuery\",\"variables\":{{\"uid\":{UserID}}},\"extensions\":{{\"hash\":\"ce1d36b9ddbc111ef36a4b8f75980152860fe766a91090e6649fbccd19ee2ad1\"}}}}" : $"{{\"queryName\":\"UserProfileFollowers_ProfileTopics_Query\",\"variables\":{{\"uid\":{UserID},\"first\":10,\"after\":\"{PaginationCount}\"}},\"extensions\":{{\"hash\":\"9435b6a6719299f6f9bc6f8adc6a8afacca261d08f1a734ac528f01255329a9e\"}}}}" :
            activityType==UserActivityType.ProfileFollowings?
            Order.Contains("MostRecent") ? $"{{\"queryName\":\"UserProfileFollowersModalQuery\",\"variables\":{{\"uid\":{UserID}}},\"extensions\":{{\"hash\":\"ce1d36b9ddbc111ef36a4b8f75980152860fe766a91090e6649fbccd19ee2ad1\"}}}}" : $"{{\"queryName\":\"UserProfileFollowingPeople_ProfileTopics_Query\",\"variables\":{{\"uid\":{UserID},\"first\":10,\"after\":\"{PaginationCount}\"}},\"extensions\":{{\"hash\":\"6608872fcef6af5a6efbd5cc35e9b063db6bb3a7339a032a15ff6acf46a6290e\"}}}}":
            activityType==UserActivityType.AnswersOfQuestion?
            $"{{\"queryName\":\"QuestionPagedListPaginationQuery\",\"variables\":{{\"count\":10,\"cursor\":{PaginationCount},\"forceScoreVersion\":null,\"initial_count\":2,\"id\":\"{Id}\"}},\"extensions\":{{\"hash\":\"0b03da91bd2d7b088201a359f60b559d20ab177fccdcf0bb4dd08ae5d638ee8d\"}}}}" :
            activityType==UserActivityType.AnswerDetail?
            string.IsNullOrEmpty(Hash)?
            $"{{\"queryName\":\"AnswerPageFooterLoaderQuery\",\"variables\":{{\"aid\":{Id},\"showAd\":false,\"tribeId\":null,\"isFollowingFeed\":false}},\"extensions\":{{\"hash\":\"7eadb34ea06af6d4b1e46c42f0254aa9161a28bb2c735d473b50177847b0f454\"}}}}" :
            $"{{\"queryName\":\"AnswerPageFooterLoaderQuery\",\"variables\":{{\"aid\":{Id},\"showAd\":false,\"tribeId\":null,\"isFollowingFeed\":false}},\"extensions\":{{\"hash\":\"{Hash}\"}}}}":
            activityType==UserActivityType.TopicAnswer?
            Order.Contains("MostRecent")?$"{{\"queryName\":\"TopicWriteMultifeed_Query\",\"variables\":{{\"multifeedAfter\":null,\"multifeedNumBundlesOnClient\":0,\"tid\":{Id},\"first\":10}},\"extensions\":{{\"hash\":\"208f138c2f190bd124fb870d819bd0aef5619c0fe722e1710f88c05c15038887\"}}}}" :$"{{\"queryName\":\"MultifeedQuery\",\"variables\":{{\"first\":10,\"multifeedAfter\":{MultifeedAfter},\"multifeedNumBundlesOnClient\":10,\"injectionType\":null,\"injectionData\":null,\"filterStoryType\":null,\"filterStoryOid\":null,\"multifeedPage\":\"top_questions_in_topic\",\"pageData\":{PageData},\"showLiveBanner\":false}},\"extensions\":{{\"hash\":\"6d85846827160c3342677c35b4ab69c3bed65b2c11ceeaf5761e9b6dccb32b6a\"}}}}" :
            activityType==UserActivityType.AnswerUpvoter?
            Order.Contains("MostRecent")?$"{{\"queryName\":\"UpvoterListModalQuery\",\"variables\":{{\"nodeId\":\"{Id}\"}},\"extensions\":{{\"hash\":\"792635a0f9c78b84fad95f01c6ce4ede510831d49625f6d45e6d92b08d70b3f6\"}}}}" :$"{{\"queryName\":\"UpvoterPagedListQuery\",\"variables\":{{\"nodeId\":\"{Id}\",\"first\":10,\"after\":\"{PaginationCount}\"}},\"extensions\":{{\"hash\":\"c221e6ef377c660d64f8eaffbdbc9a0b627f50f08830eb3b33eef2bf3bd05032\"}}}}" :
            activityType==UserActivityType.AnswerCommentor || activityType==UserActivityType.QuestionCommentor?
            Order.Contains("MostRecent") && UserActivityType.AnswerCommentor != activityType ? $"{{\"queryName\":\"CommentableCommentAreaLoaderInnerQuery\",\"variables\":{{\"id\":\"{Id}\",\"first\":5}},\"extensions\":{{\"hash\":\"c64f239de2332a1498adba341d8d0b9fc76b4ee360b43a39d45d6496eafb0241\"}}}}" :$"{{\"queryName\":\"AllCommentsListQuery\",\"variables\":{{\"id\":\"{Id}\",\"first\":5,\"after\":{PaginationCount}}},\"extensions\":{{\"hash\":\"7d2f7133010b7e36e3d71e5fdddc6a27e2c3a45bfb99a9c3a23b4faa3bb6cce9\"}}}}"
            : string.Empty;
        public static string AnswerDetailsPostData(string QuestionId)=> $"{{\"queryName\":\"QuestionCollapsedAnswerLoaderQuery\",\"variables\":{{\"qid\":{QuestionId}}},\"extensions\":{{\"hash\":\"acfc03663d1e705a3813aa270b9eccc146103986eaf6c3c0731231946ebc0b52\"}}}}";
        public static byte[] GetUTF8PostData(string PostData) => Encoding.UTF8.GetBytes(PostData);

        public static string CreateQuestionPostBody(string Question,bool IsFirst=true) 
            =>IsFirst?$"{{\"queryName\":\"askQuestionStepUtils_questionCreate_Mutation\",\"variables\":{{\"titlePlaintext\":\"{Question}\",\"sourceLocation\":7,\"limitedUserDistro\":false,\"targetType\":null,\"targetOid\":null,\"shouldCheckDuplicate\":true,\"shouldCheckCorrection\":true,\"shouldQueue\":null}},\"extensions\":{{\"hash\":\"6cfeefad00e9725fd5a39af18b1461c237c1eff48d82c081a0db29d0b2ac8db2\"}}}}":
              $"{{\"queryName\":\"askQuestionStepUtils_questionCreate_Mutation\",\"variables\":{{\"titlePlaintext\":\"{Question}\",\"sourceLocation\":7,\"limitedUserDistro\":false,\"targetType\":null,\"targetOid\":0,\"shouldCheckDuplicate\":false,\"shouldCheckCorrection\":false,\"shouldQueue\":null}},\"extensions\":{{\"hash\":\"6cfeefad00e9725fd5a39af18b1461c237c1eff48d82c081a0db29d0b2ac8db2\"}}}}";
        //public static string CreatePostBody(string TitleText, string ImageUrl, string ShareUrl) =>
        //    !string.IsNullOrEmpty(ImageUrl)?
        //    $"{{\"queryName\":\"draftUtils_postDraftSave_Mutation\",\"variables\":{{\"tribeId\":0,\"title\":\"{{\\\"sections\\\":[]}}\",\"content\":\"{{\\\"sections\\\":[{{\\\"type\\\":\\\"plain\\\",\\\"indent\\\":0,\\\"quoted\\\":false,\\\"is_rtl\\\":false,\\\"spans\\\":[{{\\\"modifiers\\\":{{}},\\\"text\\\":\\\"{TitleText}\\\"}}]}},{{\\\"type\\\":\\\"image\\\",\\\"indent\\\":0,\\\"quoted\\\":false,\\\"is_rtl\\\":false,\\\"spans\\\":[{{\\\"modifiers\\\":{{\\\"image\\\":\\\"{ImageUrl}\\\"}},\\\"text\\\":\\\"\\\"}}]}},{{\\\"type\\\":\\\"plain\\\",\\\"indent\\\":0,\\\"quoted\\\":false,\\\"is_rtl\\\":false,\\\"spans\\\":[{{\\\"modifiers\\\":{{}},\\\"text\\\":\\\"\\\"}}]}}]}}\",\"isAutoSaved\":true,\"isNullspacePost\":true}},\"extensions\":{{\"hash\":\"b3eee9e1677ff5af07c094da285039a3cbacee364e61f562a0c0495794145c2d\"}}}}" :
        //    !string.IsNullOrEmpty(ShareUrl)?
        //    $"{{\"queryName\":\"draftUtils_postDraftSave_Mutation\",\"variables\":{{\"tribeId\":0,\"title\":\"{{\\\"sections\\\":[]}}\",\"content\":\"{{\\\"sections\\\":[{{\\\"type\\\":\\\"plain\\\",\\\"indent\\\":0,\\\"quoted\\\":false,\\\"is_rtl\\\":false,\\\"spans\\\":[{{\\\"modifiers\\\":{{}},\\\"text\\\":\\\"{TitleText}\\\"}}]}},{{\\\"type\\\":\\\"hyperlink_embed\\\",\\\"indent\\\":0,\\\"quoted\\\":false,\\\"spans\\\":[{{\\\"modifiers\\\":{{\\\"embed\\\":{{\\\"url\\\":\\\"{ShareUrl}\\\"}}}}}}]}},{{\\\"type\\\":\\\"plain\\\",\\\"indent\\\":0,\\\"quoted\\\":false,\\\"is_rtl\\\":false,\\\"spans\\\":[{{\\\"modifiers\\\":{{}},\\\"text\\\":\\\"\\\"}}]}}]}}\",\"isAutoSaved\":true,\"isNullspacePost\":true}},\"extensions\":{{\"hash\":\"b3eee9e1677ff5af07c094da285039a3cbacee364e61f562a0c0495794145c2d\"}}}}" :
        //    $"{{\"queryName\":\"draftUtils_postDraftSave_Mutation\",\"variables\":{{\"tribeId\":0,\"title\":\"{{\\\"sections\\\":[]}}\",\"content\":\"{{\\\"sections\\\":[{{\\\"type\\\":\\\"plain\\\",\\\"indent\\\":0,\\\"quoted\\\":false,\\\"is_rtl\\\":false,\\\"spans\\\":[{{\\\"modifiers\\\":{{}},\\\"text\\\":\\\"{TitleText}\\\"}}]}}]}}\",\"isAutoSaved\":true,\"isNullspacePost\":true}},\"extensions\":{{\"hash\":\"b3eee9e1677ff5af07c094da285039a3cbacee364e61f562a0c0495794145c2d\"}}}}";
        //public static string UploadedPostUrlPostBody(string TitleText, string ImageUrl, string ShareUrl,string CredentialID) =>
        //    !string.IsNullOrEmpty(ImageUrl) ?
        //    $"{{\"queryName\":\"AskQuestionStepShareLinkWrapper_postAdd_Mutation\",\"variables\":{{\"tribeId\":0,\"title\":\"{{\\\"sections\\\":[{{\\\"type\\\":\\\"plain\\\",\\\"spans\\\":[{{\\\"text\\\":\\\"\\\",\\\"modifiers\\\":{{}}}}],\\\"indent\\\":0,\\\"is_rtl\\\":false,\\\"quoted\\\":false}}]}}\",\"content\":\"{{\\\"sections\\\":[{{\\\"type\\\":\\\"plain\\\",\\\"indent\\\":0,\\\"quoted\\\":false,\\\"is_rtl\\\":false,\\\"spans\\\":[{{\\\"modifiers\\\":{{}},\\\"text\\\":\\\"{TitleText}\\\"}}]}},{{\\\"type\\\":\\\"image\\\",\\\"indent\\\":0,\\\"quoted\\\":false,\\\"is_rtl\\\":false,\\\"spans\\\":[{{\\\"modifiers\\\":{{\\\"image\\\":\\\"{ImageUrl}\\\"}},\\\"text\\\":\\\"\\\"}}]}},{{\\\"type\\\":\\\"plain\\\",\\\"indent\\\":0,\\\"quoted\\\":false,\\\"is_rtl\\\":false,\\\"spans\\\":[{{\\\"modifiers\\\":{{}},\\\"text\\\":\\\"\\\"}}]}}]}}\",\"shouldQueue\":false,\"credentialId\":{CredentialID},\"accessOption\":null,\"captcha\":null}},\"extensions\":{{\"hash\":\"7b738ec94591e759e2e3ae3762f598b3390d97fafa514f037ab125f8ab15854a\"}}}}" :
        //    !string.IsNullOrEmpty(ShareUrl) ?
        //    $"{{\"queryName\":\"AskQuestionStepShareLinkWrapper_postAdd_Mutation\",\"variables\":{{\"tribeId\":0,\"title\":\"{{\\\"sections\\\":[{{\\\"type\\\":\\\"plain\\\",\\\"spans\\\":[{{\\\"text\\\":\\\"\\\",\\\"modifiers\\\":{{}}}}],\\\"indent\\\":0,\\\"is_rtl\\\":false,\\\"quoted\\\":false}}]}}\",\"content\":\"{{\\\"sections\\\":[{{\\\"type\\\":\\\"plain\\\",\\\"indent\\\":0,\\\"quoted\\\":false,\\\"is_rtl\\\":false,\\\"spans\\\":[{{\\\"modifiers\\\":{{}},\\\"text\\\":\\\"{TitleText}\\\"}}]}},{{\\\"type\\\":\\\"hyperlink_embed\\\",\\\"indent\\\":0,\\\"quoted\\\":false,\\\"spans\\\":[{{\\\"modifiers\\\":{{\\\"embed\\\":{{\\\"url\\\":\\\"{ShareUrl}\\\"}}}}}}]}},{{\\\"type\\\":\\\"plain\\\",\\\"indent\\\":0,\\\"quoted\\\":false,\\\"is_rtl\\\":false,\\\"spans\\\":[{{\\\"modifiers\\\":{{}},\\\"text\\\":\\\"\\\"}}]}}]}}\",\"shouldQueue\":false,\"credentialId\":{CredentialID},\"accessOption\":null,\"captcha\":null}},\"extensions\":{{\"hash\":\"7b738ec94591e759e2e3ae3762f598b3390d97fafa514f037ab125f8ab15854a\"}}}}" :
        //    $"{{\"queryName\":\"AskQuestionStepShareLinkWrapper_postAdd_Mutation\",\"variables\":{{\"tribeId\":0,\"title\":\"{{\\\"sections\\\":[{{\\\"type\\\":\\\"plain\\\",\\\"spans\\\":[{{\\\"text\\\":\\\"\\\",\\\"modifiers\\\":{{}}}}],\\\"indent\\\":0,\\\"is_rtl\\\":false,\\\"quoted\\\":false}}]}}\",\"content\":\"{{\\\"sections\\\":[{{\\\"type\\\":\\\"plain\\\",\\\"indent\\\":0,\\\"quoted\\\":false,\\\"is_rtl\\\":false,\\\"spans\\\":[{{\\\"modifiers\\\":{{}},\\\"text\\\":\\\"{TitleText}\\\"}}]}}]}}\",\"shouldQueue\":false,\"credentialId\":{CredentialID},\"accessOption\":null,\"captcha\":null}},\"extensions\":{{\"hash\":\"7b738ec94591e759e2e3ae3762f598b3390d97fafa514f037ab125f8ab15854a\"}}}}";
        public static string LoginPostBody(string UserName, string Password, string CaptchaTokenString = "") =>
            $"{{\"queryName\":\"LoginForm_loginDo_Mutation\",\"variables\":{{\"email\":\"{UserName}\",\"password\":\"{Password}\",\"captcha\":\"{CaptchaTokenString}\"}},\"extensions\":{{\"hash\":\"84c101336cf918326e85a2bfd01acba0a99e266346c3414a8472bc4e6e8b6415\"}}}}";
        public static string GetUserUnfollowBody(string UserId) => $"{{\"queryName\":\"UserFollow_userUnfollow_Mutation\",\"variables\":{{\"uid\":{UserId}}},\"extensions\":{{\"hash\":\"6ceb8ce281c231d01ab1da74324055bf81f4b19fcadbc94c2dfa88fc5782e0a9\"}}}}";
        #endregion
        public static string CreatePostBodyData(string TitleText, string ImageUrl)
        {
            var jsonString = string.Empty;
            try
            {
                var imgText = "{\"type\":\"image\",\"indent\":0,\"quoted\":false,\"is_rtl\":false,\"spans\":[{\"modifiers\":{\"image\":\""+ImageUrl+"\"},\"text\":\"\"}]},{\"type\":\"plain\",\"indent\":0,\"quoted\":false,\"is_rtl\":false,\"spans\":[{\"modifiers\":{},\"text\":\"\"}]}]}";
                //var _shareText = "{\"type\":\"hyperlink_embed\",\"indent\":0,\"quoted\":false,\"spans\":[{\"modifiers\":{\"embed\":{\"url\":\""+ShareUrl+"\"}}}]},{\"type\":\"plain\",\"indent\":0,\"quoted\":false,\"is_rtl\":false,\"spans\":[{\"modifiers\":{},\"text\":\"\"}]}]}";
                var data = "{\"sections\":[";
                if(!string.IsNullOrEmpty(TitleText))
                {
                    var collectionOfData = Regex.Split(TitleText, "\r\n");

                    foreach (var collection in collectionOfData)
                    {
                        if (!string.IsNullOrEmpty(collection))
                            if (collection.Contains("http"))
                            {
                                int linkStart = collection.IndexOf("http");
                                string textBeforeLink = collection.Substring(0, linkStart);
                                var link = Utilities.GetBetween(collection, "http", " ");
                                link = string.IsNullOrEmpty(link) ? collection.Substring(linkStart) : "http" + link;
                                data = data + "{\"type\":\"plain\",\"indent\":0,\"quoted\":false,\"is_rtl\":false,\"spans\":[{\"modifiers\":{},\"text\":\"" + textBeforeLink + "\"},{\"modifiers\":{\"link\":{\"type\":\"url\",\"url\":\"" + link + "\"}},\"text\":\"" + link + "\"}]},";
                            }
                            else
                                data = data + "{\"type\":\"plain\",\"indent\":0,\"quoted\":false,\"is_rtl\":false,\"spans\":[{\"modifiers\":{},\"text\":\"" + collection + "\"}]},";
                    }
                }
                else
                    data = data + "{\"type\":\"plain\",\"indent\":0,\"quoted\":false,\"is_rtl\":false,\"spans\":[{\"modifiers\":{},\"text\":\"" + TitleText + "\"}]},";

                data = data +  imgText;
                QdCreatePostPostData qdCreatePost = new QdCreatePostPostData
                {
                    queryName = "draftUtils_postDraftSave_Mutation",
                    variables = new CreatePostVariable
                    {
                        tribeId = 0,
                        title = "{\"sections\":[]}",
                        content = data,
                        isAutoSaved = true,
                        isNullspacePost = true
                    },
                    extensions = new Extensions()
                    {
                        hash = "b3eee9e1677ff5af07c094da285039a3cbacee364e61f562a0c0495794145c2d"
                    }
                };
                jsonString = JsonConvert.SerializeObject(qdCreatePost);

            }
            catch (Exception) { }
            return jsonString;
        }
        public static string UploadedPostUrlPostBodyData(string TitleText, string ImageUrl, string CredentialID)
        {
            var jsonString = string.Empty;
            try
            {
                var imgText = "{\"type\":\"image\",\"indent\":0,\"quoted\":false,\"is_rtl\":false,\"spans\":[{\"modifiers\":{\"image\":\"" + ImageUrl + "\"},\"text\":\"\"}]},{\"type\":\"plain\",\"indent\":0,\"quoted\":false,\"is_rtl\":false,\"spans\":[{\"modifiers\":{},\"text\":\"\"}]}]}";
                //var _shareText = "{\"type\":\"hyperlink_embed\",\"indent\":0,\"quoted\":false,\"spans\":[{\"modifiers\":{\"embed\":{\"url\":\"" + ShareUrl + "\"}}}]},{\"type\":\"plain\",\"indent\":0,\"quoted\":false,\"is_rtl\":false,\"spans\":[{\"modifiers\":{},\"text\":\"\"}]}]}";
                var data = "{\"sections\":[";
                if (!string.IsNullOrEmpty(TitleText))
                {
                    var collectionOfData = Regex.Split(TitleText, "\r\n");

                    foreach (var collection in collectionOfData)
                    {
                        if (!string.IsNullOrEmpty(collection))
                            if (collection.Contains("http"))
                            {
                                int linkstart = collection.IndexOf("http");
                                string textBeforeLink = collection.Substring(0, linkstart);
                                var link = Utilities.GetBetween(collection, "http", " ");
                                link = string.IsNullOrEmpty(link) ? collection.Substring(linkstart) : "http" + link;
                                data = data + "{\"type\":\"plain\",\"indent\":0,\"quoted\":false,\"is_rtl\":false,\"spans\":[{\"modifiers\":{},\"text\":\"" + textBeforeLink + "\"},{\"modifiers\":{\"link\":{\"type\":\"url\",\"url\":\"" + link + "\"}},\"text\":\"" + link + "\"}]},";
                            }
                            else
                                data = data + "{\"type\":\"plain\",\"indent\":0,\"quoted\":false,\"is_rtl\":false,\"spans\":[{\"modifiers\":{},\"text\":\"" + collection + "\"}]},";
                    }
                }
                else
                    data = data + "{\"type\":\"plain\",\"indent\":0,\"quoted\":false,\"is_rtl\":false,\"spans\":[{\"modifiers\":{},\"text\":\"" + TitleText + "\"}]},";

                data = data + imgText;
                QdUploadPostPostData qdUploadPost = new QdUploadPostPostData
                {
                    queryName = "AskQuestionStepShareLinkWrapper_postAdd_Mutation",
                    variables = new UploadPostVariable
                    {
                        tribeId = 0,
                        title = "{\"sections\":[]}",
                        content = data,
                        shouldQueue = false,
                        credentialId = (CredentialID.Contains("null")) ? null : CredentialID,
                        accessOption = null,
                        captcha = null
                    },
                    extensions = new Extensions()
                    {
                        hash = "7b738ec94591e759e2e3ae3762f598b3390d97fafa514f037ab125f8ab15854a"
                    }
                };
                 jsonString = JsonConvert.SerializeObject(qdUploadPost);

                //JObject jsonObject = JsonConvert.DeserializeObject<JObject>(jsonString);
                //jsonString = JsonConvert.SerializeObject(jsonObject);
            }
            catch (Exception) { }
            return jsonString;
        }
        public static string DecodeUnicodeEscapes(string input)
        {
            try
            {
                return Regex.Replace(input, @"\\\\u(?<Value>[a-fA-F0-9]{4})", m =>
                {
                    return ((char)Convert.ToInt32(m.Groups["Value"].Value, 16)).ToString();
                });
            }
            catch (Exception) { return ""; }

        }
        public static bool ContainsHindi(string input)
        {
            try
            {
                // Hindi Unicode range: U+0900 to U+097F
                Regex regex = new Regex(@"\p{IsDevanagari}");
                return regex.IsMatch(input);
            }
            catch (Exception) { return false; }

        }
        public static string GetJsonForAllTypePosts(string Response, string type = "post")
        {
            try
            {
                var JsonString = $"{{\"data\":{{\"{type}\":" + Utilities.GetBetween(Response, $"{{\"data\":{{\"{type}\":", "is_final\":true}}") + "is_final\":true}}";
                return JsonString;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string SharePostBodyData( string ShareUrl="")
        {
            var jsonString = string.Empty;
            try
            {
               // var imgText = "{\"type\":\"image\",\"indent\":0,\"quoted\":false,\"is_rtl\":false,\"spans\":[{\"modifiers\":{\"image\":\"" + ImageUrl + "\"},\"text\":\"\"}]},{\"type\":\"plain\",\"indent\":0,\"quoted\":false,\"is_rtl\":false,\"spans\":[{\"modifiers\":{},\"text\":\"\"}]}]}";
                var _shareText = "{\"type\":\"hyperlink_embed\",\"indent\":0,\"quoted\":false,\"spans\":[{\"modifiers\":{\"embed\":{\"url\":\"" + ShareUrl + "\"}}}]},{\"type\":\"plain\",\"indent\":0,\"quoted\":false,\"is_rtl\":false,\"spans\":[{\"modifiers\":{},\"text\":\"\"}]}]}";
                var data = "{\"sections\":[{\"type\":\"plain\",\"indent\":0,\"quoted\":false,\"is_rtl\":false,\"spans\":[{\"modifiers\":{},\"text\":\"\"}]},";
                data = data + _shareText;
                QdCreatePostPostData qdCreatePost = new QdCreatePostPostData
                {
                    queryName = "draftUtils_postDraftSave_Mutation",
                    variables = new CreatePostVariable
                    {
                        tribeId = 0,
                        title = "{\"sections\":[]}",
                        content = data,
                        isAutoSaved = true,
                        isNullspacePost = true
                    },
                    extensions = new Extensions()
                    {
                        hash = "b3eee9e1677ff5af07c094da285039a3cbacee364e61f562a0c0495794145c2d"
                    }
                };
                jsonString = JsonConvert.SerializeObject(qdCreatePost);

            }
            catch (Exception) { }
            return jsonString;
        }
        public static string UploadedShareUrlPostBodyData( string ShareUrl, string CredentialID)
        {
            var jsonString = string.Empty;
            try
            {
                //var imgText = "{\"type\":\"image\",\"indent\":0,\"quoted\":false,\"is_rtl\":false,\"spans\":[{\"modifiers\":{\"image\":\"" + ImageUrl + "\"},\"text\":\"\"}]},{\"type\":\"plain\",\"indent\":0,\"quoted\":false,\"is_rtl\":false,\"spans\":[{\"modifiers\":{},\"text\":\"\"}]}]}";
                var _shareText = "{\"type\":\"hyperlink_embed\",\"indent\":0,\"quoted\":false,\"spans\":[{\"modifiers\":{\"embed\":{\"url\":\"" + ShareUrl + "\"}}}]},{\"type\":\"plain\",\"indent\":0,\"quoted\":false,\"is_rtl\":false,\"spans\":[{\"modifiers\":{},\"text\":\"\"}]}]}";
                var data = "{\"sections\":[{\"type\":\"plain\",\"indent\":0,\"quoted\":false,\"is_rtl\":false,\"spans\":[{\"modifiers\":{},\"text\":\"\"}]},";
                data = data + _shareText;
                QdUploadPostPostData qdUploadPost = new QdUploadPostPostData
                {
                    queryName = "AskQuestionStepShareLinkWrapper_postAdd_Mutation",
                    variables = new UploadPostVariable
                    {
                        tribeId = 0,
                        title = "{\"sections\":[]}",
                        content = data,
                        shouldQueue = false,
                        credentialId = (CredentialID.Contains("null")) ? null : CredentialID,
                        accessOption = null,
                        captcha = null
                    },
                    extensions = new Extensions()
                    {
                        hash = "7b738ec94591e759e2e3ae3762f598b3390d97fafa514f037ab125f8ab15854a"
                    }
                };
                jsonString = JsonConvert.SerializeObject(qdUploadPost);
            }
            catch (Exception) { }
            return jsonString;
        }
        #region Ads Scrapper API and post data.
        public static string AdsScrapperAPI => $"https://www.quora.com/graphql/gql_para_POST?q=MultifeedQuery";
        public static string AdsPostData(string PaginationID,int NumBundleClient=6) => $"{{\"queryName\":\"MultifeedQuery\",\"variables\":{{\"first\":8,\"multifeedAfter\":\"{PaginationID}\",\"multifeedNumBundlesOnClient\":{NumBundleClient},\"injectionType\":null,\"injectionData\":null,\"filterStoryType\":null,\"filterStoryOid\":null,\"multifeedPage\":\"home_page\",\"pageData\":0,\"showLiveBanner\":false}},\"extensions\":{{\"hash\":\"2cf4707cb87272fdf561c7ef4668d337f1a4b24994734c6c08855a277113bb56\"}}}}";
        public static string UpdateAdsInDev => $"https://quora-dev.poweradspy.com/insertads";
        public static string UpdateAdsInMain => $"https://qapi.poweradspy.com/insertads";
        #endregion
    }
}