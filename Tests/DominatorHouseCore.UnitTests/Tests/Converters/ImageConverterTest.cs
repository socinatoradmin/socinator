using DominatorHouseCore.Converters;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace DominatorHouseCore.UnitTests.Tests.Converters
{
    [TestClass]
    public class ImageConverterTest
    {
        ImageConverter _sut;


        [TestInitialize]
        public void SetUp()
        {
            _sut = new ImageConverter();
        }
        [TestMethod]
        public void should_return_BitmapImage_if_value_is_path_of_file_which_is_present_in_system()
        {
            var value = @"someimage.png";
            using (var stream = this.GetType().Assembly.GetManifestResourceStream("DominatorHouseCore.UnitTests.TestData.someimage.png"))
            {
                using (var file = File.Create("someimage.png"))
                {
                    stream.CopyTo(file);
                    file.Flush();
                }
            }
            var path = new FileInfo(value);

            var result = (BitmapImage)_sut.Convert(path.FullName, value.GetType(), null, CultureInfo.CurrentUICulture);
            var expected = new Uri(path.FullName.ToString());
            result.UriSource.AbsolutePath.Should().Be(expected.AbsolutePath);
        }
        [TestMethod]
        public void should_return_NotFound_Image_if_path_is_not_present_in_system()
        {
            var value = string.Empty;
            var result = (BitmapImage)_sut.Convert(value, value.GetType(), null, CultureInfo.CurrentUICulture);
            result.UriSource.AbsolutePath.Should().EndWith(@"/NotFoundImage.png");
        }
    }
}
