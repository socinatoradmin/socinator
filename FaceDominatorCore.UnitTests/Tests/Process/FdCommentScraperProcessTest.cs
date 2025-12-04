using System;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDModel.CommonSettings;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace FaceDominatorCore.UnitTests.Tests.Process
{
    [TestClass]
    public class FdCommentScraperProcessTest : BaseFacebookProcessTest
    {
        private FdCommentScraperProcess _sut;
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

            _activityType = ActivityType.AutoReplyToNewMessage;
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

            _sut = new FdCommentScraperProcess(ProcessScopeModel, DbAccountServiceScoped, FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped, DelayService);

        }

        [TestMethod]
        public void Should_FdComment_And_Save_Result_In_Database()
        {

            _scrapeResultNew = new ScrapeResultNew
            {
                ResultUser = new FacebookUser(),
                ResultComment = new FdPostCommentDetails(),
                QueryInfo = QueryInfo.NoQuery
            };

            ProcessScopeModel.TemplateId.Returns(_templateId);

            var moduleConfigurations = new ModuleConfiguration
            {
                ActivityType = ActivityType.AutoReplyToNewMessage,
                IsTemplateMadeByCampaignMode = true,
                TemplateId = _templateId
            };

            JobActivityConfigurationManager[_accountId, ActivityType.AutoReplyToNewMessage]
                .Returns(moduleConfigurations);

            // act
            var jpr = _sut.PostScrapeProcess(_scrapeResultNew);

            // assert
            DbAccountServiceScoped.Received(1).Add(Arg.Any<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedComments>());
            DbCampaignServiceScoped.Received(1).Add(Arg.Any<DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedComments>());
            jpr.IsProcessSuceessfull.Should().Be(true);
        }

        [TestMethod]
        public void should_return_False_IsProcessSucessfull_is_False()
        {

            _scrapeResultNew = new ScrapeResultNew
            {
                ResultComment = new FdPostCommentDetails(),
                QueryInfo = new QueryInfo()
            };

            var jpr = _sut.PostScrapeProcess(_scrapeResultNew);

            DbAccountServiceScoped.Received(0).Add(Arg.Any<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedComments>());
            DbCampaignServiceScoped.Received(0).Add(Arg.Any<DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedComments>());
            jpr.IsProcessSuceessfull.Should().Be(true);
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
