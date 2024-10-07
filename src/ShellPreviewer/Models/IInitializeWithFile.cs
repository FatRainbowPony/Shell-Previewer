using System.Runtime.InteropServices;

namespace ShellPreviewer.Models
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("b7d14566-0509-4cce-a71f-0a554233bd9b")]
    public partial interface IInitializeWithFile
    {
        #region Methods

        #region Public
        public void Initialize([MarshalAs(UnmanagedType.LPWStr)] string pszFilePath, uint grfMode);
        #endregion Public

        #endregion Methods
    }
}
