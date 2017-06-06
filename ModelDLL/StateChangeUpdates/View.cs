using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    public interface View
    {
        void NotifyView();
    }

    public interface Change
    {
        bool IsMove();
        bool IsDiceState();

        //These throws exception if the object is not of the correct type. 
        //Use above methods to check before converting
        Move AsMove();
        DiceState AsDiceState();
    }
}
