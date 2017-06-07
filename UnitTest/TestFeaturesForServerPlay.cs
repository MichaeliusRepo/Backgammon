using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Json;
using ModelDLL;
using System.Text;
using System.Linq;
using static ModelDLL.CheckerColor;
using System.Collections.Generic;

namespace UnitTest
{
    [TestClass]
    public class TestFeaturesForServerPlay
    {

        private BackgammonGame bg;
        private FakeDice fd;

        [TestInitialize]
        public void Initialize()
        {
            List<int[]> moves = new List<int[]>() {ar(1,3), ar(5,3), ar(6,1), ar(1,2) };
            fd = new FakeDice(moves);
            bg = new BackgammonGame(BackgammonGame.DefaultGameBoard, fd);
        }

        private int[] ar(params int[] values)
        {
            return values;
        }

        private bool arrayEquals(int[] a, int[] b)
        {
            return Enumerable.SequenceEqual(a, b);
        }

        [TestMethod]
        public void XmlifyGameBoardState()
        {
            GameBoardState state = new GameBoardState(BackgammonGame.DefaultGameBoard, 0, 0, 0, 0);

            string expected = "<board>-2 0 0 0 0 5 0 3 0 0 0 -5 5 0 0 0 -3 0 -5 0 0 0 0 2</board>"+
                              "<whiteHome>0</whiteHome><whiteBar>0</whiteBar><blackHome>0</blackHome><blackBar>0</blackBar>";
            string xml = state.Xmlify();
            Assert.AreEqual(expected, xml);
        }

        [TestMethod]
        public void XmlifyGameBoardState2()
        {
            int[] mainBoard = new int[] {-2,0,0,0,0,3,0,3,0,0,0,-5,5,0,0,0,-1,0,-5,0,0,0,0,2};
            GameBoardState state = new GameBoardState(mainBoard, 1, 1, 1, 1);

            string expected = "<board>-2 0 0 0 0 3 0 3 0 0 0 -5 5 0 0 0 -1 0 -5 0 0 0 0 2</board>" +
                              "<whiteHome>1</whiteHome><whiteBar>1</whiteBar><blackHome>-1</blackHome><blackBar>-1</blackBar>";
            string xml = state.Xmlify();
            Assert.AreEqual(expected, xml);
        }

        [TestMethod]
        public void XmlifyMove()
        {
            Move move1 = new Move(White, 6, 3);
            Assert.AreEqual("<move>w 6 3</move>", move1.Xmlify());

            Move move2 = new Move(White, 6, White.GetBar());
            Assert.AreEqual("<move>w 6 25</move>", move2.Xmlify());

            Move move3 = new Move(White, 6, White.BearOffPositionID());
            Assert.AreEqual("<move>w 6 0</move>", move3.Xmlify());

            Move move4 = new Move(Black, 20, 24);
            Assert.AreEqual("<move>b 20 24</move>", move4.Xmlify());

            Move move5 = new Move(Black, 24, Black.BearOffPositionID());
            Assert.AreEqual("<move>b 24 25</move>", move5.Xmlify());

            Move move6 = new Move(Black, 20, Black.GetBar());
            Assert.AreEqual("<move>b 20 0</move>", move6.Xmlify());
        }

        [TestMethod]
        public void TestGetBackgammonPreviousTurn()
        {
            //Available moves {1,3}, see fake dice in initialzie
            bg.Move(White, 6, 3);
            bg.Move(White, 6, 5);

            var turn1 = bg.GetPreviousTurn();

            Assert.AreEqual(White, turn1.color);
            Assert.IsTrue(arrayEquals(ar(1, 3), turn1.dice.ToArray()));

            var moves1 = turn1.moves;
            Assert.AreEqual(2, moves1.Count());
            Assert.AreEqual("w 6 3", moves1[0].DebugString());
            Assert.AreEqual("w 6 5", moves1[1].DebugString());


            //Available moves {5,3}, see fake dice in initialzie
            bg.Move(Black, 1, 4);
            bg.Move(Black, 17, 22);

            var turn2 = bg.GetPreviousTurn();

            Assert.AreEqual(Black, turn2.color);
            Assert.IsTrue(arrayEquals(ar(5, 3), turn2.dice.ToArray()));

            var moves2 = turn2.moves;
            Assert.AreEqual(2, moves2.Count());
            Assert.AreEqual("b 1 4", moves2[0].DebugString());
            Assert.AreEqual("b 17 22", moves2[1].DebugString());

        }

        public void TestGetBackgammonPreviousTurnWithDoubleRoll()
        {
            fd = new FakeDice(ar(2, 2));
            bg = new BackgammonGame(BackgammonGame.DefaultGameBoard, fd);

            bg.Move(White, 6, 4);
            bg.Move(White, 6, 4);
            bg.Move(White, 6, 4);
            bg.Move(White, 6, 4);

            var turn = bg.GetPreviousTurn();

            Assert.AreEqual(White, turn.color);
            Assert.IsTrue(arrayEquals(ar(2, 2, 2, 2), turn.dice.ToArray()));

            var moves = turn.moves;
            Assert.AreEqual(4, moves.Count());
            Assert.AreEqual("w 6 4", moves[0].DebugString());
            Assert.AreEqual("w 6 4", moves[1].DebugString());
            Assert.AreEqual("w 6 4", moves[2].DebugString());
            Assert.AreEqual("w 6 4", moves[3].DebugString());


        }

        public void TestGetPreviousTurnWhenOnlySomeMovesCouldBeUsed()
        {
            //White will roll 1 and 3, so he will only be able to move the checker on position 24 to position 21. 
            //There will be no other legal moves, so the turn changes. We check that the previous turn mirrors this
            int[] board = new int[]
            {
                14,0,0,0,0,0,
                0,0,0,0,0,0,
                0,0,0,-3,-2,-2,
                -2,-2,0-2,-2,1
            };

            bg = new BackgammonGame(board, fd);
            bg.Move(White, 24, 21);


            var turn = bg.GetPreviousTurn();
            Assert.AreEqual(White, turn.color);
            Assert.IsTrue(arrayEquals(ar(1,3), turn.dice.ToArray()));

            var moves = turn.moves;
            Assert.AreEqual(1, moves.Count());
            Assert.AreEqual("w 24 21", moves[0].DebugString());
        }

        public void TestGetPreviousTurnWhenNoLegalMoves()
        {
            //When it becomes black's turn, there will be no legal moves, and the turn should skip
            int[] board = new int[]
            {
                -15,2,2,2,2,2,
                2,3,0,0,0,0,
                0,0,0,0,0,0,
                0,0,0,0,0,0
            };

            bg = new BackgammonGame(board, fd);
            bg.Move(White, 8, 5);
            bg.Move(White, 8, 7);

            var turn = bg.GetPreviousTurn();
            Assert.AreEqual(Black, turn.color);
            Assert.IsTrue(arrayEquals(ar(5, 3), turn.dice.ToArray()));
            Assert.AreEqual(0, turn.moves.Count());
        }
    }
}
