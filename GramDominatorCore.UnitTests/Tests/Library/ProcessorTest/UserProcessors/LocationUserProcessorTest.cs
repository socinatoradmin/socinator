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
    public class LocationUserProcessorTest : BaseGdProcessTest
    {
        private ActivityType _activityType;
        InstaFunctTest objIinstaFunctTest = new InstaFunctTest();
        IGdJobProcess _gdJobProcess;
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _gdJobProcess = Substitute.For<IGdJobProcess>();
            _activityType = ActivityType.Follow;
            var cancleJob = new CancellationTokenSource();
            _gdJobProcess.ActivityType.Returns(_activityType);
            objIinstaFunctTest = Substitute.For<InstaFunctTest>();
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
        public void should_search_For_LocationUser()
        {
            // arrange    
            var queryInfo = new QueryInfo
            {
                QueryType = "Keyword",
                QueryValue = "566966959"
            };
            var jobResult = new JobProcessResult();
            _gdJobProcess.FinalProcess(Arg.Any<ScrapeResultNew>()).Returns(jobResult);       
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            //var pageresponse = TestUtils.ReadFileFromResources(
            //                "GramDominatorCore.UnitTests.TestData.GetLocationFeedResponse.json", Assembly.GetExecutingAssembly());

            var pageresponseAlternate = TestUtils.ReadFileFromResources(
               "GramDominatorCore.UnitTests.TestData.GetLocationFeedAlternateResponse.json", Assembly.GetExecutingAssembly());


            //FeedIgResponseHandler feedResponse = new FeedIgResponseHandler(new ResponseParameter()
            //{ Response = pageresponse });

            FeedIgResponseHandlerAlternate FeedLocationAlternate = new FeedIgResponseHandlerAlternate(new ResponseParameter()
            { Response = pageresponseAlternate });

            //act
            //_gdJobProcess.instaFunct.GetLocationFeed(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(feedResponse);
            _gdJobProcess.instaFunct.GetLocationFeedAlternate(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>(), Arg.Any<string>(), Arg.Any<string>()).Returns(FeedLocationAlternate);
            LocationUsersProcessor keywordProcessor = new LocationUsersProcessor(_gdJobProcess, AccountServiceScoped,
                CampaignService, ProcessScopeModel, DelayService);
            keywordProcessor.Start(queryInfo);
            //assert
            _gdJobProcess.Received(9).FinalProcess(Arg.Any<ScrapeResultNew>());
           // _gdJobProcess.instaFunct.Received(1).GetLocationFeed(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
            _gdJobProcess.instaFunct.Received(1).GetLocationFeedAlternate(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>(), Arg.Any<string>(), Arg.Any<string>());
        }
    }
}
