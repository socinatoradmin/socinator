using Dominator.Tests.Utils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary;
using GramDominatorCore.GDLibrary.Processor.Post;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using GramDominatorCore.Response;
using GramDominatorCore.UnitTests.Test_Data.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NSubstitute;
using System.Reflection;
using System.Threading;

namespace GramDominatorCore.UnitTests.Tests.Library.ProcessorTest.PostProcessors
{
    [TestClass]
    public class FollowersOfFollowingsPostProcessorTest:BaseGdProcessTest
    {
        private ActivityType _activityType;
        IGdJobProcess _gdJobProcess;
       
        private CancellationToken CancellationToken { get; set; }
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _gdJobProcess = Substitute.For<IGdJobProcess>();
            _activityType = ActivityType.Comment;
            var cancleJob = new CancellationTokenSource();
            _gdJobProcess.ActivityType.Returns(_activityType);
            var module = new ModuleConfiguration();
            JobActivityConfigurationManager[AccountId, _activityType].Returns(module);
            CommentModel commentModel = new CommentModel();
            ProcessScopeModel.GetActivitySettingsAs<CommentModel>().Returns(commentModel);
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
        public void should_search_For_FollowersOfFollowings_Will_return_true()
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

            var usernamepageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.SearchUserNameResponse.json", Assembly.GetExecutingAssembly());
            UsernameInfoIgResponseHandler username = new UsernameInfoIgResponseHandler(new ResponseParameter()
            {
                Response = usernamepageresponse
            });

            var Followingpageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.GetUserFollowingResponse.json", Assembly.GetExecutingAssembly());
            FollowerAndFollowingIgResponseHandler FollowingResponse = new FollowerAndFollowingIgResponseHandler(new ResponseParameter()
            {
                Response = Followingpageresponse
            });

            var Followerpageresponse = TestUtils.ReadFileFromResources(
               "GramDominatorCore.UnitTests.TestData.GetUserFollowersResponse.json", Assembly.GetExecutingAssembly());
            FollowerAndFollowingIgResponseHandler FollowerResponse = new FollowerAndFollowingIgResponseHandler(new ResponseParameter()
            {
                Response = Followerpageresponse
            });

            var UserFeedpageresponse = TestUtils.ReadFileFromResources(
               "GramDominatorCore.UnitTests.TestData.GetUserFeedResponse.json", Assembly.GetExecutingAssembly());
            UserFeedIgResponseHandler UserFeed = new UserFeedIgResponseHandler(new ResponseParameter()
            {
                Response = UserFeedpageresponse
            });
            //act
            _gdJobProcess.instaFunct.SearchUsername(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(username);
            _gdJobProcess.instaFunct.GetUserFollowings(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>(), Arg.Any<string>(), Arg.Any<string>()).Returns(FollowingResponse);
            _gdJobProcess.instaFunct.GetUserFollowers(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>(), Arg.Any<string>(), Arg.Any<string>()).Returns(FollowerResponse);
            _gdJobProcess.instaFunct.GetUserFeed(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>()).Returns(UserFeed);

            FollowersOfFollowingsPostProcessor followersOfFollowersProcessor = new FollowersOfFollowingsPostProcessor(_gdJobProcess, AccountServiceScoped, CampaignService, ProcessScopeModel, DelayService);
            followersOfFollowersProcessor.Start(queryInfo);
            //assert
            _gdJobProcess.Received(4).FinalProcess(Arg.Any<ScrapeResultNew>());
            _gdJobProcess.instaFunct.Received(1).SearchUsername(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
            _gdJobProcess.instaFunct.Received(1).GetUserFollowers(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [TestMethod]
        [Timeout(15000)]
        public void should_search_For_FollowersOfFollowings_Will_return_User_Not_Found()
        {
            var queryInfo = new QueryInfo
            {
                QueryType = "",
                QueryValue = "sachin"
            };
            var jobResult = new JobProcessResult();
            _gdJobProcess.FinalProcess(Arg.Any<ScrapeResultNew>()).Returns(jobResult);
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            UsernameInfoIgResponseHandler username = new UsernameInfoIgResponseHandler(new ResponseParameter()
            {
                Response = "{\"message\": \"User not found\",\"status\": \"fail\"}"
            });
            _gdJobProcess.instaFunct.SearchUsername(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(username);
            FollowersOfFollowersPostProcessor followersOfFollowersProcessor =
                new FollowersOfFollowersPostProcessor(_gdJobProcess, AccountServiceScoped, CampaignService,
                    ProcessScopeModel, DelayService);
            followersOfFollowersProcessor.Start(queryInfo);
            _gdJobProcess.Received(0).FinalProcess(Arg.Any<ScrapeResultNew>());
        }
    }
}
