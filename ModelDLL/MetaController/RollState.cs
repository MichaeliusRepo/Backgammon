using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    internal class RollState : State
    {
        internal RollState(BackgammonGame bg, CheckerColor color) : base(bg, color)
        {}

        internal override void Execute()
        {
            int[] dice = bg.dice.RollDice();
            int i = dice[0], j = dice[1];
            if (i == j)
            {
                bg.moves = new List<int>() { i, i, i, i };
            }
            else
            {
                bg.moves = new List<int>() { i, j };
            }
            bg.NextState();
        }

        internal override State NextState()
        {
            return new CheckLegalMovesState(bg, color);
        }
    }
}
