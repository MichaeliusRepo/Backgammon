using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    class BarPosition : Position
    {
        public const int WHITE_BAR_ID = 151357818;
        public const int BLACK_BAR_ID = 612345638;

        private CheckerColor color;

        public BarPosition(int id, int checkers, CheckerColor color) : base(id, checkers)
        {
            this.color = color;

        }

        public bool AreCheckersOnBar()
        {
            return checkers != 0;
        }

        protected override void CalculateLegalMoves(CheckerColor color, HashSet<int> legalPositions, int[] movesLeft, int initialPosition)
        {



            base.CalculateLegalMoves(color, legalPositions, movesLeft, initialPosition);
        }

        public override HashSet<int> GetLegalMoves(CheckerColor color, int[] moves)
        {
            if (color != this.color)
            {
                return new HashSet<int>();
            }
            return base.GetLegalMoves(color, moves);
        }

        protected override bool LegalToMoveFromHere(CheckerColor color)
        {
            return AreCheckersOnBar();
        }

        protected override bool LegalToMoveHere(CheckerColor color)
        {
            return false;
        }
    }
}
