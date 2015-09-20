using MahApps.Metro.Controls;
using System;
using System.Windows;

namespace AuctionHouse {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow {

        Controller controller;

        public MainWindow() {
            InitializeComponent();
            controller = new Controller(this);
        }

        private void button_Click(object sender, RoutedEventArgs e) {
            try {
                double bid = double.Parse(BidTextBox.Text);
                controller.SendBid(bid);
            } catch(FormatException) {
                MessageBox.Show("Buddet skal være et tal");
            }
        }

        public void SetBiddingItem(string item) {
            if (!this.Dispatcher.CheckAccess()) {
                this.Dispatcher.Invoke(new ThreadMonitor.ThreadEventType(SetBiddingItem), item);  // indirekte recursion, men nu fra GUI tråd
                return;     // stop her, da metoden nu "gentages" (Invoke) fra GUI tråd  
            }
            BiddingItem.Content = item;
        }

        public void BidReceiver(string bid) {
            if (!this.Dispatcher.CheckAccess()) {
                this.Dispatcher.Invoke(new ThreadMonitor.ThreadEventType(BidReceiver), bid);  // indirekte recursion, men nu fra GUI tråd
                return;     // stop her, da metoden nu "gentages" (Invoke) fra GUI tråd  
            }
            HighestBid.Content = bid;
            BidListBox.Items.Add("Bud modtaget på: " + bid);
        }

        public void TimeRemaining(string time) {
            if (!this.Dispatcher.CheckAccess()) {
                this.Dispatcher.Invoke(new ThreadMonitor.ThreadEventType(TimeRemaining), time);  // indirekte recursion, men nu fra GUI tråd
                return;     // stop her, da metoden nu "gentages" (Invoke) fra GUI tråd  
            }
            
            TimeLeft.Content = time;
        }

        public void DisplayError(string message) {
            MessageBox.Show(message);
        }
    }
}
