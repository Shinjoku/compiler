using Compiler.Environment;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Compiler
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private object _lock = new object();

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
                    lock(_lock)
                    {
                        _alertMsg = value;
                        OnPropertyChanged("AlertMsg");
                    }

                }
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            KeyDown += RunCompiler_Event;
        }

        public void UpdateScreenAlert(string msg)
        {
            Task.Run(() =>
            {
                AlertMsg = msg;
                Thread.Sleep(5000);
                AlertMsg = "";
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
            var ans = await Task.Run(() => VirtualMachine.Run(Instruction.ExtractInstructions(TxtEditor.FileContent)));
            if (ans)
                UpdateScreenAlert("The Vm was closed sucessfully");
            else
                UpdateScreenAlert("The Vm could not run the commands. Check your spelling.");
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
