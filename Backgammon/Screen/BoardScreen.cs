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
     * 5) Inform players no available moves were left. Give time to animate dice rollz.
    *  2) Display Pips
    * */

    internal class BoardScreen : GameScreen, View
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
        internal CheckerColor CurrentPlayer => ViewInterface.GetNextPlayerToMove();
        private GameState State;
        private List<int> MovableCheckers, PossibleDestinations;
        private int SelectedPoint;
        private float[] DiceXPositions = { Board.midX - 4 * Board.leftX, Board.midX - 2 * Board.leftX,
            Board.midX + 2 * Board.leftX, Board.midX + 4 * Board.leftX };
        private float DiceScale = 0.8f;
        private int[] DiceRolls;
        private List<Change> NotifyPropertyChanged = new List<Change>();

        #endregion

        #region The Vision of The Scandinavian Wonderboys

        public void NotifyView()
        {
            NotifyPropertyChanged.AddRange(Model.GetChanges());
            if (NotifyPropertyChanged.Count != 0) State = GameState.Animating;
            else throw new Exception("Model notified View but returned no changes.");
        }

        private void Instantiate()
        {
            int[] InitialBoard = Board.TestBoard3; // BackgammonGame.DefaultGameBoard;
            Board = new Board(InitialBoard);
            Model = new BackgammonGame(InitialBoard, new FakeDice(new int[] { 2, 3 }));
            ViewInterface = new ViewInterface(Model);
            WhiteInterface = new PlayerInterface(Model, White, null);
            BlackInterface = new PlayerInterface(Model, Black, null);
            Model.ConnectView(this);
        }

        private void PlayAnimation(Change c)
        {
            NotifyPropertyChanged.Remove(c);
            if (c is DiceState) AnimateDice(c as DiceState);
            else if (c is Move) AnimateCheckers(c as Move);
            else throw new Exception("Unrecognized change type returned.");
        }

        private void AnimateDice(DiceState c)
        {
            DiceRolls = c.GetDiceValues();
            GenerateDiceImages();
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
            //CheckInconsistency(); // Gives and cures cancer at the same time. Have a try!
            Board.RemoveCheckerHighlight();
            Board.StopGlowPoints();
            if (AIEnabled(CurrentPlayer)) AITurn();
            else PlayerTurn();
        }

        private bool AIEnabled(CheckerColor c) { return (c == White) ? WhiteAI : BlackAI; }

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

        private void AITurn()
        { // This AI is a bastard child of the NaiveAI; I must throw her into the river and convince the village elder that a child was never concieved in the first place!
            SelectedPoint = ViewInterface.GetMoveableCheckers().First();
            MoveChecker(ViewInterface.GetLegalMovesForCheckerAtPosition(SelectedPoint).Last());
        }

        private void PlayerTurn()
        {
            MovableCheckers = ViewInterface.GetMoveableCheckers();
            Board.GlowPoints(MovableCheckers);
            if (MovableCheckers.Contains(CurrentPlayer.GetBar())) PickChecker(CurrentPlayer.GetBar());
            else State = GameState.PickChecker;
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
            if (CurrentPlayer == White) WhiteInterface.move(SelectedPoint, clickedPoint);
            else BlackInterface.move(SelectedPoint, clickedPoint);
            Board.StopGlowPoints();
            Board.RemoveCheckerHighlight();
        }

        #endregion

        #region Framework Methods

        public override void LoadContent()
        {
            base.LoadContent();
            Image.LoadContent();
            foreach (Image DiceImage in DiceImages)
                DiceImage.LoadContent();
            Instantiate();
            BeginTurn();
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
                    if (clickedPoint == SelectedPoint && !SelectedPoint.Equals(CurrentPlayer.GetBar()))
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

