using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AuctionHouseServerCLI {
    public class Server {
        private BroadcastService broadcastService = new BroadcastService("Socket chat-rum");
        private string biddingItem;
        private double bid = 0;

        public Server(string item, int port) {
            biddingItem = item;
            System.Console.WriteLine("Server startet på port:" + port);

            IPAddress ip = IPAddress.Parse("127.0.0.1");
            TcpListener listener = new TcpListener(ip, port);
            listener.Start();

            while (true) {
                Socket clientSocket = listener.AcceptSocket();

                System.Console.WriteLine("Kunde forbundet");

                ClientHandler handler = new ClientHandler(this, clientSocket, broadcastService);

                // Her sættes op til af behandling foregår parallel
                // så der kan fortsættes med en ny
                Thread clientTråd = new Thread(handler.RunClient);
                clientTråd.Start();
            }
        }

        public string GetBiddingItem() {
            return biddingItem;
        }

        public double GetBid() {
            return bid;
        }

        public bool ApplyBid(double bid) {
            if (bid > this.bid) {
                this.bid = bid;
                return true;
            } else
                return false;
        }
    }
}
