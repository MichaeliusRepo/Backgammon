using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{

    class Edge
    {
        public static Predicate<Position[]> AlwaysLegalEdge = (Position[] p) => { return true; };
        private static Predicate<Position[]> deadEnd = (Position[] p) => { return false; };
        public static  Edge DeadEdge = new Edge(null, deadEnd ,null);

        private Position target;
        private Predicate<Position[]> condition;
        private Position[] gameBoard;

        public Edge(Position target, Predicate<Position[]> condition, Position[] gameBoard)
        {
            this.target = target;
            this.condition = condition;
            this.gameBoard = gameBoard;
        }

        public bool canBeFollowed()
        {
            return condition(gameBoard);
        }

        public Position getTarget()
        {
            if (!canBeFollowed())
            {
                throw new Exception("The condition for following the edge is not satisfied");
            }
            return target;
        }

        public override string ToString()
        {
            string s = "null";
            if (target != null) {

                s = "" + target.GetId();
            }

            return "->" + s;
        }
    }
}
