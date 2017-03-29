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

        private Vector2 Acceleration = Vector2.Zero;
        private Vector2 Velocity = Vector2.Zero;
        private float DeltaHalfDistance;
        private Vector2 FinalPosition;
        private float Timer;
        private Point TargetPoint = null;

        internal bool moving { get; private set; }

        // Modify this value to change checker size
        private readonly static Vector2 Size = new Vector2(0.08f, 0.08f);

        // Do not modify.
        private readonly static float Time = 4;
        private readonly static float Second = 1.0f;

        internal Rectangle GetBounds()
        {
            return Image.GetBounds();
        }

        internal Checker(CheckerColor Color)
        {
            this.Color = Color;
            Image = new Image() { Position = Position, Scale = Size };
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

        internal void MoveToPoint(Point p)
        {
            TargetPoint = p;
            MoveToPosition(p.ReceivingPosition);
        }

        internal void MoveToPosition(Vector2 NewPosition)
        { // Don't worry, I did the math on paper first.
            FinalPosition = NewPosition;
            Vector2 DeltaPosition = NewPosition - Position;
            DeltaHalfDistance = Vector2.Distance(Position, FinalPosition) / 2;
            Acceleration = new Vector2(DeltaPosition.X / (Time * Time), DeltaPosition.Y / (Time * Time));
            moving = true;
        }

        private void MoveByDeltaTime(float DeltaTime)
        {
            if (Vector2.Distance(Position, FinalPosition) < DeltaHalfDistance)
            {
                Acceleration = -Acceleration;
                DeltaHalfDistance = -1; // Set to -1 to avoid this block, as distance cannot be less than 0
            }
            Velocity += (Acceleration * DeltaTime);
            Position += (Velocity);
            Timer += DeltaTime;

            if (Timer >= Second) // Moving animations last exactly a second.
            {
                Position = FinalPosition;
                moving = false;
                Acceleration = Velocity = Vector2.Zero;
                if (TargetPoint != null)
                {
                    TargetPoint.ArrangeCheckers();
                    TargetPoint = null;
                }
            }
            Image.Position = Position;
        }

        internal void Update(GameTime gameTime)
        {
            if (moving)
                MoveByDeltaTime((float)gameTime.ElapsedGameTime.TotalSeconds);
            Image.Update(gameTime);
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            Image.Draw(spriteBatch);
        }
    }
}
