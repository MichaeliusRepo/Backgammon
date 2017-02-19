using System;
using System.Linq;
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

        private FakeDice fd;
        private BackgammonGame bg;
        private int[] emptyArray = new int[0];
        private int[] initialGameBoard;





        // All pseudocode below was written in comment form. Typos may occur.

        //var b = new BackgammonGame();

        [TestInitialize]
        public void Initialize()
        {
            fd = new FakeDice(new int[] { 1, 1 });
            initialGameBoard = new int[] { -2, 0, 0, 0,  0,  5,
                                            0, 3, 0, 0,  0, -5,
                                            5, 0, 0, 0, -3,  0,
                                           -5, 0, 0, 0,  0,  2 };

            // b.NewGame();
            // Ensure that the player has his turn by the time this method ends!
        }


        //Checks that two arrays are equal
        private bool AreEqualArrays(int[] a1, int[] a2)
        {
            return Enumerable.SequenceEqual(a1, a2);
        }

        //Creates a HashSet<int> from an array of ints
        private HashSet<int> HashSetFromArray(int[] a)
        {
            HashSet<int> tmp = new HashSet<int>();
            foreach(int i in a)
            {
                tmp.Add(i);
            }
            return tmp;
        }

        //The FakeDice is made for testing the Bacgammon Game deterministically.
        //FakeDice needs to be tested as well
        [TestMethod]
        public void TestFakeDice()
        {

            int[] CurrentDiceValues = new int[] { 1, 1 };
            //Testing that FakeDice actually returns what is specified in the constructor
            //and that the output stays the same unless otherwise specified
            for (int i = 0; i < 30; i++)
            { 
                Assert.IsTrue(AreEqualArrays(CurrentDiceValues, fd.GetDiceValues()));
            }

            //Testing that the return change if new return values are specified
            int[] NewReturnValues = new int[] { 2, 3, 4 };
            fd.SetReturnValues(NewReturnValues);
            Assert.IsTrue(AreEqualArrays(NewReturnValues, fd.GetDiceValues()));
            
        }

        [TestMethod]
        public void TestGetLegalMovesForWhite()
        {

            bg = new BackgammonGame(initialGameBoard, fd);
            fd.SetReturnValues(new int[] { 2, 3 });

            //Get the legal moves for whites checkers on position 13.
            HashSet<int> ActualLegalMoves = bg.GetLegalMovesFor(CheckerColor.White, 13);

            //with the initial board configuration and the dice showing 2 and 3, 
            //the legal positions to move to are {11, 10, 8}
            HashSet<int> ExpectedLegalMoves = HashSetFromArray(new int[] { 8, 10, 11 });
            Assert.IsTrue(ActualLegalMoves.SetEquals(ExpectedLegalMoves));

        }

        [TestMethod]
        public void TestGetLegalMovesForBlack()
        {
            bg = new BackgammonGame(initialGameBoard, fd);
            fd.SetReturnValues(new int[] { 5,3});

            //Get the legal moves for black's checkers on position 12
            HashSet<int> ActualLegalMoves = bg.GetLegalMovesFor(CheckerColor.Black, 12);

            //with the initial board configuration and the dice showing 5 and 3, 
            //the legal positions to move to are {15, 17, 20}
            HashSet<int> ExpectedLegalMoves = HashSetFromArray(new int[] { 15, 17, 20 });
            Assert.IsTrue(ActualLegalMoves.SetEquals(ExpectedLegalMoves));
        }


        /*
         * If the dice are showing 5 and 4, the player can move one checker 5, 4 and 9 positions. However, if both the  
         * 4th and 3rd position from the player are blocked, then the 7th should also be unreachable. Therefore, no 
         * positions should be reachable. This test ensures that is the case.
         * */
        [TestMethod]
        public void TestPositionIsUnreachableIfRoadIsBlocked()
        {

            //a game board where the 4th and 5th position are blocked with regard to the black checkers on position 1 
            //and where the 4th and 5th checkers with regard to the white checkers on position 24 are blocked
            int[] gameBoard = new int[] { -2, 0, 0, 0,  2,  3,
                                            0, 3, 0, 0,  0, -5,
                                            5, 0, 0, 0, -3,  0,
                                           -3, -2, 0, 0,  0,  2 };

            this.bg = new BackgammonGame(gameBoard, fd);
            fd.SetReturnValues(new int[] { 4, 5 });

            HashSet<int> ActualLegalMovesWhite = bg.GetLegalMovesFor(CheckerColor.White, 24);
            Assert.AreEqual(0, ActualLegalMovesWhite.Count);

            HashSet<int> ActualLegalMovesBlack = bg.GetLegalMovesFor(CheckerColor.Black, 1);
            Assert.AreEqual(0, ActualLegalMovesBlack.Count);
        }


        //Checking that there are no legal moves from a point where there is no
        //Checker for that player
        [TestMethod]
        public void TestLegalMovesFromPointWithNoCheckerGivesEmptyResult()
        {
            bg = new BackgammonGame(initialGameBoard, fd);

            //Ensures that at least one open position is reachable from anywhere on the board
            fd.SetReturnValues(new int[] {1,2,3,4,5,6,7,8,9,10,11,12,
                13,14,15,16,17,18,19,20,21,22,23,24});
            
            
            //There are no white or black checkers on position 14
            //And
            HashSet<int> legalMovesWhite = 
                bg.GetLegalMovesFor(CheckerColor.White, 14);
            Assert.IsTrue(legalMovesWhite.Count == 0);

            HashSet<int> legalMovesBlack =
                bg.GetLegalMovesFor(CheckerColor.Black, 14);
            Assert.IsTrue(legalMovesBlack.Count == 0);

            //There are black, but not white, checkers on position 12
            //Test that it is not possible to move white checkers from here
            legalMovesWhite =
                bg.GetLegalMovesFor(CheckerColor.White, 12);
            Assert.IsTrue(legalMovesWhite.Count == 0);

            //There are white, but not black, checkers on position 13
            //Test that it is not possible to move black checkers from here
            legalMovesBlack =
                bg.GetLegalMovesFor(CheckerColor.Black, 13);
            Assert.IsTrue(legalMovesBlack.Count == 0);
        }

        [TestMethod]
        public void TestCannotMoveToPositionWithTwoOrMoreEnemyCheckers()
        {
            bg = new BackgammonGame(initialGameBoard, fd);
            fd.SetReturnValues(new int[] { 5, 2 });

            //Given the initial board setup and the values 5 and 2 on the dice, 
            //white can only move to position 22 from position 24,
            // due to 19 and 17 being blocked by more than 1 black checker

            HashSet<int> expectedLegalMovesWhite = new HashSet<int>();
            expectedLegalMovesWhite.Add(22);

            HashSet<int> actualLegalMovesWhite =
                bg.GetLegalMovesFor(CheckerColor.White, 24);

            Assert.IsTrue(actualLegalMovesWhite.SetEquals(expectedLegalMovesWhite));

            //Given the initial board setup and the values 5 and 2 on the dice, 
            //black can only move to position 3 from position 1, due to 6 and 8 being 
            //blocked by more than 1 white checker
            HashSet<int> expectedLegalMovesBlack = new HashSet<int>();
            expectedLegalMovesBlack.Add(3);

            HashSet<int> actualLegalMovesBlack =
                bg.GetLegalMovesFor(CheckerColor.Black, 1);

            Assert.IsTrue(actualLegalMovesBlack.SetEquals(expectedLegalMovesBlack));

        }


        [TestMethod]
        public void TestCannotMoveCheckersOutsideGameBoard()
        {
            //Moving a black checker from position 19 to position 23. WILL BE RESET BEFORE NEXT TEST
            initialGameBoard[18] += 1;
            initialGameBoard[22] -= 1;

            //Moving a white checker from position 6 to position 2. WILL BE RESET BEFORE NEXT TEST
            initialGameBoard[5] -= 1;
            initialGameBoard[1] += 1;

            bg = new BackgammonGame(initialGameBoard, fd);

            //Setting the dice values
            fd.SetReturnValues(new int[] { 6, 2 });

            //All possible moves for the white checer on position 2 ends up outside the board, while
            //white is not bearing off his checkers. Therefore, there are no legal moves for the white checker
            HashSet<int> actualLegalMovesWhite = bg.GetLegalMovesFor(CheckerColor.White, 2);
            Assert.IsTrue(actualLegalMovesWhite.Count == 0);

            //All possible moves for the black checer on position 23 ends up outside the board, while
            //black is not bearing off his checkers. Therefore, there are no legal moves for that black checker
            HashSet<int> actualLegalMovesBlack = bg.GetLegalMovesFor(CheckerColor.Black, 23);
            Assert.IsTrue(actualLegalMovesBlack.Count == 0);
        }

       /* [TestMethod]
        public void TestCannotMoveCheckersOnBoardWhenHasCheckerOnBar()
        {
            //removing a checker white at position 6. WILL BE RESET AFTER TEST
            initialGameBoard[5] -= 1;

            //removing a black checker at position 19. WILL BE RESET AFTER TEST
            initialGameBoard[19] += 1;

            //putting one black and one white checker on the bar
            bg = new BackgammonGame(initialGameBoard, fd, 1, 0, 1, 0);

            //Ensures that any checker will reach any reachable position on the board
            fd.SetReturnValues( new int[] { 1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24});

            //Since white player has a checker on the bar, it should not be possible to move another checker.
            //Checking that white cannot move to any positions with another checker
            HashSet<int> legalMovesWhite = bg.GetLegalMovesFor(CheckerColor.White, 13);
            Assert.AreEqual(0, legalMovesWhite.Count);

            //Since black player has a checker on the bar, it should not be possible to move another checker.
            //Checking that black cannot move to any positions with another checker
            HashSet<int> legalMovesBlack = bg.GetLegalMovesFor(CheckerColor.Black, 12);
            Assert.AreEqual(0, legalMovesBlack.Count);

        }*/

       /* [TestMethod]
        public void TestLegalMovesForCheckersOnBar()
        {
            //removing a checker white at position 6. WILL BE RESET AFTER TEST
            initialGameBoard[5] -= 1;

            //removing a black checker at position 19. WILL BE RESET AFTER TEST
            initialGameBoard[19] += 1;

            //Putting one black and one white checker on the bar
            bg = new BackgammonGame(initialGameBoard, fd, 1, 0, 1, 0);
            fd.SetReturnValues(new int[] { 2, 4 });


            //With the current board setup and the dice showing 2 and 4, we expect white to be able to move the bar
            //checker to positions 23, 21. However, 19 is blocked by black, so it should not be included
            HashSet<int> expectedLegalMovesWhite = HashSetFromArray(new int[] { 23, 21 });

            HashSet<int> actualLegalMovesWhiteBar =
                bg.GetLegalMovesFromBar(CheckerColor.White);

            Assert.IsTrue(actualLegalMovesWhiteBar.SetEquals(expectedLegalMovesWhite));

            //With the current board setup and the dice showing 2 and 4, we expect black to be able to move the bar
            //checker to positions 2 and 4. However,  6 is blocked by white, so it should not be included
            HashSet<int> expectedLegalMovesBlack = HashSetFromArray(new int[] { 2, 4});

            HashSet<int> actualLegalMovesBlackBar =
                bg.GetLegalMovesFromBar(CheckerColor.Black);

            Assert.IsTrue(actualLegalMovesBlackBar.SetEquals(expectedLegalMovesBlack));


        }*/

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
