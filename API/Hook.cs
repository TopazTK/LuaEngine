using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Binarysharp.MSharp;
using SharpConvention = Binarysharp.MSharp.Assembly.CallingConvention.CallingConventions;


namespace LuaEngine.API
{
    public static class Hook
    {
        public static MemorySharp SharpHook;
        public static long CallReturn(int Address, params dynamic[] Arguments)
        {
            if (Arguments == null)
                return SharpHook[(IntPtr)Address].Execute<long>();

            if (Arguments.Length == 1)
                return SharpHook[(IntPtr)Address].Execute<long>(Arguments[0]);

            else if (Arguments.Length > 1)
                return SharpHook[(IntPtr)Address].Execute<long>(SharpConvention.MicrosoftX64, Arguments);
           
            else
                return SharpHook[(IntPtr)Address].Execute<long>();
        }

        public static void JumpFunction(int Address, params dynamic[] Arguments)
        {
            if (Arguments.Length > 1)
                SharpHook[(IntPtr)Address].ExecuteJMP(SharpConvention.MicrosoftX64, Arguments);

            else
                SharpHook[(IntPtr)Address].ExecuteJMP(Arguments[0]);
        }

        public static void CallFunction(int Address, params dynamic[] Arguments)
        {
            if (Arguments == null)
                SharpHook[(IntPtr)Address].Execute<long>();

            if (Arguments.Length == 1)
                SharpHook[(IntPtr)Address].Execute<long>(Arguments[0]);

            else if (Arguments.Length > 1)
                SharpHook[(IntPtr)Address].Execute<long>(SharpConvention.MicrosoftX64, Arguments);

            else
                SharpHook[(IntPtr)Address].Execute<long>();
        }
    }
}
