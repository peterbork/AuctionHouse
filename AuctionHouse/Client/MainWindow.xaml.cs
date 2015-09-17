using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace PeterClient {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
       StreamWriter sWriter;
        ThreadMonitor threadMonitor = new ThreadMonitor("Client");
        bool AuctionRunning;
        string time;
        public MainWindow() {
            InitializeComponent();
            threadMonitor.ThreadEvent += Thread_MessageReciever;
            threadMonitor.ThreadChangeAuctionItemEvent += Thread_ChangeAuctionItem;
            threadMonitor.ThreadChangeAuctionTimeEvent += Thread_ChangeAuctionTime;
            threadMonitor.ThreadChangeAuctionHeighestBidEvent += Thread_ChangeAuctionHeighestBid;

            
            TcpClient tcpClient = new TcpClient("10.140.65.85", 1234);
            tbConsole.Text += "Connected to server.";

            sWriter = new StreamWriter(tcpClient.GetStream());

            Thread thread = new Thread(Read);
            thread.Start(tcpClient);

            
        }
        private void Read(object obj) {
            TcpClient tcpClient = (TcpClient)obj;
            StreamReader sReader = new StreamReader(tcpClient.GetStream());

            while (true) {
                string message = sReader.ReadLine();
                List<string> commands = message.Split('|').ToList<string>();
                string command = commands[0];
                commands.RemoveAt(0);

                if (command == "bid") {

                } else if (command == "console") {
                    threadMonitor.ThreadAction(commands[0]);
                } else if (command == "ChangeAuctionItem") {
                    threadMonitor.ThreadChangeAuctionItem(commands[0]);
                } else if (command == "Time") {
                    threadMonitor.ThreadChangeAuctionTime(commands[0]);
                } else if (command == "HeighestBid") {
                    threadMonitor.ThreadChangeAuctionHeighestBid(commands[0]);
                }
            }
        }

        private void btnSend_Click(object sender, RoutedEventArgs e) {
            string input = tbMessage.Text;
            sWriter.WriteLine(input);
            sWriter.Flush();
            tbMessage.Text = "";
        }
        private void Thread_MessageReciever(string message) {
            if (!this.Dispatcher.CheckAccess()) {
                this.Dispatcher.Invoke(new ThreadMonitor.ThreadEventType(Thread_MessageReciever), message);
                return;
            }
            tbConsole.Text += "\n" + message;
        }

        private void Thread_ChangeAuctionItem(string message) {
            if (!this.Dispatcher.CheckAccess()) {
                this.Dispatcher.Invoke(new ThreadMonitor.ThreadEventType(Thread_ChangeAuctionItem), message);
                return;
            }
            lblName.Content = message;
        }
        private void Thread_ChangeAuctionTime(string message) {
            if (!this.Dispatcher.CheckAccess()) {
                this.Dispatcher.Invoke(new ThreadMonitor.ThreadEventType(Thread_ChangeAuctionTime), message);
                return;
            }
            time = message;
            AuctionTimer();
            AuctionRunning = true;

        }
        public void AuctionTimer() {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(UpdateTimer);
            timer.Interval = new TimeSpan(1000); // in miliseconds
            timer.Start();
        }

        private void UpdateTimer(object sender, EventArgs e) {
            if(AuctionRunning == true) { 
                DateTime currentTime = DateTime.Now;
                var diffInSeconds = (Convert.ToDateTime(time) - currentTime).TotalSeconds;
                var ts = TimeSpan.FromSeconds(diffInSeconds);
                lblTime.Content = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);

                if (diffInSeconds <= 0) {
                    AuctionRunning = false;
                }
            }
        }

        private void Thread_ChangeAuctionHeighestBid(string message) {
            if (!this.Dispatcher.CheckAccess()) {
                this.Dispatcher.Invoke(new ThreadMonitor.ThreadEventType(Thread_ChangeAuctionHeighestBid), message);
                return;
            }
            lblHeighestBid.Content = message;
        }

        private void btnSendBid_Click(object sender, RoutedEventArgs e) {
            if (tbBidPrice.Text == "") {
                MessageBox.Show("Du kan ikke sende et tomt bud");
            } else if (int.Parse(tbBidPrice.Text) <= int.Parse(lblHeighestBid.Content.ToString())) {
                MessageBox.Show("Dit bud skal være højere");
            } else {
                string input = tbBidPrice.Text;
                sWriter.WriteLine("bid|" + input);
                sWriter.Flush();
                tbBidPrice.Text = "";
            }
        }
    }
}
