using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PeterServer {
    class Broadcaster {
        public void BroadCast(string msg, List<TcpClient> clientList) {
            for (int i = 0; i < clientList.Count; i++) {
                //if (tcpClientsList[i] != excludeClient) {
                StreamWriter sWriter = new StreamWriter(clientList[i].GetStream());
                sWriter.WriteLine(msg);
                sWriter.Flush();
                //}
            }
        }
        public void BroadCastWinner(Auction auction, List<TcpClient> clientList, TcpClient excludeClient) {
            for (int i = 0; i < clientList.Count; i++) {
                StreamWriter sWriter = new StreamWriter(clientList[i].GetStream());
                if (clientList[i] == excludeClient) {
                    sWriter.WriteLine("console|Du har vundet auktionen om \"" + auction.Name + "\", det endelige bud blev " + auction.bids.Keys.Last() + "kr.");
                    sWriter.Flush();
                } else {
                    sWriter.WriteLine("console|Brugeren " + excludeClient.Client.LocalEndPoint.ToString() + " vandt auktionen om \"" + auction.Name + "\" for " + auction.bids.Keys.Last() + "kr.");
                    sWriter.Flush();
                }
            }
        }
        public void BroadCastForNew(Auction auction, DateTime timer, List<TcpClient> clientList, TcpClient newClient) {

            for (int i = 0; i < clientList.Count; i++) {
                StreamWriter sWriter = new StreamWriter(clientList[i].GetStream());
                if (clientList[i] == newClient) {
                    sWriter.WriteLine("SetBiddingItem| " + auction.Name);
                    sWriter.Flush();
                    sWriter.WriteLine("Time| " + timer);
                    sWriter.Flush();
                    sWriter.WriteLine("Bid| " + auction.StartingPrice);
                    sWriter.Flush();
                }
            }

        }
        public static void BroadCastCountdown(List<TcpClient> clientList) {
            for (int i = 0; i < clientList.Count; i++) {
                StreamWriter sWriter = new StreamWriter(clientList[i].GetStream());
                sWriter.WriteLine("Hammer| Første");
                sWriter.Flush();
            }
            Thread.Sleep(5000);
            for (int i = 0; i < clientList.Count; i++) {
                StreamWriter sWriter = new StreamWriter(clientList[i].GetStream());
                sWriter.WriteLine("Hammer| Anden");
                sWriter.Flush();
            }
            Thread.Sleep(3000);
            for (int i = 0; i < clientList.Count; i++) {
                StreamWriter sWriter = new StreamWriter(clientList[i].GetStream());
                sWriter.WriteLine("Hammer| Solgt!");
                sWriter.Flush();
            }
        }
        public void BroadCastExtendedTime(DateTime timer, List<TcpClient> clientList) {
            for (int i = 0; i < clientList.Count; i++) {
                StreamWriter sWriter = new StreamWriter(clientList[i].GetStream());
                sWriter.WriteLine("Time| " + timer);
                sWriter.Flush();
            }
        }
    }
}
