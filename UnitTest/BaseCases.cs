using System;
using ModelDLL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTest
{
    /* TURN BEGIN
    a) Roll Dice
	    a.a) Automagically show checkers to move
		    a.a.a) Show possible moves for selected checker
			    a.a.a.a) Move Checker
				    if (Dices left != 0) GOTO a.a)
				    else END
		    	a.a.a.b) Cancel Selected Checker
			    	GO TO a.a)
    b) Doubling Cube
     */

    [TestClass]
    public class BaseCase
    {
        // All pseudocode below was written in comment form. Typos may occur.

        // var b = new Backgammon();

        [TestInitialize]
        public void Initialize()
        {
            // b.NewGame();
            // Ensure that the player has his turn by the time this method ends!
        }

        [TestMethod]
        public void TestDeselect()
        {
            // b.RollDice();
            // List<int> movableCheckers = b.GetMovableCheckers();
            // int firstChecker = movableCheckers[0];
            // List<int> legalMoves = b.GetLegalMoves(firstChecker);
            // b.DeselectChecker();

            // List<int> sameMovableCheckers = b.GetMovableCheckers();
            // AssertEquals(movableCheckers.Count, sameMovableCheckers.Count);
            // foreach(int i in movableCheckers)
            // AssertEquals(movableCheckers[i], sameMovableCheckers[i]);

            // List<int> sameLegalMoves = b.GetLegalMoves(firstChecker);
            // AssertEquals(legalMoves.Count, sameLegalMoves.Count);
            // foreach(int i in legalMoves)
            // AssertEquals(legalMoves[i], sameLegalMoves[i]);
        }

        [TestMethod]
        public void TestMove()
        {
            // b.RollDice();
            // List<int> movableCheckers = b.GetMovableCheckers();
            // List<int> legalMoves = b.GetLegalMoves(movableCheckers[0]);

            // Pick the first checker and its first legal move.
            //try
            //{
            // b.Move(movableCheckers[0], legalMoves[0]);
            //} catch (Exception e)
            // {
            // Console.WriteLine(e.Message);
            // Assert.Fail();
            // }
        }

        public void Play()
        {
            // b.RollDice();
            // List<int> movableCheckers = b.GetMovableCheckers();
            // List<int> legalMoves = b.GetLegalMoves(movableCheckers[0]);
            // b.Move(movableCheckers[0], legalMoves[0]);
        }

        [TestMethod]
        public void SimulatePlaying()
        {
            // List<int> previousField;
            // List<int> field  = b.GetPlayingField();

            // List<int> movableCheckers;
            // List<int> legalMoves;

            // bool samePlayingField = false;

            //while (! b.Ended)
            //{
            //// This copies the contents of one list into another.
            // previousField = new List<int>(field);
            // Play();
            // field = b.GetPlayingField();

            // samePlayingField = true; // true unless proven otherwise
            // foreach (int i in field)
            // if (field[i] != previousField[i]) {
            // samePlayingField = false; break; }

            // if (samePlayingField)
            // Assert.Fail();
            // }
        }
    }
}
