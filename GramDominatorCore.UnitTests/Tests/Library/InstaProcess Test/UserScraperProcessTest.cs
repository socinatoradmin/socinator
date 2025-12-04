using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.GdQuery;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FluentAssertions;
using GramDominatorCore.GDLibrary;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using GramDominatorCore.UnitTests.Test_Data.Library;
using GramDominatorCore.UnitTests.Tests.Library.ProcessorTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NSubstitute;

namespace GramDominatorCore.UnitTests.Tests.Library.InstaProcess_Test
{
    [TestClass]
    public  class UserScraperProcessTest : BaseGdProcessTest
    {
        UserScraperModel _userScraperModel;
        UserScrapeProcess _process;
        private ScrapeResultNew _scrapeResultNew;
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            var _activityType = ActivityType.CommentScraper;
            GdJobProcess.ActivityType.Returns(_activityType);
           
            var module = new ModuleConfiguration();
            JobActivityConfigurationManager[AccountId, _activityType].Returns(module);
            _userScraperModel = new UserScraperModel();
            ProcessScopeModel.GetActivitySettingsAs<UserScraperModel>().Returns(_userScraperModel);
            var jsonData = JsonConvert.SerializeObject(_userScraperModel);
            var model = new TemplateModel() { ActivitySettings = jsonData };
            var templatesFileManager = CommonServiceLocator.ServiceLocator.Current.GetInstance<ITemplatesFileManager>();
            templatesFileManager.GetTemplateById(Arg.Any<string>()).Returns(model);
            ProcessScopeModel.ActivityType.Returns(ActivityType.UserScraper);
            _process = new UserScrapeProcess(ProcessScopeModel, AccountServiceScoped, GdQueryScraperFactory, GdHttpHelper, GdBrowserManager, DelayService);
        }

        [TestMethod]
        public void Check_UserScraperProcess()
        {
            //arrange
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultUser = new InstagramUser
                {
                    UserId = "1234567",
                    Username = "satish",                
                },
                QueryInfo = new QueryInfo()
                {
                    QueryTypeDisplayName = "Custom Photos",
                    QueryType = "Custom User",
                    QueryValue = "satish",
                },
            };
            ((GdJobProcess)_process).instaFunct = InstaFunct;
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            //act
            var commentProcess = _process.PostScrapeProcess(_scrapeResultNew);
            //assert
            commentProcess.IsProcessSuceessfull.Should().BeTrue();
        }

        [TestMethod]
        public void Check_UserScraperProcess_Will_Return_False()
        {
            //arrange
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultUser =null,
                QueryInfo = new QueryInfo()
                {
                    QueryTypeDisplayName = "Custom Photos",
                    QueryType = "Custom User",
                    QueryValue = "satish",
                },
            };
            ((GdJobProcess)_process).instaFunct = InstaFunct;
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            //act
            var commentProcess = _process.PostScrapeProcess(_scrapeResultNew);
            //assert
            commentProcess.IsProcessSuceessfull.Should().BeFalse();
        }
    }
}
