using System.Reflection;
using CommonServiceLocator;
using Dominator.Tests.Utils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDLibrary.FdProcessors.UserProecessor;
using FaceDominatorCore.FDModel.CommonSettings;
using FaceDominatorCore.FDResponse.CommonResponse;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Unity;
using System.Collections.Generic;
using DominatorHouseCore.Models.SocioPublisher;
using System.Threading;

namespace FaceDominatorCore.UnitTests.Tests.Processors.UserProecessor
{
    [TestClass]
    public class UserFollowerProcessorTest : BaseFacebookProcessorTest
    {
        private UserFollowerProcessor _sut;
        private IFdJobProcess _fdJobProcess;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            List<string> accountList = new List<string>
            {
                "00000000-0000-0000-0000-000000000000"
            };

            var moduleSetting = new ModuleSetting();
            moduleSetting.SelectAccountDetailsModel = new SelectAccountDetailsModel
            {
                SelectedAccountIds = accountList,

            };
            ProcessScopeModel.GetActivitySettingsAs<ModuleSetting>().Returns(moduleSetting);

            _fdJobProcess = Substitute.For<IFdJobProcess>();
            Container.RegisterInstance(_fdJobProcess);

        }

        [TestMethod]
        public void UserFollowerProcessor_Should_Execute_Successfully()
        {

            var cancellationTokenSource = new CancellationTokenSource();
            _fdJobProcess.JobCancellationTokenSource.Returns(cancellationTokenSource);

            var queryInfo = new QueryInfo { QueryValue = "" };
            _fdJobProcess = new CommentLikerProcesss(ProcessScopeModel, DbAccountServiceScoped, FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);

            var response = TestUtils.ReadFileFromResources
      ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetUserFollowersResponse.html", Assembly.GetExecutingAssembly());

            var responseParameter = new ResponseParameter()
            {
                Response = response,
                HasError = false,
                Exception = null
            };

            var responseHandler = new FanpageLikersResponseHandler
                (responseParameter, Arg.Any<FdPageLikersParameters>());

            _fdJobProcess.FinalProcess(new ScrapeResultNew());

            FdRequestLibrary.GetUserFollowers
                (Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), null).Returns(responseHandler);

            _sut = new UserFollowerProcessor
                (_fdJobProcess, DbAccountServiceScoped, DbCampaignServiceScoped, FdRequestLibrary,
                BrowserManager, ProcessScopeModel);

            // act
            _sut.Start(queryInfo);

            // assert
            FdRequestLibrary.Received(1).GetUserFollowers
                (Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), null);

            _fdJobProcess = Substitute.For<IFdJobProcess>();
            Container.RegisterInstance(_fdJobProcess);
            var scrapeResultNew = new ScrapeResultNew();
            _fdJobProcess.FinalProcess(scrapeResultNew);
            _fdJobProcess.Received(1).FinalProcess(scrapeResultNew);
        }

        [TestMethod]
        public void UserFollowerProcessor_Should_Failed_To_Execute()
        {
            var queryInfo = new QueryInfo();
            _fdJobProcess = new CommentLikerProcesss(ProcessScopeModel, DbAccountServiceScoped, FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);

            _sut = new UserFollowerProcessor
                (_fdJobProcess, DbAccountServiceScoped, DbCampaignServiceScoped, FdRequestLibrary,
                BrowserManager, ProcessScopeModel);

            FdRequestLibrary.GetUserFollowers(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), null).
                Returns((FanpageLikersResponseHandler)null);

            // act
            _sut.Start(queryInfo);

            // assert
            FdRequestLibrary.Received(1).GetUserFollowers
                (Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), null);
        }

    }
}
