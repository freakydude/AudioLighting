using MahApps.Metro.Controls.Dialogs;


namespace AudioLighting.ViewModels
{
    public class NewDeviceViewModel
    {
        private readonly IDialogCoordinator dialogCoordinator;

        // Constructor
        public NewDeviceViewModel(IDialogCoordinator instance)
        {
            dialogCoordinator = instance;
        }

        // Methods
        private async void FooMessageAsync()
        {
            await dialogCoordinator.ShowMessageAsync(this, "HEADER", "MESSAGE");
            //await dialogCoordinator.
        }

        private void FooProgress()
        {
            // Show...
            //ProgressDialogController controller = await dialogCoordinator.ShowProgressAsync(this, "HEADER", "MESSAGE");
            //controller.SetIndeterminate();

            // Do your work... 

            // Close...
            //await controller.CloseAsync();
        }
    }
}
