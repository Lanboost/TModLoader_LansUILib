using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace LansUILib
{
    public class InjectUtils
    {
        public static void InjectSkipOnBoolean(ILContext il, Func<bool> action)
        {
            var c = new ILCursor(il);
            c.Emit(OpCodes.Call, action.GetMethodInfo());
            var after = c.DefineLabel();
            c.Emit(OpCodes.Brfalse_S, after);
            c.Emit(OpCodes.Ret);
            c.MarkLabel(after);
        }
    }
}
