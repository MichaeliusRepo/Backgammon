using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    class FinalStatesCalculator
    {

        private IEnumerable<Pair> pairs;
        private CheckerColor color;
        private List<int> movesLeft;
        private List<FinalStatesCalculator> children = new List<FinalStatesCalculator>();

        internal FinalStatesCalculator(IEnumerable<GameBoardState> initialStates, CheckerColor color, List<int> moves)
        {
            initialize(initialStates, color, moves);
        }

        private void initialize(IEnumerable<GameBoardState> initialStates, CheckerColor color, List<int> moves)
        {
            this.pairs = initialStates.Select(s => new Pair(s, null));
            this.color = color;
            movesLeft = moves;

            foreach (int move in moves)
            {
                
                var tmp = pairs.Select(pair => pair.GetReachableStates(color, move)).Aggregate((IEnumerable<GameBoardState>)new List<GameBoardState>(), (a, b) => a.Concat(b));
                children.Add(new FinalStatesCalculator(tmp, color, moves.Without(move)));
            }
        }




        internal IEnumerable<GameBoardState> Calculate()
        {
            IEnumerable<GameBoardState> output = new List<GameBoardState>();
            foreach (var child in children)
            {
                output = output.Concat(child.Calculate());
            }
            output = pairs.Where(pair => pair.IsFinal == true).Select(pair => pair.state).Concat(output);


            return output;
        }

        internal class Pair
        {
            internal GameBoardState state = null;
            internal IEnumerable<GameBoardState> reachableStates = null;
            internal bool IsFinal = true;

            internal Pair(GameBoardState state, IEnumerable<GameBoardState> reachableStates)
            {
                this.state = state;
                this.reachableStates = reachableStates;
            }

            internal void addReachableStates(IEnumerable<GameBoardState> states)
            {
                if (reachableStates == null) reachableStates = states;
                else reachableStates = reachableStates.Concat(states);
            }

            internal IEnumerable<GameBoardState> GetReachableStates(CheckerColor color, int move)
            {
                var tmp = MovesCalculator.GetMoveableCheckers(state, color, new List<int>() { move }).Select(pos => GameBoardMover.Move(state, color, pos, move));
                if (tmp.Count() > 0) IsFinal = false;
                return tmp;
            }
        }
    }

    

}
