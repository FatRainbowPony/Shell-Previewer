using System.Windows;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;

namespace ShellPreviewerDemo
{
    public partial class MainWindow : Window
    {
        #region Constructors

        #region Public
        public MainWindow() => InitializeComponent();
        #endregion Public

        #endregion Constructors

        #region Methods

        #region Private
        private void FileButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new() 
            { 
                Filter = "All files (*.*)|*.*",
                DereferenceLinks = false,
            };
            if (ofd.ShowDialog() == true)
            {
                ShellPreviewer.Source = ofd.FileName;
            }
        }

        private void FolderButtonClick(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog fbd = new();
            if (fbd.ShowDialog() == true)
            {
                ShellPreviewer.Source = fbd.SelectedPath;
            }
        }
        #endregion Private

        #endregion Methods
    }
}