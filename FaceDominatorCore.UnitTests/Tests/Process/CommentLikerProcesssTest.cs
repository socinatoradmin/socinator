using System;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDModel.CommonSettings;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using DominatorHouseCore.Enums.FdQuery;
using FaceDominatorCore.FDResponse.LikeCommentsResponse;
using DominatorHouseCore.Request;

namespace FaceDominatorCore.UnitTests.Tests.Process
{
    [TestClass]
    public class CommentLikerProcesssTest : BaseFacebookProcessTest
    {
        private CommentLikerProcesss _sut;
        private ActivityType _activityType;
        private string _accountId;
        private string _templateId;
        private ScrapeResultNew _scrapeResultNew;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            _accountId = "ID123";
            _templateId = "T123";

            _activityType = ActivityType.LikeComment;
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


            _sut = new CommentLikerProcesss(ProcessScopeModel, DbAccountServiceScoped, FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);

        }

        [TestMethod]
        public void Should_LikeComments_And_Save_Result_In_Database()
        {

            var fdPostCommentDetails = new FdPostCommentDetails();
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultComment = fdPostCommentDetails,
                QueryInfo = QueryInfo.NoQuery
            };

            ProcessScopeModel.TemplateId.Returns(_templateId);


            var moduleConfigurations = new ModuleConfiguration
            {
                ActivityType = ActivityType.LikeComment,
                IsTemplateMadeByCampaignMode = true,
                TemplateId = _templateId
            };

            JobActivityConfigurationManager[_accountId, ActivityType.LikeComment]
                .Returns(moduleConfigurations);

            var response = new CommentLikerResponseHandler(new ResponseParameter() { Response = "aria-pressed=\"true\"" });

            FdRequestLibrary.LikeComments
                (Arg.Any<DominatorAccountModel>(), Arg.Any<FdPostCommentDetails>(), ReactionType.Like)
                .Returns(response);

            // act
            var jpr = _sut.PostScrapeProcess(_scrapeResultNew);

            // assert
            FdRequestLibrary.Received(1).LikeComments(Arg.Any<DominatorAccountModel>(), Arg.Any<FdPostCommentDetails>(), ReactionType.Like);
            DbAccountServiceScoped.Received(1).Add(Arg.Any<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedComments>());
            DbCampaignServiceScoped.Received(1).Add(Arg.Any<DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedComments>());
            jpr.IsProcessSuceessfull.Should().Be(true);
        }

        [TestMethod]
        public void should_return_False_IsProcessSucessfull_is_False()
        {
            var fdPostCommentDetails = new FdPostCommentDetails();
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultComment = fdPostCommentDetails,
                ResultUser = new FacebookUser(),
                QueryInfo = new QueryInfo()
            };

            var response = new CommentLikerResponseHandler(new ResponseParameter() { Response = string.Empty });

            FdRequestLibrary.LikeComments
                (Arg.Any<DominatorAccountModel>(), Arg.Any<FdPostCommentDetails>(), ReactionType.Like).Returns(response);

            var jpr = _sut.PostScrapeProcess(_scrapeResultNew);

            // assert
            FdRequestLibrary.Received(1).LikeComments
                (Arg.Any<DominatorAccountModel>(), Arg.Any<FdPostCommentDetails>(), ReactionType.Like);
            jpr.IsProcessSuceessfull.Should().Be(false);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void should_throw_null_ref_exception_if_CommentDetails_is_null()
        {

            _scrapeResultNew = new ScrapeResultNew
            {
                ResultUser = new FacebookUser(),
                QueryInfo = new QueryInfo()
            };
            var jpr = _sut.PostScrapeProcess(_scrapeResultNew);

            // assert
            jpr.IsProcessSuceessfull.Should().Be(false);
        }


        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void should_throw_null_ref_exception_if_scrapeResultNew_is_null()
        {
            FdRequestLibrary.CancelSentRequest(Arg.Any<DominatorAccountModel>(), Arg.Any<string>()).Returns(false);
            var jpr = _sut.PostScrapeProcess(_scrapeResultNew);

            jpr.IsProcessSuceessfull.Should().Be(false);
        }
    }
}
