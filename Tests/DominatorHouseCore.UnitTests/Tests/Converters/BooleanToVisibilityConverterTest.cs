using DominatorHouseCore.Converters;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Windows;

namespace DominatorHouseCore.UnitTests.Tests.Converters
{
    [TestClass]
    public class BooleanToVisibilityConverterTest
    {
        BooleanToVisibilityConverter _sut;
        bool? value;

        [TestInitialize]
        public void Setup()
        {
            _sut = new BooleanToVisibilityConverter();
        }
        [TestMethod]
        public void should_return_Visible_if_value_is_true()
        {
            value = true;
            var result = _sut.Convert(value, value.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().Be(Visibility.Visible);
        }
        [TestMethod]
        public void should_return_Collapsed_if_value_is_false()
        {
            value = false;
            var result = _sut.Convert(value, value.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().Be(Visibility.Collapsed);
        }
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void should_throw_nullrefexception_if_value_is_null()
        {
            value = null;
            var result = _sut.Convert(value, value.GetType(), null, CultureInfo.CurrentUICulture);
        }
        [TestMethod]
        public void should_return_Visible_if_value_is_false_and_IsInversed_is_true()
        {
            value = false;
            _sut.IsInversed = true;
            var result = _sut.Convert(value, value.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().Be(Visibility.Visible);
        }
        [TestMethod]
        public void should_return_Collapsed_if_value_is_true_and_IsInversed_is_true()
        {
            value = true;
            _sut.IsInversed = true;
            var result = _sut.Convert(value, value.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().Be(Visibility.Collapsed);
        }
    }
}
