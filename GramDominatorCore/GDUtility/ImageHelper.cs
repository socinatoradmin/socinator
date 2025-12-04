using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher.Settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Models.Publisher;

namespace GramDominatorCore.GDUtility
{
    public  class ImageHelper
    {

        //public static BitmapImage BitmapToImageSource(this Bitmap bitmap)
        //{
        //    using (MemoryStream memoryStream = new MemoryStream())
        //    {
        //        bitmap.Save(memoryStream, ImageFormat.Bmp);
        //        memoryStream.Position = 0L;
        //        BitmapImage bitmapImage = new BitmapImage();
        //        bitmapImage.BeginInit();
        //        bitmapImage.StreamSource = memoryStream;
        //        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        //        bitmapImage.EndInit();
        //        return bitmapImage;
        //    }
        //}

        public static Bitmap CropImageStory(Image source, int x, int y, int width, int height, string filePath)
        {
            int fitCrop ;
            int fitSize;
            int cropSection;
            if (height <= 900)
            {
                cropSection = (1280 - height);
                fitSize = height;
                fitCrop = Convert.ToInt32(cropSection / 1.2);
            }
            else
            {
                cropSection = (1280 - height);
                fitSize = 1280 - cropSection * 2;
                fitCrop = Convert.ToInt32(cropSection * 1.2);
            }

            Rectangle srcRect1 = new Rectangle(0, fitCrop, 720, fitSize);// image size
            Rectangle srcRect2 = new Rectangle(0, 0, width, 1280);//picture box size
            Bitmap bitmap = new Bitmap(720, 1280, PixelFormat.Format24bppRgb);
            bitmap.SetResolution(source.HorizontalResolution, source.VerticalResolution);
            Bitmap getColor = new Bitmap(filePath);
            Color pixelColor = getColor.GetPixel(40, 30);
          //  String htmlColor = ColorTranslator.ToHtml(pixelColor);           
            bitmap.MakeTransparent(pixelColor);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.Clear(pixelColor);             
                graphics.DrawImage(source, srcRect1, srcRect2, GraphicsUnit.Pixel);
            }
            return bitmap;
        }

        public static Bitmap CropImage(Image source, int x, int y, int width, int height, string filePath)
        {
            Rectangle srcRect = new Rectangle(x, y, width, height);
            Bitmap bitmap = new Bitmap(srcRect.Width, srcRect.Height);
            using (Graphics graphics = Graphics.FromImage(bitmap))
                graphics.DrawImage(source, new Rectangle(0, 0, bitmap.Width, bitmap.Height), srcRect, GraphicsUnit.Pixel);
            return bitmap;
        }

        //[return: TupleElementNames(new[] { "length", "width", "height", "type" })]
        //public static ValueTuple<long, int, int, ImageFormats> GetImageDetails(string filename)
        //{
        //    if (!File.Exists(filename))
        //        throw new ArgumentException("File does not exists.");
        //    FileInfo fileInfo = new FileInfo(filename);
        //    using (Image image = Image.FromFile(filename))
        //    {
        //        string str = new ImageFormatConverter().ConvertToString(image.RawFormat);
        //        ImageFormats imageFormat;
        //        if (str != "Jpg" && str != "Jpeg")
        //        {
        //            if (str != "Png")
        //            {
        //                if (str != "Gif")
        //                    throw new ArgumentOutOfRangeException();
        //                imageFormat = ImageFormats.Gif;
        //            }
        //            else
        //                imageFormat = ImageFormats.Png;
        //        }
        //        else
        //            imageFormat = ImageFormats.Jpeg;
        //        return new ValueTuple<long, int, int, ImageFormats>(fileInfo.Length, image.Width, image.Height, imageFormat);
        //    }
        //}

        public static byte[] ImageToByteArray(string imageFile, ImageFormats format)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (Image image = Image.FromFile(imageFile))
                {
                    ImageFormat format1;
                    switch (format)
                    {
                        case ImageFormats.Jpeg:
                            format1 = ImageFormat.Jpeg;
                            break;
                        case ImageFormats.Png:
                            format1 = ImageFormat.Png;
                            break;
                        case ImageFormats.Gif:
                            format1 = ImageFormat.Gif;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    image.Save(memoryStream, format1);
                    return memoryStream.ToArray();
                }
            }
        }

        public static async Task<BitmapImage> LoadImageFromUrlAsync(string url)
        {
            return await Task.Run(() =>
            {
                using (WebClient webClient = new WebClient())
                {
                    byte[] buffer = webClient.DownloadData(url);
                    BitmapImage bitmapImage = new BitmapImage();
                    using (MemoryStream memoryStream = new MemoryStream(buffer))
                    {
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = memoryStream;
                        bitmapImage.EndInit();
                        bitmapImage.Freeze();
                        return bitmapImage;
                    }
                }
            });
        }

        //public static BitmapImage BitmapToImageSource(this Bitmap bitmap)
        //{
        //    using (MemoryStream memoryStream = new MemoryStream())
        //    {
        //        bitmap.Save(memoryStream, ImageFormat.Bmp);
        //        memoryStream.Position = 0L;
        //        BitmapImage bitmapImage = new BitmapImage();
        //        bitmapImage.BeginInit();
        //        bitmapImage.StreamSource = memoryStream;
        //        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        //        bitmapImage.EndInit();
        //        return bitmapImage;
        //    }
        //}

        //public static Bitmap CropImage(Image source, int x, int y, int width, int height)
        //{
        //    Rectangle srcRect = new Rectangle(x, y, width, height);
        //    Bitmap bitmap = new Bitmap(srcRect.Width, srcRect.Height);
        //    using (Graphics graphics = Graphics.FromImage(bitmap))
        //        graphics.DrawImage(source, new Rectangle(0, 0, bitmap.Width, bitmap.Height), srcRect, GraphicsUnit.Pixel);
        //    return bitmap;
        //}

        //[return: TupleElementNames(new[] { "length", "width", "height", "type" })]
        //public static ValueTuple<long, int, int, ImageFormats> GetImageDetails(string filename)
        //{
        //    if (!File.Exists(filename))
        //        throw new ArgumentException("File does not exists.");
        //    FileInfo fileInfo = new FileInfo(filename);
        //    using (Image image = Image.FromFile(filename))
        //    {
        //        string str = new ImageFormatConverter().ConvertToString(image.RawFormat);
        //        ImageFormats imageFormat;
        //        if (str != "Jpg" && str != "Jpeg")
        //        {
        //            if (str != "Png")
        //            {
        //                if (str != "Gif")
        //                    throw new ArgumentOutOfRangeException();
        //                imageFormat = ImageFormats.Gif;
        //            }
        //            else
        //                imageFormat = ImageFormats.Png;
        //        }
        //        else
        //            imageFormat = ImageFormats.Jpeg;
        //        return new ValueTuple<long, int, int, ImageFormats>(fileInfo.Length, image.Width, image.Height, imageFormat);
        //    }
        //}

        //public static byte[] ImageToByteArray(string imageFile, ImageFormats format)
        //{
        //    using (MemoryStream memoryStream = new MemoryStream())
        //    {
        //        using (Image image = Image.FromFile(imageFile))
        //        {
        //            ImageFormat format1;
        //            switch (format)
        //            {
        //                case ImageFormats.Jpeg:
        //                    format1 = ImageFormat.Jpeg;
        //                    break;
        //                case ImageFormats.Png:
        //                    format1 = ImageFormat.Png;
        //                    break;
        //                case ImageFormats.Gif:
        //                    format1 = ImageFormat.Gif;
        //                    break;
        //                default:
        //                    throw new ArgumentOutOfRangeException();
        //            }
        //            image.Save(memoryStream, format1);
        //            return memoryStream.ToArray();
        //        }
        //    }
        //}

        //public static async Task<BitmapImage> LoadImageFromUrlAsync(string url)
        //{
        //    return await Task.Run(() =>
        //    {
        //        using (WebClient webClient = new WebClient())
        //        {
        //            byte[] buffer = webClient.DownloadData(url);
        //            BitmapImage bitmapImage = new BitmapImage();
        //            using (MemoryStream memoryStream = new MemoryStream(buffer))
        //            {
        //                bitmapImage.BeginInit();
        //                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        //                bitmapImage.StreamSource = memoryStream;
        //                bitmapImage.EndInit();
        //                bitmapImage.Freeze();
        //                return bitmapImage;
        //            }
        //        }
        //    });
        //}


        public enum ImageFormats
        {
            Jpeg,
            Png,
            Gif,
        }

        public static string GetPnGtoJpegFormat(string filpath, Image image)
        {
            using (var b = new Bitmap(image.Width, image.Height))
            {
                b.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                using (var g = Graphics.FromImage(b))
                {
                    g.Clear(Color.White);
                    g.DrawImageUnscaled(image, 0, 0);
                }
                b.Save(filpath, ImageFormat.Jpeg);
            }
            return filpath;
        }

        public string ConvertPicIntoActualSize( string filePath, DominatorAccountModel accountModel, GdPostSettings gdPostSettings, ref List<ImagePosition> imagePostion, OtherConfigurationModel otherConfiguration)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath) || filePath.StartsWith("http"))
                    return filePath;
                string path = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Temp\\Socinator\\PostedImage";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string newImagePath = $"{path}\\{Path.GetFileName(filePath)}";
                Image image = Image.FromFile(filePath);
                //return newImagePath;
                if (filePath.Contains(".png"))
                {
                    List<string> filePaths = filePath.Split('.').ToList();
                    string newFilePath = $"{filePaths[0]}.jpeg";
                    GetPnGtoJpegFormat(newFilePath, image);
                    filePath = newFilePath;
                }
                int originalHeight = image.Height;
                int originalWidth = image.Width;
                var newHeight = originalHeight;
                var newWidth = originalWidth;
                float aspectRatio = newWidth / (float)newHeight;
                Bitmap newImage = null;

                //return filePath;
                if (!gdPostSettings.IsPostAsStoryPost)
                {

                    if (!gdPostSettings.IsPostAsStoryPost)
                    {
                        if ((aspectRatio >= 0.8f && aspectRatio <= 1.91f) && (newWidth >= 320 && newWidth <= 1080) && (newHeight >= 566 && newHeight <= 1350))
                            return filePath;

                        if ((aspectRatio >= 0.8f && aspectRatio <= 1.91f) && (newHeight < 2000 && newWidth < 2000))
                            return filePath;
                        //return filePath;

                        if (newHeight <= 566)
                            newHeight = 567;

                        if (originalWidth <= 320)
                            newWidth = 320;

                        if (newHeight >= 1350 && newWidth >= 1080)
                        {
                            newImage = newHeight > newWidth ? CropImage(image, 0, 0, 1080, 1350, filePath) : CropImage(image, 0, 0, 1350, 1080, filePath);
                            newWidth = newImage.Width;
                            newHeight = newImage.Height;
                            newImage = GetImage(newImage, newWidth, newHeight, image);
                        }
                        else if (newHeight >= 1350)
                        {
                            newImage = CropImage(image, 20, 20, newWidth, 1350, filePath);
                            newHeight = newImage.Height;
                            newImage = GetImage(newImage, newWidth, newHeight, image);
                        }
                        else if (newWidth >= 1080)
                        {
                            newImage = CropImage(image, 0, 0, 1080, newHeight, filePath);
                            newWidth = newImage.Width;
                            newImage = GetImage(newImage, newWidth, newHeight, image);
                        }

                        aspectRatio = newWidth / (float)newHeight;
                        if (aspectRatio < 0.8f)
                        {
                            checkRatio:
                            newHeight = newHeight - 54;
                            aspectRatio = (float)newWidth / newHeight;
                            if (aspectRatio < 0.78f)
                            {
                                goto checkRatio;
                            }
                            else
                            {
                                newImage = CropImage(image, 0, 0, newWidth, newHeight, filePath);
                                newWidth = newImage.Width;
                                newImage = GetImage(newImage, newWidth, newHeight, image);
                            }
                        }
                    }
                    else
                    {
                        if (newHeight >= 1280 && newWidth >= 720)
                        {
                            newImage = CropImage(image, 20, 20, 720, 1280, filePath);
                            newWidth = newImage.Width;
                            newHeight = newImage.Height;
                            newImage = GetImage(newImage, newWidth, newHeight, image);
                        }
                        else
                        {
                            newImage = CropImageStory(image, 0, 0, newWidth, newHeight, filePath);
                        }
                    }
                }
                    ImageCodecInfo imageCodecInfo = GetEncoderInfo(ImageFormat.Jpeg);

                    // Create an Encoder object for the Quality parameter.
                    System.Drawing.Imaging.Encoder encoder = Encoder.Quality;

                    // Create an EncoderParameters object. 
                    EncoderParameters encoderParameters = new EncoderParameters(1);

                    // Save the image as a JPEG file with quality level.
                    EncoderParameter encoderParameter = new EncoderParameter(encoder, 1024 * 100);
                    encoderParameters.Param[0] = encoderParameter;
                    if (newImage == null)
                    {
                        newImage = CropImage(image, 0, 0, newWidth, newHeight, filePath);
                        newImage = GetImage(newImage, newWidth, newHeight, image);
                    }
                    newImage.Save(newImagePath, imageCodecInfo, encoderParameters);
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
                    //  ImageConverter imgConverter = new ImageConverter();                                        
                    //if (gdPostSettings.IsPostAsStoryPost && gdPostSettings.IsMentionUser)                  
                
                if (gdPostSettings.IsPostAsStoryPost && gdPostSettings.IsMentionUser)
                {
                    try
                    {
                        string stickersPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Temp\\Socinator\\StickerImage";
                        if (!Directory.Exists(stickersPath))
                            Directory.CreateDirectory(stickersPath);
                        string outputImage = $"{stickersPath}\\{new Random().Next(1, 30)}.jpg";
                        MergeTwoImagesIntoOneImages(newImagePath, outputImage, ref imagePostion, gdPostSettings);
                        newImagePath = outputImage;
                    }
                    catch (Exception)
                    { }
                }
                return newImagePath;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Convert Image Error ==> " + ex.Message);
                return string.Empty;
            }
        }

        private static StickerPostition CreateBitmapImageCheck(string sImageText, string newImagePath)
        {
            Bitmap objBmpImage = new Bitmap(1, 1);
            StickerPostition stickerPosition = new StickerPostition();
            sImageText = "@" + sImageText.ToUpper();
            try
            {
                //  Create your private font collection object.
                PrivateFontCollection pfc = new PrivateFontCollection();
                //My font here is "Digireu.ttf"
                int fontLength = Resources.Language.Aveny_T_Regular.Length;
                // create a buffer to read in to
                byte[] fontData = Resources.Language.Aveny_T_Regular;
                // create an unsafe memory block for the font data
                IntPtr data = Marshal.AllocCoTaskMem(fontLength);
                // copy the bytes to the unsafe memory block
                Marshal.Copy(fontData, 0, data, fontLength);

                // pass the font to the font collection
                pfc.AddMemoryFont(data, fontLength);

                Font objFont = new Font(pfc.Families[0], 36, FontStyle.Regular, GraphicsUnit.Pixel);
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;

                // Create a graphics object to measure the text's width and height.
                Graphics objGraphics = Graphics.FromImage(objBmpImage);
                // This is where the bitmap size is determined.
                int intWidth = (int)objGraphics.MeasureString(sImageText, objFont).Width;
                int intHeight = (int)objGraphics.MeasureString(sImageText, objFont).Height;
                int widhtPoint = Convert.ToInt32(intWidth / 2);
                Point point = new Point(widhtPoint, 25);
                Rectangle rect1 = new Rectangle(0, 0, intWidth, intHeight);
                stickerPosition = new StickerPostition()
                {
                    Width = intWidth,
                    Height = intHeight

                };
                // Create the bmpImage again with the correct size for the text and font.
                objBmpImage = new Bitmap(objBmpImage, new Size(intWidth, intHeight));
                objBmpImage.SetResolution(96, 96);
                // Add the colors to the new bitmap.
                objGraphics = Graphics.FromImage(objBmpImage);
                // Set Background color
                objGraphics.Clear(Color.White);
                objGraphics.SmoothingMode = SmoothingMode.HighQuality;
                objGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                objGraphics.CompositingQuality = CompositingQuality.HighQuality;
                objGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                Color col1 = ColorTranslator.FromHtml("#F57542");
                //Color startingColor = new Color();
               var startingColor = Color.FromArgb(col1.R, col1.G, col1.B);
                Color col2 = ColorTranslator.FromHtml("#FACF60");
               // Color endingColor = new Color();
               var endingColor = Color.FromArgb(col2.R, col2.G, col2.B);

                var startDrawingColor = Color.FromArgb(startingColor.A, startingColor.R, startingColor.G, startingColor.B);
                var endDrawingColor = Color.FromArgb(endingColor.A, endingColor.R, endingColor.G, endingColor.B);
                var lgBrush = new LinearGradientBrush(rect1, startDrawingColor, endDrawingColor, LinearGradientMode.Horizontal);

                objGraphics.DrawString(sImageText, objFont, lgBrush, point, stringFormat);
                                                                                          
                objGraphics.Flush();

                Image roundedImage = RoundCorners(objBmpImage, 15, Color.Transparent);

                roundedImage.Save(newImagePath);
                roundedImage.Dispose();
                objBmpImage.Dispose();
                objGraphics.Dispose();
                
            }
            catch (Exception )
            {//Ignored
            }
            return stickerPosition;
        }

        private static void MergeTwoImagesIntoOneImages(string originalImages, string stickerOutputImage, ref List<ImagePosition> imagePostion, GdPostSettings gdPostSettings)
        {
            string stickersPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Temp\\Socinator\\StickerImage";
            if (!Directory.Exists(stickersPath))
                Directory.CreateDirectory(stickersPath);
            List<int> seperatePositionForSticker = new List<int>();
            List<string> lstStickerImagePath = new List<string>();
            StickerPostition stickerPosition = new StickerPostition();
            List<StickerPostition> lstStickerPosition = new List<StickerPostition>();

          // var objAddUpdateAccountControl =new StickerPostion();
            Image originalImage = Image.FromFile(originalImages);

            List<string> lstMention = Regex.Split(gdPostSettings.MentionUserList, "\n")
                       .Where(user => !string.IsNullOrEmpty(user)).Select(user => user.Trim()).ToList();


            for (int user = 0; user < lstMention.Count; user++)
            {
                string stickerImagePath = $"{stickersPath}\\sticker{new Random().Next(1, 1000)}.jpg";
                if (File.Exists(stickerImagePath))
                    File.Delete(stickerImagePath);

                stickerPosition = CreateBitmapImageCheck(lstMention[user], stickerImagePath);
                lstStickerImagePath.Add(stickerImagePath);
                lstStickerPosition.Add(stickerPosition);
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
            }
            for (int sticker = 0; sticker < lstStickerImagePath.Count; sticker++)
            {
                Image stickerImage = Image.FromFile(lstStickerImagePath[sticker]);
                string stickerHeight = lstStickerPosition[sticker].Height.ToString();
                string stickerWidth = lstStickerPosition[sticker].Width.ToString();
                float height = float.Parse(stickerHeight);//
                float width = float.Parse(stickerWidth) + (float.Parse(stickerWidth) / 1.5f);//new Random().Next(10, originalImage.Height);
                int x;
                int y;
                int imageWidthCheck;
                int imageHeightCheck;
                do
                {
                    x = GdUtilities.GetRandomNumber(3);
                    imageWidthCheck = originalImage.Width - Convert.ToInt32(stickerWidth) * 2;
                }
                while (x > imageWidthCheck);

                do
                {
                    modify:
                    y = GdUtilities.GetRandomNumber(3);
                    for (int i = 0; i < seperatePositionForSticker.Count; i++)
                    {
                        int sposition1 = seperatePositionForSticker[i];
                        int sposition2 = seperatePositionForSticker[i] + 70;
                        if (y >= sposition1 && y <= sposition2)
                        {
                            goto modify;
                        }

                    }

                    imageHeightCheck = originalImage.Height - Convert.ToInt32(stickerHeight) * 2;
                }
                while (y > imageHeightCheck);
                int xValue = Convert.ToInt32(x);
                int yValue = Convert.ToInt32(y);
                ImagePosition imagePosition = new ImagePosition()
                {
                    XValue = xValue.ToString(),
                    Yvalue = yValue.ToString(),
                    Width = width.ToString(CultureInfo.InvariantCulture),
                    Height = height.ToString(CultureInfo.InvariantCulture)
                };
                seperatePositionForSticker.Add(yValue);
                imagePostion.Add(imagePosition);
                Graphics gra = Graphics.FromImage(originalImage);
                Bitmap smallImg = new Bitmap(stickerImage, new Size(Convert.ToInt32(stickerWidth) * 2, Convert.ToInt32(stickerHeight) * 2));
                gra.DrawImage(smallImg, new Point(xValue, yValue));
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
                smallImg.Dispose();
                gra.Dispose();            
                stickerImage.Dispose();             
            }
            originalImage.Save(stickerOutputImage, System.Drawing.Imaging.ImageFormat.Jpeg);
            originalImage.Dispose();
            
            DeleteStickerFromPath(lstStickerImagePath);
        }

        public static ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == format.Guid);
        }

        public static Image RoundCorners(Image startImage, int cornerRadius, Color backgroundColor)
        {
            cornerRadius *= 2;
            Bitmap roundedImage = new Bitmap(startImage.Width, startImage.Height);
            Graphics g = Graphics.FromImage(roundedImage);
            g.Clear(backgroundColor);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Brush brush = new TextureBrush(startImage);
            GraphicsPath gp = new GraphicsPath();
            gp.AddArc(0, 0, cornerRadius, cornerRadius, 180, 90);
            gp.AddArc(0 + roundedImage.Width - cornerRadius, 0, cornerRadius, cornerRadius, 270, 90);
            gp.AddArc(0 + roundedImage.Width - cornerRadius, 0 + roundedImage.Height - cornerRadius, cornerRadius, cornerRadius, 0, 90);
            gp.AddArc(0, 0 + roundedImage.Height - cornerRadius, cornerRadius, cornerRadius, 90, 90);
            g.FillPath(brush, gp);
            return roundedImage;
        }

        public static Bitmap GetImage([NotNull] Bitmap newImage, int newWidth, int newHeight, Image image)
        {
            if (newImage == null) throw new ArgumentNullException(nameof(newImage));
            newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);

            // Draws the image in the specified size with quality mode set to HighQuality
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }

        public static void DeleteStickerFromPath(List<string> lstStickerImagePath)
        {
            try
            {
                for (int i = 0; i < lstStickerImagePath.Count(); i++)
                {
                    if (File.Exists(lstStickerImagePath[i]))
                    {
                        var newStreamReader = new StreamReader(File.OpenRead(lstStickerImagePath[i]));
                        newStreamReader.Close();
                        newStreamReader.Dispose();
                        File.Delete(lstStickerImagePath[i]);
                    }
                }
            }
            catch (Exception )
            {
                //ignored
            }
        }

    }

    public class ImagePosition
    {
        public string Width { get; set; }
        public string Height { get; set; }
        public string XValue { get; set; }
        public string Yvalue { get; set; }
    }
    public class StickerPostition
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
}