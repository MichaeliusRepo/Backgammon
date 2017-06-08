using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    class RemotePlayer : Player
    {
        private CheckerColor color;
        private Client client;
        private BackgammonGame model;

        public RemotePlayer(BackgammonGame model, Client client, CheckerColor color)
        {
            this.model = model;
            this.client = client;
            this.color = color;
        }
       

        public void ConnectPlayerInterface(PlayerInterface playerInterface)
        {
            throw new NotImplementedException();
        }

        public void MakeMove()
        {
            var turn = model.GetPreviousTurn();
            var moves = turn.moves.Select(move => move.Xmlify());
            var data = string.Join("", moves);
            client.SendDataToServer(data);
        }

        internal void SendData(string data)
        {

            if (data.Length == 0)
            {
                model.EndTurn(this.color);
                return;
            }

            List<Move> moves = new List<Move>();
            var split = data.Split(';');
            foreach (var s in split)
            {
                var components = s.Split(' ');
                //if (components.Count() < 3) break;
                int from = int.Parse(components[1]);
                int to = int.Parse(components[2]);
                if (components[0] == "w")
                {
                    moves.Add(new ModelDLL.Move(CheckerColor.White, from, to));
                }
                else
                {
                    moves.Add(new ModelDLL.Move(CheckerColor.Black, from, to));
                }
            }

            foreach( var move in moves)
            {
                if(move.color == this.color)
                    model.Move(move.color, move.from, move.to);
            }
        }
    }

    internal interface Client
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
            player.SendData(data);
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
            if(fromClient == client1)
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
            var changes = model.GetChanges();
            if(changes.Where(c => c is Move).Count() > 0)
            { 
                Console.WriteLine("----------------------------------------------------");
                Console.WriteLine("This is coming from ConsoleView: "+ identifier);
                Console.WriteLine(model.GetGameBoardState().Stringify());
                Console.WriteLine("----------------------------------------------------");
            }
        }
    }
}
