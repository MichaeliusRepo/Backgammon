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
        private BearOffPosition whiteBearOff;
        private BearOffPosition blackBearOff;

        public GameBoard(int[] gameBoard)
        {
            this.initialize(gameBoard, 0, 0, 0, 0);
        }

        public GameBoard(int[] gameBoard,
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

            this.whiteBearOff = new BearOffPosition(BearOffPosition.WHITE_BEAR_OFF_ID, whiteCheckersBoreOff);
            this.blackBearOff = new BearOffPosition(BearOffPosition.BLACK_BEAR_OFF_ID, blackCheckersBoreOff);

            //Creating 24 positions
            for (int i = 0; i < 24; i++)
            {
                positions[i] = new Position(i + 1, gameBoard[i], whiteBar, blackBar);
            }

            Edge[] whiteBarEdges = whiteBar.GetEdges(CheckerColor.White);
            Edge[] blackBarEdges = blackBar.GetEdges(CheckerColor.Black);
            for (int i = 0; i < 6; i++)
            {
                whiteBarEdges[i] = new Edge(positions[23 - i], Edge.AlwaysLegalEdge, positions);
                blackBarEdges[i] = new Edge(positions[i], Edge.AlwaysLegalEdge, positions);
            }


            Predicate<Position[]> whiteHomeBoardFilled = (Position[] p) =>
            {
                int checkers = 0;
                for (int i = 0; i < 6; i++)
                {
                    checkers += positions[i].NumberOfCheckersOnPosition(CheckerColor.White);
                }
                checkers += whiteBearOff.NumberOfCheckersOnPosition(CheckerColor.White);
                return checkers == 15;
            };

            Edge whiteBearOffEdge = new Edge(whiteBearOff, whiteHomeBoardFilled, null);


            Predicate<Position[]> noWhiteCheckers = (Position[] p) =>
            {
                int checkers = 0;
                foreach(Position pos in p)
                {
                    checkers += pos.NumberOfCheckersOnPosition(CheckerColor.White);
                }
                return checkers == 0;
            };

            Predicate<Position[]> canBearOffWhiteOvershoot = (Position[] p) => {
                return whiteHomeBoardFilled(null) && noWhiteCheckers(p);
            };

            Predicate<Position[]> blackHomeBoardFilled = (Position[] p) => {

                int blackCheckers = 0;
                for (int i = 18; i < 24; i++)
                {
                    blackCheckers += positions[i].NumberOfCheckersOnPosition(CheckerColor.Black);
                }
                blackCheckers += whiteBearOff.NumberOfCheckersOnPosition(CheckerColor.Black);
                int totalNumberOfCheckers = 15;
                
                return blackCheckers == totalNumberOfCheckers;
            };

            Edge blackBearOffEdge = new Edge(blackBearOff, blackHomeBoardFilled, null);

            Predicate<Position[]> noBlackCheckers = (Position[] p) => {
                int checkers = 0;
                foreach (Position pos in p)
                {
                    checkers += pos.NumberOfCheckersOnPosition(CheckerColor.Black);
                }
                return checkers == 0;
            };

            Predicate<Position[]> canBearOffBlackOverShoot = (Position[] p) =>
            {
                return blackHomeBoardFilled(null) && noBlackCheckers(p);
            };

            //Updating the neighbours of the first 6 positions, taking into consideration any edge cases 
            for (int i = 0; i < 6; i++)
            {
                Position pos = positions[i];

                Edge[] whiteEdges = pos.GetEdges(CheckerColor.White);
                Edge[] blackEdges = pos.GetEdges(CheckerColor.Black);

                for (int j = 0; j < 6; j++)
                {
                    blackEdges[j] = new Edge(positions[i + j + 1], Edge.AlwaysLegalEdge, positions);
                }


                //Initializing all white edges to be the bear off position
                for (int j = 0; j < 6; j++)
                {
                    Position[] homeBoardGreaterThanI = getSubArray(positions, i + 1, 5);
                    whiteEdges[j] = new Edge(whiteBearOff, canBearOffWhiteOvershoot, homeBoardGreaterThanI);
                }
                whiteEdges[i] = whiteBearOffEdge;

                //Fixing for the edges that should not be to bear off position
                for (int j = 0; i - j - 1 >= 0; j++)
                {
                    whiteEdges[j] = new Edge(positions[i - j - 1], Edge.AlwaysLegalEdge, positions);
                }
            }

            //Updating the neighbours of the middle twelve positions, avoiding any edge cases
            for (int i = 6; i <= 17; i++)
            {
                Position pos = positions[i];

                Edge[] whiteEdges = pos.GetEdges(CheckerColor.White);
                Edge[] blackEdges = pos.GetEdges(CheckerColor.Black);

                for (int j = 0; j < 6; j++)
                {
                    whiteEdges[j] = new Edge(positions[i - j - 1], Edge.AlwaysLegalEdge, positions);
                    blackEdges[j] = new Edge(positions[i + j + 1], Edge.AlwaysLegalEdge, positions);
                }
            }

            //Updating the neightbours of the last 6 positions, edge cases
            for (int i = 18; i < 24; i++)
            {
                Position pos = positions[i];
                Edge[] whiteEdges = pos.GetEdges(CheckerColor.White);
                Edge[] blackEdges = pos.GetEdges(CheckerColor.Black);

                //There are more than 6 elements to the left of all the elements we are looking at, so no edge cases here
                for (int j = 0; j < 6; j++)
                {
                    whiteEdges[j] = new Edge(positions[i - j - 1], Edge.AlwaysLegalEdge, positions);
                }


                
                for (int j = 0; j < 6; j++)
                {
                    Position[] homeBoardSmallerThanI = getSubArray(positions, 18, i - 1);
                    blackEdges[j] = new ModelDLL.Edge(blackBearOff, canBearOffBlackOverShoot, homeBoardSmallerThanI);
                    //blackEdges[j] = blackBearOffEdge;
                }
                blackEdges[23-i] = blackBearOffEdge;

                //There are less than 6 elements to the right of the elements we are looking at, so we need to be careful
                for (int j = 0; i + j + 1 < 24; j++)
                {
                    blackEdges[j] = new Edge(positions[i + j + 1], Edge.AlwaysLegalEdge, positions);
                }
            }
        }

        public int[] GetGameBoard()
        {
            int[] gameBoard = new int[positions.Length];
            for(int i = 0; i < gameBoard.Length; i++)
            {
                gameBoard[i] = positions[i].GetCheckers();
            }
            return gameBoard;
        }

        internal void move(CheckerColor color, int from, int distance, int[] moves)
        {
            if(!positions[from-1].isLegalMove(color, distance, moves))
            {
                throw new InvalidOperationException();
            }

            positions[from - 1].removeChecker(color);

            Position target = positions[from + (color == CheckerColor.White ? -distance : distance) - 1];
            target.addChecker(color);
        }

        public int GetCheckersOnBar(CheckerColor color)
        {
            return (color == CheckerColor.White ?
                whiteBar.NumberOfCheckersOnPosition(CheckerColor.White) :
                blackBar.NumberOfCheckersOnPosition(CheckerColor.Black));
        }

        public int GetCheckersOnTarget(CheckerColor color)
        {
            return (color == CheckerColor.White ?
                whiteBearOff.NumberOfCheckersOnPosition(CheckerColor.White) :
                blackBearOff.NumberOfCheckersOnPosition(CheckerColor.Black));
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


        private void printEdges()
        {
            foreach (Position pos in positions)
            {
                foreach(Edge e in pos.GetEdges(CheckerColor.White)){
                    Console.Write(e.ToString() + ", ");
                }
                Console.Write("   ");
                foreach(Edge e in pos.GetEdges(CheckerColor.Black))
                {
                    Console.Write(e.ToString() + ", ");
                }
                Console.WriteLine();
            }
        }

        private Position[] getSubArray(Position[] from, int firstIndex, int lastIndex)
        {
            Position[] output = new Position[lastIndex - firstIndex + 1];
            for(int i = 0; i < output.Length; i++)
            {
                output[i] = from[firstIndex + i];
            }
            return output;
        }
    }
}
 