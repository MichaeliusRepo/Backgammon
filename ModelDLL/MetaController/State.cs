using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    internal abstract class State
    {
        internal protected BackgammonGame bg;
        internal protected CheckerColor color;
        internal State(BackgammonGame bg, CheckerColor color)
        {
            this.bg = bg;
            this.color = color;
        }

        internal abstract void Execute();
        internal abstract State NextState();
        internal CheckerColor PlayerToMove()
        {
            return color;
        }
        
    }
}
