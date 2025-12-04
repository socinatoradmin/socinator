using Dominator.Tests.Utils;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using FluentAssertions;
using GramDominatorCore.GDLibrary;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using GramDominatorCore.UnitTests.Tests.Library.ProcessorTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NSubstitute;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading;

namespace GramDominatorCore.UnitTests.Tests.Library.InstaProcess_Test
{
    [TestClass]
   public class AutoReplyToNewFollowerProcessTest : BaseGdProcessTest
    {
        AutoReplyToNewMessageModel autoReplyMessageModel;
        AutoReplyToNewMessagesProcess process;
        private ScrapeResultNew _scrapeResultNew;      
        IInstaFunction instafunct;
        [TestInitialize]
        public override void SetUp()
         {
            base.SetUp();
            instafunct = Substitute.For<IInstaFunction>();
            ActivityType _activityType = ActivityType.AutoReplyToNewMessage;
            GdJobProcess.ActivityType.Returns(_activityType);         
            var module = new ModuleConfiguration();
            JobActivityConfigurationManager[AccountId, _activityType].Returns(module);          
        }

        private void InitializeProcess(AutoReplyToNewMessageModel autoReplyMessageModel)
        {
            ProcessScopeModel.GetActivitySettingsAs<AutoReplyToNewMessageModel>().Returns(autoReplyMessageModel);
            var jsonData = JsonConvert.SerializeObject(autoReplyMessageModel);
            var model = new TemplateModel() { ActivitySettings = jsonData };
            var templatesFileManager = CommonServiceLocator.ServiceLocator.Current.GetInstance<ITemplatesFileManager>();
            templatesFileManager.GetTemplateById(Arg.Any<string>()).Returns(model);
            ProcessScopeModel.ActivityType.Returns(ActivityType.AutoReplyToNewMessage);
            process = new AutoReplyToNewMessagesProcess(ProcessScopeModel, AccountServiceScoped, GdQueryScraperFactory,
                GdHttpHelper, GdBrowserManager, DelayService);
        }
        [TestMethod]
        public void Check_AutoReplyToNewFollowerProcess_Without_Link()
        {
            //arrange
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultUser = new InstagramUser { Pk = "4580718406" },
                QueryInfo = new QueryInfo() {QueryTypeDisplayName= "false<:><:>340282366841710300949128316399275322393" }
            };
            var pageresponse = TestUtils.ReadFileFromResources(
          "GramDominatorCore.UnitTests.RequiredData.AutoReplyLstData.json", Assembly.GetExecutingAssembly());
            ObservableCollection<ManageMessagesModel> list = JsonConvert.DeserializeObject<ObservableCollection<ManageMessagesModel>>(pageresponse);
            var MessageInboxList = TestUtils.ReadFileFromResources(
         "GramDominatorCore.UnitTests.RequiredData.MessageInboxList.json", Assembly.GetExecutingAssembly());
            List<UserConversation> MessageList = JsonConvert.DeserializeObject<List<UserConversation>>(MessageInboxList);
            AccountServiceScoped.GetConversationUser().Returns(MessageList);
            autoReplyMessageModel = new AutoReplyToNewMessageModel()
            {
                LstDisplayManageMessageModel = list,
            };
            InitializeProcess(autoReplyMessageModel);
            instafunct = GdLogInProcess.InstagramFunctFactory.InstaFunctions;
            process.instaFunct = instafunct;
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);
           
            var sendMessage = TestUtils.ReadFileFromResources(
              "GramDominatorCore.UnitTests.TestData.SendMessage.json", Assembly.GetExecutingAssembly());
            SendMessageIgResponseHandler SendMessageResponse = new SendMessageIgResponseHandler(new ResponseParameter
            {
                Response = sendMessage
            });
            //act
            instafunct.SendMessage(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(SendMessageResponse);          
             var jpr = process.PostScrapeProcess(_scrapeResultNew);
            //assert
            instafunct.Received(1).SendMessage(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
        }

        [TestMethod]
        public void Check_AutoReplyToNewFollowerProcess_With_Link()
        {
            //arrange
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultUser = new InstagramUser { Pk = "4580718406" },
                QueryInfo = new QueryInfo() { QueryTypeDisplayName = "false<:><:>340282366841710300949128316399275322393" }
            };
            var pageresponse = TestUtils.ReadFileFromResources(
         "GramDominatorCore.UnitTests.RequiredData.AutoReplyLstData.json", Assembly.GetExecutingAssembly());
            ObservableCollection<ManageMessagesModel> list = JsonConvert.DeserializeObject<ObservableCollection<ManageMessagesModel>>(pageresponse);
            autoReplyMessageModel = new AutoReplyToNewMessageModel()
            {
                LstDisplayManageMessageModel = list,
            };
            var MessageListBoxData = TestUtils.ReadFileFromResources(
"GramDominatorCore.UnitTests.RequiredData.MessageInboxList.json", Assembly.GetExecutingAssembly());
            List<UserConversation> messageListBox = JsonConvert.DeserializeObject<List<UserConversation>>(MessageListBoxData);
            AccountServiceScoped.GetConversationUser().Returns(messageListBox);
            autoReplyMessageModel.LstDisplayManageMessageModel[0].MessagesText = "wow www.javatpoint.com";
            InitializeProcess(autoReplyMessageModel);
            instafunct = GdLogInProcess.InstagramFunctFactory.InstaFunctions;
            ((GdJobProcess)process).instaFunct = instafunct;
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);

            var SendMessageWithpageresponse = TestUtils.ReadFileFromResources(
              "GramDominatorCore.UnitTests.TestData.SendMessageWithLink.json", Assembly.GetExecutingAssembly());
            SendMessageIgResponseHandler SendMessageResponse = new SendMessageIgResponseHandler(new ResponseParameter
            {
                Response = SendMessageWithpageresponse
            });
            //act
            instafunct.SendMessageWithLink(Arg.Any<DominatorAccountModel>(), Arg.Any<CancellationToken>(), Arg.Any<string>(), Arg.Any<string>(),Arg.Any<List<string>>(), Arg.Any<string>(), Arg.Any<AccountModel>()).Returns(SendMessageResponse);               
            var jpr = process.PostScrapeProcess(_scrapeResultNew);
           
            //assert
            instafunct.Received(1).SendMessageWithLink(Arg.Any<DominatorAccountModel>(), Arg.Any<CancellationToken>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<List<string>>(), Arg.Any<string>(), Arg.Any<AccountModel>());
        }

        [TestMethod]
        public void Check_AutoReplyToNewFollowerProcess_Without_Link_Will_Return_False()
        {
            //arrange
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultUser = new InstagramUser { Pk = "" },
                QueryInfo = new QueryInfo() { QueryTypeDisplayName = "" }
            };
            var pageresponse = TestUtils.ReadFileFromResources(
          "GramDominatorCore.UnitTests.RequiredData.AutoReplyLstData.json", Assembly.GetExecutingAssembly());
            ObservableCollection<ManageMessagesModel> list = JsonConvert.DeserializeObject<ObservableCollection<ManageMessagesModel>>(pageresponse);
            autoReplyMessageModel = new AutoReplyToNewMessageModel()
            {
                LstDisplayManageMessageModel = list,
            };
            var MessageListBoxData = TestUtils.ReadFileFromResources(
"GramDominatorCore.UnitTests.RequiredData.MessageInboxList.json", Assembly.GetExecutingAssembly());
            List<UserConversation> messageListBox = JsonConvert.DeserializeObject<List<UserConversation>>(MessageListBoxData);
            AccountServiceScoped.GetConversationUser().Returns(messageListBox);
            autoReplyMessageModel.LstDisplayManageMessageModel[0].MessagesText = "";
            InitializeProcess(autoReplyMessageModel);
            instafunct = GdLogInProcess.InstagramFunctFactory.InstaFunctions;
            ((GdJobProcess)process).instaFunct = instafunct;
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);

            var sendMessage = TestUtils.ReadFileFromResources(
              "GramDominatorCore.UnitTests.TestData.SendMessage.json", Assembly.GetExecutingAssembly());
            SendMessageIgResponseHandler SendMessageResponse = new SendMessageIgResponseHandler(new ResponseParameter
            {
                Response = sendMessage
            });
            //act
            instafunct.SendMessage(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(SendMessageResponse);
            var jpr = process.PostScrapeProcess(_scrapeResultNew);
            //assert         
            jpr.IsProcessSuceessfull.Should().BeFalse();
        }

        [TestMethod]
        public void Check_AutoReplyToNewFollowerProcess_With_Link_Will_Return_False()
        {
            //arrange
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultUser = new InstagramUser { Pk = "" },
                QueryInfo = new QueryInfo() { QueryTypeDisplayName =""}
            };
            var pageresponse = TestUtils.ReadFileFromResources(
         "GramDominatorCore.UnitTests.RequiredData.AutoReplyLstData.json", Assembly.GetExecutingAssembly());
            ObservableCollection<ManageMessagesModel> list = JsonConvert.DeserializeObject<ObservableCollection<ManageMessagesModel>>(pageresponse);
            autoReplyMessageModel = new AutoReplyToNewMessageModel()
            {
                LstDisplayManageMessageModel = list,
            };
            var MessageListBoxData = TestUtils.ReadFileFromResources(
"GramDominatorCore.UnitTests.RequiredData.MessageInboxList.json", Assembly.GetExecutingAssembly());
            List<UserConversation> messageListBox = JsonConvert.DeserializeObject<List<UserConversation>>(MessageListBoxData);
            AccountServiceScoped.GetConversationUser().Returns(messageListBox);
            autoReplyMessageModel.LstDisplayManageMessageModel[0].MessagesText = "";
            InitializeProcess(autoReplyMessageModel);
            instafunct = GdLogInProcess.InstagramFunctFactory.InstaFunctions;
            ((GdJobProcess)process).instaFunct = instafunct;
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);

            var SendMessageWithpageresponse = TestUtils.ReadFileFromResources(
              "GramDominatorCore.UnitTests.TestData.SendMessageWithLink.json", Assembly.GetExecutingAssembly());
            SendMessageIgResponseHandler SendMessageResponse = new SendMessageIgResponseHandler(new ResponseParameter
            {
                Response = SendMessageWithpageresponse
            });

            //act
            instafunct.SendMessageWithLink(Arg.Any<DominatorAccountModel>(), Arg.Any<CancellationToken>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<List<string>>(), Arg.Any<string>(), Arg.Any<AccountModel>()).Returns(SendMessageResponse);
            var jpr = process.PostScrapeProcess(_scrapeResultNew);

            //assert
            jpr.IsProcessSuceessfull.Should().BeFalse();
        }
    }
}
