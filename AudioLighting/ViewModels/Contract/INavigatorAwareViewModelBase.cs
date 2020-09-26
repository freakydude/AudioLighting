namespace AudioLighting.ViewModels.Contract
{
    /// <summary>
    /// This interface should be used, if a navigation to a view model with a parameter is needed.
    /// </summary>
    public interface INavigatorAwareViewModelBase : IBindableBase
    {
        /// <summary>
        /// This is called right after navigation with the given parameter
        /// </summary>
        /// <param name="parameter">The usage depends on the implementation.</param>
        void NavigatedTo(params object[] parameter);
    }
}