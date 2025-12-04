using Dominator.Tests.Utils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary;
using GramDominatorCore.GDLibrary.Processor.Post;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using GramDominatorCore.Response;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Reflection;
using System.Threading;

namespace GramDominatorCore.UnitTests.Tests.Library.ProcessorTest.PostProcessors
{
    [TestClass]
    public class CustomPostProcessorTest :BaseGdProcessTest
    {
        private ActivityType _activityType;
        CustomPostProcessor CustomUser;
        IGdJobProcess _gdJobProcess;
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            var cancleJob = new CancellationTokenSource();
            _gdJobProcess = Substitute.For<IGdJobProcess>();
            _activityType = ActivityType.Comment;
            _gdJobProcess.ActivityType.Returns(_activityType);         
            var module = new ModuleConfiguration();
            JobActivityConfigurationManager[AccountId, _activityType].Returns(module);         
            CommentModel commentModel = new CommentModel();
            ProcessScopeModel.GetActivitySettingsAs<CommentModel>().Returns(commentModel);
            var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(GdModuleSetting);
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
        public void CustomUserProcessor_Will_return_true()
        {
            //arrange
            var queryInfo = new QueryInfo
            {
                QueryType = "CustomPost",
                QueryValue = "Brk_sDoBHx0"
            };
            InstaFunct = GdLogInProcess.InstagramFunctFactory.InstaFunctions;
            var jobResult = new JobProcessResult();
            _gdJobProcess.FinalProcess(Arg.Any<ScrapeResultNew>()).Returns(jobResult);
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);

            var pageresponse = TestUtils.ReadFileFromResources(
               "GramDominatorCore.UnitTests.TestData.MediaInfoResponse.json", Assembly.GetExecutingAssembly());

            MediaInfoIgResponseHandler mediaInfoIgResponseHandler = new MediaInfoIgResponseHandler(new ResponseParameter
            {
                Response = pageresponse
            });
            //act
            _gdJobProcess.instaFunct.MediaInfo(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(mediaInfoIgResponseHandler);
            CustomUser = new CustomPostProcessor(_gdJobProcess, AccountServiceScoped,
               CampaignService, ProcessScopeModel, DelayService);
            CustomUser.Start(queryInfo);
            //assert
            _gdJobProcess.Received(1).FinalProcess(Arg.Any<ScrapeResultNew>());
            _gdJobProcess.Received(1).instaFunct.MediaInfo(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(mediaInfoIgResponseHandler);
        }

        [TestMethod]
        [Timeout(10000)]
        public void CustomUserProcessor_Will_return_Null()
        {
            //arrange
            var queryInfo = new QueryInfo
            {
                QueryType = "CustomPost",
                QueryValue = ""
            };
            var jobResult = new JobProcessResult();           
            _gdJobProcess.FinalProcess(Arg.Any<ScrapeResultNew>()).Returns(jobResult);
            _gdJobProcess.ModuleSetting.Returns(GdModuleSetting);
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                 "GramDominatorCore.UnitTests.TestData.MediaInfoResponse.json", Assembly.GetExecutingAssembly());

            MediaInfoIgResponseHandler mediaInfoIgResponseHandler = new MediaInfoIgResponseHandler(new ResponseParameter
            {
                Response = "{\"message\": \"Media not found or unavailable\", \"status\": \"fail\"}",
                
            });
            //act
            _gdJobProcess.instaFunct.MediaInfo(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>(),Arg.Any<CancellationToken>()).Returns(mediaInfoIgResponseHandler);
            CustomUser = new CustomPostProcessor(_gdJobProcess, AccountServiceScoped, CampaignService, ProcessScopeModel, DelayService);
            CustomUser.Start(queryInfo);
         //assert
            _gdJobProcess.Received(1).instaFunct.MediaInfo(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
        }
    }
}
