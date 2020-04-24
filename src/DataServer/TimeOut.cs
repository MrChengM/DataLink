using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer
{
    public class TimeOut
    {

        string _taskName;
        DateTime _startTime;
        DateTime _endTime;
        long _durationTime;
        long _timeOutSet;
        bool _timeOutFlag = false;
        ILog _log;

        public string TaskName
        {
            get
            {
                return _taskName;
            }
            set
            {
                _taskName =value;
            }
        }
        public DateTime StartTime {
            get
            {
                return _startTime;
            }
            set
            {
                _startTime=value;
            }
        }
        public DateTime EndTime
        {
            get
            {
                return _endTime;
            }
            set
            {
                _durationTime = (long)((EndTime - StartTime).TotalMilliseconds);
                _timeOutFlag = _durationTime >= TimeOutSet ? true : false;
                _endTime =value;
            }
        }
        public long DurationTime
        {
            get
            {
                return _durationTime;
            }
        }
        public long TimeOutSet
        {
            get
            {
                return _timeOutSet;
            }
            set
            {
                _timeOutSet = value;
            }
        }
        public bool TimeOutFlag {
            get
            {
                return _timeOutFlag;
            }
           private set
            {
                _timeOutFlag = value;
            }
        }
        public ILog Log
        {
            get
            {
                return _log;
            }
            set
            {
                _log = value;
            }
        }
        public TimeOut() { }
        public TimeOut(string taskName,long timeOutSet,ILog log)
        {
            _taskName = taskName;
            _timeOutSet = timeOutSet;
            _log = log;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            _startTime = DateTime.Now;
            _endTime = DateTime.Now;
            _durationTime = 0;
            _timeOutFlag = false;
        }
        /// <summary>
        /// TimeOut时间Log记录
        /// </summary>
        public void LogTimeOutError()
        {
            _log.ErrorLog(string.Format("{0}Time Out.", _taskName));
        }
     
    }
}
