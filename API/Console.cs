using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaEngine.API
{
    internal class Console
    {
        public static void ConsolePrint(dynamic Input, int type)
        {
            switch (type)
            {
                case 0:
                    System.Console.ForegroundColor = ConsoleColor.Green;
                    System.Console.Write("MESSAGE: ");
                    System.Console.ForegroundColor = ConsoleColor.White;
                    break;
                case 1:
                    System.Console.ForegroundColor = ConsoleColor.Yellow;
                    System.Console.Write("WARNING: ");
                    System.Console.ForegroundColor = ConsoleColor.White;
                    break;
                case 2:
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.Write("ERROR: ");
                    System.Console.ForegroundColor = ConsoleColor.White;
                    break;
            }

            System.Console.WriteLine(Input);
        }
    }
}
