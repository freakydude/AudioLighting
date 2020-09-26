using System.Collections.Generic;

namespace AudioLighting.Models
{
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
}
