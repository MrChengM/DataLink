using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;

namespace TaskHandler.Config
{
    public class TaskConfig
    {
        /***************
        _id,_taskName,_drType,_tsType
        不能为空
        ***************/
        private string _id; 
        private int _createTimeOut;
        private int _initTimeOut;
        private int _startTimeOut;
        private int _stopTimeOut;
        private string _taskName;
        private CommunicationType _drType;
        private TaskType _tsType;
        private int _initLevel;

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }
        
        public int CreatedTimeOut
        {
            get { return _createTimeOut; }
            set { _createTimeOut = value; }
        }
        public int InitTimeOut
        {
            get { return _initTimeOut; }
            set { _initTimeOut = value; }
        }
        public int StartTimeOut
        {
            get { return _startTimeOut; }
            set { _startTimeOut = value; }
        }
        public int StopTimeOut
        {
            get { return _stopTimeOut; }
            set { _stopTimeOut = value; }
        }

        public string TaskName
        {
            get { return _taskName; }
            set { _taskName = value; }
        }

        public TaskType TsType
        {
            get { return _tsType; }
            set { _tsType = value; }
        }

        public CommunicationType DrType
        {
            get { return _drType; }
            set { _drType = value; }
        }
        public int InitLevel
        {
            get { return _initLevel; }
            set { _initLevel = value; }
        }
        public TaskConfig()
        {
            _createTimeOut = 10000; //10s
            _initTimeOut = 10000; //10s
            _startTimeOut = 10000;
            _stopTimeOut = 10000;
            _initLevel = 1;
        }
    }
}
