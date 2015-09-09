using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionHouseServer {
    class BroadcastService {
        public delegate void BroadcastEventHandler(string msg);

        public event BroadcastEventHandler BroadcastEvent;

        private string name;

        public BroadcastService(string name) {
            this.name = name;
        }
        public string Name {
            get { return name; }
        }

        public void BroadcastMessage (string msg) {
            if (BroadcastEvent != null)
                BroadcastMessage(msg);
        }
    }
}
