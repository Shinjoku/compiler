using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Compiler
{
    /// <summary>
    /// Interação lógica para TextEditor.xaml
    /// </summary>
    public partial class TextEditor : UserControl, INotifyPropertyChanged
    {
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

        public TextEditor()
        {
            InitializeComponent();
            Page.DataContext = this;
            KeyDown += new KeyEventHandler(SaveFile_KeyDown);
        }

        public void UpdateFileContent()
        {
            FileContent = ReadFile();
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
                Task.Run(() =>
                {
                    if (FilePath != string.Empty)
                    {
                        using (var sw = new StreamWriter(FilePath))
                        {
                            sw.Write(FileContent);
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
                });                
            }
        }
    }
}
