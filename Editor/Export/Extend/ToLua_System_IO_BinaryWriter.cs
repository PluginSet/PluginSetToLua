using LuaInterface;

public class ToLua_System_IO_BinaryWriter
{
    public static string WriteDefined =
@"      try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2 && TypeChecker.CheckTypes<bool>(L, 2))
			{
				System.IO.BinaryWriter obj = (System.IO.BinaryWriter)ToLua.CheckObject<System.IO.BinaryWriter>(L, 1);
				bool arg0 = LuaDLL.lua_toboolean(L, 2);
				obj.Write(arg0);
				return 0;
			}
			else if (count == 2 && TypeChecker.CheckTypes<double>(L, 2))
			{
				System.IO.BinaryWriter obj = (System.IO.BinaryWriter)ToLua.CheckObject<System.IO.BinaryWriter>(L, 1);
				double arg0 = (double)LuaDLL.lua_tonumber(L, 2);
				obj.Write(arg0);
				return 0;
			}
			else if (count == 2 && TypeChecker.CheckTypes<long>(L, 2))
			{
				System.IO.BinaryWriter obj = (System.IO.BinaryWriter)ToLua.CheckObject<System.IO.BinaryWriter>(L, 1);
				long arg0 = LuaDLL.tolua_toint64(L, 2);
				obj.Write(arg0);
				return 0;
			}
			else if (count == 2 && TypeChecker.CheckTypes<ulong>(L, 2))
			{
				System.IO.BinaryWriter obj = (System.IO.BinaryWriter)ToLua.CheckObject<System.IO.BinaryWriter>(L, 1);
				ulong arg0 = LuaDLL.tolua_touint64(L, 2);
				obj.Write(arg0);
				return 0;
			}
			else if (count == 2 && TypeChecker.CheckTypes<byte[]>(L, 2))
			{
				System.IO.BinaryWriter obj = (System.IO.BinaryWriter)ToLua.CheckObject<System.IO.BinaryWriter>(L, 1);
				byte[] arg0 = ToLua.CheckByteBuffer(L, 2);
				obj.Write(arg0);
				return 0;
			}
			else if (count == 2 && TypeChecker.CheckTypes<char[]>(L, 2))
			{
				System.IO.BinaryWriter obj = (System.IO.BinaryWriter)ToLua.CheckObject<System.IO.BinaryWriter>(L, 1);
				char[] arg0 = ToLua.CheckCharBuffer(L, 2);
				obj.Write(arg0);
				return 0;
			}
			else if (count == 2 && TypeChecker.CheckTypes<string>(L, 2))
			{
				System.IO.BinaryWriter obj = (System.IO.BinaryWriter)ToLua.CheckObject<System.IO.BinaryWriter>(L, 1);
				string arg0 = ToLua.ToString(L, 2);
				obj.Write(arg0);
				return 0;
			}
			else if (count == 4 && TypeChecker.CheckTypes<byte[], int, int>(L, 2))
			{
				System.IO.BinaryWriter obj = (System.IO.BinaryWriter)ToLua.CheckObject<System.IO.BinaryWriter>(L, 1);
				byte[] arg0 = ToLua.CheckByteBuffer(L, 2);
				int arg1 = (int)LuaDLL.lua_tonumber(L, 3);
				int arg2 = (int)LuaDLL.lua_tonumber(L, 4);
				obj.Write(arg0, arg1, arg2);
				return 0;
			}
			else if (count == 4 && TypeChecker.CheckTypes<char[], int, int>(L, 2))
			{
				System.IO.BinaryWriter obj = (System.IO.BinaryWriter)ToLua.CheckObject<System.IO.BinaryWriter>(L, 1);
				char[] arg0 = ToLua.CheckCharBuffer(L, 2);
				int arg1 = (int)LuaDLL.lua_tonumber(L, 3);
				int arg2 = (int)LuaDLL.lua_tonumber(L, 4);
				obj.Write(arg0, arg1, arg2);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, ""invalid arguments to method: System.IO.BinaryWriter.Write"");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
";
    
    [UseDefinedAttribute]
    public static int Write(object any)
    {
        return 0;
    }
}
