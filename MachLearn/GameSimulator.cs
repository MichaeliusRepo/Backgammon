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
        private CheckerColor CurrentPlayer => game.playerToMove();

        internal CheckerColor Run()
        {
            game = new BackgammonGame(BackgammonGame.DefaultGameBoard, new RealDice());
            while (!TemporalDifference.GameOver(st))
            {
                var outcome = PickBestOutcome();
                for (int i = 0; i < outcome.movesMade.Count; i++)
                    game.Move(CurrentPlayer, outcome.movesMade[i].from, outcome.movesMade[i].to);
                TemporalDifference.UpdateWeights(st, outcome.state);
                st = outcome.state;
            }
            return (game.GetGameBoardState().getCheckersOnTarget(White) == 15) ? White : Black;
        }

        public MachPlayer(BackgammonGame g)
        {
            game = g;
            st = g.GetGameBoardState();
        }

        public void MakeMove()
        { // Use this for AI in View.
            var outcome = PickBestOutcome();
            for (int i = 0; i < outcome.movesMade.Count; i++)
                game.Move(CurrentPlayer, outcome.movesMade[i].from, outcome.movesMade[i].to);
        }

        private MovesCalculator.ReachableState PickBestOutcome()
        {
            var collection = MovesCalculator.GetReachableStatesThisTurn(st, CurrentPlayer, game.GetMovesLeft()).ToArray();
            return (CurrentPlayer == CheckerColor.White) ? PickHighest(collection) : PickLowest(collection);
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
