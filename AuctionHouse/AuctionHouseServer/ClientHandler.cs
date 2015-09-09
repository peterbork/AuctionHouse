using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;


namespace AuctionHouseServer {
    public class ClientHandler {
        private Socket clientSocket;
        private NetworkStream netStream;    // nu objekt data og ikke lokale - så de kan bruges på tværs af metoder
        private StreamWriter writer;        // nu objekt data og ikke lokale - så de kan bruges på tværs af metoder
        private StreamReader reader;        // nu objekt data og ikke lokale - så de kan bruges på tværs af metoder

        private BroadcastService broadcastService;    // Chat funktioner og samtidig monitor for fælles resource
        private Server server;

        public ClientHandler(Server server, Socket clientSocket, BroadcastService broadcastService) {
            this.server = server;
            this.clientSocket = clientSocket;
            this.broadcastService = broadcastService;
        }

        public void RunClient() {
            this.netStream = new NetworkStream(clientSocket);
            this.writer = new StreamWriter(netStream);
            this.reader = new StreamReader(netStream);

            // Send bidding item to client
            sendToClient("SetBiddingItem: " + server.GetBiddingItem());

            // Send current bid to client
            double bid = server.GetBid();

            if (bid > 0)
                sendToClient("Bid: " + bid);

            doDialog();

            this.writer.Close();
            this.reader.Close();
            this.netStream.Close();
            this.clientSocket.Shutdown(SocketShutdown.Both);
            this.clientSocket.Close();
        }

        //// alternativ - med using der tager sig af at lukke også ved fejl (try og finaly med close )
        //public void RunClient()
        //{
        //    using (this.netStream = new NetworkStream(clientSocket))  // bemærk at disse nu er "lever" på objekt
        //    using (this.writer = new StreamWriter(netStream))
        //    using (this.reader = new StreamReader(netStream))
        //    {
        //        doDialog();
        //    }
        //    this.clientSocket.Shutdown(SocketShutdown.Both);
        //    this.clientSocket.Close();
        //}

        private void sendToClient(string text) {
            writer.WriteLine(text);
            writer.Flush();
        }

        private string receiveFromClient() {
            try {
                return reader.ReadLine();
            } catch {
                return null;
            }
        }

        private void doDialog() {
            try {
                broadcastService.BroardCastEvent += this.BroadcastAction;      // tilføj klientens metode til broadcast tjeneste

                while (executeCommand()) ;       // fortsæt udførsel sålænge tue
            } catch { } finally {
                broadcastService.BroardCastEvent -= this.BroadcastAction;      // fjern klientens metode til broadcast tjeneste
            }
        }

        private bool executeCommand()  //returner false hvis null eller bye
        {
            // Behandling af input fra klient
            string input = receiveFromClient();

            if (input == null)
                return false;
            if (input.Trim().ToLower() == "bye")
                return false;

            // Behandling af andre komandoer
            string command = input.Split(':')[0];

            if (command == "Bid") {
                double bid = double.Parse(input.Split(':')[1]);
                if (server.ApplyBid(bid)) {
                    broadcastService.BroadCastBesked(input);
                } else {
                    sendToClient("Error: Et bud over dit modtaget.");
                }
            }

            // sendToClient(("Echo:" + input);        // ændring fra echo server - ikke med i chat
            Console.WriteLine(input);


            return true;
        }

        public void BroadcastAction(string msg) {
            sendToClient(msg);
        }
    }
}
