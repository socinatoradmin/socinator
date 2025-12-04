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
    public class AcceptRequestFollowerProcessorTest:BaseGdProcessTest
    {
        private ActivityType _activityType;
        public List<Friendships> allFollowers = new List<Friendships>();
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
        public void should_search_For_Accept_Request_Failed_If_Account_Will_Be_Not_Private()
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

            GdJobProcess.ModuleSetting.IsFollowBack = false;
            var UserInfoResponse = TestUtils.ReadFileFromResources(
            "GramDominatorCore.UnitTests.TestData.SearchUserInfoByIdResponse.json", Assembly.GetExecutingAssembly());           
            UsernameInfoIgResponseHandler userinfo = new UsernameInfoIgResponseHandler(new ResponseParameter()
            {  Response=UserInfoResponse});
            //act
            _gdJobProcess.instaFunct.SearchUserInfoById(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(userinfo);
            FollowBackProcessor OwnFollower = new FollowBackProcessor(_gdJobProcess, AccountServiceScoped,
                CampaignService, ProcessScopeModel, DelayService);

            OwnFollower.Start(queryInfo);
            //assert
            _gdJobProcess.Received(1).instaFunct.SearchUserInfoById(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
           
        }

        [TestMethod]
        public void should_search_For_AcceptRequest()
        {
            // arrange    
            var queryInfo = new QueryInfo
            {
                QueryType = "",
                QueryValue = "2071648333"
            };
            var jobResult = new JobProcessResult();
            _gdJobProcess.FinalProcess(Arg.Any<ScrapeResultNew>()).Returns(jobResult);           
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            GdJobProcess.ModuleSetting.IsFollowBack = false;

            var UserInfoResponse = TestUtils.ReadFileFromResources(
            "GramDominatorCore.UnitTests.TestData.SearchPrivateUserInfoByIdResponse.json", Assembly.GetExecutingAssembly());
            var PendingReaquestResponse = TestUtils.ReadFileFromResources(
            "GramDominatorCore.UnitTests.TestData.FriendshipPendingRequest.json", Assembly.GetExecutingAssembly());
           
            UsernameInfoIgResponseHandler userinfo = new UsernameInfoIgResponseHandler(new ResponseParameter()
            { Response = UserInfoResponse });
            FriendShipPendingResponseHandler FriendShipPending = new FriendShipPendingResponseHandler(new ResponseParameter()
            {
                Response = PendingReaquestResponse
            });   
            //act       
            _gdJobProcess.instaFunct.SearchUserInfoById(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(userinfo);
            _gdJobProcess.instaFunct.PendingRequest(Arg.Any<DominatorAccountModel>()).Returns(FriendShipPending);            
            var FollowerList = TestUtils.ReadFileFromResources(
            "GramDominatorCore.UnitTests.TestData.OwnFollowerList.json", Assembly.GetExecutingAssembly());
            List<Friendships> list = JsonConvert.DeserializeObject<List<Friendships>>(FollowerList);
            AccountServiceScoped.GetFollowers().Returns(list);          
            FollowBackProcessor OwnFollower = new FollowBackProcessor(_gdJobProcess, AccountServiceScoped, CampaignService, ProcessScopeModel, DelayService);
            OwnFollower.Start(queryInfo);
            //assert
             _gdJobProcess.Received(2).FinalProcess(Arg.Any<ScrapeResultNew>());
            _gdJobProcess.Received(2).instaFunct.SearchUserInfoById(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
            _gdJobProcess.Received(2).instaFunct.PendingRequest(Arg.Any<DominatorAccountModel>());
           
        }
    }
}
