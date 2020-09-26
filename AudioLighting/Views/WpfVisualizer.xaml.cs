using System.Windows;

namespace AudioLighting.Views
{
    /// <summary>
    /// Interaktionslogik für WpfVisualizer.xaml
    /// </summary>
    public partial class WpfVisualizer : Window
    {
        public WpfVisualizer(double scale)
        {
            InitializeComponent();
            sldScale.Value = scale;
        }
    }
}
