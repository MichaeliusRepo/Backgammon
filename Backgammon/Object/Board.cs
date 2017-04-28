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
        private Image Image = new Image() { Path = "Images/CheckerGlow", Effects = "FadeEffect", IsActive = false, Scale = new Vector2(1.1f, 1.1f) };
        private Checker movingChecker;

        private List<Point> Points { get; set; }
        public bool InAnimation { get; private set; }
        private Point WhiteOnBoard, BlackOnBoard;

        public Board(int[] board)
        {
            gameBoard = board;
            createPoints();
            Image.LoadContent();
        }

        public void GlowPoints(List<int> list)
        {
            for (int i = 1; i < Points.Count; i++)
                Points[i].Glow(list.Contains(i));
        }

        public void StopGlowPoints()
        {
            GlowPoints(new List<int>());
        }

        public void HighlightChecker(int i)
        {
            Checker c = Points[i].GetTopChecker();
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

        public bool CheckerWasCaptured(CheckerColor playerColor, int to)
        {
            return (Points[to].GetAmount() == 1 && Points[to].GetTopChecker().Color != playerColor);
        }

        internal int GetClickedPoint()
        {
            foreach (Point p in Points)
                if (InputManager.Instance.WasClicked(p.GetBounds()))
                    return Points.IndexOf(p);
            return -1;
        }

        public void MoveChecker(int from, int to)
        {  //if (Points[from - 1].IsEmpty()) throw new Exception();
            MoveChecker(from, Points[to]);
        }

        public void MoveChecker(int from, Point to)
        {  //if (Points[from - 1].IsEmpty()) throw new Exception();
            AudioManager.Instance.PlaySound("Checker");
            InAnimation = true;
            movingChecker = Points[from].GetTopChecker();
            Points[from].SendToPoint(to);
        }

        public void Capture(int at)
        {
            if (Points[at].GetTopChecker().Color == CheckerColor.White)
                MoveChecker(at, WhiteOnBoard);
            else
                MoveChecker(at, BlackOnBoard);
        }

        private void createPoints()
        {
            WhiteOnBoard = new Point(new Vector2(540, topY), new List<Checker>());
            BlackOnBoard = new Point(new Vector2(540, botY), new List<Checker>());
            // Added dummy point to remove zero-indexing
            Points = new List<Point>() { new Point(Vector2.Zero, new List<Checker>()), WhiteOnBoard, BlackOnBoard };
            for (int i = 1; i <= 24; i++)
                Points.Insert(i,(new Point(findBoard(i), getCheckers(i))));
        }

        private Vector2 findBoard(int i)
        {
            if (i <= 6) // White Home Board
                return new Vector2(rightX + ((6 - i) * pointDistance), botY);
            if (i <= 12) //Bottom Outer Board
                return new Vector2(leftX + ((12 - i) * pointDistance), botY);
            if (i <= 18) // Upper Outer Board
                return new Vector2(leftX + ((i - 13) * pointDistance), topY);
            // Black Home Board
            return new Vector2(rightX + ((i - 19) * pointDistance), topY);
        }

        private List<Checker> getCheckers(int i)
        {
            int amountOfCheckers = gameBoard[i - 1]; // Game logic uses zero indexing :,-(
            CheckerColor checkerColor = CheckerColor.Black;
            if (amountOfCheckers < 0)
                amountOfCheckers *= -1; // make positive for use in loop
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
