using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeiMsgLib.Message;
using WeiMsgLib.Context;
using WeiMsgLib.Utility;
using System.Xml.Linq;
using System.IO;
using System.Xml;
using WeiMsgLib.Factory;
using WeiMsgLib.Exceptions;

namespace WeiMsgLib.MsgHandler
{
    public abstract class MsgHandler<T> : IMsgHandler where T : class, IMsgContext, new()
    {
        //protected与private类似，不同的是protected声明的变量和方法可以被子类实例调用（不能被自身实例调用）
        protected static GlobalContext<T> StaticGlobalContext = new GlobalContext<T>();

        protected GlobalContext<T> GlobalContext
        {
            get
            {
                return StaticGlobalContext;
            }
        }



        public IRequestMsgBase RequestMsg { get; set; }
        public IResponseMsgBase ResponseMsg { get; set; }

        //当前用户上下文
        public T CurrentMsgContext
        {
            get
            {
                return GlobalContext.GetMsgContext(RequestMsg);
            }
        }

        public string OpenId
        {
            get
            {
                if (RequestMsg != null)
                {
                    return RequestMsg.FromUserName;
                }
                return null;
            }
        }

        public bool CancelExecute { get; set; }
        public XDocument RequestXml { get; set; }
        public XDocument ResponseXml
        {
            get
            {
                if (ResponseMsg == null)
                {
                    return null;
                }
                return (ResponseMsg as ResponseMsgBase).ConvertEntityToXml();
            }
           
        }

        //构造器
        public MsgHandler(Stream inputStream, int maxRecordCount = 0)
        {
            GlobalContext.MaxRecordCount = maxRecordCount;
            using(XmlReader xr = XmlReader.Create(inputStream))
            {
                RequestXml = XDocument.Load(xr);
                Init(RequestXml);
            }
        }

        public MsgHandler(XDocument requestXml, int maxRecordCount = 0)
        {
            GlobalContext.MaxRecordCount = maxRecordCount;
            Init(requestXml);
        }

        private void Init(XDocument requestXml)
        {
            RequestXml = requestXml;

            RequestMsg = RequestMsgFactory.GetRequestEntity(RequestXml);

            if (GlobalContextCtrl.UseGlobalContext)
            {
                GlobalContext.InsertMsg(RequestMsg);
            }
        }

        public void Execute()
        {
            if (CancelExecute)
            {
                return;
            }

            //预处理
            PreExecute();

            if (CancelExecute)
            {
                return;
            }

            try
            {
                if (RequestMsg == null)
                {
                    return;
                }
                //根据消息类型进行不同处理
                switch (RequestMsg.MsgType)
                {
                    case RequestMsgType.Text:
                        ResponseMsg = HandleTextRequest(RequestMsg as RequestTestMsg);
                        break;
                    case RequestMsgType.Location:
                        ResponseMsg = HandleLocationRequest(RequestMsg as RequestLocationMsg);
                        break;
                    case RequestMsgType.Image:
                        ResponseMsg = HandleImageRequest(RequestMsg as RequestImageMsg);
                        break;
                    case RequestMsgType.Voice:
                        ResponseMsg = HandleVoiceRequest(RequestMsg as RequestVoiceMsg);
                        break;
                    case RequestMsgType.Video:
                        ResponseMsg = HandleVideoRequest(RequestMsg as RequestVideoMsg);
                        break;
                    case RequestMsgType.Link:
                        ResponseMsg = HandleLinkRequest(RequestMsg as RequestLinkMsg);
                        break;
                    case RequestMsgType.Event:
                        ResponseMsg = HandleEventRequest(RequestMsg as RequestEventMsgBase);
                        break;
                    default:
                        throw new UnknowRequestMsgTypeException("TypeError: 未知的MsgType");
                }

                //记录上下文
                if (GlobalContextCtrl.UseGlobalContext && ResponseMsg != null)
                {
                    GlobalContext.InsertMsg(ResponseMsg);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                PostExecute();
            }
        }

        public void PreExecute() { }
        public void PostExecute() { }
  
        protected R CreateResponseMsg<R>() where R : ResponseMsgBase
        {
            if (RequestMsg == null)
            {
                return null;
            }
            return RequestMsg.CreateResponseMsg<R>();
        }

        //默认返回消息
        protected virtual IResponseMsgBase GetDefaultResponseMsg()
        {
            var responseMsg = CreateResponseMsg<ResponseTextMsg>();
            responseMsg.Content = "您发送的消息类型未被识别。";
            return responseMsg;
        }

        #region 每种消息类型的具体处理函数,需被重写
        protected virtual IResponseMsgBase HandleTextRequest(RequestTestMsg requestMsg)
        {
            return GetDefaultResponseMsg();
        }

        protected virtual IResponseMsgBase HandleLocationRequest(RequestLocationMsg requestMsg)
        {
            return GetDefaultResponseMsg();
        }

        protected virtual IResponseMsgBase HandleImageRequest(RequestImageMsg requestMsg)
        {
            return GetDefaultResponseMsg();
        }

        protected virtual IResponseMsgBase HandleVoiceRequest(RequestVoiceMsg requestMsg)
        {
            return GetDefaultResponseMsg();
        }

        protected virtual IResponseMsgBase HandleVideoRequest(RequestVideoMsg requestMsg)
        {
            return GetDefaultResponseMsg();
        }

        protected virtual IResponseMsgBase HandleLinkRequest(RequestLinkMsg requestMsg)
        {
            return GetDefaultResponseMsg();
        }

        protected virtual IResponseMsgBase HandleEventRequest(IRequestEventMsgBase requestMsg)
        {
            IResponseMsgBase responseMsg = null;
            switch (requestMsg.Event)
            {
                case Event.LOCATION:
                    responseMsg = HandleLocationEvent(requestMsg as RequestLocationEventMsg);
                    break;
                case Event.subscribe:
                    responseMsg = HandleSubscribeEvent(requestMsg as RequestSubscribeEventMsg);
                    break;
                case Event.unsubscribe:
                    responseMsg = HandleUnsubscribeEvent(requestMsg as RequestUnsubscribeEventMsg);
                    break;
                case Event.CLICK:
                    responseMsg = HandleClickEvent(requestMsg as RequestClickEventMsg);
                    break;
                case Event.scan:
                    responseMsg = HandleScanEvent(requestMsg as RequestScanEventMsg);
                    break;
                default:
                    throw new UnknowRequestMsgTypeException("EventTypeError：未知的Event类型.");
            }

            return responseMsg;
        }
        
        //每种事件类型请求的处理函数
        protected virtual IResponseMsgBase HandleLocationEvent(RequestLocationEventMsg requestMsg)
        {
            return GetDefaultResponseMsg();
        }

        protected virtual IResponseMsgBase HandleSubscribeEvent(RequestSubscribeEventMsg requestMsg)
        {
            return GetDefaultResponseMsg();
        }

        protected virtual IResponseMsgBase HandleUnsubscribeEvent(RequestUnsubscribeEventMsg requestMsg)
        {
            return GetDefaultResponseMsg();
        }

        protected virtual IResponseMsgBase HandleClickEvent(RequestClickEventMsg requestMsg)
        {
            return GetDefaultResponseMsg();
        }

        protected virtual IResponseMsgBase HandleScanEvent(RequestScanEventMsg requestMsg)
        {
            return GetDefaultResponseMsg();
        }

        #endregion

    }
}