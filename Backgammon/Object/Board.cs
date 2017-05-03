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
        private readonly static float midX = 540;
        private readonly static float leftX = 82;
        private readonly static float topY = 92;
        private readonly static float rightX = midX + leftX;
        private readonly static float botY = 720 + 6 - topY;

        private int[] gameBoard;
        private Image Image = new Image() { Path = "Images/CheckerGlow", Effects = "FadeEffect", IsActive = false, Scale = new Vector2(1.1f, 1.1f) };
        private Checker movingChecker;

        private List<Point> Points { get; set; }
        public bool InAnimation { get; private set; }
        private Point WhiteOnBoard, BlackOnBoard, WhiteBoreOff, BlackBoreOff;

        // Tests
        public static readonly int[] TestBoard1 = new int[] { -5, 1, 1, 1,  1,  1,
                                            1, 0, 0, 0,  0, 0,
                                            -10, 0, 0, 0, 0,  0,
                                           0, 0, 0, 0,  0,  9 };

        public static readonly int[] TestBoard2 = new int[] { -15, 2, 2, 2,  3,  3,
                                            3, 0, 0, 0,  0, 0,
                                            0, 0, 0, 0, 0,  0,
                                           0, 0, 0, 0,  0,  0 };

        public static readonly int[] TestBoard3 = new int[] { 2, 2, 2, 3,  3,  2,
                                            1, 0, 0, 0, 0, 0,
                                            0, 0, 0, 0, 0,  -1,
                                           -2, -3, -3, -2, -2, -2 };

        public static readonly int[] TestBoard4 = new int[] {-3, -3, -3, -3, -3,  4,
                                                              4,  4,  3,  0,  0,  0,
                                                              0,  0,  0,  0,  0,  0,
                                                              0,  0,  0,  0,  0,  0 };

        public Board(int[] board)
        {
            gameBoard = board;
            CreatePoints();
            Image.LoadContent();
        }

        public void GlowPoints(List<int> list)
        {
            for (int i = 1; i < Points.Count; i++)
                Points[i].Glow(list.Contains(i));
            bool canBearOff = NotOnBoard(list);
            WhiteBoreOff.Glow(canBearOff && BoardScreen.CurrentPlayer == CheckerColor.White);
            BlackBoreOff.Glow(canBearOff && BoardScreen.CurrentPlayer == CheckerColor.Black);
        }

        private bool NotOnBoard(List<int> list)
        {
            foreach (int i in list)
                if (i > 25)
                    return true;
            return false;
        }

        public void StopGlowPoints()
        {
            GlowPoints(new List<int>());
        }

        public void HighlightChecker(int i)
        {
            Checker c = Points[i].GetTopChecker();
            HighlightImage(c);
        }

        public void HighlightChecker(CheckerColor color)
        {
            HighlightImage(GetPointOnBoard(color).GetTopChecker());
        }

        private Point GetPointOnBoard(CheckerColor color)
        {
            if (color == CheckerColor.White)
                return WhiteOnBoard;
            else
                return BlackOnBoard;
        }

        private Point GetBearOffPoint(CheckerColor color)
        {
            if (color == CheckerColor.White)
                return WhiteBoreOff;
            else
                return WhiteBoreOff;
        }

        private void HighlightImage(Checker c)
        {
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
        {
            MoveChecker(Points[from], Points[to]);
        }

        public void MoveChecker(CheckerColor color, int to)
        {
            MoveChecker(GetPointOnBoard(color), Points[to]);
        }

        public void MoveChecker(Point from, Point to)
        {
            AudioManager.Instance.PlaySound("Checker");
            InAnimation = true;
            movingChecker = from.GetTopChecker();
            from.SendToPoint(to);
        }

        public void BearOff(CheckerColor color,int from)
        {
            MoveChecker(Points[from], GetBearOffPoint(color));
        }

        public void Capture(int at)
        {
            MoveChecker(Points[at], GetPointOnBoard(Points[at].GetTopChecker().Color));
            //if (Points[at].GetTopChecker().Color == CheckerColor.White)
            //    MoveChecker(Points[at], WhiteOnBoard);
            //else
            //    MoveChecker(Points[at], BlackOnBoard);
        }

        private void CreatePoints()
        {
            WhiteOnBoard = new Point(new Vector2(midX, topY), new List<Checker>());
            BlackOnBoard = new Point(new Vector2(midX, botY), new List<Checker>());
            WhiteBoreOff = new Point(new Vector2(41, botY), new List<Checker>());
            BlackBoreOff = new Point(new Vector2(41, topY), new List<Checker>());

            // Added dummy point to remove zero-indexing
            Points = new List<Point>() { new Point(Vector2.Zero, new List<Checker>()),
                                                                        WhiteOnBoard, BlackOnBoard, WhiteBoreOff, BlackBoreOff };
            for (int i = 1; i <= 24; i++)
                Points.Insert(i, (new Point(FindBoard(i), GetCheckers(i))));
        }

        private Vector2 FindBoard(int i)
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

        private List<Checker> GetCheckers(int i)
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
