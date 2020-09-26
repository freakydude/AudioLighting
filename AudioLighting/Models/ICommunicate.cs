using System.Collections.Generic;

namespace AudioLighting.Models
{
    internal interface ICommunicate
    {
        bool Start();
        bool Stop();
        bool Send(List<byte> arr);
        bool Send(string s);
        bool Ready();
        void UpdateValues(object sender, AudioAvailableEventArgs e);
    }
}
