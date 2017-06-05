using System;
using System.Text;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelDLL;
using static ModelDLL.CheckerColor;
using System.Collections.Generic;

namespace UnitTest
{

    [TestClass]
    public class TestGamePlayCorrectly
    {


        List<int[]> moves = new List<int[]>();
        BackgammonGame bg;
        FakeDice fd;

        private int[] ar(int a, int b) { return new int[] { a, b }; }
        private bool arrayEqual(int[] a, int[] b)
        {
            return Enumerable.SequenceEqual(a, b);
        }

        [TestInitialize]
        public void initialize()
        {
            moves = new List<int[]>()
            {
                ar(1,2), ar(4,3), ar(1,1), ar(1,1), ar(6,5), ar(3,3)
            };
            fd = new FakeDice(moves);
            bg = new BackgammonGame(BackgammonGame.DefaultGameBoard, fd);
            
        }

        [TestMethod]
        public void TestChangesContainsInitialMovesWhenGameStartesWhite()
        {
            fd.SetReturnValues(ar(1, 2));


            var changes = bg.GetChanges();
            Assert.AreEqual(1, changes.Count());


            Assert.IsTrue(changes[0].IsDiceState(), "expected true, got " + changes[0].IsDiceState());

            var ds = changes[0].AsDiceState();
            Assert.IsTrue(Enumerable.SequenceEqual(ds.GetDiceValues(), ar(1, 2)), "Expected {1,2}, was " + string.Join(",", ds.GetDiceValues()));
        }

        [TestMethod]
        public void TestChangesContainsInitialMovesWhenGameStartesBlack()
        {
            fd.SetReturnValues(ar(1, 2));
            bg = new BackgammonGame(BackgammonGame.DefaultGameBoard, fd, 0, 0, 0, 0, Black);

            var changes = bg.GetChanges();
            Assert.AreEqual(1, changes.Count());


            Assert.IsTrue(changes[0].IsDiceState(), "expected true, got " + changes[0].IsDiceState());

            var ds = changes[0].AsDiceState();
            Assert.IsTrue(Enumerable.SequenceEqual(ds.GetDiceValues(), ar(1, 2)), "Expected {1,2}, was " + string.Join(",", ds.GetDiceValues()));
        }


        [TestMethod]
        public void TestChangesRegisteredProperlyAfterOneMoveWhite()
        {
            List<int[]> moves = new List<int[]> { ar(1, 3) };
            fd = new FakeDice(moves);
            bg = new BackgammonGame(BackgammonGame.DefaultGameBoard, fd);

            bg.Move(White, 6, 3);

            var changes = bg.GetChanges();
            Assert.AreEqual(3, changes.Count());

            var c1 = changes[0];
            var c2 = changes[1];
            var c3 = changes[2];

            Assert.IsTrue(c1.IsDiceState(), "c1 is not dice state");
            Assert.IsTrue(c2.IsMove(), "c2 is not move");
            Assert.IsTrue(c3.IsDiceState(), "c3 is not dice state");


            Assert.IsTrue(arrayEqual(ar(1, 3), c1.AsDiceState().GetDiceValues()), "Expected 1,3 but got " + string.Join(",", c1.AsDiceState().GetDiceValues()));
            Assert.AreEqual("w 6 3", c2.AsMove().DebugString(), "Expected w 6 3, got "  + c2.AsMove().DebugString());
            Assert.IsTrue(arrayEqual(new int[] { 1 }, c3.AsDiceState().GetDiceValues()));
        }

        [TestMethod]
        public void TestChangesRegisteredProperlyAfterOneMoveBlack()
        {

            List<int[]> moves = new List<int[]> { ar(1, 3) };
            fd = new FakeDice(moves);
            bg = new BackgammonGame(BackgammonGame.DefaultGameBoard, fd, 0, 0, 0, 0, Black);

            bg.Move(Black, 1, 2);

            var changes = bg.GetChanges();
            Assert.AreEqual(3, changes.Count());

            var c1 = changes[0];
            var c2 = changes[1];
            var c3 = changes[2];

            Assert.IsTrue(c1.IsDiceState(), "c1 is not dice state");
            Assert.IsTrue(c2.IsMove(), "c2 is not move");
            Assert.IsTrue(c3.IsDiceState(), "c3 is not dice state");


            Assert.IsTrue(arrayEqual(ar(1, 3), c1.AsDiceState().GetDiceValues()), "Expected 1,3 but got " + string.Join(",", c1.AsDiceState().GetDiceValues()));
            Assert.AreEqual("b 1 2", c2.AsMove().DebugString(), "Expected b 1 2, got " + c2.AsMove().DebugString());
            Assert.IsTrue(arrayEqual(new int[] { 3 }, c3.AsDiceState().GetDiceValues()));
        }

        [TestMethod]
        public void TestChangesRegisteredProperlyAfterTwoMovesWhite()
        {
            List<int[]> moves = new List<int[]> { ar(1, 3), ar(4,5) };
            fd = new FakeDice(moves);
            bg = new BackgammonGame(BackgammonGame.DefaultGameBoard, fd);

            bg.Move(White, 6, 3);
            bg.Move(White, 6, 5);

            var changes = bg.GetChanges();
            Assert.AreEqual(5, changes.Count());

            var c1 = changes[0];
            var c2 = changes[1];
            var c3 = changes[2];
            var c4 = changes[3];
            var c5 = changes[4];

            Assert.IsTrue(c1.IsDiceState(), "c1 is not dice state");
            Assert.IsTrue(c2.IsMove(), "c2 is not move");
            Assert.IsTrue(c3.IsDiceState(), "c3 is not dice state");
            Assert.IsTrue(c4.IsMove(), "c4 is not move");
            Assert.IsTrue(c5.IsDiceState(), "c5 is not dice state");



            Assert.IsTrue(arrayEqual(ar(1, 3), c1.AsDiceState().GetDiceValues()), "Expected 1,3 but got " + string.Join(",", c1.AsDiceState().GetDiceValues()));

            Assert.AreEqual("w 6 3", c2.AsMove().DebugString(), "Expected w 6 3, got " + c2.AsMove().DebugString());

            Assert.IsTrue(arrayEqual(new int[] { 1 }, c3.AsDiceState().GetDiceValues()));

            Assert.AreEqual("w 6 5", c4.AsMove().DebugString());

            Assert.IsTrue(arrayEqual(ar(4,5), c5.AsDiceState().GetDiceValues()));


        }

        [TestMethod]
        public void TestChangesRegisteredProperlyAfterTwoMovesBlack()
        { 
            List<int[]> moves = new List<int[]> { ar(1, 3), ar(4, 5) };
            fd = new FakeDice(moves);
            bg = new BackgammonGame(BackgammonGame.DefaultGameBoard, fd, 0, 0, 0, 0, Black);

            bg.Move(Black, 19, 22);
            bg.Move(Black, 19, 20);

            var changes = bg.GetChanges();
            Assert.AreEqual(5, changes.Count());

            var c1 = changes[0];
            var c2 = changes[1];
            var c3 = changes[2];
            var c4 = changes[3];
            var c5 = changes[4];

            Assert.IsTrue(c1.IsDiceState(), "c1 is not dice state");
            Assert.IsTrue(c2.IsMove(), "c2 is not move");
            Assert.IsTrue(c3.IsDiceState(), "c3 is not dice state");
            Assert.IsTrue(c4.IsMove(), "c4 is not move");
            Assert.IsTrue(c5.IsDiceState(), "c5 is not dice state");



            Assert.IsTrue(arrayEqual(ar(1, 3), c1.AsDiceState().GetDiceValues()), "Expected 1,3 but got " + string.Join(",", c1.AsDiceState().GetDiceValues()));

            Assert.AreEqual("b 19 22", c2.AsMove().DebugString(), "Expected w 6 3, got " + c2.AsMove().DebugString());

            Assert.IsTrue(arrayEqual(new int[] { 1 }, c3.AsDiceState().GetDiceValues()));

            Assert.AreEqual("b 19 20", c4.AsMove().DebugString());

            Assert.IsTrue(arrayEqual(ar(4, 5), c5.AsDiceState().GetDiceValues()));
        }

        [TestMethod]
        public void TestChangesRegisteredProperlyAfterBothWhiteAndBlackMoves()
        {
            List<int[]> moves = new List<int[]> { ar(1, 3), ar(4, 5), ar(6,3) };
            fd = new FakeDice(moves);
            bg = new BackgammonGame(BackgammonGame.DefaultGameBoard, fd);

            bg.Move(White, 6, 3);
            bg.Move(White, 6, 5);
            bg.Move(Black, 19, 23);
            bg.Move(Black, 17, 22);

            var changes = bg.GetChanges();
            Assert.AreEqual(9, changes.Count());

            var c1 = changes[0];
            var c2 = changes[1];
            var c3 = changes[2];
            var c4 = changes[3];
            var c5 = changes[4];
            var c6 = changes[5];
            var c7 = changes[6];
            var c8 = changes[7];
            var c9 = changes[8];

            Assert.IsTrue(c1.IsDiceState(), "c1 is not dice state");
            Assert.IsTrue(c2.IsMove(), "c2 is not move");
            Assert.IsTrue(c3.IsDiceState(), "c3 is not dice state");
            Assert.IsTrue(c4.IsMove(), "c4 is not move");
            Assert.IsTrue(c5.IsDiceState(), "c5 is not dice state");
            Assert.IsTrue(c6.IsMove());
            Assert.IsTrue(c7.IsDiceState());
            Assert.IsTrue(c8.IsMove());
            Assert.IsTrue(c9.IsDiceState());



            Assert.IsTrue(arrayEqual(ar(1, 3), c1.AsDiceState().GetDiceValues()), "Expected 1,3 but got " + string.Join(",", c1.AsDiceState().GetDiceValues()));

            Assert.AreEqual("w 6 3", c2.AsMove().DebugString(), "Expected w 6 3, got " + c2.AsMove().DebugString());

            Assert.IsTrue(arrayEqual(new int[] { 1 }, c3.AsDiceState().GetDiceValues()));

            Assert.AreEqual("w 6 5", c4.AsMove().DebugString());

            Assert.IsTrue(arrayEqual(ar(4, 5), c5.AsDiceState().GetDiceValues()));

            Assert.AreEqual("b 19 23", c6.AsMove().DebugString());

            Assert.IsTrue(arrayEqual(new int[] { 5 }, c7.AsDiceState().GetDiceValues() ));

            Assert.AreEqual("b 17 22", c8.AsMove().DebugString());

            Assert.IsTrue(arrayEqual(ar(6, 3), c9.AsDiceState().GetDiceValues()));

        }

        [TestMethod]
        public void TestChangesRegisteredProperlyAfterRolledTwoEqualDiceAndAllMovesConsumed()
        {
            List<int[]> moves = new List<int[]> { ar(2,2), ar(1, 5) };
            fd = new FakeDice(moves);
            bg = new BackgammonGame(BackgammonGame.DefaultGameBoard, fd);

            bg.Move(White, 6, 4);
            bg.Move(White, 6, 4);
            bg.Move(White, 13, 11);
            bg.Move(White, 24, 22);

            var changes = bg.GetChanges();
            Assert.AreEqual(9, changes.Count());

            var c1 = changes[0];
            var c2 = changes[1];
            var c3 = changes[2];
            var c4 = changes[3];
            var c5 = changes[4];
            var c6 = changes[5];
            var c7 = changes[6];
            var c8 = changes[7];
            var c9 = changes[8];

            Assert.IsTrue(c1.IsDiceState(), "c1 is not dice state");
            Assert.IsTrue(c2.IsMove(), "c2 is not move");
            Assert.IsTrue(c3.IsDiceState(), "c3 is not dice state");
            Assert.IsTrue(c4.IsMove(), "c4 is not move");
            Assert.IsTrue(c5.IsDiceState(), "c5 is not dice state");
            Assert.IsTrue(c6.IsMove());
            Assert.IsTrue(c7.IsDiceState());
            Assert.IsTrue(c8.IsMove());
            Assert.IsTrue(c9.IsDiceState());



            Assert.IsTrue(arrayEqual(new int[] { 2, 2, 2, 2 }, c1.AsDiceState().GetDiceValues()), "Expected 1,3 but got " + string.Join(",", c1.AsDiceState().GetDiceValues()));

            Assert.AreEqual("w 6 4", c2.AsMove().DebugString());

            Assert.IsTrue(arrayEqual(new int[] { 2,2,2 }, c3.AsDiceState().GetDiceValues()));

            Assert.AreEqual("w 6 4", c4.AsMove().DebugString());

            Assert.IsTrue(arrayEqual(ar(2,2), c5.AsDiceState().GetDiceValues()));

            Assert.AreEqual("w 13 11", c6.AsMove().DebugString());

            Assert.IsTrue(arrayEqual(new int[] { 2 }, c7.AsDiceState().GetDiceValues()));

            Assert.AreEqual("w 24 22", c8.AsMove().DebugString());

            Assert.IsTrue(arrayEqual(ar(1, 5), c9.AsDiceState().GetDiceValues()));
        }

        [TestMethod]
        public void TestChangesRegisteredProperlyAfterNotifyViewFlushesChanges()
        {
            List<int[]> moves = new List<int[]>() { ar(1, 3), ar(4, 4) };
            fd = new FakeDice(moves);
            bg = new BackgammonGame(BackgammonGame.DefaultGameBoard, fd);
            bg.Move(White, 6, 3);

            var changes = bg.GetChanges();
            Assert.AreEqual(3, changes.Count());

            var c1 = changes[0];
            var c2 = changes[1];
            var c3 = changes[2];

            Assert.IsTrue(c1.IsDiceState(), "c1 is not dice state");
            Assert.IsTrue(c2.IsMove(), "c2 is not move");
            Assert.IsTrue(c3.IsDiceState(), "c3 is not dice state");


            Assert.IsTrue(arrayEqual(ar(1, 3), c1.AsDiceState().GetDiceValues()), "Expected 1,3 but got " + string.Join(",", c1.AsDiceState().GetDiceValues()));
            Assert.AreEqual("w 6 3", c2.AsMove().DebugString(), "Expected w 6 3, got " + c2.AsMove().DebugString());
            Assert.IsTrue(arrayEqual(new int[] { 1 }, c3.AsDiceState().GetDiceValues()));

            bg.NotifyViews();

            changes = bg.GetChanges();
            Assert.AreEqual(0, changes.Count());

            bg.Move(White, 6, 5);

            changes = bg.GetChanges();
            Assert.AreEqual(2, changes.Count());
            var c4 = changes[0];
            var c5 = changes[1];
            Assert.IsTrue(c4.IsMove());
            Assert.IsTrue(c5.IsDiceState());
            Assert.AreEqual("w 6 5", c4.AsMove().DebugString());
            Assert.IsTrue(arrayEqual(new int[] {4,4,4,4}, c5.AsDiceState().GetDiceValues()));
        }


        [TestMethod]
        public void TestChangesRegisteredProperlyCompundMovesOfMultipleSmallerMoves()
        {

            List<int[]> moves = new List<int[]>() { ar(1, 3), ar(4, 4) };
            fd = new FakeDice(moves);
            bg = new BackgammonGame(BackgammonGame.DefaultGameBoard, fd);
            bg.Move(White, 13, 9);

            var changes = bg.GetChanges();
            Assert.AreEqual(5, changes.Count());

            var c1 = changes[0];
            var c2 = changes[1];
            var c3 = changes[2];
            var c4 = changes[3];
            var c5 = changes[4];

            Assert.IsTrue(c1.IsDiceState(), "c1 is not dice state");
            Assert.IsTrue(c2.IsMove(), "c2 is not move");
            Assert.IsTrue(c3.IsDiceState(), "c3 is not dice state");


            Assert.IsTrue(arrayEqual(ar(1, 3), c1.AsDiceState().GetDiceValues()), "Expected 1,3 but got " + string.Join(",", c1.AsDiceState().GetDiceValues()));
            Assert.AreEqual("w 13 10", c2.AsMove().DebugString());
            Assert.IsTrue(arrayEqual(new int[] { 1 }, c3.AsDiceState().GetDiceValues()));
            Assert.AreEqual("w 10 9", c4.AsMove().DebugString());
            Assert.IsTrue(arrayEqual(new int[] { 4, 4, 4, 4 }, c5.AsDiceState().GetDiceValues()));
        }

        [TestMethod]
        public void TestThatIfBlackCheckerIsCapturedAChangeShowsItIsMovedToTheBar()
        {
            int[] mainBoard = new int[] {
                                           -1, 0, -1, 0,  0,  5,
                                            0, 3, 0, 0,  0, -5,
                                            5, 0, 0, 0, -3,  0,
                                           -5, 0, 0, 0,  0,  2 };
            fd.SetReturnValues(ar(1, 3));
            bg = new BackgammonGame(mainBoard, fd);
            bg.Move(White, 6, 3);

            var changes = bg.GetChanges();
            Assert.AreEqual(4, changes.Count());

            var c1 = changes[0];
            var c2 = changes[1];
            var c3 = changes[2];
            var c4 = changes[3];

            Assert.IsTrue(c1.IsDiceState(), "c1 is not dice state");
            Assert.IsTrue(c2.IsMove(), "c2 is not move");
            Assert.IsTrue(c3.IsMove());
            Assert.IsTrue(c4.IsDiceState(), "c3 is not dice state");


            Assert.IsTrue(arrayEqual(ar(1, 3), c1.AsDiceState().GetDiceValues()), "Expected 1,3 but got " + string.Join(",", c1.AsDiceState().GetDiceValues()));
            Assert.AreEqual("w 6 3", c2.AsMove().DebugString());
            Assert.AreEqual("b 3 " + Black.GetBar(), c3.AsMove().DebugString());
            Assert.IsTrue(arrayEqual(new int[] { 1 }, c4.AsDiceState().GetDiceValues()));


        }

        [TestMethod]
        public void TestThatIfWhiteCheckerIsCapturedAChangeShowsItIsMovedToTheBar()
        {
            int[] mainBoard = new int[] {
                                           -2, 0, 0, 0,  0,  5,
                                            0, 3, 0, 0,  0, -5,
                                            5, 0, 0, 0, -3,  0,
                                           -5, 0, 0, 1,  0,  1 };
            fd.SetReturnValues(ar(1, 3));
            bg = new BackgammonGame(mainBoard, fd, 0, 0, 0, 0, Black);
            bg.Move(Black, 19, 22);

            var changes = bg.GetChanges();
            Assert.AreEqual(4, changes.Count());

            var c1 = changes[0];
            var c2 = changes[1];
            var c3 = changes[2];
            var c4 = changes[3];

            Assert.IsTrue(c1.IsDiceState(), "c1 is not dice state");
            Assert.IsTrue(c2.IsMove(), "c2 is not move");
            Assert.IsTrue(c3.IsMove());
            Assert.IsTrue(c4.IsDiceState(), "c3 is not dice state");


            Assert.IsTrue(arrayEqual(ar(1, 3), c1.AsDiceState().GetDiceValues()), "Expected 1,3 but got " + string.Join(",", c1.AsDiceState().GetDiceValues()));
            Assert.AreEqual("b 19 22", c2.AsMove().DebugString());
            Assert.AreEqual("w 22 " + White.GetBar(), c3.AsMove().DebugString());
            Assert.IsTrue(arrayEqual(new int[] { 1 }, c4.AsDiceState().GetDiceValues()));
        }



        //Below here is obsolete

        [TestMethod]
        public void TestGetMoveHistoryWorksCorrectlyOneTurnOnly()
        {
            return;
            bg.Move(White, 6, 4);
            bg.Move(White, 6, 5);

            Assert.AreEqual(Black, bg.playerToMove());

            List<Turn> history = bg.GetTurnHistory();
            Assert.AreEqual(1, history.Count());
            Turn t0 = history.ElementAt(0);

            var dice = t0.dice;
            Assert.IsTrue(Enumerable.SequenceEqual(dice, new int[] { 1, 2 }));

            List<Move> moves = t0.moves;

            Assert.AreEqual(2, moves.Count());
            Assert.AreEqual("w 6 4", moves[0].DebugString());
            Assert.AreEqual("w 6 5", moves[1].DebugString());

        }

        [TestMethod]
        public void TestGetMoveHistoryWorksCorrectlyTwoTurns()
        {
            bg.Move(White, 6, 4);
            bg.Move(White, 6, 5);
            bg.Move(Black, 1, 4);
            bg.Move(Black, 1, 5);

            Assert.AreEqual(White, bg.playerToMove());
            List<Turn> history = bg.GetTurnHistory();
            Assert.AreEqual(2, history.Count());

            //Testing turn one
            Turn t0 = history[0];
            var dice = t0.dice;
            Assert.IsTrue(Enumerable.SequenceEqual(dice, new int[] { 1, 2 }));
            List<Move> moves = t0.moves;
            Assert.AreEqual(2, moves.Count());
            Assert.AreEqual("w 6 4", moves[0].DebugString());
            Assert.AreEqual("w 6 5", moves[1].DebugString());

            //Testing turn 2
            Turn t1 = history[1];
            var dice1 = t1.dice;
            Console.WriteLine("dice1: " + string.Join(",", dice1));
            Assert.IsTrue(Enumerable.SequenceEqual(dice1, new int[] { 4, 3 }), "expected dice1 to be 4,3, but was " + string.Join(",", dice1));
            List<Move> moves1 = t1.moves;
            Assert.AreEqual(2, moves.Count());
            Assert.AreEqual("b 1 4", moves1[0].DebugString());
            Assert.AreEqual("b 1 5", moves1[1].DebugString());

        }

        [TestMethod]
        public void TestGetMoveHistoryTwoTurnsWhenGetHistoryInBetween()
        {
            bg.Move(White, 6, 4);
            bg.Move(White, 6, 5);
            var history1 = bg.GetTurnHistory();
            Assert.AreEqual(1, history1.Count());

            //Testing turn one
            Turn t0 = history1[0];
            var dice = t0.dice;
            Assert.IsTrue(Enumerable.SequenceEqual(dice, new int[] { 1, 2 }));
            List<Move> moves = t0.moves;
            Assert.AreEqual(2, moves.Count());
            Assert.AreEqual("w 6 4", moves[0].DebugString());
            Assert.AreEqual("w 6 5", moves[1].DebugString());


            bg.Move(Black, 1, 4);
            bg.Move(Black, 1, 5);
            var history2 = bg.GetTurnHistory();
            Assert.AreEqual(1, history2.Count());
            
            //Testing turn 2
            Turn t1 = history2[0];
            var dice1 = t1.dice;
            Console.WriteLine("dice1: " + string.Join(",", dice1));
            Assert.IsTrue(Enumerable.SequenceEqual(dice1, new int[] { 4, 3 }), "expected dice1 to be 4,3, but was " + string.Join(",", dice1));
            List<Move> moves1 = t1.moves;
            Assert.AreEqual(2, moves.Count());
            Assert.AreEqual("b 1 4", moves1[0].DebugString());
            Assert.AreEqual("b 1 5", moves1[1].DebugString());
        }

        [TestMethod]
        public void TestGetHistoryThreeTurnsWithTwoEqualDiceThereforeFourMoves()
        {
            bg.Move(White, 6, 4);
            bg.Move(White, 6, 5);
            bg.Move(Black, 1, 4);
            bg.Move(Black, 1, 5);
            bg.Move(White, White.GetBar(), 24);
            bg.Move(White, White.GetBar(), 24);
            bg.Move(White, 6, 5);
            bg.Move(White, 5, 4);

            var history = bg.GetTurnHistory();
            //Assert.AreEqual(3, history.Count());

            //Testing turn one
            Turn t0 = history[0];
            var dice = t0.dice;
            Assert.IsTrue(Enumerable.SequenceEqual(dice, new int[] { 1, 2 }));
            List<Move> moves = t0.moves;
            Assert.AreEqual(2, moves.Count());
            Assert.AreEqual("w 6 4", moves[0].DebugString());
            Assert.AreEqual("w 6 5", moves[1].DebugString());

            //Testing turn 2
            Turn t1 = history[1];
            var dice1 = t1.dice;
            Console.WriteLine("dice1: " + string.Join(",", dice1));
            Assert.IsTrue(Enumerable.SequenceEqual(dice1, new int[] { 4, 3 }), "expected dice1 to be 4,3, but was " + string.Join(",", dice1));
            List<Move> moves1 = t1.moves;
            Assert.AreEqual(2, moves.Count());
            Assert.AreEqual("b 1 4", moves1[0].DebugString());
            Assert.AreEqual("b 1 5", moves1[1].DebugString());

            //Testing turn 3
            Turn t2 = history[2];
            var dice2 = t2.dice;
            Assert.IsTrue(Enumerable.SequenceEqual(dice2, new int[] { 1, 1, 1,1 }), "expected dice2 to be 1,1, but was " + string.Join(",", dice2));
            List<Move> moves2 = t2.moves;
            Assert.AreEqual(4, moves2.Count());
            Assert.AreEqual("w " + White.GetBar() + " 24", moves2[0].DebugString());
            Assert.AreEqual("w " + White.GetBar() + " 24", moves2[1].DebugString());
            Assert.AreEqual("w 6 5", moves2[2].DebugString());
            Assert.AreEqual("w 5 4", moves2[3].DebugString());
        }

        [TestMethod]
        public void TestTurnHistoryWhenCanOnlyUseOneOfTwoMoves()
        {
            int[] board = new int[]
            {
                -2,-2,-2,-2,-2,-2,
                14, 1, 0, 0, 0, 0,
                -3, 0, 0, 0, 0, 0,
                 0, 0, 0, 0, 0, 0
            };

            var diceValues = new List<int[]>();
            diceValues.Add(ar(6, 1));
            diceValues.Add(ar(4, 3));
            diceValues.Add(ar(1, 1));

            fd = new FakeDice(diceValues);
            bg = new BackgammonGame(board, fd);

            bg.Move(White, 8, 7);
            bg.Move(Black, 6, 9);
            bg.Move(Black, 6, 10);

            var history = bg.GetTurnHistory();
            Assert.AreEqual(2, history.Count());

            var t1 = history[0];
            Assert.IsTrue(Enumerable.SequenceEqual(t1.dice, new int[] { 6, 1 }));
            Assert.AreEqual(1, t1.moves.Count());
            Assert.AreEqual("w 8 7", t1.moves[0].DebugString());

            var t2 = history[1];
            Assert.IsTrue(Enumerable.SequenceEqual(t2.dice, new int[] { 4, 3 }));
            Assert.AreEqual(2, t2.moves.Count());
            Assert.AreEqual("b 6 9", t2.moves[0].DebugString());
            Assert.AreEqual("b 6 10", t2.moves[1].DebugString());
        }


        [TestMethod]
        public void TestTurnHistoryWhenNoMovesAreLegal()
        {
            int[] board = new int[]
            {
                0,0,0,0,0,0,
                0,0,0,0,0,0,
                0,0,0,0,0,-15,
                2,2,2,2,2,5
            };

            List<int[]> diceValues = new List<int[]>() { ar(3, 4), ar(5, 1) };
            fd = new FakeDice(diceValues);
            bg = new BackgammonGame(board, fd);

            bg.Move(White, 24, 21);
            bg.Move(White, 24, 20);

            var history = bg.GetTurnHistory();
            Assert.AreEqual(2, history.Count());

            var t1 = history[0];
            Assert.IsTrue(Enumerable.SequenceEqual(t1.dice, new int[] { 3, 4 }));
            Assert.AreEqual("w 24 21", t1.moves[0].DebugString());
            Assert.AreEqual("w 24 20", t1.moves[1].DebugString());


            //Checks that black players turn ended and was registered even though he 
            //did not make a move
            var t2 = history[1];
            Assert.IsTrue(Enumerable.SequenceEqual(t2.dice, new int[] { 5, 1 }));
            Assert.AreEqual(0, t2.moves.Count());

        }

        [TestMethod]
        public void TestTurnHistoryWhenCompositeMovesArePerfomed()
        {
            List<int[]> diceValues = new List<int[]>() { ar(5, 1), ar(4, 1) };
            fd = new FakeDice(diceValues);
            bg = new BackgammonGame(BackgammonGame.DefaultGameBoard, fd);

            bg.Move(White, 24, 18);
            bg.Move(Black, 12, 17);

            var history = bg.GetTurnHistory();
            Assert.AreEqual(2, history.Count());

            var t1 = history[0];
            Assert.IsTrue(Enumerable.SequenceEqual(t1.dice, new int[] { 5, 1 }));
            var moves1 = t1.moves;
            Assert.AreEqual(2, moves1.Count());
            Assert.AreEqual("w 24 23", moves1[0].DebugString());
            Assert.AreEqual("w 23 18", moves1[1].DebugString());

            var t2 = history[1];
            Assert.IsTrue(Enumerable.SequenceEqual(t2.dice, new int[] { 4, 1 }));
            var moves2 = t2.moves;
            Assert.AreEqual(2, moves2.Count());
            Assert.AreEqual("b 12 16", moves2[0].DebugString());
            Assert.AreEqual("b 16 17", moves2[1].DebugString());
        }

        [TestMethod]
        public void TestTurnHistoryWhenBlackStarts()
        {
            List<int[]> diceValues = new List<int[]>() { ar(5, 1), ar(4, 1) };
            fd = new FakeDice(diceValues);
            bg = new BackgammonGame(BackgammonGame.DefaultGameBoard, fd, 0, 0, 0, 0, Black);

            bg.Move(Black, 1, 2);
            bg.Move(Black, 12, 17);

            bg.Move(White, 6, 2);
            bg.Move(White, 6, 5);

            var history = bg.GetTurnHistory();
            Assert.AreEqual(2, history.Count());

            var t1 = history[0];
            Assert.IsTrue(Enumerable.SequenceEqual(t1.dice, new int[] { 5, 1 }));
            var moves1 = t1.moves;
            Assert.AreEqual(2, moves1.Count());
            Assert.AreEqual("b 1 2", moves1[0].DebugString());
            Assert.AreEqual("b 12 17", moves1[1].DebugString());

            var t2 = history[1];
            Assert.IsTrue(Enumerable.SequenceEqual(t2.dice, new int[] { 4, 1 }));
            var moves2 = t2.moves;
            Assert.AreEqual(2, moves2.Count());
            Assert.AreEqual("w 6 2", moves2[0].DebugString());
            Assert.AreEqual("w 6 5", moves2[1].DebugString());


        }

        //Test move history when cant use all moves
        //Test move hisotyr when turn changes, and no legal moves are possible, and turn changes again
        //Test move history when moves of multiple moves are done
    }
}
