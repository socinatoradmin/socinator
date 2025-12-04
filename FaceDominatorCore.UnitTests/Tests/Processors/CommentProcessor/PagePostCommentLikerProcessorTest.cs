using System.Reflection;
using Dominator.Tests.Utils;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDLibrary.FdProcessors.CommentProcessor;
using FaceDominatorCore.FDModel.CommonSettings;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using FaceDominatorCore.FDResponse.CommonResponse;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Unity;
using FaceDominatorCore.FDLibrary.FdFunctions;
using System.Threading;

namespace FaceDominatorCore.UnitTests.Tests.Processors.CommentProcessor
{
    [TestClass]
    public class PagePostCommentLikerProcessorTest : BaseFacebookProcessorTest
    {

        private PagePostCommentLikerProcessor _sut;
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
        public void PagePostCommentLikerProcessor_Should_Execute_Successfully()
        {
            // arrange  
            var queryInfo = new QueryInfo { QueryValue = "" };

            _fdJobProcess = new CommentLikerProcesss(ProcessScopeModel, DbAccountServiceScoped, FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);

            var commentLikerModule = new CommentLikerModule();
            ProcessScopeModel.GetActivitySettingsAs<CommentLikerModule>().Returns(commentLikerModule);

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostListFromFanpagesResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var listPostReaction = new PostReactionListResponseHandler(responseParameters).ListPostReaction;

            var responseHandler = new ScrapPostListFromFanpageResponseHandler(responseParameters, listPostReaction);

            FdRequestLibrary.GetPostListFromFanpages(_fdJobProcess.AccountModel, Arg.Any<string>(), null)
               .Returns(responseHandler);

            response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostResponse.html", Assembly.GetExecutingAssembly());

            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var postCommentorResponseHandler = new PostCommentorResponseHandler(responseParameters, false,
                                      string.Empty, string.Empty, false);

            FdRequestLibrary.GetPostCommentor(_fdJobProcess.AccountModel, Arg.Any<string>(), null, Arg.Any<CancellationToken>())
                .Returns(postCommentorResponseHandler);

            _sut = new PagePostCommentLikerProcessor
            (_fdJobProcess, DbAccountServiceScoped, DbCampaignServiceScoped, FdRequestLibrary,
                BrowserManager, ProcessScopeModel);

            // act
            _sut.Start(queryInfo);

            // assert
            FdRequestLibrary.Received(1).GetPostListFromFanpages
                (_fdJobProcess.AccountModel, Arg.Any<string>(), null);
            FdRequestLibrary.Received(1).GetPostCommentor
                (_fdJobProcess.AccountModel, Arg.Any<string>(), null, Arg.Any<CancellationToken>());

            _fdJobProcess = Substitute.For<IFdJobProcess>();
            Container.RegisterInstance(_fdJobProcess);
            var scrapeResultNew = new ScrapeResultNew();
            _fdJobProcess.FinalProcess(scrapeResultNew);
            _fdJobProcess.Received(1).FinalProcess(scrapeResultNew);
        }

        [TestMethod]
        public void PagePostCommentLikerProcessor_Should_Fail_To_Execute()
        {
            var queryInfo = new QueryInfo { QueryValue = "" };
            _fdJobProcess = new CommentLikerProcesss(ProcessScopeModel, DbAccountServiceScoped, FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);

            FdRequestLibrary.GetPostListFromFanpages(_fdJobProcess.AccountModel, Arg.Any<string>(), null)
                .Returns((ScrapPostListFromFanpageResponseHandler)null);

            FdRequestLibrary.GetPostCommentor
                (_fdJobProcess.AccountModel, Arg.Any<string>(), null, Arg.Any<CancellationToken>())
                .Returns((PostCommentorResponseHandler)null);

            _sut = new PagePostCommentLikerProcessor
                (_fdJobProcess, DbAccountServiceScoped, DbCampaignServiceScoped, FdRequestLibrary
                , BrowserManager, ProcessScopeModel);

            _sut.Start(queryInfo);

            FdRequestLibrary.Received(1).GetPostListFromFanpages
                (_fdJobProcess.AccountModel, Arg.Any<string>(), null);
            FdRequestLibrary.Received(0).GetPostCommentor
                (_fdJobProcess.AccountModel, Arg.Any<string>(), null, Arg.Any<CancellationToken>());
        }

    }
}
