using Dominator.Tests.Utils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.GdQuery;
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
using GramDominatorCore.UnitTests.Tests.Library.UserProcessors.ProcessorTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NSubstitute;
using System;
using System.Reflection;
using System.Threading;

namespace GramDominatorCore.UnitTests.Tests.Library.ProcessorTest.UserProcessors
{
    [TestClass]
    public class HashTagUserProcessorTest : BaseGdProcessTest
    {
        ActivityType _activityType;
        private HashTagUsersProcessors hashtagUserProcessor;
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
        }

        [TestMethod]
        public void Should_Search_Get_HashTag_User()
        {
            //arrange
            var queryInfo = new QueryInfo
            {
                QueryType = "HashTagUser",
                QueryValue = "nehakakkar"
            };
            var jobResult = new JobProcessResult();          
            _gdJobProcess.FinalProcess(Arg.Any<ScrapeResultNew>()).Returns(jobResult);
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            var HashTagFeedPageResponse = TestUtils.ReadFileFromResources(
               "GramDominatorCore.UnitTests.TestData.GetHashTagFeed.json", Assembly.GetExecutingAssembly());

            FeedIgResponseHandler HashTagResponse = new FeedIgResponseHandler(new ResponseParameter() { Response = HashTagFeedPageResponse });
            //act
            _gdJobProcess.instaFunct.GetHashtagFeed(Arg.Any<DominatorAccountModel>(), Arg.Any<string>()).Returns(HashTagResponse);                      
            hashtagUserProcessor = new HashTagUsersProcessors(_gdJobProcess, AccountServiceScoped, CampaignService, ProcessScopeModel, DelayService);
            hashtagUserProcessor.Start(queryInfo);
            //assert
            _gdJobProcess.Received(1).instaFunct.GetHashtagFeed(Arg.Any<DominatorAccountModel>(), Arg.Any<string>());
        }
    }
}
