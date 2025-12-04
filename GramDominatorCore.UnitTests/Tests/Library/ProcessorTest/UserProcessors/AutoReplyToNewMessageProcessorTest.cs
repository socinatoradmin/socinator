using Dominator.Tests.Utils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary;
using GramDominatorCore.GDLibrary.Processor.User;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using GramDominatorCore.Response;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NSubstitute;
using System.Reflection;
using System.Threading;

namespace GramDominatorCore.UnitTests.Tests.Library.ProcessorTest.UserProcessors
{
    [TestClass]
    public class AutoReplyToNewMessageProcessorTest : BaseGdProcessTest
    {
        private ActivityType _activityType;
        IGdJobProcess _gdJobProcess { get; set; }
        AutoReplyToNewMessageModel autoReplyModel;
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _gdJobProcess = Substitute.For<IGdJobProcess>();
            _activityType = ActivityType.AutoReplyToNewMessage;
            var cancleJob = new CancellationTokenSource();
            _gdJobProcess.ActivityType.Returns(_activityType);          
            var module = new ModuleConfiguration();
            JobActivityConfigurationManager[AccountId, _activityType].Returns(module);         
            autoReplyModel = new AutoReplyToNewMessageModel();
            autoReplyModel.IsReplyToPendingMessagesChecked = false;
            autoReplyModel.IsReplyToAllMessagesChecked = true;
            ProcessScopeModel.GetActivitySettingsAs<AutoReplyToNewMessageModel>().Returns(autoReplyModel);
            var jsonData = JsonConvert.SerializeObject(autoReplyModel);
            var model = new TemplateModel() { ActivitySettings = jsonData };         
            var templatesFileManager = CommonServiceLocator.ServiceLocator.Current.GetInstance<DominatorHouseCore.FileManagers.ITemplatesFileManager>();
            templatesFileManager.GetTemplateById(Arg.Any<string>()).Returns(model);
            _gdJobProcess.DominatorAccountModel.Returns(GdDominatorAccountModel);
            _gdJobProcess.JobCancellationTokenSource.Returns(cancleJob);
            _gdJobProcess.ModuleSetting.Returns(autoReplyModel);
            _gdJobProcess.instaFunct = _gdJobProcess.loginProcess.InstagramFunctFactory.InstaFunctions;
        }
        [TestMethod]
        public void Should_Search_AutoReplyMessageProcessor()
        {
            //arrange
            var queryInfo = new QueryInfo
            {
                QueryType = "",
                QueryValue = ""
            };
            var jobResult = new JobProcessResult();           
            _gdJobProcess.FinalProcess(Arg.Any<ScrapeResultNew>()).Returns(jobResult);
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            
            
            var pageresponse = TestUtils.ReadFileFromResources(
              "GramDominatorCore.UnitTests.TestData.Getv2InboxResponse.json", Assembly.GetExecutingAssembly());
            V2InboxResponse v2Inbox = new V2InboxResponse(new ResponseParameter
            {
                Response = pageresponse
            });

            _gdJobProcess.instaFunct.SearchUserInfoById(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>(),
                Arg.Any<CancellationToken>()).Returns(new UsernameInfoIgResponseHandler(new ResponseParameter
            {
                Response = "<!DOCTYPE html>"
            }));
            //act
            _gdJobProcess.instaFunct.Getv2Inbox(Arg.Any<DominatorAccountModel>(), Arg.Any<bool>(), Arg.Any<string>()).Returns(v2Inbox);
            AutoReplyToNewMessageProcessor autoReplyToNewMessageProcessor =
                new AutoReplyToNewMessageProcessor(_gdJobProcess, AccountServiceScoped, CampaignService,
                    ProcessScopeModel, DelayService);
            autoReplyToNewMessageProcessor.Start(queryInfo);
            //assert
            //_gdJobProcess.Received(6).FinalProcess(Arg.Any<ScrapeResultNew>());
            _gdJobProcess.instaFunct.Received(1).Getv2Inbox(Arg.Any<DominatorAccountModel>(), Arg.Any<bool>(), Arg.Any<string>());

        }
    }
}
