using DominatorHouseCore.Converters;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;

namespace DominatorHouseCore.UnitTests.Tests.Converters
{
    [TestClass]
    public class StringLengthToBooleanConverterTest
    {
        StringLengthToBooleanConverter _sut;
        object value;

        [TestInitialize]
        public void Setup()
        {
            _sut = new StringLengthToBooleanConverter();
        }
        [TestMethod]
        public void should_return_true_if_input_any_positive_number()
        {
            value = "8";
            var expected = true;
            var result = _sut.Convert(value, value.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().Be(expected);
        }
        [TestMethod]
        public void should_return_false_if_input_any_negative_number()
        {
            value = "-8";
            var expected = false;
            var result = _sut.Convert(value, value.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().Be(expected);
        }
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void should_throw_if_input_is_null()
        {
            value = null;
            var result = _sut.Convert(value, null, null, CultureInfo.CurrentUICulture);
        }
    }
}
