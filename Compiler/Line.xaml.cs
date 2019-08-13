using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace Compiler
{
    /// <summary>
    /// Interação lógica para Line.xaml
    /// </summary>
    public partial class Line : UserControl
    {
        public int Index;
        public string Text;
        public bool Breakpoint = false;

        public Line(int index, string text)
        {
            Index = index;
            Text = text;
            InitializeComponent();
        }

        public void ActivateBreakpoint(object sender, EventArgs e)
        {
            Breakpoint = !Breakpoint;
            var button = (Button) sender;
            button.Background = Breakpoint ? Brushes.Red : Brushes.DimGray;
        }
    }
}
