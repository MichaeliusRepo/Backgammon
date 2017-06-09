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
        //private PlayerInterface pi;
        private BackgammonGame model;
        private CheckerColor color;

        //public NaiveAI(PlayerInterface playerInterface)
        //{
        //    this.pi = playerInterface;
        //    this.model = pi.GetModel();
        //}

        public NaiveAI(BackgammonGame model, CheckerColor color)
        {
            this.model = model;
            this.color = color;
        }

        public void TurnEnded()
        {
            Debug.WriteLine("Turn ended for naive ai");
        }

        public void TurnStarted()
        {
            //int checkerToMove;
            //int positionToMoveTo;
            //while (pi.IsMyTurn())
            //{
            //    checkerToMove = pi.GetMoveableCheckers().ElementAt(0);
            //    positionToMoveTo = pi.GetLegalMovesForChecker(checkerToMove).ElementAt(0);
            //    pi.move(checkerToMove, positionToMoveTo);
            //}
        }

        public void MakeMove()
        {

            var moveableCheckers = model.GetMoveableCheckers(color);
            if(moveableCheckers.Count() == 0)
            {
                model.EndTurn(color);
                return;
            }

            int checkerToMove = moveableCheckers.First();
            var reachablePositions = model.GetLegalMovesFor(color, checkerToMove);
            int positionToMoveTo = reachablePositions.First();
            model.Move(color, checkerToMove, positionToMoveTo);
        }
    }
}
