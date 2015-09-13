using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeiMsgLib.Message
{
    public class RequestSubscribeEventMsg : RequestEventMsgBase
    {
        public override Event Event
        {
            get
            {
                return Event.subscribe;
            }
        }
    }

    public class RequestUnsubscribeEventMsg : RequestEventMsgBase
    {
        public override Event Event
        {
            get
            {
                return Event.unsubscribe;
            }
        }
    }

    public class RequestScanEventMsg : RequestEventMsgBase
    {
        public override Event Event
        {
            get
            {
                return Event.scan;
            }
        }

        //二维码参数
        public string Ticket { get; set; }
    }

    public class RequestLocationEventMsg : RequestEventMsgBase
    {
        public override Event Event
        {
            get
            {
                return Event.LOCATION;
            }
        }
        
        //纬度
        public double Latitude { get; set; }
        //经度
        public double Longitude { get; set; }
        //精度
        public double Precision { get; set; }
    }

    public class RequestClickEventMsg : RequestEventMsgBase
    {
        public override Event Event
        {
            get
            {
                return base.Event;
            }
        }
    }
}