﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionHouseServerCLI {
    class Program {
        static void Main(string[] args) {
            string biddingItem = Console.ReadLine();
            new Server(biddingItem, 1337);
        }
    }
}
