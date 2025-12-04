using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DominatorHouseCore.UnitTests.Tests.FileManagers
{
    [TestClass]
    public class RangeUtilitiesTest
    {
        RangeUtilities _rangeUtilities;
        int start, end;
        [TestMethod]
        public void should_return_random_number_between_start_and_end_value()
        {
            start = 0;
            end = 100000;
            _rangeUtilities = new RangeUtilities(start, end);
            var result = _rangeUtilities.GetRandom();
            result.Should().BeLessOrEqualTo(end);
            result.Should().BeGreaterOrEqualTo(start);

        }
        [TestMethod]
        public void should_return_start_or_end_value_if_both_are_same()
        {
            start = 10;
            end = 10;
            _rangeUtilities = new RangeUtilities(start, end);
            var result = _rangeUtilities.GetRandom();
            result.Should().Be(end);
            result.Should().Be(start);

        }
        [TestMethod]
        public void should_return_zero_if_no_values_are_assign_for_start_and_end()
        {
            _rangeUtilities = new RangeUtilities(start, end);
            var result = _rangeUtilities.GetRandom();
            result.Should().Be(0);
        }
        [TestMethod]
        public void should_true_if_input_is_in_range()
        {
            start = 10;
            end = 15;
            _rangeUtilities = new RangeUtilities(start, end);
            var input = 12;
            var result = _rangeUtilities.InRange(input);
            result.Should().Be(true);
        }
        [TestMethod]
        public void should_false_if_input_is_not_in_range()
        {
            start = 10;
            end = 15;
            _rangeUtilities = new RangeUtilities(start, end);
            var input = 102;
            var result = _rangeUtilities.InRange(input);
            result.Should().Be(false);
        }
        [TestMethod]
        public void should_return_false_if_no_values_are_assign_for_start_and_end()
        {
            _rangeUtilities = new RangeUtilities();
            var input = 102;
            var result = _rangeUtilities.InRange(input);
            result.Should().Be(false);
        }
    }
}
