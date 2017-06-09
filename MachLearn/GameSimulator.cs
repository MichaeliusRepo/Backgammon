using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelDLL;

namespace MachLearn
{
    class GameSimulator
    {
        static int gameCount = 0;
        private BackgammonGame game = new BackgammonGame(BackgammonGame.DefaultGameBoard, new RealDice());
        private GameBoardState st;
        private CheckerColor c => game.playerToMove();

        public void Run()
        {
            while (!TemporalDifference.GameOver(st))
            {
                var outcome = PickBestOutcome();
                for (int i = 0; i < outcome.from.Count; i++)
                    game.Move(c, outcome.from[i], outcome.to[i]);
                TemporalDifference.UpdateWeights(st, outcome.state);
                st = outcome.state;
            }
        }

        private Outcome PickBestOutcome()
        {
            var outcomes = new List<Outcome>();

            var MovableCheckers = game.GetMoveableCheckers();

            foreach (int from in MovableCheckers)
                foreach (int to in game.GetLegalMovesFor(c, from))
                {

                }




            return null;
        }

        private class Outcome
        {
            internal GameBoardState state;
            internal List<int> from = new List<int>();
            internal List<int> to = new List<int>();
            double value;
        }


    }
}
