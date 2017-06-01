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

        public void ConnectPlayerInterface(PlayerInterface playerInterface)
        {
            pi = playerInterface;
        }

        public void TurnEnded()
        {
            Debug.WriteLine("Turn ended for naive ai");
        }

        public void TurnStarted()
        {
            int checkerToMove;
            int positionToMoveTo;
            while (pi.IsMyTurn())
            {
                checkerToMove = pi.GetMoveableCheckers().ElementAt(0);
                positionToMoveTo = pi.GetLegalMovesForChecker(checkerToMove).ElementAt(0);
                pi.move(checkerToMove, positionToMoveTo);
            }
        }

        public void MakeMove()
        {
            int checkerToMove = pi.GetMoveableCheckers().ElementAt(0);
            int positionToMoveTo = pi.GetLegalMovesForChecker(checkerToMove).ElementAt(0);
            pi.move(checkerToMove, positionToMoveTo);

        }
    }
}
