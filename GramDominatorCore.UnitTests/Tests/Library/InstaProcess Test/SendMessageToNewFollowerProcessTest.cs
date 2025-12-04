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
    public class SendMessageToNewFollowerProcessTest : BaseGdProcessTest
    {
          SendMessageToFollowerModel _sendMessageToNewFollower;
        SendMessageToNewFollowersProcess _process;
        private ScrapeResultNew _scrapeResultNew;     
        IInstaFunction _instafunct;
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _instafunct = Substitute.For<IInstaFunction>();
            var _activityType = ActivityType.SendMessageToFollower;
            GdJobProcess.ActivityType.Returns(_activityType);
            var module = new ModuleConfiguration();
            JobActivityConfigurationManager[AccountId, _activityType].Returns(module);
        }

        private void InitializeProcess(SendMessageToFollowerModel sendMessagetoFollower)
        {
            ProcessScopeModel.GetActivitySettingsAs<SendMessageToFollowerModel>().Returns(sendMessagetoFollower);
            var jsonData = JsonConvert.SerializeObject(sendMessagetoFollower);
            var model = new TemplateModel() { ActivitySettings = jsonData };
            var templatesFileManager = CommonServiceLocator.ServiceLocator.Current.GetInstance<ITemplatesFileManager>();
            templatesFileManager.GetTemplateById(Arg.Any<string>()).Returns(model);
            ProcessScopeModel.ActivityType.Returns(ActivityType.AutoReplyToNewMessage);
            _process = new SendMessageToNewFollowersProcess(ProcessScopeModel, AccountServiceScoped, GdQueryScraperFactory, GdHttpHelper, GdBrowserManager, DelayService);
        }
        [TestMethod]
        public void Check_SendMessageToNewFollower_Without_Link()
        {
            //arrange
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultUser = new InstagramUser { Pk = "4580718406" },
                QueryInfo = new QueryInfo()
                {
                    QueryTypeDisplayName = "false<:><:>340282366841710300949128316399275322393",
                    QueryType = "Keywords",
                    QueryValue = "viru",
                }
            };
            _sendMessageToNewFollower = new SendMessageToFollowerModel();
            _sendMessageToNewFollower.TextMessage = "wow";
            InitializeProcess(_sendMessageToNewFollower);
            _process.instaFunct = _instafunct;
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);

            var sendMessage = TestUtils.ReadFileFromResources(
              "GramDominatorCore.UnitTests.TestData.SendMessage.json", Assembly.GetExecutingAssembly());
            SendMessageIgResponseHandler sendMessageResponse = new SendMessageIgResponseHandler(new ResponseParameter
            {
                Response = sendMessage
            });
            var MessageListBoxData = TestUtils.ReadFileFromResources(
 "GramDominatorCore.UnitTests.RequiredData.MessageInboxList.json", Assembly.GetExecutingAssembly());
            List<UserConversation> messageListBox = JsonConvert.DeserializeObject<List<UserConversation>>(MessageListBoxData);
            AccountServiceScoped.GetConversationUser().Returns(messageListBox);
            //act
            _instafunct.SendMessage(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(sendMessageResponse);
            var jpr = _process.PostScrapeProcess(_scrapeResultNew);
            //assert
            _instafunct.Received(1).SendMessage(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
        }

        [TestMethod]
        public void Check_sendMessageToNewFollower_With_Link()
        {
            //arrange
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultUser = new InstagramUser { Pk = "4580718406" },
                QueryInfo = new QueryInfo()
                {
                    QueryTypeDisplayName = "false<:><:>340282366841710300949128316399275322393",
                    QueryType = "Keywords",
                    QueryValue = "viru",
                }
            };
            _sendMessageToNewFollower = new SendMessageToFollowerModel();
            _sendMessageToNewFollower.TextMessage = "wow www.javatpoint.com";
            InitializeProcess(_sendMessageToNewFollower);

            _process.instaFunct = _instafunct;
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);

            var sendMessageWithpageresponse = TestUtils.ReadFileFromResources(
              "GramDominatorCore.UnitTests.TestData.SendMessageWithLink.json", Assembly.GetExecutingAssembly());
            SendMessageIgResponseHandler sendMessageResponse = new SendMessageIgResponseHandler(new ResponseParameter
            {
                Response = sendMessageWithpageresponse
            });

            //act
            _instafunct.SendMessageWithLink(Arg.Any<DominatorAccountModel>(), Arg.Any<CancellationToken>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<List<string>>(), Arg.Any<string>(), Arg.Any<AccountModel>()).Returns(sendMessageResponse);
            var jpr = _process.PostScrapeProcess(_scrapeResultNew);

            //assert
            _instafunct.Received(1).SendMessageWithLink(Arg.Any<DominatorAccountModel>(), Arg.Any<CancellationToken>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<List<string>>(), Arg.Any<string>(), Arg.Any<AccountModel>());

        }

        [TestMethod]
        public void Check_sendMessageToNewFollower_Without_Link_Will_return_false()
        {
            //arrange
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultUser = new InstagramUser { Pk = "" },
                QueryInfo = new QueryInfo()
                {
                    QueryTypeDisplayName = "false<:><:>340282366841710300949128316399275322393",
                    QueryType = "Keywords",
                    QueryValue = "viru",
                }
            };
            _sendMessageToNewFollower = new SendMessageToFollowerModel();
            _sendMessageToNewFollower.TextMessage = "";
            InitializeProcess(_sendMessageToNewFollower);
            _process.instaFunct = _instafunct;
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);

            var sendMessage = TestUtils.ReadFileFromResources(
              "GramDominatorCore.UnitTests.TestData.SendMessage.json", Assembly.GetExecutingAssembly());
            SendMessageIgResponseHandler sendMessageResponse = new SendMessageIgResponseHandler(new ResponseParameter
            {
                Response = sendMessage
            });
            var MessageListBoxData = TestUtils.ReadFileFromResources(
"GramDominatorCore.UnitTests.RequiredData.MessageInboxList.json", Assembly.GetExecutingAssembly());
            List<UserConversation> messageListBox = JsonConvert.DeserializeObject<List<UserConversation>>(MessageListBoxData);
            AccountServiceScoped.GetConversationUser().Returns(messageListBox);
            //act
            _instafunct.SendMessage(Arg.Any<DominatorAccountModel>(), Arg.Any<AccountModel>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(sendMessageResponse);
            var jpr = _process.PostScrapeProcess(_scrapeResultNew);
            //assert
            jpr.IsProcessSuceessfull.Should().BeFalse();
        }

        [TestMethod]
        public void Check_sendMessageToNewFollower_With_Link_will_return_false()
        {
            //arrange
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultUser = new InstagramUser { Pk = null },
                QueryInfo = new QueryInfo()
                {
                    QueryTypeDisplayName = "false<:><:>340282366841710300949128316399275322393",
                    QueryType = "Keywords",
                    QueryValue = "viru",
                }
            };
            var pageresponse = TestUtils.ReadFileFromResources(
         "GramDominatorCore.UnitTests.RequiredData.BroadcastLstData.json", Assembly.GetExecutingAssembly());
            ObservableCollection<ManageMessagesModel> list = JsonConvert.DeserializeObject<ObservableCollection<ManageMessagesModel>>(pageresponse);
            _sendMessageToNewFollower = new SendMessageToFollowerModel()
            {
                LstDisplayManageMessageModel = list,
            };
            var MessageListBoxData = TestUtils.ReadFileFromResources(
"GramDominatorCore.UnitTests.RequiredData.MessageInboxList.json", Assembly.GetExecutingAssembly());
            List<UserConversation> messageListBox = JsonConvert.DeserializeObject<List<UserConversation>>(MessageListBoxData);
            AccountServiceScoped.GetConversationUser().Returns(messageListBox);
            _sendMessageToNewFollower.LstDisplayManageMessageModel[0].MessagesText = "";
            InitializeProcess(_sendMessageToNewFollower);

            _process.instaFunct = _instafunct;
            var requestParameters = new IgRequestParameters();
            InstaFunct.GetGdHttpHelper().GetRequestParameter().Returns(requestParameters);

            var sendMessageWithpageresponse = TestUtils.ReadFileFromResources(
              "GramDominatorCore.UnitTests.TestData.SendMessageWithLink.json", Assembly.GetExecutingAssembly());
            SendMessageIgResponseHandler sendMessageResponse = new SendMessageIgResponseHandler(new ResponseParameter
            {
                Response = sendMessageWithpageresponse
            });

            //act
            _instafunct.SendMessageWithLink(Arg.Any<DominatorAccountModel>(), Arg.Any<CancellationToken>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<List<string>>(), Arg.Any<string>(), Arg.Any<AccountModel>()).Returns(sendMessageResponse);
            var jpr = _process.PostScrapeProcess(_scrapeResultNew);

            //assert
            jpr.IsProcessSuceessfull.Should().BeFalse();
        }
    }
}
