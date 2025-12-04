using Dominator.Tests.Utils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.GdQuery;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using GramDominatorCore.Response;
using GramDominatorCore.UnitTests.Test_Data.Library;
using GramDominatorCore.UnitTests.Tests.Library.ProcessorTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NSubstitute;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading;
using FluentAssertions;

namespace GramDominatorCore.UnitTests.Tests.Library.InstaProcess_Test
{
    [TestClass]
    public class UnlikeProcessTest : BaseGdProcessTest
    {
        MediaUnlikerModel _mediaUnlikerModel;
        MediaUnlikeProcess _process;
        private ScrapeResultNew _scrapeResultNew;
        IInstaFunction _instafunct;
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            var _activityType = ActivityType.Unlike;
            GdJobProcess.ActivityType.Returns(_activityType);
            _instafunct = Substitute.For<IInstaFunction>();
            var module = new ModuleConfiguration();
            JobActivityConfigurationManager[AccountId, _activityType].Returns(module);
            _mediaUnlikerModel = new MediaUnlikerModel();
            ProcessScopeModel.GetActivitySettingsAs<MediaUnlikerModel>().Returns(_mediaUnlikerModel);
            var jsonData = JsonConvert.SerializeObject(_mediaUnlikerModel);
            var model = new TemplateModel() { ActivitySettings = jsonData };
            var templatesFileManager = CommonServiceLocator.ServiceLocator.Current.GetInstance<ITemplatesFileManager>();
            templatesFileManager.GetTemplateById(Arg.Any<string>()).Returns(model);
            ProcessScopeModel.ActivityType.Returns(ActivityType.Unlike);
            _process = new MediaUnlikeProcess(ProcessScopeModel, AccountServiceScoped, GdQueryScraperFactory, GdHttpHelper, GdBrowserManager, DelayService);
        }

        [TestMethod]
        public void Check_UnlikeMediaProcess()
        {
            //arrange
            _scrapeResultNew = new ScrapeResultNew
            {                
                ResultPost=new InstagramPost
                {
                    Pk= "1857051003963384707",
                    TakenAt= 1535597752,
                    MediaType=MediaType.Image,
                    Code="abc",
                    User=new InstagramUser
                    {
                        Username="abcd"
                    }
                }          
            };
            ((GdJobProcess)_process).instaFunct = _instafunct;
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);

            var pageresponse = TestUtils.ReadFileFromResources(
        "GramDominatorCore.UnitTests.TestData.UnlikeMediaResponse.json", Assembly.GetExecutingAssembly());
            CommonIgResponseHandler UnlikeResponse = new CommonIgResponseHandler(new ResponseParameter
            {
                Response = pageresponse
            });
            //act
            _instafunct.UnlikeMedia(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<CancellationToken>(), Arg.Any<string>()).Returns(UnlikeResponse);
            var Unlike = _process.PostScrapeProcess(_scrapeResultNew);
            //assert
            _instafunct.Received(1).UnlikeMedia(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<CancellationToken>(), Arg.Any<string>());
        }

        [TestMethod]
        public void Check_UnlikeMediaProcess_Will_Return_False()
        {
            //arrange
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultPost = new InstagramPost
                {
                    Pk = "1857051003963384707",
                    TakenAt = 1535597752,
                    MediaType = MediaType.Image,
                    Code = "abc",
                    User = new InstagramUser
                    {
                        Username = "abcd"
                    }
                }
            };
            ((GdJobProcess)_process).instaFunct = _instafunct;
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);

             CommonIgResponseHandler unlikeResponse = new CommonIgResponseHandler(new ResponseParameter
            {
                Response = "{\"message\": \"The user has not liked this media\", \"status\": \"fail\"}",
            });
            //act
            _instafunct.UnlikeMedia(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<CancellationToken>(), Arg.Any<string>()).Returns(unlikeResponse);
            var unlike = _process.PostScrapeProcess(_scrapeResultNew);
            //assert
            _instafunct.Received(1).UnlikeMedia(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<CancellationToken>(), Arg.Any<string>());
            unlike.IsProcessSuceessfull.Should().BeFalse();
        }
    }
}
