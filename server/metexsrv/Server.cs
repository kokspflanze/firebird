using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperWebSocket;
using SuperSocket.SocketBase;
using Newtonsoft.Json;

namespace metexsrv
{
    class Server
    {
        private WebSocketServer server;

        public Server()
        {
            server = new WebSocketServer();
        }

        public void Listen(int port)
        {
            if (!server.Setup(port))
            {
                Console.WriteLine("Error seting up websocket server!");
                Console.ReadKey();

                return;
            }

            server.NewSessionConnected += new SessionHandler<WebSocketSession>(OnOpenSession);
            server.SessionClosed += new SessionHandler<WebSocketSession, CloseReason>(OnCloseSession);

            if (!server.Start())
            {
                Console.WriteLine("Error starting websocket server!");
                Console.ReadKey();

                return;
            }
        }

        private void OnOpenSession(WebSocketSession session)
        {
            Console.WriteLine("Client connected");

            Client.clients.Add(session, new Client(session));
        }

        private void OnCloseSession(WebSocketSession session, CloseReason reason)
        {
            Console.WriteLine("Client disconnected");

            Client.clients.Remove(session);
        }

        private void OnSessionMessage(WebSocketSession session, String message)
        {
            Client.clients[session].ProcessCommand(JsonConvert.DeserializeObject<Command>(message));
        }
    }
}
