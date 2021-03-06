﻿using System;
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
using System.Threading;

namespace AuctionHouseServer {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        Server Server;
        public MainWindow() {
            InitializeComponent();
        }

        public void UpdateClientList(List<string> clientIps) {
            ClientListBox.ItemsSource = clientIps;
        }

        private void StartAuction_Click(object sender, RoutedEventArgs e) {
            string biddingItem = BiddingItem.Text;
            Server = new Server(biddingItem, 1337);
            Thread serverThread = new Thread(Server.Start);
            serverThread.Start();
        }


    }
}
