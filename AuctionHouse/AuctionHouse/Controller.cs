using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AuctionHouse {
    class Controller {

        Client client;
        MainWindow mainWindow;
        ThreadMonitor threadMonitor = new ThreadMonitor("tt");

        public Controller(MainWindow window) {
            mainWindow = window;
            threadMonitor.ThreadEvent += mainWindow.BidReceiver;

            client = new Client(this, "127.0.0.1", 1337);
            client.SetMonitor(threadMonitor);
            Thread h = new Thread(client.Start);
            h.Start();
        }

        public void SendBid(double bid) {
            client.SendBid(bid);
        }

        public void MessageReceiver(string message) {
            string command = message.Split(':')[0];
            string parameter = message.Split(':')[1];

            if (command == "Bid") {
                threadMonitor.ThreadAction(parameter);
            }else if (command == "SetBiddingItem") {
                mainWindow.SetBiddingItem(parameter);
            }else if (command == "Error") {
                mainWindow.DisplayError(parameter);
            }
        }
    }
}
