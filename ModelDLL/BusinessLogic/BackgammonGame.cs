using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace ModelDLL
{
  
    public class BackgammonGame
    {
        public static readonly int[] DefaultGameBoard = new int[] {
                                           -2, 0, 0, 0,  0,  5,
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

        private int numberOfMovesMade = 0;

        internal int NumberOfTurnsMade { get; private set; }



        internal CheckerColor turnColor;
        internal Dice dice;
        internal List<int> movesLeft;

       // private Turn currentTurn = new Turn();
       // private List<Turn> turnHistory = new List<Turn>();

        private GameBoardState currentGameBoardState;

        private PlayerInterface whitePlayer;
        private PlayerInterface blackPlayer;

        private Turn CurrentTurn = null;
        private Turn PreviousTurn = null;


        //NOTIFYING THE VIEW

        private List<View> observers = new List<View>();

        private List<Change> Changes = new List<Change>();

        public List<Change> GetChanges()
        {
            return new List<Change>(Changes);
        }

        public void ConnectView(View view)
        {
            this.observers.Add(view);
        }

        public void DisconnectView(View view)
        {
            observers.Remove(view);
        }

        internal void NotifyAllViews()
        {
            foreach (var observer in observers)
            {
                observer.NotifyView();
            }
            if (observers.Count() > 0)
            {
                Changes = new List<Change>();
            }
        }

        //END OF NOTIFYING THE VIEW


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

        internal Turn GetPreviousTurn()
        {
            return PreviousTurn;
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
            recalculateMoves();
            this.CurrentTurn = new Turn(turnColor, new List<Move>(), new List<int>(movesLeft));

            //No need for this, as it is done in recalculate moves
            //this.Changes.Add(new DiceState(movesLeft.ToArray()));

            //this.currentTurn.dice = new List<int>(movesLeft);

            this.currentGameBoardState = new GameBoardState(gameBoard, whiteCheckersOnBar, whiteCheckersBoreOff, blackCheckersOnBar, blackCheckersBoreOff);
            
            whitePlayer = new PlayerInterface(this, WHITE, null);
            blackPlayer = new PlayerInterface(this, BLACK, null);



        }
        //Constructors end

        public PlayerInterface ConnectPlayer(CheckerColor color, Player player)
        {
            var pi = color == WHITE ? whitePlayer : blackPlayer;
            if (pi.HasPlayer())
            {
                return null;
            }
            pi.SetPlayerIfNull(player);
            player.ConnectPlayerInterface(pi);
            return pi;
        }

        internal void SetMovesLeft(List<int> movesLeft)
        {
            this.movesLeft = movesLeft;
        }

        public HashSet<int> GetLegalMovesFor(CheckerColor color, int initialPosition)
        {
            MovesCalculator root = new MovesCalculator(currentGameBoardState, color, initialPosition, GetMovesLeft());
            return new HashSet<int>(root.GetReachablePositions());

        }

        internal IEnumerable<Node> GetFinalStates()
        {
            return FinalStatesCalculator.AllReachableStatesTree(currentGameBoardState, playerToMove(), movesLeft).GetFinalStates();
        } 

        internal void MoveToFinalState(CheckerColor color, Node finalState)
        {
            if (color != playerToMove())
            {
                throw new InvalidOperationException(color + " can't move when it is " + color.OppositeColor() + "'s turn (move to final state)");
            }

            currentGameBoardState = finalState.state;
            movesLeft = new List<int>();

            List<FinalStateMove> movesMade = finalState.MovesMade();
            foreach(FinalStateMove move in movesMade)
            {
                //TODO REMOVE THIS
                numberOfMovesMade++;
                NotifyView(color, move.from, GameBoardMover.GetPositionAfterMove(color, move.from, move.distance));
            }
            if (IsGameOver())
            {
                Console.WriteLine("Game is over!! Terminating");
                return;
            }
            changeTurns();
            (turnColor == WHITE ? whitePlayer : blackPlayer).MakeMove();

        }

        public void EndTurn(CheckerColor color)
        {
            if(color != turnColor)
            {
                throw new InvalidOperationException("Can't end " + color + "'s turn when it is " + color.OppositeColor() + "'s turn");
            }
            if(GetMoveableCheckers().Count() > 0)
            {
                throw new InvalidOperationException("Can't end turn when there are legal moves available");
            }
            changeTurns();
        }

        public List<int> Move(CheckerColor color, int from, int targetPosition)
        {

            if (color != playerToMove())
            {
                throw new InvalidOperationException(color + " can't move when it is " + color.OppositeColor() + "'s turn");
            }

            //TODO REMOVE THIS SHIT
            numberOfMovesMade++;

            MovesCalculator mts = new MovesCalculator(currentGameBoardState, color, from, movesLeft);
            if (!mts.LegalToMoveToPosition(targetPosition))
            {
                throw new InvalidOperationException("Illegal to move " + color + " form " + from + " to " + targetPosition);
            }

            MovesCalculator.MoveState resultingState = mts.MoveToPosition(targetPosition);
            currentGameBoardState = resultingState.state;
            movesLeft = resultingState.movesLeft;
            Changes.AddRange(resultingState.changes);
            NotifyAllViews();

            foreach(Change change in resultingState.changes)
            {
                if (change is Move)
                {
                    CurrentTurn.moves.Add(change as Move);
                }
            }

            NotifyView(color, from, targetPosition);
            if (IsGameOver())
            {
                Console.WriteLine("Game is over!! Terminating");
                return new List<int>();
            }

            if (movesLeft.Count() == 0)
            {
                changeTurns();
            }
            else if (GetMoveableCheckers().Count() == 0)
            {
                changeTurns();
            }

            return new List<int>();

            
        }

        private void NotifyView(CheckerColor color, int from, int to) 
        {
          /*  Console.WriteLine("Moves made: " + numberOfMovesMade);
            Console.WriteLine("----------------------------------------------\n" + 
                              "Moving " + color + " from " + from + " to " + to + ". Moves left are: " + string.Join(",", movesLeft) + "\n" + currentGameBoardState.Stringify() + 
                              "\n--------------------------------------------------");*/
        }

        internal bool GameIsOver()
        {
            return this.currentGameBoardState.GetCheckersOnPosition(WHITE.GetBar()) == 15 ||
                   this.currentGameBoardState.GetCheckersOnPosition(BLACK.GetBar()) == 15;
        }

        private void changeTurns()
        {
            //recalculateMoves();
            turnColor = turnColor.OppositeColor();
            changeTurns(turnColor);

            //if (GetMoveableCheckers().Count() == 0)
            //{
             //   changeTurns();
            //}
           
            
        }

        private void changeTurns(CheckerColor color)
        {
            recalculateMoves();
            turnColor = color;

            this.PreviousTurn = this.CurrentTurn;
            this.CurrentTurn = new Turn(color, new List<Move>(), new List<int>(movesLeft));
            NumberOfTurnsMade++;
        }

        private bool IsGameOver()
        {
            return currentGameBoardState.getCheckersOnTarget(WHITE) == 15 || currentGameBoardState.getCheckersOnTarget(BLACK) == 15;
        }


        //Returns the list of moves that remains to be used
        public List<int> GetMovesLeft()
        {
            return new List<int>(movesLeft);
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
            return MovesCalculator.GetMoveableCheckers(currentGameBoardState, color, movesLeft).ToList();
        }

        private void recalculateMoves()
        {
            movesLeft = new List<int>();
            int[] diceValues = dice.RollDice();
            if (diceValues[0] == diceValues[1])
            {
                movesLeft = new List<int>() { diceValues[0], diceValues[0], diceValues[0], diceValues[0] };
            }
            else
            {
                movesLeft = new List<int>() { diceValues[0], diceValues[1] };
            }
            Changes.Add(new DiceState(movesLeft.ToArray()));
            NotifyAllViews();

        }

        //Meta rules below here
        public CheckerColor playerToMove()
        {
            return this.turnColor;
        }
    }
}