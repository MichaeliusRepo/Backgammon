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


        //Performs a move and returns the resulting state. If the move is illegal, null will be returned instead.
        public static GameBoardState Move(GameBoardState state, CheckerColor color, int initialPosiion, int distance)
        {
            int targetPosition = GetAbsolutePositionAfterMove(color, initialPosiion, distance);
            if (IsLegalMoveInternal(state, color, initialPosiion, targetPosition))
            {
                return MoveChecker(state, color, initialPosiion, targetPosition);
            }
            else return null;
        }


        //Given a state, color, orignal position and travel distance, checks if the specified move is legal
        public static bool IsLegalMove(GameBoardState state, CheckerColor color, int from, int distance)
        {
            return Move(state, color, from, distance) != null;
        }

        //Given a state, color, origin position and target position, checks if the specified move is legal
        private static bool IsLegalMoveInternal(GameBoardState state, CheckerColor color, int from, int targetPosition)
        {
            Position fromPosition = GetPosition(from);
            Position toPosition = GetPosition(targetPosition);

            return fromPosition.IsLegalToMoveFromHere(state, color, from) &&
                     toPosition.IsLegalToMoveHere(state, color, from, targetPosition);
        }



        //Given a checker color, starting position and a distance to travel, returns the position 
        //that the checker will move to, from the perspective of the client.
        //For instance, if a checker moves past the bear off position, the client will still think the checker is 
        //at the bear off position
        internal static int GetPositionAfterMove(CheckerColor color, int from, int distance)
        {
            int pos = GetAbsolutePositionAfterMove(color, from, distance);

            if (pos == WHITE.OverflowBearOffID())
            {
                return WHITE.BearOffPositionID();
            }
            if(pos == BLACK.OverflowBearOffID())
            {
                return BLACK.BearOffPositionID();
            }
            return pos;
        }


        // Performs the specified move, and returns the resulting game board state
        // Throws an exception if the move is illegal. 
        static GameBoardState MoveChecker(GameBoardState state, CheckerColor color, int from, int to)
        {
            if (!IsLegalMoveInternal(state, color, from, to))
            {
                throw new InvalidOperationException("The specified move is illegal");
            }

            Position fromPosition = GetPosition(from);
            Position toPosition = GetPosition(to);

            GameBoardState temporaryState = fromPosition.MoveCheckerFromHere(state, color, from);
            return toPosition.MoveCheckerHere(temporaryState, color, to);
        }

        

        private static int GetAbsolutePositionAfterMove(CheckerColor color, int initialPosition, int distance)
        {

            int pos = initialPosition + color.ChangeInPositionDueToTravelDistance(distance);
            if (IsBar(initialPosition))
            {
                return color.BarPositionWithRegardsToBoard() + color.ChangeInPositionDueToTravelDistance(distance);
            }
            if (pos == WHITE.BearOffPositionWithRegardsToBoard() ||
               pos == BLACK.BearOffPositionWithRegardsToBoard())
            {
                return color.BearOffPositionID();
            }
            else if (pos < WHITE.BearOffPositionWithRegardsToBoard() ||
                     pos > BLACK.BearOffPositionWithRegardsToBoard())
            {
                return color.OverflowBearOffID();
            }
            else
            {
                return pos;
            }
        }

        //Based on the id of the position, returns an instance of the corresponding Position class
        private static Position GetPosition(int position)
        {
            //If the position is one directly on the board, between 1 nad 24, return a normal position
            if(position >= GameBoardState.FIRST_POSITION_ON_BOARD &&
               position <= GameBoardState.NUMBER_OF_POSITIONS_ON_BOARD)
            {
                return new NormalPosition();
            }

            //Return a bar position if the position ID is that of a bar
            else if(IsBar(position))
            {
                return new BarPosition();
            }

            //return an exact bear off position if the position corresponds exactly to that of the bar (i.e., that a checker on position 4 does not try to 
            //bear off using a move of distance 6.
            else if(IsBearOffPosition(position))
            {
                return new ExactBearOffPosition();
            }
            //Return an "overflow" bear off position if the position is past the exact bear off position, for instance when a checker on position 4
            //tries to bear off using a move of distance 6
            else if(position == WHITE.OverflowBearOffID() || position == BLACK.OverflowBearOffID())
            {
                return new OverflowBearOffPosition();
            }
            //If none of the above applies, the position is invalid. Return an invalid position.
            else
            {
                return new InvalidPosition();
            }
        }

        private static bool IsBearOffPosition(int position)
        {
            return position == BLACK.BearOffPositionID() || position == WHITE.BearOffPositionID();
        }

        private static bool IsBar(int position)
        {
            return position == WHITE.GetBar() || position == BLACK.GetBar();
        }
    }
}
