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
using static ModelDLL.CheckerColor;

namespace Backgammon.Screen
{
    /*  1) Add Dice Roll Graphics
    *  2) Display Pips
    *  3) Implement Splash Screen
    *  4) Implement Option Screen
    *  Options screen:
* - Audio
* - Pips
* - Type of game (PvP, PvE, AIvAI, online)
* - Player color
    * */

    public class BoardScreen : GameScreen
    {
        private Image Image = new Image { Path = "Images/Board", Position = new Vector2(540, 360) };
        private List<Image> DiceImages = new List<Image>();
        private Board board;
        private BackgammonGame Model;
        private ViewInterface ViewInterface;
        private PlayerInterface WhiteInterface, BlackInterface;
        private Player WhiteAI, BlackAI;
        internal static CheckerColor CurrentPlayer { get; private set; }
        private GameState State = GameState.PickChecker;
        private List<int> MovableCheckers, PossibleDestinations;
        private int SelectedPoint;
        private List<int> SetOfMoves;
        private List<int> DiceRollsLeft;
        private float[] DiceXPositions = { Board.midX - 4 * Board.leftX, Board.midX - 2 * Board.leftX,
            Board.midX + 2 * Board.leftX, Board.midX + 4 * Board.leftX };
        private float DiceScale = 0.50f;

        public override void LoadContent()
        {
            base.LoadContent();
            Image.LoadContent();
            foreach (Image DiceImage in DiceImages)
                DiceImage.LoadContent();

            int[] gameBoard = BackgammonGame.DefaultGameBoard;
            //int[] gameBoard = Board.TestBoard3;

            Model = new BackgammonGame(gameBoard, new RealDice());
            //Model = new BackgammonGame(gameBoard, new FakeDice(new int[] { 1, 6 }));
            board = new Board(gameBoard);
            ViewInterface = new ViewInterface(Model);

            WhiteInterface = new PlayerInterface(Model, White, null);
            BlackInterface = new PlayerInterface(Model, Black, new NaiveAI(BlackInterface));
            SetState(GameState.PickChecker);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            Image.UnloadContent();
            foreach (Image DiceImage in DiceImages)
                DiceImage.UnloadContent();
        }

        private void SetAI(CheckerColor color, bool enabled)
        {
            if (enabled)
                if (color == White)
                    WhiteAI = new NaiveAI(WhiteInterface);
                else
                    BlackAI = new NaiveAI(BlackInterface);
            else if (color == White)
                WhiteAI = null;
            else
                BlackAI = null;
        }

        private void CheckInconsistencies()
        {
            int[] motherboard = Model.GetGameBoardState().getMainBoard();
            for (int i = 0; i < motherboard.Length; i++)
                if (board.GetAmountOfCheckersAtPoint(i + 1) != motherboard[i])
                    throw new Exception("View was found to be inconsistent with model.");
        }

        private void PlayerTurn()
        {
            CheckInconsistencies();
            CurrentPlayer = ViewInterface.GetNextPlayerToMove();
            DiceRollsLeft = ViewInterface.GetMoves();
            GenerateDiceImages();

            board.RemoveCheckerHighlight();
            Console.WriteLine(CurrentPlayer);
            Console.WriteLine("Dice Rolls: " + string.Join(", ", DiceRollsLeft));
            MovableCheckers = ViewInterface.GetMoveableCheckers();
            board.GlowPoints(MovableCheckers);

            if (MovableCheckers.Contains(CurrentPlayer.GetBar()))
                PickChecker(CurrentPlayer.GetBar());
        }

        private void PickChecker(int clickedPoint)
        {
            SelectedPoint = clickedPoint;
            if (NotOnBar())
                board.HighlightChecker(clickedPoint);
            else
                board.HighlightChecker(CurrentPlayer);
            PossibleDestinations = ViewInterface.GetLegalMovesForCheckerAtPosition(clickedPoint);
            SetState(GameState.PickDestination);
        }

        private void MoveChecker(int clickedPoint)
        {
            if (CurrentPlayer == White)
                SetOfMoves = WhiteInterface.move(SelectedPoint, clickedPoint);
            else
                SetOfMoves = BlackInterface.move(SelectedPoint, clickedPoint);
            board.StopGlowPoints();
            board.RemoveCheckerHighlight();
            SetState(GameState.Animating);
        }

        private void MoveCheckerAnimation()
        {
            if (CurrentPlayer == White)
                SetOfMoves[0] = -SetOfMoves[0];
            int to = SetOfMoves[0];
            if (NotOnBar())
                to += SelectedPoint;
            else if (to < 0)
                to += 25;
            if (board.CheckerWasCaptured(CurrentPlayer, to))
                board.Capture(to);
            if (NotOnBar() && OnBoard(to))
                board.MoveChecker(SelectedPoint, to);
            else if (CanBearOff() && SetOfMoves.Count == 1)
                board.BearOff(CurrentPlayer, SelectedPoint);

            else
                board.MoveChecker(CurrentPlayer, to);
            SelectedPoint = to;
            SetOfMoves.RemoveAt(0);
        }

        private bool OnBoard(int i)
        {
            return i > 0 && i < 25;
        }

        private void SetState(GameState setTo)
        {
            State = setTo;
            if (State == GameState.PickChecker)
                PlayerTurn();
            if (State == GameState.PickDestination)
                board.GlowPoints(PossibleDestinations);
        }

        private bool CanBearOff()
        {
            return PossibleDestinations.Contains(BackgammonGame.BLACK_BEAR_OFF_ID) || PossibleDestinations.Contains(BackgammonGame.WHITE_BEAR_OFF_ID);
        }

        private int BearOffTo()
        {
            if (CurrentPlayer == White)
                return BackgammonGame.WHITE_BEAR_OFF_ID;
            return BackgammonGame.BLACK_BEAR_OFF_ID;
        }

        private bool NotOnBar()
        {
            return !(SelectedPoint.Equals(BackgammonGame.BLACK_BAR_ID) || SelectedPoint.Equals(BackgammonGame.WHITE_BAR_ID));
        }

        private void GenerateDiceImages()
        {
            foreach (Image DiceImage in DiceImages)
                DiceImage.UnloadContent();
            DiceImages.Clear();

            for (int i = 0; i < DiceRollsLeft.Count; i++)
                DiceImages.Add(new Image()
                {
                    Path = "Images/" + DiceRollsLeft[i],
                    Scale = new Vector2(DiceScale, DiceScale),
                    Position = new Vector2(DiceXPositions[i], Board.midY)
                });

            foreach (Image DiceImage in DiceImages)
                DiceImage.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            int clickedPoint = board.GetClickedPoint();
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
                    if (MovableCheckers.Contains(clickedPoint))
                        PickChecker(clickedPoint);
                    break;

                case GameState.PickDestination:
                    if (clickedPoint == SelectedPoint && NotOnBar()) // Cancel selected checker
                        SetState(GameState.PickChecker);
                    else if (PossibleDestinations.Contains(clickedPoint))
                        MoveChecker(clickedPoint);
                    else if (clickedPoint > 24 && PossibleDestinations.Contains(CurrentPlayer.BearOffPositionID()))
                        MoveChecker(BearOffTo());
                    break;
            }
            base.Update(gameTime);
            Image.Update(gameTime);
            board.Update(gameTime);
            foreach (Image DiceImage in DiceImages)
                DiceImage.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Image.Draw(spriteBatch);
            board.Draw(spriteBatch);
            foreach (Image DiceImage in DiceImages)
                DiceImage.Draw(spriteBatch);
        }

        private enum GameState
        {
            PickChecker,
            PickDestination,
            Animating
        }
    }
}
