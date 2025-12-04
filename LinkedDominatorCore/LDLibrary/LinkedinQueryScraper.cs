using System;
using System.Collections.Generic;
using System.Threading;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.LdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.LDLibrary.Processor;
using LinkedDominatorCore.LDLibrary.Processor.Companies;
using LinkedDominatorCore.LDLibrary.Processor.Group;
using LinkedDominatorCore.LDLibrary.Processor.Group.NonQueryType;
using LinkedDominatorCore.LDLibrary.Processor.Jobs;
using LinkedDominatorCore.LDLibrary.Processor.Page;
using LinkedDominatorCore.LDLibrary.Processor.Posts;
using LinkedDominatorCore.LDLibrary.Processor.Users;
using LinkedDominatorCore.LDLibrary.Processor.Users.NonQueryType;
using LinkedDominatorCore.Utility;
using Unity;

namespace LinkedDominatorCore.LDLibrary
{
    internal class LinkedinQueryScraper : QueryScraper
    {
        public LinkedinQueryScraper(IJobProcess jobProcess,
            Dictionary<string, Action<QueryInfo>> scrapeWithQueriesActionTable,
            Dictionary<string, Action> scrapeWithoutQueriesActionTable)
            : base(jobProcess, scrapeWithQueriesActionTable, scrapeWithoutQueriesActionTable)
        {
        }

        //private static readonly ILogger Logger = LogManager.GetLogger(nameof(LinkedinScraper));
    }

    public interface ILinkedInScraperActionTables : IScraperActionTables
    {
    }

    //TwitterScraperActionTables
    public class LinkedInScraperActionTables : ILinkedInScraperActionTables
    {
        private readonly ILdJobProcess _jobProcess;

        private readonly IUnityContainer _unityContainer;

        public LinkedInScraperActionTables(ILdJobProcess jobProcess, IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
            _jobProcess = jobProcess;
            try
            {
                DominatorAccountModel = jobProcess.DominatorAccountModel;
                jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                ActivityType = jobProcess.ActivityType;

                ScrapeWithQueriesActionTable = new Dictionary<string, Action<QueryInfo>>
                {
                    {
                        $"{ActivityType.ConnectionRequest}{EnumUtility.GetQueryFromEnum(LDGrowConnectionUserQueryParameters.Keyword)}",
                        StartProcessWithUserKeywordsProcessor
                    },
                    {
                        $"{ActivityType.ConnectionRequest}{EnumUtility.GetQueryFromEnum(LDGrowConnectionUserQueryParameters.ProfileUrl)}",
                        StartProcessWithUrl
                    },
                    {
                        $"{ActivityType.ConnectionRequest}{EnumUtility.GetQueryFromEnum(LDGrowConnectionUserQueryParameters.Email)}",
                        StartProcessWithEmail
                    },
                    {
                        $"{ActivityType.ConnectionRequest}{EnumUtility.GetQueryFromEnum(LDGrowConnectionUserQueryParameters.SearchUrl)}",
                        StartProcessWithUserSearchUrlProcessor
                    },
                    {
                        $"{ActivityType.ConnectionRequest}{EnumUtility.GetQueryFromEnum(LDScraperUserQueryParameters.JoinedGroupUrl)}",
                        StartProcessGroupMembers
                    },
                    {
                        $"{ActivityType.ConnectionRequest}{EnumUtility.GetQueryFromEnum(LDGrowConnectionUserQueryParameters.SalesNavUserScraperCampaign)}",
                        StartProcessWithSalesNavUserScrapedProcessor
                    },
                    {
                        $"{ActivityType.ConnectionRequest}{EnumUtility.GetQueryFromEnum(LDGrowConnectionUserQueryParameters.SalesNavigatorSearchUrl)}",
                        StartProcessWithUserSearchUrlProcessor
                    },
                    {
                        $"{ActivityType.ConnectionRequest}{EnumUtility.GetQueryFromEnum(LDGrowConnectionUserQueryParameters.JobScraperCampaign)}",
                        startProcessWithJobScraperProcessor
                    },


                    {
                        $"{ActivityType.FollowPages}{EnumUtility.GetQueryFromEnum(LDGrowConnectionUserQueryParameters.Keyword)}",
                        StartProcessWithPageKeywordsProcessor
                    },
                    {
                        $"{ActivityType.FollowPages}{EnumUtility.GetQueryFromEnum(LDGrowConnectionUserQueryParameters.PageUrl)}",
                        StartProcessWithPageUrlProcessor
                    },

                    {
                        $"{ActivityType.GroupJoiner}{EnumUtility.GetQueryFromEnum(LDGroupQueryParameters.Keyword)}",
                        StartProcessWithGroupKeywordsProcessor
                    },

                    {
                        $"{ActivityType.UserScraper}{EnumUtility.GetQueryFromEnum(LDScraperUserQueryParameters.Keyword)}",
                        StartProcessWithUserKeywordsProcessor
                    },
                    {
                        $"{ActivityType.UserScraper}{EnumUtility.GetQueryFromEnum(LDScraperUserQueryParameters.ProfileUrl)}",
                        StartProcessWithUrl
                    },
                    {
                        $"{ActivityType.UserScraper}{EnumUtility.GetQueryFromEnum(LDScraperUserQueryParameters.JoinedGroupUrl)}",
                        StartProcessGroupMembers
                    },
                    {
                        $"{ActivityType.UserScraper}{EnumUtility.GetQueryFromEnum(LDScraperUserQueryParameters.Only1stConnection)}",
                        StartProcessForOnly1StConnection
                    },
                    {
                        $"{ActivityType.UserScraper}{EnumUtility.GetQueryFromEnum(LDScraperUserQueryParameters.SearchUrl)}",
                        StartProcessWithUserSearchUrlProcessor
                    },

                    {
                        $"{ActivityType.CompanyScraper}{EnumUtility.GetQueryFromEnum(LDScraperUserQueryParameters.SearchUrl)}",
                        StartProcessWithCompanySearchUrlProcessor
                    },

                    {
                        $"{ActivityType.JobScraper}{EnumUtility.GetQueryFromEnum(LDScraperUserQueryParameters.SearchUrl)}",
                        StartProcessWithJobSearchUrlProcessor
                    },
                    {
                        $"{ActivityType.Like}{EnumUtility.GetQueryFromEnum(LDEngageQueryParameters.SomeonesPosts)}",
                        StartSomeonesPostsProcess
                    },
                    {
                        $"{ActivityType.Like}{EnumUtility.GetQueryFromEnum(LDEngageQueryParameters.CompanyUrlPost)}",
                        StartCompanyPostUrlProcess
                    },
                    {
                        $"{ActivityType.Like}{EnumUtility.GetQueryFromEnum(LDEngageQueryParameters.HashtagUrlPost)}",
                        StartHashtagpostUrlProcess
                    },
                    {
                        $"{ActivityType.Comment}{EnumUtility.GetQueryFromEnum(LDEngageQueryParameters.HashtagUrlPost)}",
                        StartHashtagpostUrlProcess
                    },
                    {
                        $"{ActivityType.Comment}{EnumUtility.GetQueryFromEnum(LDEngageQueryParameters.Keyword)}",
                        StartKeywordProcess
                    },
                    {
                        $"{ActivityType.Comment}{EnumUtility.GetQueryFromEnum(LDEngageQueryParameters.SomeonesPosts)}",
                        StartSomeonesPostsProcess
                    },
                    {
                        $"{ActivityType.Like}{EnumUtility.GetQueryFromEnum(LDEngageQueryParameters.MyConnectionsPosts)}",
                        StartMyConnectionsPostsProcess
                    },
                    {
                        $"{ActivityType.Comment}{EnumUtility.GetQueryFromEnum(LDEngageQueryParameters.MyConnectionsPosts)}",
                        StartMyConnectionsPostsProcess
                    },
                    {
                        $"{ActivityType.Share}{EnumUtility.GetQueryFromEnum(LDEngageQueryParameters.MyConnectionsPosts)}",
                        StartMyConnectionsPostsProcess
                    },
                    {
                        $"{ActivityType.Like}{EnumUtility.GetQueryFromEnum(LDEngageQueryParameters.GroupsUrlPosts)}",
                        StartGroupUrlPostsProcess
                    },
                    {
                        $"{ActivityType.Comment}{EnumUtility.GetQueryFromEnum(LDEngageQueryParameters.GroupsUrlPosts)}",
                        StartGroupUrlPostsProcess
                    },
                    {
                        $"{ActivityType.Like}{EnumUtility.GetQueryFromEnum(LDEngageQueryParameters.CustomPosts)}",
                        StartCustomUrlPostsProcess
                    },
                    {
                        $"{ActivityType.Comment}{EnumUtility.GetQueryFromEnum(LDEngageQueryParameters.CustomPosts)}",
                        StartCustomUrlPostsProcess
                    },
                    //{$"{ActivityType.Share}{EnumUtility.GetQueryFromEnum(LDEngageQueryParameters.CustomPosts)}",StartCustomUrlPostsProcess },
                    {
                        $"{ActivityType.Share}{EnumUtility.GetQueryFromEnum(LDEngageQueryParameters.SomeonesPosts)}",
                        StartSomeonesPostsProcess
                    },
                    {
                        $"{ActivityType.GroupInviter}{EnumUtility.GetQueryFromEnum(LDScraperUserQueryParameters.Only1stConnection)}",
                        StartProcessForOnly1StConnection
                    },
                    {
                        $"{ActivityType.GroupInviter}{EnumUtility.GetQueryFromEnum(LDScraperUserQueryParameters.ProfileUrl)}",
                        StartProcessWithUrl
                    },
                    {
                        $"{ActivityType.SalesNavigatorUserScraper}{EnumUtility.GetQueryFromEnum(LDScraperUserQueryParameters.Keyword)}",
                        StartProcessWithUserKeywordsProcessor
                    },
                    {
                        $"{ActivityType.SalesNavigatorUserScraper}{EnumUtility.GetQueryFromEnum(LDScraperUserQueryParameters.ProfileUrl)}",
                        StartProcessWithUrl
                    },
                    {
                        $"{ActivityType.SalesNavigatorUserScraper}{EnumUtility.GetQueryFromEnum(LDScraperUserQueryParameters.SearchUrl)}",
                        StartProcessWithUserSearchUrlProcessor
                    },
                    {
                        $"{ActivityType.SalesNavigatorCompanyScraper}{EnumUtility.GetQueryFromEnum(LDScraperUserQueryParameters.SearchUrl)}",
                        StartProcessWithCompanySearchUrlProcessor
                    }
                };

                ScrapeWithoutQueriesActionTable = new Dictionary<string, Action>
                {
                    // {$"{jobProcess.ActivityType}",StartProcessNonQueryType},
                    {$"{ActivityType.RemoveConnections}", StartProcessConnectionOrUser},
                    {$"{ActivityType.WithdrawConnectionRequest}", StartWithDrawConnectionProcessor},
                    {$"{ActivityType.ExportConnection}", StartProcessConnectionOrUser},
                    {$"{ActivityType.BroadcastMessages}", StartProcessConnectionOrUser},
                    {$"{ActivityType.ProfileEndorsement}", StartProcessConnectionOrUser},
                    {$"{ActivityType.AcceptConnectionRequest}", StartProcessAcceptConnectionRequest},
                    {$"{ActivityType.AutoReplyToNewMessage}", StartProcessAutoReplyToNewMessage},
                    {$"{ActivityType.SendMessageToNewConnection}", StartProcessSendMessageToNewConnection},
                    {$"{ActivityType.SendGreetingsToConnections}", StartProcessSendGreetingsToConnections},
                    {$"{ActivityType.GroupUnJoiner}", StartProcessGroupUnJoiner},
                    {$"{ActivityType.BlockUser}", StartProcessBlockUser},
                    {$"{ActivityType.DeleteConversations}", DeleteConversation},
                    {$"{ActivityType.AttachmnetsMessageScraper}", MessageConversationScraper},
                    {$"{ActivityType.SendPageInvitations}", StartProcessSendPageInvitation},
                    {$"{ActivityType.EventInviter}", StartProcessForEventInvitation},
                    {$"{ActivityType.SendGroupInvitations}", StartProcessSendGroupInvitation}
                };

                Thread.CurrentThread.Name = DominatorAccountModel.AccountBaseModel.UserName;
            }
            catch (OperationCanceledException)
            {
                LDAccountsBrowserDetails.CloseBrowser(DominatorAccountModel);
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (AggregateException ae)
            {
                LDAccountsBrowserDetails.CloseBrowser(DominatorAccountModel);
                ae.DebugLog();
            }
            catch (Exception ex)
            {
                LDAccountsBrowserDetails.CloseBrowser(DominatorAccountModel);
                ex.DebugLog();
            }
        }

        private void StartProcessForEventInvitation()
        {
            IQueryProcessor processor = _unityContainer.Resolve<EventInviterProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }

        private void DeleteConversation()
        {
            IQueryProcessor processor = _unityContainer.Resolve<DeleteConversationsProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }

        private void MessageConversationScraper()
        {
            IQueryProcessor processor = _unityContainer.Resolve<MessageConversationProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }

        private void StartProcessGroupUnJoiner()
        {
            IQueryProcessor processor = _unityContainer.Resolve<GroupUnJoinerProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }

        #region Initializations Required For This Class

        public ActivityType ActivityType { get; set; }

        private DominatorAccountModel DominatorAccountModel { get; }


        public Dictionary<string, Action<QueryInfo>> ScrapeWithQueriesActionTable { get; set; }

        public Dictionary<string, Action> ScrapeWithoutQueriesActionTable { get; set; }

        #endregion


        #region Query processors

        protected void StartProcessWithUserKeywordsProcessor(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<UserKeywordsProcessor>();
            processor.Start(queryInfo);
        }

        protected void StartProcessWithGroupKeywordsProcessor(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<GroupKeywordsProcessor>();
            processor.Start(queryInfo);
        }
        //PageSearchUrlProcessor

        protected void StartProcessWithPageKeywordsProcessor(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<PageKeywordProcessor>();
            processor.Start(queryInfo);
        }


        protected void StartProcessWithPageUrlProcessor(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<PageUrlProcessor>();
            processor.Start(queryInfo);
        }

        protected void StartProcessWithEmail(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<EmailProcessor>();
            processor.Start(queryInfo);
            _jobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }

        protected void StartProcessForOnly1StConnection(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<Only1StConnectionProcessor>();
            processor.Start(queryInfo);
        }

        protected void StartProcessWithUrl(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<ProfileUrlProcessor>();
            processor.Start(queryInfo);
            _jobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }

        protected void StartProcessGroupMembers(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<GroupMembersProcessor>();
            processor.Start(queryInfo);
        }

        protected void StartProcessWithUserSearchUrlProcessor(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<UserSearchUrlProcessor>();
            processor.Start(queryInfo);
        }

        protected void startProcessWithJobScraperProcessor(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<JobPosterScraperProcessor>();
            processor.Start(queryInfo);
        }

        protected void StartProcessWithSalesNavUserScrapedProcessor(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<SalesNavUserScrapedProcessor>();
            processor.Start(queryInfo);
        }

        protected void StartProcessWithCompanySearchUrlProcessor(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<CompanySearchUrlProcessor>();
            processor.Start(queryInfo);
        }

        protected void StartProcessWithJobSearchUrlProcessor(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<JobSearchUrlProcessor>();
            processor.Start(queryInfo);
        }

        protected void StartSomeonesPostsProcess(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<SomeonesPostsProcessor>();
            processor.Start(queryInfo);
        }
        protected void StartKeywordProcess(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<KeywordPostsProcessor>();
            processor.Start(queryInfo);
        }
        protected void StartMyConnectionsPostsProcess(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<MyConnectionsPostsProcessor>();
            processor.Start(queryInfo);
        }


        protected void StartGroupUrlPostsProcess(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<GroupUrlPostsProcessor>();
            processor.Start(queryInfo);
        }

        protected void StartCompanyPostUrlProcess(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<CompanyUrlPostsProcessor>();
            processor.Start(queryInfo);
        }

        protected void StartHashtagpostUrlProcess(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<HashtagUrlPostProcessor>();
            processor.Start(queryInfo);
        }

        protected void StartCustomUrlPostsProcess(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<CustomUrlPostsProcessor>();
            processor.Start(queryInfo);
        }

        #endregion

        #region Non Query Processors

        private void StartProcessConnectionOrUser()
        {
            var processor = _unityContainer.Resolve<ConnectionOrUserNonQueryProcessor>();
            processor.UnityContainer = _unityContainer;
            processor.Start(QueryInfo.NoQuery);
        }

        private void StartWithDrawConnectionProcessor()
        {
            var processor = _unityContainer.Resolve<WithDrawConnectionProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }

        private void StartProcessAcceptConnectionRequest()
        {
            IQueryProcessor processor = _unityContainer.Resolve<AcceptConnectionRequestProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }

        private void StartProcessAutoReplyToNewMessage()
        {
            IQueryProcessor processor = _unityContainer.Resolve<AutoReplyToNewMessageProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }

        private void StartProcessSendMessageToNewConnection()
        {
            IQueryProcessor processor = _unityContainer.Resolve<SendMessageToNewConnectionProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }

        private void StartProcessSendGreetingsToConnections()
        {
            IQueryProcessor processor = _unityContainer.Resolve<SendGreetingsToConnectionsProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }

        private void StartProcessSendPageInvitation()
        {
            IQueryProcessor processor = _unityContainer.Resolve<SendPageInvitationProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }
        private void StartProcessSendGroupInvitation()
        {
            IQueryProcessor processor = _unityContainer.Resolve<SendGroupMemberInvitationProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }

        private void StartProcessBlockUser()
        {
            IQueryProcessor processor = _unityContainer.Resolve<BlockUserProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }

        #endregion
    }
}