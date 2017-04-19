using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    class OverflowBearOffPosition : ExactBearOffPosition
    {
        public override bool IsLegalToMoveHere(GameBoardState state, CheckerColor color, int fromPosition, int thisPosition)
        {
            if( thisPosition != color.OverflowBearOffID() )
            {
                return false;
            }
            if(state.NumberOfCheckersInHomeBoard(color) != GameBoardState.NUMBER_OF_CHECKERS_PER_PLAYER)
            {
                return false;
            }
            return state.NumberOfCheckersInHomeBoardFurtherAwayFromBar(color, fromPosition) == 0;
        }   
    }
}
