using System.Reflection;
using Dominator.Tests.Utils;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDLibrary.FdProcessors.FanpageProcessor;
using FaceDominatorCore.FDModel.CommonSettings;
using FaceDominatorCore.FDResponse.ScrapersResponse;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Unity;

namespace FaceDominatorCore.UnitTests.Tests.Processors.FanpageProcessor
{
    [TestClass]
    public class GraphSearchFnapageProcessorTest : BaseFacebookProcessorTest
    {
        private GraphSearchFnapageProcessor _sut;
        private IFdJobProcess _fdJobProcess;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            var moduleSetting = new ModuleSetting();
            ProcessScopeModel.GetActivitySettingsAs<ModuleSetting>().Returns(moduleSetting);

        }

        [TestMethod]
        public void GraphSearchFnapageProcessor_Should_Execute_Successfully()
        {
            // arrange  
            _fdJobProcess = new CommentLikerProcesss(ProcessScopeModel, DbAccountServiceScoped, 
                FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetFanpageDetailsFromGraphSearchResponse.html", Assembly.GetExecutingAssembly());
            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };
            var responseHandler = new SearchFanpageDetailsResponseHandler(responseParameters);

            var queryInfo = new QueryInfo { QueryValue = "" };

            FdRequestLibrary.GetFanpageDetailsFromGraphSearch
                (_fdJobProcess.AccountModel, Arg.Any<string>(), false, false, FanpageCategory.AnyCategory, null)
                .Returns(responseHandler);

            response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.FanPageResponse.html", Assembly.GetExecutingAssembly());
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var pageResponseHandler = new FanpageScraperResponseHandler
                (responseParameters, new FanpageDetails());

            FdRequestLibrary.GetFanpageDetails(_fdJobProcess.AccountModel, Arg.Any<FanpageDetails>(), false)
                .Returns(pageResponseHandler);

            _sut = new GraphSearchFnapageProcessor
            (_fdJobProcess, DbAccountServiceScoped, DbCampaignServiceScoped, FdRequestLibrary,
            BrowserManager, ProcessScopeModel);

            // act
            _sut.Start(queryInfo);

            // assert
            FdRequestLibrary.Received(16).GetFanpageDetails(_fdJobProcess.AccountModel, Arg.Any<FanpageDetails>());
            FdRequestLibrary.Received(1).GetFanpageDetailsFromGraphSearch
                (_fdJobProcess.AccountModel, Arg.Any<string>(), false, false, FanpageCategory.AnyCategory, null);

            _fdJobProcess = Substitute.For<IFdJobProcess>();
            Container.RegisterInstance(_fdJobProcess);
            var scrapeResultNew = new ScrapeResultNew();
            _fdJobProcess.FinalProcess(scrapeResultNew);
            _fdJobProcess.Received(1).FinalProcess(scrapeResultNew);
        }

        [TestMethod]
        public void GraphSearchFnapageProcessor_Should_Fail_To_Execute()
        {
            var queryInfo = new QueryInfo();
            _fdJobProcess = new CommentLikerProcesss(ProcessScopeModel, DbAccountServiceScoped, 
                FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);

            FdRequestLibrary.GetFanpageDetailsFromGraphSearch
                (_fdJobProcess.AccountModel, Arg.Any<string>(), false, false, FanpageCategory.AnyCategory, null)
                .Returns((SearchFanpageDetailsResponseHandler)null);

            FdRequestLibrary.GetFanpageDetails(_fdJobProcess.AccountModel, Arg.Any<FanpageDetails>())
                .Returns((FanpageScraperResponseHandler)null);

            _sut = new GraphSearchFnapageProcessor
            (_fdJobProcess, DbAccountServiceScoped, DbCampaignServiceScoped, FdRequestLibrary,
            BrowserManager, ProcessScopeModel);

            // act
            _sut.Start(queryInfo);

            // assert
            FdRequestLibrary.Received(0).GetFanpageDetails(_fdJobProcess.AccountModel, Arg.Any<FanpageDetails>());
            FdRequestLibrary.Received(1).GetFanpageDetailsFromGraphSearch
                (_fdJobProcess.AccountModel, Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<bool>(), Arg.Any<FanpageCategory>(), null);

        }

    }
}
