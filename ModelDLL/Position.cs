using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    internal interface Position
    {
        bool IsLegalToMoveFromHere(GameBoardState state, CheckerColor color, int thisPosition);
        bool IsLegalToMoveHere(GameBoardState state,CheckerColor color, int fromPosition, int thisPosition);
        GameBoardState MoveCheckerHere(GameBoardState state, CheckerColor color, int position);
        GameBoardState MoveCheckerFromHere(GameBoardState state, CheckerColor color, int position);
    }
}
