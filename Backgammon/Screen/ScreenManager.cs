using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Backgammon.Screen
{
    public class ScreenManager
    {
        public Vector2 Dimension { private set; get; }
        public ContentManager Content { private set; get; }
        GameScreen currentScreen;

        // Make a singleton class
        private static ScreenManager instance;
        public static ScreenManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new ScreenManager();
                return instance;
            }
        }
        private ScreenManager()
        { // A private constructor overrides any default, public constructors.
            Dimension = new Vector2(640, 480);
            currentScreen = new SplashScreen();
        }



        public void LoadContent(ContentManager Content)
        {
            this.Content = new ContentManager(Content.ServiceProvider, "Content");
            currentScreen.LoadContent();
        }

        public void UnloadContent()
        {
            this.Content.Unload();
            currentScreen.UnloadContent();
        }

        public void Update(GameTime gameTime)
        {
            currentScreen.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            currentScreen.Draw(spriteBatch);
        }

    }
}
