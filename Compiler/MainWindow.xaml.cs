using Compiler.Environment;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

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
        private object _inTextLock = new object();
        private object _outTextLock = new object();

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

        private string _outText;
        public string OutText
        {
            get { return _outText; }
            set
            {
                if (value != _outText)
                {
                    lock (_outTextLock)
                    {
                        _outText = value;
                        OnPropertyChanged("OutText");
                    }

                }
            }
        }

        private string _inText;
        public string InText
        {
            get { return _inText; }
            set
            {
                if (value != _inText)
                {
                    lock (_inTextLock)
                    {
                        _inText = value;
                        OnPropertyChanged("InText");
                    }

                }
            }
        }

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            KeyDown += RunCompiler_Event;
        }

        public void UpdateScreenAlert(string msg, bool isError)
        {
            Task.Run(() =>
            {
                lock (_alertLock)
                {
                    AlertMsg = msg;

                    if (isError) AlertColor = Brushes.Red;
                    else AlertColor = Brushes.LightGreen;

                    Thread.Sleep(3000);
                    AlertMsg = "";
                }
            });
        }

        public void SelectFile(object sender, RoutedEventArgs e)
        {
            var fileSelection = new FileSelection();
            fileSelection.SendFilePath += val =>
            {
                txtFilePath.Text = val;
                TxtEditor.FilePath = val;
                TxtEditor.UpdateFileContent();
            };
            fileSelection.Show();
        }

        public async void RunCompiler()
        {
            var ans = await Task.Run(() => Vm.Run(Instruction.ExtractInstructions(TxtEditor.FileContent)));
            if (ans)
                UpdateScreenAlert("The Vm was closed sucessfully", false);
            else
                UpdateScreenAlert("The Vm could not run the commands. Check your spelling.", true);
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
                RunCompiler();
        }

        public void lviRunCompiler_Click(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                RunCompiler();
        }

        private void grdTopBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        #endregion
    }
}
