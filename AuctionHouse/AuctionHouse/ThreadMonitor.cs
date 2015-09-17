using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AuctionHouse {
    class ThreadMonitor {
        public delegate void ThreadEventType(string message);
        public event ThreadEventType NewBidEvent;
        public event ThreadEventType ChangeBiddingItemEvent;
        public event ThreadEventType UpdateTimeRemainingEvent;

        public void NewBid(string message) {
            NewBidEvent(message);
        }
        public void ChangeBiddingItem(string message) {
            ChangeBiddingItemEvent(message);
        }

        public void UpdateTimeRemaining(string message) {
            UpdateTimeRemainingEvent(message);
        }
    }
}
