using DominatorHouseCore.Converters;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Windows;

namespace DominatorHouseCore.UnitTests.Tests.Converters
{
    [TestClass]
    public class FeatureVisibilityConverterTests
    {
        FeatureVisibilityConverter _sut;
        object value;

        [TestInitialize]
        public void SetUp()
        {
            _sut = new FeatureVisibilityConverter();
            FeatureFlags.Instance = new FeatureFlags();
            FeatureFlags.Instance.Add(SocialNetworks.Quora.ToString(), true);
        }
        [TestMethod]
        public void should_return_Visible_if_input_is_Quora()
        {
            value = SocialNetworks.Quora;
            var result = _sut.Convert(value, value.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().Be(Visibility.Visible);
        }
        [TestMethod]
        public void should_return_Collapsed_if_input_is_Facebook()
        {
            value = SocialNetworks.Facebook;
            var result = _sut.Convert(value, value.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().Be(Visibility.Collapsed);
        }
        [TestMethod]
        public void should_return_Collapsed_if_input_is_integer()
        {
            value = 123;
            var result = _sut.Convert(value, value.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().Be(Visibility.Collapsed);
        }
        [TestMethod]
        public void should_return_Collapsed_if_input_is_not_integer_nor_any_network()
        {
            value = "abc";
            var result = _sut.Convert(value, value.GetType(), null, CultureInfo.CurrentUICulture);

            result.Should().Be(Visibility.Collapsed);
        }
    }
}
