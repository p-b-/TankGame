﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Input
{
    // See https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowshookexa?source=recommendations
    internal enum WindowsHookTypes
    {
        WH_KEYBOARD = 2,
        WH_KEYBOARD_LL = 13,
        WH_MOUSE = 7,
        WH_MOUSE_LL = 14,
        WH_MSGFILTER=-1,
        WH_SHELL= 10,
        WH_SYSMSGFILTER=6,
        WH_CALLWNDPROC=4,
        WH_CALLWNDPROCRET=12,
        WH_CBT=5,
        WH_DEBUG=9,
        WH_FOREGROUNDIDLE=11,
        WH_GETMESSAGE=3,
        WH_JOURNALPLAYBACK=1,
        WH_JOURNALRECORD=0
    }
}
