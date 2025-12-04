using DominatorHouseCore.Converters;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
namespace DominatorHouseCore.UnitTests.Tests.Converters
{
    [TestClass]
    public class ListCountVisiblityConverterTest
    {
        ListCountVisiblityConverter _sut;
        object input;

        [TestInitialize]
        public void SetUp()
        {
            _sut = new ListCountVisiblityConverter();
        }
        [TestMethod]
        public void should_return_Visible_if_input_contains_enumerable_data()
        {
            input = new List<string>
            {
               "Kumar","Harsh"
            };
            var expected = Visibility.Visible;
            var result = _sut.Convert(input, null, null, CultureInfo.CurrentUICulture);
            result.Should().Be(expected);
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void should_throw_InvalidCastException_if_input_contains_non_enumerable_data()
        {
            input = 5;
            var result = _sut.Convert(input, null, null, CultureInfo.CurrentUICulture);
        }
        [TestMethod]
        public void should_return_Collapsed_if_input_is_null()
        {
            input = null;
            var expected = Visibility.Collapsed;
            var result = _sut.Convert(input, null, null, CultureInfo.CurrentUICulture);
            result.Should().Be(expected);
        }
    }
}
