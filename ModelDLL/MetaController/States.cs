using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    internal enum States
    {
        RollgWhite = 1, CheckLegalMovesWhite = 2, MoveWhite = 3, CheckGameFinishedWhite = 4,
        RollBlack, ChecLegalMovesBlack, MoveBlack, CheckGameFinishedBlack,
        GameOver
    }

    internal static class StatesExtensions
    {

    }
}
