using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace ShellPreviewer.Models
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("b824b49d-22ac-4161-ac8a-9916e8fa3f7f")]
    public interface IInitializeWithStream
    {
        #region Methods

        #region Public
        public void Initialize(IStream pstream, uint grfMode);
        #endregion Public

        #endregion Methods
    }
}