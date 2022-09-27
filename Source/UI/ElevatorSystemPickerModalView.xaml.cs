using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UI
{
    /// <summary>
    /// Interaction logic for ElevatorSystemPickerModalView.xaml
    /// </summary>
    public partial class ElevatorSystemPickerModalView : Window
    {
        public ElevatorSystemPickerViewModel ViewModel => (ElevatorSystemPickerViewModel)DataContext;
        public ElevatorSystemPickerModalView(Window owner)
        {
            Owner = owner;
            Owner.Opacity = 0.5;
            InitializeComponent();

            Width = owner.Width - 2 * owner.Width / 3;
            Height = owner.Height - 2 * owner.Height / 10;

            Closing += OnWindowClosing;
        }

        private void OnWindowClosing(object? sender, CancelEventArgs e)
        {
            Owner.Opacity = 1;
        }
    }
}
