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
    public class BoardScreen : GameScreen
    {
        public Image Image = new Image { Path = "Images/Board", Position = new Vector2(540, 360)};
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
            base.Update(gameTime);
            Image.Update(gameTime);
            Board.Update(gameTime);

            // TODO Deal with inputs here
            //if (InputManager.Instance.KeyPressed(Keys.Enter, Keys.Z))
            //    ScreenManager.Instance.ChangeScreens("SettingsScreen");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Image.Draw(spriteBatch);
            Board.Draw(spriteBatch);
        }
    }
}
