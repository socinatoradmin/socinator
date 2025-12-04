using Dominator.Tests.Utils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.GdQuery;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using GramDominatorCore.Response;
using GramDominatorCore.UnitTests.Test_Data.Library;
using GramDominatorCore.UnitTests.Tests.Library.ProcessorTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NSubstitute;
using System.Reflection;
using System.Threading;

namespace GramDominatorCore.UnitTests.Tests.Library.InstaProcess_Test
{
    [TestClass]
    public class LikeCommentProcessTest : BaseGdProcessTest
    {
        LikeCommentModel _likeCommentModel;
        LikeCommentProcess _process;
        private ScrapeResultNew _scrapeResultNew;
        IInstaFunction _instagramFunct;
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            var _activityType = ActivityType.LikeComment;
            GdJobProcess.ActivityType.Returns(_activityType);

            _instagramFunct = Substitute.For<IInstaFunction>();
            var module = new ModuleConfiguration();
            JobActivityConfigurationManager[AccountId, _activityType].Returns(module);
            _likeCommentModel = new LikeCommentModel();
            ProcessScopeModel.GetActivitySettingsAs<LikeCommentModel>().Returns(_likeCommentModel);
            var jsonData = JsonConvert.SerializeObject(_likeCommentModel);
            var model = new TemplateModel() { ActivitySettings = jsonData };
            var templatesFileManager = CommonServiceLocator.ServiceLocator.Current.GetInstance<ITemplatesFileManager>();
            templatesFileManager.GetTemplateById(Arg.Any<string>()).Returns(model);
            ProcessScopeModel.ActivityType.Returns(ActivityType.LikeComment);
            _process = new LikeCommentProcess(ProcessScopeModel, AccountServiceScoped, GdQueryScraperFactory, GdHttpHelper, GdBrowserManager, DelayService);
        }

        [TestMethod]
        public void Check_LikeCommentProcess()
        {
            //arrange
            _scrapeResultNew = new ScrapeResultNew
            {
                
                    ResultPost = new InstagramPost
                    {
                        Code = "BvG6NmxHFAt",
                        MediaType = MediaType.Image,
                        User = new InstagramUser()
                        {
                            Username = "jacksmith729",
                        }
                    },
                QueryInfo = new QueryInfo()
                {
                    QueryTypeDisplayName = "Custom Photos",
                    QueryType = "Custom Photos",
                    QueryValue = "BvG6NmxHFAt",
                },
                ResultPostComment = new ResultCommentItemUser()
                    {
                        CommentId = "17917605616263640",
                        ContentType = "Comment",
                        Text = "My Crush",
                        UserId = "",
                        ItemUser = new InstagramUser()
                        {
                            Pk = "",
                            Username = "jacksmith"
                        }
                    }
                    
                };
            ((GdJobProcess)_process).instaFunct = _instagramFunct;
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);

            var pageResponse = TestUtils.ReadFileFromResources(
          "GramDominatorCore.UnitTests.TestData.LikeOnCommentResponse.json", Assembly.GetExecutingAssembly());
            var likeOnCommentResponse = new CommonIgResponseHandler(new ResponseParameter
            {
                Response = pageResponse
            });
            //act
            _instagramFunct.LikePostComment(Arg.Any<string>(),Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<CancellationToken>()).Returns(likeOnCommentResponse);
            _process.PostScrapeProcess(_scrapeResultNew);
            //assert
            _instagramFunct.Received(1).LikePostComment(Arg.Any<string>(),Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<CancellationToken>());
        }
    }
}
