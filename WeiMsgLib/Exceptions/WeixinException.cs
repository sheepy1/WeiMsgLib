using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeiMsgLib.Exceptions
{
    public class WeixinException : ApplicationException
    {
        public WeixinException(string msg) : base(msg, null) { }

        public WeixinException(string msg, Exception inner) : base(msg, inner) { }
    }
}