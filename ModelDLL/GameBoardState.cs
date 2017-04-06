using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    public class GameBoardState
    {
        public const int NUMBER_OF_POSITIONS_ON_BOARD = 24;
        private const int NUMBER_OF_CHECKERS = 15;

        private readonly int[] mainBoard;
        private readonly int whiteCheckersOnBar;
        private readonly int blackCheckersOnBar;
        private readonly int whiteCheckersOnTarget;
        private readonly int blackCheckersOnTarget;

        public GameBoardState(int[] mainBoard, int whiteCheckersOnBar, int whiteCheckersOnTarget, int blackCheckersOnBar, int blackCheckersOnTarget)
        {
            this.mainBoard = mainBoard;
            this.whiteCheckersOnBar = whiteCheckersOnBar;
            this.blackCheckersOnBar = blackCheckersOnBar;
            this.whiteCheckersOnTarget = whiteCheckersOnTarget;
            this.blackCheckersOnTarget = blackCheckersOnTarget;

            int numberOfWhiteCheckers = whiteCheckersOnBar + whiteCheckersOnTarget;
            int numberOfBlackCheckers = blackCheckersOnBar + blackCheckersOnTarget;
            foreach(int i in mainBoard)
            {
                if (i > 0) numberOfWhiteCheckers += i;
                else numberOfBlackCheckers += -1 * i;
            }

            if (numberOfWhiteCheckers != NUMBER_OF_CHECKERS ||
               numberOfBlackCheckers != NUMBER_OF_CHECKERS)
            {
                throw new InvalidOperationException("There is not the expected number of checkers. There are " + numberOfWhiteCheckers
                     + " white checkers and " + numberOfBlackCheckers + " black checkers");
            }
        }

        private GameBoardState(int[] mainBoard, int whiteCheckersOnBar, int whiteCheckersOnTarget, int blackCheckersOnBar, int blackCheckersOnTarget, bool doNotTest)
        {
            this.mainBoard = mainBoard;
            this.whiteCheckersOnBar = whiteCheckersOnBar;
            this.blackCheckersOnBar = blackCheckersOnBar;
            this.whiteCheckersOnTarget = whiteCheckersOnTarget;
            this.blackCheckersOnTarget = blackCheckersOnTarget;
        }

        internal int NumberOfCheckersOnPosition(CheckerColor color, int position)
        {
            int checkers = mainBoard[position-1];
            if (color == CheckerColor.Black)
            {
                checkers *= -1;
            }
            return Math.Max(checkers, 0);
        }

        internal GameBoardState WherePositionsAre(int[] positions)
        {
            return new GameBoardState(positions, 
                                      whiteCheckersOnBar, 
                                      whiteCheckersOnTarget, 
                                      blackCheckersOnBar, 
                                      blackCheckersOnTarget, 
                                      false); 
                                    //TODO TODO TODO TODO TODO TODO TODO TODO TODO TODO TODO TODO  
                                      //this boolean parameter is a hack to avoid testing that the 
                                      //number of checkers is 15.

        }

        internal GameBoardState WithCheckersOnBar(CheckerColor color, int number)
        {
            int updatedWhiteCheckers = (color == CheckerColor.White ? number : this.whiteCheckersOnBar);
            int updatedBlackCheckers = (color == CheckerColor.Black ? number : this.blackCheckersOnBar);

            return new GameBoardState(getMainBoard(), 
                                      updatedWhiteCheckers, 
                                      whiteCheckersOnTarget, 
                                      updatedBlackCheckers, 
                                      blackCheckersOnTarget,
                                      false);
        }

        internal GameBoardState WhereCheckerIsAddedToPosition(CheckerColor color, int position)
        {
            int[] boardCopy = getMainBoard();
            boardCopy[position - 1] += (color == CheckerColor.White ? 1 : -1);
            return this.WherePositionsAre(boardCopy);
        }

        internal GameBoardState WhereCheckerIsRemovedFromPosition(CheckerColor color, int position)
        {
            int[] boardCopy = getMainBoard();
            boardCopy[position - 1] -= (color == CheckerColor.White ? 1 : -1);
            return this.WherePositionsAre(boardCopy);
        }

        internal GameBoardState WhereCheckerIsRemovedFromBar(CheckerColor color)
        {
            return WithCheckersOnBar(color, getCheckersOnBar(color) - 1);
        }

        public int[] getMainBoard()
        {
            int[] copy = new int[mainBoard.Length];
            Array.Copy(mainBoard, copy, mainBoard.Length);
            return copy;
        }

        public int getWhiteCheckersOnBar()
        {
            return whiteCheckersOnBar;
        }

        public int getBlackCheckersOnBar()
        {
            return blackCheckersOnBar;
        }

        public int getCheckersOnBar(CheckerColor color)
        {
            return (color == CheckerColor.White ? getWhiteCheckersOnBar() : getBlackCheckersOnBar() );
        }

        public int getWhiteCheckersOnTarget()
        {
            return whiteCheckersOnTarget;
        }

        public int getBlackCheckersOnTarget()
        {
            return blackCheckersOnTarget;
        }

        public int getCheckersOnTarget(CheckerColor color)
        {
            return (color == CheckerColor.White ? getWhiteCheckersOnTarget() : getBlackCheckersOnTarget());
        }
    }
}
