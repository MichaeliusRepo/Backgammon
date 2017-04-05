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

        private int[] gameBoard;
        private Image Image = new Image() { Path = "Images/CheckerGlow", Effects = "FadeEffect", IsActive = false, Scale = new Vector2(1.2f, 1.2f) };
        private Checker movingChecker;

        internal List<Point> Points { get; private set; }
        public bool InAnimation { get; private set; }

        public Board(int[] board)
        {
            gameBoard = board;
            createPoints();
            Image.LoadContent();
        }

        public void GlowPoints(List<int> list)
        {
            for (int i = 0; i < Points.Count; i++)
                Points[i].Glow(list.Contains(i + 1));
        }

        public void StopGlowPoints()
        {
            GlowPoints(new List<int>());
        }

        public void HighlightChecker(int i)
        {
            Checker c = Points[i - 1].GetTopChecker();
            Image.FadeEffect.FadeSpeed = 0.25f;
            Image.FadeEffect.MinAlpha = 0.75f;
            Image.Alpha = 0.0f;
            Image.IsActive = Image.FadeEffect.Increase = (c != null);
            Image.Position = c.Position;
        }

        public void RemoveCheckerHighlight()
        {
            Image.IsActive = false;
        }

        internal int GetClickedPoint()
        {
            foreach (Point p in Points)
                if (InputManager.Instance.WasClicked(p.GetBounds()))
                    return Points.IndexOf(p) + 1;
            return -1;
        }

        public void MoveChecker(int from, int to)
        {
            AudioManager.Instance.PlaySound("Checker");
            if (Points[from - 1].IsEmpty())
                throw new Exception();
            movingChecker = Points[from - 1].GetTopChecker();
            InAnimation = true;
            Points[from - 1].SendToPoint(Points[to - 1]);
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
            if (InAnimation && !movingChecker.moving)
                InAnimation = false;
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
