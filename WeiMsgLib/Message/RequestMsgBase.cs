using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeiMsgLib.Message
{
    public interface IRequestMsgBase : IMsgBase
    {
        //消息类型
        RequestMsgType MsgType { get; }
        //消息Id
        long MsgId { get; set; }
    }

    public class RequestMsgBase : MsgBase, IRequestMsgBase
    {
        //可以被子类override
        public virtual RequestMsgType MsgType
        {
            get 
            { 
                return RequestMsgType.Text; 
            }
        }

        public long MsgId { get; set; }
    }
}