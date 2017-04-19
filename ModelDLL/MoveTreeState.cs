using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    internal class MoveTreeState
    {

        //MAX_MOVE_DISTANCE_ACCPETED + 1 because move distances are ordinal numbers, while array indexes are cardinal.
        private MoveTreeState[] resultingStates = new MoveTreeState[BackgammonGame.MAX_MOVE_DISTANCE_ACCEPTED + 1];
        private CheckerColor color;
        private GameBoardState state;
        private int position;
        private List<int> moves;


        internal MoveTreeState(GameBoardState state, CheckerColor color, int fromPosition, List<int> moves)
        {
            this.color = color;
            this.state = state;
            this.position = fromPosition;
            this.moves = moves;

            foreach(int move in moves)
            {
                List<int> movesCopy = new List<int>(moves);
                movesCopy.Remove(move);

                int nextPosition = GameBoardMover.GetPositionAfterMove(color, position, move);

                GameBoardState newState = GameBoardMover.Move(state, color, position, move);
                if (newState == null)
                {
                    continue;
                }
                else
                {
                    resultingStates[move] = new MoveTreeState(newState, color, nextPosition, movesCopy);
                }
            }
        }

        internal HashSet<int> GetReachablePositions()
        {
            HashSet<int> output = new HashSet<int>();
            List<MoveTreeState> reachableStates = GetReachableMoveTreeStates();

            foreach(MoveTreeState state in reachableStates)
            {
                output.Add(state.position);
            }

            return output;
        }

        internal bool LegalToMoveToPosition(int position)
        {
            return GetReachablePositions().Contains(position);
        }

        internal MoveTreeState MoveToPosition(int position)
        {
            if (!LegalToMoveToPosition(position))
            {
                throw new InvalidOperationException("Illegal move to position: " + position);
            }

            return GetMoveTreeStateWithPosition(position);
        }

        //Gets a move tree state where the original checker has been moved the specified position
        public MoveTreeState GetMoveTreeStateWithPosition(int position)
        {
            //Get all the reachable move tree states from this state
            List<MoveTreeState> states = GetReachableMoveTreeStates();

            //Filter out the states that have the same position as the method argument
            List<MoveTreeState> usableStates = new List<MoveTreeState>();
            foreach(MoveTreeState state in states)
            {
                if(state.GetPosition() == position)
                {
                    usableStates.Add(state);
                }
            }

            //Return null if there are no suitable states
            if(usableStates.Count() == 0)
            {
                return null;
            }

            //Else return the best suited state
            return SelectBestMoveTreeState(usableStates);
        }


        //Given a list of MoveTreeStates, select the most beneficial to the current player.
        //The most beneficial state is the one that has the most enemy checkers on the bar
        private MoveTreeState SelectBestMoveTreeState(List<MoveTreeState> states)
        {
            CheckerColor opponent = color.OppositeColor();
            MoveTreeState bestOption = states[0];

            foreach(MoveTreeState state in states)
            {
                if(state.GetState().getCheckersOnBar(opponent) > bestOption.GetState().getCheckersOnBar(opponent))
                {
                    bestOption = state;
                }
            }
            return bestOption;
        }

        private List<MoveTreeState> GetReachableMoveTreeStates()
        {
            List<MoveTreeState> output = new List<MoveTreeState>();
            GetReachableMoveTreeStatesRecursion(output);
            output.Remove(this);
            return output;
        }

        private void GetReachableMoveTreeStatesRecursion(List<MoveTreeState> states)
        {
            states.Add(this);
            foreach(MoveTreeState state in resultingStates)
            {
                if (state != null)
                {
                    state.GetReachableMoveTreeStatesRecursion(states);
                }
            }
        }

        internal GameBoardState GetState()
        {
            return this.state;
        }

        internal int GetPosition()
        {
            return this.position;
        }

        internal List<int> GetMoves()
        {
            return this.moves;
        }
    }
}
