using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    class NormalPosition : Position
    {
        public bool IsLegalToMoveFromHere(GameBoardState state, CheckerColor color, int thisPosition)
        {
            return state.NumberOfCheckersOnPosition(color, thisPosition) > 0 && state.getCheckersOnBar(color) == 0;
        }

        public bool IsLegalToMoveHere(GameBoardState state, CheckerColor color, int fromPosition, int thisPosition)
        {
            CheckerColor oppositeColor = color.OppositeColor();
            return state.NumberOfCheckersOnPosition(oppositeColor, thisPosition) <= 1;
        }

        public GameBoardState MoveCheckerHere(GameBoardState state, CheckerColor color, int position)
        {
            GameBoardState temporaryState = state;

            CheckerColor opponent = color.OppositeColor();
            if(state.NumberOfCheckersOnPosition(opponent, position) == 1)
            {
                temporaryState = state
                    .WhereCheckerIsAddedToBar(opponent)
                    .WhereCheckerIsRemovedFromPosition(opponent, position);

            }
            return temporaryState.WhereCheckerIsAddedToPosition(color, position);
        }

        public GameBoardState MoveCheckerFromHere(GameBoardState state, CheckerColor color, int position)
        {
            return state.WhereCheckerIsRemovedFromPosition(color, position);
        }
    }
}
