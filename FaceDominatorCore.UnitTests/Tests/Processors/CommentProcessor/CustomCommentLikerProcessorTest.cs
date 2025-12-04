using System.Reflection;
using Dominator.Tests.Utils;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDLibrary.FdProcessors.CommentProcessor;
using FaceDominatorCore.FDModel.CommonSettings;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using FaceDominatorCore.FDResponse.CommonResponse;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Unity;
using DominatorHouseCore.FileManagers;
using System.Threading;

namespace FaceDominatorCore.UnitTests.Tests.Processors.CommentProcessor
{
    [TestClass]
    public class CustomCommentLikerProcessorTest : BaseFacebookProcessorTest
    {
        private CustomCommentLikerProcessor _sut;
        private IFdJobProcess _fdJobProcess;
        private IDbGlobalService _dbGlobalService;
        private IGlobalInteractionDetails _globalInteractionDetails;
        private ITemplatesFileManager _templatesFileManager;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            _fdJobProcess = Substitute.For<IFdJobProcess>();
            Container.RegisterInstance(_fdJobProcess);
            _dbGlobalService = Substitute.For<IDbGlobalService>();
            _globalInteractionDetails = Substitute.For<IGlobalInteractionDetails>();
            Container.RegisterInstance(_globalInteractionDetails);
            _templatesFileManager = Substitute.For<ITemplatesFileManager>();
            Container.RegisterInstance(_templatesFileManager);
            var moduleSetting = new ModuleSetting();
            _fdJobProcess.ModuleSetting.Returns(moduleSetting);

            ProcessScopeModel.GetActivitySettingsAs<ModuleSetting>().Returns(moduleSetting);
        }

        [TestMethod]
        public void CustomUserChannelProcessor_Should_Execute_Successfully()
        {
            var cancellationTokenSource = new System.Threading.CancellationTokenSource();
            _fdJobProcess.JobCancellationTokenSource.Returns(cancellationTokenSource);

            var commentLikerModule = new CommentLikerModule();
            ProcessScopeModel.GetActivitySettingsAs<CommentLikerModule>().Returns(commentLikerModule);

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostResponse.html", Assembly.GetExecutingAssembly());

            var responseParameter = new ResponseParameter()
            {
                Response = response,
                HasError = false,
                Exception = null
            };

            _fdJobProcess = new CommentLikerProcesss(ProcessScopeModel, DbAccountServiceScoped, FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);

            var postCommentatorResponseHandler = new PostCommentorResponseHandler
                (responseParameter, false, Arg.Any<string>(), Arg.Any<string>(), false);

            var queryInfo = new QueryInfo { QueryValue = "" };
            _fdJobProcess.FinalProcess(new ScrapeResultNew());

            FdRequestLibrary.GetPostCommentor(_fdJobProcess.AccountModel, "", null, Arg.Any<CancellationToken>())
                .Returns(postCommentatorResponseHandler);

            _sut = new CustomCommentLikerProcessor
                (_fdJobProcess, DbAccountServiceScoped, DbCampaignServiceScoped, FdRequestLibrary, BrowserManager, ProcessScopeModel);

            // act
            _sut.Start(queryInfo);

            // assert
            FdRequestLibrary.Received(1).GetPostCommentor(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), null, Arg.Any<CancellationToken>());

            _fdJobProcess = Substitute.For<IFdJobProcess>();
            Container.RegisterInstance(_fdJobProcess);
            var scrapeResultNew = new ScrapeResultNew();
            _fdJobProcess.FinalProcess(scrapeResultNew);
            _fdJobProcess.Received(1).FinalProcess(scrapeResultNew);
        }

        [TestMethod]
        public void CustomCommentLikerProcessor_Should_Fail_To_Execute()
        {
            var queryInfo = new QueryInfo { QueryValue = "" };
            _fdJobProcess = new CommentLikerProcesss(ProcessScopeModel, DbAccountServiceScoped, FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);

            FdRequestLibrary.GetPostCommentor(_fdJobProcess.AccountModel, "", null, Arg.Any<CancellationToken>())
               .Returns((PostCommentorResponseHandler)null);

            _sut = new CustomCommentLikerProcessor
                (_fdJobProcess, DbAccountServiceScoped, DbCampaignServiceScoped, FdRequestLibrary,
                BrowserManager, ProcessScopeModel);

            //act
            _sut.Start(queryInfo);

            // assert
            FdRequestLibrary.Received(1).GetPostCommentor(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), null, Arg.Any<CancellationToken>());
        }

    }
}
