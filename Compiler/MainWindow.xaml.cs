using Compiler.Environment;
using Compiler.General;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Compiler
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Properties

        private object _alertMsglock = new object();
        private object _alertColorlock = new object();
        private object _alertLock = new object();
        private object _themeLock = new object();


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _alertMsg;
        public string AlertMsg
        {
            get { return _alertMsg; }
            set {
                if(value != _alertMsg)
                {
                    lock(_alertMsglock)
                    {
                        _alertMsg = value;
                        OnPropertyChanged("AlertMsg");
                    }

                }
            }
        }

        private SolidColorBrush _alertColor;
        public SolidColorBrush AlertColor
        {
            get { return _alertColor; }
            set
            {
                if (value != _alertColor)
                {
                    lock (_alertColorlock)
                    {
                        _alertColor = value;
                        OnPropertyChanged("AlertColor");
                    }

                }
            }
        }

        private Brush _theme = Brushes.Teal;
        public Brush Theme
        {
            get { return _theme; }
            set
            {
                if (value != _theme)
                {
                    lock (_themeLock)
                    {
                        _theme = value;
                        OnPropertyChanged("Theme");
                    }

                }
            }
        }

        #endregion

        public bool _compilerMode = true;
        private int _alertCounter = 0;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            KeyDown += RunCompiler_Event;
            KeyDown += SaveFile_KeyDown;
            TxtEditor.UpdateAlert += m => UpdateScreenAlert(m, false);
        }

        public void UpdateScreenAlert(string msg, bool isError)
        {
            Task.Run(() =>
            {
                _alertCounter++;
                var alertString = _alertCounter + ": " + msg + '\n';
                AlertMsg += alertString;

                if (isError) AlertColor = Brushes.Red;
                else AlertColor = Brushes.LightGreen;

                lock (_alertLock)
                {
                    Thread.Sleep(2500);
                    AlertMsg = AlertMsg.Replace(alertString, "");
                    _alertCounter--;
                }
            });
        }

        public void ChangeMode()
        {
            _compilerMode = !_compilerMode;

            if (_compilerMode)
            {
                Theme = Brushes.Teal;
                Uri iconUri = new Uri("pack://application:,,,/Resources/Compiler.ico", UriKind.RelativeOrAbsolute);
                Icon = BitmapFrame.Create(iconUri);
                UpdateScreenAlert("Compiler mode activated.", false);
            }
            else
            {
                Theme = Brushes.DarkRed;
                Uri iconUri = new Uri("pack://application:,,,/Resources/VMRun.ico", UriKind.RelativeOrAbsolute);
                Icon = BitmapFrame.Create(iconUri);
                UpdateScreenAlert("VM mode activated.", false);
            }
        }

        public void SelectFile()
        {
            var fileSelection = new FileSelection
            {
                Owner = Application.Current.MainWindow,
                CompilerMode = _compilerMode
            };

            fileSelection.SendFilePath += val =>
            {
                txtFilePath.Text = val;
                TxtEditor.FilePath = val;
                TxtEditor.UpdateFileContent();
                UpdateScreenAlert("File loaded.", false);
            };

            fileSelection.ShowDialog();                
        }

        public async void RunVM()
        {
            var ans = await Task.Run(() => {
                try
                {
                    return Vm.Run(
                        AssemblyInstruction.ExtractAssemblyInstructions(TxtEditor.FileContent));
                }
                catch (Exception)
                {
                    UpdateScreenAlert("The Vm has stopped working.", true);
                    return new Task<bool>(() => false);
                }
            });

            if (ans)
                UpdateScreenAlert("The Vm was closed sucessfully", false);
        }

        public async void RunCompiler()
        {
            bool ans = false;
            try
            {
                ans = await Task.Run(() =>
                {
                    var lexical = new Lexical();
                    return lexical.Run(TxtEditor.FilePath);
                });
            }
            catch (NotSupportedCharacterException e)
            {
                UpdateScreenAlert(e.Message, true);
            }

            if (ans)
                UpdateScreenAlert("Your file has been compiled successfully.", false);
        }

        public void Run()
        {
            try
            {
                if (_compilerMode)
                    RunCompiler();
                else
                    RunVM();
            }
            catch(Exception e) { UpdateScreenAlert(e.Message, true); }
        }

        #region Events

        public void btnClose_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        public void btnCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            btnOpenMenu.Visibility = Visibility.Visible;
            btnCloseMenu.Visibility = Visibility.Collapsed;
        }

        public void btnOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            btnCloseMenu.Visibility = Visibility.Visible;
            btnOpenMenu.Visibility = Visibility.Collapsed;
        }

        public void RunCompiler_Event(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
                Run();
        }
        private void SaveFile_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control &&
                e.Key == Key.S)
            {
                TxtEditor.SaveFile();
            }
        }

        public void lviRunCompiler_Click(object sender, MouseEventArgs e)
        {
            var button = (ListViewItem)sender;
            button.IsEnabled = false;

            if (e.LeftButton == MouseButtonState.Pressed)
                Run();

            button.IsEnabled = true;
        }
        public void lviSelectFile_Click(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                SelectFile();
        }

        private void lviSaveFile_Click(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                TxtEditor.SaveFile();
        }

        private void lviChangeIDEMode_Click(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                ChangeMode();
        }

        private void grdTopBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void grdSelectFile_Loaded(object sender, RoutedEventArgs e)
        {
            SelectFile();
        }

        #endregion
    }
}
