using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeiMsgLib
{
    //消息格式接口，实现接口的类必须实现内部属性（用来规范数据格式）
    public interface IMsgBase
    {
        string ToUserName { get; set; }
        string FromUserName { get; set; }
        DateTime CreateTime { get; set; }   
    }
    //消息基类,子类不能override内部的属性。（默认格式）
    public class MsgBase
    {
        public string ToUserName { get; set; }
        public string FromUserName { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
