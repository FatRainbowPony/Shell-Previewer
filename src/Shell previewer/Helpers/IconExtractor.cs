using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ShellPreviewer.Models;
using SIIGBF = ShellPreviewer.Helpers.NativeMethods.SIIGBF;
using SIZE = ShellPreviewer.Helpers.NativeMethods.SIZE;

namespace ShellPreviewer.Helpers
{
    public static class IconExtractor
    {
        #region Methods

        #region Private
        private static ImageSource? GetImage(string path, SIIGBF options)
        {
            ArgumentException.ThrowIfNullOrEmpty(path, nameof(path));
            ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

            ImageSource? imageSource = null;
            IShellItem? shellItem = null;
            nint hBmp = nint.Zero;

            try
            {
                NativeMethods.SHCreateItemFromParsingName(path, nint.Zero, new Guid("7E9FB0D3-919F-4307-AB2E-9B1860310C93"), out shellItem);

                if (shellItem != null)
                {
                    ((IShellItemImageFactory)shellItem).GetImage(new SIZE
                    {
                        Width = 256,
                        Height = 256
                    }, options, out hBmp);

                    if (hBmp != nint.Zero &&
                        ImageOperations.GetBitmapFromHBitmap(hBmp) is Bitmap bmp &&
                        ImageOperations.BitmapToBitmapImage(bmp) is BitmapImage bmpImage)
                    {
                        imageSource = bmpImage;
                        hBmp = nint.Zero;
                    }
                }
            }
            catch { }

            if (hBmp != nint.Zero)
            {
                NativeMethods.DeleteObject(hBmp);
            }

            if (shellItem != null)
            {
                Marshal.ReleaseComObject(shellItem);
            }

            return imageSource;
        }
        #endregion Private

        #region Public
        public static ImageSource? GetIcon(string fileName) => GetImage(fileName, SIIGBF.SIIGBF_BIGGERSIZEOK | SIIGBF.SIIGBF_ICONONLY);

        public static ImageSource? GetThumbnail(string fileName) => GetImage(fileName, SIIGBF.SIIGBF_THUMBNAILONLY);
        #endregion Public

        #endregion Methods
    }
}