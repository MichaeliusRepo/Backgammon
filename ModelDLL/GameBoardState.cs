using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    public class GameBoardState
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


        public const int NUMBER_OF_POSITIONS_ON_BOARD = 24;
        public const int FIRST_POSITION_ON_BOARD = 1;
        public const int NUMBER_OF_CHECKERS_PER_PLAYER = 15;

        private readonly CheckerColor WHITE = CheckerColor.White;
        private readonly CheckerColor BLACK = CheckerColor.Black;

        private Dictionary<int, int> gameBoard = new Dictionary<int, int>();

        public GameBoardState(int[] mainBoard, int whiteCheckersOnBar, int whiteCheckersOnTarget, int blackCheckersOnBar, int blackCheckersOnTarget)
        {

            //Checking that both players have exactly 15 checkers on the board
            int numberOfWhiteCheckers = mainBoard
                                        .Where(i => i > 0) //Filtering out all positions that have negative numbers, or black checkers, on them
                                        .Sum() + whiteCheckersOnBar + whiteCheckersOnTarget;

            int numberOfBlackCheckers = mainBoard
                                         .Where(i => i < 0) //Filtering out all positions that have positive numbers / white checkers
                                         .Sum() * -1        //Making the black checkers count as positive
                                         + blackCheckersOnBar + blackCheckersOnTarget;

            if (numberOfWhiteCheckers != NUMBER_OF_CHECKERS_PER_PLAYER ||
               numberOfBlackCheckers != NUMBER_OF_CHECKERS_PER_PLAYER)
            {
                throw new InvalidOperationException("There is not the expected number of checkers. There are " + numberOfWhiteCheckers
                      + " white checkers and " + numberOfBlackCheckers + " black checkers");
            }


            //Adding all the supplied information to the dictionary gameBoard
            for (int i = 0; i < mainBoard.Length; i++)
            {
                gameBoard.Add(i + 1, mainBoard[i]);
            }
            gameBoard.Add(WHITE.GetBar(), whiteCheckersOnBar);
            gameBoard.Add(BLACK.GetBar(), blackCheckersOnBar);
            gameBoard.Add(WHITE.BearOffPositionID(), whiteCheckersOnTarget);
            gameBoard.Add(BLACK.BearOffPositionID(), blackCheckersOnTarget);
        }

        private GameBoardState(Dictionary<int, int> gameBoard)
        {
            this.gameBoard = gameBoard;
        }

        //Returns true if pos is a position in the homeboard of white, or the position white is bearing off to 
        private Predicate<int> PositionIsInHomeBoard = delegate (int pos)
        {
            if (pos >= 1 && pos <= 6) return true;
            if (pos == CheckerColor.White.BearOffPositionID()) return true;
            return false;
        };

        //Returns the number of white checkers in white's home board
        internal int NumberOfCheckersInHomeBoard()
        {
            return gameBoard
                    .Where(kv => PositionIsInHomeBoard(kv.Key))  //Select the positions in whites home board
                    .Select(kv => Math.Max(0, kv.Value))          //Map each key value pair to the number of white checkers (Math.Max in case there are black checkers on the position)
                    .Sum();                                       //Return the sum
        }


        //Returns the number of white checkers in white's home board, that are at a position greater than the parameter position
        internal int NumberOfCheckersInHomeBoardFurtherAwayFromBar(int position)
        {
            return gameBoard
                   .Where(kv => kv.Key >= 1 && kv.Key <= 6)    //Select the positions in white's homeboard
                   .Where(kv => kv.Key > position)                //Select the positions that are also greater than position
                   .Select(kv => Math.Max(0, kv.Value))            //Map each key-value pair to the number of white checkers (Math.Max in case of black checkers on the position)
                   .Sum();                                         //Return the sum
        }


        //TODO COMMENT THE BELOW
        //ASSUMPTION IS THAT WE ARE ONLY MOVING WHITE CHECKERS!!!!
        internal GameBoardState MoveChecker(int from, int to)
        {

            Dictionary<int, int> copy = new Dictionary<int, int>(gameBoard);

            copy[from]--;

            //If enemy checker on position to move to, capture that checker
            if (copy[to] == -1)
            {
                copy[to] = 1;
                copy[BLACK.GetBar()]++;
            }
            else
            {
                copy[to]++;
            }
            return new GameBoardState(copy);
        }

        public int[] getMainBoard()
        {
            int[] mainBoard = new int[24];
            for(int i = 1; i <= 24; i++)
            {
                mainBoard[i - 1] = gameBoard[i];
            }
            return mainBoard;

        }

        public int GetCheckersOnPosition(int position)
        {
            if (!gameBoard.Keys.Contains(position))
            {
                throw new InvalidOperationException("No position '" + position + "' on the game board");
            }
            else return gameBoard[position];
        }

        public int getCheckersOnBar(CheckerColor color)
        {
            return GetCheckersOnPosition(color.GetBar());
        }

        public int getCheckersOnTarget(CheckerColor color)
        {
            return GetCheckersOnPosition(color.BearOffPositionID());
        }

        public string Stringify()
        {
            var mainBoard = getMainBoard();
            var s = "";
            for (int i = 1; i <= 24; i++)
            {
                if (mainBoard[i - 1] < 0) s += " ";
                s += i + " ";
                if (i < 10) s += " ";
            }
            s += "\n";
            return s + string.Join(", ", mainBoard) + " bar w/b: " + GetCheckersOnPosition(WHITE.GetBar()) + "/" + GetCheckersOnPosition(BLACK.GetBar())
                                          + " bore off w/b: " + GetCheckersOnPosition(WHITE.BearOffPositionID()) + "/" + GetCheckersOnPosition(BLACK.BearOffPositionID());
        }

        internal GameBoardState InvertColor()
        {
            int[] newBoard = new int[24];
            var mainBoard = getMainBoard();
            for (int i = 0; i < 24; i++)
            {
                newBoard[i] = mainBoard[mainBoard.Length - 1 - i] * -1;
            }
            return new GameBoardState(newBoard,
                                      getCheckersOnBar   (BLACK),
                                      getCheckersOnTarget(BLACK),
                                      getCheckersOnBar   (WHITE),
                                      getCheckersOnTarget(WHITE));
        }
    }
}
