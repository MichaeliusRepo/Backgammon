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

    // TODO Checker Physics
    // TODO Implement glow them points and fade 'em.
    // TODO Input Triggas

    public class BoardScreen : GameScreen
    {
        public Image Image = new Image { Path = "Images/Board", Position = new Vector2(540, 360) };
        public Board Board;

        public override void LoadContent()
        {
            base.LoadContent();
            Image.LoadContent();
            //Image.FadeEffect.FadeSpeed = 0.5f;
            Board = new Board();
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

            if (InputManager.Instance.KeyPressed(Keys.Enter, Keys.M))
            {
                //Board.Points[0].ReturnTopChecker().MoveToPosition(new Vector2(50, 60));
                if (!Board.Points[0].IsEmpty())
                    Board.MoveChecker(Board.Points[0], Board.Points[16]);
                else if (!Board.Points[5].IsEmpty())
                    Board.MoveChecker(Board.Points[5], Board.Points[7]);
            }

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
