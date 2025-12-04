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
using GramDominatorCore.UnitTests.Test_Data.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NSubstitute;
using System;
using System.Reflection;
using System.Threading;
namespace GramDominatorCore.UnitTests.Tests.Library.ProcessorTest.UserProcessors
{
    [TestClass]
    public class HashtagScraperProcessorTest: BaseGdProcessTest
    {
        private ActivityType _activityType;       
        IGdJobProcess _gdJobProcess;
        TemplateModel model { get; set; }
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _gdJobProcess = Substitute.For<IGdJobProcess>();
            _activityType = ActivityType.HashtagsScraper;
            var cancleJob = new CancellationTokenSource();
            _gdJobProcess.ActivityType.Returns(_activityType);          
            var module = new ModuleConfiguration();
            JobActivityConfigurationManager[AccountId, _activityType].Returns(module);
            HashtagsScraperModel hashTagScraperModel = new HashtagsScraperModel();
            hashTagScraperModel.LstKeyword.Add("car");
            ProcessScopeModel.GetActivitySettingsAs<HashtagsScraperModel>().Returns(hashTagScraperModel);
            var jsonData = JsonConvert.SerializeObject(hashTagScraperModel);
            var model = new TemplateModel() { ActivitySettings = jsonData };
            var templatesFileManager = CommonServiceLocator.ServiceLocator.Current.GetInstance<DominatorHouseCore.FileManagers.ITemplatesFileManager>();
            templatesFileManager.GetTemplateById(Arg.Any<string>()).Returns(model);
            _gdJobProcess.DominatorAccountModel.Returns(GdDominatorAccountModel);
            _gdJobProcess.JobCancellationTokenSource.Returns(cancleJob);
            _gdJobProcess.ModuleSetting.Returns(hashTagScraperModel);
            _gdJobProcess.instaFunct = _gdJobProcess.loginProcess.InstagramFunctFactory.InstaFunctions;
        }

        [TestMethod]
        public void Should_Search_HashTagScraperProcessor()
        {
            //arranges
            var queryInfo = new QueryInfo
            {
                QueryType = "",
                QueryValue = ""
            };

            var jobResult = new JobProcessResult();
            _gdJobProcess.FinalProcess(Arg.Any<ScrapeResultNew>()).Returns(jobResult);
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);

            var PageResponse = TestUtils.ReadFileFromResources(
              "GramDominatorCore.UnitTests.TestData.SearchForTagResponse.json", Assembly.GetExecutingAssembly());


            SearchTagIgResponseHandler userResponse = new SearchTagIgResponseHandler(new ResponseParameter() { Response = PageResponse });

            //act
            _gdJobProcess.instaFunct.SearchForTag(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>()).Returns(userResponse);
            HashtagScraperProcessor autoReplyToNewMessageProcessor = new HashtagScraperProcessor(_gdJobProcess,
                AccountServiceScoped, CampaignService, ProcessScopeModel, DelayService);
            autoReplyToNewMessageProcessor.Start(queryInfo);
            //assert
            _gdJobProcess.Received(50).FinalProcess(Arg.Any<ScrapeResultNew>());         
            _gdJobProcess.instaFunct.Received(1).SearchForTag(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>());

        }
    }
}
