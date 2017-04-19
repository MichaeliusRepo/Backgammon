using Backgammon.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backgammon.Screen
{
    public class Button
    {
        private Image Image;
        private Image Highlighted;
        private Vector2 Position;
        public bool Triggered { get; private set; }

        public Button(Image Image, Image Highlighted, Vector2 Position)
        {
            this.Image = Image;
            this.Position = Image.Position = Highlighted.Position = Position;
            this.Highlighted = Highlighted;
        }

        public Button(Image Image, Vector2 Position)
        {
            this.Image = Image;
            this.Position = Image.Position = Position;
        }

        internal void LoadContent()
        {
            Image.LoadContent();
            if (Highlighted != null)
                Highlighted.LoadContent();
        }

        internal void UnloadContent()
        {
            Image.UnloadContent();
            if (Highlighted != null)
                Highlighted.UnloadContent();
        }

        internal void Update(GameTime gameTime)
        {
            Triggered = WasClicked();
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            if (HoveredOver())
                Highlighted.Draw(spriteBatch);
            else
                Image.Draw(spriteBatch);
        }

        private bool WasClicked()
        {
            return InputManager.Instance.WasClicked(Image.GetBounds());
        }

        private bool HoveredOver()
        {
            return (Highlighted != null && InputManager.Instance.IsWithinBounds(Image.GetBounds()));
        }
    }
}
