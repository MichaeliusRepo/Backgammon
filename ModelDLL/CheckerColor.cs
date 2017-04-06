using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{

    public static class CheckerColorExtensions
    {
        public const int WHITE_BAR_ID = 151357818;
        public const int BLACK_BAR_ID = 612345638;

        public static int GetBar(this CheckerColor color)
        {
            return color == CheckerColor.White ? WHITE_BAR_ID : BLACK_BAR_ID; 
        }


        //In the internal logic of the model, one white checker is represented by 1
        //and one black checker is represented by a -1
        public static int IntegerRepresentation(this CheckerColor color)
        {
            return color == CheckerColor.White ? 1 : -1;
        }

        //White checkers move from position 24 to position 1. For a absolute distance change of 5,
        //a white checker moves negative 5 (-5) positions. Black checkers move from 1 to 24, and 
        //they move the same number of positions as the distance they travel
        public static int ChangeInPositionDueToTravelDistance(this CheckerColor color, int distance)
        {
            return distance * (color == CheckerColor.White ? -1 : 1);
        }

        //The bars of each checker color has it's own ID. However, in reality, the bar of the white player is 
        //placed right next to position 24 (or max position), so that when a move of distance 1 is made by a 
        //white checker from the white bar, the resulting position is 24. Similarly for the black bar, it is located 
        //so that a move of 1 leads to position 1. 
        public static int BarPositionWithRegardsToBoard(this CheckerColor color)
        {
            return (color == CheckerColor.White ? GameBoardState.NUMBER_OF_POSITIONS_ON_BOARD + 1 : 0);
        }
    }

    public enum CheckerColor { White, Black };
}
