using System.Collections.Generic;
using System.Reflection;
using System.Threading;
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
using FaceDominatorCore.FDResponse.AccountsResponse;
using FaceDominatorCore.FDResponse.ScrapersResponse;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Unity;

namespace FaceDominatorCore.UnitTests.Tests.Processors.UserProecessor
{
    [TestClass]
    public class UserFrinedsProcessorTest : BaseFacebookProcessorTest
    {
        private UserFrinedsProcessor _sut;
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
        public void UserFrinedsProcessor_Should_Execute_Successfully()
        {
      //      var cancellationTokenSource = new System.Threading.CancellationTokenSource();
      //      _fdJobProcess.JobCancellationTokenSource.Returns(cancellationTokenSource);

      //      var response = TestUtils.ReadFileFromResources
      //("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetFanpageDetailsFromKeywordResponse.html", Assembly.GetExecutingAssembly());

      //      var responseParameter = new ResponseParameter()
      //      {
      //          Response = response,
      //          HasError = false,
      //          Exception = null
      //      };

      //      var responseHandler = new SearchFanpageDetailsResponseHandler(responseParameter);

      //      FdRequestLibrary.UpdateLikedPagesSync(Arg.Any<DominatorAccountModel>(), null,
      //          Arg.Any<CancellationToken>()).Returns(responseHandler);

      //      response = TestUtils.ReadFileFromResources
      //          ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.UpdateFriendsFromPageResponse.html", Assembly.GetExecutingAssembly());

      //      responseParameter = new ResponseParameter
      //      {
      //          Response = response,
      //          HasError = false,
      //          Exception = null
      //      };

      //      var frdResponseHandler = new FriendsUpdateResponseHandler(responseParameter, Arg.Any<bool>());

      //      _fdJobProcess = new CommentLikerProcesss(ProcessScopeModel, DbAccountServiceScoped, GlobalService, ExecutionLimitsManager,
      //          FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);

      //      var queryInfo = new QueryInfo { QueryValue = "" };
      //      _fdJobProcess.FinalProcess(new ScrapeResultNew());

      //      FdRequestLibrary.UpdateFriendsFromPageSync(Arg.Any<DominatorAccountModel>(), null,
      //          Arg.Any<CancellationToken>(), Arg.Any<List<string>>()).Returns(frdResponseHandler);


      //      _sut = new UserFrinedsProcessor
      //          (_fdJobProcess, DbAccountServiceScoped, DbCampaignServiceScoped, GlobalService, FdRequestLibrary, ProcessScopeModel);

      //      // act
      //      _sut.Start(queryInfo);

      //      // assert
      //      FdRequestLibrary.Received(1).UpdateFriendsFromPageSync(Arg.Any<DominatorAccountModel>(), null,
      //          Arg.Any<CancellationToken>(), Arg.Any<List<string>>());

      //      _fdJobProcess = Substitute.For<IFdJobProcess>();
      //      Container.RegisterInstance(_fdJobProcess);
      //      var scrapeResultNew = new ScrapeResultNew();
      //      _fdJobProcess.FinalProcess(scrapeResultNew);
      //      _fdJobProcess.Received(1).FinalProcess(scrapeResultNew);
        }

     
    }
}
