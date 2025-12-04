using Dominator.Tests.Utils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.GdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
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
    public class SomeOneFollowersProcessorTest : BaseGdProcessTest
    {
        private ActivityType _activityType;
        IGdJobProcess _gdJobProcess;
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _gdJobProcess = Substitute.For<IGdJobProcess>();
            _activityType = ActivityType.Follow;
            var cancleJob = new CancellationTokenSource();
            _gdJobProcess.ActivityType.Returns(_activityType);
            var module = new ModuleConfiguration();
            JobActivityConfigurationManager[AccountId, _activityType].Returns(module);
            FollowerModel FollowerModel = new FollowerModel();
            ProcessScopeModel.GetActivitySettingsAs<FollowerModel>().Returns(FollowerModel);
            var jsonData = JsonConvert.SerializeObject(GdModuleSetting);
            var model = new TemplateModel() { ActivitySettings = jsonData };
            var templatesFileManager = CommonServiceLocator.ServiceLocator.Current.GetInstance<DominatorHouseCore.FileManagers.ITemplatesFileManager>();
            templatesFileManager.GetTemplateById(Arg.Any<string>()).Returns(model);
            _gdJobProcess.DominatorAccountModel.Returns(GdDominatorAccountModel);
            _gdJobProcess.JobCancellationTokenSource.Returns(cancleJob);
            _gdJobProcess.ModuleSetting.Returns(GdModuleSetting);
            _gdJobProcess.instaFunct = _gdJobProcess.loginProcess.InstagramFunctFactory.InstaFunctions;
        }

        [TestMethod]
        [Timeout(15000)]
        public void Should_Search_SomeOneFollowers()
        {
            //arrange
            var queryInfo = new QueryInfo
            {
                QueryType = "Someone's Followers",
                QueryValue = "sachin"
            };

            var jobResult = new JobProcessResult();
            _gdJobProcess.FinalProcess(Arg.Any<ScrapeResultNew>()).Returns(jobResult);
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            var userinfo = TestUtils.ReadFileFromResources(
             "GramDominatorCore.UnitTests.TestData.SearchUserNameResponse.json", Assembly.GetExecutingAssembly());
            var GetUserFollower = TestUtils.ReadFileFromResources(
             "GramDominatorCore.UnitTests.TestData.GetUserFollowersResponse.json", Assembly.GetExecutingAssembly());

            UsernameInfoIgResponseHandler UserNameInfoResponse = new UsernameInfoIgResponseHandler(new ResponseParameter
            {
                Response = userinfo
            });
            FollowerAndFollowingIgResponseHandler GetUserFollowerResponse = new FollowerAndFollowingIgResponseHandler(new ResponseParameter
            {
                Response = GetUserFollower
            });
            //act
            _gdJobProcess.instaFunct.SearchUsername(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(UserNameInfoResponse);
            _gdJobProcess.instaFunct.GetUserFollowers(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>(), Arg.Any<string>(), Arg.Any<string>()).Returns(GetUserFollowerResponse);
            SomeonesFollowersProcessor someOneFollowerProcessor = new SomeonesFollowersProcessor(_gdJobProcess,
                AccountServiceScoped, CampaignService, ProcessScopeModel, DelayService);
            someOneFollowerProcessor.Start(queryInfo);
            //assert
            _gdJobProcess.Received(2).FinalProcess(Arg.Any<ScrapeResultNew>());
            _gdJobProcess.Received(2).instaFunct.SearchUsername(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(UserNameInfoResponse);
            _gdJobProcess.Received(2).instaFunct.GetUserFollowers(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>(), Arg.Any<string>(), Arg.Any<string>()).Returns(GetUserFollowerResponse);

        }
    }
}
