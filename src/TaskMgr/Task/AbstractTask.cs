using DataServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMgr.Task

{
    public abstract class AbstractTask : IDisposable
    {
        protected string _taskName;
        protected ILog _log;
        protected TimeOut _timeout;
        protected int _initLevel;
        public string TaskName
        {
            get { return _taskName; }
            set { _taskName = value; }
        }
        public ILog Log
        {
            get { return _log; }
            set { _log = value; }
        }
        public TimeOut TimeOut
        {
            get { return _timeout; }
            set { _timeout = value; }
        }
        public int InitLevel
        {
            get { return _initLevel; }
            set { _initLevel = value; }
        }
        public abstract bool OnInit();
        public abstract bool OnStart();
        public abstract bool OnStop();

        public abstract bool Restart();

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~AbstractTask() {
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
