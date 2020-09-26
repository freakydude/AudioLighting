using System.Windows;
using System.Windows.Controls;

namespace AudioLighting.Views
{
    /// <summary>
    /// Interaktionslogik für Settings.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        public void DeviceSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.Source == null || lblSmoothing == null)
            {
                return;
            }

            lblSmoothing.Content = ((Slider)e.Source).Value.ToString();
        }
    }
}
