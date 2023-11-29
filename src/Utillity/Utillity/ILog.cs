using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utillity
{
    public interface ILog
    {
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

        /// <summary>Gets the current loglevel of the log
        /// </summary>
        /// <returns>The loglevel</returns>
        LogLevel GetLogLevel();

        /// <summary>The name of the log
        /// </summary>
        /// <returns>The name</returns>
        string GetName();

        /// <summary>Logs a normal log
        /// </summary>
        /// <param name="format">Log string (Example:This is the log {0},{1})</param>
        /// <param name="parameters">Parameters for the log string</param>
        void NormalLog(string format, params object[] parameters);

        /// <summary>Sets the current loglevel of the log
        /// Will be removed later
        /// </summary>
        /// <param name="level">The new level</param>
        void SetLogLevel(LogLevel level);

        ///  <summary>Logs a warning log
        /// <para> A possible existing exception object can be passed as the last argument in parameters</para>
        ///  </summary>
        ///  <param name="format">Log string (Example:This is the log {0},{1})</param>
        ///  <param name="parameters">Parameters for the log string and/or Exception</param>
        void WarningLog(string format, params object[] parameters);

        /// <summary>Logs a Data Steam log
        /// </summary>
        /// <param name="format"></param>
        /// <param name="parameters"></param>
        void ByteSteamLog(ActionType action, byte[] bytes);
    }

    public enum AuditCategories
    {
        /// <summary> the default value for all stuff that couldn't be assigned to a defined category </summary>
        All,
        /// <summary> for opening or closing something (e.g. a widget)</summary>
        OpenClose,
        /// <summary> for a printing action </summary>
        Print,
        /// <summary>for logging in or off a user </summary>
        LoggingInOff,
        /// <summary> for ackknowledge faults </summary>
        AckFaults,
        /// <summary> for actions done on a controller (eg. start, stop,...) </summary>
        ControllerActions,
        /// <summary> for change a Parameter </summary>
        ChangeParameter,
        /// <summary> for changning the special sortPositions </summary>
        ChangeSpecialSortPositions,
        /// <summary> for doing changes at the sorting table </summary>
        SortingTable,
        /// <summary> for doing stuff on a batch </summary>
        Batches,
        /// <summary> for exporting Data </summary>
        ExportData
    }

    /// <summary>
    /// Available loglevels
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// NO logging
        /// </summary>
        OFF,
        /// <summary>
        /// Audit logging level to log user actions
        /// </summary>
        AUDIT,
        /// <summary>
        /// Error logs are logged
        /// </summary>
        ERROR,
        /// <summary>
        /// Error and Warning logs are logged
        /// </summary>
        WARNING,
        /// <summary>
        /// Error, Warning and normal logs are logged
        /// </summary>
        NORMAL,
        /// <summary>
        /// Error, Warning, Normal and Debug logs are logged
        /// </summary>
        DEBUG
    }
    [Flags]
    public enum ActionType
    {
        SEND,
        RECEIVE
    }
    /// <summary>
    /// log记录处理，保存文件or输出到控制台上
    /// </summary>
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

        public LogLevel GetLogLevel()
        {
            throw new NotImplementedException();
        }

        public string GetName()
        {
            return _taskName;
        }

        public void NormalLog(string format, params object[] parameters)
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

        public void SetLogLevel(LogLevel level)
        {
            throw new NotImplementedException();
        }

        public void WarningLog(string format, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        private static readonly object locker = new object();
        private void writeLog(string sFilePath, string fileName, string sLogMessage)
        {
            lock (locker)
            {
                DateTime dt = DateTime.Now;
                int i = 0;
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
    }
}
