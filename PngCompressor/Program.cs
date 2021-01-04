using nQuant;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace PngCompressor
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string source = args[0];
            var bytes = File.ReadAllBytes(source);

            // First convert image to 32 bit, then reduce to 8bit
            using(var original = GetBitmapFromBytes(bytes))
            using (var cloned32 = new Bitmap(
                original.Width,
                original.Height,
                PixelFormat.Format32bppPArgb))
            {
                using (var graphics = Graphics.FromImage(cloned32))
                {
                    graphics.DrawImage(
                        original,
                        new Rectangle(0,0,cloned32.Width,cloned32.Height));
                }

                using (Image compressedImage = new WuQuantizer().QuantizeImage(cloned32))
                {
                    compressedImage.Save(args[1], ImageFormat.Png);
                }
            }
        }

        private static Bitmap GetBitmapFromBytes(byte[] imageBytes)
        {
            TypeConverter tc = TypeDescriptor.GetConverter(typeof(Bitmap));
            Bitmap img = (Bitmap)tc.ConvertFrom(imageBytes);
            return img;
        }
    }
}
