using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PeterServer {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        ThreadMonitor threadMonitor = new ThreadMonitor("Server");
        //private DateTime AuctionStartTime;
        private DateTime timer;
        private static bool AuctionRunning;
        private static bool CountdownRunning = true;
        private static bool UnderSixtySeconds = false;

        private static TcpListener tcpListener;
        private static List<string> names = new List<string>();
        private static List<Auction> Auctions = new List<Auction>();
        private static List<TcpClient> tcpClientsList = new List<TcpClient>();
        private static Thread ThreadCountdown;


        public MainWindow() {
            InitializeComponent();
            threadMonitor.ThreadEvent += Thread_MessageReciever;

            Auction testAuction = new Auction("Sofabord", 1, 10);
            Auctions.Add(testAuction);
            listBoxAuctions.Items.Add(Auctions[0].Name);

            cbTime.Items.Add(60);

            tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 1234);
            tcpListener.Start();

            tbConsole.Text += "Server started\n";
            Thread acceptingClientsThread = new Thread(AcceptingClients);
            acceptingClientsThread.Start();

        }
        public void AcceptingClients() {
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

            foreach (Auction auction in Auctions) {
                if (auction.Status == 1) {
                    //sender igangværende auktioner til nye klienter
                    BroadCastForNew(tcpClient);
                }
            }

            while (true) {
                string message = reader.ReadLine();
                List<string> commands = message.Split(':').ToList<string>();
                string command = commands[0];
                commands.RemoveAt(0);
                if (command == "Bid") {
                    var diffInSeconds = (Convert.ToDateTime(timer) - DateTime.Now).TotalSeconds;

                    threadMonitor.ThreadAction(tcpClient.Client.LocalEndPoint.ToString() + ": " + (string)commands[0] + "\n");
                    BroadCast("Bid: " + commands[0], tcpClient);
                    Auctions[0].bids.Add(int.Parse(commands[0]), tcpClient);

                    // extend timer by 1 minute
                    if (UnderSixtySeconds == true) {
                        if (diffInSeconds <= 10) {
                            ThreadCountdown.Abort();
                            CountdownRunning = true;
                        }
                        BroadCastExtendedTime();
                    }
                } else {
                    threadMonitor.ThreadAction(tcpClient.Client.LocalEndPoint.ToString() + ": " + message + "\n");
                }
            }
        }
        #region Broadcast
        public static void BroadCast(string msg, TcpClient excludeClient) {
            for (int i = 0; i < tcpClientsList.Count; i++) {
                //if (tcpClientsList[i] != excludeClient) {
                StreamWriter sWriter = new StreamWriter(tcpClientsList[i].GetStream());
                sWriter.WriteLine(msg);
                sWriter.Flush();
                //}
            }
        }
        public static void BroadCastWinner(TcpClient excludeClient) {
            for (int i = 0; i < tcpClientsList.Count; i++) {
                StreamWriter sWriter = new StreamWriter(tcpClientsList[i].GetStream());
                if (tcpClientsList[i] == excludeClient) {
                    sWriter.WriteLine("console|Du har vundet auktionen om \"" + Auctions[0].Name + "\", det endelige bud blev " + Auctions[0].bids.Keys.Last() + "kr.");
                    sWriter.Flush();
                } else {
                    sWriter.WriteLine("console|Brugeren " + excludeClient.Client.LocalEndPoint.ToString() + " vandt auktionen om \"" + Auctions[0].Name + "\" for " + Auctions[0].bids.Keys.Last() + "kr.");
                    sWriter.Flush();
                }
            }
        }
        public void BroadCastForNew(TcpClient newClient) {

            for (int i = 0; i < tcpClientsList.Count; i++) {
                StreamWriter sWriter = new StreamWriter(tcpClientsList[i].GetStream());
                if (tcpClientsList[i] == newClient) {
                    sWriter.WriteLine("SetBiddingItem: " + Auctions[0].Name);
                    sWriter.Flush();
                    sWriter.WriteLine("Time: " + timer);
                    sWriter.Flush();
                    sWriter.WriteLine("Bid: " + Auctions[0].StartingPrice);
                    sWriter.Flush();
                }
            }

        }
        public static void BroadCastCountdown() {
            for (int i = 0; i < tcpClientsList.Count; i++) {
                StreamWriter sWriter = new StreamWriter(tcpClientsList[i].GetStream());
                sWriter.WriteLine("Hammer: Første");
                sWriter.Flush();
            }
            Thread.Sleep(5000);
            for (int i = 0; i < tcpClientsList.Count; i++) {
                StreamWriter sWriter = new StreamWriter(tcpClientsList[i].GetStream());
                sWriter.WriteLine("Hammer: Anden");
                sWriter.Flush();
            }
            Thread.Sleep(3000);
            for (int i = 0; i < tcpClientsList.Count; i++) {
                StreamWriter sWriter = new StreamWriter(tcpClientsList[i].GetStream());
                sWriter.WriteLine("Hammer: Solgt!");
                sWriter.Flush();
            }
        }
        public void BroadCastExtendedTime() {
            UnderSixtySeconds = false;
            timer = timer.AddMinutes(0.10);
            for (int i = 0; i < tcpClientsList.Count; i++) {
                StreamWriter sWriter = new StreamWriter(tcpClientsList[i].GetStream());
                sWriter.WriteLine("Time: " + timer);
                sWriter.Flush();
            }
        }
        #endregion
        private void Thread_MessageReciever(string message) {
            if (!this.Dispatcher.CheckAccess()) {
                this.Dispatcher.Invoke(new ThreadMonitor.ThreadEventType(Thread_MessageReciever), message);
                return;
            }
            tbConsole.Text += message;
        }


        private void WinnerFound(TcpClient winner) {
            BroadCastWinner(winner);
            AuctionRunning = false;
        }
        #region timer
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
        #endregion

        private void btnAddAuction_Click(object sender, RoutedEventArgs e) {
            string name = tbName.Text;
            int time = int.Parse(cbTime.SelectedValue.ToString());
            Auction newAuction = new Auction(name, time, 10);

            Auctions.Add(newAuction);
            listBoxAuctions.Items.Add(Auctions[Auctions.Count - 1].Name);
        }

        private void btnStartAuction_Click(object sender, RoutedEventArgs e) {
            Auctions[0].Status = 1;
            for (int i = 0; i < tcpClientsList.Count; i++) {
                StreamWriter sWriter = new StreamWriter(tcpClientsList[i].GetStream());

                sWriter.WriteLine("SetBiddingItem: " + Auctions[0].Name);
                sWriter.Flush();
                sWriter.WriteLine("Time: " + DateTime.Now.AddMinutes(Convert.ToDouble(Auctions[0].Time)));
                sWriter.Flush();
                sWriter.WriteLine("Bid: " + Auctions[0].StartingPrice);
                sWriter.Flush();
                timer = DateTime.Now.AddMinutes(Convert.ToDouble(Auctions[0].Time));
                AuctionTimer();
                AuctionRunning = true;
            }
        }
    }
}
