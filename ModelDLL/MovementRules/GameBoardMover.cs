using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    class GameBoardMover
    {

        /* Important note to understand the code in this code file.
         * 
         * Many of the code in the internal classes of the Backgammon game model deals explicitly only with cases where white is concerned. Whenever a request 
         * regarding the black player is made, the GameBoardState and parameters for the request are transformed, or inverted, so that was was the black player is now
         * the white player. The problem can then be solved using the code dealing with the white player, and the result is transformed back into what the client expects. 
         * 
         * The assumption that we have to deal only cases relating to the white player holds true for all the internal classes in the code file.
         * 
         */


        private static CheckerColor WHITE = CheckerColor.White;
        private static CheckerColor BLACK = CheckerColor.Black;


        //Performs a move and returns the resulting state. If the move is illegal, null will be returned instead.
        internal static GameBoardState Move(GameBoardState state, CheckerColor color, int initialPosition, int distance)
        { 

            //Since moving a checker from the black player is identical to moving a checker from the white player, if the white player's
            //situation was the same as the black player, the game board and input is inverted so this is the case, and the code for
            //moving a white checker is reused
            if(color == BLACK)
            {
                //Transforming the input
                state = state.InvertColor();
                initialPosition = convertTo(WHITE, initialPosition);

                state = Move(state, WHITE, initialPosition, distance);

                //Returns null if the move is invalid, else transforms the move 
                //back into the perspective of the black player and returns it.
                return state == null ? null : state.InvertColor();
            }

            int targetPosition = initialPosition - distance;
            if (IsLegalMove(state, initialPosition, targetPosition))
            {
                //The previous definition of the target position is useful for determining whether or not 
                //A move is legal. However, we risk getting negative values, for example when 
                //the white player bears off a checker on position 4 using a die that shows 5
                //Therefore the /actual/ target position is found using the below method call
                targetPosition = GetPositionAfterMove(color, initialPosition, distance);
                return state.MoveChecker(initialPosition, targetPosition);
            }
            else return null;
        }

        //Given a state, an initial position and a target position, returns true if it is legal for the white player
        //to move a checker from the initial position to the target position, or false if not
        private static bool IsLegalMove(GameBoardState state, int from, int targetPosition)
        { 
            return IsLegalToMoveFromPosition(state, from) && IsLegalToMoveToPosition(state, from, targetPosition);
        }

        //Given a state and a position, returns true if it is legal for the white player can move a checker from that position
        private static bool IsLegalToMoveFromPosition(GameBoardState state, int position)
        {
            //If the position is the bar, there has to be at least one checker on the bar
            if (position == WHITE.GetBar())
            {
                return state.getCheckersOnBar(WHITE) > 0;
            }
            //If not, there cannot be any checkers on the bar, the position has to be on the board, and there has to be at
            //least one checker on that position
            else
            {
                if (state.getCheckersOnBar(WHITE) > 0) return false;
                if (position < 1 || position > 24) return false;
                return state.getMainBoard()[position - 1] > 0;
            }
        }


        //Given a state, an initial position and a target position, returns true if the white player 
        //can move a checker from the initial position to the target position, or false if not.
        private static bool IsLegalToMoveToPosition(GameBoardState state, int fromPosition, int toPosition)
        {
            //If the target position is between -5 and 0, then the white player is trying to bear off a checker
            if (toPosition <= 0 && toPosition >= -5) return IsLegalToBearOff(state, fromPosition, toPosition);

            //If not, make sure the position is on the board and that there are less than two enemy checkers there
            if (toPosition < 1 || toPosition > 24) return false;
            return state.getMainBoard()[toPosition - 1] > -2;
        }

        //Given a state, an initial position and a target position, returns true if it is legal for white
        //to bear off a checker from the initial position to the target position
        private static bool IsLegalToBearOff(GameBoardState state, int from, int to)
        {
            //Check that the home board (including the position white bears off to) is filled with all whites checkers
            if (state.NumberOfCheckersInHomeBoard() != GameBoardState.NUMBER_OF_CHECKERS_PER_PLAYER) return false;
            if (to == 0) return true;

            //If the target position is less than 0, then white is for example trying to carry off a checker 
            //from position 4 using a move of 5. For this to be legal, we must ensure that there are no checkers 
            //in the home board positiond on a position greater than 4. 
            return state.NumberOfCheckersInHomeBoardFurtherAwayFromBar(from) == 0;
        }

        //Given a checker color, starting position and a distance to travel, returns the position 
        //that the checker will move to, from the perspective of the client.
        //For instance, if a checker moves past the bear off position, the client will still think the checker is 
        //at the bear off position
        internal static int GetPositionAfterMove(CheckerColor color, int from, int distance)
        {
            if(color == BLACK)
            {
                int equivalentWhitePosition = GetPositionAfterMove(WHITE, convertTo(WHITE, from), distance);
                return convertTo(BLACK, equivalentWhitePosition);
            }
            if (from == WHITE.GetBar()) from = 25;
            int pos = from - distance;
            if (pos <= 0)
            {
                return WHITE.BearOffPositionID();
            }
            else return pos;
        }


        //Given a position and a color, assumes the position is with regards to the other color and 
        //converts it to the perspective of the given color
        private static int convertTo(CheckerColor color, int i)
        {
            if (i == color.OppositeColor().GetBar()) return color.GetBar();
            if (i == color.OppositeColor().BearOffPositionID()) return color.BearOffPositionID();
            return 25 - i;
        }
    }
}
