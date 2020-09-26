using MahApps.Metro;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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


        #region NetworkScanner
        public static List<NetworkDevice> NetworkDevices = new List<NetworkDevice>();

        public class NetworkDevice
        {
            public int ping { get; set; }
            public string DisplayPing
            {
                get
                {
                    if (ping >= 0)
                    {
                        return ping.ToString();
                    }
                    else
                    {
                        return "";
                    }
                }
                set => DisplayPing = value;
            }
            public bool isLedDevice { get; set; }
            public string IP { get; set; }
            public string Hostname { get; set; }
            public string allResponse { get; set; }

            private readonly ManualResetEvent _doneEvent;

            public NetworkDevice() { }
            public NetworkDevice(string ipaddr, ManualResetEvent doneEvent)
            {
                IP = ipaddr;
                ping = -2;
                isLedDevice = false;
                _doneEvent = doneEvent;
            }

            public static string returnWebserverInfo(string ipaddr)
            {
                var addr = "http://" + ipaddr + "/all";
                var response = "";
                try
                {
                    using (var webClient = new WebClient())
                    {
                        response = webClient.DownloadString(addr);
                    }
                    if (string.IsNullOrEmpty(response))
                    {
                        return "";
                    }

                    Debug.WriteLine("Got Response from: " + ipaddr);
                    return response;
                }
                catch { return ""; }
            }

            public int PingDevice()
            {
                var p = new Ping();
                PingReply r;
                r = p.Send(IP);
                if (!(r.Status == IPStatus.Success))
                {
                    allResponse = "";
                    return -1;
                }
                return Convert.ToInt32(r.RoundtripTime);
            }

            public static Task<PingReply> PingAsync(string address)
            {
                var tcs = new TaskCompletionSource<PingReply>();
                var ping = new Ping();
                ping.PingCompleted += (obj, sender) =>
                {
                    tcs.SetResult(sender.Reply);
                };
                ping.SendAsync(address, new object());
                return tcs.Task;
            }

            public void ThreadPoolCallback(object threadContext)
            {
                ping = PingDevice(); if (ping < 0)
                {
                    return;
                }

                allResponse = returnWebserverInfo(IP);
                if (!string.IsNullOrEmpty(allResponse))
                {
                    if (allResponse.Contains("{\"name\":\"power\",\"label\":\"Power\",\"type\":\"Boolean\""))
                    {
                        isLedDevice = true;
                    }
                }
                else { }
                try
                {
                    var hr = Dns.GetHostEntry(IP);
                    Hostname = hr.HostName;
                }
                catch
                {
                    Hostname = "";
                    try     // cheesy way to retrieve hostname without parsing the json
                    {
                        if (allResponse.Contains("hostname"))
                        {
                            var p = allResponse.LastIndexOf("hostname");
                            var h = "";
                            for (var i = p; i < allResponse.Length; i++)
                            {
                                if (allResponse.Substring(i - 4, 5) == "value")
                                {
                                    h = allResponse.Substring(i + 3);
                                    h = h.Substring(0, h.IndexOf('"'));
                                }
                            }
                            if (!string.IsNullOrEmpty(h))
                            {
                                Hostname = h;
                            }
                        }
                    }
                    catch
                    {
                        Hostname = "";
                    }
                }
                _doneEvent.Set();
            }
        }
        #endregion
    }



    public class SaveObject
    {
        public List<UdpDevice> udps;
        public string audioDevice;

        public SaveObject(List<UdpDevice> udps)
        {
            this.udps = udps;
        }
        public SaveObject() { }
    }

    public partial class WebserverResponse
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public Value? Value { get; set; }

        [JsonProperty("min", NullValueHandling = NullValueHandling.Ignore)]
        public long? Min { get; set; }

        [JsonProperty("max", NullValueHandling = NullValueHandling.Ignore)]
        public long? Max { get; set; }

        [JsonProperty("options", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Options { get; set; }
    }

    public partial struct Value
    {
        public long? Integer;
        public string String;

        public static implicit operator Value(long Integer)
        {
            return new Value { Integer = Integer };
        }

        public static implicit operator Value(string String)
        {
            return new Value { String = String };
        }
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                ValueConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ValueConverter : JsonConverter
    {
        public override bool CanConvert(Type t)
        {
            return t == typeof(Value) || t == typeof(Value?);
        }

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Integer:
                    var integerValue = serializer.Deserialize<long>(reader);
                    return new Value { Integer = integerValue };
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    return new Value { String = stringValue };
            }
            throw new Exception("Cannot unmarshal type Value");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (Value)untypedValue;
            if (value.Integer != null)
            {
                serializer.Serialize(writer, value.Integer.Value);
                return;
            }
            if (value.String != null)
            {
                serializer.Serialize(writer, value.String);
                return;
            }
            throw new Exception("Cannot marshal type Value");
        }

        public static readonly ValueConverter Singleton = new ValueConverter();
    }
}
