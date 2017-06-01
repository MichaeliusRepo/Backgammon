using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelDLL;
using System.Diagnostics;
using System.Collections.Generic;

namespace UnitTest
{
    [TestClass]
    public class TestingAI
    {
        [TestInitialize]
        public void Initialize()
        {
        }


        private bool DoubleEquals(double a, double b)
        {
            return DoubleEquals(a, b, 0.001);
        }

        private bool DoubleEquals(double a, double b, double diff)
        {
            return Math.Abs(a - b) < diff;
        }


        //Testing the probabilities of white getting captured

        //Tests that the probability of white getting captured is zero if no white checkers
        //are capturable (that is, there is no white checker standing alone)
        [TestMethod]
        public void AITestProbabilityOfWhiteGettingCapturedNoCaptureableCheckers()
        {
            GameBoardState gbs = GameBoardState.DefaultGameBoardState;
            Assert.IsTrue(DoubleEquals(gbs.ProbabilityOfWhiteGettingCaptured(), 0.0));

        }


        //Checks that the probability of white getting captured is zero whenever there are no black closer to 
        //White's bar than the capturable white checkers
        [TestMethod]
        public void AITestProbabilityOfWhiteGettingCapturedOneCaptureableCheckerInTheClear()
        {
            int[] board = new int[] {  1, 1, 1, 1, 1, 1,
                                       1, 1, 1, 2, 2 ,2,
                                      -6, 0, 0, 0, 0,-3,
                                       0, 0,-6, 0, 0, 0};
            GameBoardState gbs = new GameBoardState(board, 0, 0, 0, 0);
            Assert.IsTrue(DoubleEquals(gbs.ProbabilityOfWhiteGettingCaptured(), 0.0));
        }

        //Checks that the probability of white getting captured is 2/36 (36 is possible combinations of values on two dice) when exactly one combination of
        //dies results in a capture i.e. (1,2) and (2,1). Two permutations, one combination
        [TestMethod]
        public void AITestProbabilityOfWhiteGettingCapturedOneCombinationOfDies()
        {
            //The below board yeilds that black can capture white only if the dice show (5,4) or (4,5)
            int[] board = new int[] { -2, 2, 2, 2, 2, 0,
                                       2, 2, 2, 1, 0 ,0,
                                      -2,-2, 0, 0, 0, 0,
                                       0, 0, 0, 0, 0,-9};
            GameBoardState gbs = new GameBoardState(board, 0, 0, 0, 0);
            Assert.IsTrue(DoubleEquals(gbs.ProbabilityOfWhiteGettingCaptured(), 2.0/36));


        }

        /*Checks that the probability of white getting captured is 4/36 when 
        exactly two combinations of two different dies (
        i.e, not (1,1), (2,2) etc.) results in a capture */
        [TestMethod]
        public void AITestProbabilityOfWhiteGettingCapturedTwoCombinationsOfDies()
        {
            //The below board yeilds that black can capture white if (5,4), (4,5), (5,3), or (3,5) 
            int[] board = new int[] { -2, 2, 2, 2, 2, 0,
                                       2, 3, 1, 1, 0 ,0,
                                      -2,-2, 0, 0, 0, 0,
                                       0, 0, 0, 0, 0,-9};
            GameBoardState gbs = new GameBoardState(board, 0, 0, 0, 0);


            double actualProbability = gbs.ProbabilityOfWhiteGettingCaptured();
            double expectedProbability = 4.0 / 36;

          //  Console.WriteLine("actual probability: " + actualProbability + "   expected probability: " + expectedProbability + "     2/36:  " + 2.0/36);

            //Four out of 36 permutations yields a capture, so the expected probability is 4/
            Assert.IsTrue(DoubleEquals(actualProbability, expectedProbability));
        }

        /*Checks that the probability of getting captured when only one die 
        (i.e. (5,x) and (x,5) is needed to capture is 11/36, as 5 appears in 
        11 of the 36 permutations
        */
        [TestMethod]
        public void AITestProbabilityOfWhiteGettingCapturedOnlyOneDieNeeded()
        {
            //The below board yeilds that black can capture white if (5,x), (x,5), and (5,5)
            // x can be 1,2,3,4 or 6 
            int[] board = new int[] { -2, 2, 2, 2, 2, 1,
                                       2, 2, 2, 0, 0 ,0,
                                      -2,-2, 0, 0, 0, 0,
                                       0, 0, 0, 0, 0,-9};
            GameBoardState gbs = new GameBoardState(board, 0, 0, 0, 0);


            double actualProbability = gbs.ProbabilityOfWhiteGettingCaptured();
            double expectedProbability = 11.0 / 36;

            //Console.WriteLine("actual probability: " + actualProbability + "   expected probability: " + expectedProbability);

            
            Assert.IsTrue(DoubleEquals(actualProbability, expectedProbability));
        }

        /*If two of the same die is rolled, then the player gets four moves of 
         * the length specified by the die. This test checks that this is 
         * taken into consideration when calculating the probability
         */
        [TestMethod]
        public void AITestProbabilityOfWhiteGettingCapturedTwoEqualDiceRolled()
        {
            //The below board yeilds that black can capture white only if (5,5)
            int[] board = new int[] { -15,14, 0, 0, 0, 0,
                                        0, 0, 0, 0, 0, 0,
                                        0, 0, 0, 0, 0, 0,
                                        0, 0, 1, 0, 0, 0};
            GameBoardState gbs = new GameBoardState(board, 0, 0, 0, 0);


            double actualProbability = gbs.ProbabilityOfWhiteGettingCaptured();
            double expectedProbability = 1.0 / 36;

            //Console.WriteLine("actual probability: " + actualProbability + "   expected probability: " + expectedProbability);


            Assert.IsTrue(DoubleEquals(actualProbability, expectedProbability));
        }

        /* Test that combines the two previous tests into one to check 
         * that it works as expected when the capturing moves stack,
         * without counting some capturing moves twice
         */
        [TestMethod]
        public void AITestProbabilityOfWhiteGettingCapturedWhenBothOneDieAndTwoEqualDiceCapture()
        {
            //The below board yeilds that black can can capture for any move containing 5
            //It also captures the checker on position 21 for (5,5), but that is already counted 
            //in the above. The probability should be 11/36
            int[] board = new int[] { -15, 7, 2, 2, 2, 1,
                                        0, 0, 0, 0, 0, 0,
                                        0, 0, 0, 0, 0, 0,
                                        0, 0, 1, 0, 0, 0};
            GameBoardState gbs = new GameBoardState(board, 0, 0, 0, 0);


            double actualProbability = gbs.ProbabilityOfWhiteGettingCaptured();
            double expectedProbability = 11.0 / 36;

            //Console.WriteLine("actual probability: " + actualProbability + "   expected probability: " + expectedProbability);


            Assert.IsTrue(DoubleEquals(actualProbability, expectedProbability));
        }

        /* Again, testing combination of two previous cases
         * Any move containing 4 and any move containing 5 should
         * capture. Probability: 1 - not 4 or 5 = 1 - 4/6*4/6 = 20/36
         * 
         */
        [TestMethod]
        public void AITestProbabilityOfWhiteGettingCapturedWhenAny4AndAny5Captures()
        {
            //The below board yeilds that black can can capture for any move containing 5
            //It also captures the checker on position 21 for (5,5), but that is already counted 
            //in the above. The probability should be 11/36
            int[] board = new int[] { -15, 7, 2, 4, 1, 1,
                                        0, 0, 0, 0, 0, 0,
                                        0, 0, 0, 0, 0, 0,
                                        0, 0, 0, 0, 0, 0};
            GameBoardState gbs = new GameBoardState(board, 0, 0, 0, 0);


            double actualProbability = gbs.ProbabilityOfWhiteGettingCaptured();
            double expectedProbability = 20.0 / 36;

            //Console.WriteLine("actual probability: " + actualProbability + "   expected probability: " + expectedProbability);


            Assert.IsTrue(DoubleEquals(actualProbability, expectedProbability));
        }

        /* Testing probability of getting captured when 
         * black has checkers on the bar
         */
        [TestMethod]
        public void AITestProbabilityOfWhiteGettingCapturedWhenBlackHasOneCheckerOnBar()
        {
            //The below board yeilds that black can can capture for any move containing 5,
            //and also for (1,4) and (4,1). That is in total 13/36
            int[] board = new int[] {   0, 6, 2, 4, 1, 2,
                                        0, 0, 0, 0, 0, 0,
                                        0, 0, 0, 0, 0, 0,
                                        0, 0, 0, 0, 0,-14};
            GameBoardState gbs = new GameBoardState(board, 0, 0, 1, 0);


            double actualProbability = gbs.ProbabilityOfWhiteGettingCaptured();
            double expectedProbability = 13.0 / 36;

            Console.WriteLine("actual probability: " + actualProbability + "   expected probability: " + expectedProbability);


            Assert.IsTrue(DoubleEquals(actualProbability, expectedProbability));
        }

        /* Testing that whenever black has checkers on the bar such that 
         * no capturing move is available, even though it would've been 
         * if there were no checkers on the bar, the probability to get
         * captured is still zero
         */
        [TestMethod]
        public void AITestProbabilityOfWhiteGettingCapturedZeroWhenCheckersOnBarAndNoCapturingMoveFromThere()
        {
            //The below board yields that that black cannot capture any checkers for any roll of the dice
            //due to the number of checkers stuck on the bar. Note however, that if there were no checkers on
            //the bar, the checker on position 23 could capture the white checker on position 24
            //for any roll of the dice containing a 1. Expected probability is zero
            int[] board = new int[] {   0, 0, 0, 0, 0, 0,
                                        0, 0, 0, 0, 0, 0,
                                        0, 0, 0, 0, 0, 0,
                                        0, 0, 0, 14, -1, 1};
            GameBoardState gbs = new GameBoardState(board, 0, 0, 14, 0);


            double actualProbability = gbs.ProbabilityOfWhiteGettingCaptured();
            double expectedProbability = 0.0 / 36;

            //Console.WriteLine("actual probability: " + actualProbability + "   expected probability: " + expectedProbability);

            Assert.IsTrue(DoubleEquals(actualProbability, expectedProbability));
        }


        /* In some cases, a checker has to be moved of the bar and then some 
         * other checker somewhere else on the board can capture an enemy checker.
         * This test checks that this is accounted for
         */
        [TestMethod]
        public void AITestProbabilityOfWhiteGettingCapturedWhenCheckerOnBarHasToBeMovedBeforeCaptureIsPossible()
        {
            //In the case below, first black has to move the checker of the bar using a one
            //After that, a checker can be captured by a cheker on position 18 using anything 
            //greater than a one. So black can capture for dice (1,x) and (x,1), 
            //where x is in {2,3,4,5,6}. Also, if the roll is (1,1) we get to move (1,1,1,1), and we can move the black checker off the bar
            //and move the checker on position 18 two or three steps forward to capture. That is 11 possibilities, so prob = 11/36
            int[] board = new int[] {   0, 2, 2, 2, 2, 2,
                                        0, 0, 0, 0, 0, 0,
                                        0, 0, 0, 0, 0, -14,
                                        0, 1, 1, 1, 1, 1};
            GameBoardState gbs = new GameBoardState(board, 0, 0, 1, 0);


            double actualProbability = gbs.ProbabilityOfWhiteGettingCaptured();
            double expectedProbability = 11.0 / 36;

            Console.WriteLine("actual probability: " + actualProbability + "   expected probability: " + expectedProbability);

            Assert.IsTrue(DoubleEquals(actualProbability, expectedProbability));
        }

        /* Testing a complicated compound situation. A black checker is stuck on the bar
         * and can get free only with a limited set of moves. One of them captures a checker. 
         * After the checker on the bar is freed, it can capture another checker by performing a new move
         * Also there is another checker somewhere else on the board which can also capture a checker
         */
        [TestMethod]
        public void AITestProbabilityOfWhiteGettingCapturedComplicatedSituation()
        {
            //The checker on the bar has to be freed first. This can be fone with a move of 1 and 5.
            // 1 captures a checker. 
            //From 5 a move of 3 captures another checker. Also, if black rolls (5,5), the checker
            //on the bar can capture the checker on position 20.
            //After the checker has been freed with either a 5 or a 1, a checker from position 21
            //can capture using either a 1 or a 2

            /* Capturing moves are: 
             * -Anything involving a 1:                (1,2) & (2,1), (1,3) & (3,1), (1,4) & (4,1), (1,5) & (5,1), (1,6) & (6,1) and (1,1) 
             * a one or a five, together with a 1 or 2 (1,2) & (2,1), (1,3) & (3,1), (5,2) & (2,5) (5,3) & (3,5)
             * -A 5 and a 3                                                                        (5,3) & (3,5)
             * - Two fives                                                                                      (5,5)
             *
             * 
             * In total, there are 16 unique permutations above. Expected probability is 16/32
             */

            int[] board = new int[] {   1, 3, 2, 3, 0, 2,
                                        0, 1, 0, 0, 0, 0,
                                        0, 0, 0, 0, 0, 0,
                                        0, 1, -14, 1, 1, 0};
            GameBoardState gbs = new GameBoardState(board, 0, 0, 1, 0);


            double actualProbability = gbs.ProbabilityOfWhiteGettingCaptured();
            double expectedProbability = 16.0 / 36;

            //Console.WriteLine("actual probability: " + actualProbability + "   expected probability: " + expectedProbability);


            Assert.IsTrue(DoubleEquals(actualProbability, expectedProbability));
        }


        //TODO enable this test later
        //Checks that the time needed to calculate the probability is not too high
        [TestMethod]
        public void AITestTimeToCalculateDifficultSituationNotTooHigh()
        {
            int[] board = new int[] {  -1, 2, -1, -1, 1, 3,
                                       1, 1, 1, 2, 2 ,2,
                                      -2,-2,0,0,-1,-1,
                                      -1,-1,0,-1,-1,-1};
            GameBoardState gbs = new GameBoardState(board, 0, 0, 1, 0);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            gbs.ProbabilityOfWhiteGettingCaptured();
            stopwatch.Stop();
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 1000);
        }       
    }
}
