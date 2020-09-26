using AudioLighting.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AudioLighting.Views
{
    /// <summary>
    /// Interaktionslogik für Spectrum.xaml
    /// </summary>
    public partial class SpectrumUserControl : UserControl
    {
        public WpfUserControlDevice wucd;
        public List<ProgressBar> bars;

        public SpectrumUserControl()
        {
            InitializeComponent();
        }

        public void enable()
        {
            if (wucd == null)
            {
                return;
            }

            wucd.Start();
        }

        public void disable()
        {
            if (wucd == null)
            {
                return;
            }

            wucd.Stop();
        }

        public int Smoothing
        {
            get => (int)GetValue(SmoothingProperty);
            set => SetValue(SmoothingProperty, value);
        }



        public string SpecName
        {
            get => (string)GetValue(SpecProperty);
            set => SetValue(SpecProperty, value);
        }

        // Using a DependencyProperty as the backing store for Name.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SpecProperty =
            DependencyProperty.Register("SpecName", typeof(string), typeof(SpectrumUserControl), new PropertyMetadata("placeholder"));



        // Using a DependencyProperty as the backing store for Smoothing.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SmoothingProperty =
            DependencyProperty.Register("Smoothing", typeof(int), typeof(SpectrumUserControl), new FrameworkPropertyMetadata(0,
                 FrameworkPropertyMetadataOptions.AffectsRender,
                   new PropertyChangedCallback(OnSmoothingChanged)));

        private static void OnSmoothingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var xyz = d as SpectrumUserControl;
            var runnin = false; if (xyz.wucd != null && xyz.wucd.Ready())
            {
                runnin = true;
            }

            xyz.disable();
            xyz.wucd = new WpfUserControlDevice(xyz.Lines, xyz, xyz.SpecName)
            {
                Smoothing = xyz.Smoothing
            };
            xyz.bars = xyz.GenerateProgressBars();
            xyz.icProgressBars.ItemsSource = xyz.bars;
            xyz.disable();
            if (runnin)
            {
                xyz.enable();
            }
        }

        public int Lines
        {
            get => (int)GetValue(LinesProperty);
            set => SetValue(LinesProperty, value);
        }

        // Using a DependencyProperty as the backing store for Lines.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LinesProperty =
            DependencyProperty.Register("Lines", typeof(int), typeof(SpectrumUserControl), new FrameworkPropertyMetadata(10,
                 FrameworkPropertyMetadataOptions.AffectsRender,
                   new PropertyChangedCallback(OnLineCountSet)));

        private static void OnObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var xyz = d as SpectrumUserControl;
            //xyz.SpectrumVisualizer.Height = xyz.BarHeight;
            //xyz.MainGrid.Height = xyz.BarHeight;
            //xyz.SpectrumVisualizer.Width = xyz.TotalWidth;
            //xyz.MainGrid.Width = xyz.TotalWidth;
            var runnin = false; if (xyz.wucd != null && xyz.wucd.Ready())
            {
                runnin = true;
            }

            xyz.Width = xyz.TotalWidth;

            xyz.Height = xyz.BarHeight;
            xyz.disable();
            xyz.wucd = new WpfUserControlDevice(xyz.Lines, xyz, xyz.SpecName)
            {
                Smoothing = xyz.Smoothing
            };
            xyz.bars = xyz.GenerateProgressBars();
            xyz.icProgressBars.ItemsSource = xyz.bars;
            xyz.disable();
            if (runnin)
            {
                xyz.enable();
            }
        }

        private static void OnLineCountSet(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var xyz = d as SpectrumUserControl;
            var runnin = false; if (xyz.wucd != null && xyz.wucd.Ready())
            {
                runnin = true;
            }

            xyz.disable();
            xyz.wucd = new WpfUserControlDevice(xyz.Lines, xyz, xyz.SpecName)
            {
                Smoothing = xyz.Smoothing
            };
            xyz.bars = xyz.GenerateProgressBars();
            xyz.icProgressBars.ItemsSource = xyz.bars;
            xyz.disable();
            if (runnin)
            {
                xyz.enable();
            }
        }

        public int BarHeight
        {
            get => (int)GetValue(BarHeightProperty);
            set => SetValue(BarHeightProperty, value);
        }

        // Using a DependencyProperty as the backing store for Height.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BarHeightProperty =
            DependencyProperty.Register("BarHeight", typeof(int), typeof(SpectrumUserControl), new FrameworkPropertyMetadata(20,
                 FrameworkPropertyMetadataOptions.AffectsRender,
                   new PropertyChangedCallback(OnObjectChanged)));



        public int TotalWidth
        {
            get => (int)GetValue(TotalWidthProperty);
            set => SetValue(TotalWidthProperty, value);
        }

        // Using a DependencyProperty as the backing store for Totalwidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TotalWidthProperty =
            DependencyProperty.Register("TotalWidth", typeof(int), typeof(SpectrumUserControl), new FrameworkPropertyMetadata(300,
                 FrameworkPropertyMetadataOptions.AffectsRender,
                   new PropertyChangedCallback(OnObjectChanged)));



        /*
        public List<ProgressBar> GenerateProgressBars
        {
          get
          {
            List<ProgressBar> src = new List<ProgressBar>();
            for (int i = 0; i < Lines; i++)
            {
              ProgressBar p = new ProgressBar();
              p.Value = 100; p.Name = "c" + (i + 1).ToString(); p.HorizontalAlignment = HorizontalAlignment.Left;p.VerticalAlignment = VerticalAlignment.Top; p.Height = BarHeight; p.Orientation = Orientation.Vertical;
              p.Width = TotalWidth / Lines;
              Thickness ma = p.Margin;
              ma.Top = 10;
              ma.Left = (TotalWidth / Lines) * (i) + i;
              ma.Right = i; ma.Bottom = 0;
              p.Margin = ma;

              src.Add(p);
            }
            return src;
          }
        }
    */
        public List<ProgressBar> GenerateProgressBars(bool isRGB = true)
        {
            var src = new List<ProgressBar>();
            for (var i = 0; i < Lines; i++)
            {
                var p = new ProgressBar();
                //MetroProgressBar p = new MetroProgressBar();
                var tw_withSpacing = 300;
                if (TotalWidth > 0)
                {
                    tw_withSpacing = TotalWidth - Lines;
                }
                //p.Value = 255 - ((i * 255) / Lines);
                p.Value = 0;
                p.Name = "c" + (i + 1).ToString(); p.HorizontalAlignment = HorizontalAlignment.Left; p.VerticalAlignment = VerticalAlignment.Top; p.Orientation = Orientation.Vertical;
                //p.Minimum = 0;
                p.Minimum = 1;
                p.Maximum = 255;
                p.Width = tw_withSpacing / Lines;
                p.Height = BarHeight;
                var ma = p.Margin;
                ma.Top = 0;
                ma.Left = (tw_withSpacing / Lines) * (i) + 2 * i;
                ma.Right = 0; ma.Bottom = 0;
                p.Margin = ma;

                var g = new LinearGradientBrush
                {
                    StartPoint = new Point(1, 1),
                    EndPoint = new Point(0, 0)
                };
                var c = MyUtils.HSL2RGB(i / ((double)Lines), 1.0, 0.6);
                var c2 = MyUtils.HSL2RGB(i / ((double)Lines), 1.0, 0.4);
                g.GradientStops.Add(new GradientStop(Color.FromArgb(255, c.Item1, c.Item2, c.Item3), 0.0));
                g.GradientStops.Add(new GradientStop(Color.FromArgb(255, c2.Item1, c2.Item2, c2.Item3), 1.0));
                if (isRGB)
                {
                    p.Style = null;
                    p.Foreground = g;
                    p.Background = null;
                    p.BorderBrush = null;
                    //p.BorderBrush = (Brush)Application.Current.FindResource("RainbowBorderBrush"); ;
                }

                src.Add(p);
            }
            return src;
        }
    }
}
