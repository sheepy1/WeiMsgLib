using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeiMsgLib.Message;
using System.Xml.Linq;

namespace WeiMsgLib.Utility
{
    public class EventHelper
    {
        public static Event GetEventType(XDocument xml)
        {
            return GetEventType(xml.Root.Element("Event").Value);
        }

        public static Event GetEventType(string eventStr)
        {
            return (Event)Enum.Parse(typeof(Event), eventStr, true);
        }
    }
}