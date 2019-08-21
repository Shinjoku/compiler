using System;
using System.Windows;


namespace Compiler
{
    /// <summary>
    /// Lógica interna para UserInput.xaml
    /// </summary>
    public partial class UserInput : Window
    {
        public UserInput()
        {
            InitializeComponent();
        }

        public event Action<string> SendInput;

        public void Save(object sender, RoutedEventArgs e)
        {
            SendInput(txtInput.Text);
            Close();
        }
    }
}
