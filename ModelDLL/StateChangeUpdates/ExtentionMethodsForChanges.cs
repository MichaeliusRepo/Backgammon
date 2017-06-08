using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{


    //Note about this class
    // Initially the following methods were declared in the Change interface and implemented in 
    // both DiceState and Move. I didn't do my research, because later I became aware that this 
    // functionality is built into the language. At that point it seemed quite stupid, and I 
    // wanted to remove it. However, the methods had already been used a significant amount of 
    // times in unit tests. I still want to remove the usage of these methods, 
    // but it is currently too far down on the list of priorities, as it would take quite some time. 
    // This solution is a simple hack that reduces /some/ of the clutter.    
    // Now I can remove the mehtods from the public interface of Change, while keeping
    // the method calls in the unit tests.

    internal static class ExtentionMethodsForChanges
    {
        public static bool IsMove(this Change change)
        {
            return change is Move;
        }

        public static bool IsDiceState(this Change change)
        {
            return change is DiceState;
        }

        public static Move AsMove(this Change change)
        {
            if (!IsMove(change))
            {
                throw new InvalidOperationException("Tried to cast DiceState into move");
            }
            return change as Move;
        }

        public static DiceState AsDiceState(this Change change)
        {
            if (!IsDiceState(change))
            {
                throw new InvalidOperationException("Tried to cast Move into DiceState");
            }
            return change as DiceState;
        }
    }
}
