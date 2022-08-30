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
        public static void InjectDelegate(ILContext il, Action action)
        {
            var c = new ILCursor(il);
            
            c.Emit(OpCodes.Call, action.GetMethodInfo());
        }

        public static void InjectDelegateEnd(ILContext il, Action action)
        {
            var c = new ILCursor(il);
            c.Goto(c.Instrs.Last());
            c.Emit(OpCodes.Call, action.GetMethodInfo());
        }

        public static void InjectSkipOnBoolean(ILContext il, Func<bool> action)
        {
            var c = new ILCursor(il);
            c.Emit(OpCodes.Call, action.GetMethodInfo());
            var after = c.DefineLabel();
            c.Emit(OpCodes.Brfalse_S, after);
            c.Emit(OpCodes.Ret);
            c.MarkLabel(after);
        }

        public static void InjectSkipOnBooleanWithReturnValue(ILContext il, Func<bool> action, Func<object> retValue)
        {
            var c = new ILCursor(il);
            c.Emit(OpCodes.Call, action.GetMethodInfo());
            var after = c.DefineLabel();
            c.Emit(OpCodes.Brfalse_S, after);
            c.Emit(OpCodes.Call, retValue.GetMethodInfo());
            c.Emit(OpCodes.Ret);
            c.MarkLabel(after);
        }
    }
}
