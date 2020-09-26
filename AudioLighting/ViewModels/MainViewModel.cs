using AudioLighting.ViewModels.Common;
using Microsoft.Extensions.Logging;

namespace AudioLighting
{
    public class MainViewModel : BindableBase, IMainViewModel
    {
        private readonly ILogger<MainViewModel> logger;

        public MainViewModel()
        {

        }

        public MainViewModel(ILogger<MainViewModel> logger)
        {
            this.logger = logger;
        }
    }
}