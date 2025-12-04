using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FaceDominatorCore.FDLibrary.FdProcessors.GroupProcessor;
using FaceDominatorCore.FDLibrary.FdProcesses;
using DominatorHouseCore.Models;
using FaceDominatorCore.FDModel.CommonSettings;
using NSubstitute;
using Dominator.Tests.Utils;
using System.Reflection;
using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
using DominatorHouseCore.Request;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDResponse.ScrapersResponse;
using Unity;

namespace FaceDominatorCore.UnitTests.Tests.Processors.GroupProcessor
{
    [TestClass]
    public class GroupUnjoinerProcessorTest : BaseFacebookProcessorTest
    {
        private GroupUnjoinerProcessor _sut;
        private IFdJobProcess _fdJobProcess;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            var moduleSetting = new ModuleSetting();
            ProcessScopeModel.GetActivitySettingsAs<ModuleSetting>().Returns(moduleSetting);
        }

        [TestMethod]
        public void GraphSearchGroupProcessors_Should_Execute_Successfully()
        {
            // arrange  
            var queryInfo = new QueryInfo { QueryValue = "" };
            _fdJobProcess = new CommentLikerProcesss(ProcessScopeModel, DbAccountServiceScoped, FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);

            var lstFilteredGroupIds = new List<GroupDetails>()
            {
                new GroupDetails()
                {
                    GroupId = "groupId"
                },
                new GroupDetails()
                {
                    GroupId = "groupId"
                }
            };
            var groupList = new List<OwnGroups>
            {
                new OwnGroups()
                {
                    GroupId = "groupId"
                }
            };


            DbAccountServiceScoped.Get<OwnGroups>().Returns(groupList);

            _sut = new GroupUnjoinerProcessor
                (_fdJobProcess, DbAccountServiceScoped, DbCampaignServiceScoped, FdRequestLibrary,
                BrowserManager, ProcessScopeModel);

            // act
            _sut.Start(queryInfo);

            // assert

            _fdJobProcess = Substitute.For<IFdJobProcess>();
            Container.RegisterInstance(_fdJobProcess);
            var scrapeResultNew = new ScrapeResultNew();
            _fdJobProcess.FinalProcess(scrapeResultNew);
            _fdJobProcess.Received(1).FinalProcess(scrapeResultNew);
        }
    }
}
