using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
   public class FakeDice : Dice
    {

        private int[] DiceValues = { 0, 0 };

        public FakeDice(int[] returnValues)
        {
            this.DiceValues = returnValues;
        }
 
        public override int[] RollDice()
        {
            return DiceValues;
        }

        public override int[] GetDiceValues()
        {
            return DiceValues;
        }

        public void SetReturnValues(int[] diceValues)
        {
            this.DiceValues = diceValues;
        }
    }
}
