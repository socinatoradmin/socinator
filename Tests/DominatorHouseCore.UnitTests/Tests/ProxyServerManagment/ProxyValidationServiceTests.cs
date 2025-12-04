using DominatorHouseCore.ProxyServerManagment;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DominatorHouseCore.UnitTests.Tests.ProxyServerManagment
{
    [TestClass]
    public class ProxyValidationServiceTests
    {
        private IProxyValidationService _sut;

        [TestInitialize]
        public void SetUp()
        {
            _sut = new ProxyValidationService();
        }

        [TestMethod]
        public void should_pass_validation_ip_and_port()
        {
            // arrange
            var ip = "1.1.1.1";
            var port = "123";

            // act  
            var result = _sut.IsValidProxy(ip, port);

            // assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void should_NOT_pass_validation_due_to_incorrect_ip()
        {
            // arrange
            var ip = "321.1.1.1";
            var port = "123";

            // act  
            var result = _sut.IsValidProxy(ip, port);

            // assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void should_NOT_pass_validation_due_to_incorrect_port()
        {
            // arrange
            var ip = "1.1.1.1";
            var port = "122222223";

            // act  
            var result = _sut.IsValidProxy(ip, port);

            // assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void should_NOT_pass_validation_due_to_null_ip()
        {
            // arrange
            string ip = null;
            var port = "123";

            // act  
            var result = _sut.IsValidProxy(ip, port);

            // assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void should_NOT_pass_validation_due_to_null_port()
        {
            // arrange
            var ip = "1.1.1.1";
            string port = null;

            // act  
            var result = _sut.IsValidProxy(ip, port);

            // assert
            result.Should().BeFalse();
        }
    }
}
