using DominatorHouseCore.ProxyServerManagment;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.UnitTests.Tests.ProxyServerManagment
{
    [TestClass]
    public class ProxyServerParserServiceTests
    {
        private IProxyValidationService _proxyValidationService;
        private IProxyServerParserService _sut;

        [TestInitialize]
        public void SetUp()
        {
            _proxyValidationService = Substitute.For<IProxyValidationService>();
            _sut = new ProxyServerParserService(_proxyValidationService);
            _proxyValidationService.IsValidProxy(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
        }

        [TestMethod]
        public void should_parse_proxy_if_there_are_2_values()
        {
            // arrange
            var row = "1.1.1.1\t9874";

            // act
            var result = _sut.ParseProxies(new List<string> { row });

            // assert
            result.InvalidProxies.Should().BeEmpty();
            result.Proxies.Count.Should().Be(1);
            var proxy = result.Proxies.Single();
            proxy.AccountProxy.ProxyIp.Should().Be("1.1.1.1");
            proxy.AccountProxy.ProxyPort.Should().Be("9874");
        }

        [TestMethod]
        public void should_parse_proxy_if_there_are_4_values()
        {
            // arrange
            var row = "1.1.1.1\t9874\tMyUserName\tMyPwd";

            // act
            var result = _sut.ParseProxies(new List<string> { row });

            // assert
            result.InvalidProxies.Should().BeEmpty();
            result.Proxies.Count.Should().Be(1);
            var proxy = result.Proxies.Single();
            proxy.AccountProxy.ProxyIp.Should().Be("1.1.1.1");
            proxy.AccountProxy.ProxyPort.Should().Be("9874");
            proxy.AccountProxy.ProxyUsername.Should().Be("MyUserName");
            proxy.AccountProxy.ProxyPassword.Should().Be("MyPwd");
        }

        [TestMethod]
        public void should_parse_proxy_if_there_are_6_values()
        {
            // arrange
            var row = "MyProxyGroup\tMyProxyName\t1.1.1.1\t9874\tMyUserName\tMyPwd";

            // act
            var result = _sut.ParseProxies(new List<string> { row });

            // assert
            result.InvalidProxies.Should().BeEmpty();
            result.Proxies.Count.Should().Be(1);
            var proxy = result.Proxies.Single();
            proxy.AccountProxy.ProxyIp.Should().Be("1.1.1.1");
            proxy.AccountProxy.ProxyPort.Should().Be("9874");
            proxy.AccountProxy.ProxyUsername.Should().Be("MyUserName");
            proxy.AccountProxy.ProxyPassword.Should().Be("MyPwd");
            proxy.AccountProxy.ProxyGroup.Should().Be("MyProxyGroup");
            proxy.AccountProxy.ProxyName.Should().Be("MyProxyName");
        }

        [DataTestMethod]
        [DataRow("325.1.1.1\t98743333")]
        [DataRow("325.1.1.1\t98743333\tMyUserName\tMyPwd")]
        [DataRow("MyProxyGroup\tMyProxyName\t325.1.1.1\t98743333\tMyUserName\tMyPwd")]
        public void should_return_invalid_proxies_if_proxy_didnt_passed_validation(string row)
        {
            // arrange
            _proxyValidationService.IsValidProxy("325.1.1.1", "98743333").Returns(false);

            // act
            var result = _sut.ParseProxies(new List<string> { row });

            // assert
            result.Proxies.Should().BeEmpty();
            result.InvalidProxies.Count.Should().Be(1);
            var invalidProxy = result.InvalidProxies.Single();
            invalidProxy.Should().Be(row);
        }
    }
}
