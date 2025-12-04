using DominatorHouseCore.Converters;
using DominatorHouseCore.Enums;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DominatorHouseCore.UnitTests.Tests.Converters
{
    [TestClass]
    public class ActivityTypesSourceConverterTests
    {
        ActivityTypesSourceConverter _sut;
        List<ActivityType?> LstActivityType;
        object[] values;

        [TestInitialize]
        public void Setup()
        {
            _sut = new ActivityTypesSourceConverter();
            LstActivityType = Enum.GetValues(typeof(ActivityType)).OfType<ActivityType?>().ToList();
        }
        [TestMethod]
        public void should_return_collection_of_all_network_Activities_if_input_contains_list_and_network_is_social()
        {
            values = new object[] { LstActivityType, SocialNetworks.Social };
            var result = (IEnumerable<ActivityType?>)_sut.Convert(values, values.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().NotBeEmpty().And.HaveCount(LstActivityType.Count);
        }
        [TestMethod]
        public void should_return_quora_acitivity_if_input_contain_list_of_activityType_and_network_is_Quora()
        {
            values = new object[] { LstActivityType, SocialNetworks.Quora };
            var result = (IEnumerable<ActivityType?>)_sut.Convert(values, values.GetType(), null, CultureInfo.CurrentUICulture);
            //result count should be 13 because Quora contains 13 Activities
            result.Should().NotBeEmpty().And.HaveCount(14);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void should_throw_ArgumentNullException_if_input_null_and_any_network()
        {
            values = new object[] { null, SocialNetworks.Quora };
            var result = (IEnumerable<ActivityType?>)_sut.Convert(values, values.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().NotBeEmpty();
        }
    }
}
