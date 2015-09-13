using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeiMsgLib.Message;
using System.Xml.Linq;
using WeiMsgLib.Utility;
using WeiMsgLib.Exceptions;

namespace WeiMsgLib.Factory
{
    public class RequestMsgFactory
    {
        public static IRequestMsgBase GetRequestEntity(XDocument xml)
        {
            RequestMsgBase requestMsg = null;
            RequestMsgType msgType;
            try
            {
                msgType = MsgTypeHelper.GetRequestMsgType(xml);
                switch (msgType)
                {
                    case RequestMsgType.Text:
                        requestMsg = new RequestTestMsg();
                        break;
                    case RequestMsgType.Location: 
                        requestMsg = new RequestLocationMsg();
                        break;
                    case RequestMsgType.Image:
                        requestMsg = new RequestImageMsg();
                        break;
                    case RequestMsgType.Voice: 
                        requestMsg = new RequestVoiceMsg();
                        break;
                    case RequestMsgType.Video:
                        requestMsg = new RequestVideoMsg();
                        break;
                    case RequestMsgType.Link:
                        requestMsg = new RequestLinkMsg();
                        break;
                    case RequestMsgType.Event:
                        var eventType = EventHelper.GetEventType(xml);
                        switch (eventType)
                        {
                            case Event.LOCATION:
                                requestMsg = new RequestLocationEventMsg();
                                break;
                            case Event.subscribe:
                                requestMsg = new RequestSubscribeEventMsg();
                                break;
                            case Event.unsubscribe:
                                requestMsg = new RequestUnsubscribeEventMsg();
                                break;
                            case Event.CLICK:
                                requestMsg = new RequestClickEventMsg();
                                break;
                            case Event.scan:
                                requestMsg = new RequestScanEventMsg();
                                break;
                            default:
                                requestMsg = new RequestEventMsgBase();
                                break;
                        }
                        break;
                    default:
                        throw new UnknowRequestMsgTypeException(string.Format("MsgType {0} 在RequestMsgFactory中没有对应的处理程序.", msgType), new ArgumentOutOfRangeException());
                }
                //根据XML信息映射实体
                requestMsg.FillEntityWithXml(xml);
            }
            catch (ArgumentException ex)
            {
                throw new WeixinException(string.Format("RequestMsg映射失败，可能是MsgType不存在, XML: {0}", xml.ToString()), ex);
            }

            return requestMsg;
        }
    }
}