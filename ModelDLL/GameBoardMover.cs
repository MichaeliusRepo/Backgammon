using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    class GameBoardMover
    {

        private static CheckerColor WHITE = CheckerColor.White;
        private static CheckerColor BLACK = CheckerColor.Black;


        // *** This is the most important method in this class. Start reading the code here. ***
        public static GameBoardState Move(GameBoardState state, CheckerColor color, int from, int distance)
        {
            if (!IsLegalMove(state, color, from, distance))
            {
                return null;
            }

            int toPosition = GetPositionAfterMove(color, from, distance);
            return MoveChecker(state, color, from, toPosition);
        }

        static bool IsLegalMove(GameBoardState state, CheckerColor color, int from, int distance)
        {
            int targetPosition = GetPositionAfterMove(color, from, distance);
            return IsLegalToMoveFromPosition(state, color, from) &&
                   IsLegalToMoveToPosition(state, color, targetPosition); 
        }

        static bool IsLegalToMoveFromPosition(GameBoardState state, CheckerColor color, int position)
        {

            if(IsBar(position))
            {
                return IsLegalToMoveFromBar(state, color, position);
            }

            //Checks that there is a checker at the from position that can be moved
            bool hasACheckerToMove = state.NumberOfCheckersOnPosition(color, position) > 0;
            bool noCheckersOnBar = state.getCheckersOnBar(color) == 0;
            return hasACheckerToMove && noCheckersOnBar;
        }

        private static bool IsLegalToMoveFromBar(GameBoardState state, CheckerColor color, int position)
        {
            //If the position is not equal to the ID of the bar of the checker color, then
            //we are, for example, trying to move a white checker from the black bar.
            //In this case, the move is obviously not legal
            if( position != color.GetBar())
            {
                return false;
            }

            //Otherwise, it required that there is a checker on the bar
            return state.getCheckersOnBar(color) > 0;
        }


        static bool IsLegalToMoveToPosition(GameBoardState state, CheckerColor color, int position)
        {
            //Return false if the target position is outside the board
            if (position < 1) return false;
            if (position > GameBoardState.NUMBER_OF_POSITIONS_ON_BOARD) return false;

            //Otherwise, there has to be less than two enemy checkers on the position.
            return PositionIsOpen(state, color, position);
        }



        //Checks whether or not there are less than two enemy checkers on the given position
        //returns true if this holds, false if it doesn't. 
        static bool PositionIsOpen(GameBoardState state, CheckerColor color, int position)
        {
            CheckerColor opponentColor = (color == WHITE ? BLACK : WHITE);
            int opponentCheckers = state.NumberOfCheckersOnPosition(opponentColor, position);

            return opponentCheckers <= 1;
        }


        //Given a checker color, starting position and a distance to travel, returns the position 
        //that the checker will move to
        internal static int GetPositionAfterMove(CheckerColor color, int from, int distance)
        {


            //The white and black bar has unique IDs, that do not represent their location 
            //with regards to the rest of the game board.
            //In the case that we get one of these IDs, we transform them to be their actual
            //position with regards to the board and recall this method 
            if (IsBar(from))
            {
                return GetPositionAfterMove(color, color.BarPositionWithRegardsToBoard(), distance);
            }
            
            //White checkers have a negative change in positions for positive distances
            //Black checkers have a positive change in positions for positive distances            
            return from + color.ChangeInPositionDueToTravelDistance(distance);
        }

        static GameBoardState MoveChecker(GameBoardState state, CheckerColor color, int from, int to)
        {

            if(from == color.GetBar())
            {
                return state
                    .WhereCheckerIsRemovedFromBar(color)
                    .WhereCheckerIsAddedToPosition(color, to);
            }

            return state
                .WhereCheckerIsRemovedFromPosition(color, from)
                .WhereCheckerIsAddedToPosition(color, to);
        }

        private static bool IsBar(int position)
        {
            return position == WHITE.GetBar() || position == BLACK.GetBar();
        }
    }
}
