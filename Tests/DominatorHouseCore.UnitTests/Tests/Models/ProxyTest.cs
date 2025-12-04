using DominatorHouseCore.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using Dominator.Tests.Utils;
using DominatorHouseCore.Utility;
using NSubstitute;
using Unity;

namespace DominatorHouseCore.UnitTests.Tests.Models
{
    [TestClass]
    public class ProxyTest : UnityInitializationTests
    {
        private Proxy _proxy;
        private IWebService _webService;
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _proxy = new Proxy();
            _webService = Substitute.For<IWebService>();
            Container.RegisterInstance<IWebService>(_webService);
        }
        //[TestMethod]
        //public void should_return_false_if_proxy_is_not_working()
        //{
        //    _proxy.ProxyIp = "1.9.0.8";
        //    _proxy.ProxyPort = "12";
        //    _webService.CheckProxy(Arg.Any<Uri>(), Arg.Any<WebProxy>()).Returns(false);
        //    var result = _proxy.CheckProxy();
        //    result.Should().BeFalse();
        //}
        //[TestMethod]
        //public void should_return_true_if_proxy_is_working()
        //{
        //    _proxy.ProxyIp = "104.144.108.118";
        //    _proxy.ProxyPort = "3128";
        //    _webService.CheckProxy(Arg.Any<Uri>(), Arg.Any<WebProxy>()).Returns(true);
        //    var result = _proxy.CheckProxy();
        //    result.Should().BeTrue();
        //}
       
        [TestMethod]
        public void should_return_false_if_proxyip_is_not_in_correct_format()
        {
            _proxy.ProxyIp = "9991.9.0.8";
          
            var result = Proxy.IsValidProxyIp(_proxy.ProxyIp);
            result.Should().BeFalse();
        }
        [TestMethod]
        public void should_return_true_if_proxyip_is_in_correct_format()
        {
            _proxy.ProxyIp = "104.144.108.118";
            var result = Proxy.IsValidProxyIp(_proxy.ProxyIp);
            result.Should().BeTrue();
        }
        [TestMethod]
        public void should_return_false_if_proxyport_is_not_in_range()
        {
            _proxy.ProxyPort = "65536";
            var result = Proxy.IsValidProxyPort(_proxy.ProxyPort);
            result.Should().BeFalse();
        }

        [TestMethod]
        public void should_return_true_if_proxyport_is_in_range()
        {
            _proxy.ProxyPort = "65535";
            var result = Proxy.IsValidProxyPort(_proxy.ProxyPort);
            result.Should().BeTrue();
        }

        [TestMethod]
        public void should_throw_ArgumentNullException_if_proxyip_is_null()
        {
            Assert.ThrowsException<ArgumentNullException>(() => Proxy.IsValidProxyIp(_proxy.ProxyIp));
        }

        //[TestMethod]
        //public void should_throw_ArgumentException_if_proxyip_or_port_is_null()
        //{
        //    Assert.ThrowsException<ArgumentException>(() => _proxy.CheckProxy());
        //}

      
        [TestMethod]
        public void should_throw_ArgumentNullException_if_proxyport_is_null()
        {
            Assert.ThrowsException<ArgumentNullException>(() => Proxy.IsValidProxyPort(_proxy.ProxyPort));
        }
    }
}
