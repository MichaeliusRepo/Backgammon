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
        internal Dice dice;
        //private GameBoard gameBoard;
        internal List<int> moves;

        internal State state;
        internal void NextState()
        {
            state = state.NextState();
            state.Execute();
        }



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
            this.turnColor = playerToMove;
            this.dice = dice;


            this.currentGameBoardState = new GameBoardState(gameBoard, whiteCheckersOnBar, whiteCheckersBoreOff, blackCheckersOnBar, blackCheckersBoreOff);
            this.state = new RollState(this, playerToMove);
            state.Execute();
        }

        //Constructors end

        public HashSet<int> GetLegalMovesFor(CheckerColor color, int initialPosition)
        {
            MoveTreeState root = new MoveTreeState(currentGameBoardState, color, initialPosition, GetMovesLeft());
            return root.GetReachablePositions();

        }

        public List<int> Move(CheckerColor color, int from, int targetPosition)
        {
            if(color != state.PlayerToMove())
            {
                throw new InvalidOperationException();
            }

            MoveTreeState mts = new MoveTreeState(currentGameBoardState, color, from, moves);
            if (mts.LegalToMoveToPosition(targetPosition))
            {
                MoveTreeState resultingState = mts.MoveToPosition(targetPosition);
                currentGameBoardState = resultingState.GetState();
                moves = resultingState.GetMovesLeft();

                NextState();
                return resultingState.GetMovesTaken();
            }
            else
            {
                throw new InvalidOperationException("The move is illegal");
            }

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
            return GetMoveableCheckers(playerToMove());
        }

        public List<int> GetMoveableCheckers(CheckerColor color)
        {

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

        //Meta rules below here
        public CheckerColor playerToMove()
        {
            // return this.turnColor;
            return state.PlayerToMove();
        }

    }
}




