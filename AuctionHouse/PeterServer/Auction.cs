using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PeterServer {
    class Auction {
        public Dictionary<int, TcpClient> bids = new Dictionary<int, TcpClient>();
        public int Status { get; set; }
        public string Name { get; set; }
        public int Time { get; set; }
        public int StartingPrice { get; set; }

        public Auction(string name, int time, int startingprice) {
            this.Status = 0;
            this.Name = name;
            this.Time = time;
            this.StartingPrice = startingprice;
        }
    }
}
