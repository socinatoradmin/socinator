using System.Collections.Generic;
using System.Reflection;
using Dominator.Tests.Utils;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDLibrary.FdProcessors.CommentProcessor;
using FaceDominatorCore.FDModel.CommonSettings;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using FaceDominatorCore.FDResponse.LikeCommentsResponse;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Unity;

namespace FaceDominatorCore.UnitTests.Tests.Processors.CommentProcessor
{
    [TestClass]
    public class NewsfeedPostCommentProcessorTest : BaseFacebookProcessorTest
    {
        private NewsfeedPostCommentProcessor _sut;
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
        public void GroupPostCommentLikerProcessor_Should_Execute_Successfully()
        {
            // arrange  

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostListFromGroupsNewResponse.html", Assembly.GetExecutingAssembly());

            IResponseParameter responseParameter = new ResponseParameter
            {
                Response = response,
                Exception = null,
                HasError = false
            };

            var commentLikerModule = new CommentLikerModule();
            ProcessScopeModel.GetActivitySettingsAs<CommentLikerModule>().Returns(commentLikerModule);

            _fdJobProcess = new CommentLikerProcesss(ProcessScopeModel, DbAccountServiceScoped, FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);

            var listOfKeyValuePair = new List<KeyValuePair<string, string>>();
            KeyValuePair<string, string> keyValuePair = new KeyValuePair<string, string>("1651012428259980", "752<:>2723<:>95");
            listOfKeyValuePair.Add(keyValuePair);

            keyValuePair = new KeyValuePair<string, string>("2515048618523019", "0<:>7<:>1");
            listOfKeyValuePair.Add(keyValuePair);

            keyValuePair = new KeyValuePair<string, string>("2514729028554978", "7<:>46<:>12");
            listOfKeyValuePair.Add(keyValuePair);

            keyValuePair = new KeyValuePair<string, string>("2514182385276309", "13<:>134<:>31");
            listOfKeyValuePair.Add(keyValuePair);

            var responseHandler
                = new ScrapPostListFromNewsFeedResponseHandlerNew
                    (responseParameter, null, listOfKeyValuePair, FdHttpHelper)
                {
                    Status = true
                };

            var queryInfo = new QueryInfo { QueryValue = "" };

            FdRequestLibrary.GetPostListFromNewsFeed(_fdJobProcess.AccountModel, null)
                .Returns(responseHandler);

            _sut = new NewsfeedPostCommentProcessor
            (_fdJobProcess, DbAccountServiceScoped, DbCampaignServiceScoped, FdRequestLibrary,
               BrowserManager,  ProcessScopeModel);

            // act
            _sut.Start(queryInfo);

            // assert
            FdRequestLibrary.Received(1).GetPostListFromNewsFeed(_fdJobProcess.AccountModel, null);
            _fdJobProcess = Substitute.For<IFdJobProcess>();
            Container.RegisterInstance(_fdJobProcess);
            var scrapeResultNew = new ScrapeResultNew();
            _fdJobProcess.FinalProcess(scrapeResultNew);
            _fdJobProcess.Received(1).FinalProcess(scrapeResultNew);
        }

        [TestMethod]
        public void GroupPostCommentLikerProcessor_Should_Failed_To_Execute()
        {
            var queryInfo = new QueryInfo { QueryValue = "" };
            _fdJobProcess = new CommentLikerProcesss(ProcessScopeModel, DbAccountServiceScoped, FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);

            FdRequestLibrary.GetPostListFromNewsFeed(_fdJobProcess.AccountModel, null)
                .Returns((ScrapPostListFromNewsFeedResponseHandlerNew)null);

            _sut = new NewsfeedPostCommentProcessor
                  (_fdJobProcess, DbAccountServiceScoped, DbCampaignServiceScoped, FdRequestLibrary, BrowserManager, ProcessScopeModel);

            _sut.Start(queryInfo);

            FdRequestLibrary.Received(1).GetPostListFromNewsFeed(_fdJobProcess.AccountModel, null);
        }

    }
}
