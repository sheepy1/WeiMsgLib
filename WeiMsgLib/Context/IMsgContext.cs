using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeiMsgLib.Message;

namespace WeiMsgLib.Context
{
    public interface IMsgContext
    {
        string UserName { get; set; }
        //最近一次发送请求的时间
        DateTime LastActiveTime { get; set; }
        MsgContainer<IRequestMsgBase> RequestMsgs { get; set; }
        MsgContainer<IResponseMsgBase> ResponseMsgs { get; set; }
        int MaxRecordCount { get; set; }
        //预留字段，框架使用者可用来存储临时数据
        object StorageData { get; set; }
        //用来注册用户上下文被移除时需要额外执行的操作
        event EventHandler<WeixinContextRemovedEventArgs> MsgContextRemoved;
        //移除用户会话上下文时调用的回调函数
        void OnRemoved();
    }
}