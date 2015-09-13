using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeiMsgLib.Message
{
    public enum RequestMsgType
    {
        Text,
        Location,
        Image,
        Voice,
        Video,
        Link,
        Event
    }

    public enum Event
    {
        //上报地理位置事件
        LOCATION,
        //关注
        subscribe,
        //取关
        unsubscribe,
        //自定义菜单点击事件
        CLICK,
        //扫描二维码
        scan
    }

    public enum ResponseMsgType
    {
        Text,
        //图文
        News,
        Music,
        Image,
        Voice,
        Video
    }
}