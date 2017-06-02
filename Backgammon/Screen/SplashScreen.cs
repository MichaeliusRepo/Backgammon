using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Backgammon.Input;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Backgammon.Screen
{
    public class SplashScreen : GameScreen
    {
        public Image Image = new Image() { Path = "Images/SplashScreen", Position = new Vector2(540, 360)};

        public override void LoadContent()
        {
            base.LoadContent();
            Image.LoadContent();
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

            if (InputManager.Instance.KeyPressed(Keys.Enter) || InputManager.Instance.MouseLeftPressed())
                //ScreenManager.Instance.ChangeScreens("SettingsScreen");
                ScreenManager.Instance.ChangeScreens("BoardScreen");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Image.Draw(spriteBatch);
        }

    }
}
