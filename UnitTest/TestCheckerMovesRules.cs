using System;
using System.Linq;
using ModelDLL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTest
{

    [TestClass]
    public class TestCheckerMovesRules
    {

        private readonly CheckerColor WHITE = CheckerColor.White;
        private readonly CheckerColor BLACK = CheckerColor.Black;

        private FakeDice fd;
        private BackgammonGame bg;
        private int[] emptyArray = new int[0];
        private int[] initialGameBoard;

        [TestInitialize]
        public void Initialize()
        {
            fd = new FakeDice(new int[] { 1, 2 });
            initialGameBoard = new int[] { -2, 0, 0, 0,  0,  5,
                                            0, 3, 0, 0,  0, -5,
                                            5, 0, 0, 0, -3,  0,
                                           -5, 0, 0, 0,  0,  2 };
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

            int[] CurrentDiceValues = new int[] { 1, 2 };
            //Testing that FakeDice actually returns what is specified in the constructor
            //and that the output stays the same unless otherwise specified
            for (int i = 0; i < 30; i++)
            { 
                Assert.IsTrue(AreEqualArrays(CurrentDiceValues, fd.RollDice()));
            }

            //Testing that the return change if new return values are specified
            int[] NewReturnValues = new int[] { 2, 3, 4 };
            fd.SetReturnValues(NewReturnValues);
            Assert.IsTrue(AreEqualArrays(NewReturnValues, fd.RollDice()));
            
        }



        
        [TestMethod]
        public void TestGetLegalMovesForWhite()
        {

            fd.SetReturnValues(new int[] { 2, 3 });
            bg = new BackgammonGame(initialGameBoard, fd);
            

            //Get the legal moves for whites checkers on position 13.
            HashSet<int> ActualLegalMoves = bg.GetLegalMovesFor(WHITE, 13);

            //with the initial board configuration and the dice showing 2 and 3, 
            //the legal positions to move to are {11, 10, 8}
            HashSet<int> ExpectedLegalMoves = HashSetFromArray(new int[] { 8, 10, 11 });
            Assert.IsTrue(ActualLegalMoves.SetEquals(ExpectedLegalMoves));

        }
        

        
        [TestMethod]
        public void TestGetLegalMovesForBlack()
        {
            fd.SetReturnValues(new int[] { 5, 3 });
            bg = new BackgammonGame(initialGameBoard, fd);
            

            //Get the legal moves for black's checkers on position 12
            HashSet<int> ActualLegalMoves = bg.GetLegalMovesFor(BLACK, 12);

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

            fd.SetReturnValues(new int[] { 4, 5 });
            this.bg = new BackgammonGame(gameBoard, fd);
            

            HashSet<int> ActualLegalMovesWhite = bg.GetLegalMovesFor(WHITE, 24);
            Assert.AreEqual(0, ActualLegalMovesWhite.Count);

            HashSet<int> ActualLegalMovesBlack = bg.GetLegalMovesFor(BLACK, 1);
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
                bg.GetLegalMovesFor(WHITE, 14);
            Assert.IsTrue(legalMovesWhite.Count == 0);

            HashSet<int> legalMovesBlack =
                bg.GetLegalMovesFor(BLACK, 14);
            Assert.IsTrue(legalMovesBlack.Count == 0);

            //There are black, but not white, checkers on position 12
            //Test that it is not possible to move white checkers from here
            legalMovesWhite =
                bg.GetLegalMovesFor(WHITE, 12);
            Assert.IsTrue(legalMovesWhite.Count == 0);

            //There are white, but not black, checkers on position 13
            //Test that it is not possible to move black checkers from here
            legalMovesBlack =
                bg.GetLegalMovesFor(BLACK, 13);
            Assert.IsTrue(legalMovesBlack.Count == 0);
        }

        
        [TestMethod]
        public void TestCannotMoveToPositionWithTwoOrMoreEnemyCheckers()
        {
            fd.SetReturnValues(new int[] { 5, 2 });
            bg = new BackgammonGame(initialGameBoard, fd);

            //Given the initial board setup and the values 5 and 2 on the dice, 
            //white can only move to position 22 from position 24,
            // due to 19 and 17 being blocked by more than 1 black checker

            HashSet<int> expectedLegalMovesWhite = new HashSet<int>();
            expectedLegalMovesWhite.Add(22);

            HashSet<int> actualLegalMovesWhite =
                bg.GetLegalMovesFor(WHITE, 24);

            Assert.IsTrue(actualLegalMovesWhite.SetEquals(expectedLegalMovesWhite));

            //Given the initial board setup and the values 5 and 2 on the dice, 
            //black can only move to position 3 from position 1, due to 6 and 8 being 
            //blocked by more than 1 white checker
            HashSet<int> expectedLegalMovesBlack = new HashSet<int>();
            expectedLegalMovesBlack.Add(3);

            HashSet<int> actualLegalMovesBlack =
                bg.GetLegalMovesFor(BLACK, 1);

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
            HashSet<int> actualLegalMovesWhite = bg.GetLegalMovesFor(WHITE, 2);
            Assert.IsTrue(actualLegalMovesWhite.Count == 0);

            //All possible moves for the black checer on position 23 ends up outside the board, while
            //black is not bearing off his checkers. Therefore, there are no legal moves for that black checker
            HashSet<int> actualLegalMovesBlack = bg.GetLegalMovesFor(BLACK, 23);
            Assert.IsTrue(actualLegalMovesBlack.Count == 0);
        }

        // TODO: Write tests that check that it is illegal to move from a position that is outside the board, other than the bar positions
        //in other words, fromPosition >=1, fromPosition <= 24, or fromposition == whitebar or blackbar

        [TestMethod]
        public void TestCannotMoveFromOutsideTheGameBoard()
        {
            initialGameBoard[0]++; //Removing a black checker from position 1
            initialGameBoard[23]--; //Removing a white checker from position 24

            fd.SetReturnValues(new int[] { 3, 2 });

            //Creating a game with the above specificed game board, and one checker of each type bore offsa
            bg = new BackgammonGame(initialGameBoard, fd, 0, 1, 0, 1);

            HashSet<int>  whiteMovesFromBearOff = bg.GetLegalMovesFor(WHITE, WHITE.BearOffPositionID());
            Assert.IsTrue(whiteMovesFromBearOff.Count() == 0);

            HashSet<int>  whiteMovesFromOutsideBoard = bg.GetLegalMovesFor(WHITE, 26);
            Assert.IsTrue(whiteMovesFromOutsideBoard.Count() == 0);

            HashSet<int>  blackMovesFromBearOff = bg.GetLegalMovesFor(BLACK, BLACK.BearOffPositionID());
            Assert.IsTrue(blackMovesFromBearOff.Count() == 0);

            HashSet<int>  blackMovesFromOutsideBoard = bg.GetLegalMovesFor(BLACK, -2);
            Assert.IsTrue(blackMovesFromOutsideBoard.Count() == 0);
        }



        [TestMethod]
        public void TestCannotMoveCheckersOnBoardWhenHasCheckerOnBar()
        {
            //removing a checker white at position 6. WILL BE RESET AFTER TEST
            initialGameBoard[5] -= 1;

            //removing a black checker at position 19. WILL BE RESET AFTER TEST
            initialGameBoard[18] += 1;

            //putting one black and one white checker on the bar
            bg = new BackgammonGame(initialGameBoard, fd, 1, 0, 1, 0);

            //Ensures that any checker will reach any reachable position on the board
            fd.SetReturnValues( new int[] { 1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24});

            //Since white player has a checker on the bar, it should not be possible to move another checker.
            //Checking that white cannot move to any positions with another checker
            HashSet<int> legalMovesWhite = bg.GetLegalMovesFor(WHITE, 13);
            Assert.AreEqual(0, legalMovesWhite.Count);

            //Since black player has a checker on the bar, it should not be possible to move another checker.
            //Checking that black cannot move to any positions with another checker
            HashSet<int> legalMovesBlack = bg.GetLegalMovesFor(BLACK, 12);
            Assert.AreEqual(0, legalMovesBlack.Count);

        }
        
        
        [TestMethod]
        public void TestGetLegalMovesForCheckersOnBar()
        {
            //removing a checker white at position 6. WILL BE RESET AFTER TEST
            initialGameBoard[5] -= 1;

            //removing a black checker at position 19. WILL BE RESET AFTER TEST
            initialGameBoard[18] += 1;

            fd.SetReturnValues(new int[] { 6, 3 });

            //putting one black and one white checker on the bar
            bg = new BackgammonGame(initialGameBoard, fd, 1, 0, 1, 0);

            

            //With the current setup, white should be able to move to positions 22, 16
            //and black to positions 3 and 9
            HashSet<int> expectedLegalMovesBlack = HashSetFromArray(new int[] { 3, 9 });
            HashSet<int> actualLegalMovesBlack =
                bg.GetLegalMovesFor(BLACK, BackgammonGame.BLACK_BAR_ID);

            Console.WriteLine("Expected black moves: " + string.Join(", ", expectedLegalMovesBlack));
            Console.WriteLine("Actual black moves: " + string.Join(", ", actualLegalMovesBlack));

            Assert.IsTrue(actualLegalMovesBlack.SetEquals(expectedLegalMovesBlack));


            HashSet<int> expectedLegalMovesWhite = HashSetFromArray(new int[] { 22, 16 });
            HashSet<int> actualLegalMovesWhite =
               bg.GetLegalMovesFor(WHITE, BackgammonGame.WHITE_BAR_ID);

            Assert.IsTrue(actualLegalMovesWhite.SetEquals(expectedLegalMovesWhite));
        }

        

        //Test that the white player cannot move from the black bar, and vice versa
        [TestMethod]
        public void TestGetWhiteMovesFromBlackBarEmptyAndViceVersa()
        {
            bg = new BackgammonGame(initialGameBoard, fd);
            fd.SetReturnValues(new int[] { 1, 2 });
            HashSet<int> legalMovesWhite = 
                bg.GetLegalMovesFor(BLACK, BackgammonGame.WHITE_BAR_ID);
            Assert.AreEqual(0, legalMovesWhite.Count());

            HashSet<int> legalMovesBlack =
                bg.GetLegalMovesFor(WHITE, BackgammonGame.BLACK_BAR_ID);
            Assert.AreEqual(0, legalMovesBlack.Count());
        }


        //Test that one cannot bear off when not all checkers have been moved to the home board
        [TestMethod]
        public void TestCannotBearOffCheckersWhenNotAllCheckersInHomeBoard()
        {
            bg = new BackgammonGame(initialGameBoard, fd);
            fd.SetReturnValues(new int[] { 6,1});
            //With this setup, the black checkers on position 19 and the white checkers on position 6 are able to 
            //reach their bear off position. However, all checkers are not in the homeboard, so it should not be available.

            HashSet<int> legalBlackMoves = bg.GetLegalMovesFor(BLACK, 19);
            Assert.IsFalse(legalBlackMoves.Contains(BackgammonGame.BLACK_BEAR_OFF_ID));

            HashSet<int> legalWhiteMoves = bg.GetLegalMovesFor(WHITE, 6);
            Assert.IsFalse(legalWhiteMoves.Contains(BackgammonGame.WHITE_BEAR_OFF_ID));
        }


        //Test that checkers can be bore off if all checkers are in home board
        [TestMethod]
        public void TestCanBearCheckersOffWhenAllCheckersInHomeBoard()
        {

            int[] gameBoard = new int[] {  0, 3, 3, 3,  3,  3,
                                           0, 0, 0, 0,  0,  0,
                                           0, 0, 0, 0,  0,  0,
                                          -3, -3, -3, -3,  -3, 0 };
            fd.SetReturnValues(new int[] { 2, 3 });
            bg = new BackgammonGame(gameBoard, fd);

            //With this setup, both players should be able to bear off checkers that are two, three or five positions away

            HashSet<int> expectedLegalMovesBlack = HashSetFromArray(new int[] { 22, 23, BackgammonGame.BLACK_BEAR_OFF_ID } );
            HashSet<int> actualLegalMovesBlack = bg.GetLegalMovesFor(BLACK, 20);

            Console.WriteLine("Expected black moves: " + string.Join(", ", expectedLegalMovesBlack));
            Console.WriteLine("Actual black moves: " + string.Join(", ", actualLegalMovesBlack));


            Assert.IsTrue(expectedLegalMovesBlack.SetEquals(actualLegalMovesBlack));

            HashSet<int> expectedLegalMovesWhite = new HashSet<int>() { 2, 3,  BackgammonGame.WHITE_BEAR_OFF_ID};
            HashSet<int> actualLegalMovesWhite = bg.GetLegalMovesFor(WHITE, 5);

            Assert.IsTrue(expectedLegalMovesWhite.SetEquals(actualLegalMovesWhite));

        }


        /*
         * Sometimes, a checker can be bore off if the eyes on the dice are greater than the distance to the bear off position.
         * For instance, if you have a checker on position 5, and you get a 6 on a die, you can bear off the checker on position 5
         * if you don't have a checker on position 6. This test checks that this criteria is maintained.
         */
        [TestMethod]
        public void TestCannotBearOffUsingOvershootIfCheckersFurtherAwayArePresent()
        {
            
            int[] gameBoard = new int[] {  0, 3, 3, 3,  3,  3,
                                           0, 0, 0, 0,  0,  0,
                                           0, 0, 0, 0,  0,  0,
                                          -3, -3, -3, -3,  -3, 0 };
            fd.SetReturnValues(new int[] { 6, 4 });
            bg = new BackgammonGame(gameBoard, fd);
            

            HashSet<int> expectedWhiteMoves = HashSetFromArray(new int[] {1});
            HashSet<int> actualWhiteMoves = bg.GetLegalMovesFor(WHITE, 5);

            Assert.IsTrue(expectedWhiteMoves.SetEquals(actualWhiteMoves));

            HashSet<int> expectedBlackMoves = HashSetFromArray(new int[] { 24 });
            HashSet<int> actualBlackMoves = bg.GetLegalMovesFor(BLACK, 20);

            Assert.IsTrue(expectedBlackMoves.SetEquals(actualBlackMoves));
        }
        

        //Test that a checker can be bore off with a move that is longer than the distance to the bear off position
        //If there are no other checkers further away from the bar position
        [TestMethod]
        public void CanBearOffUsingOvershootIfNoCheckersFurtherAwayArePresent()
        {
            int[] gameBoard = new int[] {  0, 3, 3, 3,  6,  0,
                                           0, 0, 0, 0,  0,  0,
                                           0, 0, 0, 0,  0,  0,
                                           0, -6, -3, -3,  -3, 0 };
            fd.SetReturnValues(new int[] { 6, 4 });
            bg = new BackgammonGame(gameBoard, fd);
            

            HashSet<int> expectedWhiteMoves = HashSetFromArray(new int[] { 1, BackgammonGame.WHITE_BEAR_OFF_ID});
            HashSet<int> actualWhiteMoves = bg.GetLegalMovesFor(WHITE, 5);

            Console.WriteLine("Expected white moves: " + string.Join(", ", expectedWhiteMoves));
            Console.WriteLine("  Actual white moves: " + string.Join(", ", actualWhiteMoves));

            Assert.IsTrue(expectedWhiteMoves.SetEquals(actualWhiteMoves));

            HashSet<int> expectedBlackMoves = HashSetFromArray(new int[] { 24, BackgammonGame.BLACK_BEAR_OFF_ID });
            HashSet<int> actualBlackMoves = bg.GetLegalMovesFor(BLACK, 20);

            Console.WriteLine("Expected black moves: " + string.Join(", ", expectedBlackMoves));
            Console.WriteLine("  Actual black moves: " + string.Join(", ", actualBlackMoves));

            Assert.IsTrue(expectedBlackMoves.SetEquals(actualBlackMoves));

        }

        //Test that an exception is thrown if the attempted move is illegal
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestIllegalMoveWhiteThrowsException()
        {
            fd.SetReturnValues(new int[] { 5, 1 });
            bg = new BackgammonGame(initialGameBoard, fd);

            bg.Move(WHITE, 6, 1);
        }

        //Test that an exception is thrown if the attempted move is illegal
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestIllegalMoveBlackThrowsException()
        {
            fd.SetReturnValues(new int[] { 5, 1 });
            bg = new BackgammonGame(initialGameBoard, fd);
            bg.Move(BLACK, 19, 24);
        }


        //Test that the game board state is updated correctly when a move is performed
        [TestMethod]
        public void TestMoveUpdatesCheckersOnPositionsWhite()
        {
            fd.SetReturnValues(new int[] { 2, 3 });
            bg = new BackgammonGame(initialGameBoard, fd);

            bg.Move(WHITE, 6, 3);

            int[] actualResult = bg.GetGameBoardState().getMainBoard();

            int[] expectedResult =  new int[] { -2, 0, 1, 0,  0,  4,
                                                 0, 3, 0, 0,  0, -5,
                                                 5, 0, 0, 0, -3,  0,
                                               - 5, 0, 0, 0,  0,  2 };

            Assert.IsTrue(Enumerable.SequenceEqual(actualResult, expectedResult));
        }

        //Test that the game board state is updated correctly when a move is performed
        [TestMethod]
        public void TestMoveUpdatesCheckersOnPositionsBlack()
        {
            fd.SetReturnValues(new int[] { 2, 3 });
            bg = new BackgammonGame(initialGameBoard, fd, 0,0,0,0,BLACK);
            bg.Move(BLACK, 19, 22);
            int[] actualResult = bg.GetGameBoardState().getMainBoard();
            int[] expectedResult = new int[] { -2, 0, 0, 0,  0,  5,
                                                 0, 3, 0, 0,  0, -5,
                                                 5, 0, 0, 0, -3,  0,
                                               - 4, 0, 0, -1,  0,  2 };

            Assert.IsTrue(Enumerable.SequenceEqual(actualResult, expectedResult));
        }


        //Test that the game board state is updated correctly whenever a move in which a checker is captured is made
        [TestMethod]
        public void TestMoveToEnemyPositionWithOneCheckerCapturesCheckerWhite()
        {
            int[] gameBoard =       new int[] { -1, -1, 0, 0,  0,  5,
                                                 0, 3, 0, 0,  0, -5,
                                                 5, 0, 0, 0, -3,  0,
                                               - 5, 0, 0, 0,  1,  1 };
            fd.SetReturnValues(new int[] { 3,4});
            bg = new BackgammonGame(gameBoard, fd);

            bg.Move(WHITE, 6, 2);
            int[] actualResult = bg.GetGameBoardState().getMainBoard();
            int[] expectedResult =  new int[] { -1, 1, 0, 0,  0,  4,
                                                 0, 3, 0, 0,  0, -5,
                                                 5, 0, 0, 0, -3,  0,
                                               - 5, 0, 0, 0,  1,  1 };

            Assert.IsTrue(Enumerable.SequenceEqual(actualResult, expectedResult), "Game boards are not equal");

            Assert.IsTrue(bg.GetGameBoardState().GetCheckersOnPosition(BLACK.GetBar()) == 1);
        }

        //Test that the game board state is updated correctly whenever a move in which a checker is captured is made
        [TestMethod]
        public void TestMoveToEnemyPositionWithOneCheckerCapturesCheckerBlack()
        {
            int[] gameBoard = new int[] { -1, -1, 0, 0,  0,  5,
                                                 0, 3, 0, 0,  0, -5,
                                                 5, 0, 0, 0, -3,  0,
                                               - 5, 0, 0, 0,  1,  1 };
            fd.SetReturnValues(new int[] { 3, 4 });
            bg = new BackgammonGame(gameBoard, fd, 0,0,0,0, BLACK);

            bg.Move(BLACK, 19, 23);
            int[] actualResult = bg.GetGameBoardState().getMainBoard();
            int[] expectedResult = new int[] { -1, -1, 0, 0,  0,  5,
                                                 0, 3, 0, 0,  0, -5,
                                                 5, 0, 0, 0, -3,  0,
                                               - 4, 0, 0, 0,  -1,  1 };

            Assert.IsTrue(Enumerable.SequenceEqual(actualResult, expectedResult), "Game boards are not equal");
            Assert.IsTrue(bg.GetGameBoardState().GetCheckersOnPosition(WHITE.GetBar()) == 1);
        }

        [TestMethod]
        public void TestMoveConsistingOfMultipleMovesWhite()
        {
            fd.SetReturnValues(new int[] {3,1});
            bg = new BackgammonGame(initialGameBoard, fd);

            bg.Move(WHITE, 6, 2);

            int[] expectedGameBoard = new int[] { -2, 1, 0, 0,  0,  4,
                                                   0, 3, 0, 0,  0, -5,
                                                   5, 0, 0, 0, -3,  0,
                                                  -5, 0, 0, 0,  0,  2 };

            int[] actualGameBoard = bg.GetGameBoardState().getMainBoard();

            Assert.IsTrue(Enumerable.SequenceEqual(expectedGameBoard, actualGameBoard));
        }

        [TestMethod]
        public void TestMoveConsistingOfMultipleMovesBlack()
        {
            fd.SetReturnValues(new int[] { 3, 1 });
            bg = new BackgammonGame(initialGameBoard, fd, 0,0,0,0, BLACK);

            bg.Move(BLACK, 19, 23);

            int[] expectedGameBoard = new int[] { -2, 0, 0, 0,  0,  5,
                                                   0, 3, 0, 0,  0, -5,
                                                   5, 0, 0, 0, -3,  0,
                                                  -4, 0, 0, 0, -1,  2 };

            int[] actualGameBoard = bg.GetGameBoardState().getMainBoard();

            Assert.IsTrue(Enumerable.SequenceEqual(expectedGameBoard, actualGameBoard));
        }

        //Whenever there are multiple ways to get from one position to another, and one of the ways captures a checker
        //While the others do not, the way capturing a checker is to be prioritized 
        [TestMethod]
        public void TestMoveConsistingOfMultipleMovesPrioritisesCapturingChecker()
        {
            int[] gameBoard =         new int[] { -1, 0, 0, -1,  0,  5,
                                                   0, 3, 0,  0,  0, -5,
                                                   5, 0, 0,  0, -3,  0,
                                                  -5, 0, 0,  0,  0,  2 };

            fd.SetReturnValues(new int[] { 1, 2 });
            bg = new BackgammonGame(gameBoard, fd);

            List<int> actualMovesTaken = bg.Move(WHITE, 6, 3);

            int[] expectedGameBoard = new int[] { -1, 0, 1, 0,  0,  4,
                                                   0, 3, 0, 0,  0, -5,
                                                   5, 0, 0, 0, -3,  0,
                                                  -5, 0, 0, 0,  0,  2 };

            int[] actualGameBoard = bg.GetGameBoardState().getMainBoard();

            Assert.IsTrue(Enumerable.SequenceEqual(expectedGameBoard, actualGameBoard));
            Assert.IsTrue(bg.GetGameBoardState().getCheckersOnBar(BLACK) == 1);


            //Checks that the moves taken are equal to the moves expected
            List<int> expectedMovesTaken = new List<int>() { 2, 1 };
            Assert.IsTrue(Enumerable.SequenceEqual(actualMovesTaken, expectedMovesTaken));

        }

    }
}
