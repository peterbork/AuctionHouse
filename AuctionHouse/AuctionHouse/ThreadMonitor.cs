using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AuctionHouse {
    class ThreadMonitor {
        public delegate void ThreadEventType(string message);      // Her defineres "metode-typen": returtype=void, parameterliste=(string)
        public event ThreadEventType ThreadEvent;                   // Her erklæres en "delegat-metode", hvor man kan indsætte metoder, der skal kaldes 

        string name;

        public ThreadMonitor(string name) {
            this.name = name;
        }

        public void ThreadAction(string message) {
            ThreadEvent(message);
        }
    }
}
