using Dominator.Tests.Utils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary;
using GramDominatorCore.GDLibrary.Processor.User;
using GramDominatorCore.GDLibrary.Response;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NSubstitute;
using System.Reflection;
using System.Threading;

namespace GramDominatorCore.UnitTests.Tests.Library.ProcessorTest
{
    [TestClass]
    public class SuggestedUserProcessor : BaseGdProcessTest
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
        public void Should_Search_Suggested_User()
        {
            var queryInfo = new QueryInfo
            {
                QueryType = "Suggested Users",
                QueryValue = "sachin"
            };
            var jobResult = new JobProcessResult();
            _gdJobProcess.FinalProcess(Arg.Any<ScrapeResultNew>()).Returns(jobResult);
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            var SearchUserNameResponse = TestUtils.ReadFileFromResources(
               "GramDominatorCore.UnitTests.TestData.SearchUserNameResponse.json", Assembly.GetExecutingAssembly());     
            var GetSuggestedUserResponse = TestUtils.ReadFileFromResources(
               "GramDominatorCore.UnitTests.TestData.GetSuggestedUsersResponse.json", Assembly.GetExecutingAssembly());
            UsernameInfoIgResponseHandler usernameResponse = new UsernameInfoIgResponseHandler(new ResponseParameter()
            {
                Response = SearchUserNameResponse
            });        
            SuggestedUsersIgResponseHandler GetSuggestedUser = new SuggestedUsersIgResponseHandler(new ResponseParameter()
            {
                Response = GetSuggestedUserResponse
            });
            //act
            _gdJobProcess.instaFunct.SearchUsername(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(usernameResponse);      
            _gdJobProcess.instaFunct.GetSuggestedUsers(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>(), Arg.Any<string>()).Returns(GetSuggestedUser);
            SuggestedUsersProcessor SuggestedUser = new SuggestedUsersProcessor(_gdJobProcess, AccountServiceScoped,
                CampaignService, ProcessScopeModel, DelayService);
            SuggestedUser.Start(queryInfo);
            //assert
            _gdJobProcess.Received(30).FinalProcess(Arg.Any<ScrapeResultNew>());
            _gdJobProcess.Received(2).instaFunct.SearchUsername(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>());            
            _gdJobProcess.Received(2).instaFunct.GetSuggestedUsers(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>() ,Arg.Any<string>());

        }
    }
}
