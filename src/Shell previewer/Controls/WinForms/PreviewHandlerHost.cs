using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using ShellPreviewer.Helpers;
using ShellPreviewer.Models;
using RECT = ShellPreviewer.Helpers.NativeMethods.RECT;

namespace ShellPreviewer.Controls.WinForms
{
    public class PreviewHandlerHost : Control
    {
        #region Enums

        #region Public
        public enum StatusValue
        {
            None, 
            Ready,
            Error
        }
        #endregion Public
        
        #endregion Enums

        #region Fields

        #region Private
        private object? previewHandler;
        private Guid previewHandlerGuid = Guid.Empty;
        private Stream? previewHandlerStream;
        private string source = string.Empty;
        #endregion Private

        #endregion Fields

        #region Properties

        #region Public
        public string Source
        {
            get => source;
            set 
            { 
                source = value;

                OpenPreview(source);
            }
        }

        public StatusValue Status { get; private set; } = StatusValue.None;
        #endregion Public

        #endregion Properties

        #region Constructors

        #region Public
        public PreviewHandlerHost() : base() { }
        #endregion Public

        #endregion Constructors
                
        #region Methods

        #region Private
        private static Guid GetPreviewHandlerGuid(string path)
        {
            using (RegistryKey? extRegKey = Registry.ClassesRoot.OpenSubKey(Path.GetExtension(path)))
            {
                using (RegistryKey? typeRegKey = extRegKey?.OpenSubKey("shellex\\{8895b1c6-b41f-4c1c-a562-0d564250836f}"))
                {
                    if (typeRegKey?.GetValue(null) is object guid)
                    {
                        extRegKey?.Dispose();
                        typeRegKey?.Dispose();

                        return new Guid($"{guid}");
                    }
                }

                if (extRegKey?.GetValue(null) is object className)
                {
                    using RegistryKey? typeRegKey = Registry.ClassesRoot.OpenSubKey($"{className}\\\\shellex\\\\{{8895b1c6-b41f-4c1c-a562-0d564250836f}}");

                    if (typeRegKey?.GetValue(null) is object guid)
                    {
                        return new Guid($"{guid}");
                    }
                }
            }

            return Guid.Empty;
        }

        private void UnloadPreviewHandler()
        {
            previewHandlerStream?.Close();
            previewHandlerStream?.Dispose();
            previewHandlerStream = null;

            if (previewHandler is IPreviewHandler handler)
            {
                handler.Unload();
            }

            if (previewHandler != null)
            {
                Marshal.FinalReleaseComObject(previewHandler);
                previewHandler = null;
            }

            GC.Collect();
        }
        #endregion Private

        #region Protected
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e); 
            
            if (previewHandler is IPreviewHandler handler)
            {
                RECT rect = new(ClientRectangle);
                handler.SetRect(rect);
            }
        }

        protected virtual void OpenPreview(string path)
        {
            ArgumentException.ThrowIfNullOrEmpty(path, nameof(path));
            ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

            Status = StatusValue.None;

            if (GetPreviewHandlerGuid(path) is Guid guid && guid != Guid.Empty)
            {
                previewHandlerGuid = guid;

                try
                {
                    UnloadPreviewHandler();

                    if (Type.GetTypeFromCLSID(previewHandlerGuid) is Type comType)
                    {
                        previewHandler = Activator.CreateInstance(comType);
                    }

                    switch (previewHandler)
                    {
                        case IInitializeWithFile initializeWithFile:
                            initializeWithFile.Initialize(path, 0);
                            break;

                        case IInitializeWithStream initializeWithStream:
                            previewHandlerStream = File.Open(path, FileMode.Open);
                            initializeWithStream.Initialize(new StreamWrapper(previewHandlerStream), 0);
                            break;

                        case IInitializeWithItem initializeWithItem:
                            NativeMethods.SHCreateItemFromParsingName(path, IntPtr.Zero, new Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe"), out IShellItem shellItem);
                            initializeWithItem.Initialize(shellItem, 0);
                            break;

                        default:
                            Status = StatusValue.Error;
                            break;
                    }

                    if (Status != StatusValue.Error && previewHandler is IPreviewHandler handler)
                    {
                        RECT rect = new(ClientRectangle);

                        handler.SetWindow(Handle, ref rect);
                        handler.DoPreview();

                        Status = StatusValue.Ready;
                    }
                    else
                    {
                        UnloadPreviewHandler();

                        Status = StatusValue.Error;
                    }
                }
                catch
                {
                    Status = StatusValue.Error;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            UnloadPreviewHandler();

            base.Dispose(disposing);
        }
        #endregion Protected

        #region Public
        public void OpenPreview() => OpenPreview(Source);
        #endregion Public

        #endregion Methods
    }
}