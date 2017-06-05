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
        internal static CheckerColor CurrentPlayer { get; private set; }
        private GameState State = GameState.PickChecker;
        private List<Turn> TurnHistory = new List<Turn>();
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
            Board.RemoveCheckerHighlight();

            GetTurnHistory();
            if (TurnHistory.Count != 0) AIMove();
            else PlayerTurn();
        }

        private void GetTurnHistory()
        {
            if (!WhiteAI && !BlackAI) return;
            var history = Model.GetTurnHistory();
            if (history.Count == 0 || (WhiteAI && BlackAI)) return;
            TurnHistory = history;
            foreach (Turn t in new List<Turn>(TurnHistory))
                foreach (Move m in t.moves)
                    if (m.color == CurrentPlayer && !AIEnabled(CurrentPlayer))
                        TurnHistory.Remove(t);
        }

        private bool AIEnabled(CheckerColor c)
        {
            return (c == White) ? WhiteAI : BlackAI;
        }

        private void AIMove()
        {
            Turn t = TurnHistory[0];
            if (t.moves.Count != 0)
            {
                Move m = t.moves[0];
                t.moves.RemoveAt(0);
                CurrentPlayer = Board.GetColorAtPoint(m.from); // Code relies on knowing player color.
                DiceRollsLeft = t.dice;
                GenerateDiceImages();

                // Define 1) SetOfMoves (distance) and 2) SelectedPoint (starting pos) and 3) PossibleDestintions
                PossibleDestinations.Clear();
                PossibleDestinations.Add(m.to);

                SelectedPoint = m.from;
                var subtraction = (SelectedPoint == CurrentPlayer.GetBar()) ? 0 : SelectedPoint;
                if (m.to == CurrentPlayer.BearOffPositionID())
                    SetOfMoves.Add(6); // Checker is bearing off anyway
                else
                    SetOfMoves.Add(Math.Abs(m.to - subtraction));
                SetState(GameState.Animating);
            }
            else
            {
                TurnHistory.RemoveAt(0);
                BeginTurn();
            }

        }

        private bool BoardIsInconsistent()
        {
            int[] motherboard = Model.GetGameBoardState().getMainBoard();
            for (int i = 0; i < motherboard.Length; i++)
                if (Board.GetAmountOfCheckersAtPoint(i + 1) != motherboard[i])
                {
                    Console.WriteLine("View was found to be inconsistent with model. \n" +
                        Board.GetAmountOfCheckersAtPoint(i + 1) + " != " + motherboard[i]);
                    return true;
                }
            return false;
        }

        private void CheckInconsistency()
        {
            int[] motherboard = Model.GetGameBoardState().getMainBoard();
            for (int i = 0; i < motherboard.Length; i++)
                if (Board.GetAmountOfCheckersAtPoint(i + 1) != motherboard[i])
                    throw new Exception();
        }

        private void PlayerTurn()
        {
            //CheckInconsistency();
            DiceRollsLeft = ViewInterface.GetMoves();
            GenerateDiceImages();
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
            else // SetOfMoves is empty when the move wins the game :,((((((
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


            //Throws not implemented error
            NotifyView();

            //int[] gameBoard = BackgammonGame.DefaultGameBoard;
            int[] gameBoard = Board.TestBoard3;

            //Model = new BackgammonGame(gameBoard, new RealDice());
            Model = new BackgammonGame(gameBoard, new FakeDice(new int[] { 2, 3 }));
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

        public void NotifyView()
        {
            throw new NotImplementedException();
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
