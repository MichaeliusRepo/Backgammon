using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    class InvalidPosition : Position
    {
        public bool IsLegalToMoveFromHere(GameBoardState state, CheckerColor color, int thisPosition)
        {
            return false;
        }

        public bool IsLegalToMoveHere(GameBoardState state, CheckerColor color, int fromPosition, int thisPosition)
        {
            return false;
        }

        public GameBoardState MoveCheckerHere(GameBoardState state, CheckerColor color, int position)
        {
            throw new InvalidOperationException("Illegal to move to non existant position");
        }

        public GameBoardState MoveCheckerFromHere(GameBoardState state, CheckerColor color, int position)
        {
            throw new InvalidOperationException("Illegal to move from non existant position");
        }
    }
}
