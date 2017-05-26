using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    public class NaiveAI : Player
    {
        private PlayerInterface pi;

        public NaiveAI(PlayerInterface playerInterface)
        {
            this.pi = playerInterface;
        }

        public void TurnEnded()
        {
            Debug.WriteLine("Turn ended for naive ai");
        }

        public void TurnStarted()
        {
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("Ai turn started");
            int checkerToMove;
            int positionToMoveTo;
            while (pi.IsMyTurn())
            {
                checkerToMove = pi.GetMoveableCheckers().ElementAt(0);
                positionToMoveTo = pi.GetLegalMovesForChecker(checkerToMove).ElementAt(0);

                Console.WriteLine("Moving from " + checkerToMove + " to " + positionToMoveTo);
                pi.move(checkerToMove, positionToMoveTo);
                Console.WriteLine(pi.GetGameBoardState().Stringify() + "\n\n");
            }
            Console.WriteLine("AI turn ended");
            Console.WriteLine("--------------------------------------");
        }
    }
}
