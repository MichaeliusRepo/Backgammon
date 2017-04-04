using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    public class GameBoardState
    {
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
                throw new InvalidOperationException("There is not the expected number of checkers");
            }
        }

        public int[] getMainBoard()
        {
            return mainBoard;
        }

        public int getWhiteCheckersOnBar()
        {
            return whiteCheckersOnBar;
        }

        public int getBlackCheckersOnBar()
        {
            return blackCheckersOnBar;
        }

        public int getWhiteCheckersOnTarget()
        {
            return whiteCheckersOnTarget;
        }

        public int getBlackCheckersOnTarget()
        {
            return blackCheckersOnTarget;
        }
    }
}
