using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Threading;

namespace PeterServer {
    class ClientHandler {

        private static TcpListener tcpListener;
        private static List<TcpClient> tcpClientsList = new List<TcpClient>();

        private Server server;
        private Broadcaster broadcaster;

        public ClientHandler(Server server, TcpListener listener) {
            this.server = server;
            tcpListener = listener;
            broadcaster = new Broadcaster();
        }

        public void AcceptClients() {
            while (true) {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                tcpClientsList.Add(tcpClient);

                Thread thread = new Thread(ClientListener);
                thread.Start(tcpClient);
            }
        }

        public void ClientListener(object obj) {
            TcpClient tcpClient = (TcpClient)obj;
            StreamReader reader = new StreamReader(tcpClient.GetStream());
            //tbConsole.Text = "Client connected! " + tcpClientsList.Count + " Client(s) Connected.";

            foreach (Auction auction in server.GetAuctions()) {
                if (auction.Status == 1) {
                    //sender igangværende auktioner til nye klienter
                    broadcaster.BroadCastForNew(server.GetAuctions()[0], server.GetTimer(), tcpClientsList, tcpClient);
                }
            }

            while (true) {
                string message = reader.ReadLine();
                List<string> commands = message.Split('|').ToList<string>();
                string command = commands[0];
                commands.RemoveAt(0);
                if (command == "Bid") {
                    var diffInSeconds = (Convert.ToDateTime(server.GetTimer()) - DateTime.Now).TotalSeconds;

                    server.SendCommand(tcpClient.Client.LocalEndPoint.ToString(), (string)commands[0]);
                    broadcaster.BroadCast("Bid| " + commands[0], tcpClientsList);
                    server.AddBid(int.Parse(commands[0]), tcpClient);

                    // extend timer by 1 minute
                    if (server.IsUnderSixty() == true) {
                        if (diffInSeconds <= 10) {
                            server.AbortThreadCountdown();
                            server.StartCountdown();
                        }
                        server.ExtendTime();
                        broadcaster.BroadCastExtendedTime(server.GetTimer(), tcpClientsList);
                    }
                } else {
                    server.SendCommand(tcpClient.Client.LocalEndPoint.ToString(), message);
                }
            }
        }

        public void AuctionTimer() {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(UpdateTimer);
            timer.Interval = new TimeSpan(1000); // in miliseconds
            timer.Start();
        }

        private void UpdateTimer(object sender, EventArgs e) {

            if (AuctionRunning == true) {
                DateTime currentTime = DateTime.Now;
                var diffInSeconds = (Convert.ToDateTime(timer) - currentTime).TotalSeconds;
                var ts = TimeSpan.FromSeconds(diffInSeconds);
                if (diffInSeconds <= 0) {
                    WinnerFound(Auctions[0].bids.Values.Last());
                }
                if (diffInSeconds <= 10 && CountdownRunning == true) {
                    ThreadCountdown = new Thread(BroadCastCountdown);
                    ThreadCountdown.Start();
                    CountdownRunning = false;
                }
                if (diffInSeconds <= 60) {
                    UnderSixtySeconds = true;
                }
                lblTime.Content = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
            }
        }
    }
}
