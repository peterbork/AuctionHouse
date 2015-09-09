using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace AuctionHouseServer
{
    class Server {
        private BroadcastService broadcastService = new BroadcastService("Socket rum");
        private int Port;

        public Server(int port) {
            Port = port;
        }

        public void Start() {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            TcpListener listener = new TcpListener(ip, Port);
            listener.Start();

            Console.WriteLine("Server start");

            while (true) {
                Socket clientSocket = listener.AcceptSocket();

                ClientHandler handler = new ClientHandler(clientSocket, broadcastService);

                Thread clientThread = new Thread(handler.StartClient);
                clientThread.Start();
            }
        }

    }
}
