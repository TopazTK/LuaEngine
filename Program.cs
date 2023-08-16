using System;
using System.Linq;
using System.Diagnostics;
using System.Security.Cryptography;
using System.IO.Hashing;

using LuaEngine.API;

using Binarysharp.MSharp;
using SharpConvention = Binarysharp.MSharp.Assembly.CallingConvention.CallingConventions;

using NLua;
using System.Text;
using SysConsole = System.Console;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.RepresentationModel;
using NLua.Exceptions;
using YamlDotNet.Core.Tokens;

namespace LuaEngine
{
    public static class Program
    {
        static List<Lua> ScriptList = new List<Lua>();
        static List<Task> ThreadList = new List<Task>();

        public static void RegisterAPI(Lua Input)
        {
            // Function Calling
            Input.RegisterFunction("CallReturn", typeof(Hook).GetMethod("CallReturn"));
            Input.RegisterFunction("CallFunction", typeof(Hook).GetMethod("CallFunction"));
            Input.RegisterFunction("JumpFunction", typeof(Hook).GetMethod("JumpFunction"));

            // Writing Memory
            Input.RegisterFunction("WriteArray", typeof(Memory).GetMethod("WriteArray"));
            Input.RegisterFunction("WriteString", typeof(Memory).GetMethod("WriteString"));
            Input.RegisterFunction("WriteBoolean", typeof(Memory).GetMethod("WriteBoolean"));
            Input.RegisterFunction("WriteByte", typeof(Memory).GetMethod("WriteByte"));
            Input.RegisterFunction("WriteShort", typeof(Memory).GetMethod("WriteShort"));
            Input.RegisterFunction("WriteInt", typeof(Memory).GetMethod("WriteInt"));
            Input.RegisterFunction("WriteLong", typeof(Memory).GetMethod("WriteLong"));
            Input.RegisterFunction("WriteDouble", typeof(Memory).GetMethod("WriteDouble"));
            Input.RegisterFunction("WriteFloat", typeof(Memory).GetMethod("WriteFloat"));

            // Reading Memory
            Input.RegisterFunction("ReadArray", typeof(Memory).GetMethod("ReadArray"));
            Input.RegisterFunction("ReadString", typeof(Memory).GetMethod("ReadString"));
            Input.RegisterFunction("ReadBoolean", typeof(Memory).GetMethod("ReadBoolean"));
            Input.RegisterFunction("ReadByte", typeof(Memory).GetMethod("ReadByte"));
            Input.RegisterFunction("ReadShort", typeof(Memory).GetMethod("ReadShort"));
            Input.RegisterFunction("ReadInt", typeof(Memory).GetMethod("ReadInt"));
            Input.RegisterFunction("ReadLong", typeof(Memory).GetMethod("ReadLong"));
            Input.RegisterFunction("ReadDouble", typeof(Memory).GetMethod("ReadDouble"));
            Input.RegisterFunction("ReadFloat", typeof(Memory).GetMethod("ReadFloat"));

            // Console Interaction
            Input.RegisterFunction("ConsolePrint", typeof(API.Console).GetMethod("ConsolePrint"));
        }

        public static Lua InitalizeScript(string ScriptName)
        {
            var _luaState = new Lua();
            var _crcCalc = new Crc32();

            _crcCalc.Append(Encoding.Default.GetBytes(Hypervisor.Process.ProcessName));

            // Game Properties
            _luaState["GAME_ID"] = BitConverter.ToInt32(_crcCalc.GetCurrentHash(), 0);
            _luaState["EXE_ADDRESS"] = Hypervisor.PureAddress;
            _luaState["BASE_ADDRESS"] = Hypervisor.BaseAddress;
            _luaState["MEMORY_OFFSET"] = Hypervisor.PureAddress & Hypervisor.MemoryOffset;

            // Engine Properties
            _luaState["ENGINE_VERSION"] = 6;
            _luaState["ENGINE_TYPE"] = "ENGINE_REDUX";

            // Pathways.
            _luaState["CHEATS_PATH"] = "NOT_AVAILABLE";
            _luaState["SCRIPT_PATH"] = ScriptName;

            RegisterAPI(_luaState);

            _luaState.DoFile(ScriptName);

            return _luaState;
        }

        public static void Main()
        {
            SysConsole.ForegroundColor = ConsoleColor.White;

            SysConsole.WriteLine("==========================================");
            SysConsole.WriteLine("===   LuaEngine - Rewritten | v1.00S   ===");
            SysConsole.WriteLine("== Programmed and Maintained by TopazTK ==");
            SysConsole.WriteLine("==========================================");

            SysConsole.WriteLine("");

            if (!File.Exists("gameFile.yml"))
            {
                API.Console.ConsolePrint("The configuration file \"gameFile.yml\" was not found! Shutting down...", 2);
                SysConsole.WriteLine("Press any key to continue...");

                SysConsole.ReadKey();
                return;
            }

            var _scriptBlacklist = new List<Lua>();

            var _processArray = new Process[0];
            var _configStream = new StreamReader("gameFile.yml");

            var _configYaml = new YamlStream();
            _configYaml.Load(_configStream);

            var _configDocument = _configYaml.Documents[0].RootNode as YamlMappingNode;
            var _configRoot = _configDocument.Children[0].Value as YamlSequenceNode;

            var _exeNames = new List<string>();

            foreach (YamlMappingNode _child in _configRoot.Children)
                _exeNames.Add(_child.Children[(YamlNode)"name"].ToString());

            API.Console.ConsolePrint("Searching for a compatible game... Please wait...", 0);

            while (true)
            {
                foreach (var _name in _exeNames)
                    _processArray = Process.GetProcessesByName(_name);

                if (_processArray.Length > 0x00)
                    break;
            };

            API.Console.ConsolePrint("Compatible game found! Executing...", 0);

            var _processGame = _processArray[0];
            var _processName = _processGame.ProcessName;

            var _configChild = new YamlMappingNode();

            foreach (YamlMappingNode _child in _configRoot.Children)
            {
                if (_child.Children[(YamlNode)"name"].ToString() == _processName)
                    _configChild = _child;
            }

            var _baseAddress = _configChild.Children[(YamlNode)"base_address"].ToString();
            var _memoryOffset = _configChild.Children[(YamlNode)"memory_offset"].ToString();

            Hypervisor.AttachProcess(_processGame, Convert.ToUInt64(_baseAddress, 16), Convert.ToUInt64(_memoryOffset, 16));
            Hook.SharpHook = new MemorySharp(Hypervisor.Process);

            var _scriptList = new List<string>();
            var _pathList = _configChild.Children[(YamlNode)"script_paths"] as YamlSequenceNode;

            foreach (var _path in _pathList.Children)
            {
                var _string = _path.ToString();
                _scriptList.AddRange(Directory.GetFiles(_string.Contains("$/") ? _string.Replace("$/", "scripts/") : _string));
            }

            foreach (var _s in _scriptList)
            {
                SysConsole.WriteLine("");

                API.Console.ConsolePrint("Initializing Script: " + Path.GetFileName(_s), 0);

                var _script = InitalizeScript(_s);
                var _initFunction = _script["_OnInit"] as LuaFunction;

                if (_initFunction != null)
                {
                    API.Console.ConsolePrint("Init Function Found on Script: " + Path.GetFileName(_s), 0);

                    try { _initFunction.Call(); }
                    catch (LuaScriptException _error)
                    { API.Console.ConsolePrint("An exception was caught -> " + _error.Message, 2); }
                }

                var _frameFunction = _script["_OnFrame"] as LuaFunction;

                if (_frameFunction != null)
                    API.Console.ConsolePrint("Frame Function Found on Script: " + Path.GetFileName(_s), 0);

                API.Console.ConsolePrint("Executing script...", 0);

                ScriptList.Add(_script);
            }

            foreach (var _s in ScriptList)
            {
                var _task = Task.Run(() =>
                {
                    var _frameFunction = _s["_OnFrame"] as LuaFunction;

                    if (_frameFunction != null)
                        while(true) 
                        { 
                            try { _frameFunction.Call(); }
                            catch (LuaScriptException _error) 
                            {
                                API.Console.ConsolePrint("An exception was caught! -> " + _error.Source + " " +  _error.Message, 2);
                                _frameFunction = null;
                            }
                        };
                });

                ThreadList.Add(_task);
            }

            SysConsole.ReadLine();
        }
    }
}