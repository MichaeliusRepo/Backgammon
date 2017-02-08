using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Backgammon.Screen
{
    public class SplashScreen : GameScreen
    {
        Texture2D image;
        string path;

        public override void LoadContent()
        {
            base.LoadContent();
            path = "Images/Backgammons";
            image = content.Load<Texture2D>(path);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //base.Draw(spriteBatch);
            spriteBatch.Draw(image, Vector2.Zero, Color.White);
        }


    }
}
