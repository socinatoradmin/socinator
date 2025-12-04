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
    public class FollowersOfSomeoneFollowersProcessorTest:BaseGdProcessTest
    {
        private ActivityType _activityType;
        InstaFunctTest objIinstaFunctTest = new InstaFunctTest();
        IGdJobProcess _gdJobProcess;
        private CancellationToken CancellationToken { get; set; }
        
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
        [Timeout(15000)]
        public void should_search_For_FollowersOfFollowers()
        {          
            //arrange 
             var queryInfo = new QueryInfo
            {
                QueryType = "",
                QueryValue = "sachin"
            };
            var jobResult = new JobProcessResult();          
            _gdJobProcess.FinalProcess(Arg.Any<ScrapeResultNew>()).Returns(jobResult);
        
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            var Followerpageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.GetUserFollowersResponse.json", Assembly.GetExecutingAssembly());

            var Userpageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.SearchUserNameResponse.json", Assembly.GetExecutingAssembly());

            UsernameInfoIgResponseHandler userResponse = new UsernameInfoIgResponseHandler(new ResponseParameter() { Response = Userpageresponse });
            FollowerAndFollowingIgResponseHandler FollowResponse = new FollowerAndFollowingIgResponseHandler(new ResponseParameter() { Response = Followerpageresponse });
            //act
            _gdJobProcess.instaFunct.GetUserFollowers(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>(), Arg.Any<string>(), Arg.Any<string>()).Returns(FollowResponse);
            _gdJobProcess.instaFunct.SearchUsername(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(userResponse);

            FollowersOfFollowersProcessor followersOfFollowersProcessor =
                new FollowersOfFollowersProcessor(_gdJobProcess, AccountServiceScoped, CampaignService,
                    ProcessScopeModel, DelayService);
            followersOfFollowersProcessor.Start(queryInfo);
            //assert
            _gdJobProcess.Received(4).FinalProcess(Arg.Any<ScrapeResultNew>());
            _gdJobProcess.instaFunct.Received(1).SearchUsername(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
          //  _gdJobProcess.instaFunct.Received(1).GetUserFollowers(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>());

        }
    }
}
