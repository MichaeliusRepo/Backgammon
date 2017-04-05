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
    // Massive to-do list here! :-)))))))

    // TODO Glow Selected Checker
    // TODO Audio Manager
    // TODO Input Triggas

    /// <summary>
    /// Get Game Board()
    /// PlayerToMove() returns CheckerColor
    /// Get Movable Checkers()
    /// Get Legal Moves For Checker (int position)
    ///  Move (int from, int to)
    ///  Get#Checkers on Bar(CheckerColor color) returns int 
    /// </summary>
    /*a) Roll Dice
	    a.a) Automagically show checkers to move
            a.a.a) Show possible moves for selected checker
                a.a.a.a) Move Checker
				    if (Dices left != 0) GOTO a.a)
				    else END
                a.a.a.b) Cancel Selected Checker
                    GO TO a.a)
        */

    public class BoardScreen : GameScreen
    {
        #region You have a right to privacy, and your rights are important because you do not know when you will need them.
        private Image Image = new Image { Path = "Images/Board", Position = new Vector2(540, 360) };
        private Board board;
        private FakeDice fakeDice;
        private BackgammonGame Model;
        private ViewInterface ViewInterface;
        private PlayerInterface WhitePlayer;
        private PlayerInterface BlackPlayer;
        private CheckerColor CurrentPlayer;
        private GameState State = GameState.PickChecker;
        private List<int> MovableCheckers;
        private List<int> PossibleDestinations;
        private int SelectedPoint;
        #endregion

        private void PlayerTurn()
        {
            Console.WriteLine(CurrentPlayer);

            List<int> getDice = ViewInterface.GetMoves();
            Console.WriteLine("Dice Rolls: " + string.Join(", ", getDice));

            MovableCheckers = ViewInterface.GetMoveableCheckers();
            Console.WriteLine("Movable Checkers: " + string.Join(", ", MovableCheckers));

            board.GlowPoints(MovableCheckers);

            #region Who needs to read when one can simply watch? I farewell thy writing, lest you let me within thy world.
            //// TODO Click a checker here
            //if (CurrentPlayer == CheckerColor.White)
            //    PossibleDestinations = ViewInterface.GetLegalMovesForCheckerAtPosition(24);
            //if (CurrentPlayer == CheckerColor.Black)
            //    PossibleDestinations = ViewInterface.GetLegalMovesForCheckerAtPosition(17);

            //Console.WriteLine(string.Join(", ", PossibleDestinations));
            #endregion
        }

        public override void LoadContent()
        {
            base.LoadContent();
            Image.LoadContent();
            int[] defaultGameBoard = BackgammonGame.DefaultGameBoard;

            #region One day, I long to own a true dice in this fleeting, ephemeral manifistation of virtuality
            List<int[]> moves = new List<int[]>();
            moves.Add(new int[] { 5, 6 });
            moves.Add(new int[] { 2, 2 });
            fakeDice = new FakeDice(moves);
            #endregion

            Model = new BackgammonGame(defaultGameBoard, fakeDice);
            board = new Board(defaultGameBoard);
            ViewInterface = new ViewInterface(Model);
            CurrentPlayer = ViewInterface.GetNextPlayerToMove();
            WhitePlayer = new PlayerInterface(Model, CheckerColor.White, null);
            BlackPlayer = new PlayerInterface(Model, CheckerColor.Black, null);

            PlayerTurn();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            Image.UnloadContent();
        }

        private void PickChecker(int pointIndex)
        {
            board.HighlightChecker(pointIndex);
            PossibleDestinations = ViewInterface.GetLegalMovesForCheckerAtPosition(pointIndex);
            SetState(GameState.PickDestination);
            SelectedPoint = pointIndex;
        }

        private void MoveChecker(int pointIndex)
        {
            if (CurrentPlayer == CheckerColor.White)
                WhitePlayer.move(SelectedPoint, new List<int>() { 6, 5 });
            else
                BlackPlayer.move(SelectedPoint, new List<int>() { 6, 5 });

            SetState(GameState.Animating);
            board.StopGlowPoints();
            board.RemoveCheckerHighlight();
            board.MoveChecker(SelectedPoint, pointIndex);
            Console.WriteLine(string.Join(", ", ViewInterface.GetGameBoardState().getMainBoard()));
        }

        private void SetState(GameState setTo)
        {
            State = setTo;
            if (State == GameState.PickChecker)
            {
                board.GlowPoints(MovableCheckers);
                board.RemoveCheckerHighlight();
            }
            if (State == GameState.PickDestination)
                board.GlowPoints(PossibleDestinations);
        }

        public override void Update(GameTime gameTime)
        {
            int pointIndex = board.GetClickedPoint();
            switch (State)
            {
                case GameState.Animating:
                    if (!board.InAnimation)
                        SetState(GameState.PickChecker);
                    break;

                case GameState.PickChecker:
                    if (MovableCheckers.Contains(pointIndex))
                        PickChecker(pointIndex);
                    break;

                case GameState.PickDestination:
                    if (pointIndex == SelectedPoint) // Cancel selected checker
                        SetState(GameState.PickChecker);
                    else if (PossibleDestinations.Contains(pointIndex))
                        MoveChecker(pointIndex);
                    break;
            }

            #region From a day long past, I remind thee with beautiful memories from whence we explored the depths of our dreams
            //if (InputManager.Instance.KeyPressed(Keys.X))
            //{
            //    WhitePlayer.move(24, new List<int>() { 6, 5 });
            //    board.StopGlowPoints();
            //    board.RemoveGlow();
            //    board.MoveChecker(24, 13);
            //    Console.WriteLine(string.Join(", ", ViewInterface.GetGameBoardState().getMainBoard()));
            //}

            //if (InputManager.Instance.KeyPressed(Keys.Z))
            //{
            //    board.PlaceGlow(24);
            //    board.GlowPoints(PossibleDestinations);
            //}

            //if (InputManager.Instance.KeyPressed(Keys.C))
            //{
            //    //fakeDice.SetReturnValues(new int[] { 2,2});
            //    PlayerTurn();

            //}

            //if (InputManager.Instance.KeyPressed(Keys.V))
            //{
            //    board.PlaceGlow(17);
            //    board.GlowPoints(PossibleDestinations);
            //}

            //if (InputManager.Instance.KeyPressed(Keys.B))
            //{
            //    WhitePlayer.move(17, new List<int>() { 2,2,2,2 });
            //    board.StopGlowPoints();
            //    board.RemoveGlow();
            //    board.MoveChecker(23, 12);
            //    Console.WriteLine(string.Join(", ", ViewInterface.GetGameBoardState().getMainBoard()));
            //}

            //if (InputManager.Instance.KeyPressed(Keys.Enter))
            //    //Board.Points[0].ReturnTopChecker().MoveToPosition(new Vector2(50, 60));
            //    if (!board.Points[0].IsEmpty())
            //        board.MoveChecker(board.Points[0], board.Points[16]);
            //    else if (!board.Points[5].IsEmpty())
            //        board.MoveChecker(board.Points[5], board.Points[7]);

            //if (InputManager.Instance.KeyPressed(Keys.T))
            //    board.PlaceGlow(board.Points[7].GetTopChecker());
            #endregion

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
