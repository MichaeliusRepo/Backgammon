using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ModelDLL
{
    //Player1 is represented by positive numbers, 
    //Player2 is represented by negative numbers



    

    public class BackgammonGame
    {
        public const int WHITE_BAR_ID = BarPosition.WHITE_BAR_ID;
        public const int BLACK_BAR_ID = BarPosition.BLACK_BAR_ID;
        public const int WHITE_BEAR_OFF_ID = BearOffPosition.WHITE_BEAR_OFF_ID;
        public const int BLACK_BEAR_OFF_ID = BearOffPosition.BLACK_BEAR_OFF_ID;


        private CheckerColor turnColor;
        private Dice dice;
        private GameBoard gameBoard;
        private List<int> moves;

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
            this.gameBoard = new ModelDLL.GameBoard(gameBoard, whiteCheckersOnBar, whiteCheckersBoreOff, blackCheckersOnBar, blackCheckersBoreOff);
            this.turnColor = playerToMove;
            this.dice = dice;
            recalculateMoves();
        }

        public HashSet<int> GetLegalMovesFor(CheckerColor color, int initialPosition)
        {
            return gameBoard.GetLegalMovesFor(color, initialPosition, moves.ToArray());
        }

        public void move(CheckerColor color, int from, int distance)
        {
            if(turnColor != color)
            {
                throw new InvalidOperationException("Player " + color.ToString() + " tried to move when it was not his turn");
            }
            if (!moves.Contains(distance))
            {
                throw new InvalidOperationException("A move of that magnitude is not avalable");
            }
            gameBoard.move(color, from, distance, moves.ToArray());
            moves.Remove(distance);
            if(moves.Count() == 0)
            {
                changeTurns();
            }
        }

        private void changeTurns()
        {
            recalculateMoves();
            turnColor = (turnColor == CheckerColor.White ? CheckerColor.Black : CheckerColor.White); 
        }

        public List<int> GetMovesLeft()
        {
            return new List<int>(moves);
        }

        public GameBoardState GetGameBoardState()
        {
            return new GameBoardState(gameBoard.GetGameBoard(),
                                      gameBoard.GetCheckersOnBar(CheckerColor.White),
                                      gameBoard.GetCheckersOnTarget(CheckerColor.White),
                                      gameBoard.GetCheckersOnBar(CheckerColor.Black),
                                      gameBoard.GetCheckersOnTarget(CheckerColor.Black));
                
        }

        public List<int> GetMoveableCheckers()
        {
            List<int> output = new List<int>();
            for(int i = 1; i <= 24; i++)
            {
                if(GetLegalMovesFor(playerToMove(), i).Count() >= 1)
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
            if(diceValues[0] == diceValues[1])
            {
                moves = new List<int>() { diceValues[0], diceValues[0], diceValues[0], diceValues[0] };
            }
            else
            {
                moves = new List<int>() { diceValues[0], diceValues[1]};
            }
            
        }

        //Meta rules below here
        public CheckerColor playerToMove()
        {
            return this.turnColor;
        }

    } 
}




