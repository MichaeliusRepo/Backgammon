using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    class BearOffPosition : Position
    {
        public const int WHITE_BEAR_OFF_ID = 71524733;
        public const int BLACK_BEAR_OFF_ID = 91241467;

        private Position[] homeBoard;


        public BearOffPosition(int id, int checkers) : base(id, checkers) { }

        public void SetHomeBoard(Position[] homeBoard)
        {
            this.homeBoard = homeBoard;
        }

        private CheckerColor GetColorOfBearOffPosition()
        {
            return this.id == WHITE_BEAR_OFF_ID ? CheckerColor.White : CheckerColor.Black;
        }

        protected override void CalculateLegalMoves(CheckerColor color, HashSet<int> legalPositions, int[] movesLeft, int initialPosition)
        {
            legalPositions.Add(this.id);
            return;
        }

        public override HashSet<int> GetLegalMoves(CheckerColor color, int[] moves)
        {
            return new HashSet<int>();
        }

        protected override bool LegalToMoveFromHere(CheckerColor color)
        {
            return false;
        }

        protected override bool LegalToMoveHere(CheckerColor color)
        {

            return true;
            //bool isLegal = color == GetColorOfBearOffPosition();
            //return isLegal;
        }

        private int NumberOfCheckersInHomeBoard()
        {
            CheckerColor color = GetColorOfBearOffPosition();
            int sum = 0;
            foreach (Position pos in homeBoard)
            {
                sum += pos.NumberOfCheckersOnPosition(color);
            }

            return sum;
        }
    }
}
