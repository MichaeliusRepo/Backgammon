using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelDLL;
using System.Xml;

namespace UnitTest
{
    [TestClass]
    public class TestingServerCommunication
    {
        [TestMethod]
        public void TestGameBoardStateXMLIfyWorksWithNoCheckersOnTargetOrOnBar()
        {
            GameBoardState gbs = new GameBoardState(BackgammonGame.DefaultGameBoard, 0,0,0,0);
            string xml = gbs.GetXML();
            string expected = "<board>-2 0 0 0 0 5 0 3 0 0 0 -5 5 0 0 0 -3 0 -5 0 0 0 0 2</board>" +
                              "<whiteBar>0</whiteBar><whiteHome>0</whiteHome><blackBar>0</blackBar><blackHome>0</blackHome>";
            Assert.AreEqual(expected, xml);
        }

        [TestMethod]
        public void TestGameBoardStateXMLIfyWorksWithCheckerssOnTargetAndOnBar()
        {
            int[] mainBoard = new int[] { 0, 0, 0, 0, 0, 5, 0, 3, 0, 0, 0, -5, 5, 0, 0, 0, -3, 0, -4, 0, 0, 0, 0, 0, };
            GameBoardState gbs = new GameBoardState(mainBoard, 1, 1, 2, 1);
            string xml = gbs.GetXML();
            string expected = "<board>0 0 0 0 0 5 0 3 0 0 0 -5 5 0 0 0 -3 0 -4 0 0 0 0 0</board>" +
                              "<whiteBar>1</whiteBar><whiteHome>1</whiteHome><blackBar>2</blackBar><blackHome>1</blackHome>";
            Assert.AreEqual(expected, xml);
        }

        [TestMethod]
        public void TestParsingXML()
        {
            int[] mainBoard = new int[] { 0, 0, 0, 0, 0, 5, 0, 3, 0, 0, 0, -5, 5, 0, 0, 0, -3, 0, -4, 0, 0, 0, 0, 0, };
            GameBoardState gbs = new GameBoardState(mainBoard, 1, 1, 2, 1);
            string xml = gbs.GetXML();

            xml = "<update>" + xml + "</update>";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            var elements = doc.GetElementsByTagName("board");
            foreach (XmlNode element in elements)
            {
                Console.WriteLine("writing:...."); Console.WriteLine(element.InnerText);
            }

            //Console.WriteLine(xml);

           // XmlReader reader = XmlReader.Create(new StringReader(xml));

           // while (reader.Read())
           // {
           //
          //  }

            
        }
    }
}
