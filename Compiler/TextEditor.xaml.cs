using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using System.Threading.Tasks;
using System.ComponentModel;
using System;

namespace Compiler
{
    /// <summary>
    /// Interação lógica para TextEditor.xaml
    /// </summary>
    public partial class TextEditor : UserControl, INotifyPropertyChanged
    {
        private object _fileAccessLock = new object();

        #region Properties
        public event Action<string, bool> UpdateAlert;
        public string FilePath { get; set; }
        private string _fileContent;
        public string FileContent
        {
            get { return _fileContent; }
            set
            {
                if (value != _fileContent)
                {
                    _fileContent = value;
                    OnPropertyChanged("FileContent");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public TextEditor()
        {
            InitializeComponent();
            Page.DataContext = this;
        }

        public void UpdateFileContent()
        {
            FileContent = ReadFile();
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
            Task.Run(() =>
            {
                try
                {
                    lock (_fileAccessLock)
                    {
                        if (FilePath != string.Empty)
                        {
                            using (var sw = new StreamWriter(FilePath))
                            {
                                sw.Write(FileContent);
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
                catch (IOException e)
                {
                    UpdateAlert("Can't read file. Is it opened in another program?", true);
                }
        });
        }

        #region Events

        #endregion
    }
}
