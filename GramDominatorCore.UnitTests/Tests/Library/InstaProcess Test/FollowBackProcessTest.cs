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

namespace GramDominatorCore.UnitTests.Tests.Library.InstaProcess_Test
{
    [TestClass]
    public class FollowBackProcessTest : BaseGdProcessTest
    {
        FollowBackModel _followBackModel;
        FollowBackProcess _process;
        private ScrapeResultNew _scrapeResultNew;
        IInstaFunction _function;
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            ActivityType _activityType = ActivityType.FollowBack;
            GdJobProcess.ActivityType.Returns(_activityType);
            _function = Substitute.For<IInstaFunction>();
            var module = new ModuleConfiguration();
            JobActivityConfigurationManager[AccountId, _activityType].Returns(module);
            _followBackModel = new FollowBackModel();
            _followBackModel.IsFollowBack = true;
            ProcessScopeModel.GetActivitySettingsAs<FollowBackModel>().Returns(_followBackModel);
            var jsonData = JsonConvert.SerializeObject(_followBackModel);
            var model = new TemplateModel() { ActivitySettings = jsonData };
            var templatesFileManager = CommonServiceLocator.ServiceLocator.Current.GetInstance<ITemplatesFileManager>();
            templatesFileManager.GetTemplateById(Arg.Any<string>()).Returns(model);
            ProcessScopeModel.ActivityType.Returns(ActivityType.FollowBack);
            _process = new FollowBackProcess(ProcessScopeModel, AccountServiceScoped, GdQueryScraperFactory, GdHttpHelper, GdBrowserManager, DelayService);
        }

        [TestMethod]
        public void Check_FollowBackProcess()
        {
            //arrange
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultUser = new InstagramUser
                {
                    UserId = "9925711872"
                },
            };
            ((GdJobProcess)_process).instaFunct = _function;
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);

            var followPageResponse = TestUtils.ReadFileFromResources(
           "GramDominatorCore.UnitTests.TestData.FollowResponse.json", Assembly.GetExecutingAssembly());
            var followResponse = new FriendshipsResponse(new ResponseParameter
            {
                Response = followPageResponse
            });
            //act
            _function.Follow(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<CancellationToken>(), Arg.Any<string>()).Returns(followResponse);
            var commentProcess = _process.PostScrapeProcess(_scrapeResultNew);
            //assert
            _function.Received(1).Follow(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<CancellationToken>(), Arg.Any<string>());

        }
    }
}
