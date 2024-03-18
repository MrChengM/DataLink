using DataServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskMgr.Task;
using DataServer.Config;
using DataServer.Points;
using TaskMgr.Factory;
using WCFRestFullAPI.Service;

namespace TaskMgr
{
    public class TaskMagr
    {

        private List<AbstractTask> _tasks;
        private ILog _log;
        private ProjectConfig _projectConfig;
        private IPointMapping _pointMapping;
        private static TaskMagr _intance;
        private Dictionary<string, Type> _driverTypes;
        private void creatTask()
        {
            _tasks = new List<AbstractTask>();

            var client = _projectConfig.Client;
            var ctFactory = new ChannelTaskFactory(_pointMapping, _driverTypes, _log);
            foreach (var item in client.Channels)
            {
                var channelTask = ctFactory.CreatChannelTask(item.Value);

                _tasks.Add(channelTask);
            }

            var servers = _projectConfig.Server;

            foreach (var item in servers.Items)
            {
                var severTask = new ServerTask(item.Value, _pointMapping,_log);
                _tasks.Add(severTask);
            }

            //var records = _projectConfig.Records;
            //foreach (var item in records.RecordGroup)
            //{

            //}

            //var alarms = _projectConfig.Alarms;
          

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
                if (t.OnStart())
                {
                    continue;
                }
                else
                {
                    _log.ErrorLog(string.Format("Task<{0}> Start failed!", t.TaskName));
                    return false;
                }
            }
            return true;
        }

        private bool stop()
        {
            foreach (var task in _tasks)
            {
                if (task.OnStop())
                {
                    continue;
                }
                else
                {
                    _log.ErrorLog(string.Format("Task<{0}> Stop failed!", task.TaskName));
                    return false;
                }
            }
            return true;
        }
        private void openConfigRestApiServer()
        {
            ConfigRestServerHost configRestServerHost = new ConfigRestServerHost();

            configRestServerHost.Open();

            _projectConfig = configRestServerHost.RestService.Config;
            _driverTypes= configRestServerHost.RestService.DriverTypes;
            configRestServerHost.RestService.ProConfRefreshEvent += RestService_ProConfRefreshEvent;
        }

        private void RestService_ProConfRefreshEvent(ProjectConfig config)
        {
            if (stop()&&init())
            {
                run();
            }
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
            _log = new Log4netWrapper();
            _log.LogNotifyEvent += t => { Console.WriteLine(t); };
            _pointMapping = new PointMapping();
            openConfigRestApiServer();
            creatTask();


        }
        public static void Main(string[] args)
        {
            _intance = new TaskMagr();
            if (_intance.init())
                _intance.run();
        }
    }
}
