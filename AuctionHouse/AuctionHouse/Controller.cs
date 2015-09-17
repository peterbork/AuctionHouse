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
        DateTime time;
        bool finished = false;

        public Controller(MainWindow window) {
            mainWindow = window;
            // Set up element events
            threadMonitor.NewBidEvent += mainWindow.BidReceiver;
            threadMonitor.ChangeBiddingItemEvent += mainWindow.SetBiddingItem;
            threadMonitor.UpdateTimeRemainingEvent += mainWindow.TimeRemaining;

            client = new Client(this, "127.0.0.1", 1234);
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
                threadMonitor.NewBid(parameter);
            } else if (command == "SetBiddingItem") {
                threadMonitor.ChangeBiddingItem(parameter);
            } else if (command == "Time") {
                time = Convert.ToDateTime(parameter);
                Thread timeThread = new Thread(UpdateTimer);
                timeThread.Start();
            } else if (command == "Hammer") { // time
                finished = true;
                threadMonitor.UpdateTimeRemaining(parameter);
            } else if (command == "Error") {
                mainWindow.DisplayError(parameter);
            }
        }

        public void UpdateTimer() {
            while (!finished) {
                string timeRemaining = time.Subtract(DateTime.Now).ToString("hh\\:mm\\:ss");
                threadMonitor.UpdateTimeRemaining(timeRemaining);
                Thread.Sleep(1000);
            }
        }
    }
}
