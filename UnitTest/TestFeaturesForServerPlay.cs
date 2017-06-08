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

            string expected = "<gameboard><board>-2 0 0 0 0 5 0 3 0 0 0 -5 5 0 0 0 -3 0 -5 0 0 0 0 2</board>"+
                              "<whiteGoal>0</whiteGoal><whiteBar>0</whiteBar><blackGoal>0</blackGoal><blackBar>0</blackBar></gameboard>";
            string xml = UpdateCreatorParser.CreateXmlForGameBoardState(state, "gameboard");
            GameBoardState parsedState = UpdateCreatorParser.ParseGameBoardState(xml);

            Assert.AreEqual(expected, xml);
        }

        [TestMethod]
        public void XmlifyGameBoardState2()
        {
            int[] mainBoard = new int[] {-2,0,0,0,0,3,0,3,0,0,0,-5,5,0,0,0,-1,0,-5,0,0,0,0,2};
            GameBoardState state = new GameBoardState(mainBoard, 1, 1, 1, 1);

            string expected = "<gameboard><board>-2 0 0 0 0 3 0 3 0 0 0 -5 5 0 0 0 -1 0 -5 0 0 0 0 2</board>" +
                              "<whiteGoal>1</whiteGoal><whiteBar>1</whiteBar><blackGoal>-1</blackGoal><blackBar>-1</blackBar></gameboard>";
            string xml = UpdateCreatorParser.CreateXmlForGameBoardState(state, "gameboard");
            GameBoardState parsedState = UpdateCreatorParser.ParseGameBoardState(xml);

            Assert.AreEqual(expected, xml);
            Assert.AreEqual(state, parsedState);
        }

        [TestMethod]
        public void XmlifyMoveAndParseBackFromString()
        {
            Move move1 = new Move(White, 6, 3);
            string move1Xml = UpdateCreatorParser.CreateXmlForMove(move1);
            Move parsedMove1 = UpdateCreatorParser.ParseMove(move1Xml);

            Assert.AreEqual("<move>w 6 3</move>", move1Xml);
            Assert.AreEqual(move1, parsedMove1);
           
            Move move2 = new Move(White, 6, White.GetBar());
            string move2Xml = UpdateCreatorParser.CreateXmlForMove(move2);
            Move parsedMove2 = UpdateCreatorParser.ParseMove(move2Xml);

            Assert.AreEqual("<move>w 6 25</move>", move2Xml);
            Assert.AreEqual(move2, parsedMove2, "this one failing");

            Move move3 = new Move(White, 6, White.BearOffPositionID());
            string move3Xml = UpdateCreatorParser.CreateXmlForMove(move3);
            Move parsedMove3 = UpdateCreatorParser.ParseMove(move3Xml);

            Assert.AreEqual("<move>w 6 28</move>", move3Xml);
            Assert.AreEqual(move3, parsedMove3);
            

            Move move4 = new Move(Black, 20, 24);
            string move4Xml = UpdateCreatorParser.CreateXmlForMove(move4);
            Move parsedMove4 = UpdateCreatorParser.ParseMove(move4Xml);

            Assert.AreEqual("<move>b 20 24</move>", move4Xml);
            Assert.AreEqual(move4, parsedMove4);

            Move move5 = new Move(Black, 24, Black.BearOffPositionID());
            string move5Xml = UpdateCreatorParser.CreateXmlForMove(move5);
            Move parsedMove5 = UpdateCreatorParser.ParseMove(move5Xml);

            Assert.AreEqual("<move>b 24 27</move>", move5Xml);
            Assert.AreEqual(move5, parsedMove5);
            

            Move move6 = new Move(Black, 20, Black.GetBar());
            string move6Xml = UpdateCreatorParser.CreateXmlForMove(move6);
            Move parsedMove6 = UpdateCreatorParser.ParseMove(move6Xml);

            Assert.AreEqual("<move>b 20 26</move>", move6Xml);
            Assert.AreEqual(move6, parsedMove6);
        }

        [TestMethod]
        public void TestCanGenerateAndParseListOfMoves()
        {
            Move move1 = new Move(White, 5, 2);
            Move move2 = new Move(Black, 20, Black.GetBar());
            Move move3 = new Move(White, 1, White.BearOffPositionID());

            List<Move> moves = new List<Move>() { move1, move2, move3 };

            string xml = UpdateCreatorParser.GenerateXmlForListOfMoves(moves);
            string expected = "<moves><move>w 5 2</move><move>b 20 26</move><move>w 1 28</move></moves>";
            Assert.AreEqual(expected, xml);

            List<Move> parsedMoves = UpdateCreatorParser.ParseListOfMoves(xml);

            Assert.IsTrue(parsedMoves.Contains(move1));
            Assert.IsTrue(parsedMoves.Contains(move2));
            Assert.IsTrue(parsedMoves.Contains(move3));
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

        [TestMethod]
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

        [TestMethod]
        public void TestGetPreviousTurnWhenOnlySomeMovesCouldBeUsed()
        {
            //White will roll 1 and 3, so he will only be able to move the checker on position 24 to position 21. 
            //There will be no other legal moves, so the turn changes. We check that the previous turn mirrors this
            int[] board = new int[]
            {
                14,0,0,0,0,0,
                0,0,0,0,0,0,
                0,0,0,-3,-2,-2,
                -2,-2,0,-2,-2,1
            };

            fd.SetReturnValues(ar(1, 3));
            bg = new BackgammonGame(board, fd);
            bg.Move(White, 24, 21);


            var turn = bg.GetPreviousTurn();
            Assert.AreEqual(White, turn.color);
            Assert.IsTrue(arrayEquals(ar(1,3), turn.dice.ToArray()));

            var moves = turn.moves;
            Assert.AreEqual(1, moves.Count());
            Assert.AreEqual("w 24 21", moves[0].DebugString());
        }

        [TestMethod]
        public void TestGetPreviousTurnWhenNoLegalMoves()
        {
            //When it becomes black's turn, there will be no legal moves, but the turn should change to white
            //before bg.EndTurn(Black) is called
            int[] board = new int[]
            {
                -15,2,2,2,2,2,
                2,3,0,0,0,0,
                0,0,0,0,0,0,
                0,0,0,0,0,0
            };

            List<int[]> moves = new List<int[]>() { ar(1, 3), ar(5, 3) };
            fd = new FakeDice(moves);

            bg = new BackgammonGame(board, fd);
            bg.Move(White, 8, 5);
            bg.Move(White, 8, 7);

            var turn = bg.GetPreviousTurn();
            Assert.AreEqual(White, turn.color);
            Assert.IsTrue(arrayEquals(ar(1, 3), turn.dice.ToArray()));
            Assert.AreEqual(2, turn.moves.Count());

            bg.EndTurn(Black);
            turn = bg.GetPreviousTurn();
            Assert.AreEqual(Black, turn.color);
            Assert.IsTrue(arrayEquals(ar(5, 3), turn.dice.ToArray()));
            Assert.AreEqual(0, turn.moves.Count());
        }
    }
}
