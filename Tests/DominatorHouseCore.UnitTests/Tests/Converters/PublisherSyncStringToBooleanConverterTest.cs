using DominatorHouseCore.Converters;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace DominatorHouseCore.UnitTests.Tests.Converters
{
    [TestClass]
    public class PublisherSyncStringToBooleanConverterTest
    {
        PublisherSyncStringToBooleanConverter _sut;
        object value;

        [TestInitialize]
        public void SetUp()
        {
            _sut = new PublisherSyncStringToBooleanConverter();
        }
        [TestMethod]
        public void should_return_true_if_value_is_Click_to_Sync()
        {
            value = "Click to Sync";
            var expacted = true;
            var result = _sut.Convert(value, null, null, CultureInfo.CurrentUICulture);
            result.Should().Be(expacted);
        }
        [TestMethod]
        public void should_return_false_if_value_is_other_than_Click_to_Sync()
        {
            value = "Welcome to Socinator";
            var expacted = false;
            var result = _sut.Convert(value, null, null, CultureInfo.CurrentUICulture);
            result.Should().Be(expacted);
        }
        [TestMethod]
        public void should_return_false_if_value_is_null()
        {
            value = null;
            var expacted = false;
            var result = _sut.Convert(value, null, null, CultureInfo.CurrentUICulture);
            result.Should().Be(expacted);
        }
    }
}
