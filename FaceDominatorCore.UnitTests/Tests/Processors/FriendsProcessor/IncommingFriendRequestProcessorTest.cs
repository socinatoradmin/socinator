using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FaceDominatorCore.FDLibrary.FdProcessors.FriendsProcessor;
using FaceDominatorCore.FDLibrary.FdProcesses;
using NSubstitute;
using Unity;
using FaceDominatorCore.FDModel.CommonSettings;
using Dominator.Tests.Utils;
using DominatorHouseCore.Request;
using FaceDominatorCore.FDResponse.FriendsResponse;
using System.Reflection;
using DominatorHouseCore.Models;
using System.Threading;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using NSubstitute.Core.Arguments;
using FaceDominatorCore.FDResponse.CommonResponse;

namespace FaceDominatorCore.UnitTests.Tests.Processors.FriendsProcessor
{
    [TestClass]
    public class IncommingFriendRequestProcessorTest : BaseFacebookProcessorTest
    {
        private IncommingFriendRequestProcessor _sut;
        private IFdJobProcess _fdJobProcess;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            _fdJobProcess = Substitute.For<IFdJobProcess>();
            Container.RegisterInstance(_fdJobProcess);
            var moduleSetting = new ModuleSetting();

            ProcessScopeModel.GetActivitySettingsAs<ModuleSetting>().Returns(moduleSetting);
        }

        [TestMethod]
        public void IncommingFriendRequestProcessor_Should_Execute_Successfully()
        {
            _fdJobProcess = new CommentLikerProcesss(ProcessScopeModel, DbAccountServiceScoped, 
                FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);

            var response = TestUtils.ReadFileFromResources
            ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.IncomingFriendRequestResponse.html",
                Assembly.GetExecutingAssembly());
            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var responseHandler = new IncomingFriendListResponseHandler(responseParameters);

            var queryInfo = new QueryInfo { QueryValue = "" };
            _fdJobProcess.FinalProcess(new ScrapeResultNew());

            FdRequestLibrary.GetIncomingFriendRequests(Arg.Any<DominatorAccountModel>(), null)
                .Returns(responseHandler);

            _sut = new IncommingFriendRequestProcessor
            (_fdJobProcess, DbAccountServiceScoped, DbCampaignServiceScoped, FdRequestLibrary,
               BrowserManager, ProcessScopeModel);
            _fdJobProcess.ModuleSetting.ManageFriendsModel.IsAcceptRequest = true;

            var aboutFriendResponse = TestUtils.ReadFileFromResources(
                "FaceDominatorCore.UnitTests.TestData.FdLibraryData.FriendAboutResponse.html",
                Assembly.GetExecutingAssembly());

            responseParameters = new ResponseParameter
            {
                Response = aboutFriendResponse,
                Exception = null,
                HasError = false
            };

            var responseHandlerInfo = new FdUserInfoResponseHandlerMobile(responseParameters, new FacebookUser());
            responseHandlerInfo.ObjFdScraperResponseParameters.FacebookUser.UserId = "100032129147843";
            FdRequestLibrary.GetDetailedInfoUserMobile(Arg.Any<FacebookUser>(), Arg.Any<DominatorAccountModel>(),
                Arg.Any<bool>(), Arg.Any<bool>()).Returns(responseHandlerInfo);

            // act
            _sut.Start(queryInfo);

            // assert
            FdRequestLibrary.Received(1).GetIncomingFriendRequests(Arg.Any<DominatorAccountModel>(), null);

            _fdJobProcess = Substitute.For<IFdJobProcess>();
            Container.RegisterInstance(_fdJobProcess);
            var scrapeResultNew = new ScrapeResultNew();
            _fdJobProcess.FinalProcess(scrapeResultNew);
            _fdJobProcess.Received(1).FinalProcess(scrapeResultNew);
        }

    }
}
