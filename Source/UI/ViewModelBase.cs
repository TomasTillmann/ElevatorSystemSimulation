namespace UI
{
    public class ViewModelBase<TModel>
    {
        protected TModel Model { get; }

        public ViewModelBase(TModel model)
        {
            Model = model;
        }
    }
}
