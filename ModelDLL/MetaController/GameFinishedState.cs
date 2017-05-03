using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    class GameFinishedState : State
    {
        public GameFinishedState(BackgammonGame bg, CheckerColor color) : base(bg, color)
        {}

        internal override void Execute()
        {
            bg.NextState();
        }

        internal override State NextState()
        {
            return new CheckLegalMovesState(bg, color);
        }
    }
}
