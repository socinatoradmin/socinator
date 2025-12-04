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
using GramDominatorCore.UnitTests.Tests.Library.ProcessorTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NSubstitute;
using System;
using System.Reflection;
using System.Threading;

namespace GramDominatorCore.UnitTests.Tests.Library.UserProcessors.ProcessorTest
{
    [TestClass]
    public class KeywordProcessorTest : BaseGdProcessTest
    {
        private ActivityType _activityType;    
        InstaFunctTest objIinstaFunctTest = new InstaFunctTest();
        IGdJobProcess _gdJobProcess;
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _gdJobProcess = Substitute.For<IGdJobProcess>();
            _activityType = ActivityType.Follow;
            var cancleJob = new CancellationTokenSource();
            _gdJobProcess.ActivityType.Returns(_activityType);
            objIinstaFunctTest = Substitute.For<InstaFunctTest>();
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
        public void should_search_For_Keyword()
        {
            // arrange    
            var queryInfo = new QueryInfo
            {
                QueryType = "Keyword",
                QueryValue = "sachin"
            };
            var jobResult = new JobProcessResult();
            _gdJobProcess.FinalProcess(Arg.Any<ScrapeResultNew>()).Returns(jobResult);           
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            var PageResponse = TestUtils.ReadFileFromResources(
             "GramDominatorCore.UnitTests.TestData.SearchKeywordResponse.json", Assembly.GetExecutingAssembly());
            SearchKeywordIgResponseHandler userResponse = new SearchKeywordIgResponseHandler(new ResponseParameter() { Response = PageResponse });
            //act
            _gdJobProcess.instaFunct.SearchForkeyword(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(userResponse);
            KeywordProcessor keywordProcessor = new KeywordProcessor(_gdJobProcess, AccountServiceScoped,
                CampaignService, ProcessScopeModel, DelayService);
            keywordProcessor.Start(queryInfo);
            //assert
            _gdJobProcess.Received(51).FinalProcess(Arg.Any<ScrapeResultNew>());
            _gdJobProcess.instaFunct.Received(1).SearchForkeyword(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>());

        }
    }
}
