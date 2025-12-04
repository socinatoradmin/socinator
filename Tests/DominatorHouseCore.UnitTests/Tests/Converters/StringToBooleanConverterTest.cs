using DominatorHouseCore.Converters;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace DominatorHouseCore.UnitTests.Tests.Converters
{
    [TestClass]
    public class StringToBooleanConverterTest
    {
        StringToBooleanConverter _sut;
        object input;

        [TestInitialize]
        public void Setup()
        {
            _sut = new StringToBooleanConverter();
        }
        [TestMethod]
        public void should_return_true_if_input_is_Active()
        {
            input = "Active";
            var expected = true;
            var output = _sut.Convert(input, input.GetType(), null, CultureInfo.CurrentUICulture);
            output.Should().Be(expected);
        }
        [TestMethod]
        public void should_return_false_if_input_is_other_then_Active()
        {
            input = "kumar";
            var expected = false;
            var output = _sut.Convert(input, null, null, CultureInfo.CurrentUICulture);
            output.Should().Be(expected);
        }
        [TestMethod]
        public void should_return_Active_if_input_is_true()
        {
            input = true;
            var expected = "Active";
            var output = _sut.ConvertBack(input, input.GetType(), null, CultureInfo.CurrentUICulture);
            output.Should().Be(expected);
        }
        [TestMethod]
        public void should_return_false_if_input_is_false()
        {
            input = false;
            var expected = "Paused";
            var output = _sut.ConvertBack(input, null, null, CultureInfo.CurrentCulture);
            output.Should().Be(expected);
        }
        [TestMethod]
        public void should_return_null_if_input_is_null()
        {
            input = null;
            object expected = null;
            var output = _sut.ConvertBack(input, null, null, CultureInfo.CurrentUICulture);
            output.Should().Be(expected);
        }
    }
}
