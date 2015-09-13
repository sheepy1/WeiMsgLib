using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeiMsgLib.Message;
using System.Xml.Linq;

namespace WeiMsgLib.Utility
{
    public class MsgTypeHelper
    {
        public static RequestMsgType GetRequestMsgType(XDocument xml)
        {
            return GetRequestMsgType(xml.Root.Element("MsgType").Value);
        }

        public static RequestMsgType GetRequestMsgType(string typeStr)
        {
            //将xml节点中MsgType的字符串值映射到对应RequestMsgType类型
            return (RequestMsgType)Enum.Parse(typeof(RequestMsgType), typeStr, true);
        }
    }
}