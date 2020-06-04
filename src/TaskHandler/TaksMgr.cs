using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHandler.Config;
using ts=TaskHandler.Factory;
using DataServer.Utillity;
using DataServer;
using TaskHandler.Builder;

namespace TaskHandler
{
    public class TaskMgr
    {

        List<TaskConfig> _configs;
        const string _configFath= "../../../../conf/Configuration.xml";
        List<AbstractTask> _tasks;
        static TaskMgr _intance;
        const string _nodeElement = "task";
        const string _handler = "All";
        const string _taskName = "TaskMgr";
        int _initLevel;

        ILog _log;
        
        private bool setUp()
        {
            //bool flag;
            _configs = ReaderXMLUtil.ReadXMLConfig<TaskConfig>(_configFath, ConfigUtilly.ReadConfig, _nodeElement, _handler);
            if (_configs != null)
            {
                _log.NormalLog(string.Format("{0}:Down=>Creating","SetUp()"));
                var taskFactory = new ts.TaskFactory();
                foreach(var config in _configs)
                {
                    ILog log = new DefaultLog(config.TaskName) { Handle = LogHandle.writeFile, ByteSteamLogSwicth = false };
                    var task = taskFactory.CreateTask(config, log);
                    if (task != null)
                    {
                        task.InitLevel = config.InitLevel;
                        _log.NormalLog(string.Format("{0}:Task<{1}>,Creating=>Created", "SetUp()", config.TaskName));
                        addTask(task);
                    }
                }
                sortTasks();
                foreach(var t in _tasks)
                {
                    ChangInitLevel(t.InitLevel);
                    if (t.OnInit())
                    {
                        
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        private void addTask(AbstractTask task)
        {
            if (_tasks.Contains(task))
            {
                _log.ErrorLog(string.Concat("AddTask : Duplicate tasks with same Name: Name=<", task.TaskName, ">"));
            }
            _tasks.Add(task);
        }
        private void sortTasks()
        {
            if (_tasks != null)
            {
                _tasks.Sort((AbstractTask TaskA, AbstractTask TaskB) => (TaskA.InitLevel != TaskB.InitLevel ? TaskA.InitLevel - TaskB.InitLevel : string.Compare(TaskA.TaskName, TaskB.TaskName, StringComparison.Ordinal)));
            }
        }
        private bool run()
        {
            foreach(var t in _tasks)
            {
                if (t.OnStart())
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private void ChangInitLevel(int initLevel)
        {
            if (initLevel != _initLevel)
            {
                _log.NormalLog("Changing to init level {0} (from {1})", initLevel, _initLevel);
                _initLevel = initLevel;
            }
        }
        private TaskMgr()
        {
            _configs = new List<TaskConfig>();
            _tasks = new List<AbstractTask>();
            _log = new DefaultLog(_taskName) {Handle=LogHandle.writeFile};
        }
        public static void Main(string[] args)
        {
            _intance = new TaskMgr();
            if (_intance.setUp())
                _intance.run();
        }
    }
}
