using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    public class Move
    {
        public readonly CheckerColor color;
        public readonly int from;
        public readonly int to;

        internal Move(CheckerColor color, int from, int to)
        {
            this.color = color;
            this.from = from;
            this.to = to;
        }

        public override string ToString()
        {
            return "Move: " + (color == CheckerColor.White ? "W" : "B") + " from " + from + " to " + to;
        }

        public string DebugString()
        {
            return (color == CheckerColor.White ? "W" : "B") + " " + from + " " + to;
        }
    }

    public class Turn
    {
        public List<Move> moves = new List<Move>();
        public List<int> dice = new List<int>();

        public Turn(){ }

        public Turn(List<Move> moves, List<int> dice)
        {
            this.moves = moves;
            this.dice = dice;
        }
    }
}
