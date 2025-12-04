using System.Reflection;
using CommonServiceLocator;
using Dominator.Tests.Utils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
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
    public class FriendOFriendProcessorTest : BaseFacebookProcessorTest
    {
        private FriendOFriendProcessor _sut;
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
        public void FriendOFriendProcessor_Should_Execute_Successfully()
        {

            var cancellationTokenSource = new System.Threading.CancellationTokenSource();
            _fdJobProcess.JobCancellationTokenSource.Returns(cancellationTokenSource);

            _fdJobProcess = new CommentLikerProcesss(ProcessScopeModel, DbAccountServiceScoped, FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);

            var response = TestUtils.ReadFileFromResources
      ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.FriendOfFriendResponse.html", Assembly.GetExecutingAssembly());

            var responseParameter = new ResponseParameter()
            {
                Response = response,
                HasError = false,
                Exception = null
            };

            var responseHandler = new FdFriendOfFriendResponseHandler
                (responseParameter, Arg.Any<bool>(), Arg.Any<string>());

            var queryInfo = new QueryInfo { QueryValue = "" };
            _fdJobProcess.FinalProcess(new ScrapeResultNew());

            FdRequestLibrary.GetFriendOfFriend
                (Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), null).Returns(responseHandler);
            _sut = new FriendOFriendProcessor
                (_fdJobProcess, DbAccountServiceScoped, DbCampaignServiceScoped, FdRequestLibrary,
                BrowserManager, ProcessScopeModel);

            // act
            _sut.Start(queryInfo);

            // assert
            FdRequestLibrary.Received(1).GetFriendOfFriend
                (Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), null);

            _fdJobProcess = Substitute.For<IFdJobProcess>();
            Container.RegisterInstance(_fdJobProcess);
            var scrapeResultNew = new ScrapeResultNew();
            _fdJobProcess.FinalProcess(scrapeResultNew);
            _fdJobProcess.Received(1).FinalProcess(scrapeResultNew);
        }

        [TestMethod]
        public void FriendOFriendProcessor_Should_Failed_To_Execute()
        {
            var queryInfo = new QueryInfo();
            _fdJobProcess = new CommentLikerProcesss(ProcessScopeModel, DbAccountServiceScoped, FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);

            _sut = new FriendOFriendProcessor
                (_fdJobProcess, DbAccountServiceScoped, DbCampaignServiceScoped, FdRequestLibrary,
                BrowserManager, ProcessScopeModel);

            FdRequestLibrary.GetFriendOfFriend
                (Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), null).Returns((FdFriendOfFriendResponseHandler)null);

            // act
            _sut.Start(queryInfo);

            // assert
            FdRequestLibrary.Received(1).GetFriendOfFriend
                (Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), null);
        }

    }
}
