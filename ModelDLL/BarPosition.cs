using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    class BarPosition : Position
    {
        //It is legal to move from the bar only if there are checkers on the bar
        //and if the the bar trying be moved from belongs to the player trying to move
        public bool IsLegalToMoveFromHere(GameBoardState state, CheckerColor color, int thisPosition)
        {
            if(thisPosition != color.GetBar())
            {
                return false;
            }
            else
            {
                return state.getCheckersOnBar(color) > 0;
            }
        }

        //It is allways illegal to make a move to the bar
        public bool IsLegalToMoveHere(GameBoardState state, CheckerColor color, int fromPosition, int thisPosition)
        {
            return false;
        }

        public GameBoardState MoveCheckerHere(GameBoardState state, CheckerColor color, int position)
        {
            throw new NotImplementedException();
        }

        public GameBoardState MoveCheckerFromHere(GameBoardState state, CheckerColor color, int position)
        {
            return state.WhereCheckerIsRemovedFromBar(color);
        }
    }
}
