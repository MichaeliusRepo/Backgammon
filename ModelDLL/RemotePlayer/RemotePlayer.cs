using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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

        //Generates XML for the changes that happened during the previous turn,
        //and transmits it to the client, which will pass it to the server
        public void MakeMove()
        {
            var turn = model.GetPreviousTurn();

            var state = model.GetGameBoardState();
            var data = UpdateCreatorParser.CreateXmlForGameBoardState(state, "");

            var moves = turn.moves;
            data += UpdateCreatorParser.GenerateXmlForListOfMoves(moves);

            data += UpdateCreatorParser.GenerateXmlForDice(turn.dice);

            data = "<update>" + data + "</update>";
            client.SendDataToServer(data);
        }

        
        //Receives XML for the changes that happened on the other side of the connection,
        //parses it into moves, and performs those move on the BackgammonGame instance it is 
        //connected to. 
        internal void ReceiveData(string data)
        {
            List<int> newMovesLeft = UpdateCreatorParser.ParseDiceFromXml(data);

            model.UpdateMovesLeft(newMovesLeft);

            List<Move> moves = UpdateCreatorParser.ParseListOfMoves(data);
            if (moves.None())
            {
                model.EndTurn(color);

            }
            else
            {
                foreach (var move in moves)
                {
                    if (move.color == this.color)
                        model.Move(move.color, move.from, move.to);
                }
            }
        }

        
    }
}
