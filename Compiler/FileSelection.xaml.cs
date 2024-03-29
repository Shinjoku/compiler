﻿using System;
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
        public bool CompilerMode;

        public FileSelection()
        {
            InitializeComponent();
        }

        public void OpenExplorer(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = CompilerMode ? "Pascal files (.pas)|*.pas" : "Assembly files (.obj)|*.obj"
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
