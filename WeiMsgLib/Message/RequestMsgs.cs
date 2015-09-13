using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeiMsgLib.Message
{
    public class RequestTestMsg : RequestMsgBase
    {
        public string Content { get; set; }
    }

    public class RequestImageMsg : RequestMsgBase
    {
        public override RequestMsgType MsgType
        {
            get
            {
                return RequestMsgType.Image;
            }
        }
       
        public string MediaId { get; set; }
        public string PicUrl { get; set; }
    }

    public class RequestVoiceMsg : RequestMsgBase
    {
        public override RequestMsgType MsgType
        {
            get
            {
                return RequestMsgType.Voice;
            }
        }

        public string MediaId { get; set; }
        //语音格式:amr
        public string Format { get; set; }
        //语音识别结果,UTF8编码
        public string Recognition { get; set; }
    }

    public class RequestVideoMsg : RequestMsgBase
    {
        public override RequestMsgType MsgType
        {
            get { return RequestMsgType.Video; }
        }

        public string MediaId { get; set; }
        public string ThumbMediaId { get; set; }
    }

    public class RequestLocationMsg : RequestMsgBase
    {
        public override RequestMsgType MsgType
        {
            get
            {
                return RequestMsgType.Location;
            }
        }

        //纬度
        public double Location_X { get; set; }
        //经度
        public double Location_Y { get; set; }
        public int Scale { get; set; }
        public string Label { get; set; }
    }

    public class RequestLinkMsg : RequestMsgBase
    {
        public override RequestMsgType MsgType
        {
            get
            {
                return RequestMsgType.Link;
            }
        }

        public string Title { get; set; }
        public string Descripthon { get; set; }
        public string Url { get; set; }
    }
}