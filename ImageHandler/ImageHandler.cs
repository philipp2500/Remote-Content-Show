namespace ImageHandler
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Windows.Media.Imaging;

    public static class ImageHandler
    {

        public static bool AreEqual(Image bitmap1, Image bitmap2)
        {
            bool equal = GetHashFromImage(bitmap1).SequenceEqual(GetHashFromImage(bitmap2));

            return equal;
        }

        public static Image Resize(Image image, Size size)
        {
            return (Image)(new Bitmap(image, size));
        }

        /*public static byte[] GetHashFromImage(BitmapImage image)
        {
            using (var md5 = MD5.Create())
            {
                byte[] data;

                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                using (MemoryStream ms = new MemoryStream())
                {
                    encoder.Save(ms);
                    data = ms.ToArray();
                }

                return md5.ComputeHash(data);
            }
        }*/

        public static byte[] GetHashFromImage(Image image)
        {
            using (var md5 = MD5.Create())
            {
                byte[] data;

                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, ImageFormat.Bmp);

                    data = m.ToArray();
                }

                return data;

                //return md5.ComputeHash(data);
            }
        }
        
        public static byte[] ImageToBytes(Image img)
        {
            byte[] bytes = null;

            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, ImageFormat.Png);
                stream.Close();

                bytes = stream.ToArray();
            }

            return bytes;
        }

        public static BitmapImage BytesToImage(byte[] data)
        {
            BitmapImage bmp = new BitmapImage();

            using (var memory = new MemoryStream(data))
            {
                memory.Position = 0;
                bmp.BeginInit();
                bmp.StreamSource = memory;
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.EndInit();
            }

            return bmp;
        }

        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        public static Bitmap BitmapImageToBitmap(BitmapImage image)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(image));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }
    }
}
