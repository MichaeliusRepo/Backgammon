using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelDLL;
using static ModelDLL.CheckerColor;

namespace MachLearn
{
    public class MachPlayer : Player
    {
        private BackgammonGame game;
        private GameBoardState st;

        internal CheckerColor Run()
        {
            while (!TemporalDifference.GameOver(st))
            {
                var outcome = PickBestOutcome();
                if (outcome == null)
                    game.EndTurn(game.playerToMove());
                else
                {
                    for (int i = 0; i < outcome.movesMade.Count; i++)
                        game.Move(game.playerToMove(), outcome.movesMade[i].from, outcome.movesMade[i].to);
                    TemporalDifference.UpdateWeights(st, outcome.state);
                    st = outcome.state;
                }
            }
            TemporalDifference.UpdateWeights(st, st); // st1 doesn't matter when game is won.
            return (game.GetGameBoardState().getCheckersOnTarget(White) == 15) ? White : Black;
        }

        public MachPlayer(BackgammonGame g)
        {
            game = g;
            st = g.GetGameBoardState();
        }

        public void MakeMove()
        { // Use this for AI in View.
            st = game.GetGameBoardState();
            var outcome = PickBestOutcome();
            if (outcome == null)
                game.EndTurn(game.playerToMove());
            else
                for (int i = 0; i < outcome.movesMade.Count; i++)
                    game.Move(game.playerToMove(), outcome.movesMade[i].from, outcome.movesMade[i].to);
        }

        private MovesCalculator.ReachableState PickBestOutcome()
        {
            var collection = MovesCalculator.GetReachableStatesThisTurn(st, game.playerToMove(), game.GetMovesLeft()).ToArray();
            if (collection.Length == 0)
                return null;
            return PickHighest(collection);
            //return (game.playerToMove() == White) ? PickHighest(collection) : PickLowest(collection);
        } // The color check should only be done once.

        private MovesCalculator.ReachableState PickHighest(MovesCalculator.ReachableState[] array)
        {
            var best = array[0];
            foreach (var rs in array)
                if (TemporalDifference.ValueFunction(rs.state) > TemporalDifference.ValueFunction(best.state))
                    best = rs;
            return best;
        }

        private MovesCalculator.ReachableState PickLowest(MovesCalculator.ReachableState[] array)
        {
            var best = array[0];
            foreach (var rs in array)
                if (TemporalDifference.ValueFunction(rs.state) < TemporalDifference.ValueFunction(best.state))
                    best = rs;
            return best;
        }

    }
}
