using System;
using System.Linq;
using ModelDLL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTest
{
    [TestClass]
    public class TestMetaRules
    {

        BackgammonGame bg;
        FakeDice fd;
        int[] initialGameBoard;
        Player player1;
        Player player2;
        PlayerInterface pi;

        [TestInitialize]
        public void Initialize()
        {
            player1 = null;
            player2 = null;
            pi = null;

            initialGameBoard = new int[] { -2, 0, 0, 0,  0,  5,
                                            0, 3, 0, 0,  0, -5,
                                            5, 0, 0, 0, -3,  0,
                                           -5, 0, 0, 0,  0,  2 };
            fd = new FakeDice(new int[] { 1, 2 });
            bg = new BackgammonGame(initialGameBoard, fd);

        }


        [TestMethod]
        public void TestPlayerToMoveIsWhiteFromInitialState()
        {
            Assert.AreEqual(CheckerColor.White, bg.playerToMove());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestIllegalToMoveIfNotYourTurn()
        {
            bg.Move(CheckerColor.Black, 19, 20);
        }

        [TestMethod]
        public void TestTurnDoesNotChangeIfNotAllMovesDepleted()
        {
            bg.Move(CheckerColor.White, 6, 4);
            Assert.AreEqual(CheckerColor.White, bg.playerToMove());
        }

        [TestMethod]
        public void TestTurnDoesChangeIfAllMovesDepleted()
        {
            bg.Move(CheckerColor.White, 6, 4);
            bg.Move(CheckerColor.White, 4, 3);
            Assert.AreEqual(CheckerColor.Black, bg.playerToMove());
        }

        [TestMethod]
        public void TestMovesDepletedForMovesConsistingOfMultipleDice()
        {
            bg.Move(CheckerColor.White, 6, 3);
            Assert.AreEqual(CheckerColor.Black, bg.playerToMove());
        }

        [TestMethod]
        public void TestPlayerInterfaceIsMyTurn()
        {

            PlayerInterface white = new PlayerInterface(bg, CheckerColor.White, player1);
            PlayerInterface black = new PlayerInterface(bg, CheckerColor.Black, player2);
            Assert.IsTrue(white.IsMyTurn());
            Assert.IsFalse(black.IsMyTurn());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestPlayerGetMovesLeftThrowsExceptionIfNotYourTurn()
        {
            PlayerInterface black = new PlayerInterface(bg, CheckerColor.Black, player2);
            black.GetMovesLeft();
        }

        [TestMethod]
        public void TestPlayerGetMovesLeft()
        {

            PlayerInterface white = new PlayerInterface(bg, CheckerColor.White, player1);
            List<int> movesLeft = white.GetMovesLeft();

            Assert.AreEqual(2, movesLeft.Count());

            //These two values are initilalized further up, in the fake dice
            Assert.IsTrue(movesLeft.Contains(1));
            Assert.IsTrue(movesLeft.Contains(2));

            bg.Move(CheckerColor.White, 6, 4);

            movesLeft = white.GetMovesLeft();
            Assert.AreEqual(1, movesLeft.Count());
            Assert.IsTrue(movesLeft.Contains(1));
        }


    }
}
