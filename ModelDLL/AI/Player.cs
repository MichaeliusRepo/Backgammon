using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    public interface Player
    {
        void MakeMove();
        void ConnectPlayerInterface(PlayerInterface playerInterface);
    }
}
