using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;

namespace ImageFinder
{
    public class ImageConverter
    {
        public static void Convert(byte[] bytes, int maxWidth, int maxHeight, int quality, string fileName, string ext, int? skipWidth)
        {
            Bitmap image = GetBitmapFromBytes(bytes);

            // Get the image's original width and height
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            // To preserve the aspect ratio
            float ratioX = (float)maxWidth / (float)originalWidth;
            float ratioY = (float)maxHeight / (float)originalHeight;
            float ratio = Math.Min(ratioX, ratioY);

            // Don't enlarge small images or images that are to scale.
            if (ratio >= 1)
            {
                ratio = 1;
            }

            // Special case for banner images with a certain width
            if (skipWidth.HasValue && originalWidth == skipWidth)
            {
                ratio = 1;
            }

            // New width and height based on aspect ratio
            int newWidth = (int)(originalWidth * ratio);
            int newHeight = (int)(originalHeight * ratio);

            // Convert other formats (including CMYK) to RGB.
            Bitmap newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);

            // Draws the image in the specified size with qualitymode set to high
            using(var graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            ImageCodecInfo imageCodecInfo = null;
            if (ext == ".jpg" || ext == ".jpeg")
            {
                imageCodecInfo = GetEncoderInfo(ImageFormat.Jpeg);
            }
            else if (ext == ".png")
            {
                imageCodecInfo = GetEncoderInfo(ImageFormat.Png);
            }
            else if (ext == ".gif")
            {
                imageCodecInfo = GetEncoderInfo(ImageFormat.Gif);
            }

            // Create an Encoder object for the Quality parameter.
            Encoder encoder = Encoder.Quality;

            // Create an EncoderParameters object. 
            EncoderParameters encoderParameters = new EncoderParameters(1);

            // Save the image as a JPEG file with quality level.
            EncoderParameter encoderParameter = new EncoderParameter(encoder, quality);
            encoderParameters.Param[0] = encoderParameter;
            newImage.Save(fileName, imageCodecInfo, encoderParameters);    
            
            // Clean up bitmaps immediately to stop exhausting system memory
            image.Dispose();
            newImage.Dispose();
        }

        private static Bitmap GetBitmapFromBytes(byte[] imageBytes)
        {
            TypeConverter tc = TypeDescriptor.GetConverter(typeof(Bitmap));
            Bitmap img = (Bitmap) tc.ConvertFrom(imageBytes);
            return img;
        }

        private static ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == format.Guid);
        }
    }
}