//using FaceDominatorCore.FDLibrary.FdProcesses;
//using FaceDominatorCore.FDLibrary.FdProcessors.CommentProcessor;
//using FaceDominatorCore.FDModel.CommonSettings;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using NSubstitute;
//using Unity;

//namespace FaceDominatorCore.UnitTests.Tests.Processors.CommentProcessor
//{
//    [TestClass]
//    public class CustomWebPostReCommentLikerProcessorTest : BaseFacebookProcessorTest
//    {
//        private CustomWebPostReCommentLikerProcessor _sut;
//        private IFdJobProcess _fdJobProcess;

//        [TestInitialize]
//        public override void SetUp()
//        {
//            base.SetUp();

//            _fdJobProcess = Substitute.For<IFdJobProcess>();
//            Container.RegisterInstance(_fdJobProcess);
//            var moduleSetting = new ModuleSetting();
//            _fdJobProcess.ModuleSetting.Returns(moduleSetting);

//        }

//        [TestMethod]
//        public void CustomUserChannelProcessor_Should_Execute_Successfully()
//        {
//            //// arrange  
//            //var queryInfo = new QueryInfo
//            //{
//            //    QueryType = string.Empty,
//            //    QueryValue = "href=https://xyz.com?fb_comment_id=7346"
//            //};

//            //var jobcancel = new System.Threading.CancellationTokenSource();
//            //_fdJobProcess.JobCancellationTokenSource.Returns(jobcancel);

//            //var account = new DominatorAccountModel
//            //{
//            //    AccountBaseModel = new DominatorAccountBaseModel
//            //    {
//            //        AccountNetwork = SocialNetworks.Facebook,
//            //        UserName = "Facebook Account Name"
//            //    }
//            //};

//            //_fdJobProcess.DominatorAccountModel.Returns(account);

//            //         var webPostCommentLikerResponseHandler
//            //    = new WebPostCommentLikerResponseHandler(new ResponseParameter(), Arg.Any<string>(), Arg.Any<DominatorAccountModel>());

//            //FdRequestLibrary.GetPostCommentorForWebPage
//            //    (Arg.Any<DominatorAccountModel>(),Arg.Any<string>(), null)
//            //    .Returns(webPostCommentLikerResponseHandler);

//            //_fdJobProcess = new FdReplyToCommentProcess(ProcessScopeModel, DbAccountServiceScoped, GlobalService, ExecutionLimitsManager,
//            //    FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);


//            //_sut = new CustomWebPostReCommentLikerProcessor
//            //(_fdJobProcess, DbAccountServiceScoped, DbCampaignServiceScoped, GlobalService, FdRequestLibrary, ProcessScopeModel);

//            //// act
//            //_sut.Start(queryInfo);

//            //// assert

//        }
//    }
//}
