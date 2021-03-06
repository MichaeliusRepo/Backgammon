﻿using System;
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
using static ModelDLL.CheckerColor;

namespace Backgammon.Object
{
    internal class Board
    {
        // Modify this value for X distance between points
        private readonly static float pointDistance = 76;

        // Modify algorithm's distance of points.
        internal readonly static float midX = 1080 / 2;
        internal readonly static float midY = 720 / 2;
        internal readonly static float leftX = 82;
        private readonly static float topY = 92;
        private readonly static float rightX = midX + leftX;
        private readonly static float botY = 720 + 6 - topY;

        private int[] gameBoard;
        private Image Image = new Image() { Path = "Images/CheckerGlow", Effects = "FadeEffect", IsActive = false, Scale = new Vector2(1.1f, 1.1f) };
        private Checker movingChecker;

        private List<Point> Points { get; set; }
        internal List<Point> GamePadPointOrdering { get; private set; }
        internal bool InAnimation { get; private set; }
        private Point WhiteBar, BlackBar, WhiteBearOff, BlackBearOff;

        // Tests
        internal static readonly int[] TestBoard1 = new int[] { -5, 1, 1, 1,  1,  1,
                                            1, 0, 0, 0,  0, 0,
                                            -10, 0, 0, 0, 0,  0,
                                           0, 0, 0, 0,  0,  9 };

        internal static readonly int[] TestBoard2 = new int[] { -15, 2, 2, 2,  3,  3,
                                            3, 0, 0, 0,  0, 0,
                                            0, 0, 0, 0, 0,  0,
                                           0, 0, 0, 0,  0,  0 };

        internal static readonly int[] TestBoard3 = new int[] { 2, 2, 2, 3,  3,  2,
                                            1, 0, 0, 0, 0, 0,
                                            0, 0, 0, 0, 0,  -1,
                                           -2, -3, -3, -2, -2, -2 };

        internal static readonly int[] TestBoard4 = new int[] {-3, -3, -3, -3, -3,  4,
                                                              4,  4,  3,  0,  0,  0,
                                                              0,  0,  0,  0,  0,  0,
                                                              0,  0,  0,  0,  0,  0 };

        internal Board(int[] board)
        {
            gameBoard = board;
            CreatePoints();
            Image.LoadContent();
        }

        internal void GlowPoints(List<int> list)
        {
            for (int i = 1; i < Points.Count; i++)
                Points[i].Glow(list.Contains(i));
            WhiteBar.Glow(list.Contains(BackgammonGame.WHITE_BAR_ID));
            BlackBar.Glow(list.Contains(BackgammonGame.BLACK_BAR_ID));
            WhiteBearOff.Glow(list.Contains(BackgammonGame.WHITE_BEAR_OFF_ID));
            BlackBearOff.Glow(list.Contains(BackgammonGame.BLACK_BEAR_OFF_ID));
        }

        private bool NotOnBoard(List<int> list)
        {
            foreach (int i in list)
                if (i > 25)
                    return true;
            return false;
        }

        internal void StopGlowPoints()
        {
            GlowPoints(new List<int>());
        }

        internal void HighlightChecker(CheckerColor color, int i)
        {
            if (i == color.GetBar())
                HighlightImage(GetOnBarPoint(color).GetTopChecker());
            else
                HighlightImage(Points[i].GetTopChecker());
        }

        private Point GetOnBarPoint(CheckerColor color)
        {
            if (color == White)
                return WhiteBar;
            return BlackBar;
        }

        private Point GetBearOffPoint(CheckerColor color)
        {
            if (color == White)
                return WhiteBearOff;
            return BlackBearOff;
        }

        private void HighlightImage(Checker c)
        {
            Image.FadeEffect.FadeSpeed = 0.25f;
            Image.FadeEffect.MinAlpha = 0.75f;
            Image.Alpha = 0.0f;
            Image.IsActive = Image.FadeEffect.Increase = (c != null);
            Image.Position = c.Position;
        }

        internal void RemoveCheckerHighlight()
        {
            Image.IsActive = false;
        }

        internal bool CheckerWasCaptured(CheckerColor playerColor, int to)
        {
            try
            {
                return (Points[to].GetAmount() == 1 && Points[to].GetTopChecker().Color != playerColor);
            }
            catch (ArgumentOutOfRangeException) { return false; }
        }

        internal int GetClickedPoint(int i)
        {
            if (GamePadPointOrdering[i] == WhiteBearOff) return White.BearOffPositionID();
            else if (GamePadPointOrdering[i] == BlackBearOff) return Black.BearOffPositionID();
            return Points.IndexOf(GamePadPointOrdering[i]);
        }

        internal int GetClickedPoint()
        {
            foreach (Point p in Points)
                if (InputManager.Instance.WasClicked(p.GetBounds()))
                    if (p.Equals(WhiteBar)) return White.GetBar();
                    else if (p.Equals(WhiteBearOff)) return White.BearOffPositionID();
                    else if (p.Equals(BlackBar)) return Black.GetBar();
                    else if (p.Equals(BlackBearOff)) return Black.BearOffPositionID();
                    else
                        return Points.IndexOf(p);
            return -1;
        }

        internal void MoveChecker(int from, int to)
        {
            MoveChecker(Points[from], Points[to]);
        }

        internal void MoveFromBar(CheckerColor color, int to)
        {
            MoveChecker(GetOnBarPoint(color), Points[to]);
        }

        internal void BearOff(CheckerColor color, int from)
        {
            MoveChecker(Points[from], GetBearOffPoint(color));
        }

        private void MoveChecker(Point from, Point to)
        {
            AudioManager.Instance.PlaySound("Checker");
            InAnimation = true;
            movingChecker = from.GetTopChecker();
            from.SendToPoint(to);
        }

        internal void Capture(int at)
        {
            MoveChecker(Points[at], GetOnBarPoint(Points[at].GetTopChecker().Color));
        }

        internal int GetAmountOfCheckersAtPoint(int i)
        {
            if (i == CheckerColor.Black.GetBar())
                return BlackBar.GetAmount();
            else if (i == White.GetBar())
                return WhiteBar.GetAmount();
            else if (i == White.BearOffPositionID())
                return WhiteBearOff.GetAmount();
            else if (i == Black.BearOffPositionID())
                return BlackBearOff.GetAmount();
            else
            {
                int value = Points[i].GetAmount();
                if (value != 0 && Points[i].GetTopChecker().Color == Black)
                    return -value;
                return value;
            }
        }

        internal CheckerColor GetColorAtPoint(int i)
        {
            if (i == BackgammonGame.BLACK_BAR_ID || i == BackgammonGame.BLACK_BEAR_OFF_ID)
                return Black;
            if (i == BackgammonGame.WHITE_BAR_ID || i == BackgammonGame.WHITE_BEAR_OFF_ID)
                return White;
            return Points[i].GetTopChecker().Color;
        }

        internal bool GameOver() { return WhiteBearOff.GetAmount() == 15 || BlackBearOff.GetAmount() == 15; }

        private void CreatePoints()
        {
            WhiteBar = new Point(new Vector2(midX, topY), new List<Checker>());
            BlackBar = new Point(new Vector2(midX, botY), new List<Checker>());
            WhiteBearOff = new Point(new Vector2(1060, botY), new List<Checker>(), true);
            BlackBearOff = new Point(new Vector2(1060, topY), new List<Checker>(), true);

            // Added dummy point to remove zero-indexing
            Points = new List<Point>() { new Point(Vector2.Zero, new List<Checker>()),
                                                                        WhiteBar, BlackBar, WhiteBearOff, BlackBearOff };
            for (int i = 1; i <= 24; i++)
                Points.Insert(i, (new Point(FindBoard(i), GetCheckers(i))));

            // This is to help support the GamePad navigate around.
            GamePadPointOrdering = CreateGamePadList();
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
            CheckerColor checkerColor = Black;
            if (amountOfCheckers < 0)
                amountOfCheckers *= -1; // make positive for use in loop
            else
                checkerColor = White;
            List<Checker> checkers = new List<Checker>();
            for (int k = 0; k < amountOfCheckers; k++)
                checkers.Add(new Checker(checkerColor));
            return checkers;
        }

        private List<Point> CreateGamePadList()
        {
            var list = new List<Point>();
            for (int i = 13; i < 25; i++)
                list.Add(Points[i]);
            list.Add(BlackBearOff);
            for (int i = 12; i > 0; i--)
                list.Add(Points[i]);
            list.Add(WhiteBearOff);
            return list;
        }

        internal void Update(GameTime gameTime)
        {
            if (InAnimation && !movingChecker.Moving)
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
