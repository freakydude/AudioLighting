using System.Windows;

namespace AudioLighting.Views
{
    /// <summary>
    /// Interaktionslogik für WpfVisualizer.xaml
    /// </summary>
    public partial class VisualizerWindow : Window
    {
        public VisualizerWindow(double scale)
        {
            InitializeComponent();
            sldScale.Value = scale;
        }
    }
}
