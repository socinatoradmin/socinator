#region

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.Converters
{
    public class ImageConverter : IValueConverter
    {
        public object Convert(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            var filePath = ConstantVariable.GetNotFoundImage();
            try
            {
                if (File.Exists(value?.ToString()) || ImageExtracter.IsValidUrl(value?.ToString()))
                    return string.IsNullOrEmpty(value?.ToString())
                        ? new BitmapImage()
                        : new BitmapImage(new Uri(value.ToString()));

                if (!File.Exists(filePath))
                    Utilities.DownloadNotFound();

                var filesize = new FileInfo(filePath).Length;
                if (filesize == 0)
                {
                    File.Delete(filePath);
                    Utilities.DownloadNotFound();
                }

                return new BitmapImage(new Uri(filePath));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return new BitmapImage(new Uri(filePath));
            }
        }

        [ExcludeFromCodeCoverage]
        public object ConvertBack(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}