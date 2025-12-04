using System;
using Dominator.Tests.Utils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDModel.CommonSettings;
using FaceDominatorCore.FDModel.InviterModel;
using FaceDominatorCore.FDResponse.InviterResponse;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Reflection;

namespace FaceDominatorCore.UnitTests.Tests.Process
{
    [TestClass]
    public class GroupInviterProcessTest : BaseFacebookProcessTest
    {
        private GroupInviterProcess _sut;
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

            _activityType = ActivityType.GroupInviter;
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

            var groupInviterModel = new GroupInviterModel();
            groupInviterModel.InviterOptionsModel = new DominatorHouseCore.Models.FacebookModels.InviterOptions()
            {
                IsSendInvitationWithNote = true,
                Note = _note,
            };
            ProcessScopeModel.GetActivitySettingsAs<GroupInviterModel>().Returns(groupInviterModel);

            _sut = new GroupInviterProcess(ProcessScopeModel, DbAccountServiceScoped, FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);

        }

        [TestMethod]
        public void Should_SendGroupInvittationTofriends_And_Save_Result_In_Database()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.SendGroupInvittationTofriendsResponse.html", Assembly.GetExecutingAssembly());

            var responseParmater = new ResponseParameter()
            {
                Response = response,
                Exception = null,
                HasError = false
            };

            _scrapeResultNew = new ScrapeResultNew
            {
                ResultUser = new FacebookUser(),
                ResultGroup = new GroupDetails { GroupId = "65447687" },
                QueryInfo = QueryInfo.NoQuery
            };

            ProcessScopeModel.TemplateId.Returns(_templateId);


            var moduleConfigurations = new ModuleConfiguration
            {
                ActivityType = ActivityType.GroupInviter,
                IsTemplateMadeByCampaignMode = true,
                TemplateId = _templateId
            };

            JobActivityConfigurationManager[_accountId, ActivityType.GroupInviter]
                .Returns(moduleConfigurations);

            GroupInviterResponseHandler groupInviterResponseHandler
                = new GroupInviterResponseHandler(responseParmater);

            FdRequestLibrary.SendGroupInvittationTofriends(Arg.Any<DominatorAccountModel>(),
                    Arg.Any<string>(), Arg.Any<FacebookUser>(), _note).Returns(groupInviterResponseHandler);

            // act
            var jpr = _sut.PostScrapeProcess(_scrapeResultNew);

            // assert
            FdRequestLibrary.Received(1).SendGroupInvittationTofriends(Arg.Any<DominatorAccountModel>(),
                    Arg.Any<string>(), Arg.Any<FacebookUser>(), _note);
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
                ResultGroup = new GroupDetails { GroupId = "65447687" },
                QueryInfo = QueryInfo.NoQuery
            };

             FdRequestLibrary.SendGroupInvittationTofriends(Arg.Any<DominatorAccountModel>(),
                Arg.Any<string>(), Arg.Any<FacebookUser>(), _note).Returns((GroupInviterResponseHandler)null);

            var jpr = _sut.PostScrapeProcess(_scrapeResultNew);

            jpr.IsProcessSuceessfull.Should().Be(false);
            FdRequestLibrary.Received(1).SendGroupInvittationTofriends(Arg.Any<DominatorAccountModel>(),
                Arg.Any<string>(), Arg.Any<FacebookUser>(), _note);
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
