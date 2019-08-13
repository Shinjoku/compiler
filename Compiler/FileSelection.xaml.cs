using System;
using System.Windows;
using Microsoft.Win32;


namespace Compiler
{
    /// <summary>
    /// Lógica interna para FileSelection.xaml
    /// </summary>
    public partial class FileSelection : Window
    {

        public event Action<string> SendFilePath;

        public FileSelection()
        {
            InitializeComponent();
        }

        public void OpenExplorer(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = "Text documents (.txt)|*.txt"
            };

            bool? result = fileDialog.ShowDialog();
            if (result == true)
            {
                txtFilePath.Text = fileDialog.FileName;
            }
        }

        public void Save(object sender, RoutedEventArgs e)
        {
            SendFilePath(txtFilePath.Text);
            Close();
        }
    }
}
