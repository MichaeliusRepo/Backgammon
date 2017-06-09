using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelDLL;
using static ModelDLL.CheckerColor;

namespace MachLearn
{
    static class TemporalDifference
    {
        internal static double[] w = new double[8] { 0.1, -0.1, 0, 0, 0.1, -0.1, 0.001, -0.001 };
        internal static double[] EligibilityTraces = new double[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
        internal static double lambda = 0.7, alpha = 0.02; // default values were lambda = 0.7, alpha = 0.1 or 0.02.
        internal static int[] dVdw = new int[8];

        private static void UpdateF(GameBoardState st)
        {
            //dVdwPrev = dVdw;
            dVdw = new int[8] {
                st.getCheckersOnBar(White),
                st.getCheckersOnBar(Black),
                st.getCheckersOnTarget(White),
                st.getCheckersOnTarget(Black),
                st.capturableCheckers(White),
                st.capturableCheckers(Black),
                st.pip(White),
                st.pip(Black)
            };
        }

        private static double EvaluationFunction(GameBoardState st)
        {// Source: http://modelai.gettysburg.edu/2013/tdgammon/pa2.pdf
            UpdateF(st);
            double sum = 0;
            for (int i = 0; i < w.Length; i++)
                sum = sum + w[i] * dVdw[i];
            return sum;
        }

        public static double ValueFunction(GameBoardState st)
        { // This is the squashing function, the Sigmoid Function o(a) = 1 / (1 + e^-a)
            // sigma(4) = 0.98, so KEEP VALUES LOW
            return (double)(1.0 / (1.0 + (double)Math.Pow(Math.E, -EvaluationFunction(st))));
            //return EvaluationFunction(st);
        }

        private static double DifferenceFunction(GameBoardState st, GameBoardState st1)
        {
            double v = ValueFunction(st);
            if (!GameOver(st))
                return ValueFunction(st1) - v;
            double z = (st.getCheckersOnTarget(White) == 15) ? 1 : 0;
            return z - v;
        }

        private static void DefineEligbilityTraces()
        {
            for (int i = 0; i < EligibilityTraces.Length; i++)
                EligibilityTraces[i] = lambda * EligibilityTraces[i] + dVdw[i];
        }

        private static void ParameterUpdate(GameBoardState st, GameBoardState st1)
        {
            for (int i = 0; i < w.Length; i++)
                w[i] = w[i] + alpha * DifferenceFunction(st, st1) * EligibilityTraces[i];
        }

        public static void UpdateWeights(GameBoardState st, GameBoardState st1)
        {
            UpdateF(st);
            DefineEligbilityTraces();
            ParameterUpdate(st, st1);
        }

        public static bool GameOver(GameBoardState s)
        {
            if (s.getCheckersOnTarget(White) == 15 || (s.getCheckersOnTarget(Black) == 15))
                return true;
            return false;
        }
    }
}
