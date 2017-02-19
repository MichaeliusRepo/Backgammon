using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    //Player1 is represented by positive numbers, 
    //Player2 is represented by negative numbers

    public enum CheckerColor { White, Black };


    public class BackgammonGame
    {

        private Dice dice;
        private GameBoard gameBoard;

        public BackgammonGame(int[] gameBoard, Dice dice, int whiteCheckersOnBar, int whiteCheckersBoreOff,
                             int blackCheckersOnBar, int blackCheckersBoreOff)
        {
            this.dice = dice;
            this.gameBoard = new GameBoard(gameBoard);
        }

        public BackgammonGame(int[] gameBoard, Dice dice)
        {
            this.gameBoard = new GameBoard(gameBoard);
            this.dice = dice;
        }

        public HashSet<int> GetLegalMovesFor(CheckerColor color, int initialPosition)
        {
            return gameBoard.GetLegalMovesFor(color, initialPosition, dice.GetDiceValues());
        }
    } 
}




/*private int[] gameBoard;

    private int whiteCheckersOnBar = 0;
    private int whiteCheckersBoreOff = 0;
    private int blackCheckersOnBar = 0;
    private int blackCheckersBoreOff = 0;

    public BackgammonGame(int[] gameBoard, Dice dice, int whiteCheckersOnBar, int whiteCheckersBoreOff, 
                          int blackCheckersOnBar, int blackCheckersBoreOff)
    {
        this.gameBoard = gameBoard;
        this.dice = dice;

        //-------------------
        this.gb = new GameBoard(gameBoard);
        //-------------------

        this.whiteCheckersOnBar = whiteCheckersOnBar;
        this.whiteCheckersBoreOff = whiteCheckersBoreOff;
        this.blackCheckersOnBar = blackCheckersOnBar;
        this.blackCheckersBoreOff = blackCheckersBoreOff;
    }

    public BackgammonGame(int[] gameBoard, Dice dice)
    {
        //-------------------
        this.gb = new GameBoard(gameBoard);
        //-------------------

        this.gameBoard = gameBoard;
        this.dice = dice;
    }

    private int[] AllCombinationOfValuesFromDice()
    { 
        int[] diceVals = dice.GetDiceValues();
        int[] tmp = new int[3];
        tmp[0] = diceVals[0];
        tmp[1] = diceVals[1];
        tmp[2] = tmp[0] + tmp[1];
        Array.Sort(tmp);
        return tmp;
    }


    //White checkers move towards zero, while black checkers move towards 23
    private int NewPositionAfterMove(CheckerColor color, int initialPosition, int moveDistance)
    { 
        return color == CheckerColor.White ? 
            initialPosition - moveDistance : initialPosition + moveDistance; 
    }


    //Given a checker color, and the position the checker is on,
    //returns a set containing all the positions the player can move
    //the checker to. 
    public HashSet<int> GetLegalMovesFor(CheckerColor color, int initialPosition)
    {
       //The set to be returned
        HashSet<int> output = new HashSet<int>();

        //All different distances that can be moved, based on the values of the dice
        int[] moveDistances = AllCombinationOfValuesFromDice();


        foreach (int i in moveDistances)
        {

            //The new position after performing the move
            int newPosition = NewPositionAfterMove(color, initialPosition, i);

            //If it is a legal move to move the specified color of checker 
            // 'i' positions from its original position,
            //add that new position
            if(IsLegalMove(color, initialPosition, newPosition))
            {
                output.Add(NewPositionAfterMove(color, initialPosition, i));
            }
        }

        return output;

    }



    private bool IsLegalMove(CheckerColor color, int initialPosition, int targetPosition)
    {
        bool isLegal = (targetPosition >= 1 && targetPosition <= 24); 
        isLegal = isLegal && PositionIsOpen(color, targetPosition);
        isLegal = isLegal && numberOfCheckersOnPosition(color, initialPosition) > 0;
        isLegal = isLegal && !hasCheckersOnBar(color);
        return isLegal;
    }


    //A position is closed if there are two or more enemy checkers on the position
    private bool PositionIsOpen(CheckerColor color, int targetPosition)
    {
        CheckerColor otherColor = (color == CheckerColor.White) ? CheckerColor.Black : CheckerColor.White;
        return numberOfCheckersOnPosition(otherColor, targetPosition) < 2;
    }

    private int numberOfCheckersOnPosition(CheckerColor color, int position)
    {
        int c = GetValueAtPosition(position);
        //White checkers are represented by a positive number
        if (color == CheckerColor.White)
        {
            return c > 0 ? c : 0;
        }
        //Black checkers are represented by a negative number
        else 
        {
            return c < 0 ? -c : 0; 
        }
    }

    private bool hasCheckersOnBar(CheckerColor color)
    {
        return color == CheckerColor.White ? whiteCheckersOnBar > 0 : blackCheckersOnBar > 0;
    }

    //Positions are 1-indexed, while the array representing them
    //is zero indexed
    private int GetValueAtPosition(int i)
    {
        return gameBoard[i-1];
    }
}*/
