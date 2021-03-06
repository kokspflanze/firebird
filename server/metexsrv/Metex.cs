﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO.Ports;
using System.IO;
using SuperWebSocket;
using SuperSocket.SocketBase;
using System.Text.RegularExpressions;

namespace metexsrv
{
    class DeviceValue
    {
        public String flow  = "";
        public String unit  = "";
        public String value = "";
        public DateTime timestamp = DateTime.Now;

        public static DeviceValue FromString(String source)
        {
            DeviceValue value = new DeviceValue();

            String[] values = source.Split(new String[] {" ", "\t"}, StringSplitOptions.RemoveEmptyEntries);
            
            int startsWithComma = values[0] == "." ? 1 : 0;

            // Wert in Einzelteile zerteilen
            if (values.Length == 2 || startsWithComma != 0)
            {
                String tempValue = values[0 + startsWithComma];
                tempValue = tempValue.Replace("O", "0");
                tempValue = tempValue.Replace("L", "0");

                while (tempValue.StartsWith("00"))
                    tempValue = tempValue.Remove(0, 1);

                value.flow = "";
                value.value = tempValue;
                value.unit = values[1 + startsWithComma];
                value.timestamp = DateTime.Now;
            }
            else
            if (values.Length == 3)
            {
                String tempValue = values[1];

                while (tempValue.StartsWith("00"))
                    tempValue = tempValue.Remove(0, 1);

                value.flow = values[0];
                value.value = tempValue;
                value.unit = values[2];
                value.timestamp = DateTime.Now;
            }

            return value;
        }
    }

    class Metex : Device
    {
        class Config
        {
            public const int BaudRate = 1200;
            public const Parity Parity = System.IO.Ports.Parity.None;
            public const int DataBits = 7;
            public const StopBits StopBits = System.IO.Ports.StopBits.Two;
            public const bool DtrEnabled = true;
            public const bool RtsEnabled = false;
            public const int DataLength = 14;
        }

        private String portName;
        private SerialPort port;

        public Metex(String portName)
        {
            this.portName = portName;
            this.Init();
        }

        public void Init()
        {
            port = new SerialPort(portName, Metex.Config.BaudRate, Metex.Config.Parity, Metex.Config.DataBits, Metex.Config.StopBits);

            port.DtrEnable = Metex.Config.DtrEnabled;
            port.RtsEnable = Metex.Config.RtsEnabled;

            port.DataReceived += new SerialDataReceivedEventHandler(DataRecieved);
        }

        public void Open()
        {
            if (port != null)
            {
                port.Open();

                if (!port.IsOpen)
                    Console.WriteLine("Error opening COM connection");
            }
        }

        public void Close()
        {
            if (port != null)
                port.Close();
        }

        private void DataRecieved(Object sender, SerialDataReceivedEventArgs arguments)
        {
            SerialPort serialPort = (SerialPort)sender;

            if (serialPort.BytesToRead >= Metex.Config.DataLength)
            {
                byte[] rawData = new byte[Metex.Config.DataLength];
                serialPort.Read(rawData, 0, Metex.Config.DataLength);

                String data = Encoding.UTF8.GetString(rawData, 0, rawData.Length).Trim();

                Console.WriteLine(String.Format("[DEBUG] Recieved data '{0}'", data));

                foreach (KeyValuePair<WebSocketSession, Client> pair in Client.clients)
                {
                    pair.Value.SendValue(DeviceValue.FromString(data));
                }
            }
        }
    }
}
