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
    public  class FollowProcessTest : BaseGdProcessTest
    {
        private ActivityType _activityType;
        FollowerModel followModel;
        FollowProcess process;
        private ScrapeResultNew _scrapeResultNew;
        IInstaFunction instafunct;
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            _activityType = ActivityType.Follow;
            GdJobProcess.ActivityType.Returns(_activityType);
            instafunct = Substitute.For<IInstaFunction>();
            var module = new ModuleConfiguration();
            JobActivityConfigurationManager[AccountId, _activityType].Returns(module);
            followModel = new FollowerModel();
            ProcessScopeModel.GetActivitySettingsAs<FollowerModel>().Returns(followModel);
            var jsonData = JsonConvert.SerializeObject(followModel);
            var model = new TemplateModel() { ActivitySettings = jsonData };
            var templatesFileManager = CommonServiceLocator.ServiceLocator.Current.GetInstance<ITemplatesFileManager>();
            templatesFileManager.GetTemplateById(Arg.Any<string>()).Returns(model);
            ProcessScopeModel.ActivityType.Returns(ActivityType.Follow);
            process = new FollowProcess(ProcessScopeModel, AccountServiceScoped, GdQueryScraperFactory, GdHttpHelper, GdLogInProcess,GdBrowserManager, DelayService);
        }

        [TestMethod]
        [Timeout(10000)]
        public void Check_FollowProcess()
        {
            //arrange
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultUser = new InstagramUser
                {
                    Pk = "9925711872"
                },
                QueryInfo=new QueryInfo
                {
                    QueryType="abc"
                }
            };
            
            instafunct = GdLogInProcess.InstagramFunctFactory.InstaFunctions;
            ((GdJobProcess)process).instaFunct = instafunct;
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
           "GramDominatorCore.UnitTests.TestData.FollowResponse.json", Assembly.GetExecutingAssembly());
            var FollowResponse = new FriendshipsResponse(new ResponseParameter
            {
                Response = pageresponse
            });
            //act
            instafunct.Follow(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<CancellationToken>(), Arg.Any<string>(), Arg.Any<string>()).Returns(FollowResponse);
            var followProcess = process.PostScrapeProcess(_scrapeResultNew);
            //assert
            instafunct.Received(1).Follow(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<CancellationToken>(), Arg.Any<string>(), Arg.Any<string>());
        }
    }
}
