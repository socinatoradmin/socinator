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
using GramDominatorCore.UnitTests.Test_Data.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NSubstitute;
using System.Reflection;
using System.Threading;

namespace GramDominatorCore.UnitTests.Tests.Library.ProcessorTest.UserProcessors
{
    [TestClass]
    public class SendMessageToNewFollowerProcessorTest : BaseGdProcessTest
    {
        private ActivityType _activityType;
        IGdJobProcess _gdJobProcess;
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _gdJobProcess = Substitute.For<IGdJobProcess>();
            _activityType = ActivityType.SendMessageToFollower;
            var cancleJob = new CancellationTokenSource();
            _gdJobProcess.ActivityType.Returns(_activityType);          
            var module = new ModuleConfiguration();
            JobActivityConfigurationManager[AccountId, _activityType].Returns(module);

            SendMessageToFollowerModel sendMessageModel = new SendMessageToFollowerModel();
            ProcessScopeModel.GetActivitySettingsAs<SendMessageToFollowerModel>().Returns(sendMessageModel);
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
        [Timeout(15000)]
        public void should_search_For_SendMessageToFollower()
        {
            // arrange    
            var queryInfo = new QueryInfo
            {
                QueryType = "",
                QueryValue = ""
            };
            var jobResult = new JobProcessResult();
            _gdJobProcess.FinalProcess(Arg.Any<ScrapeResultNew>()).Returns(jobResult);
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            var GetRecentFollowerResponse = TestUtils.ReadFileFromResources(
             "GramDominatorCore.UnitTests.TestData.GetRecentFollowersResponse.json", Assembly.GetExecutingAssembly());
            FollowerAndFollowingIgResponseHandler GetRecentResponse = new FollowerAndFollowingIgResponseHandler(new ResponseParameter
            {
                Response = GetRecentFollowerResponse
            });
            //act
            _gdJobProcess.instaFunct.GetRecentFollowers(Arg.Any<DominatorAccountModel>()).Returns(GetRecentResponse);
            SendMessageToNewFollowersProcessor SendMessageToNewFollowerProcessor =
                new SendMessageToNewFollowersProcessor(_gdJobProcess, AccountServiceScoped, CampaignService,
                    ProcessScopeModel, DelayService);
            SendMessageToNewFollowerProcessor.Start(queryInfo);
            //assert
            _gdJobProcess.Received(22).FinalProcess(Arg.Any<ScrapeResultNew>());
            _gdJobProcess.Received(1).instaFunct.GetRecentFollowers(Arg.Any<DominatorAccountModel>());
        }
    }
}
