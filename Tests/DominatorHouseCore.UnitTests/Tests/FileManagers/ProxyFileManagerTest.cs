using Dominator.Tests.Utils;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using Unity;

namespace DominatorHouseCore.UnitTests.Tests.FileManagers
{
    [TestClass]
    public class ProxyFileManagerTest : UnityInitializationTests
    {
        private IBinFileHelper _binFileHelper;
        IProxyFileManager _proxyFileManager;
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _binFileHelper = Substitute.For<IBinFileHelper>();
            Container.RegisterInstance(_binFileHelper);
            _proxyFileManager = new ProxyFileManager();

        }

        [TestMethod]
        public void should_proxy_saved_successfully()
        {
            var proxy = new ProxyManagerModel();
            var result = _proxyFileManager.SaveProxy(proxy);
            result.Should().Be(true);
        }
        [TestMethod]
        public void should_return_all_proxy()
        {
            var proxylist = new List<ProxyManagerModel>() {
                new ProxyManagerModel(),new ProxyManagerModel()
            };
            _proxyFileManager.GetAllProxy().Returns(proxylist);
            var result = _proxyFileManager.GetAllProxy();
            result.Should().NotBeEmpty().And.HaveCount(2);
        }
        [TestMethod]
        public void should_update_proxy()
        {
            var proxylist = new List<ProxyManagerModel>() {
                new ProxyManagerModel() { AccountProxy = new Proxy() { ProxyId = "1", } },
                new ProxyManagerModel() { AccountProxy = new Proxy() { ProxyId = "2" } }
            };
            var proxyToEdit = new ProxyManagerModel() { AccountProxy = new Proxy() { ProxyId = "1", ProxyName = "test" } };

            _proxyFileManager.GetAllProxy().Returns(proxylist);
            _proxyFileManager.EditProxy(proxyToEdit);
            var result = _proxyFileManager.GetAllProxy();
            result.Should().NotBeEmpty().And.HaveCount(2);
        }

        [TestMethod]
        public void should_update_all_proxy()
        {

            var proxy = new List<ProxyManagerModel>
            {
                new ProxyManagerModel { AccountProxy = new Proxy() { ProxyId = "1" } },
                new ProxyManagerModel { AccountProxy = new Proxy() { ProxyId = "2" } }
            };
            var newProxy = new List<ProxyManagerModel>
            {
                new ProxyManagerModel { AccountProxy = new Proxy() { ProxyId = "1" } },
                new ProxyManagerModel { AccountProxy = new Proxy() { ProxyId = "2", } },
                new ProxyManagerModel { AccountProxy = new Proxy() { ProxyId = "3", } },
            };

            _binFileHelper.GetProxyDetails().Returns(proxy);

            _binFileHelper.UpdateAllProxy(Arg.Do((List<ProxyManagerModel> a) =>
             {
                 a.Should().BeEquivalentTo(newProxy);
             })).Returns(true);

            _proxyFileManager.EditAllProxy(newProxy);

            _binFileHelper.Received(1).UpdateAllProxy(Arg.Any<List<ProxyManagerModel>>());

        }

        [TestMethod]
        public void should_delete_proxy()
        {
            var proxy = new List<ProxyManagerModel>
            {
                new ProxyManagerModel { AccountProxy = new Proxy() { ProxyId = "1" } },
                new ProxyManagerModel { AccountProxy = new Proxy() { ProxyId = "2" } },
           };
            _binFileHelper.GetProxyDetails().Returns(proxy);
            _binFileHelper.UpdateAllProxy(Arg.Do((List<ProxyManagerModel> a) =>
            {
                a.Should().BeEquivalentTo(proxy);
            })).Returns(true);
            _proxyFileManager.Delete(x => x.AccountProxy.ProxyId == "1");
            _binFileHelper.GetProxyDetails().Count.Should().Be(1);

        }
        [TestMethod]
        public void should_return_proxy_with_matched_id()
        {
            var proxyTofind = new ProxyManagerModel { AccountProxy = new Proxy() { ProxyId = "1" } };
            var proxy = new List<ProxyManagerModel>
            {
                proxyTofind,
                new ProxyManagerModel { AccountProxy = new Proxy() { ProxyId = "2" } },
           };
            _binFileHelper.GetProxyDetails().Returns(proxy);
            _binFileHelper.UpdateAllProxy(Arg.Do((List<ProxyManagerModel> a) =>
            {
                a.Should().BeEquivalentTo(proxy);
            })).Returns(true);
            var result = _proxyFileManager.GetProxyById("1");
            result.Should().Be(proxyTofind);

        }
       
    }
}
