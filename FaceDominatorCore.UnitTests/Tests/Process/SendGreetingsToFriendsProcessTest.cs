using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dominator.Tests.Utils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDModel.CommonSettings;
using FaceDominatorCore.FDResponse.MessagesResponse;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Reflection;
using CommonServiceLocator;
using FaceDominatorCore.FDModel.MessageModel;

namespace FaceDominatorCore.UnitTests.Tests.Process
{
    [TestClass]
    public class SendGreetingsToFriendsProcessTest : BaseFacebookProcessTest
    {
        private SendGreetingsToFriendsProcess _sut;
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
            var sendGreetingsToFriendsModel = new SendGreetingsToFriendsModel();
            var selectedQuery = new ObservableCollection<QueryContent>();
            selectedQuery.Add(new QueryContent() { IsContentSelected = true, Content = new QueryInfo() });
            sendGreetingsToFriendsModel.LstDisplayManageMessageModel.Add
                (

                new ManageMessagesModel
                {
                    SelectedQuery = selectedQuery,
                    MessageId = "",
                    MessagesText = "hai"
                }
                );


            ProcessScopeModel.GetActivitySettingsAs<SendGreetingsToFriendsModel>().Returns(sendGreetingsToFriendsModel);

            _sut = new SendGreetingsToFriendsProcess(ProcessScopeModel, DbAccountServiceScoped, FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);

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
                QueryInfo = new QueryInfo()
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

            ProcessScopeModel.GetActivitySettingsAs<CampaignDetails>().Returns(
                new CampaignDetails
                {
                    CampaignId = "456567"
                });

            FdRequestLibrary
                .SendTextMessage(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(new FdSendTextMessageResponseHandler(responseParameter));

            var binFileHelper = ServiceLocator.Current.GetInstance<IBinFileHelper>();
            binFileHelper.GetCampaignDetail().Returns(
                new List<CampaignDetails>
                {
                    new CampaignDetails {CampaignId = ""}
                });

            // act
            var jpr = _sut.PostScrapeProcess(_scrapeResultNew);

            // assert
            FdRequestLibrary.Received(1).SendTextMessage(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<string>());
            DbAccountServiceScoped.Received(1).Add((Arg.Any<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedUsers>()));
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

            var binFileHelper = ServiceLocator.Current.GetInstance<IBinFileHelper>();
            binFileHelper.GetCampaignDetail().Returns(
                new List<CampaignDetails>
                {
                    new CampaignDetails {CampaignId = ""}
                });

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
                .Returns((FdSendTextMessageResponseHandler)null);

            var jpr = _sut.PostScrapeProcess(_scrapeResultNew);

            jpr.IsProcessSuceessfull.Should().Be(false);
        }

    }
}
