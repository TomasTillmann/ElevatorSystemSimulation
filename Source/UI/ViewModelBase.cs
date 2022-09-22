using System.Windows;

namespace UI
{
    public abstract class ViewModelBase<TModel> : DependencyObject
    {
        protected TModel Model { get; }

        public ViewModelBase(TModel model)
        {
            Model = model;
        }
    }
}
