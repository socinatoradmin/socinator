using DominatorHouseCore.Converters;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace DominatorHouseCore.UnitTests.Tests.Converters
{
    [TestClass]
    public class AverageConverterTests
    {
        AverageConverter _sut;
        object[] values;

        [TestInitialize]
        public void Setup()
        {
            _sut = new AverageConverter();
        }
        [TestMethod]
        public void should_return_average_of_number()
        {
            values = new object[] { 1, 2, 3, 4, 5 };
            var result = _sut.Convert(values, values.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().Be(15 / 5);
        }
        [TestMethod]
        public void should_return_zero_if_input_is_not_number()
        {
            values = new object[] { '1', "harsh", "kumar"};
            var result = _sut.Convert(values, values.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().Be(0);
        }
        [TestMethod]
        public void should_return_zero_if_input_is_null()
        {
            values = new object[] { null };
            var result = _sut.Convert(values, values.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().Be(0);
        }
    }
}
