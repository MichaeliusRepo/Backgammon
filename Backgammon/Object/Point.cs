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

    internal class Point
    {
        internal int Number { get; private set; }
        internal Vector2 Position { get; private set; }
        private List<Checker> Checkers;

        // Modify this value for Y distance (if < 5) between checkers
        private readonly static float checkerDistance = 50.0f;

        internal Point(Vector2 Position, List<Checker> Checkers)
        {
            this.Position = Position;
            this.Checkers = Checkers;
            arrangeCheckers();
        }

        internal void AddChecker(Checker checker)
        {
            Checkers.Add(checker);
            arrangeCheckers();
        }

        internal void RemoveChecker(Checker checker)
        {
            Checkers.Remove(checker);
            arrangeCheckers();
        }

        private void arrangeCheckers()
        {
            float dist = checkerDistance;
            if (Checkers.Count > 5)
                dist = checkerDistance * (1 / Checkers.Count);

            for (int i = 0; i < Checkers.Count; i++)
                if (Position.Y > 360) // Is this Point at bottom?
                    Checkers[i].SetPosition(Position.X, Position.Y - i * checkerDistance);
                else
                    Checkers[i].SetPosition(Position.X, Position.Y + i * checkerDistance);
        }

        internal void Update(GameTime gameTime)
        {
            foreach (Checker c in Checkers)
                c.Update(gameTime);
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            foreach (Checker c in Checkers)
                c.Draw(spriteBatch);
        }


    }
}
