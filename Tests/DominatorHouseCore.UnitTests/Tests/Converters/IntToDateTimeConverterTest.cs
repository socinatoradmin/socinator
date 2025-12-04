using Dominator.Tests.Utils;
using DominatorHouseCore.Converters;
using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Globalization;
namespace DominatorHouseCore.UnitTests.Tests.Converters
{
    [TestClass]
    public class IntToDateTimeConverterTest : UnityInitializationTests
    {
        IntToDateTimeConverter _sut;
        object value;
        IDateProvider dateProvider;
        [TestInitialize]
        public void SetUp()
        {
            _sut = new IntToDateTimeConverter();
            base.SetUp();
            dateProvider = Substitute.For<IDateProvider>();
        }
        [TestMethod]
        public void should_return_Not_Updated_Yet_if_value_is_zero()
        {
            value = 0;
            var result = _sut.Convert(value, value.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().Be("LangKeyNotUpdatedYet");
        }
        [TestMethod]
        public void should_return_local_datetime_of_value_if_value_is_nonzero()
        {
            value = 1552886966;
            var result = _sut.Convert(value, value.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().Be((new DateTime(2019, 3, 18, 5, 29, 26)).ToString("dd MMM yyyy HH:mm:ss tt"));
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
