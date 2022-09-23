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
    public partial class RequestView : UserControl
    {
        public string Text { get { return (string)GetValue(TextProperty); } set { SetValue(TextProperty, value); } }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(RequestView));

        public double FloorHeight { get { return (double)GetValue(FloorHeightProperty); } set { SetValue(FloorHeightProperty, value); } }
        public static readonly DependencyProperty FloorHeightProperty = DependencyProperty.Register("FloorHeight", typeof(double), typeof(RequestView));

        public double FloorWidth { get { return (double)GetValue(FloorWidthProperty); } set { SetValue(FloorWidthProperty, value); } }
        public static readonly DependencyProperty FloorWidthProperty = DependencyProperty.Register("FloorWidth", typeof(double), typeof(RequestView));

        public double TextHeight { get { return (double)GetValue(TextHeightProperty); } set { SetValue(TextHeightProperty, value); } }
        public static readonly DependencyProperty TextHeightProperty = DependencyProperty.Register("TextHeight", typeof(double), typeof(RequestView));

        public double TextWidth { get { return (double)GetValue(TextWidthProperty); } set { SetValue(TextWidthProperty, value); } }
        public static readonly DependencyProperty TextWidthProperty = DependencyProperty.Register("TextWidth", typeof(double), typeof(RequestView));

        public readonly static SolidColorBrush DefaultBackground = new SolidColorBrush(Colors.Transparent);

        public RequestView()
        {
            InitializeComponent();
        }
    }
}
