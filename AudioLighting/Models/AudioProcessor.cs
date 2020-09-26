using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using Un4seen.Bass;
using Un4seen.BassWasapi;

namespace AudioLighting.Models
{
    public class AudioProcessor
    {
        public delegate void AudioAvailableEventHandler(object sender, AudioAvailableEventArgs e);
        public event AudioAvailableEventHandler AudioAvailable;

        private readonly bool _enable;               //enabled status
        private readonly DispatcherTimer timer;         //timer that refreshes the display
        private readonly float[] _fft;               //buffer for fft data
        private double leftChannelIntensity, rightChannelIntensity;         //progressbars for left and right channel intensity
        private readonly WASAPIPROC _process;        //callback function to obtain data
        private int lastOutLevel;             //last output level
        private int lastOutputLevelCounter;                //last output level counter
        private readonly List<byte> _spectrumdata;   //spectrum data buffer
        private bool _initialized;          //initialized flag
        private int devindex;               //used device index


        public AudioProcessor()
        {

            _fft = new float[1024];
            lastOutLevel = 0;
            lastOutputLevelCounter = 0;
            timer = new DispatcherTimer();
            timer.Tick += OnTimerTick;
            timer.Interval = TimeSpan.FromMilliseconds(20); //40hz refresh rate
            timer.IsEnabled = false;
            leftChannelIntensity = 0;
            rightChannelIntensity = 0;
            rightChannelIntensity = ushort.MaxValue;
            leftChannelIntensity = ushort.MaxValue;
            _process = new WASAPIPROC(Process);
            _spectrumdata = new List<byte>();

            devindex = 0;
            _initialized = false;
        }

        public bool StartDevice(int deviceIndex)
        {
            var result2 = false;
            var result = false;

            if (!_initialized)
            {
                devindex = deviceIndex;

                result2 = Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                if (!result2)
                {
                    var error = Bass.BASS_ErrorGetCode();
                    //MessageBox.Show(error.ToString());
                }
                result = BassWasapi.BASS_WASAPI_Init(devindex, 0, 0, BASSWASAPIInit.BASS_WASAPI_BUFFER, 1f, 0.05f, _process, IntPtr.Zero);
                if (!result)
                {
                    var error = Bass.BASS_ErrorGetCode();
                    //MessageBox.Show(error.ToString());
                }
                if (result && result2)
                {
                    _initialized = true;
                    var started = BassWasapi.BASS_WASAPI_Start();
                    timer.IsEnabled = true;
                }
            }

            return result && result2;
        }

        public bool StopDevice()
        {
            timer.IsEnabled = false;
            var result = BassWasapi.BASS_WASAPI_Stop(true);
            var result2 = true;//Bass.BASS_Stop();

            var wasSuccessful = BassWasapi.BASS_WASAPI_Free();
            var wasSuccessful2 = Bass.BASS_Free();

            return result && result2 && wasSuccessful && wasSuccessful2;
        }

        //timer 
        private void OnTimerTick(object sender, EventArgs e)
        {
            // get fft data. Return value is -1 on error
            var ret = BassWasapi.BASS_WASAPI_GetData(_fft, (int)BASSData.BASS_DATA_FFT2048);
            if (ret < 0)
            {
                return;
            }

            OnAudioAvailable(_fft);
            _spectrumdata.Clear();


            var level = BassWasapi.BASS_WASAPI_GetLevel();
            leftChannelIntensity = Utils.LowWord32(level);
            rightChannelIntensity = Utils.HighWord32(level);
            if (level == lastOutLevel && level != 0)
            {
                lastOutputLevelCounter++;
            }

            lastOutLevel = level;

            //Required, because some programs hang the output. If the output hangs for a 75ms
            //this piece of code re initializes the output
            //so it doesn't make a gliched sound for long.

            if (lastOutputLevelCounter > 3)
            {
                lastOutputLevelCounter = 0;
                leftChannelIntensity = 0;
                rightChannelIntensity = 0;
            }
        }



        // WASAPI callback, required for continuous recording
        private int Process(IntPtr buffer, int length, IntPtr user)
        {
            return length;
        }


        public static List<byte> getSpectrumData(float[] fftData, int bands, double r, double factor)
        {
            var retBands = bands;
            bands = Convert.ToInt32(bands / r);
            return getSpectrumData(fftData, bands, factor).GetRange(0, retBands);
        }

        public static List<byte> getSpectrumData(float[] fftData, int bands, double factor)
        {
            var max = fftData.Max();
            var min = fftData.Min();
            var result = new List<byte>();


            int x, y;
            var b0 = 0;
            for (x = 0; x < bands; x++)
            {
                float peak = 0;
                var b1 = (int)Math.Pow(2, x * 10.0 / (bands - 1));
                if (b1 > 1023)
                {
                    b1 = 1023;
                }

                if (b1 <= b0)
                {
                    b1 = b0 + 1;
                }

                for (; b0 < b1; b0++)
                {
                    if (peak < fftData[1 + b0])
                    {
                        peak = fftData[1 + b0];
                    }
                }
                y = (int)(Math.Sqrt(peak) * 3 * 255 - 4);
                if (y > 255)
                {
                    y = 255;
                }

                if (y < 0)
                {
                    y = 0;
                }

                result.Add((byte)y);
            }

            return applyFactor(result, factor);
        }

        public static List<byte> applyFactor(List<byte> x, double f)
        {
            var ret = new List<byte>();
            foreach (var b in x)
            {
                var t = (int)(b * f);
                if (t < 0)
                {
                    t = 0;
                }
                else if (t > 255)
                {
                    t = 255;
                }

                ret.Add((byte)t);
            }
            return ret;
        }


        protected void OnAudioAvailable(float[] _toConv)
        {
            var audioAvailable = AudioAvailable;
            if (audioAvailable != null)
            {
                audioAvailable(this, new AudioAvailableEventArgs(_toConv));
            }
            else
            {
                //throw new NullReferenceException("No Handler!");
            }
        }
    }

    public class AudioAvailableEventArgs : EventArgs
    {
        private readonly float[] data;
        public AudioAvailableEventArgs(float[] fftData)
        {
            data = fftData;
        }
        public float[] AudioAvailable => data;
    }
}