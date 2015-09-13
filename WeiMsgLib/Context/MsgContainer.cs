using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeiMsgLib.Context
{
    public class MsgContainer<T> : List<T>
    {
        //最大消息数，若为0则无限制
        public int MaxRecordCount { get; set; }
        public MsgContainer(int maxRecordCount)
        {
            MaxRecordCount = maxRecordCount;
        }
        //基类的下列方法都不是virtual方法，不允许override，只能new
        new public void Add(T item)
        {
            base.Add(item);
            RemoveEarlyItems();
        }

        new public void AddRange(IEnumerable<T> collection)
        {
            base.AddRange(collection);
            RemoveEarlyItems();
        }

        new public void Insert(int index, T item)
        {
            base.Insert(index, item);
            RemoveEarlyItems();
        }

        new public void InsertRange(int index, IEnumerable<T> collection)
        {
            base.InsertRange(index, collection);
            RemoveEarlyItems();
        }
        //移除超出数量限制的最早消息
        private void RemoveEarlyItems()
        {
            if (MaxRecordCount > 0 && base.Count > MaxRecordCount)
            {
                base.RemoveRange(0, base.Count - MaxRecordCount);
            }
        }
    }
}