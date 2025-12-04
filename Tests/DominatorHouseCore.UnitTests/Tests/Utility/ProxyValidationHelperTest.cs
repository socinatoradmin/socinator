using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;

namespace DominatorHouseCore.UnitTests.Tests.FileManagers
{
    [TestClass]
    public class ProxyValidationHelperTest
    {
        ProxyValidationHelper _proxyValidationHelper;
        string input;
        [TestInitialize]
        public void StartUp()
        {
            _proxyValidationHelper = new ProxyValidationHelper();
        }
        [TestMethod]
        public void should_return_Required_Field_for_proxyAddress_if_input_is_empty_string()
        {
            input = "";
            _proxyValidationHelper.Sender = "txtProxyAddress";
            var expected= new ValidationResult(false, "LangKeyRequiredField");
            var result= _proxyValidationHelper.Validate(input, CultureInfo.CurrentCulture);
            result.Should().Be(expected);
        }
        [TestMethod]
        public void should_return_Invalid_Address_for_proxyAddress_if_input_is_random_string()
        {
            input = "myproxy";
            _proxyValidationHelper.Sender = "txtProxyAddress";
            var expected = new ValidationResult(false, "LangKeyInvalidAddress");
            var result = _proxyValidationHelper.Validate(input, CultureInfo.CurrentCulture);
            result.Should().Be(expected);
        }
        [TestMethod]
        public void should_return_true_for_proxyAddress_if_input_is_valid_proxy()
        {
            input = "1.2.3.45";
            _proxyValidationHelper.Sender = "txtProxyAddress";
            var expected = new ValidationResult(true, null); 
            var result = _proxyValidationHelper.Validate(input, CultureInfo.CurrentCulture);
            result.Should().Be(expected);
        }
        [TestMethod]
        public void should_return_Required_Field_for_proxyport_if_input_is_empty_string()
        {
            input = "";
            _proxyValidationHelper.Sender = "txtProxyPort";
            var expected = new ValidationResult(false, "LangKeyRequiredField");
            var result = _proxyValidationHelper.Validate(input, CultureInfo.CurrentCulture);
            result.Should().Be(expected);
        }
        [TestMethod]
        public void should_return_tru_for_proxyport()
        {
            input = "8080";
            _proxyValidationHelper.Sender = "txtProxyPort";
            var expected = new ValidationResult(true, null); 
            var result = _proxyValidationHelper.Validate(input, CultureInfo.CurrentCulture);
            result.Should().Be(expected);
        }
    }
}
