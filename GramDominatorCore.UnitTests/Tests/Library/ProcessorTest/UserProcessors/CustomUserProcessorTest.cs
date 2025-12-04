using Dominator.Tests.Utils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary;
using GramDominatorCore.GDLibrary.Processor.User;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using GramDominatorCore.UnitTests.Test_Data.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NSubstitute;
using System.Reflection;
using System.Threading;

namespace GramDominatorCore.UnitTests.Tests.Library.ProcessorTest.UserProcessors
{
    [TestClass]
    public class CustomUserProcessorTest: BaseGdProcessTest
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
            var jsonData = JsonConvert.SerializeObject(FollowerModel);
            var model = new TemplateModel() { ActivitySettings = jsonData };
            var templatesFileManager = CommonServiceLocator.ServiceLocator.Current.GetInstance<DominatorHouseCore.FileManagers.ITemplatesFileManager>();
            templatesFileManager.GetTemplateById(Arg.Any<string>()).Returns(model);
            _gdJobProcess.DominatorAccountModel.Returns(GdDominatorAccountModel);
            _gdJobProcess.JobCancellationTokenSource.Returns(cancleJob);
            _gdJobProcess.ModuleSetting.Returns(FollowerModel);
            _gdJobProcess.instaFunct = _gdJobProcess.loginProcess.InstagramFunctFactory.InstaFunctions;
        }

        [TestMethod]
        [Timeout(15000)]
        public void CustomUserProcessor()
        {
            //arrange
            var queryInfo = new QueryInfo
            {
                QueryType = "Suggested Users",
                QueryValue = "sachin"
            };
            var jobResult = new JobProcessResult();
            _gdJobProcess.FinalProcess(Arg.Any<ScrapeResultNew>()).Returns(jobResult);
            var requestParameters = new IgRequestParameters();
            var pageresponse = TestUtils.ReadFileFromResources(
              "GramDominatorCore.UnitTests.TestData.SearchUserNameResponse.json", Assembly.GetExecutingAssembly());
            UsernameInfoIgResponseHandler usernameResponse = new UsernameInfoIgResponseHandler(new ResponseParameter
            {
                Response = pageresponse
            });
            //act
            _gdJobProcess.instaFunct.SearchUsername(Arg.Any<DominatorAccountModel>(),Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(usernameResponse);
            CustomUsersProcessors CustomUser = new CustomUsersProcessors(_gdJobProcess, AccountServiceScoped, CampaignService, ProcessScopeModel, DelayService);
            CustomUser.Start(queryInfo);
            //assert
            _gdJobProcess.Received(1).FinalProcess(Arg.Any<ScrapeResultNew>());
            _gdJobProcess.Received(1).instaFunct.SearchUsername(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>() );

        }
    }
}
