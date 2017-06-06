using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    public class DiceState : Change
    {
        private int[] diceValues;

        internal DiceState(int[] dicevalues)
        {
            this.diceValues = dicevalues;
        }

        internal DiceState(List<int> diceValues)
        {
            this.diceValues = diceValues.ToArray();
        }
        

        public int[] GetDiceValues()
        {
            return new List<int>(diceValues).ToArray();
        }

        public DiceState AsDiceState()
        {
            return this;
        }

        public Move AsMove()
        {
            throw new InvalidOperationException("Tried to cast Move into DiceState");
        }

        public bool IsDiceState()
        {
            return true;
        }

        public bool IsMove()
        {
            return false;
        }
    }
}
