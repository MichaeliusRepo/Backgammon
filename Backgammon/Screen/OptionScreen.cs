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
        internal static Button AudioMuted, ShowPips, WhiteAI, BlackAI;
        private List<Button> ButtonList;
        private int Hover = 0;
        private float HighlightOffset = 33;

        internal Image Image = new Image() { Path = "Images/OptionScreen", Position = new Vector2(540, 360) };
        internal Image Highlight = new Image { Path = "Images/OptionSelect", IsActive = false, Effects = "FadeEffect" };

        private void MakeButtons()
        {
            AudioMuted = new Button(new Vector2(200, 200), !AudioManager.Instance.AudioMuted());
            ShowPips = new Button(new Vector2(200, 400), true);
            WhiteAI = new Button(new Vector2(600, 200));
            BlackAI = new Button(new Vector2(600, 400), true);
            ButtonList = new List<Button>() { AudioMuted, ShowPips, WhiteAI, BlackAI };
        }

        private void MoveHighlight(Buttons button)
        {
            if (!Highlight.IsActive)
                Highlight.IsActive = true;
            else if (button == Buttons.DPadLeft)
                Hover = (Hover + 2) % 4;
            else if (Hover < 2)
                Hover = (Hover == 0) ? 1 : 0;
            else
                Hover = (Hover == 2) ? 3 : 2;
            Highlight.Position = ButtonList[Hover].Position + new Vector2(0, (Hover % 2 == 0) ? -HighlightOffset : HighlightOffset);
        }

        public override void LoadContent()
        {
            base.LoadContent();
            Image.LoadContent();
            Highlight.LoadContent();
            MakeButtons();
            foreach (Button b in ButtonList)
                b.LoadContent();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            Image.UnloadContent();
            Highlight.UnloadContent();
            foreach (Button b in ButtonList)
                b.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (InputManager.Instance.KeyPressed(Keys.Enter) || InputManager.Instance.GamePadButtonPressed(Buttons.Start))
                ScreenManager.Instance.ChangeScreens("BoardScreen");
            if (InputManager.Instance.GamePadButtonPressed(Buttons.DPadUp, Buttons.DPadDown))
                MoveHighlight(Buttons.DPadUp);
            else if (InputManager.Instance.GamePadButtonPressed(Buttons.DPadLeft, Buttons.DPadRight))
                MoveHighlight(Buttons.DPadLeft);
            if (InputManager.Instance.GamePadButtonPressed(Buttons.A, Buttons.B, Buttons.X, Buttons.Y))
                ButtonList[Hover].Trigger();

            if (AudioMuted.Triggered)
                AudioManager.Instance.ToggleAudio();

            base.Update(gameTime);
            Image.Update(gameTime);
            Highlight.Update(gameTime);
            foreach (Button b in ButtonList)
                b.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Image.Draw(spriteBatch);
            Highlight.Draw(spriteBatch);
            foreach (Button b in ButtonList)
                b.Draw(spriteBatch);
        }
    }
}
