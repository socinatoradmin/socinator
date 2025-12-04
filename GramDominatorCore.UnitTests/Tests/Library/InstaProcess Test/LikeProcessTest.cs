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
using System.Reflection;
using System.Threading;
using FluentAssertions;

namespace GramDominatorCore.UnitTests.Tests.Library.InstaProcess_Test
{
    [TestClass]
    public class LikeProcessTest : BaseGdProcessTest
    {
        LikeModel _likeModel;
        LikeProcess _process;
        private ScrapeResultNew _scrapeResultNew;
        IInstaFunction _instafunct;
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            var _activityType = ActivityType.Comment;
            GdJobProcess.ActivityType.Returns(_activityType);
            _instafunct = Substitute.For<IInstaFunction>();
            var module = new ModuleConfiguration();
            JobActivityConfigurationManager[AccountId, _activityType].Returns(module);          
            _likeModel = new LikeModel();
            GdDominatorAccountModel.AccountBaseModel.UserId = "7150086983";
            ProcessScopeModel.GetActivitySettingsAs<LikeModel>().Returns(_likeModel);
            var jsonData = JsonConvert.SerializeObject(_likeModel);
            var model = new TemplateModel() { ActivitySettings = jsonData };
            var templatesFileManager = CommonServiceLocator.ServiceLocator.Current.GetInstance<ITemplatesFileManager>();
            templatesFileManager.GetTemplateById(Arg.Any<string>()).Returns(model);
            ProcessScopeModel.ActivityType.Returns(ActivityType.Like);
            _process = new LikeProcess(ProcessScopeModel, AccountServiceScoped, GdQueryScraperFactory, GdHttpHelper, GdLogInProcess,GdBrowserManager, DelayService);
        }

        [TestMethod]
        public void Check_LikeProcess()
        {
            //arrange
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultPost=new InstagramPost
                {

                    Pk= "1947724516487055651",
                    User=new InstagramUser
                    {
                        Pk = "7150086983",
                        Username = "jacksmith729",
                    }
                    
                },
                QueryInfo = new QueryInfo()
                {
                    
                }
            };
            _instafunct = GdLogInProcess.InstagramFunctFactory.InstaFunctions;
            ((GdJobProcess)_process).instaFunct = _instafunct;
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
         "GramDominatorCore.UnitTests.TestData.LikeResponse.json", Assembly.GetExecutingAssembly());

            string resp = "{ message: \"feedback_required\",spam: true,feedback_title: \"Action Blocked\",feedback_message: \"This action was blocked. Please try again later. We restrict certain content and actions to protect our community. Tell us if you think we made a mistake.\",feedback_url: \"repute/report_problem/instagram_like_add/\",feedback_appeal_label: \"Report problem\",feedback_ignore_label: \"OK\",feedback_action: \"report_problem\",status: \"fail\" },name: \"ActionSpamError\",message: \"This action was disabled due to block from instagram!\" }";
            LikeResponse likeResponse = new LikeResponse(new ResponseParameter
            {
                Response = pageresponse
            });
            //act
            _instafunct.Like(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<CancellationToken>(), Arg.Any<string>(), Arg.Any<string>() ,Arg.Any<string>(), Arg.Any<QueryInfo>()).Returns(likeResponse);
            var commentProcess = _process.PostScrapeProcess(_scrapeResultNew);
            //assert
            _instafunct.Received(1).Like(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<CancellationToken>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<QueryInfo>());
        }

        [TestMethod]
        public void Check_LikeProcess_Will_Return_False_When_Action_Will_Be_Block()
        {
            //arrange
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultPost = new InstagramPost
                {

                    Pk = "1947724516487055651",
                    User = new InstagramUser
                    {
                        Pk = "7150086983",
                        Username = "jacksmith729",
                    }

                },
                QueryInfo = new QueryInfo()
                {

                }
            };
            _instafunct = GdLogInProcess.InstagramFunctFactory.InstaFunctions;
            ((GdJobProcess)_process).instaFunct = _instafunct;
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
 
            string resp = "{ \"message\": \"feedback_required\",\"spam\": true,\"feedback_title\": \"Action Blocked\",\"feedback_message\": \"This action was blocked. Please try again later. We restrict certain content and actions to protect our community. Tell us if you think we made a mistake.\",\"feedback_action\": \"report_problem\",\"status\": \"fail\" }";
            LikeResponse likeResponse = new LikeResponse(new ResponseParameter
            {
                Response = resp
            });
            //act
            _instafunct.Like(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<CancellationToken>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<QueryInfo>()).Returns(likeResponse);
            var LikeProcess = _process.PostScrapeProcess(_scrapeResultNew);
            //assert
            _instafunct.Received(1).Like(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<CancellationToken>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<QueryInfo>());
            LikeProcess.IsProcessSuceessfull.Should().BeFalse();
        }
    }
}


