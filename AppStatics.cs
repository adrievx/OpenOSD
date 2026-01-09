using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenOSD {
    public class AppStatics {
        public static readonly Dictionary<int, (string label, string icon)> LockKeys = new Dictionary<int, (string, string)> {
            { NativeStatics.VK_CAPITAL, ("CAPS LOCK",   "/OpenOSD;component/Images/capslock.png") },
            { NativeStatics.VK_NUMLOCK, ("NUM LOCK",    "/OpenOSD;component/Images/genlock.png") },
            { NativeStatics.VK_SCROLL,  ("SCROLL LOCK", "/OpenOSD;component/Images/scrolllock.png") }
        };
    }
}