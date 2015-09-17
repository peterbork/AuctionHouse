using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterClient {
    class ThreadMonitor {
        public delegate void ThreadEventType(string message);
        public event ThreadEventType ThreadEvent;
        public event ThreadEventType ThreadChangeAuctionItemEvent;
        public event ThreadEventType ThreadChangeAuctionTimeEvent;
        public event ThreadEventType ThreadChangeAuctionHeighestBidEvent;

        string Name;

        public ThreadMonitor(string name) {
            this.Name = name;
        }

        public void ThreadAction(string message) {
            ThreadEvent(message);
        }
        public void ThreadChangeAuctionItem(string item) {
            ThreadChangeAuctionItemEvent(item);
        }
        public void ThreadChangeAuctionTime(string time) {
            ThreadChangeAuctionTimeEvent(time);
        }
        public void ThreadChangeAuctionHeighestBid(string startingprice) {
            ThreadChangeAuctionHeighestBidEvent(startingprice);
        }
    }
}
