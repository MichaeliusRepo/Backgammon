using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    class MoveState : State
    {
        public MoveState(BackgammonGame bg, CheckerColor color) : base(bg, color)
        {}

        internal override void Execute()
        {
        }

        internal override State NextState()
        {
            return new GameFinishedState(bg, color);
        }
    }
}
