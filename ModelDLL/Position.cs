using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ModelDLL
{
    class Position
    {

        
        public static Position OOBP = new OutOfBoundsPosition(0, 0);

        //Initialize all the neighbours for each position to be unreachable. The reachable neighbours are added
        //in GameBoard.cs
        
        //The positions that are reachable for white checkers
        private Position[] whiteMoves = new Position[] { OOBP, OOBP, OOBP, OOBP, OOBP, OOBP };

        //The positions that are reachable for black checkers
        private Position[] blackMoves = new Position[] { OOBP, OOBP, OOBP, OOBP, OOBP, OOBP };


        //The id for this position. For the "Points" on the Backgammon game board, this will range from 1 to 24. 
        public int id;

        //The number of checkers that are currently on this position. Positive numbers for white checkers
        //and negative numbers for black checkers
        public int checkers;
        private BarPosition blackBar;
        private BarPosition whiteBar;

        public Position(int id, int checkers)
        {
            this.id = id;
            this.checkers = checkers;
        }

        public Position(int id, int checkers, BarPosition whiteBar, BarPosition blackBar)
        {
            this.id = id;
            this.checkers = checkers;
            this.whiteBar = whiteBar;
            this.blackBar = blackBar;
        }

        //Returns the reachable positions based on the color of the checkers
        public Position[] GetNeighboursArray(CheckerColor color)
        {
            return (color == CheckerColor.White) ? whiteMoves : blackMoves;
        }


        //returns the id of this position
        public int GetId()
        {
            return id;
        }

        //Returns all the legal move from this position based on the color of the checkers 
        //and the moves (values on the dice) that are available
        public virtual HashSet<int> GetLegalMoves(CheckerColor color, int[] moves)
        { 

            //If it is not legal to move any checkers of the specified color from here
            //then the reachable positions are none
            if ( !LegalToMoveFromHere(color))
            {
                return new HashSet<int>();
            }

            //Gets the reachable positions
            Position[] nextPositions = GetNeighboursArray(color);

            //The HashSet<int> to store the output result into
            HashSet<int> output = new HashSet<int>();

            //For each of the moves...
            for(int i = 0; i < moves.Length; i++)
            {
                //Create a list over all the moves except this move
                int[] movesLeft = CopyArrayWithoutIndex(moves, i);

                //Move to the next position
                int move = moves[i];
                Position positionAfterMove = GetPositionAfterMove(color, move);

                //Check what other positions are reachable from this new position 
                //after we exclude the move we used to get here
                //Save these positions to the output set
                //See CalculateLegalMoves for more info
                positionAfterMove.CalculateLegalMoves(color, output, movesLeft);
            }

            return output;
        }


        //The recursive method that is used for calculating all reachable positions
        protected virtual void CalculateLegalMoves(CheckerColor color, HashSet<int> output, int[] moves)
        {
            if (LegalToMoveHere(color))
            {
                //If it is in fact legal to move here, add this position to the output list
                output.Add(GetId());

                //Repeat procedure from GetLegalMoves...
                Position[] nextPositions = GetNeighboursArray(color);
                for (int i = 0; i < moves.Length; i++)
                {
                    int[] movesLeft = CopyArrayWithoutIndex(moves, i);

                    int move = moves[i];
                    Position positionAfterMove = GetPositionAfterMove(color, move);
                    positionAfterMove.CalculateLegalMoves(color, output, movesLeft);
                }
            }

            //If it is not legal to move here, then it is not possible to move to any other positions via 
            //this position. End without adding anything
            else return;
        }


        //Checks whether it is legal to move a specified color of checker away from here
        protected virtual bool LegalToMoveFromHere(CheckerColor color)
        {
            bool legal = (color == CheckerColor.White) ? this.checkers > 0 : this.checkers < 0;
            legal = legal && !GetBar(color).AreCheckersOnBar();
            return legal;

        }

        //Checks whether it is legal to move a specified color of checker to this position
        protected virtual bool LegalToMoveHere(CheckerColor color)
        { 
            //Checks that the position is open: less than two enemy checkers
           return color == CheckerColor.White ? checkers > -2 : checkers < 2;
            
        }


        //Copy an entire array, excluding the one element that is located at a specified index
        private int[] CopyArrayWithoutIndex(int[] a, int i)
        {
            int[] output = new int[a.Length - 1];
            int j = 0;
            foreach(int k in a)
            {
                if (k != a[i])
                {
                    output[j] = k;
                    j++;
                }
            }
            return output;
        }


        //Given a checker color and the distance of the move to be performed, gives the position the move ends at
        private Position GetPositionAfterMove(CheckerColor color, int moveDistance)
        {
            return GetNeighboursArray(color)[moveDistance - 1];
        }

        //Given the checker color, return the bar for that color of checkers
        private BarPosition GetBar(CheckerColor color)
        {
            return ( color == CheckerColor.White )? whiteBar : blackBar;
        }
    }


    //Class representing the Bar
    class BarPosition : Position
    {
        public const int WHITE_BAR_ID = 151357818;
        public const int BLACK_BAR_ID = 612345638;

        private CheckerColor color;

        public BarPosition(int id, int checkers, CheckerColor color) : base(id, checkers)
        {
            this.color = color;

        }

        public bool LegalToMoveFromHere()
        {
            return AreCheckersOnBar();
        }

        public bool LegalToMoveHere()
        {
            return false;
        }

        public void CalculateLegalMoves()
        {
            return;
        }

        public bool AreCheckersOnBar()
        {
            return checkers != 0;
        }
    }

    //This position represents positions outside the board
    class OutOfBoundsPosition : Position
    {
        public OutOfBoundsPosition(int id, int checkers) : base(id, checkers)
        {
        }

        protected override void CalculateLegalMoves(CheckerColor color, HashSet<int> legalPositions, int[] movesLeft)
        {
            return;
        }

        public override HashSet<int> GetLegalMoves(CheckerColor color, int[] moves)
        {
            return new HashSet<int>();
        }

        protected override bool LegalToMoveFromHere(CheckerColor color)
        {
            return false;
        }

        protected override bool LegalToMoveHere(CheckerColor color)
        {
            return false;
        }
    }
}
