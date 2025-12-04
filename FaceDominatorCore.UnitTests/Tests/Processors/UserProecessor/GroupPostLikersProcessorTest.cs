using System.Reflection;
using CommonServiceLocator;
using Dominator.Tests.Utils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDLibrary.FdProcessors.UserProecessor;
using FaceDominatorCore.FDModel.CommonSettings;
using FaceDominatorCore.FDResponse.CommonResponse;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Unity;

namespace FaceDominatorCore.UnitTests.Tests.Processors.UserProecessor
{
    [TestClass]
    public class GroupPostLikersProcessorTest : BaseFacebookProcessorTest
    {
        private GroupPostLikersProcessor _sut;
        private IFdJobProcess _fdJobProcess;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            _fdJobProcess = Substitute.For<IFdJobProcess>();
            Container.RegisterInstance(_fdJobProcess);
            var moduleSetting = new ModuleSetting();
            ProcessScopeModel.GetActivitySettingsAs<ModuleSetting>().Returns(moduleSetting);

            var moduleConfiguration = new ModuleConfiguration();
            var jobActivityConfigurationManager = ServiceLocator.Current.GetInstance<IJobActivityConfigurationManager>();
            jobActivityConfigurationManager[Arg.Any<string>(), Arg.Any<ActivityType>()].Returns(moduleConfiguration);
        }

        [TestMethod]
        public void GroupPostLikersProcessor_Should_Execute_Successfully()
        {
            var cancellationTokenSource = new System.Threading.CancellationTokenSource();
            _fdJobProcess.JobCancellationTokenSource.Returns(cancellationTokenSource);

            _fdJobProcess = new CommentLikerProcesss(ProcessScopeModel, DbAccountServiceScoped, FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);

            var response = TestUtils.ReadFileFromResources
      ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostListFromFanpagesResponse.html", Assembly.GetExecutingAssembly());

            var responseParameter = new ResponseParameter()
            {
                Response = response,
                HasError = false,
                Exception = null
            };

            var listOfKeyValuePair =
                new PostReactionListResponseHandler(responseParameter).ListPostReaction;

            response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GroupMemberCount.html", Assembly.GetExecutingAssembly());

            responseParameter = new ResponseParameter()
            {
                Response = response,
                HasError = false,
                Exception = null
            };

            var responseHandler = new ScrapGroupPostListResponseHandler
                (responseParameter, listOfKeyValuePair, Arg.Any<string>())
            {
                Status = true
            };

            responseHandler.Status = true;

            var queryInfo = new QueryInfo { QueryValue = "" };
            _fdJobProcess.FinalProcess(new ScrapeResultNew());

            FdRequestLibrary.GetPostListFromGroupsNew
                (Arg.Any<DominatorAccountModel>(), null, Arg.Any<string>()).Returns(responseHandler);

            _sut = new GroupPostLikersProcessor
                (_fdJobProcess, DbAccountServiceScoped, DbCampaignServiceScoped, FdRequestLibrary, 
                BrowserManager, ProcessScopeModel);

            // act
            _sut.Start(queryInfo);

            // assert
            FdRequestLibrary.Received(1).GetPostListFromGroupsNew
                (Arg.Any<DominatorAccountModel>(), null, Arg.Any<string>());

            _fdJobProcess = Substitute.For<IFdJobProcess>();
            Container.RegisterInstance(_fdJobProcess);
            var scrapeResultNew = new ScrapeResultNew();
            _fdJobProcess.FinalProcess(scrapeResultNew);
            _fdJobProcess.Received(1).FinalProcess(scrapeResultNew);
        }

        [TestMethod]
        public void GroupPostLikersProcessor_Should_Failed_To_Execute()
        {
            var queryInfo = new QueryInfo();
            _fdJobProcess = new CommentLikerProcesss(ProcessScopeModel, DbAccountServiceScoped, FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);

            _sut = new GroupPostLikersProcessor
            (_fdJobProcess, DbAccountServiceScoped, DbCampaignServiceScoped, FdRequestLibrary,
                BrowserManager, ProcessScopeModel);

            FdRequestLibrary.GetPostListFromGroupsNew
                (Arg.Any<DominatorAccountModel>(), null, Arg.Any<string>()).Returns((ScrapGroupPostListResponseHandler)null);
            // act
            _sut.Start(queryInfo);

            // assert
            FdRequestLibrary.Received(1).GetPostListFromGroupsNew
                (Arg.Any<DominatorAccountModel>(), null, Arg.Any<string>());

        }

    }
}
