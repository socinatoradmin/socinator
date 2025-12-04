using System;
using Dominator.Tests.Utils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDModel.CommonSettings;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using FaceDominatorCore.FDResponse.LikeCommentsResponse;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Reflection;
using DominatorHouse.ThreadUtils;

namespace FaceDominatorCore.UnitTests.Tests.Process
{
    [TestClass]
    public class FdReplyToCommentProcessTest : BaseFacebookProcessTest
    {

        private FdReplyToCommentProcess _sut;
        private ActivityType _activityType;
        private string _userId;
        private string _accountId;
        private string _templateId;
        private ScrapeResultNew _scrapeResultNew;
        private string _commentId;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            _userId = "User ID";
            _accountId = "ID123";
            _templateId = "T123";
            _commentId = "23566";

            _activityType = ActivityType.ReplyToComment;
            ProcessScopeModel = Substitute.For<IProcessScopeModel>();
            ProcessScopeModel.Account.Returns(new DominatorAccountModel
            {
                AccountId = _accountId,
                AccountBaseModel = new DominatorAccountBaseModel
                {
                    AccountNetwork = SocialNetworks.Facebook,
                }
            });

            ProcessScopeModel.ActivityType.Returns(_activityType);
            var moduleSetting = new ModuleSetting();
            ProcessScopeModel.GetActivitySettingsAs<ModuleSetting>().Returns(moduleSetting);

            var replyToCommentModel = new ReplyToCommentModel()
            {
                IsMentionUsersChecked = false,
            };
            ProcessScopeModel.GetActivitySettingsAs<ReplyToCommentModel>().Returns(replyToCommentModel);


            _sut = new FdReplyToCommentProcess(ProcessScopeModel, DbAccountServiceScoped, FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary,DbCampaignServiceScoped);

        }

        [TestMethod]
        public void Should_ReplyOnPost_And_Save_Result_In_Database()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.CommentOnPostResponse.html", Assembly.GetExecutingAssembly());

            var responseParameter = new ResponseParameter()
            {
                Response = response,
                Exception = null,
                HasError = false
            };

            var fdPostCommentDetails = new FdPostCommentDetails()
            {
                CommenterID = _commentId
            };
            ProcessScopeModel.GetActivitySettingsAs<FdPostCommentDetails>().Returns(fdPostCommentDetails);
           
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultComment = new FdPostCommentDetails { CommenterID = _commentId },
                ResultUser = new FacebookUser(),
                QueryInfo = QueryInfo.NoQuery
            };

            ProcessScopeModel.TemplateId.Returns(_templateId);


            var moduleConfigurations = new ModuleConfiguration
            {
                ActivityType = ActivityType.ReplyToComment,
                IsTemplateMadeByCampaignMode = true,
                TemplateId = _templateId
            };

            JobActivityConfigurationManager[_accountId, ActivityType.ReplyToComment]
                .Returns(moduleConfigurations);

            CommentOnPostResponseHandler commentOnPostResponseHandler
                = new CommentOnPostResponseHandler(responseParameter);

            FdRequestLibrary.ReplyOnPost
                (Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(commentOnPostResponseHandler);

            // act
            var jpr = _sut.PostScrapeProcess(_scrapeResultNew);

            // assert
            FdRequestLibrary.Received(1).ReplyOnPost
                (Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
            DbAccountServiceScoped.Received(1).Add((Arg.Any<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedComments>()));
            DbCampaignServiceScoped.Received(1).Add(Arg.Any<DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedComments>());
            jpr.IsProcessSuceessfull.Should().Be(true);
        }

        [TestMethod]
        public void should_return_False_IsProcessSucessfull_is_False()
        {
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultComment = new FdPostCommentDetails { CommenterID = _commentId },
                ResultUser = new FacebookUser(),
                QueryInfo = QueryInfo.NoQuery
            };

            FdRequestLibrary.ReplyOnPost
                    (Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns((CommentOnPostResponseHandler) null);

            var jpr = _sut.PostScrapeProcess(_scrapeResultNew);

            jpr.IsProcessSuceessfull.Should().Be(false);
            FdRequestLibrary.Received(1).ReplyOnPost
                (Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void should_throw_null_ref_exception_if_scrapeResultNew_is_null()
        {
            var jpr = _sut.PostScrapeProcess(_scrapeResultNew);

            jpr.IsProcessSuceessfull.Should().Be(false);
        }

    }
}
