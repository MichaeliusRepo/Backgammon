using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    public class RealDice : Dice
    {
        private static Random generator = new Random();

        public override int[] RollDice()
        {
            int first = generator.Next(1, 7);
            int second = generator.Next(1, 7);

            return new int[] { first, second };
        }
    }
}
