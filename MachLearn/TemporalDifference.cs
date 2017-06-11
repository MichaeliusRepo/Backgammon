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
        //internal static double[] w = new double[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
        private static CheckerColor color;
        internal static double[] theta = new double[8] { 0.5, 0.5, 3, 3, 0.5, 0.5, 0.05, 0.05 };  // => (color == White) ? thetaWhite : thetaBlack;
        internal static double[] et => (color == White) ? etWhite : etBlack;
        internal static double[] thetaWhite = new double[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
        internal static double[] thetaBlack = new double[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
        internal static double[] etWhite = new double[8] { 0.1, 0.1, 3, 3, 0.1, 0.1, 0.01, 0.01 };
        internal static double[] etBlack = new double[8] { 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.01, 0.01 };
        internal static double lambda = 0.9, alpha = 0.1; // default values were lambda = 0.7, alpha = 0.1 or 0.02.
        internal static int[] F = new int[8];

        private static void UpdateF(GameBoardState st)
        {
            F = new int[8] {
                -st.getCheckersOnBar(White),
                st.getCheckersOnBar(Black),
                st.getCheckersOnTarget(White),
                -st.getCheckersOnTarget(Black),
                -st.capturableCheckers(White),
                st.capturableCheckers(Black),
                -st.pip(White),
                st.pip(Black)
            };
        }

        private static double EvaluationFunction(GameBoardState st)
        {// Source: http://modelai.gettysburg.edu/2013/tdgammon/pa2.pdf
            UpdateF(st);
            double sum = 0;
            for (int i = 0; i < theta.Length; i++)
                sum = sum + theta[i] * F[i];
            return sum;
        }

        public static double ValueFunction(GameBoardState st)
        { // This is the squashing function, the Sigmoid Function o(a) = 1 / (1 + e^-a)
            // sigma(4) = 0.98, so KEEP VALUES LOW [-5:5]
            return (1.0 / (1.0 + Math.Pow(Math.E, -EvaluationFunction(st))));
        }

        private static double DifferenceFunction(GameBoardState st, GameBoardState st1)
        {
            double v = ValueFunction(st);
            if (!GameOver(st))
                return ValueFunction(st1) - v;
            double z = (st.getCheckersOnTarget(White) == 15) ? 1 : 0;
            return z - v;
        }

        private static void DefineEligibilityTraces(GameBoardState st)
        {
            for (int i = 0; i < et.Length; i++)
                et[i] = lambda * et[i] + DerivativeFunction(F[i], st);
        }

        private static double DerivativeFunction(double d, GameBoardState st)
        { // This is diff(Vt(st),w_i) = diff(sigma(F(w_1,w_2,w_3,...,w_8))w_i);
            var e = Math.Pow(Math.E, -EvaluationFunction(st));
            return d * (e / Math.Pow((1.0 + e), 2));
        }

        private static void ParameterUpdate(GameBoardState st, GameBoardState st1)
        {
            for (int i = 0; i < theta.Length; i++)
                theta[i] = theta[i] + alpha * DifferenceFunction(st, st1) * et[i];
        }

        public static void UpdateWeights(GameBoardState st, GameBoardState st1, CheckerColor c)
        {
            color = c;
            UpdateF(st);
            DefineEligibilityTraces(st);
            ParameterUpdate(st, st1);
            if (double.IsNaN(et[3]))
                throw new Exception();
        }

        public static bool GameOver(GameBoardState s)
        {
            if (s.getCheckersOnTarget(White) == 15 || (s.getCheckersOnTarget(Black) == 15))
                return true;
            return false;
        }
    }
}
