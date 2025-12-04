using DominatorHouseCore.Converters;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;

namespace DominatorHouseCore.UnitTests.Tests.Converters
{
    [TestClass]
    public class BooleanToStringSelectConverterTests
    {
        BooleanToStringSelectConverter _sut;
        bool? value;

        [TestInitialize]
        public void Setup()
        {
            _sut = new BooleanToStringSelectConverter();
        }
        [TestMethod]
        public void should_return_SelectAll_if_value_is_true()
        {
            value = true;
            var result = _sut.Convert(value, value.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().Be("SelectAll");
        }
        [TestMethod]
        public void should_return_SelectNone_if_value_is_false()
        {
            value = false;
            var result = _sut.Convert(value, value.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().Be("SelectNone");
        }
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void should_throw_nullrefexception_if_value_is_null()
        {
            value = null;
            var result = _sut.Convert(value, value.GetType(), null, CultureInfo.CurrentUICulture);
        }
    }
}
