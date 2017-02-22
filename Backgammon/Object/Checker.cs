using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backgammon.Screen;
using Microsoft.Xna.Framework;
using ModelDLL;
using Microsoft.Xna.Framework.Graphics;

namespace Backgammon.Object
{

    internal class Checker
    {
        internal CheckerColor Color { get; private set; }
        internal Vector2 Position { get; private set; }
        private Image Image;

        // Modify this value to change checker size!
        private readonly static Vector2 Size = new Vector2(0.09f, 0.09f);

        internal Checker(CheckerColor Color)
        {
            this.Color = Color;
            Image = new Image() { Position = this.Position, Scale = Size };
            if (Color == CheckerColor.White)
                Image.Path = "Images/White Checker";
            else
                Image.Path = "Images/Black Checker";
            Image.LoadContent();
        }

        internal void SetPosition(Vector2 Position)
        {
            this.Position = Position;
            Image.Position = Position;
        }

        internal void SetPosition(float X, float Y)
        {
            this.Position = new Vector2(X, Y);
            Image.Position = Position;
        }

        internal void MoveToPosition(Vector2 NewPosition)
        {
            throw new NotImplementedException();
        }

        internal void Update(GameTime gameTime)
        {
            Image.Update(gameTime);
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            Image.Draw(spriteBatch);
        }
    }
}
