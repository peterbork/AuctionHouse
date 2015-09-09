using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;


namespace AuctionHouseServer {
    class ClientHandler {
        private Socket clientSocket;
        private NetworkStream netStream;
        private StreamWriter writer;
        private StreamReader reader;

        private BroadcastService broadcastService;

        public ClientHandler(Socket clientSocket, BroadcastService broadcastService) {
            this.clientSocket = clientSocket;
            this.broadcastService = broadcastService;
        }
        public void StartClient() {
            this.netStream = new NetworkStream(clientSocket);
            this.writer = new StreamWriter(netStream);
            this.reader = new StreamReader(netStream);

            AddClient();

            this.writer.Close();
            this.reader.Close();
            this.netStream.Close();
            this.clientSocket.Shutdown(SocketShutdown.Both);
            this.clientSocket.Close();
            
        }
        private void SendToClient(string text) {
            writer.WriteLine(text);
            writer.Flush();
        }
        private string ReceiveFromClient() {
            try {
                return reader.ReadLine();
            } catch {
                return null;
            }
        }

        private void AddClient() {
            try {
                SendToClient("Server Klar");
                broadcastService.BroadcastEvent += this.BroadcastAction;
                while (executeCommand()) ;

            } catch {
            } finally {
                broadcastService.BroadcastEvent -= this.BroadcastAction;
            }
        }
        private bool executeCommand() {
            string input = ReceiveFromClient();
            if (input == null) {
                return false;
            } else if (input.Trim().ToLower() == "bye") {
                return false;
            }
            broadcastService.BroadcastMessage(input);
            return true;
        }
        public void BroadcastAction(string msg) {
            SendToClient("Broadcast: " + msg);
        }
    }
}
