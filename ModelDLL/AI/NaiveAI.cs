using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{

    /**
     * This class is a naive AI made for testing purposes. It selects the first moveable checker that it finds,
     * and moves it to the first position that it can.
     */

    public class NaiveAI : Player
    {
        private BackgammonGame model;
        private CheckerColor color;

        public NaiveAI(BackgammonGame model, CheckerColor color)
        {
            this.model = model;
            this.color = color;
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
