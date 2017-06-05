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
    }
}
