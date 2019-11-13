using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using System.Threading.Tasks;
using System.ComponentModel;
using System;
using System.Reflection;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Highlighting;

namespace Compiler
{
    /// <summary>
    /// Interação lógica para TextEditor.xaml
    /// </summary>
    public partial class TextEditor : UserControl, INotifyPropertyChanged
    {
        private object _fileAccessLock = new object();
        private const string _syntaxFilePath = @"Properties\LPDHighlighting.xshd";

        #region Properties
        public event Action<string, bool> UpdateAlert;
        public string FilePath { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public TextEditor()
        {
            InitializeComponent();
            Editor.DataContext = this;
            Editor.SyntaxHighlighting = HighlightingLoader.Load(GetSyntaxHighlighting(), HighlightingManager.Instance);
        }

        public static XmlTextReader GetSyntaxHighlighting()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), _syntaxFilePath);
            return new XmlTextReader(path);
        }

        public void UpdateFileContent()
        {
            Editor.Text = ReadFile();
        }

        public string ReadFile()
        {
            var sb = new StringBuilder();
            try
            {
                lock (_fileAccessLock)
                {
                    using (StreamReader sr = File.OpenText(FilePath))
                    {
                        string currentLine;
                        while ((currentLine = sr.ReadLine()) != null)
                        {
                            sb.AppendLine(currentLine);
                        }
                    }
                }
            }
            catch (IOException)
            {
                UpdateAlert("Can't read file. Is it opened in another program?", true);
            }

            return sb.ToString();
        }

        public void SaveFile()
        {
            App.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                try
                {
                    lock (_fileAccessLock)
                    {
                        if (FilePath != string.Empty)
                        {
                            using (var sw = new StreamWriter(FilePath))
                            {
                                sw.Write(Editor.Text);
                            }

                            UpdateAlert("The file has been saved successfully.", false);
                        }
                        else
                        {
                            var fileDialog = new OpenFileDialog();
                            bool? result = fileDialog.ShowDialog();
                            if (result == true && fileDialog.FileName != string.Empty)
                            {
                                File.Create(fileDialog.FileName);
                                UpdateAlert("The file has been created successfully.", false);
                            }
                            else
                                UpdateAlert("Can't create file.", true);

                        }
                    }
                }
                catch (IOException)
                {
                    UpdateAlert("Can't read file. Is it opened in another program?", true);
                }
        }));
        }

        #region Events

        #endregion
    }
}
