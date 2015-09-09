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
        ThreadMonitor threadMonitor = new ThreadMonitor();

        public Controller(MainWindow window) {
            mainWindow = window;
            // Set up element events
            threadMonitor.NewBidEvent += mainWindow.BidReceiver;
            threadMonitor.ChangeBiddingItemEvent += mainWindow.SetBiddingItem;

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
            Console.WriteLine("called");
            if (command == "Bid") {
                threadMonitor.NewBid(parameter);
            }else if (command == "SetBiddingItem") {
                threadMonitor.ChangeBiddingItem(parameter);
            }else if (command == "Error") {
                mainWindow.DisplayError(parameter);
            }
        }
    }
}
