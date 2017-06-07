using System;
using System.Collections.Generic;

namespace ModelDLL
{
    public class PlayerInterface
    {
        private readonly CheckerColor color;
        private readonly BackgammonGame bg;
        private Player player;

        public PlayerInterface(BackgammonGame bg, CheckerColor color, Player player)
        {
            this.color = color;
            this.bg = bg;
            this.player = player;
        }

        internal bool HasPlayer()
        {
            return player != null;
        }

        public void SetPlayerIfNull(Player player)
        {
            if(this.player == null)
            {
                this.player = player;
            }
        }

        public CheckerColor MyColor()
        {
            return color;
        }

        public bool IsMyTurn()
        {
            return bg.playerToMove() == color;
        }

        public HashSet<int> GetMoveableCheckers()
        {
            if (!IsMyTurn())
            {
                throw new InvalidOperationException("Player interface for player " + color + " tried to get moveable checkers when not his turn");
            }
            return new HashSet<int>(bg.GetMoveableCheckers());
        }

        internal void EndTurn()
        {
            bg.EndTurn(this.color);
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

        public HashSet<int> GetLegalMovesForChecker(int position)
        {
            return bg.GetLegalMovesFor(this.color, position);
        }

        public List<int> move(int intialPosition, int targetPosition)
        {
            return bg.Move(this.color, intialPosition, targetPosition);
            return new List<int>();
        }

        public void move(int from, List<int> moves)
        {
            foreach(int i in moves)
            {
                bg.Move(color, from, i);
                from += (color == CheckerColor.White ? -i : i);
            }
        }

        internal IEnumerable<Node> GetFinalStates()
        {
            if (!IsMyTurn())
            {
                throw new InvalidOperationException("Player " + color + " tried to get all final states when not his turn");
            }
            else return bg.GetFinalStates();
        }

        internal void MoveTo(Node finalState)
        {
            if (!IsMyTurn())
            {
                throw new InvalidOperationException("Player " + color + " tried to move to  final state when not his turn");
            }
            else bg.MoveToFinalState(color, finalState);
        }


        internal void MakeMove()

        {
            if (player != null) player.MakeMove();
        }

        /*public List<Turn> GetAndFlushTurnHistory()
        {
            return bg.GetTurnHistory();
        }*/
    }
}