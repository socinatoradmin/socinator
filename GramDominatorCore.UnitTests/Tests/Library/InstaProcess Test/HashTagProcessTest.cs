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
    public  class HashTagProcessTest : BaseGdProcessTest
    {
        HashtagsScraperModel hashTagModel;
        HashtagScrapeProcess process;
        ScrapeResultNewTag scrapResultTag;
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            var _activityType = ActivityType.HashtagsScraper;
            GdJobProcess.ActivityType.Returns(_activityType);
            scrapResultTag= Substitute.For<ScrapeResultNewTag>();
            var module = new ModuleConfiguration();
            JobActivityConfigurationManager[AccountId, _activityType].Returns(module);
            hashTagModel = new HashtagsScraperModel();
            ProcessScopeModel.GetActivitySettingsAs<HashtagsScraperModel>().Returns(hashTagModel);
            var jsonData = JsonConvert.SerializeObject(hashTagModel);
            var model = new TemplateModel() { ActivitySettings = jsonData };
            var templatesFileManager = CommonServiceLocator.ServiceLocator.Current.GetInstance<ITemplatesFileManager>();
            templatesFileManager.GetTemplateById(Arg.Any<string>()).Returns(model);
            ProcessScopeModel.ActivityType.Returns(ActivityType.HashtagsScraper);
            process = new HashtagScrapeProcess(ProcessScopeModel, AccountServiceScoped, GdQueryScraperFactory, GdHttpHelper, GdBrowserManager, DelayService);
        }

        [TestMethod]
        public void Check_HashTagProcess()
        {
            //arrange
            scrapResultTag = new ScrapeResultNewTag
            {
                QueryInfo = new QueryInfo()
                {
                    QueryValue = "car"
                },
                TagDetails = new TagDetails("123",3455,"check")
              
            };
            ((GdJobProcess)process).instaFunct = InstaFunct;
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            //act
            var commentProcess = process.PostScrapeProcess(scrapResultTag);
            //assert
            commentProcess.IsProcessSuceessfull.Should().BeTrue();
        }

        [TestMethod]
        public void Check_HashTagProcess_Will_Return_False()
        {
            //arrange
            scrapResultTag = new ScrapeResultNewTag
            {
                QueryInfo = new QueryInfo()
                {
                    QueryValue = "car"
                },
                TagDetails = null

            };
            ((GdJobProcess)process).instaFunct = InstaFunct;
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            //act
            var commentProcess = process.PostScrapeProcess(scrapResultTag);
            //assert
            commentProcess.IsProcessSuceessfull.Should().BeFalse();
        }
    }
}
