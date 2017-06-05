using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    class DiceState : Change
    {
        public int[] diceValues;

        public DiceState(int[] dicevalues)
        {
            this.diceValues = dicevalues;
        }
    }
}
