using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backgammon.Audio;
using Backgammon.Input;
using Backgammon.Object;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ModelDLL;
using static ModelDLL.CheckerColor;

namespace Backgammon.Screen
{
    /*
    *  2) Display Pips
    * */

    internal class BoardScreen : GameScreen
    {
        #region Fields

        private Image Image = new Image { Path = "Images/Board", Position = new Vector2(540, 360) };
        private List<Image> DiceImages = new List<Image>();

        private Board Board;
        private BackgammonGame Model;
        private ViewInterface ViewInterface;
        private PlayerInterface WhiteInterface, BlackInterface;
        private bool WhiteAI => OptionScreen.WhiteAI.SwitchedOn;
        private bool BlackAI => OptionScreen.BlackAI.SwitchedOn;
        internal static CheckerColor CurrentPlayer { get; private set; }
        private GameState State = GameState.PickChecker;

        private List<int> MovableCheckers, PossibleDestinations, SetOfMoves, DiceRollsLeft;
        private int SelectedPoint;

        private float[] DiceXPositions = { Board.midX - 4 * Board.leftX, Board.midX - 2 * Board.leftX,
            Board.midX + 2 * Board.leftX, Board.midX + 4 * Board.leftX };
        private float DiceScale = 0.8f;

        #endregion

        #region Auxilary Methods

        private PlayerInterface SetAI(CheckerColor color, bool enabled)
        {
            if (enabled) return Model.ConnectPlayer(color, new NaiveAI(null));
            else return new PlayerInterface(Model, color, null);
        }

        private void BeginTurn()
        {
            CurrentPlayer = ViewInterface.GetNextPlayerToMove();
            DiceRollsLeft = ViewInterface.GetMoves();
            GenerateDiceImages();
            Board.RemoveCheckerHighlight();

            var turns = Model.GetTurnHistory();
            if (turns.Count != 0)
                throw new Exception();
            //AIMove(turns);
            PlayerTurn();
        }

        private void AIMove(List<Turn> turns)
        {
            throw new NotImplementedException("dam son you did it");
        }

        private void CheckInconsistencies()
        {
            int[] motherboard = Model.GetGameBoardState().getMainBoard();
            for (int i = 0; i < motherboard.Length; i++)
                if (Board.GetAmountOfCheckersAtPoint(i + 1) != motherboard[i])
                    throw new Exception("View was found to be inconsistent with model. \n" +
                                            Board.GetAmountOfCheckersAtPoint(i + 1) + " != " + motherboard[i]);
        }

        private void PlayerTurn()
        {
            //CheckInconsistencies();
            MovableCheckers = ViewInterface.GetMoveableCheckers();
            Board.GlowPoints(MovableCheckers);

            if (MovableCheckers.Contains(CurrentPlayer.GetBar()))
                PickChecker(CurrentPlayer.GetBar());
        }

        private void PickChecker(int clickedPoint)
        {
            AudioManager.Instance.PlaySound("MenuClick");
            SelectedPoint = clickedPoint;
            if (NotOnBar())
                Board.HighlightChecker(clickedPoint);
            else
                Board.HighlightChecker(CurrentPlayer);
            PossibleDestinations = ViewInterface.GetLegalMovesForCheckerAtPosition(clickedPoint);
            SetState(GameState.PickDestination);
        }

        private void MoveChecker(int clickedPoint)
        {
            if (CurrentPlayer == White)
                SetOfMoves = WhiteInterface.move(SelectedPoint, clickedPoint);
            else
                SetOfMoves = BlackInterface.move(SelectedPoint, clickedPoint);
            Board.StopGlowPoints();
            Board.RemoveCheckerHighlight();
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
            if (Board.CheckerWasCaptured(CurrentPlayer, to))
                Board.Capture(to);
            if (NotOnBar() && OnBoard(to))
                Board.MoveChecker(SelectedPoint, to);
            else if (CanBearOff() && SetOfMoves.Count == 1)
                Board.BearOff(CurrentPlayer, SelectedPoint);

            else
                Board.MoveChecker(CurrentPlayer, to);
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
                BeginTurn();
            if (State == GameState.PickDestination)
                Board.GlowPoints(PossibleDestinations);
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
        #endregion

        #region Framework Methods

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
            Board = new Board(gameBoard);
            ViewInterface = new ViewInterface(Model);
            WhiteInterface = SetAI(White, WhiteAI);
            BlackInterface = SetAI(Black, BlackAI);

            //WhiteInterface = new PlayerInterface(Model, White, null);

            /* My mistake. The player interface was connected to BackgammonGame, 
             * but the opposite wasn't true.
               In BackgammonGame there is a method that looks like this:
            
            public PlayerInterface ConnectPlayer(CheckerColor color, Player player)
            {
                var pi = color == WHITE ? whitePlayer : blackPlayer;
                if (pi.HasPlayer())
                {
                    return null;
                }
                pi.SetPlayerIfNull(player);
                player.ConnectPlayerInterface(pi);
                return pi;
             }

               Use it like this:
             */

            //NaiveAI nai = new NaiveAI(null);
            //BlackInterface = Model.ConnectPlayer(Black, nai);

            //Now the backgammon game creates a new player interface, connects the player to 
            //the player interface and returns the playerinterface

            // Thanks mate.

            SetState(GameState.PickChecker);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            Image.UnloadContent();
            foreach (Image DiceImage in DiceImages)
                DiceImage.UnloadContent();
        }



        public override void Update(GameTime gameTime)
        {
            if (InputManager.Instance.KeyPressed(Keys.M)) AudioManager.Instance.ToggleAudio();

            int clickedPoint = Board.GetClickedPoint();

            switch (State)
            {
                case GameState.Animating:
                    if (!Board.InAnimation)
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
                    if (clickedPoint == SelectedPoint && NotOnBar())
                    { // Cancel selected checker
                        SetState(GameState.PickChecker);
                        AudioManager.Instance.PlaySound("MenuClick");
                    }
                    else if (PossibleDestinations.Contains(clickedPoint))
                        MoveChecker(clickedPoint);
                    else if (clickedPoint > 24 && PossibleDestinations.Contains(CurrentPlayer.BearOffPositionID()))
                        MoveChecker(BearOffTo());
                    break;
            }
            base.Update(gameTime);
            Image.Update(gameTime);
            Board.Update(gameTime);
            foreach (Image DiceImage in DiceImages)
                DiceImage.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Image.Draw(spriteBatch);
            Board.Draw(spriteBatch);
            foreach (Image DiceImage in DiceImages)
                DiceImage.Draw(spriteBatch);
        }

        #endregion

        private enum GameState
        {
            PickChecker,
            PickDestination,
            Animating
        }
    }
}
