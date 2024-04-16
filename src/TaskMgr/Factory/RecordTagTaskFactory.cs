using DataServer;
using DataServer.Config;
using DataServer.Points;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskMgr.Task;

namespace TaskMgr.Factory
{
 public class RecordTagTaskFactory
    {
        private IPointMapping _pointMapping;
        private ILog _log;

        public RecordTagTaskFactory(IPointMapping pointMapping,ILog log)
        {
            _pointMapping = pointMapping;
            _log = log;
        }
        public AbstractTask CreatRecordTask(RecordItemConfig recordItemConfig)
        {

            RecordWay way = recordItemConfig.Option;
            switch (way)
            {
                case RecordWay.OnTime:
                    return new RecordTaskOnTime(_pointMapping, recordItemConfig, _log);
                case RecordWay.OnChange:
                    return new RecordTaskOnChange(_pointMapping, recordItemConfig, _log);
                default:
                    return null;
            }
        }
    }
}
