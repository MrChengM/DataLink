using log4net;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataServer.Log
{
    public interface ILog
    {

        void Init(string format, params object[] parameters);
        /// <summary>Logs a debug log
        /// </summary>
        /// <param name="format">Log string (Example:This is the log {0},{1})</param>
        /// <param name="parameters">Parameters for the log string</param>
        void DebugLog(string format, params object[] parameters);

        ///  <summary>Logs a error log
        /// <para> A possible existing exception object can be passed as the last argument in parameters</para>
        ///  </summary>
        ///  <param name="format">Log string (Example:This is the log {0},{1})</param>
        ///  <param name="parameters">Parameters for the log string and/or Exception</param>
        void ErrorLog(string format, params object[] parameters);

        /// <summary>Logs a normal log
        /// </summary>
        /// <param name="format">Log string (Example:This is the log {0},{1})</param>
        /// <param name="parameters">Parameters for the log string</param>
        void InfoLog(string format, params object[] parameters);

        /// <summary>
        /// Logs a fata log
        /// </summary>
        /// <param name="format"></param>
        /// <param name="parameters"></param>
        void FatalLog(string format, params object[] parameters);
        ///  <summary>Logs a warning log
        /// <para> A possible existing exception object can be passed as the last argument in parameters</para>
        ///  </summary>
        ///  <param name="format">Log string (Example:This is the log {0},{1})</param>
        ///  <param name="parameters">Parameters for the log string and/or Exception</param>
        void WarningLog(string format, params object[] parameters);
        event Action<LogLevel,string> LogNotifyEvent;

    }
    public enum LogLevel
    {
        Debug,
        Info,
        Warn,
        Error,
        Fatal

    }
    public class Log4netWrapper : ILog
    {
        private log4net.ILog log;

        public event Action<LogLevel,string> LogNotifyEvent;
        public void DebugLog(string format, params object[] parameters)
        {
            var message = string.Format(format, parameters);
            log.Debug(message);
            rasieLogNotifyEvent(LogLevel.Debug, message);
        }

        public void ErrorLog(string format, params object[] parameters)
        {
            var message = string.Format(format, parameters);
            log.Error(string.Format(format, parameters));
            rasieLogNotifyEvent(LogLevel.Error, message);
        }

        public void FatalLog(string format, params object[] parameters)
        {
            var message = string.Format(format, parameters);
            log.Fatal(string.Format(format, parameters));
            rasieLogNotifyEvent(LogLevel.Fatal, message);

        }

        public void InfoLog(string format, params object[] parameters)
        {
            var message = string.Format(format, parameters);
            log.Info(string.Format(format, parameters));
            rasieLogNotifyEvent(LogLevel.Info, message);

        }

        public void WarningLog(string format, params object[] parameters)
        {
            var message = string.Format(format, parameters);
            log.Warn(string.Format(format, parameters));
            rasieLogNotifyEvent(LogLevel.Warn, message);
        }

        private void rasieLogNotifyEvent(LogLevel level,string message)
        {
            LogNotifyEvent?.Invoke(level ,message);
        }

        public void Init(string format, params object[] parameters)
        {
            var param = string.Format(format, parameters);
            log = LogManager.GetLogger(param);

        }
    }
    [Flags]
    public enum ActionType
    {
        SEND,
        RECEIVE
    }

    [Flags]
    public enum LogHandle
    {
        debug,
        writeFile
    }
    public class DefaultLog : ILog
    {
        string _taskName;
        string _filePath = "../../../../log/";
        LogHandle _handle = LogHandle.debug;
        bool _byteSteamLogSwicth;
        public string TasKName
        {
            get { return _taskName; }
            set { _taskName = value; }

        }
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }

        }
        public LogHandle Handle
        {
            get { return _handle; }
            set { _handle = value; }

        }
        public bool ByteSteamLogSwicth
        {
            get { return _byteSteamLogSwicth; }
            set { _byteSteamLogSwicth = value; }
        }

        public DefaultLog(string name)
        {
            _taskName = name;
        }
        public void ByteSteamLog(ActionType action, byte[] bytes)
        {
            if (_byteSteamLogSwicth)
            {
                string byteSteamString = "";
                string message = "";
                string fileName = "ByteSteam";
                foreach (byte bt in bytes)
                {
                    byteSteamString += string.Format("{0:x2}", bt) + " ";
                }
                switch (action)
                {
                    case ActionType.RECEIVE:
                        message = string.Format("{0}=> Rx:{1}", TasKName, byteSteamString);
                        break;
                    case ActionType.SEND:
                        message = string.Format("{0}=> Tx:{1}", TasKName, byteSteamString);
                        break;
                }
                if (byteSteamString != "")
                {
                    switch (_handle)
                    {
                        case LogHandle.debug:
                            debug(message);
                            break;
                        case LogHandle.writeFile:
                            writeLog(_filePath, fileName, message);
                            break;

                    }
                }
            }

        }

        public void DebugLog(string format, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public void ErrorLog(string format, params object[] parameters)
        {
            string fileName = "Error";
            var message = string.Format(format, parameters);
            message = string.Format("{0}=>{1}", TasKName, message);
            switch (_handle)
            {
                case LogHandle.debug:
                    debug(message);
                    break;
                case LogHandle.writeFile:
                    writeLog(_filePath, fileName, message);
                    break;

            }
        }
        public string GetName()
        {
            return _taskName;
        }
        public void WarningLog(string format, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        private static readonly object locker = new object();

        public event Action<LogLevel, string> LogNotifyEvent;

        private void writeLog(string sFilePath, string fileName, string sLogMessage)
        {
            lock (locker)
            {
                DateTime dt = DateTime.Now;
                string sDataNow = dt.Year.ToString() + dt.Month.ToString() + dt.Day.ToString();
                fileName = string.Format("{0}{1}_log{2}.txt", fileName, sDataNow, "00");
                string sfilePath = @sFilePath + fileName;
                FileStream fs;
                if (File.Exists(sfilePath))
                {
                    fs = new FileStream(sfilePath, FileMode.Append, FileAccess.Write);
                }
                else
                {
                    fs = new FileStream(sfilePath, FileMode.CreateNew, FileAccess.Write);
                }
                StreamWriter sw = new StreamWriter(fs);
                sLogMessage = dt + " " + sLogMessage;
                sw.WriteLine(sLogMessage);
                sw.Close();
                sw.Dispose();
            }
        }

        private void debug(string message)
        {
            Console.WriteLine(message);
        }

        public void InfoLog(string format, params object[] parameters)
        {
            string fileName = "Normal";
            var message = string.Format(format, parameters);
            message = string.Format("{0}=>{1}", TasKName, message);
            switch (_handle)
            {
                case LogHandle.debug:
                    debug(message);
                    break;
                case LogHandle.writeFile:
                    writeLog(_filePath, fileName, message);
                    break;

            }
        }

        public void FatalLog(string format, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public void Init(string format, params object[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}

