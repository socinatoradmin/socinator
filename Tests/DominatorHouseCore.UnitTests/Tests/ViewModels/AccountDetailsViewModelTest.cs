using FluentAssertions;
using DominatorHouseCore.Models;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DominatorUIUtility.ViewModel;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Utility;
using Dominator.Tests.Utils;
using NSubstitute;
using Unity;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using System.Windows;
using System.Threading.Tasks;

namespace DominatorHouseCore.UnitTests.Tests.ViewModels
{
    [TestClass]
    public class AccountDetailsViewModelTest : UnityInitializationTests
    {
        IAccountsFileManager _accountsFileManager;
        IAccountScopeFactory _accountScopeFactory;
        IProxyFileManager _proxyFileManager;
        DominatorAccountModel _dominatorAccountModel;
        AccountDetailsViewModel _accountDetailsViewModel;

        INetworkCollectionFactory _networkCollectionFactory;
        IAccountVerificationFactory _accountVarifiaction;
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _accountsFileManager = Substitute.For<IAccountsFileManager>();
            Container.RegisterInstance(_accountsFileManager);
            _accountScopeFactory = Substitute.For<IAccountScopeFactory>();
            Container.RegisterInstance(_accountScopeFactory);
            _proxyFileManager = Substitute.For<IProxyFileManager>();
            Container.RegisterInstance(_proxyFileManager);
            _dominatorAccountModel = new DominatorAccountModel
            {
                AccountBaseModel = new DominatorAccountBaseModel
                {
                    UserName = "kumar",
                    Password = "kumar",
                    AccountId = "accountId",
                },
                AccountId = "accountId"
            };
            _accountDetailsViewModel = new AccountDetailsViewModel(_dominatorAccountModel);
        }
        [TestMethod]
        public void AddNewCookiesCommand_Shuold_add_new_empty_cookies_to_account()
        {
            _accountDetailsViewModel.AddNewCookiesCommand.Execute(new object());
            _dominatorAccountModel.CookieHelperList.Count.Should().Be(1);
            _dominatorAccountModel.CookieHelperList.FirstOrDefault().Value.Should().BeEmpty();
        }
        [TestMethod]
        public void SaveCommand_Shuold_edit_account_and_update_to_bin_file()
        {
            _accountsFileManager.GetAccountById("accountId").ReturnsForAnyArgs(_dominatorAccountModel);
            _accountDetailsViewModel.SaveCommand.Execute(new object());
            _accountDetailsViewModel.OldDominatorAccountModel.AccountBaseModel.UserName.Should().BeEquivalentTo(_dominatorAccountModel.AccountBaseModel.UserName);
            _accountDetailsViewModel.OldDominatorAccountModel.AccountBaseModel.Password.Should().BeEquivalentTo(_dominatorAccountModel.AccountBaseModel.Password);
            _accountsFileManager.Received(2).GetAccountById("accountId");
            _accountsFileManager.Received(2).Edit(_dominatorAccountModel);
        }
        [TestMethod]
        public void VerifyAccountCommand_Shuold_verify_account()
        {
            _dominatorAccountModel.AccountBaseModel.AccountNetwork = SocialNetworks.Instagram;
            _dominatorAccountModel.VarificationCode = "123456";
            _accountVarifiaction = Substitute.For<IAccountVerificationFactory>();
            Container.RegisterInstance(_accountVarifiaction);
            _networkCollectionFactory = Substitute.For<INetworkCollectionFactory>();
            SocinatorInitialize.SocialNetworkRegister(_networkCollectionFactory, SocialNetworks.Instagram);
            _accountVarifiaction.VerifyAccountAsync(_dominatorAccountModel, VerificationType.Phone,
                          _dominatorAccountModel.Token).ReturnsForAnyArgs(true);
            _accountDetailsViewModel.VerifyAccountCommand.Execute(new object());

        }
        [TestMethod]
        public void SendVerificationCodeCommand_Shuold_send_verify_code_for_account()
        {
            _dominatorAccountModel.AccountBaseModel.AccountNetwork = SocialNetworks.Instagram;
            _accountVarifiaction = Substitute.For<IAccountVerificationFactory>();
            Container.RegisterInstance(_accountVarifiaction);
            _networkCollectionFactory = Substitute.For<INetworkCollectionFactory>();
            SocinatorInitialize.SocialNetworkRegister(_networkCollectionFactory, SocialNetworks.Instagram);
            _accountVarifiaction.SendVerificationCode(_dominatorAccountModel, VerificationType.Phone,
                          _dominatorAccountModel.Token).ReturnsForAnyArgs(true);
          Task.Factory.StartNew(()=>  _accountDetailsViewModel.SendVerificationCodeCommand.Execute(new object())).ContinueWith((x)=> {
              _accountDetailsViewModel.BtnSendVerificationCodeVisibility.Should().Be(Visibility.Visible);
          });
        }
    }
}
