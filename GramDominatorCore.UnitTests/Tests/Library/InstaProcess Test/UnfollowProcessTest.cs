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
    public class UnfollowProcessTest : BaseGdProcessTest
    {
        UnfollowerModel _unfollowModel;
        UnFollowProcess _process;
        private ScrapeResultNew _scrapeResultNew;
        
        IInstaFunction _instafunct;
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

           var _activityType = ActivityType.Unfollow;
            GdJobProcess.ActivityType.Returns(_activityType);
            _instafunct = Substitute.For<IInstaFunction>();
            var module = new ModuleConfiguration();
            JobActivityConfigurationManager[AccountId, _activityType].Returns(module);
            _unfollowModel = new UnfollowerModel();
            _unfollowModel.IsUnfollowFollowings = true;
            ProcessScopeModel.GetActivitySettingsAs<UnfollowerModel>().Returns(_unfollowModel);
            var jsonData = JsonConvert.SerializeObject(_unfollowModel);
           var model = new TemplateModel() { ActivitySettings = jsonData };
            var templatesFileManager = CommonServiceLocator.ServiceLocator.Current.GetInstance<ITemplatesFileManager>();
            templatesFileManager.GetTemplateById(Arg.Any<string>()).Returns(model);
            ProcessScopeModel.ActivityType.Returns(ActivityType.Unfollow);
            _process = new UnFollowProcess(ProcessScopeModel, AccountServiceScoped, GdQueryScraperFactory, GdHttpHelper, null, DelayService);
        }

        [TestMethod]
        public void Check_unFollowProcess()
        {
            //arrange
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultUser = new InstagramUser
                {
                    Pk = "24931858"
                },
            };
            ((GdJobProcess)_process).instaFunct = _instafunct;
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);

            var pageresponse = TestUtils.ReadFileFromResources(
      "GramDominatorCore.UnitTests.TestData.UnfollowResponse.json", Assembly.GetExecutingAssembly());
            FriendshipsResponse UnfollowResponse = new FriendshipsResponse(new ResponseParameter
            {
                Response = pageresponse
            });
            //act
            _instafunct.Unfollow(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<CancellationToken>(), Arg.Any<string>()).Returns(UnfollowResponse);
            var commentProcess = _process.PostScrapeProcess(_scrapeResultNew);
            //assert
            _instafunct.Received(1).Unfollow(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<CancellationToken>(), Arg.Any<string>());
        }

        [TestMethod]
        public void Check_unFollowProcess_Will_Return_False()  // needs response for proper checking
        {
            //arrange
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultUser = new InstagramUser
                {
                    Pk = "24931858",
                    Username = "abc"
                },
            };
            _process.instaFunct = _instafunct;
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);

             FriendshipsResponse unfollowResponse = new FriendshipsResponse(new ResponseParameter
            {
                Response = "{\"message\":\"abc\", \"status\": \"fail\",\"error_type\":\"User not found\"}",
            });
            //act
            _instafunct.Unfollow(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<CancellationToken>(), Arg.Any<string>()).Returns(unfollowResponse);
            var unFollowProcess = _process.PostScrapeProcess(_scrapeResultNew);
            //assert
            _instafunct.Received(1).Unfollow(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<CancellationToken>(), Arg.Any<string>());
            unFollowProcess.IsProcessSuceessfull.Should().BeFalse();
        }
    }
}
