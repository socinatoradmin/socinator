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
    public class UnlikingPostProcessorTest : BaseGdProcessTest
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
        public void Should_Search_UnlikePost()
        {
            //arrange
            var jobResult = new JobProcessResult();
            _gdJobProcess.FinalProcess(Arg.Any<ScrapeResultNew>()).Returns(jobResult);
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);

            //var GetFeedpageresponse = TestUtils.ReadFileFromResources(
            //    "GramDominatorCore.UnitTests.TestData.GetUserFeedResponse.json", Assembly.GetExecutingAssembly());
            var GetMediaCommentpageresponse = TestUtils.ReadFileFromResources(
               "GramDominatorCore.UnitTests.TestData.GetLikedMediaForUnlikeResponse.json", Assembly.GetExecutingAssembly());

            //UserFeedIgResponseHandler GetUserFeed = new UserFeedIgResponseHandler(new ResponseParameter()
            //{
            //    Response = GetFeedpageresponse
            //});
            UserFeedResponse mediaLike = new UserFeedResponse(new ResponseParameter()
            {
                Response = GetMediaCommentpageresponse
            });
            //act
            _gdJobProcess.instaFunct.GetLikedMedia(Arg.Any<DominatorAccountModel>()).Returns(mediaLike);
            //_gdJobProcess.instaFunct.GetUserFeed(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>()).Returns(GetUserFeed);
            UnlikingPostProcessor unlikingPostProcessor = new UnlikingPostProcessor(_gdJobProcess, AccountServiceScoped,
                CampaignService, ProcessScopeModel, DelayService);
            unlikingPostProcessor.Start(QueryInfo.NoQuery);
            //assert
            _gdJobProcess.Received(21).FinalProcess(Arg.Any<ScrapeResultNew>());
            _gdJobProcess.Received(1).instaFunct.GetMediaLikers(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
            //_gdJobProcess.Received(2).instaFunct.GetUserFeed(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>());
        }
    }
}
