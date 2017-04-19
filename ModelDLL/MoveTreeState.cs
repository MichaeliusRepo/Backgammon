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


        internal MoveTreeState(GameBoardState state, CheckerColor color, int fromPosition, List<int> moves)
        {
            this.color = color;
            this.state = state;
            this.position = fromPosition;

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
            GetReachablePositionsRecursion(output);
            output.Remove(position);
            return output;
        }

        private void GetReachablePositionsRecursion(HashSet<int> set)
        {
            set.Add(position);
            foreach(MoveTreeState mts in resultingStates)
            {
                if(mts != null)
                {
                    mts.GetReachablePositionsRecursion(set);
                }
            }
        }

        internal bool LegalToMoveToPosition(int position)
        {
            return GetReachablePositions().Contains(position);
        }

        internal GameBoardState MoveToPosition(int position)
        {
            if (!LegalToMoveToPosition(position))
            {
                throw new InvalidOperationException("Illegal move to position: " + position);
            }

            return GetStatesBelongingToPosition(position);
        }

        private GameBoardState GetStatesBelongingToPosition(int position)
        {
            List<GameBoardState> states = new List<GameBoardState>();

            GetStatesBelongingToPositionRecursive(position, states);

            return SelectBestAlternative(states);
        }

        private GameBoardState SelectBestAlternative(List<GameBoardState> states)
        {
            if(states.Count() == 1)
            {
                return states[0];
            }
            CheckerColor opponent = color.OppositeColor();
            GameBoardState best = states[0];
            foreach(GameBoardState state in states)
            {
                if(state.getCheckersOnBar(opponent) > best.getCheckersOnBar(opponent))
                {
                    best = state;
                }
            }

            return best;
        }

        private void GetStatesBelongingToPositionRecursive(int position, List<GameBoardState> states)
        {
            if(this.position == position)
            {
                states.Add(this.state);
                return;
            }
            else
            {
                foreach(MoveTreeState mts in resultingStates)
                {
                    if(mts != null)
                    {
                        mts.GetStatesBelongingToPositionRecursive(position, states);
                    }
                }
            }
        }
    }
}
