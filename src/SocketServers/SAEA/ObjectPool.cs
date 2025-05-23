﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace SocketServers.SAEA
{
    public abstract class  ObjectPool<T> :IDisposable where T:class
    {

        private ConcurrentQueue<T> _queue;

        public ConcurrentQueue<T> Queue => _queue;
        public ObjectPool()
        {
            _queue = new ConcurrentQueue<T>();
        }

        protected abstract T Create();

        public bool IsEmpty
        {
            get {return _queue.IsEmpty; }
        }

        public int Count
        {
            get { return _queue.Count; }
        }

        public virtual void Add(T item)
        {
            _queue.Enqueue(item);
        }
        public virtual T Get()
        {
            T item;
            return _queue.TryDequeue(out item) ? item : Create();
        }
        public virtual void Return(T obj)
        {
            if(obj!=null)
            _queue.Enqueue(obj);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    var item = Get() as IDisposable;
                    if (item != null)
                    {
                        item.Dispose();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ObjectPool() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
