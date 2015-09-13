using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeiMsgLib.Message
{
    public interface IRequestEventMsgBase : IRequestMsgBase
    {
        //事件类型
        Event Event { get; }
        //事件Key值，对应自定义菜单接口中的Key值
        string EventKey { get; set; }

    }
    //用户发送事件消息基类
    public class RequestEventMsgBase : RequestMsgBase, IRequestEventMsgBase
    {
        public override RequestMsgType MsgType
        {
            get
            {
                return RequestMsgType.Event;
            }
        }

        public virtual Event Event
        {
            get
            {
                return Event.CLICK;
            }
        }

        public string EventKey { get; set; }
    }
}