using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeiMsgLib.Exceptions
{
    public class UnknowRequestMsgTypeException : WeixinException
    {
        public UnknowRequestMsgTypeException(string msg) : base(msg, null) { }

        public UnknowRequestMsgTypeException(string msg, Exception inner) : base(msg, inner) { }
    }
}