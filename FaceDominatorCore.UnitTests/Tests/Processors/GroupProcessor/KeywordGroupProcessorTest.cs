using System.Reflection;
using Dominator.Tests.Utils;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDLibrary.FdProcessors.GroupProcessor;
using FaceDominatorCore.FDModel.CommonSettings;
using FaceDominatorCore.FDResponse.ScrapersResponse;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Unity;

namespace FaceDominatorCore.UnitTests.Tests.Processors.GroupProcessor
{
    [TestClass]
    public class KeywordGroupProcessorTest : BaseFacebookProcessorTest
    {
        private KeywordGroupProcessor _sut;
        private IFdJobProcess _fdJobProcess;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            var moduleSetting = new ModuleSetting();
            ProcessScopeModel.GetActivitySettingsAs<ModuleSetting>().Returns(moduleSetting);
        }

        [TestMethod]
        public void KeywordGroupProcessor_Should_Execute_Successfully()
        {
            // arrange  
            var queryInfo = new QueryInfo { QueryValue = "" };
            _fdJobProcess = new CommentLikerProcesss(ProcessScopeModel, DbAccountServiceScoped, FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.ScrapGroupsResponse.html", Assembly.GetExecutingAssembly());

            var responseParameter = new ResponseParameter()
            {
                Response = response,
                HasError = false,
                Exception = null
            };

            var responseHandler = new GroupScraperResponseHandler(responseParameter, null);

            FdRequestLibrary.ScrapGroups
            (Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<GroupMemberShip>(), Arg.Any<GroupType>(),
                null, Arg.Any<string>()).Returns(responseHandler);
            _sut = new KeywordGroupProcessor
                (_fdJobProcess, DbAccountServiceScoped, DbCampaignServiceScoped, FdRequestLibrary,
                BrowserManager, ProcessScopeModel);

            // act
            _sut.Start(queryInfo);

            // assert
            FdRequestLibrary.Received(1).ScrapGroups
            (Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<GroupMemberShip>(), Arg.Any<GroupType>(),
                null, Arg.Any<string>());

            _fdJobProcess = Substitute.For<IFdJobProcess>();
            Container.RegisterInstance(_fdJobProcess);
            var scrapeResultNew = new ScrapeResultNew();
            _fdJobProcess.FinalProcess(scrapeResultNew);
            _fdJobProcess.Received(1).FinalProcess(scrapeResultNew);
        }

        [TestMethod]
        public void KeywordGroupProcessor_Should_Failed_To_Execute()
        {
            var queryInfo = new QueryInfo();
            _fdJobProcess = new CommentLikerProcesss(ProcessScopeModel, DbAccountServiceScoped, FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);

            _sut = new KeywordGroupProcessor
                (_fdJobProcess, DbAccountServiceScoped, DbCampaignServiceScoped, FdRequestLibrary,
                BrowserManager, ProcessScopeModel);

            FdRequestLibrary.ScrapGroups
            (Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<GroupMemberShip>(), Arg.Any<GroupType>(),
                null, Arg.Any<string>()).Returns((GroupScraperResponseHandler)null);

            // act
            _sut.Start(queryInfo);

            // assert
            FdRequestLibrary.Received(1).ScrapGroups
            (Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<GroupMemberShip>(), Arg.Any<GroupType>(),
                null, Arg.Any<string>());

        }
    }
}
