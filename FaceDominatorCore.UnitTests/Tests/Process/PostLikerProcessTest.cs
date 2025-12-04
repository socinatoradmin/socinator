using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDModel.CommonSettings;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;

namespace FaceDominatorCore.UnitTests.Tests.Process
{
    [TestClass]
    public class PostLikerProcessTest : BaseFacebookProcessTest
    {
        private PostLikerProcess _sut;
        private ActivityType _activityType;
        private string _userId;
        private string _accountId;
        private string _templateId;
        private ScrapeResultNew _scrapeResultNew;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            _userId = "User ID";
            _accountId = "ID123";
            _templateId = "T123";

            _activityType = ActivityType.PostCommentor;
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
            PostLikerModel postLikeModel = new PostLikerModel();
            ProcessScopeModel.GetActivitySettingsAs<PostLikerModel>().Returns(postLikeModel);

            _sut = new PostLikerProcess(ProcessScopeModel, DbAccountServiceScoped, FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped, DelayService);

        }

        [TestMethod]
        public void Should_LikeUnlikePost_And_Save_Result_In_Database()
        {

            var facebookUser = new FacebookUser()
            {
                UserId = _userId,
            };

            var facebookPostDetails = new FacebookPostDetails();

            _scrapeResultNew = new ScrapeResultNew
            {
                ResultUser = facebookUser,
                ResultPost = facebookPostDetails,
                QueryInfo = QueryInfo.NoQuery
            };

            ProcessScopeModel.TemplateId.Returns(_templateId);

            var moduleConfigurations = new ModuleConfiguration
            {
                ActivityType = _activityType,
                IsTemplateMadeByCampaignMode = true,
                TemplateId = _templateId
            };

            JobActivityConfigurationManager[_accountId, _activityType]
                .Returns(moduleConfigurations);

            FdRequestLibrary.LikeUnlikePost
                (Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), ReactionType.Like).Returns(true);

            // act
            var jpr = _sut.PostScrapeProcess(_scrapeResultNew);

            // assert
            FdRequestLibrary.Received(1).LikeUnlikePost
                (Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), ReactionType.Like);
            DbAccountServiceScoped.Received(1).Add((Arg.Any<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedPosts>()));
            DbCampaignServiceScoped.Received(1).Add(Arg.Any<DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedPosts>());
            jpr.IsProcessSuceessfull.Should().Be(true);
        }

        [TestMethod]
        public void should_return_False_IsProcessSucessfull_is_False()
        {
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultPost = new FacebookPostDetails(),
                ResultUser = new FacebookUser(),
                QueryInfo = new QueryInfo()
            };

            FdRequestLibrary.LikeUnlikePost
                (Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), ReactionType.Like).Returns(false);

            var jpr = _sut.PostScrapeProcess(_scrapeResultNew);

            FdRequestLibrary.Received(1).LikeUnlikePost(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), ReactionType.Like);
            jpr.IsProcessSuceessfull.Should().Be(false);
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
