using DominatorHouseCore.Converters;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Windows;

namespace DominatorHouseCore.UnitTests.Tests.Converters
{
    [TestClass]
    public class IntToVisibilityConverterTest
    {
        IntToVisibilityConverter _sut;
        object input;

        [TestInitialize]
        public void SetUp()
        {
            _sut = new IntToVisibilityConverter();
        }
        [TestMethod]
        public void should_return_Visible_if_input_is_any_number_but_not_minus_one()
        {
            input = 0;
            var expected = Visibility.Visible;
            var result = _sut.Convert(input, input.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().Be(expected);
        }
        [TestMethod]
        public void should_return_Visible_if_input_is_any_string_that_can_cast_to_integer_but_not_minus_one()
        {
            input = "54345";
            var expected = Visibility.Visible;
            var result = _sut.Convert(input, input.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().Be(expected);
        }
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void should_throw_FormatException_if_input_is_any_string_that_can_not_cast_to_integer()
        {
            input = "harsh";
            var result = _sut.Convert(input, input.GetType(), null, CultureInfo.CurrentUICulture);
        }
        [TestMethod]
        public void should_return_Collapsed_if_input_is_minus_one()
        {
            input = "-1";
            var expected = Visibility.Collapsed;
            var result = _sut.Convert(input, input.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().Be(expected);
        }
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void should_throw_NullReferenceException_if_input_null()
        {
            input = null;
            var result = _sut.Convert(input, input?.GetType(), null, CultureInfo.CurrentUICulture);
        }
    }
}
