using Compiler.Model;
using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Microsoft.Win32;


namespace Compiler
{
    /// <summary>
    /// Interação lógica para TextEditor.xaml
    /// </summary>
    public partial class TextEditor : UserControl
    {
        public string FilePath;
        public string FileContent;

        public TextEditor()
        {
            InitializeComponent();
            KeyDown += new KeyEventHandler(SaveFile_KeyDown);
        }

        public void UpdateFileContent()
        {
            Page.Text = ReadFile();
        }

        public string ReadFile()
        {
            var sb = new StringBuilder();
            using (StreamReader sr = File.OpenText(FilePath))
            {
                string currentLine;
                while ((currentLine = sr.ReadLine()) != null)
                {
                    sb.AppendLine(currentLine);
                }
            }

            return sb.ToString();
        }

        private void SaveFile_KeyDown(object sender, KeyEventArgs e)
        {
            if(Keyboard.Modifiers == ModifierKeys.Control &&
                e.Key == Key.S)
            {
                App.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (FilePath != string.Empty)
                    {
                        using (var sw = new StreamWriter(FilePath))
                        {
                            sw.Write(Page.Text);
                        }
                    }
                    else
                    {
                        var fileDialog = new OpenFileDialog();
                        bool? result = fileDialog.ShowDialog();
                        if (result == true)
                        {
                            File.Create(fileDialog.FileName);
                        }
                    }
                }));                
            }
        }
    }
}
