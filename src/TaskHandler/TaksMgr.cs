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

                //创建外部task
                var taskFactory = new ts.TaskFactory(_configFath);
                //task类型配置
                foreach(var config in _configs)
                {
                    ILog log = new DefaultLog(config.TaskName) { Handle = LogHandle.debug, ByteSteamLogSwicth = false };
                    //根据task类型配置创建具体的task列表
                    var tasks = taskFactory.CreateTasks(config, log);
                    foreach(var task in tasks)
                    {
                        if (task != null)
                        {
                            task.InitLevel = config.InitLevel;
                            _log.NormalLog(string.Format("{0}:Task<{1}>,Creating=>Created", "SetUp()", config.TaskName));
                            addTask(task);
                        }
                    }
                }

                //创建内部Task

                //自定义协议，用于信号监控及界面通讯
                string freeTaskName = "FreedomServerTaskHandler";
                ILog freelog = new DefaultLog(freeTaskName);
                TimeOut freeTimeout = new TimeOut(freeTaskName, 1000, freelog);
                EthernetSetUp freeSetup = new EthernetSetUp("127.0.0.1", 9527);
                FreedomServerTask freeServerTask = new FreedomServerTask(freeTaskName,freelog,freeTimeout,freeSetup);
                _log.NormalLog(string.Format("{0}:Task<{1}>,Creating=>Created", "SetUp()", freeServerTask.TaskName));
                addTask(freeServerTask);

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
                        _log.ErrorLog(string.Format("Task<{0}> Inited failed!", t.TaskName));
                        return false;
                    }
                }
            }
            else
            {
                _log.ErrorLog("Tasks config Error!");
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
                    _log.ErrorLog(string.Format("Task<{0}> Start failed!", t.TaskName));
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
            _log = new DefaultLog(_taskName) {Handle=LogHandle.debug};
        }
        public static void Main(string[] args)
        {
            _intance = new TaskMgr();
            if (_intance.setUp())
                _intance.run();
        }
    }
}
