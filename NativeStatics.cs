using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenOSD {
    public class NativeStatics {
        public const int WH_KEYBOARD_LL   = 13;
        public const int WM_KEYDOWN       = 0x0100;
        public const int VK_CAPITAL       = 0x14;
        public const int VK_NUMLOCK       = 0x90;
        public const int VK_SCROLL        = 0x91;
        public const int GWL_EXSTYLE      = -20;
        public const int WS_EX_TOOLWINDOW = 0x00000080;
        public const int WS_EX_APPWINDOW  = 0x00040000;
        public const int WM_SYSCOMMAND    = 0x0112;
        public const int SC_CLOSE         = 0xF060;
    }
}
