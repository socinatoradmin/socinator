using Dominator.Tests.Utils;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary;
using GramDominatorCore.GDLibrary.Processor.Post;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NSubstitute;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace GramDominatorCore.UnitTests.Tests.Library.ProcessorTest.PostProcessors
{
    [TestClass]
    public class DeletePostProcessorTest :BaseGdProcessTest
    {
        private ActivityType _activityType;
        DeletePostProcessor CustomUser;
        IGdJobProcess _gdJobProcess;      
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            var cancleJob = new CancellationTokenSource();
            _gdJobProcess = Substitute.For<IGdJobProcess>();
            _activityType = ActivityType.DeletePost;
            _gdJobProcess.ActivityType.Returns(_activityType);          
            var module = new ModuleConfiguration();
            JobActivityConfigurationManager[AccountId, _activityType].Returns(module);
            DeletePostModel deletepostModel = new DeletePostModel();
            ProcessScopeModel.GetActivitySettingsAs<DeletePostModel>().Returns(deletepostModel);
            var jsonData =JsonConvert.SerializeObject(GdModuleSetting);
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
        public void DeletePostProcess_Will_Return_No_More_Data_When_Post_Will_Be_Zero()
        {
            //arrange
            var queryInfo = new QueryInfo
            {
                QueryType = "DeletePost",
                QueryValue = ""
            };
            var jobResult = new JobProcessResult();          
            _gdJobProcess.FinalProcess(Arg.Any<ScrapeResultNew>()).Returns(jobResult);         
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            //act
            CustomUser = new DeletePostProcessor(GdJobProcess, AccountServiceScoped, CampaignService, ProcessScopeModel, DelayService);
            CustomUser.Start(queryInfo);
            //assert
            _gdJobProcess.Received(0).FinalProcess(Arg.Any<ScrapeResultNew>());
           
        }
        [TestMethod]
        [Timeout(15000)]
        public void Check_Posted_By_Software_Sucess()
        {
            //arrange
            var queryInfo = new QueryInfo
            {
                QueryType = "CustomPost",
                QueryValue = ""
            };
            var jobResult = new JobProcessResult();         
            _gdJobProcess.FinalProcess(Arg.Any<ScrapeResultNew>()).Returns(jobResult);
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.Post_PostedBy_Software.json", Assembly.GetExecutingAssembly());
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            GdJobProcess.ModuleSetting.ChkDeletePostWhichIsPostedBySoftware = true;
            List<FeedInfoes> list = JsonConvert.DeserializeObject<List<FeedInfoes>>(pageresponse);
            AccountServiceScoped.GetFeedInfos().Returns(list);
            //act
            CustomUser = new DeletePostProcessor(GdJobProcess, AccountServiceScoped, CampaignService, ProcessScopeModel, DelayService);
            CustomUser.Start(queryInfo);
            //assert
            _gdJobProcess.Received(0).FinalProcess(Arg.Any<ScrapeResultNew>());
        }

        [TestMethod]
        [Timeout(15000)]
        public void Check_Posted_By_OutSoftware_Sucess()
        {
            //arrange
            var queryInfo = new QueryInfo
            {
                QueryType = "CustomPost",
                QueryValue = ""
            };
            var jobResult = new JobProcessResult()
            {
                IsProcessCompleted = true
            };           
            _gdJobProcess.FinalProcess(Arg.Any<ScrapeResultNew>()).Returns(jobResult);
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            GdJobProcess.ModuleSetting.ChkDeletePostWhichIsPostedByOutsideSoftware = true;
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.Post_Posted_By_Outside_Software.json", Assembly.GetExecutingAssembly());
            List<FeedInfoes> list = JsonConvert.DeserializeObject<List<FeedInfoes>>(pageresponse);
            AccountServiceScoped.GetFeedInfos().Returns(list);
            //act
            CustomUser = new DeletePostProcessor(GdJobProcess, AccountServiceScoped, CampaignService, ProcessScopeModel, DelayService);
            CustomUser.Start(queryInfo);
            //assert
            _gdJobProcess.Received(0).FinalProcess(Arg.Any<ScrapeResultNew>());
        }
        [TestMethod]
        [Timeout(15000)]
        public void Check_Posted_By_OutSoftware_Or_By_Software_Sucess()
        {
            var queryInfo = new QueryInfo
            {
                QueryType = "CustomPost",
                QueryValue = ""
            };
            var jobResult = new JobProcessResult();           
            _gdJobProcess.FinalProcess(Arg.Any<ScrapeResultNew>()).Returns(jobResult);
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            GdJobProcess.ModuleSetting.ChkDeletePostWhichIsPostedBySoftware = true;
            GdJobProcess.ModuleSetting.ChkDeletePostWhichIsPostedByOutsideSoftware = true;
            var pageresponse = TestUtils.ReadFileFromResources(
               "GramDominatorCore.UnitTests.TestData.Posted_By_Software_And_OutSideSoftware.json", Assembly.GetExecutingAssembly());
            List<FeedInfoes> list = JsonConvert.DeserializeObject<List<FeedInfoes>>(pageresponse);
            AccountServiceScoped.GetFeedInfos().Returns(list);
            CustomUser = new DeletePostProcessor(GdJobProcess, AccountServiceScoped, CampaignService, ProcessScopeModel, DelayService);
            CustomUser.Start(queryInfo);
            _gdJobProcess.Received(0).FinalProcess(Arg.Any<ScrapeResultNew>());
        }
    }
}
