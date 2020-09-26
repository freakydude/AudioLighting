using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace AudioLighting.Models
{
    public static class MyUtils
    {
        public static AudioProcessor ap;
        public static string audioDevice;
        public static List<UdpDevice> UdpDevices = new List<UdpDevice>();

        public static double sourceFactor = 1.0;

        public static int MapValue(double a0, double a1, double b0, double b1, byte a)
        {
            var x = b0 + (b1 - b0) * ((a - a0) / (a1 - a0));
            if (x < b0)
            {
                x = b0;
            }
            else if (x > b1)
            {
                x = b1;
            }

            return (int)x;
        }

        public static int MapValue(double a0, double a1, double b0, double b1, double a)
        {
            var x = b0 + (b1 - b0) * ((a - a0) / (a1 - a0));
            if (x < b0)
            {
                x = b0;
            }
            else if (x > b1)
            {
                x = b1;
            }

            return (int)x;
        }

        public static Tuple<byte, byte, byte> HSL2RGB(double h, double sl, double l)
        {
            double v;
            double r, g, b;
            r = l;   // default to gray
            g = l;
            b = l;
            v = l <= 0.5 ? l * (1.0 + sl) : l + sl - l * sl;
            if (v > 0)
            {
                double m;
                double sv;
                int sextant;
                double fract, vsf, mid1, mid2;
                m = l + l - v;
                sv = (v - m) / v;
                h *= 6.0;
                sextant = (int)h;
                fract = h - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;
                switch (sextant)
                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;
                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;
                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;
                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;
                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                }
            }
            var rgb = new Tuple<byte, byte, byte>(Convert.ToByte(r * 255.0f), Convert.ToByte(g * 255.0f), Convert.ToByte(b * 255.0f));
            return rgb;
        }

        public static List<byte> GetAverageSpectrum(Queue<List<byte>> toSmooth, int Smoothing)
        {
            var result = new byte[toSmooth.First().Count];
            var size = toSmooth.Count;
            foreach (var measure in toSmooth)
            {
                for (var i = 0; i < measure.Count; i++)
                {
                    int x = Convert.ToByte(measure[i] / Smoothing);
                    if (result[i] + x > 255)
                    {
                        result[i] = 255;
                    }
                    else
                    {
                        result[i] += (byte)x;
                    }
                }
            }
            return result.ToList();
        }

        public static bool IsWindowOpen<T>(string name = "") where T : Window
        {
            return string.IsNullOrEmpty(name)
               ? Application.Current.Windows.OfType<T>().Any()
               : Application.Current.Windows.OfType<T>().Any(w => w.Name.Equals(name));
        }

        public static bool ValidateIp(string ip)
        {
            return IPAddress.TryParse(ip, out var ipd);
        }

        public static bool IpReachable(string ip)
        {
            var pingSender = new Ping();
            var data = "ping";
            var buffer = Encoding.ASCII.GetBytes(data);
            var timeout = 1000;
            var options = new PingOptions(64, true);
            var reply = pingSender.Send(ip, timeout, buffer, options);
            if (reply.Status == IPStatus.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    var child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (var childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public static void SaveToProperties(SaveObject toSave)
        {
            var ser = new XmlSerializer(typeof(SaveObject));
            var sw = new StringWriter();
            ser.Serialize(sw, toSave);
            var xml = sw.ToString();
            Properties.Settings.Default.Configuration = xml;
            Properties.Settings.Default.Save();

        }

        public static SaveObject RetrieveSettings()
        {
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Configuration))
            {
                var ser = new XmlSerializer(typeof(SaveObject));
                try
                {
                    using (TextReader reader = new StringReader(Properties.Settings.Default.Configuration))
                    {
                        return (SaveObject)ser.Deserialize(reader);
                    }
                }
                catch { }
            }
            return null;
        }

        public static void EnableAll()
        {
            foreach (var u in UdpDevices)
            {
                u.setPowerAsync(1);
                u.Start();
            }
        }

        public static void DisableAll()
        {
            foreach (var u in UdpDevices)
            {
                u.setPowerAsync(0);
                u.Stop();
            }
        }

        public static void SwitchDeviceFromString(string s)
        {
            var success = int.TryParse(s.Split(' ')[0], out var d);
            if (success)
            {
                if (ap != null)
                {
                    ap.StopDevice();
                }

                ap = new AudioProcessor();
                ap.StartDevice(d);
                audioDevice = s;
            }
        }

        public static void OnProcessExit(object sender, EventArgs e)
        {
            var s = new SaveObject(UdpDevices)
            {
                audioDevice = audioDevice
            };

            SaveToProperties(s);
        }


  
        public static List<NetworkDevice> NetworkDevices = new List<NetworkDevice>();

    }
}
