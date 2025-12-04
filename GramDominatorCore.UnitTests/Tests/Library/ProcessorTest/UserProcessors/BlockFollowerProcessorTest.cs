using Dominator.Tests.Utils;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary;
using GramDominatorCore.GDLibrary.Processor.User;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using GramDominatorCore.UnitTests.Test_Data.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NSubstitute;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace GramDominatorCore.UnitTests.Tests.Library.ProcessorTest.UserProcessors
{
    [TestClass]
    public class BlockFollowerProcessorTest:BaseGdProcessTest
    {
        private ActivityType _activityType;
        public List<Friendships> allFollowers = new List<Friendships>();
        IGdJobProcess _gdJobProcess;
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _gdJobProcess = Substitute.For<IGdJobProcess>();
            _activityType = ActivityType.BlockFollower;
            var cancleJob = new CancellationTokenSource();
            _gdJobProcess.ActivityType.Returns(_activityType);           
            var module = new ModuleConfiguration();
            JobActivityConfigurationManager[AccountId, _activityType].Returns(module);

            BlockFollowerModel blockFollowerModel = new BlockFollowerModel();
            ProcessScopeModel.GetActivitySettingsAs<BlockFollowerModel>().Returns(blockFollowerModel);
            var jsonData = JsonConvert.SerializeObject(blockFollowerModel);
            var model = new TemplateModel() { ActivitySettings = jsonData };
            var templatesFileManager = CommonServiceLocator.ServiceLocator.Current.GetInstance<DominatorHouseCore.FileManagers.ITemplatesFileManager>();
            templatesFileManager.GetTemplateById(Arg.Any<string>()).Returns(model);
            _gdJobProcess.DominatorAccountModel.Returns(GdDominatorAccountModel);
            _gdJobProcess.JobCancellationTokenSource.Returns(cancleJob);
            _gdJobProcess.ModuleSetting.Returns(blockFollowerModel);
        }

        [TestMethod]
        public void should_search_For_BlockFollower()
        {
            // arrange    
            var queryInfo = new QueryInfo
            {
                QueryType = "",
                QueryValue = ""
            };
            var jobResult = new JobProcessResult();
            _gdJobProcess.FinalProcess(Arg.Any<ScrapeResultNew>()).Returns(jobResult);
            _gdJobProcess.ModuleSetting.Returns(GdModuleSetting);
            var requestParameters = new IgRequestParameters();
            var pageresponse = TestUtils.ReadFileFromResources(
         "GramDominatorCore.UnitTests.TestData.OwnFollowerList.json", Assembly.GetExecutingAssembly());
            List<Friendships> list = JsonConvert.DeserializeObject<List<Friendships>>(pageresponse);
            AccountServiceScoped.GetFollowers().Returns(list);
            //act     
            BlockFollowerProcessor OwnFollower = new BlockFollowerProcessor(_gdJobProcess, AccountServiceScoped, CampaignService, ProcessScopeModel, DelayService);
            OwnFollower.Start(queryInfo);
            //assert
            _gdJobProcess.Received(175).FinalProcess(Arg.Any<ScrapeResultNew>());
        }
    }
}
