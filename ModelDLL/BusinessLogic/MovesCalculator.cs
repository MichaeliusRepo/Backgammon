using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ModelDLL.CheckerColor;

namespace ModelDLL
{
    class MovesCalculator
    {

        /* Given an initial state, a checker color, an initial position and the moves left to take, 
         * returns an object that can calculate what positions are reachable from the checkers on
         * the initial position, whether it is legal to move a checker from the initial position to 
         * a given position, and can return the state the game will be in after such a move.
         */


        private List<MoveState> reachableStates = new List<MoveState>();
        private MoveState initialMoveState;

        internal MovesCalculator(GameBoardState state, CheckerColor color, int fromPosition, List<int> moves)
        {
            initialMoveState = new MoveState(state, color, fromPosition, moves, new List<Change>());


            //Initialises the set of reachable states given the starting GameBoardState, position and moves
            IEnumerable<MoveState> tmp1 = initialMoveState.GenerateMoveStatesForPosition();
            while(tmp1.Count() > 0)
            {
                reachableStates.AddRange(tmp1); //Adds all the states in tmp1 to the reachable states

                tmp1 = tmp1
                    //Maps every state 'a' in tmp1 to a list that contains the states that are reachable from 'a'
                    .Select(s => s.GenerateMoveStatesForPosition())
                    //Concatenates all the lists, yielding a new set of reachable states
                    .Aggregate((a, b) => a.Concat(b));   
                                                                
            }
        }

        internal IEnumerable<int> GetReachablePositions()
        {
            //Maps each reachable state 'a' to the position the checker on the initial position
            //is moved to in order for the resulting state to be 'a'
            return new HashSet<int>(reachableStates.Select(s => s.position));
        }

        internal bool LegalToMoveToPosition(int position)
        {
            return GetReachablePositions().Contains(position);
        }


        private Func<MoveState, int> NumberOfEnemyCheckersOnBar = s => s.state.getCheckersOnBar(s.color.OppositeColor());
        internal MoveState MoveToPosition(int position)
        {
            if (!LegalToMoveToPosition(position))
            {
                throw new InvalidOperationException("Illegal move to position: " + position);
            }

            
            return reachableStates
                //Select states where a checker has been moved to the desired position
                .Where(x => x.position == position)   

                //Sort them in descending order by number of enemy checkers on the bar
                .OrderByDescending(x => NumberOfEnemyCheckersOnBar(x))

                //Select the one with the most enemy checkers on the bar
                .ElementAt(0);

        }

        internal static IEnumerable<int> GetMoveableCheckers(GameBoardState state, CheckerColor color, List<int> moves)
        {
            HashSet<int> output = new HashSet<int>();
            if(new MovesCalculator(state, color, color.GetBar(), moves).GetReachablePositions().Count() > 0){
                output.Add(color.GetBar());
            }
            for(int i = 1; i <= 24; i++)
            {
                if(new MovesCalculator(state, color, i, moves).GetReachablePositions().Count() > 0)
                {
                    output.Add(i);
                }
            }
            return output;
        }


        internal class MoveState
        {
            internal GameBoardState state;
            internal CheckerColor color;
            internal int position;
            internal List<int> movesLeft;
            internal List<Change> changes;

            internal MoveState(GameBoardState state, CheckerColor color, int position, 
                              List<int> movesLeft, List<Change> changes)
            {
                this.state = state;
                this.color = color;
                this.position = position;
                this.movesLeft = movesLeft;
                this.changes = changes;
            }

            internal IEnumerable<MoveState> GenerateMoveStatesForPosition()
            {
                List<MoveState> output = new List<MoveState>();

                //For every move, if it is legal, creates a new MoveState that corresponds to taking the move, and adds it to output
                foreach (int move in movesLeft)
                {
                    
                    var newState = GameBoardMover.Move(state, color, position, move);
                    if(newState != null)
                    {
                        int positionAfterMove = GameBoardMover.GetPositionAfterMove(color, position, move);

                        var newChanges = ComputeChanges(color, position, positionAfterMove, movesLeft);

                        var newMoveState = new MoveState(newState, color, positionAfterMove,
                                                        movesLeft.Without(move), newChanges);
                        output.Add(newMoveState);
                    }
                }

                return output;
                
            }

            private List<Change> ComputeChanges(CheckerColor color, int from, int to, List<int> moves)
            {
                //Add the performed move to the changes
                List<Change> output = changes.With(new Move(color, from, to));

                //If the above move happened to capture a checker, add the capturing move to the changes
                if (CapturesChecker(color, to))
                {
                    var m2 = new Move(color.OppositeColor(), to, color.OppositeColor().GetBar());
                    output.Add(m2);
                }

                //If there is at least one move left, add it to the changes
                var movesLeft = moves.Without(  Math.Abs(to-from) );
                if (movesLeft.Count() > 0) output.Add(new DiceState(movesLeft));

                return output;
            }

            private bool CapturesChecker(CheckerColor color, int to)
            {
                if (color == White) return state.GetCheckersOnPosition(to) == -1;
                else return state.GetCheckersOnPosition(to) == 1;
            }
        }
    }
}

