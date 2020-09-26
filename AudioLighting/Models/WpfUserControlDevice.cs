using AudioLighting.Views;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AudioLighting.Models
{

    public class WpfUserControlDevice : ICommunicate
    {
        private readonly int lines;
        private bool enable;
        private readonly SpectrumUserControl spec;
        private readonly Queue<List<byte>> lastVals = new Queue<List<byte>>();
        private int smoothing;
        public double range = 0.7;
        public string name;

        public WpfUserControlDevice(int lines, SpectrumUserControl spec, string n)
        {
            this.lines = lines;
            this.spec = spec;
            enable = false;
            Smoothing = 10;
            name = n;
        }

        public bool Smooth { get => Smoothing > 0; set { if (!value) { smoothing = 0; } } }
        public int Smoothing { get => smoothing; set => smoothing = value; }

        public bool Ready()
        {
            return enable;
        }

        public bool Send(List<byte> arr)
        {
            foreach (var p in spec.bars)
            {
                var num = Convert.ToInt32(p.Name.Substring(1));
                //if (num <= arr.Count) p.Value = MyUtils.MapValue(0,255,0,100,arr[num - 1]);
                if (num <= arr.Count)
                {
                    p.Value = arr[num - 1];
                }
            }
            return true;
        }

        public bool Send(string s)
        {
            return true;
        }

        public bool Start()
        {
            if (!enable && MyUtils.ap != null)
            {
                MyUtils.ap.AudioAvailable += new AudioProcessor.AudioAvailableEventHandler(UpdateValues);
                enable = true;
            }
            return true;
        }

        public bool Stop()
        {
            if (enable && MyUtils.ap != null)
            {
                try
                {
                    enable = false;
                    MyUtils.ap.AudioAvailable -= new AudioProcessor.AudioAvailableEventHandler(UpdateValues);
                    Send(new byte[lines].ToList());
                }
                catch { }
            }
            enable = false;
            return true;
        }

        public void UpdateValues(object sender, AudioAvailableEventArgs e)
        {
            var newData = AudioProcessor.getSpectrumData(e.AudioAvailable, lines, range, MyUtils.sourceFactor);
            lastVals.Enqueue(newData);
            while (lastVals.Count > Smoothing)
            {
                lastVals.Dequeue();
            }
            if (!Smooth)
            {
                Send(newData);
            }
            else
            {
                Send(MyUtils.GetAverageSpectrum(lastVals, Smoothing));
            }
        }
    }
}
