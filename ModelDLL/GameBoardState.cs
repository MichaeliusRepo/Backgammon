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
        public static readonly GameBoardState DefaultGameBoardState = new GameBoardState(BackgammonGame.DefaultGameBoard, 0, 0, 0, 0);

        private readonly CheckerColor WHITE = CheckerColor.White;
        private readonly CheckerColor BLACK = CheckerColor.Black;

        private Dictionary<int, int> gameBoard = new Dictionary<int, int>();

        private static readonly int MaximumNumberOfMoves = 4;

        private static readonly List<List<int>> AllRolledTwice = new List<List<int>>() {
                new List<int>() { 1,2 },
                new List<int>() { 1,3 },
                new List<int>() { 1,4 },
                new List<int>() { 1,5 },
                new List<int>() { 1,6 },
                new List<int>() { 2,3 },
                new List<int>() { 2,4 },
                new List<int>() { 2,5 },
                new List<int>() { 2,6 },
                new List<int>() { 3,4 },
                new List<int>() { 3,5 },
                new List<int>() { 3,6 },
                new List<int>() { 4,5 },
                new List<int>() { 4,6 },
                new List<int>() { 5,6 }
        };

        private static readonly List<List<int>> AllRolledOnce = new List<List<int>>() {
            new List<int>() {1,1,1,1},
            new List<int>() {2,2,2,2},
            new List<int>() {3,3,3,3},
            new List<int>() {4,4,4,4},
            new List<int>() {5,5,5,5},
            new List<int>() {6,6,6,6}
        };

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

        internal int pip(CheckerColor color)
        {
            if(color == BLACK)
            {
                return this.InvertColor().pip(WHITE);
            }

            return gameBoard.Where(kv => kv.Key >= 1 && kv.Key <= 24).Where(kv => kv.Value > 0).Select(kv => kv.Key * kv.Value).Sum() + getCheckersOnBar(WHITE)*25;
        }

        internal int capturableCheckers(CheckerColor color)
        {
            if(color == BLACK)
            {
                return InvertColor().capturableCheckers(WHITE);
            }

            return gameBoard.Where(kv => kv.Key >= 1 && kv.Key <= 24).Where(kv => kv.Value == 1).Count();
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

        private double ProbabilityOfWhiteCapturing()
        {
            return InvertColor().ProbabilityOfWhiteGettingCaptured();
        }

        public double ProbabilityOfWhiteGettingCaptured()
        {
          

            if (getCheckersOnBar(BLACK) > 0)
             {
                  return ProbabilityOfWhiteGettingCapturedIfBlackHasCheckersOnBar();
             }

            return ProbabilityOfWhiteGettingCaptured(AllRolledTwice, AllRolledOnce);
        }

        private Predicate<int> IsInMainBoard = delegate (int i) { return i >= 1 && i <= 24; };

        private double ProbabilityOfWhiteGettingCaptured(List<List<int>> movesThatAppearTwiceLeft, List<List<int>> movesThatAppearOnceLeft)
        {
            //A subset of the game board excluding the bars and target positions
            IEnumerable<KeyValuePair<int, int>> mainBoard = gameBoard.Where(kv => IsInMainBoard(kv.Key));


            //The positions of the capturable white checkers/the white checkers that are standing alone
            IEnumerable<int> capturable = mainBoard.Where(kv => kv.Value == 1).Select(kv => kv.Key);

            //If no checkers are captruable, then the probability is zero
            if (capturable.Count() == 0) return 0;


            //The positions of the black checkers on the board
            IEnumerable<int> blackCheckers = mainBoard.Where(kv => kv.Value < 0).Select(kv => kv.Key);


            double a = movesThatAppearTwiceLeft.Where(moves => BlackCapturesGivenMoves(capturable, blackCheckers, moves)).Count() * 2.0 / 36;
            double b = movesThatAppearOnceLeft.Where(moves => BlackCapturesGivenMoves(capturable, blackCheckers, moves)).Count() / 36.0;

            return a + b;
        }

        private double ProbabilityOfWhiteGettingCapturedIfBlackHasCheckersOnBar()
        {
            var allRolledWtwice = AllRolledTwice.Where(movesTMP => BlackCapturesGivenMoves(movesTMP));
            double a = allRolledWtwice.Count() * 2.0 / 36;

            var allRolledOnce = AllRolledOnce.Where(movesTMP => BlackCapturesGivenMoves(movesTMP));
            double b = allRolledOnce.Count() / 36.0;


            return a + b;
        }
       
        //Given a collection of positions describing where there are white checkers that are capturable,
        //the positions of the black checkers and a list of moves, determines whether or not at least one 
        //white checker can be captured by a black checker
        private bool BlackCapturesGivenMoves(IEnumerable<int> capturable, IEnumerable<int> blackCheckers, List<int> moves)
        {
            //Gets all the positions that are reachable for the black checkers
            var blackReachable = blackCheckers

                                //Creates a collection of collections of ints. 
                                //Each of the ints represent a position that a black checker can get to
                                .Select(p => new MovesCalculator(this, BLACK, p, moves).GetReachablePositions()) 

                                //Combines the multiple collections in the previous line into one collection
                                .Aggregate((a, b) => a.Concat(b));


            //Finding the positions that appear in both the capturable white checkers
            //and the reachable positions for black
            var checkersBlackCanCapture = blackReachable.Intersect(capturable);

            //Returns true if there is at least one such position
            return checkersBlackCanCapture.Count() > 0;

        }

        private bool BlackCapturesGivenMoves(List<int> moves)
        {
            //var finalStates = MovesCalculator.GetFinalStates(this, BLACK, moves);
            //var finalStates = new FinalStatesCalculator(new List<GameBoardState>() { this }, BLACK, moves).Calculate();
            var finalStates = FinalStatesCalculator2.AllReachableStatesTree(this, BLACK, moves).GetFinalStates().Select(node => node.state);
           
            
            // Console.WriteLine("Number of final states: " + finalStates.Count());
            //Console.WriteLine(string.Join(",", finalStates.Select(s => s.Stringify() + "\n\n\n")));
            return BlackCaptured(finalStates);
        }

        private bool BlackCaptured(IEnumerable<GameBoardState> states)
        {
            return states.Where(s => s.getCheckersOnBar(WHITE) > this.getCheckersOnBar(WHITE)).Count() > 0;
        }



        public int[] getMainBoard()
        {
            return gameBoard
                .Where(kv => IsInMainBoard(kv.Key))
                .OrderBy(kv => kv.Key)
                .Select(kv => kv.Value)
                .ToArray();

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
