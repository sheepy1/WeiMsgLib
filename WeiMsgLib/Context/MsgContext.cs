using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeiMsgLib.Message;

namespace WeiMsgLib.Context
{
    public class MsgContext : IMsgContext
    {
        private int _maxRecordCount;
        public string UserName { get; set; }
        public DateTime LastActiveTime { get; set; }
        public MsgContainer<IRequestMsgBase> RequestMsgs { get; set; }
        public MsgContainer<IResponseMsgBase> ResponseMsgs { get; set; }
        public int MaxRecordCount
        {
            get
            {
                return _maxRecordCount;
            }
            set
            {
                RequestMsgs.MaxRecordCount = value;
                ResponseMsgs.MaxRecordCount = value;
                _maxRecordCount = value;
            }
        }

        public object StorageData { get; set; }
        public event EventHandler<WeixinContextRemovedEventArgs> MsgContextRemoved = null;

        public MsgContext()
        {
            RequestMsgs = new MsgContainer<IRequestMsgBase>(MaxRecordCount);
            ResponseMsgs = new MsgContainer<IResponseMsgBase>(MaxRecordCount);
            LastActiveTime = DateTime.Now;
        }

        public virtual void OnRemoved()
        {
            var OnRemovedArg = new WeixinContextRemovedEventArgs(this);
            OnMsgContextRemoved(OnRemovedArg);
        }

        private void OnMsgContextRemoved(WeixinContextRemovedEventArgs e)
        {
            if (MsgContextRemoved != null)
            {
                MsgContextRemoved(this, e);
            }

        }
    }
}