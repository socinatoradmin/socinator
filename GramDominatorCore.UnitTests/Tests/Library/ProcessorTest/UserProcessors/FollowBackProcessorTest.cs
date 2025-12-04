using Dominator.Tests.Utils;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using FluentAssertions;
using GramDominatorCore.GDLibrary;
using GramDominatorCore.GDLibrary.Processor.User;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using GramDominatorCore.Response;
using GramDominatorCore.UnitTests.Test_Data.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GramDominatorCore.UnitTests.Tests.Library.ProcessorTest.UserProcessors
{
    [TestClass]
    public class FollowBackProcessorTest:BaseGdProcessTest
    {
        private ActivityType _activityType;
        public List<Friendships> allFollowers = new List<Friendships>();
        IGdJobProcess _gdJobProcess;
        private InstaFunctTest objIinstaFunctTest = new InstaFunctTest();
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _gdJobProcess = Substitute.For<IGdJobProcess>();
            _activityType = ActivityType.FollowBack;
            var cancleJob = new CancellationTokenSource();
            _gdJobProcess.ActivityType.Returns(_activityType);
            objIinstaFunctTest = Substitute.For<InstaFunctTest>();
            var module = new ModuleConfiguration();
            JobActivityConfigurationManager[AccountId, _activityType].Returns(module);

            FollowBackModel followBackModel = new FollowBackModel();
            followBackModel.IsFollowBack = true;
            ProcessScopeModel.GetActivitySettingsAs<FollowBackModel>().Returns(followBackModel);
            var jsonData = JsonConvert.SerializeObject(followBackModel);
            var model = new TemplateModel() { ActivitySettings = jsonData };
            var templatesFileManager = CommonServiceLocator.ServiceLocator.Current.GetInstance<DominatorHouseCore.FileManagers.ITemplatesFileManager>();
            templatesFileManager.GetTemplateById(Arg.Any<string>()).Returns(model);
            _gdJobProcess.DominatorAccountModel.Returns(GdDominatorAccountModel);
            _gdJobProcess.JobCancellationTokenSource.Returns(cancleJob);
            _gdJobProcess.ModuleSetting.Returns(followBackModel);
            _gdJobProcess.instaFunct = _gdJobProcess.loginProcess.InstagramFunctFactory.InstaFunctions;
        }

        [TestMethod]
        public void should_search_For_FollowerBack()
        {
            // arrange    
            var queryInfo = new QueryInfo
            {
                QueryType = "",
                QueryValue = ""
            };
            var jobResult = new JobProcessResult();         
            _gdJobProcess.FinalProcess(Arg.Any<ScrapeResultNew>()).Returns(jobResult);
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.UserFriendshipResponse.json", Assembly.GetExecutingAssembly());

            UserFriendshipResponse userFriendShip = new UserFriendshipResponse(new ResponseParameter() { Response=pageresponse});

            _gdJobProcess.instaFunct.UserFriendship(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>()).Returns(userFriendShip);

            var FollowerList = TestUtils.ReadFileFromResources(
         "GramDominatorCore.UnitTests.TestData.OwnFollowerList.json", Assembly.GetExecutingAssembly());
            List<Friendships> list = JsonConvert.DeserializeObject<List<Friendships>>(FollowerList);
            AccountServiceScoped.GetFollowers().Returns(list);
           //act
            FollowBackProcessor OwnFollower = new FollowBackProcessor(_gdJobProcess, AccountServiceScoped,
               CampaignService, ProcessScopeModel, DelayService);
            OwnFollower.Start(queryInfo);
            //assert
            _gdJobProcess.Received(0).FinalProcess(Arg.Any<ScrapeResultNew>());
            _gdJobProcess.Received(1).instaFunct.SearchUsername(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(),Arg.Any<CancellationToken>());

        }
    }
}
