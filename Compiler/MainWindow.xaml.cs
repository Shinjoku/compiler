using System.Windows;


namespace Compiler
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
    }
}
