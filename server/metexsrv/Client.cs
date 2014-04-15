using System;
using System.Collections.Generic;
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

        public Client(WebSocketSession _session)
        {
            Session = _session;
            RecievesValues = true;
        }

        public void SendValue(DeviceValue value)
        {
            if (RecievesValues)
            {
                Command command = new Command();
                command.command = "update";
                command.parameter = new Dictionary<string, string>() {
                    {"value", value.value},
                    {"unit", value.unit},
                    {"timestamp", Convert.ToString(value.timestamp)}
                };

                Session.Send(JsonConvert.SerializeObject(command));
            }
        }

        public void ProcessCommand(Command command)
        {
            switch (command.command.ToLower())
            {
                case "start":
                    RecievesValues = true;
                    break;

                case "stop":
                    RecievesValues = false;
                    break;

                case "intervall":
                    Console.WriteLine("Command not implemented yet");
                    break;
            }
        }
    }
}
