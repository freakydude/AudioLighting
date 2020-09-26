using System.Collections.Generic;
using System.IO.Ports;

namespace AudioLighting.Models
{
    public class SerialComDevice : ICommunicate
    {
        public SerialPort Serial { get; set; }
        public int Lines { get => lines; set => lines = value; }

        private int lines;

        public SerialComDevice(SerialPort s)
        {
            Serial = s;
        }

        public SerialComDevice(string port, int baud, int bands)
        {
            Serial = new SerialPort(port, baud);
            Lines = bands;
        }

        public bool Send(List<byte> data)
        {
            if (Serial != null)
            {
                Serial.Write(data.ToArray(), 0, data.Count);
                return true;
            }
            return false;
        }

        public bool Send(string s)
        {
            if (Serial != null)
            {
                Serial.Write(s);
                return true;
            }
            return false;
        }

        public bool Start()
        {
            if (Serial == null)
            {
                return false;
            }

            if (!Serial.IsOpen)
            {
                Serial.Open();
            }
            return true;
        }

        public bool Stop()
        {
            if (Serial != null && Serial.IsOpen)
            {
                Serial.Close();
            }

            return true;
        }

        public void UpdateValues(object sender, AudioAvailableEventArgs e)
        {
            if (Ready())
            {
                Send(AudioProcessor.getSpectrumData(e.AudioAvailable, lines, MyUtils.sourceFactor));
            }
        }

        public bool Ready()
        {
            return Serial != null && Serial.IsOpen;
        }
    }
}
