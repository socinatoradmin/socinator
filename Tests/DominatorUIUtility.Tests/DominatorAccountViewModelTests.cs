using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Dominator.Tests.Utils;
using DominatorHouseCore.DatabaseHandler.CoreModels;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Settings;
using DominatorHouseCore.ViewModel;
using DominatorUIUtility.ViewModel;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Unity;

namespace DominatorUIUtility.Tests
{
    [TestClass]
    public class DominatorAccountViewModelTests : UnityInitializationTests
    {
        private DominatorAccountViewModel _sut;
        private Fixture _fixture;
        private IMainViewModel _mainViewModel;
        private ISelectedNetworkViewModel _selectedNetworkViewModel;
        private IProxyManagerViewModel _proxyManagerViewModel;
        private IAccountsFileManager _accountsFileManager;
        private IAccountCollectionViewModel _accountCollectionViewModel;
        private IDataBaseHandler _dataBaseHandler;
        private IProxyFileManager _proxyFileManager;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _fixture = new Fixture();
            _fixture.Customize(new AutoNSubstituteCustomization());
            _mainViewModel = Substitute.For<IMainViewModel>();
            _selectedNetworkViewModel = Substitute.For<ISelectedNetworkViewModel>();
            _proxyManagerViewModel = Substitute.For<IProxyManagerViewModel>();
            _accountsFileManager = Substitute.For<IAccountsFileManager>();
            _accountCollectionViewModel = Substitute.For<IAccountCollectionViewModel>();
            _dataBaseHandler = Substitute.For<IDataBaseHandler>();
            _proxyFileManager = Substitute.For<IProxyFileManager>();
            var softwareSettings = Substitute.For<ISoftwareSettings>();
            var softwareSettingsFileManager = Substitute.For<ISoftwareSettingsFileManager>();
            softwareSettingsFileManager.GetSoftwareSettings().Returns(new SoftwareSettingsModel());
            Container.RegisterInstance(softwareSettingsFileManager);
            _sut = new DominatorAccountViewModel(_mainViewModel, _selectedNetworkViewModel, _proxyManagerViewModel, softwareSettings,
                _accountsFileManager, _accountCollectionViewModel, _dataBaseHandler, _proxyFileManager);
        }

        [TestMethod]
        public void should_UpdateProxy_and_return_success()
        {
            // arrange
            var dominatorModel = new DominatorAccountBaseModel
            {
                UserName = "TestUser",
                AccountNetwork = SocialNetworks.Facebook,
                AccountProxy = new Proxy
                {
                    ProxyId = "123",
                    ProxyIp = "10.0.0.2",
                    ProxyPort = "3234"
                }
            };
            var dominatorModelOld = new DominatorAccountBaseModel
            {
                UserName = "TestUser",
                AccountNetwork = SocialNetworks.Facebook,
                AccountProxy = new Proxy
                {
                    ProxyId = "123",
                    ProxyIp = "10.0.0.1",
                    ProxyPort = "3233"
                }
            };
            var proxy = new ProxyManagerModel
            {
                AccountProxy = new Proxy
                {
                    ProxyId = "123",
                    ProxyIp = "10.0.0.1",
                    ProxyPort = "3233"
                }
            };
            _proxyFileManager.GetAllProxy().Returns(new List<ProxyManagerModel>
            {
                proxy
            });
            _accountsFileManager
                .GetAccount(dominatorModel.UserName, dominatorModel.AccountNetwork).Returns(new DominatorAccountModel
                {
                    AccountBaseModel = dominatorModelOld
                });

            // act
            var result = _sut.UpdateProxy(dominatorModel);

            // assert
            result.Should().BeTrue();
            _proxyFileManager.Received(1).EditProxy(proxy);
            proxy.AccountProxy.ProxyIp.Should().Be("10.0.0.2");
            proxy.AccountProxy.ProxyPort.Should().Be("3234");
        }
    }
}
