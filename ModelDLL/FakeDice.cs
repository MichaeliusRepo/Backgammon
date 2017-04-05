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

        private List<int[]> moves;

        public FakeDice(List<int[]> moves)
        {
            this.moves = moves;
        }
        public FakeDice(int[] returnValues)
        {
            this.moves = new List<int[]>() { returnValues };
        }
 
        public override int[] RollDice()
        {
            if (moves.Count()== 1)
            {
                return moves[0];
            }
            else
            {
                int[] m = moves[0];
                moves.Remove(m);
                return m;
            }
        }

        public void SetReturnValues(int[] diceValues)
        {
            this.moves = new List<int[]>() { diceValues };
            //this.DiceValues = diceValues;
        }
    }
}
