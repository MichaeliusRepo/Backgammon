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
        public Image Image = new Image { Path = "Images/Board", Position = new Vector2(540, 360) };
        public Board board;
        private FakeDice fakeDice;
        public BackgammonGame Model;
        //public PlayerInterface Player1 = new PlayerInterface(new GameBoard)
        public ViewInterface ViewInterface;
        public PlayerInterface WhitePlayer;
        public PlayerInterface BlackPlayer; // inser race joke


        // Temporary!
        public List<int> temp1;

        private CheckerColor CurrentPlayer;

        private void PlayerTurn()
        {
            Console.WriteLine(CurrentPlayer);

            List<int> getDice = ViewInterface.GetMoves();
            Console.WriteLine(string.Join(", ", getDice));

            List<int> MovableCheckers = ViewInterface.GetMoveableCheckers();
            Console.WriteLine(string.Join(", ", MovableCheckers));

            board.GlowPoints(MovableCheckers);

            // TODO Click a checker here
            if (CurrentPlayer == CheckerColor.White)
                temp1 = ViewInterface.GetLegalMovesForCheckerAtPosition(24);
            if (CurrentPlayer == CheckerColor.Black)
                temp1 = ViewInterface.GetLegalMovesForCheckerAtPosition(17);

            Console.WriteLine(string.Join(", ", temp1));
        }

        public override void LoadContent()
        {
            base.LoadContent();
            Image.LoadContent();
            int[] defaultGameBoard = BackgammonGame.DefaultGameBoard;

            List<int[]> moves = new List<int[]>();
            moves.Add(new int[] { 5, 6 });
            moves.Add(new int[] { 2, 2 });
            fakeDice = new FakeDice(moves);
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

        public override void Update(GameTime gameTime)
        {
            // TODO Deal with inputs here
            //if (InputManager.Instance.KeyPressed(Keys.Enter, Keys.Z))
            //    ScreenManager.Instance.ChangeScreens("SettingsScreen");
            //if (!Board.Points[0].ReturnTopChecker().moving && InputManager.Instance.KeyPressed(Keys.Enter, Keys.M))
            //if (!board.Points[0].IsEmpty())
            //    board.Points[0].Glow(true); // TEST - Delete this line
            //else if (!board.Points[5].IsEmpty())
            //    board.Points[5].Glow(true); // TEST - Delete this line

            // Dummy method
            if (InputManager.Instance.KeyPressed(Keys.Z))
            {
                board.PlaceGlow(24);
                board.GlowPoints(temp1);
            }

            if (InputManager.Instance.KeyPressed(Keys.X))
            {
                WhitePlayer.move(24, new List<int>() { 6, 5 });
                board.StopGlowPoints();
                board.RemoveGlow();
                board.MoveChecker(23, 12);
                Console.WriteLine(string.Join(", ", ViewInterface.GetGameBoardState().getMainBoard()));
            }

            if (InputManager.Instance.KeyPressed(Keys.C))
            {
                //fakeDice.SetReturnValues(new int[] { 2,2});
                PlayerTurn();

            }

            if (InputManager.Instance.KeyPressed(Keys.V))
            {
                board.PlaceGlow(17);
                board.GlowPoints(temp1);
            }

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

            base.Update(gameTime);
            Image.Update(gameTime);
            board.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Image.Draw(spriteBatch);
            board.Draw(spriteBatch);
        }
    }
}
