using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Windows.Controls;

namespace DominatorHouseCore.UnitTests.Tests.FileManagers
{
    [TestClass]
    public class UrlValidatorTest
    {
        UrlValidator _urlValidator;
        string input;
        [TestInitialize]
        public void StartUp()
        {
            _urlValidator = new UrlValidator();
        }
        [TestMethod]
        public void should_return_true_if_input_is_valid_url()
        {
            input = "www.google.com";
            var expected = new ValidationResult(true, string.Empty);
            var result = _urlValidator.Validate(input, CultureInfo.CurrentCulture);
            result.Should().Be(expected);
        }
        [TestMethod]
        public void should_return_Invalid_URL_if_input_is_invalid_url()
        {
            input = "www.com";
            var expected = new ValidationResult(false, "Invalid URL");
            var result = _urlValidator.Validate(input, CultureInfo.CurrentCulture);
            result.Should().Be(expected);
        }
        [TestMethod]
        public void should_return_Invalid_URL_if_input_is_null()
        {
            input = null;
            var expected = new ValidationResult(false, "LangKeyInvalidURL");
            var result = _urlValidator.Validate(input, CultureInfo.CurrentCulture);
            result.Should().Be(expected);
        }
    }
}
