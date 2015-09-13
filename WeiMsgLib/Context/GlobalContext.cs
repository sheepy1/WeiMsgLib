using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeiMsgLib.Message;

namespace WeiMsgLib.Context
{
    
    public static class GlobalContextCtrl
    {
        public static object Lock = new object();
        //是否开启上下文记录，有些场景不需要上下文，关闭可提高性能
        public static bool UseGlobalContext = true;
    }

    //全局上下文
    public class GlobalContext<M> where M : class, IMsgContext, new()
    {
        //默认过期时间：90分钟
        private const int DEFAULT_EXPIRE_MINUTES = 90;
        //所有MsgContext集合，用字典存储
        protected Dictionary<string, M> MsgCollection { get; set; } 
        //按LastActiveTime升序排序，越前面表示连接时间越早，故只需判断前几个是否需要删除即可
        protected List<M> MsgQueue { get; set; }    
        //MsgContext过期时间
        public Double ExpireMinutes { get; set; }   
        //最大会话数量
        public int MaxRecordCount { get; set; }
        //重置
        public GlobalContext()
        {
            Restore();
        }
        //清空
        public void Restore()
        {
            MsgCollection = new Dictionary<string,M>(StringComparer.OrdinalIgnoreCase);//排序字典
            MsgQueue = new List<M>();
            ExpireMinutes = DEFAULT_EXPIRE_MINUTES;
        }
        //获取MsgContext，同时移除过期上下文
        private M GetMsgContext(string userName)
        {
            //检查并移除过期记录，为了尽量节约资源，暂不使用独立线程轮询
            foreach(var msgContext in MsgQueue)
            {
                //var msgContext = MsgQueue[0];
                var timeInterval = DateTime.Now - msgContext.LastActiveTime;
                if (timeInterval.TotalMinutes >= ExpireMinutes)
                {
                    MsgQueue.RemoveAt(0);
                    MsgCollection.Remove(msgContext.UserName);

                    //事件回调
                    msgContext.OnRemoved();//Todo:可进行异步处理
                }
                else
                {
                    break;
                }
            }

            if (!MsgCollection.ContainsKey(userName))
            {
                return null;
            }
            return MsgCollection[userName];
        }

        private M GetMsgContext(string userName, bool createIfNotExist)
        {
            var msgContext = GetMsgContext(userName);

            if (msgContext == null && createIfNotExist)
            {
                MsgCollection[userName] = new M
                {
                    UserName = userName,
                    MaxRecordCount = MaxRecordCount
                };

                msgContext = GetMsgContext(userName);
                MsgQueue.Add(msgContext);//最新的排到末尾
            }

            return msgContext;
        }
        
        public M GetMsgContext(IRequestMsgBase requestMsg)
        {
            //线程安全：GlobalContextCtrl.Lock作为monitor，只能被一个线程持有
            lock (GlobalContextCtrl.Lock)
            {
                return GetMsgContext(requestMsg.FromUserName, true);
            }
        }

        public M GetMsgContext(IResponseMsgBase responseMsg)
        {
            lock (GlobalContextCtrl.Lock)
            {
                return GetMsgContext(responseMsg.ToUserName, true);
            }
        }

        public void InsertMsg(IRequestMsgBase requestMsg)
        {
            lock (GlobalContextCtrl.Lock)
            {
                var userName = requestMsg.FromUserName;
                var msgContext = GetMsgContext(userName, true);
                //若队列中已存在该用户消息上下文，先从队列中删除旧的上下文
                if (msgContext.RequestMsgs.Count > 0)
                {
                    var msgContextIndexInQueue = MsgQueue.FindIndex(m => m.UserName == userName);
                    if (msgContextIndexInQueue >= 0)
                    {
                        MsgQueue.RemoveAt(msgContextIndexInQueue);
                    }
                }
                MsgQueue.Add(msgContext);//加入到队列末尾
                msgContext.LastActiveTime = DateTime.Now;
                msgContext.RequestMsgs.Add(requestMsg);
            }
        }

        public void InsertMsg(IResponseMsgBase responseMsg)
        {
            lock (GlobalContextCtrl.Lock)
            {
                var msgContext = GetMsgContext(responseMsg.ToUserName, true);
                msgContext.ResponseMsgs.Add(responseMsg);
            }
        }
        //获取最新请求数据，不存在则返回null
        public IRequestMsgBase GetLastRequestMsg(string userName)
        {
            lock (GlobalContextCtrl.Lock)
            {
                var msgContext = GetMsgContext(userName, true);
                return msgContext.RequestMsgs.LastOrDefault();
            }
        }

        public IResponseMsgBase GetLastResponseMsg(string userName)
        {
            lock (GlobalContextCtrl.Lock)
            {
                var msgContext = GetMsgContext(userName, true);
                return msgContext.ResponseMsgs.LastOrDefault();
            }
        }
    }
}