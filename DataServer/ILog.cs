using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer
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
        void ByteSteamLog(ActionType action  ,byte[] bytes);
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
    public enum ActionType
    {
        SEND,
        RECEIVE
    }
}
