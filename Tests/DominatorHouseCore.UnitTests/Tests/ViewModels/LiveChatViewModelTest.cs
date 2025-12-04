using Dominator.Tests.Utils;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Unity;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.UnitTests.Tests.ViewModels
{
    [TestClass, Ignore("Mockup dispatcher calls")]
    public class LiveChatViewModelTest : UnityInitializationTests
    {
        private IGenericFileManager _genericFileManager;
        LiveChatViewModel liveChatViewModel;
        IProtoBuffBase protoBuffBase;
        ILockFileConfigProvider lockFileConfigProvider;
        IFileSystemProvider fileSystemProvider;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _genericFileManager = Substitute.For<IGenericFileManager>();
            Container.RegisterInstance(_genericFileManager);
            protoBuffBase = Substitute.For<IProtoBuffBase>();
            Container.RegisterInstance(protoBuffBase);
            lockFileConfigProvider = Substitute.For<ILockFileConfigProvider>();
            Container.RegisterInstance(lockFileConfigProvider);
            fileSystemProvider = Substitute.For<IFileSystemProvider>();
            Container.RegisterInstance(fileSystemProvider);
            liveChatViewModel = new LiveChatViewModel(Enums.SocialNetworks.Facebook);
        }
        [TestMethod]
        public void should_UserSelectionChangedCommand_update_sender_list_depend_on_account_selection()
        {
            var account = new DominatorAccountModel
            {
                AccountBaseModel = new DominatorAccountBaseModel
                {
                    UserName = "kumar",
                    AccountNetwork = Enums.SocialNetworks.Facebook
                },
                AccountId = "AccountId"
            };
            liveChatViewModel.LstAccountModel.Add(account);
            liveChatViewModel.LiveChatModel.SelectedAccount = account.AccountBaseModel.UserName;
            var lstSender = new List<SenderDetails>
            {
                 new SenderDetails
            {
                SenderName = "Nk",
                LastMesseges = "Hello",
                  AccountId="AccountId"
            },
                 new SenderDetails
            {
                SenderName = "Pk",
                LastMesseges = "Hi",
                AccountId="AccountId"
            }
            };
            var filePath = FileDirPath.GetFriendDetailFile(account.AccountBaseModel.AccountNetwork);
            _genericFileManager.GetModuleDetails<SenderDetails>(filePath).ReturnsForAnyArgs(lstSender);

            liveChatViewModel.UserSelectionChangedCommand.Execute(new object());

            liveChatViewModel.LiveChatModel.LstSender.Count.Should().Be(2);
            liveChatViewModel.LiveChatModel.LstSender[0].SenderName.Should().Be("Nk");
            liveChatViewModel.LiveChatModel.LstSender[0].LastMesseges.Should().Be("Hello");
            liveChatViewModel.LiveChatModel.LstSender[1].SenderName.Should().Be("Pk");
            liveChatViewModel.LiveChatModel.LstSender[1].LastMesseges.Should().Be("Hi");
        }

        [TestMethod]
        public void should_sender_list_empty_if_no_friend_is_present_for_account()
        {
            var account = new DominatorAccountModel
            {
                AccountBaseModel = new DominatorAccountBaseModel
                {
                    UserName = "kumar",
                    AccountNetwork = Enums.SocialNetworks.Facebook
                },
                AccountId = "AccountId"
            };
            liveChatViewModel.LstAccountModel.Add(account);
            liveChatViewModel.LiveChatModel.SelectedAccount = account.AccountBaseModel.UserName;
            var lstSender = new List<SenderDetails>();
            var filePath = FileDirPath.GetFriendDetailFile(account.AccountBaseModel.AccountNetwork);
            _genericFileManager.GetModuleDetails<SenderDetails>(filePath).ReturnsForAnyArgs(lstSender);

            liveChatViewModel.UserSelectionChangedCommand.Execute(new object());

            liveChatViewModel.LiveChatModel.LstSender.Count.Should().Be(0);

        }

        [TestMethod]
        public void should_FriendSelectionChangedCommand_update_Chat_of_selected_friend_of_account()
        {
            var account = new DominatorAccountModel
            {
                AccountBaseModel = new DominatorAccountBaseModel
                {
                    AccountNetwork = Enums.SocialNetworks.Facebook
                },
            };
            liveChatViewModel.LiveChatModel.DominatorAccountModel = account;

            liveChatViewModel.LiveChatModel.SelectedAccount = account.AccountBaseModel.UserName;
            liveChatViewModel.LiveChatModel.SenderDetails = new SenderDetails
            {
                SenderId = "SenderId"
            };
            var chatlist = new List<ChatDetails>
            {
                 new ChatDetails
                    {
                        Sender = "kumar",
                        Messeges = "Hello",
                        SenderId="SenderId"
                    },
                 new ChatDetails
                    {
                        Sender = "kumar2",
                        Messeges = "whats up",
                        SenderId="SenderId"
                    }
            };
            var filePath = FileDirPath.GetFriendDetailFile(account.AccountBaseModel.AccountNetwork);
            _genericFileManager.GetModuleDetails<ChatDetails>(filePath).ReturnsForAnyArgs(chatlist);

            liveChatViewModel.FriendSelectionChangedCommand.Execute(new object());

            liveChatViewModel.LiveChatModel.LstChat.Count.Should().Be(2);
            liveChatViewModel.LiveChatModel.LstChat[0].Sender.Should().Be("kumar");
            liveChatViewModel.LiveChatModel.LstChat[0].Messeges.Should().Be("Hello");
            liveChatViewModel.LiveChatModel.LstChat[1].Sender.Should().Be("kumar2");
            liveChatViewModel.LiveChatModel.LstChat[1].Messeges.Should().Be("whats up");
        }

        [TestMethod]
        public void should_UserSelectionChangedCommand_return_empty_sender_list_if_account_is_not_selected()
        {
            var account = new DominatorAccountModel
            {
                AccountBaseModel = new DominatorAccountBaseModel
                {
                    UserName = "kumar",
                    AccountNetwork = Enums.SocialNetworks.Facebook
                },
                AccountId = "AccountId"
            };
            liveChatViewModel.LstAccountModel.Add(account);
            var lstSender = new List<SenderDetails>
            {
                 new SenderDetails
                {
                    SenderName = "Nk",
                    LastMesseges = "Hello",
                      AccountId="AccountId"
                }
            };
            var filePath = FileDirPath.GetFriendDetailFile(account.AccountBaseModel.AccountNetwork);
            _genericFileManager.GetModuleDetails<SenderDetails>(filePath).ReturnsForAnyArgs(lstSender);

            liveChatViewModel.UserSelectionChangedCommand.Execute(new object());

            liveChatViewModel.LiveChatModel.LstSender.Count.Should().Be(0);

        }
        [TestMethod]
        public void should_FriendSelectionChangedCommand_update_Chat_list_to_empty_if_SenderDetails_is_null()
        {
            var account = new DominatorAccountModel
            {
                AccountBaseModel = new DominatorAccountBaseModel
                {
                    AccountNetwork = Enums.SocialNetworks.Facebook
                },
            };
            liveChatViewModel.LiveChatModel.DominatorAccountModel = account;

            liveChatViewModel.LiveChatModel.SelectedAccount = account.AccountBaseModel.UserName;
            liveChatViewModel.LiveChatModel.SenderDetails = null;
            liveChatViewModel.FriendSelectionChangedCommand.Execute(new object());
            liveChatViewModel.LiveChatModel.LstChat.Count.Should().Be(0);
        }
    }
}
