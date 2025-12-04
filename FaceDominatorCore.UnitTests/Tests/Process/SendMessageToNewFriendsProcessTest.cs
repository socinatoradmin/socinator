using System;
using Dominator.Tests.Utils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDModel.CommonSettings;
using FaceDominatorCore.FDModel.MessageModel;
using FaceDominatorCore.FDResponse.MessagesResponse;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.ObjectModel;
using System.Reflection;

namespace FaceDominatorCore.UnitTests.Tests.Process
{
    [TestClass]
    public class SendMessageToNewFriendsProcessTest : BaseFacebookProcessTest
    {
        private SendMessageToNewFriendsProcess _sut;
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

            var messageRecentFriendsModel = new MessageRecentFriendsModel();
            var selectedQuery = new ObservableCollection<QueryContent>();
            selectedQuery.Add(new QueryContent() { IsContentSelected = true, Content = new QueryInfo() });
            messageRecentFriendsModel.LstDisplayManageMessageModel.Add
            (
                new ManageMessagesModel
                {
                    SelectedQuery = selectedQuery,
                    MessageId = "",
                    MessagesText = "hai"
                }
            );


            ProcessScopeModel.GetActivitySettingsAs<MessageRecentFriendsModel>().Returns(messageRecentFriendsModel);

            _sut = new SendMessageToNewFriendsProcess(ProcessScopeModel, DbAccountServiceScoped, FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);

        }

        [TestMethod]
        public void Should_SendTextMessage_And_Save_Result_In_Database()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.SendTextMessageResponse.html", Assembly.GetExecutingAssembly());

            var responseParameter = new ResponseParameter
            {
                Response = response,
                Exception = null,
                HasError = false
            };

            _scrapeResultNew = new ScrapeResultNew
            {
                ResultUser = new FacebookUser { UserId = _userId },
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
                .SendTextMessage(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(new FdSendTextMessageResponseHandler(responseParameter));

            // act
            var jpr = _sut.PostScrapeProcess(_scrapeResultNew);

            // assert
            FdRequestLibrary.Received(1).SendTextMessage(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<string>());
            DbAccountServiceScoped.Received(1).Add(Arg.Any<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedUsers>());
            DbCampaignServiceScoped.Received(1).Add(Arg.Any<DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedUsers>());
            jpr.IsProcessSuceessfull.Should().Be(true);
        }

        [TestMethod]
        public void should_return_False_IsProcessSucessfull_is_False()
        {

            _scrapeResultNew = new ScrapeResultNew
            {
                ResultUser = new FacebookUser() { UserId = _userId },
                QueryInfo = QueryInfo.NoQuery
            };
       
            FdRequestLibrary
                .SendTextMessage(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns((FdSendTextMessageResponseHandler)null);

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
