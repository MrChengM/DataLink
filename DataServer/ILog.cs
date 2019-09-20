using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer
{
    public interface ILog
    {
        /// <summary>Audit log should be called for logging user interactions
        /// will be written to an extra logfile by the logserver and the Wimap gets a widgets to read only the audit logs
        /// works like the prefix log. if you need aditional parameters as the default ones, just add them as key, value to the parameters array
        /// </summary>
        /// <param name="user">the User who has done the interaction</param>
        /// <param name="usergroup">the group(groups) to which the user belongs to</param>
        /// <param name="action">the action that has been done (maybe as a number to be translatable)</param>
        /// <param name="terminal">the terminal/workstation where the action has taken place</param>
        /// <param name="controller">the controller for which the action has been done</param>
        /// <param name="widget">the widgetName of the widget in which the action has taken place</param>
        /// <param name="category">defines the category for thsi auditLog (default is All)</param>
        /// <param name="oldVal">the old value of the change</param>
        /// <param name="newVal">the new value for the change</param>
        /// <param name="parameters">list of name, value pairs to log, names should be taken from ParameterNameConstants</param>
        void AuditLog(string user, string usergroup, string action, string terminal, string controller, string widget, AuditCategories category = 0, string oldVal = null, string newVal = null, params object[] parameters);
       
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
}
