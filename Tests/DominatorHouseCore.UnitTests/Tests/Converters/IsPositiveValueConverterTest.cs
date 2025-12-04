using DominatorHouseCore.Converters;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace DominatorHouseCore.UnitTests.Tests.Converters
{
    [TestClass]
    public class IsPositiveValueConverterTest
    {
        IsPositiveValueConvertor _sut;
        object input;

        [TestInitialize]
        public void SetUp()
        {
            _sut = new IsPositiveValueConvertor();
        }
        [TestMethod]
        public void should_return_true_if_input_is_positive_number()
        {
            input = 34;
            var expected = true;
            var result = _sut.Convert(input, input.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().Be(expected);
        }
        [TestMethod]
        public void should_return_false_if_input_is_negative_number()
        {
            input = -34;
            var expected = false;
            var result = _sut.Convert(input, input.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().Be(expected);
        }
        [TestMethod]
        public void should_return_false_if_input_is_zero()
        {
            input = 0;
            var expected = false;
            var result = _sut.Convert(input, input.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().Be(expected);
        }
        [TestMethod]
        public void should_return_false_if_input_is_null()
        {
            input = null;
            var expected = false;
            var result = _sut.Convert(input, input?.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().Be(expected);
        }
    }
}
