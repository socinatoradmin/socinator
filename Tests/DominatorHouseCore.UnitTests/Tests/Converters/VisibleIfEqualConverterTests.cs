using DominatorHouseCore.Converters;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Windows;

namespace DominatorHouseCore.UnitTests.Tests.Converters
{
    [TestClass]
    public class VisibleIfEqualConverterTests
    {
        private VisibleIfEqualConverter _sut;

        [TestInitialize]
        public void SetUp()
        {
            _sut = new VisibleIfEqualConverter();
        }

        [TestMethod]
        public void should_return_Visible_if_input_equal_to_expected()
        {
            // arrange
            var inputValue = 1;
            _sut.Expected = 1;

            // act
            var result = (Visibility)_sut.Convert(inputValue, inputValue.GetType(), null, CultureInfo.CurrentUICulture);

            // assert
            result.Should().Be(Visibility.Visible);
        }

        [TestMethod]
        public void should_return_Collapsed_if_input_NOT_equal_to_expected()
        {
            // arrange
            var inputValue = 1;
            _sut.Expected = 2;

            // act
            var result = (Visibility)_sut.Convert(inputValue, inputValue.GetType(), null, CultureInfo.CurrentUICulture);

            // assert
            result.Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void should_return_Collapsed_if_input_equal_to_expected_and_Inversed()
        {
            // arrange
            var inputValue = 1;
            _sut.Expected = 1;
            _sut.Inversed = true;

            // act
            var result = (Visibility)_sut.Convert(inputValue, inputValue.GetType(), null, CultureInfo.CurrentUICulture);

            // assert
            result.Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void should_return_Visible_if_input_NOT_equal_to_expected_and_Inversed()
        {
            // arrange
            var inputValue = 1;
            _sut.Expected = 2;
            _sut.Inversed = true;

            // act
            var result = (Visibility)_sut.Convert(inputValue, inputValue.GetType(), null, CultureInfo.CurrentUICulture);

            // assert
            result.Should().Be(Visibility.Visible);
        }
    }
}
