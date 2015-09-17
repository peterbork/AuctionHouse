using System;

namespace ThreadMonitor {
    internal class ThreadEventType {
        private Action<string> thread_ChangeAuctionHeighestBid;

        public ThreadEventType(Action<string> thread_ChangeAuctionHeighestBid) {
            this.thread_ChangeAuctionHeighestBid = thread_ChangeAuctionHeighestBid;
        }
    }
}