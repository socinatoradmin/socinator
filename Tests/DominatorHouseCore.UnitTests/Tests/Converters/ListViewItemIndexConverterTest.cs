using DominatorHouseCore.Converters;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Windows.Controls;

namespace DominatorHouseCore.UnitTests.Tests.Converters
{
    [TestClass]
    public class ListViewItemIndexConverterTest
    {
        ListViewItemIndexConverter _sut;
        object value;

        [TestInitialize]
        public void SetUp()
        {
            _sut = new ListViewItemIndexConverter();
        }
        [TestMethod]
        public void should_return_zero_if_value_is_ListViewItem()
        {
            var listview = new ListView();
            value = new ListViewItem();
            listview.Items.Add(value);

            var expacted = "0";
            var result = _sut.Convert(value, null, null, CultureInfo.CurrentUICulture);

            result.Should().Be(expacted);
        }
        [TestMethod]
        public void should_return_one_if_value_is_not_ListViewItem()
        {
            value = "listviewitem";
            var expacted = "1";
            var result = _sut.Convert(value, null, null, CultureInfo.CurrentUICulture);

            result.Should().Be(expacted);
        }
        [TestMethod]
        public void should_return_one_if_value_is_null()
        {
            value = null;
            var expacted = "1";
            var result = _sut.Convert(value, null, null, CultureInfo.CurrentUICulture);

            result.Should().Be(expacted);
        }
    }
}
