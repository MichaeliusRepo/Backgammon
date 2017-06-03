using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Backgammon.Screen;
using Backgammon.Input;
using Backgammon.Audio;

namespace Backgammon.Screen
{
    internal class OptionScreen : GameScreen
    {
        // Audio, Pips, Player/AI for each color.
        //internal static bool AudioMuted => AudioManager.Instance.AudioMuted();
        //internal static bool ShowPips { get; private set; }
        //internal static bool WhiteAI = false; // { get; private set; }
        //internal static bool BlackAI = true; // { get; private set; }

        internal static Button AudioMuted, ShowPips, WhiteAI, BlackAI;
        private List<Button> Buttons;

        internal Image Image = new Image() { Path = "Images/SplashScreenBlurry", Position = new Vector2(540, 360) };

        private void MakeButtons()
        {
            AudioMuted = new Button(new Vector2(200, 200));
            ShowPips = new Button(new Vector2(200, 400), true);
            WhiteAI = new Button(new Vector2(600, 200));
            BlackAI = new Button(new Vector2(600, 400), true);
            Buttons = new List<Button>() { AudioMuted, ShowPips, WhiteAI, BlackAI};
        }

        public override void LoadContent()
        {
            base.LoadContent();
            Image.LoadContent();
            MakeButtons();
            foreach (Button b in Buttons)
                b.LoadContent();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            Image.UnloadContent();
            foreach (Button b in Buttons)
                b.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Image.Update(gameTime);

            foreach (Button b in Buttons)
                b.Update(gameTime);

            if (AudioMuted.Triggered) AudioManager.Instance.ToggleAudio();

            if (InputManager.Instance.KeyPressed(Keys.Enter))
                ScreenManager.Instance.ChangeScreens("BoardScreen");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Image.Draw(spriteBatch);
            foreach (Button b in Buttons)
                b.Draw(spriteBatch);
        }
    }
}
