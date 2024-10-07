using System.Runtime.InteropServices;

namespace ShellPreviewer.Models
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe")]
    public interface IShellItem
    {
        #region Methods

        #region Public
        public void BindToHandler(nint pbc, [MarshalAs(UnmanagedType.LPStruct)] Guid bhid, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, out nint ppv);

        public void GetParent(out IShellItem ppsi);

        public void GetDisplayName(uint sigdnName, out nint ppszName);

        public void GetAttributes(uint sfgaoMask, out uint psfgaoAttribs);

        public void Compare(IShellItem psi, uint hint, out int piOrder);
        #endregion Public

        #endregion Methods
    }
}