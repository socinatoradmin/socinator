using DominatorHouseCore.Converters;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;

namespace DominatorHouseCore.UnitTests.Tests.Converters
{
    [TestClass]
    public class IndexToRowNoConverterTest
    {
        IndexToRowNoConverter _sut;
        object value;

        [TestInitialize]
        public void SetUp()
        {
            _sut = new IndexToRowNoConverter();
        }
        [TestMethod]
        public void should_return_value_plush_one_if_value_is_any_integer()
        {
            value = 5;
            var result = _sut.Convert(value, value.GetType(), null, CultureInfo.CurrentUICulture);

            result.Should().Be(6);
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void should_throw_InvalidCastException_if_value_is_any_noninteger()
        {
            value = 5.0;
            var result = _sut.Convert(value, value.GetType(), null, CultureInfo.CurrentUICulture);
        }
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void should_throw_NullReferenceException_if_value_is_null()
        {
            value = null;
            var result = _sut.Convert(value, value.GetType(), null, CultureInfo.CurrentUICulture);
        }
    }
}
