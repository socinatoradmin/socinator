using System;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDModel.CommonSettings;
using FaceDominatorCore.FDModel.InviterModel;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;


namespace FaceDominatorCore.UnitTests.Tests.Process
{
    [TestClass]
    public class EventInviterProcessTest : BaseFacebookProcessTest
    {
        private EventInviterProcess _sut;
        private ActivityType _activityType;
        private string _userId;
        private string _accountId;
        private string _templateId;
        private ScrapeResultNew _scrapeResultNew;
        private string _note;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            _userId = "User ID";
            _accountId = "ID123";
            _templateId = "T123";
            _note = "Invited";

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
            var eventInviterModel = new EventInviterModel
            {
                InviterOptionsModel = new DominatorHouseCore.Models.FacebookModels.InviterOptions
                {
                    Note = _note,
                    IsSendInvitationInMessanger = true,
                }

            };

            ProcessScopeModel.GetActivitySettingsAs<EventInviterModel>().Returns(eventInviterModel);

            _sut = new EventInviterProcess(ProcessScopeModel, DbAccountServiceScoped, FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);

        }

        [TestMethod]
        public void Should_SendEventInvittationTofriends_And_Save_Result_In_Database()
        {

            _scrapeResultNew = new ScrapeResultNew
            {
                ResultUser = new FacebookUser(),
                ResultEvent = new FdEvents { EventId = "732462387" },
                QueryInfo = new QueryInfo()
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

            FdRequestLibrary.SendEventInvittationTofriends
                (Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<FacebookUser>(), _note).Returns(true);

            // act
            var jpr = _sut.PostScrapeProcess(_scrapeResultNew);

            // assert
            FdRequestLibrary.Received(1).SendEventInvittationTofriends
                (Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<FacebookUser>(), _note);
            DbAccountServiceScoped.Received(1).Add((Arg.Any<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedUsers>()));
            DbCampaignServiceScoped.Received(1).Add(Arg.Any<DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedUsers>());
            jpr.IsProcessSuceessfull.Should().Be(true);
        }


        [TestMethod]
        public void should_return_False_IsProcessSucessfull_is_False()
        {
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultUser = new FacebookUser(),
                ResultEvent = new FdEvents(),
                QueryInfo = new QueryInfo()
            };
            FdRequestLibrary.SendEventInvittationTofriends
                (Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<FacebookUser>(), _note).Returns(false);

            var jpr = _sut.PostScrapeProcess(_scrapeResultNew);

            // assert
            FdRequestLibrary.Received(1).SendEventInvittationTofriends
                (Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<FacebookUser>(), _note);
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
