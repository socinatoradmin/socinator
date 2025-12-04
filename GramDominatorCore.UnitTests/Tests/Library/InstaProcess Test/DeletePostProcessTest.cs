using Dominator.Tests.Utils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using GramDominatorCore.UnitTests.Tests.Library.ProcessorTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NSubstitute;
using System;
using System.Reflection;
using System.Threading;

namespace GramDominatorCore.UnitTests.Tests.Library.InstaProcess_Test
{
    [TestClass]
    public  class DeletePostProcessTest : BaseGdProcessTest
    {
        DeletePostModel _deletePostModel;
        DeletePostProcess _process;
        private ScrapeResultNew _scrapeResultNew;
        private IInstaFunction _funct;
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            ActivityType _activityType = ActivityType.DeletePost;
            GdJobProcess.ActivityType.Returns(_activityType);
            _funct = Substitute.For<IInstaFunction>();
            var module = new ModuleConfiguration();
            JobActivityConfigurationManager[AccountId, _activityType].Returns(module);
            _deletePostModel = new DeletePostModel();
            ProcessScopeModel.GetActivitySettingsAs<DeletePostModel>().Returns(_deletePostModel);
            var jsonData = JsonConvert.SerializeObject(_deletePostModel);
           var model = new TemplateModel() { ActivitySettings = jsonData };
            var templatesFileManager = CommonServiceLocator.ServiceLocator.Current.GetInstance<ITemplatesFileManager>();
            templatesFileManager.GetTemplateById(Arg.Any<string>()).Returns(model);
            ProcessScopeModel.ActivityType.Returns(ActivityType.DeletePost);
            _process = new DeletePostProcess(ProcessScopeModel, AccountServiceScoped, GdQueryScraperFactory, GdHttpHelper, GdBrowserManager, DelayService);
        }

        [TestMethod]
        [Timeout(20000)]
        public void Check_DeletePostProcess()
        {
            //arrange
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultPost = new InstagramPost
                {
                    Pk= "1878286034182173977"
                },
            };
            _funct = GdLogInProcess.InstagramFunctFactory.InstaFunctions;
            ((GdJobProcess)_process).instaFunct = _funct;
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            var deletePageResponse = TestUtils.ReadFileFromResources(
            "GramDominatorCore.UnitTests.TestData.DeleteMedia.json", Assembly.GetExecutingAssembly());
            var deleteMediaIgResponseHandler  = new DeleteMediaIgResponseHandler(new ResponseParameter
            {
                Response = deletePageResponse
            });
            //act
            _funct.DeleteMedia(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<CancellationToken>(), Arg.Any<string>()).Returns(deleteMediaIgResponseHandler);
            _process.PostScrapeProcess(_scrapeResultNew);
            //assert
            _funct.Received(1).DeleteMedia(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<CancellationToken>(), Arg.Any<string>());
        }

        [TestMethod]
        [Timeout(20000)]
       [ExpectedException(typeof(ArgumentException),"Invalid json string.")]
        public void Check_DeletePostProcess_Will_Return_False()  //Need Response for checking this Unittest Method
        {
            //arrange
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultPost = new InstagramPost
                {
                    Pk = "1878286034182173977"
                },
            };
            _funct = GdLogInProcess.InstagramFunctFactory.InstaFunctions;
              ((GdJobProcess)_process).instaFunct = _funct;
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);           
            var deleteMediaIgResponseHandler = new DeleteMediaIgResponseHandler(new ResponseParameter
            {
                Response =null
            });
            //act
            _funct.DeleteMedia(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<CancellationToken>(), Arg.Any<string>()).Returns(deleteMediaIgResponseHandler);
            _process.PostScrapeProcess(_scrapeResultNew);
            //assert
            _funct.Received(1).DeleteMedia(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<CancellationToken>(), Arg.Any<string>());
        }
    }
}
