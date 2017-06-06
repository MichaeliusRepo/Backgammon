using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ModelDLL
{
    class FinalStatesCalculator
    {
        internal static Node AllReachableStatesTree(GameBoardState initialState, CheckerColor color, List<int> moves)
        {
            var root = new Node(initialState, color, null, null, moves);
            return root;
        }
    }

    


    internal class Node
    {

        private static int nodesCreated = 0;

        internal readonly GameBoardState state;
        internal readonly FinalStateMove moveMadeToGetHere;
        internal readonly Node parent;
        internal readonly List<int> movesLeft;
        internal readonly List<Node> children = new List<Node>();
        internal readonly CheckerColor color;

        private readonly int NodeID;

        internal Node(GameBoardState state, CheckerColor color, FinalStateMove moveMadeToGetHere, Node parent, List<int> movesLeft)
        {

            


            this.state = state;
            this.moveMadeToGetHere = moveMadeToGetHere;
            this.parent = parent;
            this.movesLeft = movesLeft;
            this.color = color;

            nodesCreated++;
            this.NodeID = nodesCreated;
           // Console.WriteLine("Creating node: " + nodesCreated);
          //  Console.WriteLine(PathToTop());

            var moveable = MovesCalculator.GetMoveableCheckers(state, color, movesLeft);

            GameBoardState tmpState = null;
            FinalStateMove tmpMove = null;
            Node tmpNode = null;


            if (movesLeft.Distinct().Count() < movesLeft.Count())
            {
                int move = movesLeft.ElementAt(0);
                foreach (int position in moveable)
                {
                    tmpState = GameBoardMover.Move(state, color, position, move);
                    if (tmpState != null)
                    {
                        tmpMove = new ModelDLL.FinalStateMove(position, move);
                        tmpNode = new ModelDLL.Node(tmpState, color, tmpMove, this, movesLeft.Without(move));
                        children.Add(tmpNode);
                    }
                }
            }
            else
            {
                foreach (int move in movesLeft)
                {
                    //Console.WriteLine("move: " + move);
                    foreach (int position in moveable)
                    {
                        tmpState = GameBoardMover.Move(state, color, position, move);
                        if (tmpState != null)
                        {
                            tmpMove = new ModelDLL.FinalStateMove(position, move);
                            tmpNode = new ModelDLL.Node(tmpState, color, tmpMove, this, movesLeft.Without(move));
                            children.Add(tmpNode);
                        }
                    }
                }
            }
        }

        internal IEnumerable<Node> GetFinalStates()
        {
            List<Node> output = new List<Node>();
            GetFinalStatesRecursion(output);
            return output;
        }

        private void GetFinalStatesRecursion(List<Node> output)
        {
            if (children.Count() == 0)
            {
                output.Add(this);
            }
            else
            {
                foreach(Node child in children)
                {
                    child.GetFinalStatesRecursion(output);
                } 
            }
        }

        internal List<FinalStateMove> MovesMade()
        {
            List<FinalStateMove> output = new List<FinalStateMove>();
            Node current = this;
            while(current.parent != null)
            {
                output.Add(current.moveMadeToGetHere);
                current = current.parent;
            }
            return output;
        }

        private string PathToTop()
        {
            Node tmp = this;
            string output = "";
            while(tmp != null)
            {
                output += tmp.NodeID + "<-";
                tmp = tmp.parent;
            }
            output += "null";
            return output;
        }
    }

    class FinalStateMove
    {
        internal readonly int from;
        internal readonly int distance;

        public FinalStateMove(int from, int distance)
        {
            this.from = from;
            this.distance = distance;
        }
    }

    
}
