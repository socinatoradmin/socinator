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
    public class ScrapUserWhoMessagedUsProcessorTest: BaseGdProcessTest
    {
        private ActivityType _activityType;
        IGdJobProcess _gdJobProcess;
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _gdJobProcess = Substitute.For<IGdJobProcess>();
            _activityType = ActivityType.Follow;
            var cancleJob = new CancellationTokenSource();
            _gdJobProcess.ActivityType.Returns(_activityType);          
            var module = new ModuleConfiguration();
            JobActivityConfigurationManager[AccountId, _activityType].Returns(module);
            FollowerModel FollowerModel = new FollowerModel();
            ProcessScopeModel.GetActivitySettingsAs<FollowerModel>().Returns(FollowerModel);
            var jsonData = JsonConvert.SerializeObject(GdModuleSetting);
            var model = new TemplateModel() { ActivitySettings = jsonData };
            var templatesFileManager = CommonServiceLocator.ServiceLocator.Current.GetInstance<DominatorHouseCore.FileManagers.ITemplatesFileManager>();
            templatesFileManager.GetTemplateById(Arg.Any<string>()).Returns(model);
            _gdJobProcess.DominatorAccountModel.Returns(GdDominatorAccountModel);
            _gdJobProcess.JobCancellationTokenSource.Returns(cancleJob);
            _gdJobProcess.ModuleSetting.Returns(GdModuleSetting);
            _gdJobProcess.instaFunct = _gdJobProcess.loginProcess.InstagramFunctFactory.InstaFunctions;
        }

        [TestMethod]
        [Timeout(30000)]
        public void Should_Search_Get_ScrapUserWhoMessagedUs()
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

            var GetVisualThreadpageresponse = TestUtils.ReadFileFromResources(
               "GramDominatorCore.UnitTests.TestData.GetVisualThreadResponse.json", Assembly.GetExecutingAssembly());
            var Getv2Inboxpageresponse = TestUtils.ReadFileFromResources(
              "GramDominatorCore.UnitTests.TestData.Getv2InboxResponse.json", Assembly.GetExecutingAssembly());          
            var userinfo = TestUtils.ReadFileFromResources(
              "GramDominatorCore.UnitTests.TestData.AccountUserInfo.json", Assembly.GetExecutingAssembly());
            VisualThreadResponse visualThreadResponse = new VisualThreadResponse(new ResponseParameter
            {
                Response = GetVisualThreadpageresponse
            });
            V2InboxResponse v2Inbox = new V2InboxResponse(new ResponseParameter
            {
                Response = Getv2Inboxpageresponse
            });
            UsernameInfoIgResponseHandler username = new UsernameInfoIgResponseHandler(new ResponseParameter
            {
                Response = userinfo
            });
            //act
            _gdJobProcess.instaFunct.Getv2Inbox(Arg.Any<DominatorAccountModel>(), Arg.Any<bool>(), Arg.Any<string>()).Returns(v2Inbox);
            _gdJobProcess.instaFunct.GetVisualThread(Arg.Any<DominatorAccountModel>(),Arg.Any<string>(), Arg.Any<string>()).Returns(visualThreadResponse);
            _gdJobProcess.instaFunct.SearchUsername(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(username);

            ScrapUserWhoMessagedUsProcessor scrapUserWhoMessagedUsProcessor =
                new ScrapUserWhoMessagedUsProcessor(_gdJobProcess, AccountServiceScoped, CampaignService,
                    ProcessScopeModel, DelayService);
            scrapUserWhoMessagedUsProcessor.Start(queryInfo);
            //assert
            _gdJobProcess.Received(0).FinalProcess(Arg.Any<ScrapeResultNew>());
            _gdJobProcess.Received(3).instaFunct.Getv2Inbox(Arg.Any<DominatorAccountModel>(), Arg.Any<bool>(), Arg.Any<string>());
            _gdJobProcess.Received(3).instaFunct.GetVisualThread(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<string>());
            _gdJobProcess.Received(3).instaFunct.SearchUsername(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>());

        }
    }
}
