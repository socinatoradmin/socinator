using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FluentAssertions;
using GramDominatorCore.GDLibrary;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using GramDominatorCore.UnitTests.Tests.Library.ProcessorTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NSubstitute;

namespace GramDominatorCore.UnitTests.Tests.Library.InstaProcess_Test
{
    [TestClass]
    public class CommentScraperProcessTest : BaseGdProcessTest
    {
        private ActivityType _activityType;
        CommentScraperModel commentModel;
        CommentScraperProcess process;
        TemplateModel model { get; set; }
        private ScrapeResultNew _scrapeResultNew;
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            _activityType = ActivityType.CommentScraper;
            GdJobProcess.ActivityType.Returns(_activityType);
            var module = new ModuleConfiguration();
            JobActivityConfigurationManager[AccountId, _activityType].Returns(module);       
            commentModel = new CommentScraperModel();
            ProcessScopeModel.GetActivitySettingsAs<CommentScraperModel>().Returns(commentModel);
            var jsonData = JsonConvert.SerializeObject(commentModel);
            model = new TemplateModel() { ActivitySettings = jsonData };
            var templatesFileManager = CommonServiceLocator.ServiceLocator.Current.GetInstance<ITemplatesFileManager>();
            templatesFileManager.GetTemplateById(Arg.Any<string>()).Returns(model);
            ProcessScopeModel.ActivityType.Returns(ActivityType.CommentScraper);
            process = new CommentScraperProcess(ProcessScopeModel, AccountServiceScoped, GdQueryScraperFactory, GdHttpHelper, GdBrowserManager, DelayService);
        }

        [TestMethod]
        public void Check_CommentScraperProcess()
        {
            //arrange
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultPost = new InstagramPost
                { Code = "BvG6NmxHFAt",
                  MediaType= MediaType.Image,
                  User=new InstagramUser()
                  {
                      Username="jacksmith729",
                  }
                },
                QueryInfo = new QueryInfo()
                {
                    QueryTypeDisplayName = "Custom Photos",
                    QueryType = "Custom Photos",
                    QueryValue = "BvG6NmxHFAt",
                },
                ResultPostComment = new ResultCommentItemUser()
                {
                    CommentId = "12345678987654321",
                    ContentType = "Comment",
                    Text = "My Crush",
                    UserId = "",
                    ItemUser=new InstagramUser()
                    {
                        Pk="",
                        Username="jacksmith"
                    }
                }
            };
            ((GdJobProcess)process).instaFunct = InstaFunct;
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            //act
            var commentProcess = process.PostScrapeProcess(_scrapeResultNew);
            //assert
            commentProcess.IsProcessSuceessfull.Should().BeTrue();
        }

        [TestMethod]
        public void Check_CommentScraperProcess_Will_Return_False_If_ResultPostComment_will_be_Null()
        {
            //arrange
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultPost = new InstagramPost
                {
                    Code = "BvG6NmxHFAt",
                    MediaType = MediaType.Image,
                    User = new InstagramUser()
                    {
                        Username = "jacksmith729",
                    }
                },
                QueryInfo = new QueryInfo()
                {
                    QueryTypeDisplayName = "Custom Photos",
                    QueryType = "Custom Photos",
                    QueryValue = "BvG6NmxHFAt",
                },
                ResultPostComment = new ResultCommentItemUser()
            };
            ((GdJobProcess)process).instaFunct = InstaFunct;
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            //act
            var commentProcess = process.PostScrapeProcess(_scrapeResultNew);
            //assert
            commentProcess.IsProcessSuceessfull.Should().BeFalse();
        }
    }
}
