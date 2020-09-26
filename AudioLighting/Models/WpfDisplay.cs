using AudioLighting.Views;
using System;
using System.Collections.Generic;

namespace AudioLighting.Models
{
    public class WpfDisplay : ICommunicate
    {
        private readonly VisualizerWindow w;
        private double scale;
        private bool open = false;

        public WpfDisplay() { scale = 1; w = new VisualizerWindow(scale); }
        public WpfDisplay(double scale) { this.scale = scale; w = new VisualizerWindow(scale); }

        public WpfDisplay(VisualizerWindow w)
        {
            this.w = w;
            scale = 1;
        }
        public WpfDisplay(double scale, VisualizerWindow w)
        {
            this.w = w;
            this.scale = scale;
        }

        public double Scale { get => scale; set => scale = value; }

        public bool Ready()
        {
            return open;
            //return w != null && w.IsActive;
            //return MyUtils.IsWindowOpen<WpfVisualizer>();
        }

        public bool Send(List<byte> arr)
        {
            if (!Ready())
            {
                return false;
            }

            var n = new List<int>();
            foreach (var b in arr)
            {
                n.Add((byte)MyUtils.MapValue(0, 255, 0, 100, b * w.sldScale.Value));
            }

            if (n.Count >= 32)
            {
                #region PROGRESS_BAR
                w.c1.Value = n[0];
                w.c2.Value = n[1];
                w.c3.Value = n[2];
                w.c4.Value = n[3];
                w.c5.Value = n[4];
                w.c6.Value = n[5];
                w.c7.Value = n[6];
                w.c8.Value = n[7];
                w.c9.Value = n[8];
                w.c10.Value = n[9];
                w.c11.Value = n[10];
                w.c12.Value = n[11];
                w.c13.Value = n[12];
                w.c14.Value = n[13];
                w.c15.Value = n[14];
                w.c16.Value = n[15];
                w.c17.Value = n[16];
                w.c18.Value = n[17];
                w.c19.Value = n[18];
                w.c20.Value = n[19];
                w.c21.Value = n[20];
                w.c22.Value = n[21];
                w.c23.Value = n[22];
                w.c24.Value = n[23];
                w.c25.Value = n[24];
                w.c26.Value = n[25];
                w.c27.Value = n[26];
                w.c28.Value = n[27];
                w.c29.Value = n[28];
                w.c30.Value = n[29];
                w.c31.Value = n[30];
                w.c32.Value = n[31];
                #endregion
            }
            return true;
        }

        public bool Send(string s)
        {
            throw new NotImplementedException();
        }

        public bool Start()
        {
            if (!Ready())
            {
                w.Show();
            }

            open = true;
            return true;
        }

        public bool Stop()
        {
            w.Close();
            open = false;
            return true;
        }

        public void UpdateValues(object sender, AudioAvailableEventArgs e)
        {
            if (Ready())
            {
                Send(AudioProcessor.getSpectrumData(e.AudioAvailable, 32, MyUtils.sourceFactor));
            }
        }
    }
}
