
using Microsoft.Extensions.DependencyInjection;

namespace AudioLighting.ViewModels.Common
{
    /// <summary>
    /// Used for xaml designer to get code completion and the default data context.
    /// </summary>
    public class ViewModelLocator
    {
        public IMainViewModel MainViewModel
        {
            get
            { 
                return App.ServiceProvider.GetRequiredService<IMainViewModel>(); 
            }
        }
    }
}