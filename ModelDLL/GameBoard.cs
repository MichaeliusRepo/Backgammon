using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    class GameBoard
    {
        Position[] positions = new Position[24];
        private BarPosition whiteBar;
        private BarPosition blackBar;

        public GameBoard(int[] gameBoard)
        {
            this.initialize(gameBoard, 0,0,0,0);
        }

        public GameBoard(int[]gameBoard, 
                         int whiteCheckersOnBar, 
                         int whiteCheckersBoreOff, 
                         int blackCheckersOnBar, 
                         int blackCheckersBoreOff)
        {
            this.initialize(gameBoard, 
                            whiteCheckersOnBar, 
                            whiteCheckersBoreOff,
                            blackCheckersOnBar,
                            blackCheckersBoreOff);
        }
        // constructor end


        private void initialize(
            int[] gameBoard,
            int whiteCheckersOnBar,
            int whiteCheckersBoreOff,
            int blackCheckersOnBar,
            int blackCheckersBoreOff)
        {

            this.whiteBar = new BarPosition(BarPosition.WHITE_BAR_ID,
                                            whiteCheckersOnBar,
                                            CheckerColor.White);

            this.blackBar = new BarPosition(BarPosition.BLACK_BAR_ID,
                                            blackCheckersOnBar,
                                            CheckerColor.Black);


            //Creating 24 positions
            for (int i = 0; i < 24; i++)
            {
                positions[i] = new Position(i + 1, gameBoard[i], whiteBar, blackBar);
            }

            Position[] whiteBarNeighbours = whiteBar.GetNeighboursArray(CheckerColor.White);
            Position[] blackBarNeighbours = whiteBar.GetNeighboursArray(CheckerColor.Black);

            for(int i = 0; i < 6; i++)
            {
                whiteBarNeighbours[i] = positions[23 - i];
                blackBarNeighbours[i] = positions[i];
            }

            //Updating the neighbours of the first 6 positions, taking into consideration any edge cases 
            for (int i = 0; i < 6; i++)
            {
                Position pos = positions[i];
                Position[] whiteNext = pos.GetNeighboursArray(CheckerColor.White);
                Position[] blackNext = pos.GetNeighboursArray(CheckerColor.Black);

                for (int j = 0; j < 6; j++)
                {
                    blackNext[j] = positions[i + j + 1];
                }
                for (int j = 0; i - j - 1 >= 0; j++)
                {
                    whiteNext[j] = positions[i - j - 1];
                }
            }



            //Updating the neighbours of the middle twelve positions, avoiding any edge cases
            for (int i = 6; i <= 17; i++)
            {
                Position pos = positions[i];
                Position[] whiteNext = pos.GetNeighboursArray(CheckerColor.White);
                Position[] blackNext = pos.GetNeighboursArray(CheckerColor.Black);
                for (int j = 0; j < 6; j++)
                {

                    whiteNext[j] = positions[i - j - 1];
                    blackNext[j] = positions[i + j + 1];
                }
            }

            //Updating the neightbours of the last 6 positions, edge cases
            for (int i = 18; i < 24; i++)
            {
                Position pos = positions[i];
                Position[] whiteNext = pos.GetNeighboursArray(CheckerColor.White);
                Position[] blackNext = pos.GetNeighboursArray(CheckerColor.Black);

                //There are more than 6 elements to the left of all the elements we are looking at, so no edge cases here
                for (int j = 0; j < 6; j++)
                {
                    whiteNext[j] = positions[i - j - 1];
                }

                //There are less than 6 elements to the right of the elements we are looking at, so we need to be careful
                for (int j = 0; i + j + 1 < 24; j++)
                {
                    blackNext[j] = positions[i + j + 1];
                }
            }
        }


        public HashSet<int> GetLegalMovesFor(CheckerColor color, int initialPosition, int[] moves)
        {
            switch (initialPosition)
            {
                case BackgammonGame.WHITE_BAR_ID:
                    return whiteBar.GetLegalMoves(color, moves);
                case BackgammonGame.BLACK_BAR_ID:
                    return blackBar.GetLegalMoves(color, moves);
                default:
                    return positions[initialPosition - 1].GetLegalMoves(color, moves);
            }
        }





        public void printPositionsForDebug()
        {
            foreach (Position pos in positions)
            {
                Position[] whiteNext = pos.GetNeighboursArray(CheckerColor.White);
                Position[] blackNext = pos.GetNeighboursArray(CheckerColor.Black);


                string nextWhites = "   Next whites:  "; 
                for(int i = 0; i < whiteNext.Length; i++)
                {
                    if(whiteNext[i] != null)
                    {
                        nextWhites += whiteNext[i].GetId() + ", ";
                    }
                    
                }

                string nextBlack = "   Next blackes:  ";
                for (int i = 0; i < blackNext.Length; i++)
                {
                    if(blackNext[i] != null)
                    {
                        nextBlack += blackNext[i].GetId() + ", ";
                    }
                }

                Console.WriteLine(pos.GetId() + nextWhites + nextBlack);
            }
        }
    }
}