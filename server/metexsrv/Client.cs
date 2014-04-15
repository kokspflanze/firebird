using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperWebSocket;
using SuperSocket.SocketBase;
using Newtonsoft.Json;

namespace metexsrv
{
    class Client
    {
        public static Dictionary<WebSocketSession, Client> clients = new Dictionary<WebSocketSession,Client>();

        public WebSocketSession Session { set;  get; }
        public Boolean RecievesValues { set; get; }
        public Double Intervall { set; get; }

        private DateTime LastSend = DateTime.Now;

        public Client(WebSocketSession _session)
        {
            Session = _session;
            RecievesValues = true;
        }

        public void SendValue(DeviceValue value)
        {
            if (RecievesValues && LastSend.AddSeconds(Intervall) <= DateTime.Now)
            {
                Command command = new Command();
                command.command = "update";
                command.parameter = new Dictionary<string, string>() {
                    {"value", value.value},
                    {"unit", value.unit},
                    {"timestamp",  (value.timestamp.Ticks / TimeSpan.TicksPerMillisecond).ToString(CultureInfo.InvariantCulture)}
                };

                Session.Send(JsonConvert.SerializeObject(command));
                LastSend = DateTime.Now;
            }
        }

        public void ProcessCommand(Command command)
        {
            switch (command.command.ToLower())
            {
                case "start":
                    RecievesValues = true;
                    Intervall = Double.Parse(command.parameter["intervall"], CultureInfo.InvariantCulture);
                    LastSend = DateTime.Now;
                    break;

                case "stop":
                    RecievesValues = false;
                    break;
            }
        }
    }
}
