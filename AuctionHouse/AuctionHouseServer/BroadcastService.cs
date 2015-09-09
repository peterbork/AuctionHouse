using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionHouseServer {
    public class BroadcastService {
        public delegate void BroardCastEventHandler(string msg);  // Intern type-declaration ~ intern klasse - husk public

        public event BroardCastEventHandler BroardCastEvent;      // collection af metoder der skal kaldes

        private string name;

        public BroadcastService(string name) {
            this.name = name;
        }
        public string Name {
            get { return this.name; }
        }

        public void BroadCastBesked(string msg) {
            if (this.BroardCastEvent != null)   // check at objekt findes - mindst een har været tilmeldt med +=
                BroardCastEvent(msg);           // aktiver alle tilmeldte metoder
        }
    }
}
