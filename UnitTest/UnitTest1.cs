using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Json;
using ModelDLL;
using System.Text;
using System.Linq;
using static ModelDLL.CheckerColor;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
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
    }
}
