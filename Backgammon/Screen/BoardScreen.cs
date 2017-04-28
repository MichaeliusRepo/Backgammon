using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Backgammon.Input;
using Backgammon.Object;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ModelDLL;

namespace Backgammon.Screen
{
    /* 0) Re-sync with game logic
    *  Checkers on board: Create a point representing the stack.
    *  Create animation to put stuff on board.
    *  Create animation for capturing checkers.
    * 
    *  1) Add Dice Roll Graphics
    *  2) Display Pips
    *  3) Implement Splash Screen
    *  4) Implement Option Screen
    *  Options screen:
* - Audio
* - Pips
* - Type of game (PvP, PvE, AIvAI, online)
* - Player color
    * */

    /// <summary>
    /// Get Game Board()
    /// PlayerToMove() returns CheckerColor
    /// Get Movable Checkers()
    /// Get Legal Moves For Checker (int position)
    ///  Move (int from, int to)
    ///  Get#Checkers on Bar(CheckerColor color) returns int 
    /// </summary>

    public class BoardScreen : GameScreen
    {
        private Image Image = new Image { Path = "Images/Board", Position = new Vector2(540, 360) };
        private Board board;
        private BackgammonGame Model;
        private ViewInterface ViewInterface;
        private PlayerInterface WhitePlayer, BlackPlayer;
        private CheckerColor CurrentPlayer;
        private GameState State = GameState.PickChecker;
        private List<int> MovableCheckers, PossibleDestinations;
        private int SelectedPoint;
        private List<int> SetOfMoves;


        public override void LoadContent()
        {
            base.LoadContent();
            Image.LoadContent();

            //int[] gameBoard = BackgammonGame.DefaultGameBoard;
            int[] gameBoard = Board.TestBoard2;

            Model = new BackgammonGame(gameBoard, new RealDice());
            board = new Board(gameBoard);
            ViewInterface = new ViewInterface(Model);
            WhitePlayer = new PlayerInterface(Model, CheckerColor.White, null);
            BlackPlayer = new PlayerInterface(Model, CheckerColor.Black, null);

            SetState(GameState.PickChecker);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            Image.UnloadContent();
        }

        private void PlayerTurn()
        {
            CurrentPlayer = ViewInterface.GetNextPlayerToMove();

            List<int> getDice = ViewInterface.GetMoves();
            board.RemoveCheckerHighlight();
            Console.WriteLine(CurrentPlayer);
            Console.WriteLine("Dice Rolls: " + string.Join(", ", getDice));
            MovableCheckers = ViewInterface.GetMoveableCheckers();
            Console.WriteLine("Movable Checkers: " + string.Join(", ", MovableCheckers));
            board.GlowPoints(MovableCheckers);
            Console.WriteLine(string.Join(", ", ViewInterface.GetGameBoardState().getMainBoard()));

            if (MovableCheckers.Contains(CurrentPlayer.GetBar()))
                PickChecker(CurrentPlayer.GetBar());
            if (MovableCheckers.Count == 0)
                Console.WriteLine("No possible moves for " + CurrentPlayer);
        }


        private void PickChecker(int pointIndex)
        {
            SelectedPoint = pointIndex;
            if (NotOnBar())
                board.HighlightChecker(pointIndex);
            else
                board.HighlightChecker(CurrentPlayer);
            PossibleDestinations = ViewInterface.GetLegalMovesForCheckerAtPosition(pointIndex);
            SetState(GameState.PickDestination);
        }

        private void MoveChecker(int pointIndex)
        {
            if (CurrentPlayer == CheckerColor.White)
                SetOfMoves = WhitePlayer.move(SelectedPoint, pointIndex);
            else
                SetOfMoves = BlackPlayer.move(SelectedPoint, pointIndex);

            board.StopGlowPoints();
            board.RemoveCheckerHighlight();
            SetState(GameState.Animating);
        }

        private void MoveCheckerAnimation()
        {
            if (CurrentPlayer == CheckerColor.White)
                SetOfMoves[0] = -SetOfMoves[0];
            int to = SetOfMoves[0];
            if (NotOnBar())
                to += SelectedPoint;
            else if (to < 0)
                to += 25;
            if (board.CheckerWasCaptured(CurrentPlayer, to))
                board.Capture(to);
            if (NotOnBar())
                board.MoveChecker(SelectedPoint, to);
            else
                board.MoveChecker(CurrentPlayer, to);
            SelectedPoint = to;
            SetOfMoves.RemoveAt(0);
        }

        private void SetState(GameState setTo)
        {
            State = setTo;
            if (State == GameState.PickChecker)
                PlayerTurn();
            if (State == GameState.PickDestination)
                board.GlowPoints(PossibleDestinations);
        }

        private bool NotOnBar()
        {
            return SelectedPoint <= 24;
        }

        public override void Update(GameTime gameTime)
        {
            int pointIndex = board.GetClickedPoint();
            switch (State)
            {
                case GameState.Animating:
                    if (!board.InAnimation)
                        if (SetOfMoves.Count != 0)
                            MoveCheckerAnimation();
                        else
                            SetState(GameState.PickChecker);
                    break;

                case GameState.PickChecker:
                    if (MovableCheckers.Contains(pointIndex))
                        PickChecker(pointIndex);
                    break;

                case GameState.PickDestination:
                    if (pointIndex == SelectedPoint && NotOnBar()) // Cancel selected checker
                        SetState(GameState.PickChecker);
                    else if (PossibleDestinations.Contains(pointIndex))
                        MoveChecker(pointIndex);
                    break;
            }
            base.Update(gameTime);
            Image.Update(gameTime);
            board.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Image.Draw(spriteBatch);
            board.Draw(spriteBatch);
        }

        private enum GameState
        {
            PickChecker,
            PickDestination,
            Animating
        }
    }
}
