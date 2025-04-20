using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskMgr.Task;
using TaskMgr.Factory;
using DataServer;
using DataServer.Log;
using DataServer.Config;
using DataServer.Points;
using DataServer.Permission;
using DataServer.Task;
using DataServer.Alarm;
using Unity;
using SignalRSelfHost;
using DBHandler_EF.Services;
using WCFRestFullAPI.Service;
using SignalRSelfHost.Hubs;

namespace TaskMgr
{
    public class TaskMagr
    {

        private List<AbstractTask> _tasks;
        private ILog _log;
        private ProjectConfig _projectConfig;
        private static TaskMagr _intance;
        private Dictionary<string, Type> _driverTypes;

        private static UnityContainer _container;
        private void creatTask()
        {
            _tasks = new List<AbstractTask>();

            var client = _projectConfig.Client;
            var ctFactory = _container.Resolve<ChannelTaskFactory>();
            ctFactory.DriverTypes= _driverTypes;
            foreach (var item in client.Channels)
            {
                var channelTask = ctFactory.CreatChannelTask(item.Value);

                _tasks.Add(channelTask);
            }

            var servers = _projectConfig.Server;

            foreach (var item in servers.Items)
            {
                var serverTask = _container.Resolve<ServerTask>();
                serverTask.ServerConfig = item.Value;
                _tasks.Add(serverTask);
            }

            var records = _projectConfig.Records;
            var recordFactory =_container.Resolve<RecordTagTaskFactory>();
            foreach (var item in records.RecordGroup)
            {
                var recordTask = recordFactory.CreatRecordTask(item.Value);
                _tasks.Add(recordTask);
            }

            //var records = _projectConfig.Records;
            //foreach (var item in records.RecordGroup)
            //{

            //}

            var alarms = _projectConfig.Alarms;

            var alarmTask =_container.Resolve<IAlarmTask>();
            alarmTask.AlarmsConfig = alarms;
            _tasks.Add((AbstractTask)alarmTask);

        }
        private bool init()
        {
            creatTask();
            sortTasks();
            foreach (var task in _tasks)
            {
                if (task.OnInit())
                {
                    continue;
                }
                else
                {
                    _log.ErrorLog(string.Format("Task<{0}> init failed!", task.TaskName));
                    return false;
                }
            }
            return true;
        }

        //private void addTask(AbstractTask task)
        //{
        //    if (_tasks.Contains(task))
        //    {
        //        _log.ErrorLog(string.Concat("AddTask : Duplicate tasks with same Name: Name=<", task.TaskName, ">"));
        //    }
        //    _tasks.Add(task);
        //}
        private void sortTasks()
        {
            if (_tasks != null)
            {
                _tasks.Sort((AbstractTask TaskA, AbstractTask TaskB) => TaskA.InitLevel != TaskB.InitLevel ? TaskA.InitLevel - TaskB.InitLevel : string.Compare(TaskA.TaskName, TaskB.TaskName, StringComparison.Ordinal));
            }
        }
     
        private bool run()
        {
            foreach (var t in _tasks)
            {
                if (!t.OnStart())
                {
                    _log.ErrorLog(string.Format("Task<{0}> Start failed!", t.TaskName));
                }
            }
            //openSignalRServer();
            return true;
        }

        private bool stop()
        {
            bool result = true;
            _tasks.Reverse();
            foreach (var task in _tasks)
            {
                if (!task.OnStop())
                {
                    //_log.ErrorLog(string.Format("Task<{0}> Stop failed!", task.TaskName));
                    result = false;
                }
            }
            _tasks.Clear();
            return result;
        }
        private bool restart()
        {
            bool result = true;
            _log.InfoLog(string.Format("Restarting"));

            if (!stop())
            {
                return false;
            }
            if (!init())
            {
                return false;

            }
            if (!run())
            {
                return false;
            }
            return result;
        }
        //private void openConfigRestApiServer()
        //{
        //    ConfigRestServerHost configRestServerHost = new ConfigRestServerHost();

        //    configRestServerHost.Open();

        //    _projectConfig = configRestServerHost.RestService.Config;
        //    _driverTypes= configRestServerHost.RestService.DriverTypes;
        //    configRestServerHost.RestService.ProConfRefreshEvent += RestService_ProConfRefreshEvent;
        //}

        private void openSignalRServer() 
        {
            var signalR = _container.Resolve<SignalRServer>();
            signalR.StartServer("http://localhost:3051",_container);

            var configServer = _container.Resolve<IConfigServer>();
            _projectConfig = configServer.Config;
            _driverTypes = configServer.DriverTypes;
            configServer.ProConfRefreshEvent += RestService_ProConfRefreshEvent;
        }
        private void RestService_ProConfRefreshEvent(ProjectConfig config)
        {
            restart();
        }

        
        //private void ChangInitLevel(int initLevel)
        //{
        //    if (initLevel != _initLevel)
        //    {
        //        _log.InfoLog("Changing to init level {0} (from {1})", initLevel, _initLevel);
        //        _initLevel = initLevel;
        //    }
        //}
        private TaskMagr()
        {

        }

        public static void Main(string[] args,string loggerName)
        {

            _intance = new TaskMagr();
            _container = new UnityContainer();
            _container.RegisterSingleton<IPointMapping, PointMapping>();
            _container.RegisterSingleton<ILog, Log4netWrapper>();
            _container.RegisterSingleton<IAlarmTask, AlarmTask>();
            _container.RegisterSingleton<IHisAlarmRecordCRUD, LogHistoryAlarmSerivce>();
            _container.RegisterSingleton<IPermissionsCRUD, PermissionSerivce>();
            _container.RegisterSingleton<IOperateRecordCRUD, OperateRecoredSerivce>();

            _container.RegisterSingleton<ISignalsHubProxy, SignalsHubProxy>();
            _container.RegisterSingleton<IAlarmHubProxy, AlarmHubProxy>();

            _container.RegisterSingleton<IConfigServer, ConfigService>();
            _intance._log = _container.Resolve<ILog>();
            _intance._log.Init(loggerName);
            _intance._log.LogNotifyEvent += (l, m) => { if (l != LogLevel.Debug) Console.WriteLine($"{DateTime.Now} {l} {m}"); };

            
            _intance.openSignalRServer();
            _intance.creatTask();
            if (_intance.init())
                _intance.run();
        }
    }
}
