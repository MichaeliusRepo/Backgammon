using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backgammon.Audio;
using Backgammon.Input;
using Backgammon.Object;
using Backgammon.AI_Networking;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ModelDLL;
using static ModelDLL.CheckerColor;

namespace Backgammon.Screen
{
    /*
     * 5) Inform players no available moves were left. Give time to animate dice rollz.
    *  2) Display Pips
    * */

    internal class BoardScreen : GameScreen, View
    {
        #region Fields

        private Image Image = new Image { Path = "Images/Board", Position = new Vector2(540, 360) };
        private Image GamePadArrow = new Image { Path = "Images/GamePadArrow", IsActive = false };
        private List<Image> DiceImages = new List<Image>();
        private Image WhitePips, BlackPips;

        private Board Board;
        private BackgammonGame Model;
        private ViewInterface ViewInterface;
        private PlayerType WhitePlayer => OptionScreen.WhiteAI.SwitchedOn ? PlayerType.Computer : PlayerType.Human;
        private PlayerType BlackPlayer => OptionScreen.BlackAI.SwitchedOn ? PlayerType.Computer : PlayerType.Human;
        internal CheckerColor CurrentPlayer => ViewInterface.GetNextPlayerToMove();
        private GameState State;

        private List<int> MovableCheckers, PossibleDestinations;
        private int SelectedPoint;
        private static readonly float[] DiceXPositions = { Board.midX - 4 * Board.leftX, Board.midX - 2 * Board.leftX,
            Board.midX + 2 * Board.leftX, Board.midX + 4 * Board.leftX };
        private static readonly float DiceScale = 0.8f;
        private int[] DiceRolls;
        private List<Change> NotifyPropertyChanged = new List<Change>();
        private int GamePadIndex = 0;

        #endregion

        #region Auxilary Methods

        public void NotifyView()
        {
            NotifyPropertyChanged.AddRange(Model.GetChanges());
            if (NotifyPropertyChanged.Count != 0) State = GameState.Animating;
            else throw new Exception("Model notified View but returned no changes.");
        }

        private void Instantiate()
        {
            int[] InitialBoard = BackgammonGame.DefaultGameBoard; // Board.TestBoard3;
            Board = new Board(InitialBoard);
            Model = new BackgammonGame(InitialBoard, new RealDice());
            ViewInterface = new ViewInterface(Model);
            Model.ConnectView(this);
            if (WhitePlayer == PlayerType.Online)
                TcpInstance.Instance.Instantiate(Model, White);
            if (BlackPlayer == PlayerType.Online)
                TcpInstance.Instance.Instantiate(Model, Black);
            if (OptionScreen.ShowPips.SwitchedOn)
            {
                WhitePips = new Image() { Text = "White: " + Model.GetGameBoardState().pip(White), Position = new Vector2(Board.midX, 700) };
                BlackPips = new Image() { Text = "Black: " + Model.GetGameBoardState().pip(Black), Position = new Vector2(Board.midX, 20) };
                WhitePips.LoadContent();
                BlackPips.LoadContent();
            }
        }

        private void PlayAnimation(Change c)
        {
            NotifyPropertyChanged.Remove(c);
            if (c is DiceState) AnimateDice(c as DiceState);
            if (c is Move) AnimateCheckers(c as Move);
            //else throw new Exception("Unrecognized change type returned.");
        }

        private void AnimateDice(DiceState c)
        {
            DiceRolls = c.GetDiceValues();
            GenerateDiceImages();
        }

        private void NotifyNoPossibleMovesAvailable()
        { // TODO
            Model.EndTurn(CurrentPlayer);
            Console.WriteLine("No possible moves were available.");
        }

        private void AnimateCheckers(Move m)
        {
            if (m.to == m.color.GetBar()) return; // Ditch capture animations.

            if (Board.CheckerWasCaptured(m.color, m.to))
                Board.Capture(m.to); // Capture check
            if (m.from == m.color.GetBar())
                Board.MoveFromBar(m.color, m.to); // From bar to board
            else if (m.to == m.color.BearOffPositionID())
                Board.BearOff(m.color, m.from); // From board to bearoff
            else
                Board.MoveChecker(m.from, m.to); // Basic board move
        }

        private void GenerateDice()
        {
            DiceRolls = Model.GetMovesLeft().ToArray();
            GenerateDiceImages();
        }

        private void GenerateDiceImages()
        {
            foreach (Image DiceImage in DiceImages)
                DiceImage.UnloadContent();
            DiceImages.Clear();

            for (int i = 0; i < DiceRolls.Length; i++)
                DiceImages.Add(new Image()
                {
                    Path = "Images/" + DiceRolls[i],
                    Scale = new Vector2(DiceScale, DiceScale),
                    Position = new Vector2(DiceXPositions[i], Board.midY)
                });

            foreach (Image DiceImage in DiceImages)
                DiceImage.LoadContent();
        }

        private void BeginTurn()
        {
            if (Board.GameOver())
                return;
            //CheckInconsistency(); // Gives and cures cancer at the same time. Have a taste!
            GenerateDice();
            Board.RemoveCheckerHighlight();
            Board.StopGlowPoints();
            if (ViewInterface.GetMoveableCheckers().Count == 0)
            {
                NotifyNoPossibleMovesAvailable();
                return;
            }

            if (IsPlayerType(CurrentPlayer, PlayerType.Online))
                OnlineTurn();
            else if (IsPlayerType(CurrentPlayer, PlayerType.Computer))
                AITurn();
            else
                PlayerTurn();
        }

        private bool IsPlayerType(CheckerColor c, PlayerType p) { return (c == White) ? (WhitePlayer == p) : (BlackPlayer == p); }

        private void CheckInconsistency()
        {
            int[] motherboard = Model.GetGameBoardState().getMainBoard();
            for (int i = 0; i < motherboard.Length; i++)
                if (Board.GetAmountOfCheckersAtPoint(i + 1) != motherboard[i])
                    throw new Exception("View was inconsistent with model." +
                        "\n At point" + (i + 1) + " view returned " +
                        Board.GetAmountOfCheckersAtPoint(i + 1) +
                        ", model's got " + motherboard[i]);
        }

        private void OnlineTurn() { TcpInstance.Instance.MakeMove(Model, CurrentPlayer); }

        private void AITurn() { AIInstance.Instance.Move(Model, CurrentPlayer); }

        private void PlayerTurn()
        {
            MovableCheckers = ViewInterface.GetMoveableCheckers();
            Board.GlowPoints(MovableCheckers);
            if (MovableCheckers.Contains(CurrentPlayer.GetBar()))
                PickChecker(CurrentPlayer.GetBar());
            else
                State = GameState.PickChecker;
        }

        private void PickChecker(int clickedPoint)
        {
            AudioManager.Instance.PlaySound("MenuClick");
            SelectedPoint = clickedPoint;
            Board.StopGlowPoints();
            Board.HighlightChecker(CurrentPlayer, clickedPoint);
            PossibleDestinations = ViewInterface.GetLegalMovesForCheckerAtPosition(clickedPoint);
            Board.GlowPoints(PossibleDestinations);
            State = GameState.PickDestination;
        }

        private void MoveChecker(int clickedPoint)
        {
            Model.Move(CurrentPlayer, SelectedPoint, clickedPoint);
            Board.StopGlowPoints();
            Board.RemoveCheckerHighlight();
        }

        private void GamePadMoveHighlight()
        {
            if (!GamePadArrow.IsActive)
                GamePadArrow.IsActive = true;
            else if (InputManager.Instance.GamePadButtonPressed(Buttons.DPadDown, Buttons.DPadUp))
            {
                GamePadIndex = (GamePadIndex + 13) % 26;
                GamePadArrow.Scale = GamePadArrow.Scale * (-1);
            }
            else
            {
                bool upperRow = (GamePadIndex < 13);
                if (InputManager.Instance.GamePadButtonPressed(Buttons.DPadLeft))
                    GamePadIndex--;
                else
                    GamePadIndex++;
                GamePadIndex = (upperRow) ? (GamePadIndex + 13) % 13 : (GamePadIndex % 13) + 13;
            }
            GamePadArrow.Position = new Vector2(Board.GamePadPointOrdering[GamePadIndex].Position.X, (GamePadIndex < 13) ? 20 : 700);
        }

        #endregion

        #region Framework Methods

        public override void LoadContent()
        {
            base.LoadContent();
            Image.LoadContent();
            GamePadArrow.LoadContent();
            Instantiate();
            BeginTurn();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            Image.UnloadContent();
            GamePadArrow.UnloadContent();
            if (OptionScreen.ShowPips.SwitchedOn)
            {
                WhitePips.UnloadContent();
                BlackPips.UnloadContent();
            }
            foreach (Image DiceImage in DiceImages)
                DiceImage.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (InputManager.Instance.KeyPressed(Keys.M)) AudioManager.Instance.ToggleAudio();
            if (InputManager.Instance.KeyPressed(Keys.R) || InputManager.Instance.GamePadButtonPressed(Buttons.Back))
                ScreenManager.Instance.ChangeScreens("SplashScreen");

            if (Board.GameOver() && (InputManager.Instance.KeyPressed(Keys.Enter) || InputManager.Instance.MouseLeftPressed() || InputManager.Instance.GamePadButtonPressed(Buttons.Start)))
                ScreenManager.Instance.ChangeScreens("SplashScreen");
            if (InputManager.Instance.GamePadButtonPressed(Buttons.DPadUp, Buttons.DPadDown, Buttons.DPadLeft, Buttons.DPadRight))
                GamePadMoveHighlight();
            int clickedPoint = (InputManager.Instance.GamePadButtonPressed(Buttons.A, Buttons.X, Buttons.Y)) ? Board.GetClickedPoint(GamePadIndex) : Board.GetClickedPoint();

            switch (State)
            {
                case GameState.Animating:
                    if (!Board.InAnimation)
                        if (NotifyPropertyChanged.Count != 0)
                            PlayAnimation(NotifyPropertyChanged[0]);
                        else
                            BeginTurn();
                    break;

                case GameState.PickChecker:
                    if (MovableCheckers.Contains(clickedPoint))
                        PickChecker(clickedPoint);
                    break;

                case GameState.PickDestination:
                    if ((clickedPoint == SelectedPoint && !SelectedPoint.Equals(CurrentPlayer.GetBar())) || InputManager.Instance.GamePadButtonPressed(Buttons.B))
                    { // Cancel selected checker
                        BeginTurn();
                        AudioManager.Instance.PlaySound("MenuClick");
                    }
                    else if (ViewInterface.GetLegalMovesForCheckerAtPosition(SelectedPoint).Contains(clickedPoint))
                        MoveChecker(clickedPoint);
                    break;
            }

            base.Update(gameTime);
            Image.Update(gameTime);
            GamePadArrow.Update(gameTime);
            Board.Update(gameTime);
            if (OptionScreen.ShowPips.SwitchedOn)
            {
                WhitePips.Text = "White: " + Model.GetGameBoardState().pip(White);
                BlackPips.Text = "Black: " + Model.GetGameBoardState().pip(Black);
                WhitePips.Update(gameTime);
                BlackPips.Update(gameTime);
            }
            foreach (Image DiceImage in DiceImages)
                DiceImage.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Image.Draw(spriteBatch);
            GamePadArrow.Draw(spriteBatch);
            Board.Draw(spriteBatch);
            if (OptionScreen.ShowPips.SwitchedOn)
            {
                WhitePips.Draw(spriteBatch);
                BlackPips.Draw(spriteBatch);
            }
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

        private enum PlayerType
        {
            Human,
            Computer,
            Online
        }

    }



}

