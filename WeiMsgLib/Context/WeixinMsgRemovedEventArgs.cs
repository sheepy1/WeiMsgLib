using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeiMsgLib.Context
{
    public class WeixinContextRemovedEventArgs : EventArgs
    {
        public string OpenId
        {
            get
            {
                return MsgContext.UserName;
            }
        }

        public DateTime LastActiveTime
        {
            get
            {
                return MsgContext.LastActiveTime;
            }
        }
        //用户上下文对象
        public IMsgContext MsgContext { get; set; }
        public WeixinContextRemovedEventArgs(IMsgContext msgContext)
        {
            MsgContext = msgContext;
        }
    }
}