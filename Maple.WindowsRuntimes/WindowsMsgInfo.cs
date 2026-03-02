namespace Maple.WindowsRuntimes
{
    public class WindowsMsgInfo
    {
        //   public nint Handle { set; get; }
        public EnumWindowMsgCode Msg { set; get; }
        public nint WParam { set; get; }
        public nint LParam { set; get; }

    }


    public class WindowsMsgInfo<T>(T data)  : WindowsMsgInfo
    {
 

        public T Data { set; get; } = data;
    }
}
