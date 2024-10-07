using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace ShellPreviewer.Helpers
{
    public class StreamWrapper(Stream inner) : IStream
    {
        #region Fields

        #region Private
        private readonly Stream inner = inner;
        #endregion Private

        #endregion Fields

        #region Methods

        #region Public
        public void Clone(out IStream ppstm) => throw new NotImplementedException();

        public void Commit(int grfCommitFlags) => throw new NotImplementedException();

        public void CopyTo(IStream pstm, long cb, nint pcbRead, nint pcbWritten) => throw new NotImplementedException();

        public void LockRegion(long libOffset, long cb, int dwLockType) => throw new NotImplementedException();

        public void Read(byte[] pv, int cb, nint pcbRead)
        {
            long bytesRead = inner.Read(pv, 0, cb);

            if (pcbRead != nint.Zero)
            {
                Marshal.WriteInt64(pcbRead, bytesRead);
            }
        }

        public void Revert() => throw new NotImplementedException();

        public void Seek(long dlibMove, int dwOrigin, nint plibNewPosition)
        {
            long pos = inner.Seek(dlibMove, (SeekOrigin)dwOrigin);

            if (plibNewPosition != nint.Zero)
            {
                Marshal.WriteInt64(plibNewPosition, pos);
            }
        }

        public void SetSize(long libNewSize) => throw new NotImplementedException();

        public void Stat(out STATSTG pstatstg, int grfStatFlag) => pstatstg = new STATSTG
        {
            cbSize = inner.Length,
            type = 2,
            pwcsName = inner is FileStream stream ? stream.Name : string.Empty
        };

        public void UnlockRegion(long libOffset, long cb, int dwLockType) => throw new NotImplementedException();

        public void Write(byte[] pv, int cb, nint pcbWritten)
        {
            inner.Write(pv, 0, cb);

            if (pcbWritten != nint.Zero)
            {
                Marshal.WriteInt64(pcbWritten, cb);
            }
        }
        #endregion Public

        #endregion Methods
    }
}