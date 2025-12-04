using Dominator.Tests.Utils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.GdQuery;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using FluentAssertions;
using GramDominatorCore.GDLibrary;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using GramDominatorCore.Response;
using GramDominatorCore.UnitTests.Test_Data.Library;
using GramDominatorCore.UnitTests.Tests.Library.ProcessorTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NSubstitute;
using System.Reflection;
using System.Threading;

namespace GramDominatorCore.UnitTests.Tests.Library.InstaProcess_Test
{
    [TestClass]
    public class BlockFollowerProcessTest : BaseGdProcessTest
    {
        BlockFollowerProcess process;
        IInstaFunction instafunct;
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            ActivityType _activityType = ActivityType.BlockFollower;
            GdJobProcess.ActivityType.Returns(_activityType);
            instafunct = Substitute.For<IInstaFunction>();
            var module = new ModuleConfiguration();
            JobActivityConfigurationManager[AccountId, _activityType].Returns(module);
            BlockFollowerModel blockFollowerModel = new BlockFollowerModel();
            blockFollowerModel.ManageBlackWhiteListModel.IsAddToBlackListOnceUnfollowed = false;

            ProcessScopeModel.GetActivitySettingsAs<BlockFollowerModel>().Returns(blockFollowerModel);
            var jsonData = JsonConvert.SerializeObject(blockFollowerModel);
            var model = new TemplateModel() { ActivitySettings = jsonData };
            var templatesFileManager = CommonServiceLocator.ServiceLocator.Current.GetInstance<ITemplatesFileManager>();
            templatesFileManager.GetTemplateById(Arg.Any<string>()).Returns(model);
            ProcessScopeModel.ActivityType.Returns(ActivityType.BlockFollower);
            process = new BlockFollowerProcess(ProcessScopeModel, AccountServiceScoped, GdQueryScraperFactory, GdHttpHelper, GdBrowserManager, DelayService);
        }

        [TestMethod]
        public void Check_BlockFollower()
        {
            //arrange
            ScrapeResultNew _scrapeResultNew = new ScrapeResultNew
            {
                ResultUser = new InstagramUser { UserId = "4651924329" },
                QueryInfo = new QueryInfo()
            };
            ((GdJobProcess)process).instaFunct = instafunct;
            var requestParameters = new IgRequestParameters();
            instafunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
             "GramDominatorCore.UnitTests.TestData.BlockFollower.json", Assembly.GetExecutingAssembly());
            FriendshipsResponse FriendshipResponse = new FriendshipsResponse(new ResponseParameter
            {
                Response = pageresponse
            });
            //act
            instafunct.Block(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<CancellationToken>(), Arg.Any<string>()).Returns(FriendshipResponse);
             var jpr = process.PostScrapeProcess(_scrapeResultNew);
            //assert
            instafunct.Received(1).Block(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<CancellationToken>(), Arg.Any<string>());
        }

        [TestMethod]
        public void Check_BlockFollower_Will_Return_False()
        {
            //arrange
            ScrapeResultNew  _scrapeResultNew = new ScrapeResultNew
            {
                ResultUser = new InstagramUser { UserId = "46519243" },
                QueryInfo = new QueryInfo()
            };
            ((GdJobProcess)process).instaFunct = InstaFunct;
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
             "GramDominatorCore.UnitTests.TestData.BlockFollower.json", Assembly.GetExecutingAssembly());
            FriendshipsResponse FriendShipResponse = new FriendshipsResponse(new ResponseParameter
            {
                Response = pageresponse
            });
            //act
            instafunct.Block(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<CancellationToken>(), Arg.Any<string>()).Returns(FriendShipResponse);
            var jpr = process.PostScrapeProcess(_scrapeResultNew);    
            //assert      
            jpr.IsProcessSuceessfull.Should().BeFalse();
        }
    }
}
