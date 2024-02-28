using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Input
{
    internal enum WindowsHookCodes
    {
        HC_ACTION = 0,
        HC_GETNEXT = 1,
        HC_SKIP = 2,
        HC_NOREMOVE = 3,
        HC_NOREM = HC_NOREMOVE,
        HC_SYSMODALON = 4,
        HC_SYSMODALOFF = 5
    }
}
