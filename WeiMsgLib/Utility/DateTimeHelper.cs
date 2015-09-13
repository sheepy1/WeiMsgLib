using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeiMsgLib.Utility
{
    public class DateTimeHelper
    {
        //Unix起始时间
        public static DateTime BaseTime = new DateTime(1970, 1, 1);

        //转换微信DateTime到C#时间
        public static DateTime GetDateTimeFromXml(long datetimeFromXml)
        {
            //datetimeFromXml表示从格林威治标准时间1970-01-01 00:00:00到当前时间的秒数,加8个小时转化为北京时间，毕竟吾等所在为东八区！
            return BaseTime.AddTicks((datetimeFromXml + 8 * 60 * 60) * 10000000);//1后面7个0
        }

        public static DateTime GetDateTimeFromXml(string dateTimeFromXml)
        {
            return GetDateTimeFromXml(long.Parse(dateTimeFromXml));
        }

        //获取微信DateTime
        public static long GetWeixinDateTime(DateTime dateTime) {
            return (dateTime.Ticks - BaseTime.Ticks) / 10000000 - 8 * 60 * 60;
        }
    }
}