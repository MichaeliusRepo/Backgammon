using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    public class ViewInterface
    {
        private readonly BackgammonGame bg;

        public ViewInterface(BackgammonGame bg)
        {
            this.bg = bg;
        }

        public List<int> GetLegalMovesForCheckerAtPosition(int position)
        {

            return new List<int>(bg.GetLegalMovesFor(bg.playerToMove(), position));
        }

        public GameBoardState GetGameBoardState()
        {
            return bg.GetGameBoardState();
        }

        public CheckerColor GetNextPlayerToMove()
        {
            return bg.playerToMove();
        }

        public List<int> GetMoveableCheckers()
        {
            return bg.GetMoveableCheckers();
        }

        public List<int> GetMoves()
        {
            return bg.GetMovesLeft();
        }

        public List<Turn> GetAndFlushTurnHistory()
        {
            return bg.GetTurnHistory();
        }


    }
}
