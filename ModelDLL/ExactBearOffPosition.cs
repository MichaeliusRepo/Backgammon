using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    class ExactBearOffPosition : Position
    {
        public bool IsLegalToMoveFromHere(GameBoardState state, CheckerColor color, int thisPosition)
        {
            return false;
        }

        public virtual bool IsLegalToMoveHere(GameBoardState state, CheckerColor color, int fromPosition, int thisPosition)
        {
            if(thisPosition != color.BearOffPositionID())
            {
                return false;
            }
            else
            {
                return state.NumberOfCheckersInHomeBoard(color) == GameBoardState.NUMBER_OF_CHECKERS_PER_PLAYER;
            }
        }

        public GameBoardState MoveCheckerHere(GameBoardState state, CheckerColor color, int position)
        {
            return state.WhereCheckerIsAddedToTarget(color);
        }

        public GameBoardState MoveCheckerFromHere(GameBoardState state, CheckerColor color, int position)
        {
            throw new NotImplementedException();
        }
    }
}
