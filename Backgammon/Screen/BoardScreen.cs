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
        public Board Board;

        public override void LoadContent()
        {
            base.LoadContent();
            Image.LoadContent();
            Board = new Board(null);
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
            if (!Board.Points[0].IsEmpty())
                Board.Points[0].Glow(true); // TEST - Delete this line
            else if (!Board.Points[5].IsEmpty())
                Board.Points[5].Glow(true); // TEST - Delete this line

            if (InputManager.Instance.KeyPressed(Keys.Enter))
                //Board.Points[0].ReturnTopChecker().MoveToPosition(new Vector2(50, 60));
                if (!Board.Points[0].IsEmpty())
                    Board.MoveChecker(Board.Points[0], Board.Points[16]);
                else if (!Board.Points[5].IsEmpty())
                    Board.MoveChecker(Board.Points[5], Board.Points[7]);

            if (InputManager.Instance.KeyPressed(Keys.T))
                Board.PlaceGlow(Board.Points[7].GetTopChecker());

            base.Update(gameTime);
            Image.Update(gameTime);
            Board.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Image.Draw(spriteBatch);
            Board.Draw(spriteBatch);
        }
    }
}
