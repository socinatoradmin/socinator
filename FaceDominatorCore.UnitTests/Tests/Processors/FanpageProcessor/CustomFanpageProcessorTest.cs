using System.Reflection;
using Dominator.Tests.Utils;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDLibrary.FdProcessors.FanpageProcessor;
using FaceDominatorCore.FDResponse.ScrapersResponse;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Unity;

namespace FaceDominatorCore.UnitTests.Tests.Processors.FanpageProcessor
{
    [TestClass]
    public class CustomFanpageProcessorTest : BaseFacebookProcessorTest
    {
        private CustomFanpageProcessor _sut;
        private IFdJobProcess _fdJobProcess;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            _fdJobProcess = Substitute.For<IFdJobProcess>();
            Container.RegisterInstance(_fdJobProcess);
        }

        [TestMethod]
        public void CustomFanpageProcessor_Should_Execute_Successfully()
        {
            // arrange  
            _fdJobProcess = new CommentLikerProcesss(ProcessScopeModel, DbAccountServiceScoped, FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.FanPageResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var responseHandler = new FanpageScraperResponseHandler
                (responseParameters, new FanpageDetails());

            var queryInfo = new QueryInfo { QueryValue = "" };

            FdRequestLibrary.GetPageIdFromUrl(_fdJobProcess.AccountModel, Arg.Any<string>())
                .Returns("519504621470372");

            FdRequestLibrary.GetFanpageDetails(_fdJobProcess.AccountModel, Arg.Any<FanpageDetails>())
                .Returns(responseHandler);

            _sut = new CustomFanpageProcessor
            (_fdJobProcess, DbAccountServiceScoped, DbCampaignServiceScoped, FdRequestLibrary,
            BrowserManager, ProcessScopeModel);

            // act
            _sut.Start(queryInfo);

            // assert
            FdRequestLibrary.Received(1).GetFanpageDetails(_fdJobProcess.AccountModel, Arg.Any<FanpageDetails>());
            FdRequestLibrary.Received(1).GetPageIdFromUrl(_fdJobProcess.AccountModel, Arg.Any<string>());

            _fdJobProcess = Substitute.For<IFdJobProcess>();
            Container.RegisterInstance(_fdJobProcess);
            var scrapeResultNew = new ScrapeResultNew();
            _fdJobProcess.FinalProcess(scrapeResultNew);
            _fdJobProcess.Received(1).FinalProcess(scrapeResultNew);

        }

        [TestMethod]
        public void CustomFanpageProcessor_Should_Fail_To_Execute()
        {
            var queryInfo = new QueryInfo();
            _fdJobProcess = new CommentLikerProcesss(ProcessScopeModel, DbAccountServiceScoped, FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);

            _sut = new CustomFanpageProcessor
                (_fdJobProcess, DbAccountServiceScoped, DbCampaignServiceScoped, FdRequestLibrary,
                BrowserManager, ProcessScopeModel);

            // act
            _sut.Start(queryInfo);

            // assert

            FdRequestLibrary.Received(1).GetFanpageDetails(_fdJobProcess.AccountModel, Arg.Any<FanpageDetails>());
            FdRequestLibrary.Received(1).GetPageIdFromUrl(_fdJobProcess.AccountModel, Arg.Any<string>());
        }

    }
}
