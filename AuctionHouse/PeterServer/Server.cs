using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace PeterServer {
    class Server {

        MainWindow view;
        ThreadMonitor threadMonitor = new ThreadMonitor();

        private DateTime timer;
        private static bool AuctionRunning;
        private static bool CountdownRunning = true;
        private static bool UnderSixtySeconds = false;

        private static List<string> names = new List<string>();
        private static List<Auction> Auctions = new List<Auction>();
        public static Thread ThreadCountdown;

        public Server() {
            view = new MainWindow();

            threadMonitor.ThreadEvent += view.Thread_MessageReciever;

            Auction testAuction = new Auction("Sofabord", 1, 10);
            Auctions.Add(testAuction);
            view.AddAuctionItem(Auctions[0].Name);

            TcpListener tcpListener = new TcpListener(IPAddress.Any, 1234);
            tcpListener.Start();

            ClientHandler handler = new ClientHandler(this, tcpListener);
            Thread clientHandlerThread = new Thread(handler.AcceptClients);
            clientHandlerThread.Start();

        }

        public List<Auction> GetAuctions() {
            return Auctions;
        }

        public DateTime GetTimer() {
            return timer;
        }

        internal void SendCommand(string ip, string command) {
            threadMonitor.ThreadAction(ip + ": " + command + "\n");
        }

        internal void AddBid(int bid, TcpClient tcpClient) {
            Auctions[0].bids.Add(bid, tcpClient);
        }

        internal bool IsUnderSixty() {
            return UnderSixtySeconds;
        }

        internal void AbortThreadCountdown() {
            ThreadCountdown.Abort();
        }

        internal void StartCountdown() {
            CountdownRunning = true;
        }

        internal void ExtendTime() {
            UnderSixtySeconds = false;
            timer = timer.AddMinutes(0.10);
        }
    }
}
