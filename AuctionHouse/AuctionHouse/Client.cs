using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AuctionHouse {
    class Client {
        string Ip;
        int Port;
        bool connected = false;
        Controller Controller;
        NetworkStream stream;
        ThreadMonitor threadMonitor;

        public Client(Controller controller, string ip, int port) {
            Controller = controller;
            Ip = ip;
            Port = port;
        }
        public void Start() {

            TcpClient server = null;

            while (!connected) {
                try {
                    server = new TcpClient(Ip, Port);
                    Console.WriteLine("Forbundet til server!");
                    connected = true;
                } catch (Exception) {
                    Console.WriteLine("Kunne ikke forbinde til server. Prøver igen om 1 sekund...");
                    System.Threading.Thread.Sleep(1000);
                }
            }

            stream = server.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream);
            writer.AutoFlush = true;

            bool stopped = false;

            while (!stopped) {
                try {
                    string message = reader.ReadLine();
                    if (message.Contains("|"))
                        Controller.MessageReceiver(message);
                    else
                        Console.WriteLine(message);

                    Console.WriteLine(message);
                }catch(IOException) {

                }
            }

            writer.Close();
            reader.Close();
            stream.Close();
            server.Close();

            Console.ReadKey();
        }

        public void SendBid(double bid) {
            if (connected) {
                StreamWriter writer = new StreamWriter(stream);
                writer.AutoFlush = true;

                writer.WriteLine("Bid| " + bid);
            } else
                Controller.MessageReceiver("Error: Ikke forbundet til server");
        }

        internal void SetMonitor(ThreadMonitor threadMonitor) {
            this.threadMonitor = threadMonitor;
        }
    }
}
