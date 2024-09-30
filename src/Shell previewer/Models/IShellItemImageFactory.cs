using System.Runtime.InteropServices;
using SIIGBF = ShellPreviewer.Helpers.NativeMethods.SIIGBF;
using SIZE = ShellPreviewer.Helpers.NativeMethods.SIZE;

namespace ShellPreviewer.Models
{
    [ComImport]
    [Guid("bcc18b79-ba16-442f-80c4-8a59c30c463b")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IShellItemImageFactory
    {
        #region Methods

        #region Public
        [PreserveSig]
        public void GetImage([In, MarshalAs(UnmanagedType.Struct)] SIZE size, [In] SIIGBF flags, [Out] out nint phbm);
        #endregion Public

        #endregion Methods
    }
}