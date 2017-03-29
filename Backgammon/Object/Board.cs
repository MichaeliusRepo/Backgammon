using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Backgammon.Screen;
using Backgammon.Audio;
using Microsoft.Xna.Framework;
using ModelDLL;
using Microsoft.Xna.Framework.Graphics;
using Backgammon.Input;

namespace Backgammon.Object
{
    public class Board
    {
        // Modify this value for X distance between points
        private readonly static float pointDistance = 76;

        // Modify algorithm's distance of points.
        private readonly static float leftX = 82;
        private readonly static float topY = 92;
        private readonly static float rightX = 540 + leftX;
        private readonly static float botY = 720 + 6 - topY;

        private readonly int[] defaultGameBoard = new int[] { -2, 0, 0, 0,  0,  5,
                                            0, 3, 0, 0,  0, -5,
                                            5, 0, 0, 0, -3,  0,
                                           -5, 0, 0, 0,  0,  2 };

        private int[] gameBoard;
        private Image Image = new Image() { Path = "Images/CheckerGlow", Effects = "FadeEffect", IsActive = false };

        internal List<Point> Points { get; private set; }

        public Board(int[] board)
        {
            if (board != null)
                gameBoard = board;
            else
                gameBoard = defaultGameBoard;
            createPoints();
            Image.LoadContent();
        }

        internal void PlaceGlow(Checker c)
        {
            Image.FadeEffect.FadeSpeed = 0.25f;
            Image.FadeEffect.MinAlpha = 0.75f;
            Image.Alpha = 0.0f;
            Image.IsActive = Image.FadeEffect.Increase = (c != null);
            Image.Position = c.Position;
        }

        internal Point GetClickedPoint()
        {
            foreach (Point p in Points)
                if (InputManager.Instance.IsWithinBounds(p.GetBounds()))
                    return p;
            return null;
        }

        internal Checker GetClickedChecker()
        {
            List<Checker> list = getTopPoints();
            foreach (Checker c in list)
                if (InputManager.Instance.IsWithinBounds(c.GetBounds()))
                    return c;
            return null;
        }

        private List<Checker> getTopPoints()
        {
            List<Checker> list = new List<Checker>();
            foreach (Point p in Points)
                if (p.GetTopChecker() != null)
                    list.Add(p.GetTopChecker());
            return list;
        }

        internal void MoveChecker(Point from, Point to)
        {
            AudioManager.Instance.PlaySound("Checker");
            if (from.IsEmpty())
                throw new Exception();
            from.SendToPoint(to);
        }

        private void createPoints()
        {
            Points = new List<Point>();

            for (int i = 1; i <= 6; i++) // White Home Board
                Points.Add(new Point(new Vector2(rightX + ((6 - i) * pointDistance), botY), getCheckers(i)));

            for (int i = 7; i <= 12; i++) // Bottom Outer Board
                Points.Add(new Point(new Vector2(leftX + ((12 - i) * pointDistance), botY), getCheckers(i)));

            for (int i = 13; i <= 18; i++) // Upper Outer Board
                Points.Add(new Point(new Vector2(leftX + ((i - 13) * pointDistance), topY), getCheckers(i)));

            for (int i = 19; i <= 24; i++) // Black Home Board
                Points.Add(new Point(new Vector2(rightX + ((i - 19) * pointDistance), topY), getCheckers(i)));
        }

        private List<Checker> getCheckers(int i)
        {
            int amountOfCheckers = gameBoard[i - 1];
            CheckerColor checkerColor;

            if (amountOfCheckers < 0)
            {
                checkerColor = CheckerColor.Black;
                amountOfCheckers *= -1; // make positive for use in loop
            }
            else
                checkerColor = CheckerColor.White;

            List<Checker> checkers = new List<Checker>();
            for (int k = 0; k < amountOfCheckers; k++)
                checkers.Add(new Checker(checkerColor));

            return checkers;
        }

        internal void Update(GameTime gameTime)
        {
            Image.Update(gameTime);
            foreach (Point p in Points)
                p.Update(gameTime);
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            Image.Draw(spriteBatch);
            foreach (Point p in Points)
                p.Draw(spriteBatch);
        }

    }
}
