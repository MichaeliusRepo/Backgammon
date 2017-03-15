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

        internal List<Point> Points { get; private set; }

        public Board()
        {
            gameBoard = defaultGameBoard;
            createPoints();
        }

        public Board(int[] board)
        {
            gameBoard = board;
            createPoints();
        }

        internal void MoveChecker(Point from, Point to)
        {
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
            foreach (Point p in Points)
                p.Update(gameTime);
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            foreach (Point p in Points)
                p.Draw(spriteBatch);
        }

    }
}
