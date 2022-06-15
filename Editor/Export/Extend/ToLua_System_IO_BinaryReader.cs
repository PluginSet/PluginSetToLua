using LuaInterface;

public class ToLua_System_IO_BinaryReader
{

    public static string ReadDefined =
@"      try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 1)
			{
				System.IO.BinaryReader obj = (System.IO.BinaryReader)ToLua.CheckObject<System.IO.BinaryReader>(L, 1);
				int o = obj.Read();
				LuaDLL.lua_pushinteger(L, o);
				return 1;
			}
			else if (count == 4 && TypeChecker.CheckTypes<char[], int, int>(L, 2))
			{
				System.IO.BinaryReader obj = (System.IO.BinaryReader)ToLua.CheckObject<System.IO.BinaryReader>(L, 1);
				char[] arg0 = ToLua.CheckCharBuffer(L, 2);
				int arg1 = (int)LuaDLL.lua_tonumber(L, 3);
				int arg2 = (int)LuaDLL.lua_tonumber(L, 4);
				int o = obj.Read(arg0, arg1, arg2);
				LuaDLL.lua_pushinteger(L, o);
				return 1;
			}
			else if (count == 4 && TypeChecker.CheckTypes<byte[], int, int>(L, 2))
			{
				System.IO.BinaryReader obj = (System.IO.BinaryReader)ToLua.CheckObject<System.IO.BinaryReader>(L, 1);
				byte[] arg0 = ToLua.CheckByteBuffer(L, 2);
				int arg1 = (int)LuaDLL.lua_tonumber(L, 3);
				int arg2 = (int)LuaDLL.lua_tonumber(L, 4);
				int o = obj.Read(arg0, arg1, arg2);
				LuaDLL.lua_pushinteger(L, o);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, ""invalid arguments to method: System.IO.BinaryReader.Read"");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
";
    
    
    [UseDefinedAttribute]
    public static int Read()
    {
	    return 0;
    }
}
