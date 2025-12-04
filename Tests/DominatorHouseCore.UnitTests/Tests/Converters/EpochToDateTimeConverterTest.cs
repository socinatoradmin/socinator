using Dominator.Tests.Utils;
using DominatorHouseCore.Converters;
using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Globalization;
using Unity;

namespace DominatorHouseCore.UnitTests.Tests.Converters
{
    [TestClass]
    public class EpochToDateTimeConverterTest : UnityInitializationTests
    {
        EpochToDateTimeConverter _sut;
        object value;
        IDateProvider dateProvider;

        [TestInitialize]
        public void SetUp()
        {
            base.SetUp();
            _sut = new EpochToDateTimeConverter();
            dateProvider = Substitute.For<IDateProvider>();
            Container.RegisterInstance(dateProvider);
        }
        [TestMethod]
        public void should_return_local_datetime_if_input_length_is_ten()
        {
            value = 1552886966;
            var result = _sut.Convert(value, value.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().Be(new DateTime(2019, 3, 18, 5, 29, 26));
        }
        [TestMethod]
        public void should_return_utc_datetime_if_input_length_greater_than_ten()
        {
            value = 15528869665;
            var result = _sut.Convert(value, value.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().Be(new DateTime(1970, 6, 29, 17, 34, 29, 665));
        }
        [TestMethod]
        public void should_return_input_value_if_input_length_less_than_ten()
        {
            value = 155288699;
            var result = _sut.Convert(value, value.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().Be(value);
        }
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void should_throw_NullReferenceException_if_input_is_null()
        {
            value = null;
            var result = _sut.Convert(value, value.GetType(), null, CultureInfo.CurrentUICulture);
        }
    }
}