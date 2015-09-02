using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace AuctionHouseServer
{
    class Server
    {
        private IPAddress ip;
        private int port;
        private volatile bool stop = false;

        public Server(string ip, string port)
        {
            this.ip = IPAddress.Parse(ip);
            this.port = int.Parse(port);
        }

        public void Run()
        {
            TcpListener listener = new TcpListener(ip, port);
            listener.Start();
            while (!stop)
            {
                // Modtager klient
                Socket clientSocket = listener.AcceptSocket();

                // Streams
                NetworkStream netStream = new NetworkStream(clientSocket);
                StreamWriter writer = new StreamWriter(netStream);
                StreamReader reader = new StreamReader(netStream);

                // Lukker forbindelser
                writer.Close();
                reader.Close();
                netStream.Close();
                clientSocket.Close();
            }
        }
    }
}
