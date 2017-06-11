using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Contains implementations for fake clients and server, so that BackgammonGames communication 
 * through the server can be tested without actually using a server
 * 
 */

namespace ModelDLL
{
    public interface Client
    {
        void SendDataToServer(string data);
        void SendDataToPlayer(string data);
    }

    class FakeClient : Client
    {
        internal RemotePlayer player;
        internal FakeServer server;

        internal FakeClient(RemotePlayer player, FakeServer server)
        {
            this.player = player;
            this.server = server;
        }

        public void SendDataToPlayer(string data)
        {
            player.ReceiveData(data);
        }

        public void SendDataToServer(string data)
        {
            server.Send(this, data);
        }
    }

    class FakeServer
    {
        internal Client client1;
        internal Client client2;

        internal FakeServer(Client client1, Client client2)
        {
            this.client1 = client1;
            this.client2 = client2;
        }

        internal void Send(Client fromClient, string data)
        {
            if (fromClient == client1)
            {
                client2.SendDataToPlayer(data);
            }
            else
            {
                client1.SendDataToPlayer(data);
            }
        }
    }

    class ConsoleView : View
    {
        private string identifier;
        private BackgammonGame model;

        internal ConsoleView(BackgammonGame model, string identifier)
        {
            this.model = model;
            this.identifier = identifier;
        }

        public void NotifyView()
        {

           /* Console.WriteLine("----------------------------------------------------");
            Console.WriteLine("Moves left" + string.Join(",", model.GetMovesLeft()));
            Console.WriteLine("Number of final reachable states: " + MovesCalculator.GetReachableStatesThisTurn(model.GetGameBoardState(), model.playerToMove(), model.GetMovesLeft()).Count());
            Console.WriteLine("----------------------------------------------------");
            //Console.ReadLine();*/

             var changes = model.GetChanges();
             if (changes.Where(c => c is Move).Count() > 0)
             {
                 Console.WriteLine("----------------------------------------------------");
                 Console.WriteLine("This is coming from ConsoleView: " + identifier);
                 Console.WriteLine(model.GetGameBoardState().Stringify());
                 Console.WriteLine("----------------------------------------------------");
             }
        }
    }
}
