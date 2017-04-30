using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ModelDLL
{
    //Player1 is represented by positive numbers, 
    //Player2 is represented by negative numbers





    public class BackgammonGame
    {
        public static readonly int[] DefaultGameBoard = new int[] { -2, 0, 0, 0,  0,  5,
                                            0, 3, 0, 0,  0, -5,
                                            5, 0, 0, 0, -3,  0,
                                           -5, 0, 0, 0,  0,  2 };

        private const CheckerColor WHITE = CheckerColor.White;
        private const CheckerColor BLACK = CheckerColor.Black;
        public static readonly int WHITE_BAR_ID = WHITE.GetBar();
        public static readonly int BLACK_BAR_ID = BLACK.GetBar();
        public static readonly int WHITE_BEAR_OFF_ID = WHITE.BearOffPositionID();
        public static readonly int BLACK_BEAR_OFF_ID = BLACK.BearOffPositionID();
        public const int MAX_MOVE_DISTANCE_ACCEPTED = 6;


        private CheckerColor turnColor;
        private Dice dice;
        //private GameBoard gameBoard;
        private List<int> moves;


        private GameBoardState currentGameBoardState;


        //Constructors
        public BackgammonGame(int[] gameBoard, Dice dice, int whiteCheckersOnBar, int whiteCheckersBoreOff,
                             int blackCheckersOnBar, int blackCheckersBoreOff, CheckerColor playerToMove)
        {
            initialize(gameBoard, dice, whiteCheckersOnBar, whiteCheckersBoreOff, blackCheckersOnBar, blackCheckersBoreOff, playerToMove);
        }

        public BackgammonGame(int[] gameBoard, Dice dice, int whiteCheckersOnBar, int whiteCheckersBoreOff,
                             int blackCheckersOnBar, int blackCheckersBoreOff)
        {
            initialize(gameBoard, dice, whiteCheckersOnBar, whiteCheckersBoreOff, blackCheckersOnBar, blackCheckersBoreOff, CheckerColor.White);
        }

        public BackgammonGame(int[] gameBoard, Dice dice)
        {
            initialize(gameBoard, dice, 0, 0, 0, 0, CheckerColor.White);
        }

        private void initialize(int[] gameBoard, Dice dice, int whiteCheckersOnBar, int whiteCheckersBoreOff,
                             int blackCheckersOnBar, int blackCheckersBoreOff, CheckerColor playerToMove)
        {
            //  this.gameBoard = new ModelDLL.GameBoard(gameBoard, whiteCheckersOnBar, whiteCheckersBoreOff, blackCheckersOnBar, blackCheckersBoreOff);
            this.turnColor = playerToMove;
            this.dice = dice;
            recalculateMoves();

            this.currentGameBoardState = new GameBoardState(gameBoard, whiteCheckersOnBar, whiteCheckersBoreOff, blackCheckersOnBar, blackCheckersBoreOff);
        }

        //Constructors end

        public HashSet<int> GetLegalMovesFor(CheckerColor color, int initialPosition)
        {
            MoveTreeState root = new MoveTreeState(currentGameBoardState, color, initialPosition, GetMovesLeft());
            return root.GetReachablePositions();

        }

        public List<int> Move(CheckerColor color, int from, int targetPosition)
        {

            //Debug.WriteLine("-------------------------------------------------");
            //Debug.WriteLine("Moving " + color + " from " + from + " to " + targetPosition);
            if (color != playerToMove())
            {
                throw new InvalidOperationException();
            }

            MoveTreeState mts = new MoveTreeState(currentGameBoardState, color, from, moves);
            if (mts.LegalToMoveToPosition(targetPosition))
            {
                MoveTreeState resultingState = mts.MoveToPosition(targetPosition);
                currentGameBoardState = resultingState.GetState();
                moves = resultingState.GetMovesLeft();
                //Debug.WriteLine("Moves left are: " + string.Join(",", moves));
                //Debug.WriteLine("Done making the move");
                
                //Change turns if no moves, or no legal moves left
                if (moves.Count() == 0)
                {
                    Debug.WriteLine("Changing turns as there are no more moves left.");
                    changeTurns();
                }

                if(GetMoveableCheckers().Count() == 0)
                {
                    Debug.WriteLine("There are still more moves left, but none are legal, so the turn is changed.");
                    changeTurns();
                }

                

                //Debug.WriteLine("Moveable checkers are:");
                //Debug.WriteLine(string.Join(",", GetMoveableCheckers()));
                //Debug.WriteLine("Game board looks like: " + string.Join(",", currentGameBoardState.getMainBoard()));
                //Debug.WriteLine("Checkers on bar, white/black: " + currentGameBoardState.getCheckersOnBar(WHITE) + "/" + currentGameBoardState.getCheckersOnBar(BLACK));
                //Debug.WriteLine("Checkers bore off, white/black: " + currentGameBoardState.getCheckersOnTarget(WHITE) +  "/" + currentGameBoardState.getCheckersOnTarget(BLACK));

                //Debug.WriteLine("-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-");
                return resultingState.GetMovesTaken();
            }
            else
            {
                throw new InvalidOperationException("The move is illegal");
            }

        }


        // TODO Doesnt change turns in the case where there are no legal moves left.
        //Also communicate when turn is changed to view
        private void changeTurns()
        {
            recalculateMoves();
            turnColor = (turnColor == CheckerColor.White ? CheckerColor.Black : CheckerColor.White);
        }


        //Returns the list of moves that remains to be used
        public List<int> GetMovesLeft()
        {
            return new List<int>(moves);
        }


        //Returns the current state of the game
        public GameBoardState GetGameBoardState()
        {
            return currentGameBoardState;
        }


        //Returns a set of integers, representing the positions from which a checker can be moved
        //based on the state of the game and the remina
        public List<int> GetMoveableCheckers()
        {

            CheckerColor color = playerToMove();

            List<int> output = new List<int>();


            //Add all the checkers for which there is at least one legal move to the output
            if (GetLegalMovesFor(color, color.GetBar()).Count() >= 1)
            {
                output.Add(color.GetBar());
            }
            
            for (int i = 1; i <= 24; i++)
            {
                if (GetLegalMovesFor(color, i).Count() >= 1)
                {
                    output.Add(i);
                }
            }
            return output;
        }

        private void recalculateMoves()
        {
            moves = new List<int>();
            int[] diceValues = dice.RollDice();
            if (diceValues[0] == diceValues[1])
            {
                moves = new List<int>() { diceValues[0], diceValues[0], diceValues[0], diceValues[0] };
            }
            else
            {
                moves = new List<int>() { diceValues[0], diceValues[1] };
            }

            //Debug.WriteLine("Recalculating moves. New moves are:");
            //Debug.WriteLine(string.Join(",", moves));

        }

        //Meta rules below here
        public CheckerColor playerToMove()
        {
            return this.turnColor;
        }

    }
}




