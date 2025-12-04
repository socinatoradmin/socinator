using Dominator.Tests.Utils;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.GdQuery;
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
using System.Reflection;
using System.Text;
using System.Threading;
namespace GramDominatorCore.UnitTests.Tests.Library.ProcessorTest.UserProcessors
{
    [TestClass]
    public class UnfollowProcessorTest: BaseGdProcessTest
    {
        private ActivityType _activityType;
        InstaFunctTest objIinstaFunctTest = new InstaFunctTest();
        IGdJobProcess _gdJobProcess;
        protected GdUserQuery GdUserQuery;
        UnfollowerModel UnfollowerModel;
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _gdJobProcess = Substitute.For<IGdJobProcess>();
            _activityType = ActivityType.Unfollow;
            var cancleJob = new CancellationTokenSource();
            _gdJobProcess.ActivityType.Returns(_activityType);
            objIinstaFunctTest = Substitute.For<InstaFunctTest>();
            var module = new ModuleConfiguration();
            JobActivityConfigurationManager[AccountId, _activityType].Returns(module);
            UnfollowerModel = new UnfollowerModel();
            ProcessScopeModel.GetActivitySettingsAs<UnfollowerModel>().Returns(UnfollowerModel);
            var jsonData = JsonConvert.SerializeObject(UnfollowerModel);
            var model = new TemplateModel() { ActivitySettings = jsonData };
            var templatesFileManager = CommonServiceLocator.ServiceLocator.Current.GetInstance<DominatorHouseCore.FileManagers.ITemplatesFileManager>();
            templatesFileManager.GetTemplateById(Arg.Any<string>()).Returns(model);
            
            _gdJobProcess.JobCancellationTokenSource.Returns(cancleJob);
            _gdJobProcess.DominatorAccountModel.Returns(GdDominatorAccountModel);
            GdDominatorAccountModel.IsUserLoggedIn = true;
        }

      // [TestMethod]
        public void Should_search_For_UnFollower_when_ByOutsidesofware_And_BySoftware_Will_Be_Check()
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
            UnfollowerModel = new UnfollowerModel();
            UnfollowerModel.IsChkPeopleFollowedOutsideSoftwareChecked = true;
            UnfollowerModel.IsChkPeopleFollowedBySoftwareChecked = true;
            ProcessScopeModel.GetActivitySettingsAs<UnfollowerModel>().Returns(UnfollowerModel);
            var jsonData = JsonConvert.SerializeObject(UnfollowerModel);
            var model = new TemplateModel() { ActivitySettings = jsonData };
            var templatesFileManager = CommonServiceLocator.ServiceLocator.Current.GetInstance<DominatorHouseCore.FileManagers.ITemplatesFileManager>();
            templatesFileManager.GetTemplateById(Arg.Any<string>()).Returns(model);                       
            _gdJobProcess.ModuleSetting.Returns(UnfollowerModel);
            
          
            var UserFriendShippageresponse = TestUtils.ReadFileFromResources(
               "GramDominatorCore.UnitTests.TestData.UserFriendshipResponse.json", Assembly.GetExecutingAssembly());
            var GetAccountUserFriendShipPageResponse = TestUtils.ReadFileFromResources(
              "GramDominatorCore.UnitTests.TestData.GetAccountUserFollowingResponse.json", Assembly.GetExecutingAssembly());
            UserFriendshipResponse UserFriendShipResponse = new UserFriendshipResponse(new ResponseParameter()
            {
                Response = UserFriendShippageresponse
            });
            FollowerAndFollowingIgResponseHandler GetUserFriendShipResponse = new FollowerAndFollowingIgResponseHandler(new ResponseParameter()
            {
                Response = GetAccountUserFriendShipPageResponse
            });
            //act
            _gdJobProcess.instaFunct.UserFriendship(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>()).Returns(UserFriendShipResponse);
            _gdJobProcess.instaFunct.GetUserFollowings(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(GetUserFriendShipResponse);
            UnfollowProcessor UnFollower = new UnfollowProcessor(_gdJobProcess, AccountServiceScoped, CampaignService,
                ProcessScopeModel, DelayService);
            UnFollower.Start(queryInfo);
            //assert
            _gdJobProcess.Received(1137).FinalProcess(Arg.Any<ScrapeResultNew>());
            _gdJobProcess.Received(3).instaFunct.UserFriendship(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>());
            _gdJobProcess.Received(3).instaFunct.GetUserFollowings(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
        }

       // [TestMethod]
        public void Should_search_For_UnFollower_when_ByOutsidesofware_Will_Be_Check()
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
            UnfollowerModel = new UnfollowerModel();
            UnfollowerModel.IsChkPeopleFollowedOutsideSoftwareChecked = true;
            ProcessScopeModel.GetActivitySettingsAs<UnfollowerModel>().Returns(UnfollowerModel);
            var jsonData = JsonConvert.SerializeObject(UnfollowerModel);
            var model = new TemplateModel() { ActivitySettings = jsonData };
            var templatesFileManager = CommonServiceLocator.ServiceLocator.Current.GetInstance<DominatorHouseCore.FileManagers.ITemplatesFileManager>();
            templatesFileManager.GetTemplateById(Arg.Any<string>()).Returns(model);
            _gdJobProcess.ModuleSetting.Returns(UnfollowerModel);

            var UserFriendShippageresponse = TestUtils.ReadFileFromResources(
               "GramDominatorCore.UnitTests.TestData.UserFriendshipResponse.json", Assembly.GetExecutingAssembly());            
            var FollowingList = TestUtils.ReadFileFromResources(
         "GramDominatorCore.UnitTests.TestData.AccountFollowingList.json", Assembly.GetExecutingAssembly());
            var GetAccountUserFriendShipPageResponse = TestUtils.ReadFileFromResources(
           "GramDominatorCore.UnitTests.TestData.GetAccountUserFollowingResponse.json", Assembly.GetExecutingAssembly());


            UserFriendshipResponse UserFriendShipResponse = new UserFriendshipResponse(new ResponseParameter()
            {
                Response = UserFriendShippageresponse
            });
            FollowerAndFollowingIgResponseHandler GetUserFriendShipResponse = new FollowerAndFollowingIgResponseHandler(new ResponseParameter()
            {
                Response = GetAccountUserFriendShipPageResponse
            });
                      List<Friendships> list = JsonConvert.DeserializeObject<List<Friendships>>(FollowingList);
            AccountServiceScoped.GetFollowings().Returns(list);

            //act
            _gdJobProcess.instaFunct.UserFriendship(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>()).Returns(UserFriendShipResponse);
            _gdJobProcess.instaFunct.GetUserFollowings(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(GetUserFriendShipResponse);
            UnfollowProcessor UnFollower = new UnfollowProcessor(_gdJobProcess, AccountServiceScoped, CampaignService, ProcessScopeModel, DelayService);
            UnFollower.Start(queryInfo);
            //assert
            _gdJobProcess.Received(1135).FinalProcess(Arg.Any<ScrapeResultNew>());
            _gdJobProcess.Received(3).instaFunct.UserFriendship(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>());
            _gdJobProcess.Received(3).instaFunct.GetUserFollowings(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>());

        }

       // [TestMethod]
        public void Should_search_For_UnFollower_when_Bysidesofware_Will_Be_Check()
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
            UnfollowerModel = new UnfollowerModel();
            UnfollowerModel.IsChkPeopleFollowedBySoftwareChecked = true;
            ProcessScopeModel.GetActivitySettingsAs<UnfollowerModel>().Returns(UnfollowerModel);
            var jsonData = JsonConvert.SerializeObject(UnfollowerModel);
            var model = new TemplateModel() { ActivitySettings = jsonData };
            var templatesFileManager = CommonServiceLocator.ServiceLocator.Current.GetInstance<DominatorHouseCore.FileManagers.ITemplatesFileManager>();
            templatesFileManager.GetTemplateById(Arg.Any<string>()).Returns(model);
            _gdJobProcess.ModuleSetting.Returns(UnfollowerModel);


            var UserFriendShippageresponse = TestUtils.ReadFileFromResources(
               "GramDominatorCore.UnitTests.TestData.UserFriendshipResponse.json", Assembly.GetExecutingAssembly());
            var FollowingList = TestUtils.ReadFileFromResources(
        "GramDominatorCore.UnitTests.TestData.AccountFollowerList.json", Assembly.GetExecutingAssembly());
            var GetAccountUserFriendShipPageResponse = TestUtils.ReadFileFromResources(
         "GramDominatorCore.UnitTests.TestData.GetAccountUserFollowingResponse.json", Assembly.GetExecutingAssembly());
            UserFriendshipResponse UserFriendShipResponse = new UserFriendshipResponse(new ResponseParameter()
            {
                Response = UserFriendShippageresponse
            });
            FollowerAndFollowingIgResponseHandler GetUserFriendShipResponse = new FollowerAndFollowingIgResponseHandler(new ResponseParameter()
            {
                Response = GetAccountUserFriendShipPageResponse
            });

            List<InteractedUsers> list = JsonConvert.DeserializeObject<List<InteractedUsers>>(FollowingList);
            AccountServiceScoped.GetInteractedUserFriends().Returns(list);

            _gdJobProcess.instaFunct.UserFriendship(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>()).Returns(UserFriendShipResponse);
            _gdJobProcess.instaFunct.GetUserFollowings(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(GetUserFriendShipResponse);
            UnfollowProcessor UnFollower = new UnfollowProcessor(_gdJobProcess, AccountServiceScoped, CampaignService,
                ProcessScopeModel, DelayService);
            UnFollower.Start(queryInfo);
            //assert
            _gdJobProcess.Received(0).FinalProcess(Arg.Any<ScrapeResultNew>());
            _gdJobProcess.Received(3).instaFunct.UserFriendship(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>());
            _gdJobProcess.Received(3).instaFunct.GetUserFollowings(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>());

        }
    }
}
