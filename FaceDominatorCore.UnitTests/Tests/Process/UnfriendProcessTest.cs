using System;
using Dominator.Tests.Utils;
using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDModel.CommonSettings;
using FaceDominatorCore.FDResponse.FriendsResponse;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Reflection;

namespace FaceDominatorCore.UnitTests.Tests.Process
{
    [TestClass]
    public class UnfriendProcessTest:BaseFacebookProcessTest
    {
        private UnfriendProcess _sut;
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

            Friends friends = new Friends();
            ProcessScopeModel.GetActivitySettingsAs<Friends>().Returns(friends);

            _sut = new UnfriendProcess(ProcessScopeModel, DbAccountServiceScoped, FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary,DbCampaignServiceScoped);

        }
       
        [TestMethod]
        public void Should_Unfrie_And_Save_Result_In_Database()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.cancelRequestResponse.html", Assembly.GetExecutingAssembly());

            var responseParameter = new ResponseParameter
            {
                Response = response,
                Exception = null,
                HasError = false
            };

            var facebookUser = new FacebookUser()
            {
                UserId = _userId,
            };

            _scrapeResultNew = new ScrapeResultNew
            {
                ResultUser = facebookUser,
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

            FdRequestLibrary
                .Unfriend(Arg.Any<DominatorAccountModel>(),ref facebookUser)
                .Returns(new CancelSentRequestResponseHandler(responseParameter));

            // act
            var jpr = _sut.PostScrapeProcess(_scrapeResultNew);

            // assert
            FdRequestLibrary.Received(1).Unfriend(Arg.Any<DominatorAccountModel>(), ref facebookUser);
            DbAccountServiceScoped.Received(1).Add(Arg.Any<InteractedUsers>());
            DbCampaignServiceScoped.Received(1).Add(Arg.Any<DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedUsers>());
            jpr.IsProcessSuceessfull.Should().Be(false);
        }

        [TestMethod]
        public void should_return_False_IsProcessSucessfull_is_False()
        {

            _scrapeResultNew = new ScrapeResultNew
            {
                ResultUser = new FacebookUser { UserId = _userId },
                QueryInfo = QueryInfo.NoQuery
            };

            var facebookUser=new FacebookUser();

            FdRequestLibrary
                .Unfriend(Arg.Any<DominatorAccountModel>(), ref facebookUser)
                .Returns((CancelSentRequestResponseHandler)null);

            var jpr = _sut.PostScrapeProcess(_scrapeResultNew);

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
