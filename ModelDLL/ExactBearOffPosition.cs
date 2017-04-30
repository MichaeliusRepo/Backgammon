using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                Debug.WriteLine("Illegal to bear off from pos " + fromPosition + " since trying to bear off white to black... or vice versa");
                return false;
            }
            else
            {
                bool isLegal = state.NumberOfCheckersInHomeBoard(color) == GameBoardState.NUMBER_OF_CHECKERS_PER_PLAYER;
                if (isLegal)
                {
                    Debug.WriteLine("Legal to bear off  from pos " + fromPosition + " since correct number of checkers in home board");
                    return true;
                }
                else
                {
                    Debug.WriteLine("Illeegal to bear off from pos " + fromPosition + " since incorrect number of checkers in home board");
                    Debug.WriteLine("There are only " + state.NumberOfCheckersInHomeBoard(color) + " checkers in the home board");
                    return false;
                }
                
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
