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
    public class PostScraperProcessTest : BaseGdProcessTest
    {
        DownloadPhotosModel _downloadPhotosModel;
        DownloadPhotoProcess _process ;
        private ScrapeResultNew _scrapeResultNew;
        IInstaFunction _instafunct;
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            var _activityType = ActivityType.PostScraper;
            GdJobProcess.ActivityType.Returns(_activityType);
            _instafunct = Substitute.For<IInstaFunction>();
              var module = new ModuleConfiguration();
            JobActivityConfigurationManager[AccountId, _activityType].Returns(module);
            _downloadPhotosModel = new DownloadPhotosModel();
            ProcessScopeModel.GetActivitySettingsAs<DownloadPhotosModel>().Returns(_downloadPhotosModel);
            var jsonData = JsonConvert.SerializeObject(_downloadPhotosModel);
            var model = new TemplateModel() { ActivitySettings = jsonData };
            var templatesFileManager = CommonServiceLocator.ServiceLocator.Current.GetInstance<ITemplatesFileManager>();
            templatesFileManager.GetTemplateById(Arg.Any<string>()).Returns(model);
            ProcessScopeModel.ActivityType.Returns(ActivityType.PostScraper);
            _downloadPhotosModel.DownloadedFolderPath = "C:\\Users\\GLB-123\\Documents";
            _process = new DownloadPhotoProcess(ProcessScopeModel, AccountServiceScoped, GdQueryScraperFactory,
                GdHttpHelper, GdBrowserManager, DelayService);
        }

        [TestMethod]
        public void Check_PostScraperProcess()
        {
            //arrange
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultPost = new InstagramPost
                {
                  Code= "BwADsb1HLDV",
                  Caption="abc",
                  CommentCount=123,
                  LikeCount=456,
                  TakenAt= 1554740127,
                  MediaType=MediaType.Image,
                  Pk= "2017628880740593877",
                  Id= "2017628880740593877_18428658",
                  User=new InstagramUser
                  {
                      Username="abc"
                  }
                },
                QueryInfo = new QueryInfo()
                {
                    QueryType = "Custom Photos",
                    QueryValue = "BwADsb1HLDV",
                },
            };
            ((GdJobProcess)_process).instaFunct = _instafunct;
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            //act
            var commentProcess = _process.PostScrapeProcess(_scrapeResultNew);
            //assert
            commentProcess.IsProcessSuceessfull.Should().BeTrue();
        }

        [TestMethod]
        public void Check_PostScraperProcess_With_RequiredData()
        {
            //arrange
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultPost = new InstagramPost
                {
                    Code = "BwADsb1HLDV",
                    Caption = "abc",
                    CommentCount = 123,
                    LikeCount = 456,
                    TakenAt = 1554740127,
                    MediaType = MediaType.Image,
                    Pk = "2017628880740593877",
                    Id = "2017628880740593877_18428658",
                    User = new InstagramUser
                    {
                        Username = "abc"
                    }
                },
                QueryInfo = new QueryInfo()
                {
                    QueryType = "Custom Photos",
                    QueryValue = "BwADsb1HLDV",
                },
            };
            ((GdJobProcess)_process).instaFunct = _instafunct;
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            //act
            var commentProcess = _process.PostScrapeProcess(_scrapeResultNew);
            //assert
            commentProcess.IsProcessSuceessfull.Should().BeTrue();
        }
    }
}
