//using DominatorHouseCore.Enums;
//using DominatorHouseCore.Interfaces;
//using DominatorHouseCore.Models;
//using FaceDominatorCore.FDLibrary.DAL;
//using FaceDominatorCore.FDLibrary.FdProcesses;
//using FaceDominatorCore.FDLibrary.FdProcessors.CommentProcessor;
//using FaceDominatorCore.FDModel.CommonSettings;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using NSubstitute;
//using Unity;

//namespace FaceDominatorCore.UnitTests.Tests.Processors.CommentProcessor
//{
//    [TestClass]
//    public class CustomWebPostCommentLikerProcessorTest : BaseFacebookProcessorTest
//    {
//        private CustomWebPostCommentLikerProcessor _sut;
//        private IFdJobProcess _fdJobProcess;
//        private IDbGlobalService _dbGlobalService;
//        private IGlobalInteractionDetails _globalInteractionDetails;

//        [TestInitialize]
//        public override void SetUp()
//        {
//            base.SetUp();

//            _fdJobProcess = Substitute.For<IFdJobProcess>();
//            Container.RegisterInstance(_fdJobProcess);
//            _dbGlobalService = Substitute.For<IDbGlobalService>();
//            _globalInteractionDetails = Substitute.For<IGlobalInteractionDetails>();
//            Container.RegisterInstance(_globalInteractionDetails);
//            var moduleSetting = new ModuleSetting();
//            _fdJobProcess.ModuleSetting.Returns(moduleSetting);

//        }

//        [TestMethod]
//        public void CustomWebPostCommentLikerProcessor_Should_Execute_Successfully()
//        {
//            // arrange  
//            var queryInfo = new QueryInfo
//            {
//                QueryType = string.Empty,
//                QueryValue = "href=https://xyz.com?fb_comment_id=7346"
//            };

//            var jobcancel = new System.Threading.CancellationTokenSource();
//            _fdJobProcess.JobCancellationTokenSource.Returns(jobcancel);

//            var account = new DominatorAccountModel
//            {
//                AccountBaseModel = new DominatorAccountBaseModel
//                {
//                    AccountNetwork = SocialNetworks.Facebook,
//                    UserName = "Facebook Account Name"
//                }
//            };

//            var moduleSetting = new ModuleSetting();
//            _fdJobProcess.ModuleSetting.Returns(moduleSetting);
//            _fdJobProcess.DominatorAccountModel.Returns(account);

//            _fdJobProcess = new CommentLikerProcesss(ProcessScopeModel, DbAccountServiceScoped, GlobalService, ExecutionLimitsManager,
//                FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);

//            _sut = new CustomWebPostCommentLikerProcessor
//            (_fdJobProcess, DbAccountServiceScoped, DbCampaignServiceScoped, _dbGlobalService, FdRequestLibrary,
//            BrowserManager, null, ProcessScopeModel);

//            // act
//            _sut.Start(queryInfo);

//        }


//    }
//}
