using System.Windows.Input;

namespace UI
{
    public interface IScalable
    {
        double Scale { get; set; }
        void Zoom(object sender, MouseWheelEventArgs e);
    }
}