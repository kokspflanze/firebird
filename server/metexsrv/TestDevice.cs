using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using SuperWebSocket;
using SuperSocket.SocketBase;

namespace metexsrv
{
    class TestDevice : Device
    {
        private Timer updateTimer = new Timer(500);
        private Double lastValue;

        public TestDevice()
        {
            updateTimer.Elapsed += new ElapsedEventHandler(OnTick);
        }

        public void Open()
        {
            updateTimer.Start();
        }

        public void Close()
        {
            updateTimer.Stop();
        }

        private void OnTick(object sender, ElapsedEventArgs args)
        {
            foreach (KeyValuePair<WebSocketSession, Client> pair in Client.clients)
            {
                DeviceValue value = new DeviceValue();
                value.unit = "mV";
                value.flow = "DC";
                value.value = (Math.Sin(lastValue % Math.PI*2) * 5.0).ToString(System.Globalization.CultureInfo.InvariantCulture);

                pair.Value.SendValue(value);
            }

            lastValue += 0.1;
        }
    }
}
