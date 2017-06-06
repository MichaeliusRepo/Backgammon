using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ModelDLL.Node;

namespace ModelDLL
{
    public class GreedyAI : Player
    {
        private PlayerInterface pi;

        public GreedyAI(PlayerInterface playerInterface)
        {
            this.pi = playerInterface;
        }

        public void ConnectPlayerInterface(PlayerInterface playerInterface)
        {
            pi = playerInterface;
        }

        public void TurnEnded()
        {
            throw new NotImplementedException();
        }

        public void TurnStarted()
        {
            throw new NotImplementedException();
        }

        public void MakeMove()
        {
            IEnumerable<Node> finalStates = pi.GetFinalStates();
            var chosenState = finalStates.Aggregate((a, b) => EvaluationFunction(a.state) > EvaluationFunction(b.state) ? a : b);
            pi.MoveTo(chosenState);
            
        }

        private double EvaluationFunction(GameBoardState state)
        {
            CheckerColor myColor = pi.MyColor();

            return 5 * state.pip(myColor) - 3 * state.capturableCheckers(myColor) + 0.9 * state.capturableCheckers(myColor.OppositeColor());

            //return 500;
            //return 500;
            
            if(myColor == CheckerColor.Black)
            {
                state = state.InvertColor();
            }

            return -1000 * state.InvertColor().ProbabilityOfWhiteGettingCaptured() + 500 * state.ProbabilityOfWhiteGettingCaptured();
        }

    }
}
