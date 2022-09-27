using System.Windows;

namespace UI
{
    public abstract class ViewModelBase<TModel> : DependencyObject
    {
        protected TModel Model { get; set; }

        public ViewModelBase(TModel model)
        {
            Model = model;
        }
    }

    public abstract class ViewModelBase : DependencyObject
    {
        protected object? Model { get; set; }

        public ViewModelBase(object? model = null)
        {
            Model = model;
        }
    }
}
