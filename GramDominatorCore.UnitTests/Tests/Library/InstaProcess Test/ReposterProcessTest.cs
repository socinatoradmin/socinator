using Dominator.Tests.Utils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using GramDominatorCore.UnitTests.Tests.Library.ProcessorTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NSubstitute;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace GramDominatorCore.UnitTests.Tests.Library.InstaProcess_Test
{
    [TestClass]
    public class ReposterProcessTest : BaseGdProcessTest
    {
        RePosterModel reposterModel;
        ReposterProcess process;
        private ScrapeResultNew _scrapeResultNew;
        IInstaFunction instafunct;
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            instafunct = Substitute.For<IInstaFunction>();
            ActivityType _activityType = ActivityType.Reposter;
            GdJobProcess.ActivityType.Returns(_activityType);
            var module = new ModuleConfiguration();
            JobActivityConfigurationManager[AccountId, _activityType].Returns(module);
        }
        private void InitializeProcess(RePosterModel reposterModel)
        {
            ProcessScopeModel.GetActivitySettingsAs<RePosterModel>().Returns(reposterModel);
            var jsonData = JsonConvert.SerializeObject(reposterModel);
            var model = new TemplateModel() { ActivitySettings = jsonData };
            var templatesFileManager = CommonServiceLocator.ServiceLocator.Current.GetInstance<ITemplatesFileManager>();
            templatesFileManager.GetTemplateById(Arg.Any<string>()).Returns(model);
            ProcessScopeModel.ActivityType.Returns(ActivityType.Reposter);
            process = new ReposterProcess(ProcessScopeModel, AccountServiceScoped, GdQueryScraperFactory, GdHttpHelper, GdBrowserManager, DelayService);
        }
        [TestMethod]
        public void Check_ReposterProcess_UploadingImage()
        {
            //arrange
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultPost = new InstagramPost
                {
                    Code = "B8YZfilhV10",
                    Caption="Too love",
                    Id= "2240652933220097396_4749765220",
                    MediaType=MediaType.Image
                },
                QueryInfo = new QueryInfo
                {
                    QueryType = "B8YZfilhV10"
                }
            };
            List<string> imageList = new List<string>();
            var imageStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("GramDominatorCore.UnitTests.RequiredData.B4AEAZHgzVi.jpg");
            var pageresponse = TestUtils.ReadFileFromResources(
        "GramDominatorCore.UnitTests.RequiredData.B8YZfilhV10.jpg", Assembly.GetExecutingAssembly());

            reposterModel = new RePosterModel()
            {
                IsChkPostCaption=false,
                IsChkUserTag=false,
            };
            InitializeProcess(reposterModel);
            instafunct = GdLogInProcess.InstagramFunctFactory.InstaFunctions;
            process.instaFunct = instafunct;
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
            //act
            instafunct.UploadPhotoAlbum(GdDominatorAccountModel,AccountModel,CancellationToken.None, imageList, _scrapeResultNew.ResultPost.Caption,null,null);
            //  instafunct.SendMessage(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns("");
            var jpr = process.PostScrapeProcess(_scrapeResultNew);
            //assert
            instafunct.Received(1).UploadPhotoAlbum(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<CancellationToken>(), Arg.Any<List<string>>(), Arg.Any<string>(), Arg.Any<string>());
        }
    }
}
