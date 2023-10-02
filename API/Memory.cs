using NLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaEngine.API
{
    internal class Memory
    {
        public static void WriteArray(ulong Address, dynamic Value, bool Absolute = false)
        {
            if (Value is LuaTable)
            {
                var _value = (Value as LuaTable).Values.Cast<long>().ToArray();
                var _byteArray = new byte[_value.Length];

                for (int i = 0; i < _byteArray.Length; i++)
                    _byteArray[i] = Convert.ToByte(_value[i]);

                Hypervisor.WriteArray(Address, _byteArray, Absolute);
            }

            else
                Hypervisor.WriteArray(Address, Value, Absolute);
        }

        public static void WriteString(ulong Address, char[] Value, bool Absolute = false)
        {
            var _byteArray = new byte[Value.Length];

            for (int i = 0; i < _byteArray.Length; i++)
                _byteArray[i] = Convert.ToByte(Value[i]);

            Hypervisor.WriteArray(Address, _byteArray, Absolute);
        }

        public static void WriteBoolean(ulong Address, bool Value, bool Absolute = false) => Hypervisor.Write(Address, Value, Absolute);
        public static void WriteByte(ulong Address, byte Value, bool Absolute = false) => Hypervisor.Write(Address, Value, Absolute);
        public static void WriteShort(ulong Address, ushort Value, bool Absolute = false) => Hypervisor.Write(Address, Value, Absolute);
        public static void WriteInt(ulong Address, uint Value, bool Absolute = false) => Hypervisor.Write(Address, Value, Absolute);
        public static void WriteLong (ulong Address, ulong Value, bool Absolute = false) => Hypervisor.Write(Address, Value, Absolute);
        public static void WriteDouble(ulong Address, double Value, bool Absolute = false) => Hypervisor.Write(Address, Value, Absolute);
        public static void WriteFloat(ulong Address, float Value, bool Absolute = false) => Hypervisor.Write(Address, Value, Absolute);

        public static byte[] ReadArray(ulong Address, int Length, bool Absolute = false) => Hypervisor.ReadArray(Address, Length, Absolute);

        public static string ReadString(ulong Address, int Length, bool Absolute = false) => Encoding.Default.GetString(Hypervisor.ReadArray(Address, Length, Absolute));

        public static bool ReadBoolean(ulong Address, bool Absolute = false) => Hypervisor.Read<bool>(Address, Absolute);
        public static byte ReadByte(ulong Address, bool Absolute = false) => Hypervisor.Read<byte>(Address, Absolute);
        public static ushort ReadShort(ulong Address, bool Absolute = false) => Hypervisor.Read<ushort>(Address, Absolute);
        public static uint ReadInt(ulong Address, bool Absolute = false) => Hypervisor.Read<uint>(Address, Absolute);
        public static ulong ReadLong(ulong Address, bool Absolute = false) => Hypervisor.Read<ulong>(Address, Absolute);
        public static double ReadDouble(ulong Address, bool Absolute = false) => Hypervisor.Read<double>(Address, Absolute);
        public static float ReadFloat(ulong Address, bool Absolute = false) => Hypervisor.Read<float>(Address, Absolute);
    }
}
