using System.Runtime.InteropServices;
using MSG = ShellPreviewer.Helpers.NativeMethods.MSG;
using RECT = ShellPreviewer.Helpers.NativeMethods.RECT;

namespace ShellPreviewer.Models
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("8895b1c6-b41f-4c1c-a562-0d564250836f")]
    public interface IPreviewHandler
    {
        #region Methods

        #region Public
        public void SetWindow(nint hwnd, ref RECT rect);

        public void SetRect(ref RECT rect);

        public void DoPreview();

        public void Unload();

        public void SetFocus();

        public void QueryFocus(out nint phwnd);

        [PreserveSig]
        public uint TranslateAccelerator(ref MSG pmsg);
        #endregion Public

        #endregion Methods
    }
}