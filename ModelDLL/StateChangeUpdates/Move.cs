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

        public bool IsMove()
        {
            return true;
        }

        public bool IsDiceState()
        {
            return false;
        }

        public Move AsMove()
        {
            return this;
        }

        public DiceState AsDiceState()
        {
            throw new InvalidOperationException("Tried to cast Move into DiceState");
        }

        internal string Xmlify()
        {
            int to;
            if (CheckerColorExtensions.IsBar(this.to))
            {
                to = color.BarPositionWithRegardsToBoard();
            }
            else if (CheckerColorExtensions.IsBearOffPosition(this.to))
            {
                to = color.BearOffPositionWithRegardsToBoard();
            }
            else to = this.to;
            return "<move>" + (color == CheckerColor.White ? "w" : "b") + " " + from + " " + to + "</move>";
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
