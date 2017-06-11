using Backgammon.Input;
using Backgammon.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backgammon.Screen
{
    internal class Button
    {
        private Image Off;
        private Image On;
        internal Vector2 Position;
        internal bool Triggered { get; private set; }
        internal bool SwitchedOn => On.IsActive;

        internal Button(Image Off, Image On, Vector2 Position)
        {
            this.Off = Off;
            this.On = On;
            this.Position = Off.Position = On.Position = Position;
            On.IsActive = false;
        }

        internal Button(Vector2 Position, bool Enabled) : this(Position)
        {
            On.IsActive = Enabled;
        }

        internal Button(Vector2 Position)
        {
            this.Position = Position;
            Off = new Image() { Path = "Images/Off", Position = Position };
            On = new Image() { Path = "Images/On", Position = Position, IsActive = false };
        }

        internal void Trigger()
        {
            Triggered = true;
            On.IsActive = !On.IsActive;
            AudioManager.Instance.PlaySound("MenuClick");
        }

        public void LoadContent()
        {
            Off.LoadContent();
            On.LoadContent();
        }

        public void UnloadContent()
        {
            Off.UnloadContent();
            On.UnloadContent();
        }

        public void Update(GameTime gameTime)
        {
            Triggered = InputManager.Instance.WasClicked(Off.GetBounds());
            if (Triggered)
                Trigger();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (On.IsActive) On.Draw(spriteBatch);
            else Off.Draw(spriteBatch);
        }
    }
}
