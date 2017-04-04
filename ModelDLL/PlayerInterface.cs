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

        public void move(int from, int distance)
        {
            bg.move(color, from, distance);
        }

        void TurnStarted()
        {
            player.TurnStarted();
        }

        void TurnEnded()
        {
            player.TurnEnded();
        }
    }
}