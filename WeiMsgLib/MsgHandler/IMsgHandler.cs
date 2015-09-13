using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using WeiMsgLib.Message;

namespace WeiMsgLib.MsgHandler
{
    public interface IMsgHandler
    {
        //发送者用户名
        string OpenId { get; }
        //为True时接下来的操作都不执行
        bool CancelExecute { get; set; }
        //在构造器中得到请求XML
        XDocument RequestXml { get; set; }
        //每次请求都根据当前的ResponseMsg生成，若需重用可使用缓存
        XDocument ResponseXml { get;}
        IRequestMsgBase RequestMsg { get; set; }
        //Execute()后得到
        IResponseMsgBase ResponseMsg { get; set; }
        //根据不同的消息类型进行处理
        void Execute();
        //消息处理前的操作
        void PreExecute();
        //消息处理后的操作
        void PostExecute();

    }
}