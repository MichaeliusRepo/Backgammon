using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ModelDLL
{
    class Position
    {

        private Edge[] whiteMovesEdges = new Edge[] { Edge.DeadEdge, Edge.DeadEdge, Edge.DeadEdge, Edge.DeadEdge, Edge.DeadEdge, Edge.DeadEdge };
        private Edge[] blackMovesEdges = new Edge[] { Edge.DeadEdge, Edge.DeadEdge, Edge.DeadEdge, Edge.DeadEdge, Edge.DeadEdge, Edge.DeadEdge };

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

        //Returns edges based on checker color type
        public Edge[] GetEdges(CheckerColor color)
        {
            if (color == CheckerColor.White)
            {
                return whiteMovesEdges;
            }
            else return blackMovesEdges;
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
            //If it is illegal to move the specified color from this position, there is nowhere to go.
            if (!LegalToMoveFromHere(color))
            {
                return new HashSet<int>();
            }

            //The set to which all the reachable positions will be added to 
            HashSet<int> output = new HashSet<int>();

            //Recursive helper function
            CalculateLegalMoves(color, output, moves, this.id);

            //Returns the output
            return output;
        }



        //The recursive helper function that is used for calculating all reachable positions
        protected virtual void CalculateLegalMoves(CheckerColor color, HashSet<int> output, int[] moves, int initialPosition)
        {
            if (LegalToMoveHere(color) || GetId() == initialPosition)
            {

                if (GetId() != initialPosition)
                {
                    //If it is in fact legal to move here, add this position to the output list, as long as it is not the 
                    //initial position we started moving from
                    output.Add(GetId());
                }

                //Get the edges to the next 6 positions, based on checker color
                Edge[] edges = GetEdges(color);
                for (int i = 0; i < moves.Length; i++)
                {

                    int move = moves[i];
                    //Checks if the edge is legal to be followed
                    if (edges[move - 1].canBeFollowed())
                    {
                        //Remove the move that was used to travel that edge, and recurse on the next position
                        int[] movesLeft = CopyArrayWithoutIndex(moves, i);
                        Position positionAfterMove = edges[move - 1].getTarget();
                        positionAfterMove.CalculateLegalMoves(color, output, movesLeft, initialPosition);
                    }
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


       /** //Given a checker color and the distance of the move to be performed, gives the position the move ends at
        private Position GetPositionAfterMove(CheckerColor color, int moveDistance)
        {
            return GetNeighboursArray(color)[moveDistance - 1];
        }*/


        //Given the checker color, return the bar for that color of checkers
        private BarPosition GetBar(CheckerColor color)
        {
            return ( color == CheckerColor.White )? whiteBar : blackBar;
        }

        public int NumberOfCheckersOnPosition(CheckerColor color)
        {
            if( color == CheckerColor.White)
            {
                return checkers > 0 ? checkers : 0;
            }
            else
            {
                return checkers < 0 ? checkers * -1 : 0;
            }
        }

        public Boolean isLegalMove(CheckerColor color, int distance, int[] moves)
        {
            if (!LegalToMoveFromHere(color))
            {
                return false;
            }

            int target = GetId() + (color == CheckerColor.White ? -distance : distance);

            return GetLegalMoves(color, moves).Contains(target);
        } 

        public int GetCheckers()
        {
            return this.checkers;
        }

        public void removeChecker(CheckerColor color)
        {
            this.checkers += (color == CheckerColor.White ? -1 : 1);
        }

        public void addChecker(CheckerColor color)
        {
            if (color == CheckerColor.White)
            {
                if (NumberOfCheckersOnPosition(CheckerColor.Black) == 1)
                {
                    checkers = 0;
                    blackBar.addChecker(CheckerColor.Black);
                }
            }
            else
            {
                if (NumberOfCheckersOnPosition(CheckerColor.White) == 1)
                {
                    checkers = 0;
                    whiteBar.addChecker(CheckerColor.White);
                }
            }
            this.checkers += (color == CheckerColor.White ? 1 : -1);
        }
    }


    

   

    
}
