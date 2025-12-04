using Dominator.Tests.Utils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GramDominatorCore.UnitTests.Tests.Library.ProcessorTest.PostProcessors
{
    [TestClass]
    public class TaggedPostProcessorTest : BaseGdProcessTest
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
        public void Should_Search_TaggedSomeonePost()
        {
            //arrange
            var queryInfo = new QueryInfo
            {
                QueryType = "",
                QueryValue = "243103112"
            };

            var jobResult = new JobProcessResult();           
            _gdJobProcess.FinalProcess(Arg.Any<ScrapeResultNew>()).Returns(jobResult);
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            var SearchUserNameResponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.SearchUserNameResponse.json", Assembly.GetExecutingAssembly());

            var GetFeedpageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.GetUserFeedResponse.json", Assembly.GetExecutingAssembly());
            var GetTaggedPostResponse = TestUtils.ReadFileFromResources(
               "GramDominatorCore.UnitTests.TestData.SomeoneTaggedPostResponse.json", Assembly.GetExecutingAssembly());
            UsernameInfoIgResponseHandler usernameResponse = new UsernameInfoIgResponseHandler(new ResponseParameter()
            {
                Response = SearchUserNameResponse
            });
            UserFeedIgResponseHandler GetUserFeed = new UserFeedIgResponseHandler(new ResponseParameter()
            {
                Response = GetFeedpageresponse
            });
            TaggedPostResponseHandler GetTaggedUser = new TaggedPostResponseHandler(new ResponseParameter()
            {
                Response = GetTaggedPostResponse
            });
            //act
            _gdJobProcess.instaFunct.SearchUsername(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(),Arg.Any<CancellationToken>()).Returns(usernameResponse);
            _gdJobProcess.instaFunct.GetUserFeed(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>()).Returns(GetUserFeed);
            _gdJobProcess.instaFunct.SomeoneTaggedPost(Arg.Any<DominatorAccountModel>(),Arg.Any<string>(), Arg.Any<CancellationToken>(), Arg.Any<string>()).Returns(GetTaggedUser);

            TaggedPostProcessor SomeoneFollowerPostProcessor = new TaggedPostProcessor(_gdJobProcess,
                AccountServiceScoped, CampaignService, ProcessScopeModel, DelayService);
            SomeoneFollowerPostProcessor.Start(queryInfo);
            //assert
            _gdJobProcess.Received(21).FinalProcess(Arg.Any<ScrapeResultNew>());
            _gdJobProcess.Received(3).instaFunct.SearchUsername(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
            _gdJobProcess.Received(3).instaFunct.GetUserFeed(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>());
            _gdJobProcess.Received(3).instaFunct.SomeoneTaggedPost(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>(), Arg.Any<string>());
        }
    }
}
