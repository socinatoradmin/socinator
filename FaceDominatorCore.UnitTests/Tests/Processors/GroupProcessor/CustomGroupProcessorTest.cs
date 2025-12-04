using CommonServiceLocator;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDLibrary.FdProcessors.GroupProcessor;
using FaceDominatorCore.FDModel.CommonSettings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using DominatorHouseCore.Utility;
using Unity;

namespace FaceDominatorCore.UnitTests.Tests.Processors.GroupProcessor
{
    [TestClass]
    public class CustomGroupProcessorTest : BaseFacebookProcessorTest
    {
        private CustomGroupProcessor _sut;
        private IFdJobProcess _fdJobProcess;


        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            var moduleSetting = new ModuleSetting();
            ProcessScopeModel.GetActivitySettingsAs<ModuleSetting>().Returns(moduleSetting);

        }

        [TestMethod]
        public void CustomGroupProcessor_Should_Execute_Successfully()
        {
            var job = new JobLimitsHolder();
            ProcessScopeModel.GetActivitySettingsAs<JobLimitsHolder>().Returns(job);

            // arrange  
            var queryInfo = new QueryInfo { QueryValue = "" };
            _fdJobProcess = new CommentLikerProcesss(ProcessScopeModel, DbAccountServiceScoped, 
                FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);

            FdRequestLibrary.GetGroupIdFromUrl(Arg.Any<DominatorAccountModel>(), Arg.Any<string>())
                .Returns("589307534524989");

            _sut = new CustomGroupProcessor
            (_fdJobProcess, DbAccountServiceScoped,DbCampaignServiceScoped, FdRequestLibrary,
            BrowserManager, ProcessScopeModel);

            // act
            _sut.Start(queryInfo);

            // assert
            FdRequestLibrary.Received(1).GetGroupIdFromUrl(Arg.Any<DominatorAccountModel>(), Arg.Any<string>());

            _fdJobProcess = Substitute.For<IFdJobProcess>();
            Container.RegisterInstance(_fdJobProcess);
            var scrapeResultNew = new ScrapeResultNew();
            _fdJobProcess.FinalProcess(scrapeResultNew);
            _fdJobProcess.Received(1).FinalProcess(scrapeResultNew);
        }

        [TestMethod]
        public void CustomGroupProcessor_Should_Failed_To_Execute()
        {
            var queryInfo = new QueryInfo();
            _fdJobProcess = new CommentLikerProcesss(ProcessScopeModel, DbAccountServiceScoped, FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);

            _sut = new CustomGroupProcessor
                (_fdJobProcess, DbAccountServiceScoped, DbCampaignServiceScoped, FdRequestLibrary,
                BrowserManager, ProcessScopeModel);

            FdRequestLibrary.GetGroupIdFromUrl(Arg.Any<DominatorAccountModel>(), Arg.Any<string>())
                .Returns((string)null);

            // act
            _sut.Start(queryInfo);

            // assert
            FdRequestLibrary.Received(1).GetGroupIdFromUrl(Arg.Any<DominatorAccountModel>(), Arg.Any<string>());

        }

    }
}
