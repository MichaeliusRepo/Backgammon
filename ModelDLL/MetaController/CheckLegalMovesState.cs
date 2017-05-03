using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    class CheckLegalMovesState : State
    {
        private bool hasLegalMoves;
        private State nextState;
        public CheckLegalMovesState(BackgammonGame bg, CheckerColor color) : base(bg, color){}

        internal override void Execute()
        {
            hasLegalMoves = bg.GetMoveableCheckers(color).Count() > 0;
            if (hasLegalMoves)
            {
                nextState = new MoveState(bg, color);
            }
            else
            {
                nextState = new RollState(bg, color.OppositeColor());
            }
            bg.NextState();
        }

        internal override State NextState()
        {
            return nextState;
        }
    }
}
