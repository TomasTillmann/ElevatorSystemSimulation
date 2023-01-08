using System.Windows;

namespace UI
{
    public class ViewBase<TViewModel> : UIElement
    {
        protected TViewModel Model { get; }

        public ViewBase(TViewModel model)
        {
            Model = model;
        }
    }
}
