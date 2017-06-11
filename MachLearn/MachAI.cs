using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelDLL;
using static ModelDLL.CheckerColor;

namespace MachLearn
{
    public class MachAI : Player
    {
        private BackgammonGame game;
        private GameBoardState st;
        private MovesCalculator.ReachableState outcome;

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
                    TemporalDifference.UpdateWeights(st, outcome.state, game.playerToMove());
                    st = outcome.state;
                }
            }
            TemporalDifference.UpdateWeights(st, st, White);
            TemporalDifference.UpdateWeights(st, st, Black); // st1 doesn't matter when game is won.
            return (game.GetGameBoardState().getCheckersOnTarget(White) == 15) ? White : Black;
        }

        public MachAI(BackgammonGame g)
        {
            game = g;
            st = g.GetGameBoardState();
        }

        public void MakeMove()
        { // Use this for AI in View.
            st = game.GetGameBoardState();
            outcome = PickBestOutcome();
            if (outcome == null)
                game.EndTurn(game.playerToMove());
            else
            {
                for (int i = 0; i < outcome.movesMade.Count; i++)
                    game.Move(game.playerToMove(), outcome.movesMade[i].from, outcome.movesMade[i].to);
            }
        }

        internal void MakeMoveAndLearn()
        {
            MakeMove();
            TemporalDifference.UpdateWeights(st, outcome.state, game.playerToMove());
        }

        private MovesCalculator.ReachableState PickBestOutcome()
        {
            var collection = MovesCalculator.GetReachableStatesThisTurn(st, game.playerToMove(), game.GetMovesLeft()).ToArray();
            if (collection.Length == 0)
                return null;
            return (game.playerToMove() == White) ? PickHighest(collection) : PickLowest(collection);
            //return (game.playerToMove() == White) ? PickLowest(collection) : PickHighest(collection);
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
