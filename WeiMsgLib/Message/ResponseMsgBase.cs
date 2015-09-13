using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeiMsgLib.Message
{
    public interface IResponseMsgBase : IMsgBase
    {
        ResponseMsgType MsgType { get; }
    }

    public class ResponseMsgBase : MsgBase, IResponseMsgBase 
    {
        public virtual ResponseMsgType MsgType
        {
            get
            {
                return ResponseMsgType.Text;
            }
        }
    }
}