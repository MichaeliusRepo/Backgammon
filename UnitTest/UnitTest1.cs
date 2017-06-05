using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Json;
using ModelDLL;
using System.Text;
using System.Linq;

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

            /*var exp = expected.ToCharArray();
            var act = xml.ToCharArray();

            Console.WriteLine(expected);
            Console.WriteLine(xml);

            for(int i = 0; i < expected.Length; i++)
            {
                if(exp[i] != act[i])
                {
                    Console.WriteLine("First error on position " + i + ". Expected " + exp[i] + " but got " + act[i]);
                    Assert.IsTrue(false);
                }
            }*/
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
        public void GameBoardFromXmlStrings()
        {

        }
    }
}
