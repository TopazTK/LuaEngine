## LuaEngine Cheatsheet v6.00

---

## Global Variables

- ``GAME_ID`` => Returns the current game's CRC32 checksum as an integer. This checksum is generated from the process name at this moment.
- ``LUA_NAME`` => The name of your Lua script, used by functions such as ConsolePrint. It is the filename of your Lua by default.
- ``SCRIPT_PATH`` => Returns the path which Lua scripts are loaded from as a string. If multiple pats are detected, returns the first path. Ex: ``"C:\Users\Topaz\Desktop\LuaBackend\scripts"``
- ``CHEATS_PATH`` => Always returns ``"NOT_AVAILABLE"``
- ``EXE_ADDRESS`` => Returns the EXE's base address without any alterations.
- ``BASE_ADDRESS`` => Returns the EXE's base address altered by the given offset.
- ``MEMORY_OFFSET`` => Returns the EXE's base address bitwised with a given pattern, for instance ``EXE_ADDRESS & 0xFFFF00000000 == 0x7FF600000000``.
- ``ENGINE_VERSION`` => Returns the LuaBackend's engine version as a float. For this instance: ``6``.
- ``ENGINE_TYPE`` => Always returns ``"ENGINE_REDUX"``

---

## Memory Functions

### Read\[XXXX\](Address, Absolute = false) / ReadString(Address, Length, Absolute = false)

Reads a certain value from **Address**. Returns the read value. If reading a String, length must also be declared.  
The "**XXXX**" is a placeholder for the value type.  
  
If **Absolute** is true, the address is taken as written and base address is not added into it.  
  
List of types available: Byte, Short, Int, Long, Float, String, Boolean

Example:
```lua
    local _tempInt = ReadInt(0xDEADB00) -- Read an integer from address BaseAddress+0xDEADB00
    local _tempByte = ReadByte(0xB007555) -- Read a byte from address BaseAddress+0xB007555
```

### Write\[XXXX\](Address, Value, Abolsute = false)

Writes **Value** to **Address** calculated relative to the base address. 
The "**\[XXXX\]**" is a placeholder for the value type.  
  
If **Absolute** is true, the address is taken as written and base address is not added into it.  
  
List of types available: Byte, Short, Int, Long, Float, String, Boolean

Example:
```lua
    WriteShort(0xDEADB00, 0xFFFF) -- Write 0xFFFF to BaseAddress+0xDEADB00
    WriteBoolean(0xB007555, true) -- Write true to BaseAddress+0xB007555
```


### ReadArray(Address, Length, Absolute = false)

Reads **Length** amount of bytes from memory, starting at **Address**. Returns the read bytes as an array.  
If **Absolute** is true, the address is taken as written and base address is not added into it.  

Example:
```lua
    local _tempArray = ReadArray(0xBEEFDED, 0x10) -- Reads 16 bytes starting at BaseAddress+0xBEEFDED
```

### WriteArray(Address, Array, Absolute = false)

Writes **Array** to the memory, starting at **Address**. The array must be made of bytes (8-bit integers).  
If **Absolute** is true, the address is taken as written and base address is not added into it.  

Example:
```lua
    local _tempArray = { 0xFE, 0xA5, 0x70 }
    WriteArray(0x6660420, _tempArray) -- Writes _tempArray to memory, starting at BaseAddress+0x6660420
```

---

## Game Control Functions

### CallFunction(Address, Arguments = null)

Calls the function from the game that resides at **Address**.  
Calls can be made without any parameters, or with many.  

```lua
    CallFunction(0x1571D0, EXE_ADDRESS + 0x800000) -- Calls the function at 0x1571D0, with EXE_ADDRESS + 0x800000 as it's argument.
```

### CallReturn(Address, Arguments = null)

Calls the function from the game that resides at **Address**, and returns it's return value.    
Calls can be made without any parameters, or with many.  

```lua
    local _soraPoint = CallReturn(0x03B16F0) -- Calls the function at 0x03B16F0, and stores it's return to _soraPoint.
```

### JumpFunction(Address, Arguments = null)

Jumps to the function from the game that resides at **Address**.  
Jumps can be made without any parameters, or with many.  
Only use this if calling the said function causes issues.

```lua
  local _soraPoint = CallReturn(0x03B16F0)
  JumpFunction(0x3D0720, MEMORY_OFFSET + _soraPoint, -120, 0, true)
```

---

## Console Functions

### ConsolePrint(Text, Type = NULL)

Prints **Text** to the console, in the format given below. 

**Type** can be the following:
- 0 = MESSAGE
- 1 = SUCCESS
- 2 = WARNING
- 3 = ERROR

Format:
```
[LUA_NAME] TYPE: Text
```

Example:
```lua
    LUA_NAME = "SomeDudeScript"
    ConsolePrint("NICE VIEW FROM UP HERE!", 0) -- Prints the following: MESSAGE: NICE VIEW FROM UP HERE!
```

#### * Any functions present in this document signify compatibility with the latest version. Any function absent in this document is no longer supported and should be considered obsolete.
