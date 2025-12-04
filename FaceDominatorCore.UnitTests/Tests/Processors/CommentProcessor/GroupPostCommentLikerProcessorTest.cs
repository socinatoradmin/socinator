using System.Reflection;
using System.Threading;
using Dominator.Tests.Utils;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDLibrary.FdProcessors.CommentProcessor;
using FaceDominatorCore.FDModel.CommonSettings;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using FaceDominatorCore.FDResponse.CommonResponse;
using FaceDominatorCore.UnitTests.Tests.FdResponseHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Unity;

namespace FaceDominatorCore.UnitTests.Tests.Processors.CommentProcessor
{
    [TestClass]
    public class GroupPostCommentLikerProcessorTest : BaseFacebookProcessorTest
    {
        private GroupPostCommentLikerProcessor _sut;
        private IFdJobProcess _fdJobProcess;
        private FdResponseHandlerTest _fdResponseHandlerTest;


        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();


            _fdResponseHandlerTest = Substitute.For<FdResponseHandlerTest>();
            Container.RegisterInstance(_fdResponseHandlerTest);
            _fdJobProcess = Substitute.For<IFdJobProcess>();
            Container.RegisterInstance(_fdJobProcess);
            var moduleSetting = new ModuleSetting();

            ProcessScopeModel.GetActivitySettingsAs<ModuleSetting>().Returns(moduleSetting);
        }

        [TestMethod]
        public void GroupPostCommentLikerProcessor_Should_Execute_Successfully()
        {
            // arrange  
            _fdJobProcess = new CommentLikerProcesss(ProcessScopeModel, DbAccountServiceScoped, FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);
            
            var postResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostResponse.html", Assembly.GetExecutingAssembly());

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostListFromGroupsNewResponse.html", Assembly.GetExecutingAssembly());

            var responseParameter = new ResponseParameter
            {
                Response = response,
                Exception = null,
                HasError = false
            };
            var postResponseParameter = new ResponseParameter()
            {
                Response = postResponse,
                Exception = null,
                HasError = false
            };

            var commentLikerModule = new CommentLikerModule();
            ProcessScopeModel.GetActivitySettingsAs<CommentLikerModule>().Returns(commentLikerModule);

            var listOfKeyValuePair =
                new PostReactionListResponseHandler(responseParameter).ListPostReaction;

            var scrapGroupPostListResponseHandler
                = new ScrapGroupPostListResponseHandler
                    (responseParameter, listOfKeyValuePair, Arg.Any<string>());

            var postCommentatorResponseHandler = new PostCommentorResponseHandler
                (postResponseParameter, Arg.Any<bool>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>());

            var queryInfo = new QueryInfo { QueryValue = "" };
            _fdJobProcess.FinalProcess(new ScrapeResultNew());

            FdRequestLibrary.GetPostListFromGroupsNew
                (_fdJobProcess.AccountModel, null, Arg.Any<string>())
                .Returns(scrapGroupPostListResponseHandler);

            FdRequestLibrary.GetPostCommentor(_fdJobProcess.AccountModel, Arg.Any<string>(), null,Arg.Any<CancellationToken>())
                .Returns(postCommentatorResponseHandler);

            _sut = new GroupPostCommentLikerProcessor
            (_fdJobProcess, DbAccountServiceScoped, DbCampaignServiceScoped, FdRequestLibrary,
            BrowserManager, ProcessScopeModel);

            // act
            _sut.Start(queryInfo);

            // assert
            FdRequestLibrary.Received(11).GetPostCommentor(_fdJobProcess.AccountModel, Arg.Any<string>(), null, Arg.Any<CancellationToken>());
            FdRequestLibrary.Received(1).GetPostListFromGroupsNew(_fdJobProcess.AccountModel, null, Arg.Any<string>());

        }

        [TestMethod]
        public void GroupPostCommentLikerProcessor_Should_Failed_To_Execute()
        {
            var queryInfo = new QueryInfo { QueryValue = "" };
            _fdJobProcess = new CommentLikerProcesss(ProcessScopeModel, DbAccountServiceScoped, FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);

            FdRequestLibrary.GetPostListFromGroups
                    (_fdJobProcess.AccountModel, null, Arg.Any<string>())
                .Returns((ScrapGroupPostListResponseHandler)null);

            FdRequestLibrary.GetPostCommentor(_fdJobProcess.AccountModel, Arg.Any<string>(), null,new System.Threading.CancellationToken())
                .Returns((PostCommentorResponseHandler)null);

            _sut = new GroupPostCommentLikerProcessor
                (_fdJobProcess, DbAccountServiceScoped, DbCampaignServiceScoped, FdRequestLibrary,
                BrowserManager, ProcessScopeModel);

            _sut.Start(queryInfo);

            FdRequestLibrary.Received(0).GetPostCommentor(_fdJobProcess.AccountModel, Arg.Any<string>(), null,new System.Threading.CancellationToken());
            FdRequestLibrary.Received(1).GetPostListFromGroupsNew(_fdJobProcess.AccountModel, null, Arg.Any<string>());

        }
    }
}
