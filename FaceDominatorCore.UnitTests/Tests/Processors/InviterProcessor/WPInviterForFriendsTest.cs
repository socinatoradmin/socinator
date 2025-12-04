using System.Collections.Generic;
using System.Reflection;
using Dominator.Tests.Utils;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Request;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDModel.CommonSettings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using FaceDominatorCore.FDLibrary.FdProcessors.InviterProcessor;
using FaceDominatorCore.FDResponse.CommonResponse;
using Unity;
using FaceDominatorCore.FDLibrary.FdClassLibrary;

namespace FaceDominatorCore.UnitTests.Tests.Processors.InviterProcessor
{
    [TestClass]
    public class WpInviterForFriendsTest : BaseFacebookProcessorTest
    {
        private WpInviterForFriends _sut;
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
        }

        [TestMethod]
        public void GraphSearchGroupProcessors_Should_Execute_Successfully()
        {
            // arrange  
            var queryInfo = new QueryInfo { QueryValue = "" };
            _fdJobProcess = new CommentLikerProcesss(ProcessScopeModel, DbAccountServiceScoped, FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostDetailNewResponse.html", Assembly.GetExecutingAssembly());

            var responseParameter = new ResponseParameter()
            {
                Response = response,
                HasError = false,
                Exception = null
            };

            var responseHandler = new PostScraperResponseHandler(responseParameter, null);

            var facebookPostDetails = new FacebookPostDetails
            {
                Id = "10205581200565265",
            };

            FdRequestLibrary.GetPostDetailNew
                (Arg.Any<DominatorAccountModel>(), facebookPostDetails).Returns(responseHandler);

            _sut = new WpInviterForFriends
                (_fdJobProcess, DbAccountServiceScoped, DbCampaignServiceScoped,  FdRequestLibrary,
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
