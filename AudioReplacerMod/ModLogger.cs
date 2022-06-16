using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;

namespace AudioReplacerMod
{
    class ModLogger
    {
        public static void Log(string in_msg)
        {
            if (ModManager.isDebugActivated)
                MelonLogger.Msg(in_msg);
        }
    }
}
