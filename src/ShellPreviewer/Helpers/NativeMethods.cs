using System.Runtime.InteropServices;
using ShellPreviewer.Models;

namespace ShellPreviewer.Helpers
{
    public static class NativeMethods
    {
        #region Enums

        #region Public
        [Flags]
        public enum SIIGBF
        {
            SIIGBF_RESIZETOFIT = 0x00,
            SIIGBF_BIGGERSIZEOK = 0x01,
            SIIGBF_MEMORYONLY = 0x02,
            SIIGBF_ICONONLY = 0x04,
            SIIGBF_THUMBNAILONLY = 0x08,
            SIIGBF_INCACHEONLY = 0x10
        }

        [Flags]
        public enum SHGFI
        {
            SHGFI_ICON = 0x000000100,
            SHGFI_DISPLAYNAME = 0x000000200,
            SHGFI_TYPENAME = 0x000000400,
            SHGFI_ATTRIBUTES = 0x000000800,
            SHGFI_ICONLOCATION = 0x000001000,
            SHGFI_EXETYPE = 0x000002000,
            SHGFI_SYSICONINDEX = 0x000004000,
            SHGFI_LINKOVERLAY = 0x000008000,
            SHGFI_SELECTED = 0x000010000,
            SHGFI_ATTR_SPECIFIED = 0x000020000,
            SHGFI_LARGEICON = 0x000000000,
            SHGFI_SMALLICON = 0x000000001,
            SHGFI_OPENICON = 0x000000002,
            SHGFI_SHELLICONSIZE = 0x000000004,
            SHGFI_PIDL = 0x000000008,
            SHGFI_USEFILEATTRIBUTES = 0x000000010,
            SHGFI_ADDOVERLAYS = 0x000000020,
            SHGFI_OVERLAYINDEX = 0x000000040
        }
        #endregion Public

        #endregion Enums

        #region Structures

        #region Public
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT(int x, int y)
        {
            public int X = x,
                       Y = y;

            public static implicit operator Point(POINT p) => new(p.X, p.Y);

            public static implicit operator POINT(Point p) => new(p.X, p.Y);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SIZE
        {
            private int width,
                        height;

            public int Width { set => width = value; }

            public int Height { set => height = value; }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT(int left, int top, int right, int bottom)
        {
            public int Left = left,
                       Top = top,
                       Right = right,
                       Bottom = bottom;

            public RECT(Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom) { }

            public int X
            {
                readonly get => Left;
                set
                {
                    Right -= Left - value;
                    Left = value;
                }
            }

            public int Y
            {
                readonly get => Top;
                set
                {
                    Bottom -= Top - value;
                    Top = value;
                }
            }

            public int Height
            {
                readonly get => Bottom - Top;
                set => Bottom = value + Top;
            }

            public int Width
            {
                readonly get => Right - Left;
                set => Right = value + Left;
            }

            public Point Location
            {
                readonly get => new(Left, Top);
                set
                {
                    X = value.X;
                    Y = value.Y;
                }
            }

            public Size Size
            {
                readonly get => new(Width, Height);
                set
                {
                    Width = value.Width;
                    Height = value.Height;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSG
        {
            public nint hwnd,
                          lParam;

            public int time,
                       lPrivate;

            public uint message;
            public nuint wParam;
            public POINT pt;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHFILEINFO
        {
            public nint hIcon;
            public int iIcon;
            public uint dwAttributes;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }
        #endregion Public

        #endregion Structures

        #region Methods

        #region Public
        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject(nint hObject);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void SHCreateItemFromParsingName([In][MarshalAs(UnmanagedType.LPWStr)] string pszPath,
            [In] nint pbc, [In][MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [Out][MarshalAs(UnmanagedType.Interface, IidParameterIndex = 2)] out IShellItem ppv);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern nint SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);
        #endregion Public

        #endregion Methods
    }
}