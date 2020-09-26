using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace AudioLighting.Models
{

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
}
