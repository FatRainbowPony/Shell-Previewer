using System.Runtime.InteropServices;

namespace ShellPreviewer.Models
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("7F73BE3F-FB79-493C-A6C7-7EE14E245841")]
    public interface IInitializeWithItem
    {
        #region Methods

        #region Public
        public void Initialize(IShellItem psi, uint grfMode);
        #endregion Public

        #endregion Methods
    }
}