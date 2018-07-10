using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

namespace Ares.Common.Tool
{
    /// <summary>
    /// 条件触发队列: 当队列中的数据达到指定数量 或者达到指定的时间, 才触发事件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WakenQueue<T>
    {
        /// <summary>
        /// 等待前一个消息处理完才进行下一次唤醒通知操作
        /// </summary>
        private readonly bool _waitPreviousNotice = false;

        /// <summary>
        /// 休眠时间达到指定时间时唤醒, 秒
        /// </summary>
        private readonly int _wakenTimeout = 10;

        /// <summary>
        /// 当记录数达到 wakenRecords 值时, 唤醒
        /// </summary>
        private readonly int _wakenRecords = 1000;

        /// <summary>
        /// 数据队列
        /// </summary>
        private readonly ConcurrentQueue<T> _dataList;

        private readonly object _look = new object();
        private bool wakening = false;

        private readonly Timer _timer;

        /// <summary>
        /// 队列中记录总数
        /// </summary>
        /// <value></value>
        public int Count { get => _dataList.Count; }

        /// <summary>
        /// 唤醒通知
        /// </summary>
        public event Action<IList<T>> WakenNotice;

        public WakenQueue() : this(1000, 10)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wakenRecords">当记录数达到指定数量的时候触发事件</param>
        /// <param name="wakenTimeout">唤醒超时时间: 秒</param>
        /// <param name="waitPreviousNotice"> 是否等待前一个消息处理完才进行下一次唤醒通知操作, false 不等待</param>
        /// <returns></returns>
        public WakenQueue(int wakenRecords, int wakenTimeout, bool waitPreviousNotice = false)
        {
            _dataList = new ConcurrentQueue<T>();
            this._wakenRecords = wakenRecords;
            this._wakenTimeout = wakenTimeout;
            this._waitPreviousNotice = waitPreviousNotice;

            _timer = new Timer((state) =>
            {
                Waken(true);
            }, null, _wakenTimeout * 1000, _wakenTimeout * 1000);
        }

        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue(T item)
        {
            if (WakenNotice == null) return;
            _dataList.Enqueue(item);
            if (!wakening && _dataList.Count >= _wakenRecords) Waken(false);
        }

        /// <summary>
        /// 唤醒
        /// </summary>
        private void Waken(bool isTimeout)
        {
            if (wakening) return;
            lock (_look)
            {
                if (wakening || (!isTimeout && _dataList.Count < _wakenRecords)) return;

                wakening = true;
                Task.Run(() =>
                {
                    try
                    {
                        Wakening();
                    }
                    finally
                    {
                        wakening = false;
                    }
                });
            }
        }

        /// <summary>
        /// 唤醒中操作
        /// </summary>
        private void Wakening()
        {
            while (_dataList.Count > 0)
            {
                var list = new List<T>();
                while (list.Count < _wakenRecords && _dataList.TryDequeue(out var item))
                {
                    list.Add(item);
                }
                try
                {
                    if (!_waitPreviousNotice)
                    {
                        Task.Run(() => WakenNotice(list));
                    }
                    else
                    {
                        WakenNotice(list);
                    }
                }
                catch (System.Exception e)
                {
                    System.Console.WriteLine(e);
                }
            }
            _timer.Change(_wakenTimeout * 1000, _wakenTimeout * 1000);
        }
    }
}