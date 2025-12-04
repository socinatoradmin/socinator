using Dominator.Tests.Utils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using GramDominatorCore.Response;
using GramDominatorCore.UnitTests.Tests.Library.ProcessorTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NSubstitute;
using SQLite;
using System.Reflection;
using System.Threading;
using FluentAssertions;

namespace GramDominatorCore.UnitTests.Tests.Library.InstaProcess_Test
{
    [TestClass]
    public class CommentprocessTest : BaseGdProcessTest
    {
        private ActivityType _activityType;
        CommentModel _commentModel;
        CommentProcess _process;
        private ScrapeResultNew _scrapeResultNew;
        IInstaFunction _instafunct;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            _activityType = ActivityType.Comment;
            GdJobProcess.ActivityType.Returns(_activityType);
            _instafunct = Substitute.For<IInstaFunction>();
            var module = new ModuleConfiguration();
            JobActivityConfigurationManager[AccountId, _activityType].Returns(module);
            _commentModel = new CommentModel();
            ProcessScopeModel.GetActivitySettingsAs<CommentModel>().Returns(_commentModel);
            var jsonData = JsonConvert.SerializeObject(_commentModel);
            var model = new TemplateModel() {ActivitySettings = jsonData};
            var templatesFileManager = CommonServiceLocator.ServiceLocator.Current.GetInstance<ITemplatesFileManager>();
            templatesFileManager.GetTemplateById(Arg.Any<string>()).Returns(model);
            ProcessScopeModel.ActivityType.Returns(ActivityType.Comment);
            _process = new CommentProcess(ProcessScopeModel, AccountServiceScoped, GdQueryScraperFactory, GdHttpHelper, GdLogInProcess, GdBrowserManager, DelayService);
        }

        [TestMethod]
        public void Check_CommentProcess()
        {
            //arrange
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultPost = new InstagramPost
                {
                    Code = "abc",
                    MediaType = MediaType.Image,
                    User = new InstagramUser()
                    {
                        Username = "sachin"
                    }
                },
                QueryInfo = new QueryInfo()
                {
                    QueryType = "Keyword",
                    QueryValue = "sachin"
                }
            };
            _instafunct = GdLogInProcess.InstagramFunctFactory.InstaFunctions;
            ((GdJobProcess) _process).instaFunct = _instafunct;
            //instaFunct = loginProcess.InstagramFunctFactory.InstaFunctions;
           
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            var commentPageResponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.CommentResponse.json", Assembly.GetExecutingAssembly());
            var CheckOffensiveResponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.CheckOffensiveComment.json", Assembly.GetExecutingAssembly());
            CommentResponse commentResponse = new CommentResponse(new ResponseParameter
            {
                Response = commentPageResponse
            });
            CheckOffensiveCommentResponseHandler CheckOffensiveComment = new CheckOffensiveCommentResponseHandler(new ResponseParameter
            {
                Response= CheckOffensiveResponse
            });
            //act
            _instafunct.Comment(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<CancellationToken>(),
                Arg.Any<string>(), Arg.Any<string>()).Returns(commentResponse);
            _instafunct.CheckOffensiveComment(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<CancellationToken>(),
                Arg.Any<string>(), Arg.Any<string>()).Returns(CheckOffensiveComment);
            var commentProcess = _process.PostScrapeProcess(_scrapeResultNew);
            //assert
            _instafunct.Received(1).Comment(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(),
                Arg.Any<CancellationToken>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [TestMethod]
        public void Check_CommentProcess_Will_Return_False()
        {
            //arrange
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultPost = new InstagramPost
                {
                    Code = "abc",
                    MediaType = MediaType.Image,
                    User = new InstagramUser()
                    {
                        Username = "sachin"
                    }
                },
                QueryInfo = new QueryInfo()
                {
                    QueryType = "Keyword",
                    QueryValue = "sachin"
                }
            };
            _instafunct = GdLogInProcess.InstagramFunctFactory.InstaFunctions;
            ((GdJobProcess) _process).instaFunct = _instafunct;
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            var CheckOffensiveResponse = TestUtils.ReadFileFromResources(
              "GramDominatorCore.UnitTests.TestData.CheckOffensiveComment.json", Assembly.GetExecutingAssembly());

            CommentResponse commentResponse = new CommentResponse(new ResponseParameter
            {
                Response =
                    "{ \"message\": \"feedback_required\",\"spam\": true,\"feedback_title\": \"Action Blocked\",\"feedback_message\": \"This action was blocked. Please try again later. We restrict certain content and actions to protect our community. Tell us if you think we made a mistake.\",\"feedback_action\": \"report_problem\",\"status\": \"fail\" }"
            });
            CheckOffensiveCommentResponseHandler CheckOffensiveComment = new CheckOffensiveCommentResponseHandler(new ResponseParameter
            {
                Response = CheckOffensiveResponse
            });
            //act
            _instafunct.CheckOffensiveComment(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<CancellationToken>(),
               Arg.Any<string>(), Arg.Any<string>()).Returns(CheckOffensiveComment);
            _instafunct.Comment(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<CancellationToken>(),
                Arg.Any<string>(), Arg.Any<string>()).Returns(commentResponse);
            var commentProcess = _process.PostScrapeProcess(_scrapeResultNew);
            //assert
            _instafunct.Received(1).Comment(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(),
                Arg.Any<CancellationToken>(), Arg.Any<string>(), Arg.Any<string>());
            commentProcess.IsProcessSuceessfull.Should().BeFalse();
        }
    }
}
