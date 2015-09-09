using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public void DisplayError(string message) {
            MessageBox.Show(message);
        }
    }
}
