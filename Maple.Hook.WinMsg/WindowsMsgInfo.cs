
namespace Maple.Hook.WinMsg
{
    public class WindowsMsgInfo
    {
        //   public nint Handle { set; get; }
        public uint Msg { set; get; }
        public nuint WParam { set; get; }
        public nint LParam { set; get; }

    }


    public class WindowsMsgInfo<T>(T data)  : WindowsMsgInfo
    {
 

        public T Data { set; get; } = data;
    }
}
