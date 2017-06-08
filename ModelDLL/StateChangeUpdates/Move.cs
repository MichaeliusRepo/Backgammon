using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static ModelDLL.CheckerColor;

namespace ModelDLL
{
    [DataContractAttribute]
    public class Move : Change
    {
        [DataMember]
        public readonly CheckerColor color;
        [DataMember]
        public readonly int from;
        [DataMember]
        public readonly int to;

        internal static Move NoMove = new Move(White, 0, 0);

        internal Move(CheckerColor color, int from, int to)
        {
            this.color = color;
            this.from = from;
            this.to = to;
        }

        internal static Move CapturingMove(CheckerColor color, int from)
        {
            return new Move(color, from, color.GetBar());
        }

        public override string ToString()
        {
            return "Move: " + (color == CheckerColor.White ? "W" : "B") + " from " + from + " to " + to;
        }

        public string DebugString()
        {
            return (color == CheckerColor.White ? "w" : "b") + " " + from + " " + to;
        }

        public override bool Equals(Object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            Move move = (Move)obj;
            return (color == move.color) &&
                   (from == move.from) &&
                   (to == move.to);
        }

        public override int GetHashCode()
        {
            return from ^ to  * (color == White? 1 : 17);
        }
    }

    public class Turn
    {
        public List<Move> moves = new List<Move>();
        public List<int> dice = new List<int>();
        public CheckerColor color;

        public Turn(){ }

        public Turn(CheckerColor color, List<Move> moves, List<int> dice)
        {
            this.color = color;
            this.moves = moves;
            this.dice = dice;
        }
    }
}
