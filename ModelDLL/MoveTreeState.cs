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
            output.Remove(this.position);
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




    }
}
