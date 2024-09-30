using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace ShellPreviewer.Helpers
{
    public static class ImageOperations
    {
        #region Methods

        #region Public
        public static Bitmap? GetBitmapFromHBitmap(nint hBmp)
        {
            try
            {
                Bitmap bmp = Image.FromHbitmap(hBmp);
                if (Image.GetPixelFormatSize(bmp.PixelFormat) < 32)
                {
                    return bmp;
                }

                Rectangle bmpRect = new(0, 0, bmp.Width, bmp.Height);
                BitmapData bmpData = bmp.LockBits(bmpRect, ImageLockMode.ReadOnly, bmp.PixelFormat);
                if (IsAlphaBitmap(bmpData))
                {
                    Bitmap aplha = GetAlphaBitmap(bmpData);
                    bmp.UnlockBits(bmpData);
                    bmp.Dispose();

                    return aplha;
                }

                bmp.UnlockBits(bmpData);

                return bmp;
            }
            catch { }

            NativeMethods.DeleteObject(hBmp);

            return null;
        }

        public static BitmapImage? BitmapToBitmapImage(Bitmap bmp)
        {
            MemoryStream stream = new();
            bmp.Save(stream, ImageFormat.Png);
            stream.Seek(0, SeekOrigin.Begin);

            BitmapImage bmpImage = new();
            bmpImage.BeginInit();
            bmpImage.CacheOption = BitmapCacheOption.OnLoad;
            bmpImage.StreamSource = stream;
            bmpImage.EndInit();

            stream.Dispose();
            bmp.Dispose();

            return bmpImage;
        }

        public static Bitmap GetAlphaBitmap(BitmapData bmpData)
        {
            Bitmap tmp = new(bmpData.Width, bmpData.Height, bmpData.Stride, PixelFormat.Format32bppArgb, bmpData.Scan0),
                   clone = new(tmp.Width, tmp.Height, tmp.PixelFormat);

            Graphics grp = Graphics.FromImage(clone);
            grp.DrawImage(tmp, new Rectangle(0, 0, clone.Width, clone.Height));

            tmp.Dispose();
            grp.Dispose();

            return clone;
        }

        public static bool IsAlphaBitmap(BitmapData bmpData)
        {
            for (int y = 0; y <= bmpData.Height - 1; y++)
            {
                for (int x = 0; x <= bmpData.Width - 1; x++)
                {
                    Color pixelColor = Color.FromArgb(Marshal.ReadInt32(bmpData.Scan0, bmpData.Stride * y + 4 * x));
                    if (pixelColor.A > 0 & pixelColor.A < 255)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        #endregion Public

        #endregion Methods
    }
}