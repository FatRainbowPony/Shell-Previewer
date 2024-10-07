using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using ShellPreviewer.Helpers;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Image = System.Windows.Controls.Image;
using Orientation = System.Windows.Controls.Orientation;
using PreviewHandlerHost = ShellPreviewer.Controls.WinForms.PreviewHandlerHost;
using PreviewHandlerHostStatusValue = ShellPreviewer.Controls.WinForms.PreviewHandlerHost.StatusValue;
using SHFILEINFO = ShellPreviewer.Helpers.NativeMethods.SHFILEINFO;
using SHGFI = ShellPreviewer.Helpers.NativeMethods.SHGFI;
using Size = System.Windows.Size;
using UserControl = System.Windows.Controls.UserControl;
using VerticalAlignment = System.Windows.VerticalAlignment;

namespace ShellPreviewer.Controls.Wpf
{
    public partial class ShellPreviewer : UserControl, IDisposable
    {
        #region Enums

        #region Public
        public enum StatusValue
        {
            None,
            Handler,
            Thumbnail,
            Icon,
            Error
        }
        #endregion Public

        #endregion Enums

        #region Fields

        #region Private
        private bool disposed;
        #endregion Private

        #endregion Fields

        #region Properties

        #region Public
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source),
                typeof(string),
                typeof(ShellPreviewer),
                new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnSourceChanged)));

        public string Source
        {
            get => (string)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public static readonly DependencyPropertyKey StatusProperty =
            DependencyProperty.RegisterReadOnly(nameof(Status),
                typeof(StatusValue),
                typeof(ShellPreviewer),
                new PropertyMetadata(StatusValue.None));

        public StatusValue Status
        {
            get => (StatusValue)GetValue(StatusProperty.DependencyProperty);
            private set => SetValue(StatusProperty, value);
        }
        #endregion Public

        #endregion Properties

        #region Constructors

        #region Public
        public ShellPreviewer()
        {
            InitializeComponent();
            Dispatcher.ShutdownStarted += OnShutdownStarted;
        }
        #endregion Public

        #endregion Constructors

        #region Destructors
        ~ShellPreviewer() => Dispose(false);
        #endregion Destructors

        #region Methods

        #region Private
        private void OnShutdownStarted(object? sender, EventArgs e) => Dispose(true);

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ShellPreviewer previewer && e.NewValue is string source)
            {
                previewer.OpenPreview(source);
            }
        }

        private UIElement? RestoreRootElement(Type typeElement)
        {
            if (typeElement.Name == "PreviewHandlerHost")
            {
                PART_RootGrid.Children.Add(new WindowsFormsHost
                {
                    Name = "PART_HandlerHost",
                    Child = new PreviewHandlerHost()
                });
            }
            else if (typeElement.Name == "DockPanel")
            {
                PART_RootGrid.Children.Add(new DockPanel
                {
                    Name = "PART_ThumbnailOrIconHost",
                    LastChildFill = true
                });
            }

            return PART_RootGrid.Children.Count > 0 ? PART_RootGrid.Children[0] : default;
        }

        private void ClearRootElement() 
        {
            if (PART_RootGrid.Children.Count > 0)
            {
                if (PART_RootGrid.Children[0] is WindowsFormsHost winFormsHost && 
                    winFormsHost.Name == "PART_HandlerHost" && 
                    winFormsHost.Child is PreviewHandlerHost handlerHost)
                {
                    handlerHost.Dispose();
                    winFormsHost.Dispose();

                    winFormsHost.Child = null;

                    PART_RootGrid.Children.Clear();
                }
                else if (PART_RootGrid.Children[0] is DockPanel thumbnailOrIconHost &&
                        thumbnailOrIconHost.Name == "PART_ThumbnailOrIconHost" &&
                        thumbnailOrIconHost.Children.Count > 0)
                {
                    thumbnailOrIconHost.Children.Clear();
                    PART_RootGrid.Children.Clear();
                }
            }
        }

        private void AddPreviewInfo(string path, string typeName, string displayName)
        {
            if (PART_RootGrid.Children.Count > 0 && PART_RootGrid.Children[0] is DockPanel thumbnailOrIconHost)
            {
                string type = $"{Resources["TypeText"]}: {typeName}";
                string changeDate = $"{Resources["ChangeDateText"]}: {new DirectoryInfo(path).LastWriteTime.ToString(Language.GetSpecificCulture())}";

                StackPanel childPanel = new()
                {
                    Orientation = Orientation.Vertical,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 10, 10, 10)
                };
                childPanel.Children.Add(new TextBlock
                {
                    Text = displayName,
                    TextAlignment = TextAlignment.Left,
                    TextTrimming = TextTrimming.WordEllipsis,
                    TextWrapping = TextWrapping.Wrap,
                    Foreground = Foreground,
                    FontSize = 20,
                    FontWeight = FontWeights.Bold,
                    LineStackingStrategy = LineStackingStrategy.BlockLineHeight,
                    LineHeight = 25,
                    MaxHeight = 100,
                    Margin = new Thickness(0, 0, 0, 6)
                });
                childPanel.Children.Add(new TextBlock
                {
                    Text = type,
                    TextAlignment = TextAlignment.Left,
                    TextTrimming = TextTrimming.WordEllipsis,
                    TextWrapping = TextWrapping.Wrap,
                    Foreground = Foreground,
                    FontSize = 15,
                    LineStackingStrategy = LineStackingStrategy.BlockLineHeight,
                    LineHeight = 25,
                    MaxHeight = 100,
                    Margin = new Thickness(0, 0, 0, 6)
                });
                childPanel.Children.Add(new TextBlock
                {
                    Text = changeDate,
                    TextAlignment = TextAlignment.Left,
                    TextTrimming = TextTrimming.WordEllipsis,
                    TextWrapping = TextWrapping.Wrap,
                    Foreground = Foreground,
                    FontSize = 15,
                    LineStackingStrategy = LineStackingStrategy.BlockLineHeight,
                    LineHeight = 25,
                    MaxHeight = 100
                });

                thumbnailOrIconHost.Children.Add(childPanel);
            }
        }
        #endregion Private

        #region Protected
        protected virtual void OpenPreview(string path)
        {
            ArgumentException.ThrowIfNullOrEmpty(path, nameof(path));
            ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

            Status = StatusValue.None;

            ClearRootElement();

            if (RestoreRootElement(typeof(PreviewHandlerHost)) is WindowsFormsHost winFormsHost && winFormsHost.Child is PreviewHandlerHost handlerHost)
            {
                handlerHost.Source = path;

                if (handlerHost.Status == PreviewHandlerHostStatusValue.Ready)
                {
                    UpdateLayout();

                    Status = StatusValue.Handler;
                }
            }

            if (Status == StatusValue.None)
            {
                ClearRootElement();

                if (RestoreRootElement(typeof(DockPanel)) is DockPanel thumbnailOrIconHost)
                {
                    try
                    {
                        SHFILEINFO info = new();
                        NativeMethods.SHGetFileInfo(path, 0, ref info, (uint)Marshal.SizeOf(info), (uint)(SHGFI.SHGFI_TYPENAME | SHGFI.SHGFI_DISPLAYNAME | SHGFI.SHGFI_ATTRIBUTES));

                        if (!string.IsNullOrEmpty(info.szTypeName) && !string.IsNullOrEmpty(info.szDisplayName))
                        {
                            if (IconExtractor.GetThumbnail(path) is ImageSource thumbnailSource)
                            {
                                thumbnailOrIconHost.Children.Add(new Image
                                {
                                    Source = thumbnailSource,
                                    RenderSize = new Size(256, 256),
                                    Stretch = Stretch.Uniform,
                                    Width = 256,
                                    Height = 256,
                                    VerticalAlignment = VerticalAlignment.Center,
                                    Margin = new Thickness(10)
                                });

                                DockPanel.SetDock(thumbnailOrIconHost.Children[0], Dock.Left);

                                AddPreviewInfo(path, info.szTypeName, info.szDisplayName);

                                Status = StatusValue.Thumbnail;
                            }
                            else if (IconExtractor.GetIcon(path) is ImageSource iconSource)
                            {
                                thumbnailOrIconHost.Children.Add(new Image
                                {
                                    Source = iconSource,
                                    RenderSize = new Size(256, 256),
                                    Stretch = Stretch.Uniform,
                                    Width = 256,
                                    Height = 256,
                                    VerticalAlignment = VerticalAlignment.Center,
                                    Margin = new Thickness(10)
                                });

                                DockPanel.SetDock(thumbnailOrIconHost.Children[0], Dock.Left);

                                AddPreviewInfo(path, info.szTypeName, info.szDisplayName);

                                Status = StatusValue.Icon;
                            }
                        }

                        if (info.hIcon != IntPtr.Zero)
                        {
                            NativeMethods.DeleteObject(info.hIcon);
                        }
                    }
                    catch { }
                }
            }

            if (Status == StatusValue.None)
            {
                if (PART_RootGrid.Children[0] is DockPanel errorHost)
                {
                    StackPanel childPanel = new()
                    {
                        Orientation = Orientation.Vertical,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    childPanel.Children.Add(new ContentControl
                    {
                        Content = (Canvas)Resources["ErrorIcon"],
                        RenderSize = new Size(256, 256),
                        Width = 256,
                        Height = 256,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(10, 6, 10, 10)
                    });
                    childPanel.Children.Add(new TextBlock
                    {
                        Text = $"{Resources["ErrorMessageText"]}",
                        TextAlignment = TextAlignment.Center,
                        TextTrimming = TextTrimming.WordEllipsis,
                        TextWrapping = TextWrapping.Wrap,
                        Foreground = Foreground,
                        FontSize = 20,
                        LineStackingStrategy = LineStackingStrategy.BlockLineHeight,
                        LineHeight = 25,
                        MaxHeight = 100,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(10, 0, 10, 10)
                    });

                    errorHost.Children.Add(childPanel);

                    Status = StatusValue.Error;
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                Dispatcher.ShutdownStarted -= OnShutdownStarted;
                ClearRootElement();
            }

            disposed = true;
        }
        #endregion Protected

        #region Public
        public void OpenPreview() => OpenPreview(Source);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion Public

        #endregion Methods
    }
}