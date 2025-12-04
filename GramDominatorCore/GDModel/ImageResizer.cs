namespace GramDominatorCore.GDModel
{
    public static class ImageResizer
    {
        //public const int MaxHeight = 1350;
        //public const float MaxRatio = 1.91f;
        //public const int MaxWidth = 1080;
        //public const float MinRatio = 0.8f;

        //public static PhotoDetails FixImage(string file)
        //{
        //    ValueTuple<long, int, int, ImageHelper.ImageFormat> imageDetails1 = ImageHelper.GetImageDetails(file);
        //    try
        //    {
        //        return new PhotoDetails((int)imageDetails1.Item3, (int)imageDetails1.Item2, file, (long)imageDetails1.Item1, (ImageHelper.ImageFormat)imageDetails1.Item4);
        //    }
        //    catch
        //    {
        //        ValueTuple<int, int, double> downscaledRatio = ImageResizer.GetDownscaledRatio((int)imageDetails1.Item2, (int)imageDetails1.Item3, 1080, 1350);
        //        ValueTuple<int, int> modifiedRatioSize = ImageResizer.GetModifiedRatioSize((int)downscaledRatio.Item1, (int)downscaledRatio.Item2, 0.800000011920929, 1.9099999666214);
        //        using (MemoryStream memoryStream = new MemoryStream(File.ReadAllBytes(file)))
        //        {
        //            //using (ImageFactory imageFactory = new ImageFactory(false))
        //            //{
        //            //    imageFactory.Load((Stream)memoryStream).Format((ISupportedImageFormat)new JpegFormat()).Resize(new ResizeLayer(new Size((int)downscaledRatio.Item1, (int)downscaledRatio.Item2), (ResizeMode)2, (AnchorPosition)0, true, (float[])null, new Size?(), (List<Size>)null, new Point?())).Crop(new CropLayer(0.0f, 0.0f, (float)modifiedRatioSize.Item1, (float)modifiedRatioSize.Item2, (CropMode)0)).Save(file);
        //            //    ValueTuple<long, int, int, ImageHelper.ImageFormat> imageDetails2 = ImageHelper.GetImageDetails(file);
        //            //    return new PhotoDetails((int)imageDetails2.Item3, (int)imageDetails2.Item2, file, (long)imageDetails2.Item1, (ImageHelper.ImageFormat)imageDetails2.Item4);
        //            //}
        //        }
        //    }
        //}

        //[return: TupleElementNames(new[] { "width", "height", "ratioChange" })]
        //public static ValueTuple<int, int, double> GetDownscaledRatio(int width, int height, int maxWidth, int maxHeight)
        //{
        //    int num1 = width;
        //    int num2 = height;
        //    double num3 = 1.0;
        //    if (height > maxHeight || width > maxWidth)
        //    {
        //        num3 = Math.Min(Math.Round(maxWidth / (double)width, 2), Math.Round(maxHeight / (double)height, 2));
        //        num2 = (int)Math.Ceiling(height * num3);
        //        num1 = (int)Math.Ceiling(width * num3);
        //    }
        //    return new ValueTuple<int, int, double>(num1, num2, num3);
        //}

        public static void IsValidImage(PhotoDetails details)
        {
            //if (details.Type != null)
            //    throw new ArgumentException(Resources.ImageResizer_IsValidImage_Image_is_not_of_JPEG_format_);
            //if (details.Width > 1080 || details.Height > 1350)
            //    throw new ArgumentException(Resources.ImageResizer_IsValidImage_Image_exceeds_maximum_allowed_resolution_);
            //float num = (float)details.Width / (float)details.Height;
            //if ((double)num < 0.800000011920929 || (double)num > 1.9099999666214)
            //    throw new ArgumentException(Resources.ImageResizer_IsValidImage_Image_exceeds_allowed_ratio_);
        }

       // [return: TupleElementNames(new[] { "width", "height" })]
        //private static ValueTuple<int, int> GetModifiedRatioSize(int width, int height, double minRatio, double maxRatio)
        //{
        //    int num1 = width / height;
        //    if (num1 < minRatio)
        //    {
        //        double num2 = width / (height * minRatio);
        //        return new ValueTuple<int, int>(width, (int)Math.Ceiling(height * num2));
        //    }
        //    if (num1 <= maxRatio)
        //        return new ValueTuple<int, int>(width, height);
        //    double num3 = height / (width * minRatio);
        //    return new ValueTuple<int, int>((int)Math.Floor(width * num3), height);
        //}
    }
}