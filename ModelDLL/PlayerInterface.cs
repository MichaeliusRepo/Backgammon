using System;
using System.Collections.Generic;

namespace ModelDLL
{
    public class PlayerInterface
    {
        private readonly CheckerColor color;
        private readonly BackgammonGame bg;
        private readonly Player player;

        public PlayerInterface(BackgammonGame bg, CheckerColor color, Player player)
        {
            this.color = color;
            this.bg = bg;
            this.player = player;
        }

        public bool IsMyTurn()
        {
            return bg.playerToMove() == color;
        }

        public List<int> GetMovesLeft()
        {
            if (!IsMyTurn())
            {
                throw new InvalidOperationException("Player" + color + " tried to get moves left when not his turn");
            }
            return bg.GetMovesLeft();
        }

        public GameBoardState GetGameBoardState()
        {
            return bg.GetGameBoardState();
        }

        //public List<int> move(int from, int to)

        public void move(int from, List<int> moves)
        {
            foreach(int i in moves)
            {
                bg.Move(color, from, i);
                from += (color == CheckerColor.White ? -i : i);
            }
        }

        void TurnStarted()
        {
            if (player != null)
                player.TurnStarted();
        }

        void TurnEnded()
        {
            if (player != null)
                player.TurnEnded();
        }

    }
}