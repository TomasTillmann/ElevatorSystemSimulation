using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UI
{
    /// <summary>
    /// Interaction logic for ElevatorView.xaml
    /// </summary>
    public partial class ElevatorView : UserControl
    {
        public int PeopleCount { get { return (int)GetValue(PeopleCountProperty); } set { SetValue(PeopleCountProperty, value); } }
        public static readonly DependencyProperty PeopleCountProperty = DependencyProperty.Register("PeopleCount", typeof(int), typeof(ElevatorView), new PropertyMetadata(0));

        public ElevatorView()
        {
            InitializeComponent();
        }
    }
}
