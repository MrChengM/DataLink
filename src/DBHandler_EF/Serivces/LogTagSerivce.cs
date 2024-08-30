using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;
using DataServer.Points;
using DBHandler_EF.Modules;

namespace DBHandler_EF.Serivces
{
    public class LogTagSerivce : ITagRecord
    {
        public int Delete(ITag tag)
        {
            int result;
            using (var tagContext = new LogContent())
            {
                var ltag = tagContext.LogTags.FirstOrDefault(s => s.PointName == tag.Name && s.TimeStamp == tag.TimeStamp && s.Value == tag.Value);
                tagContext.LogTags.Remove(ltag);
                result = tagContext.SaveChanges();
            }
            return result;
        }

        public int Delete(List<ITag> tags)
        {
            int result;
            using (var tagContext = new LogContent())
            {
                foreach (var tag in tags)
                {
                    var ltag = tagContext.LogTags.FirstOrDefault(s => s.PointName == tag.Name && s.TimeStamp == tag.TimeStamp && s.Value == tag.Value);
                    tagContext.LogTags.Remove(ltag);
                }
                result = tagContext.SaveChanges();
            }
            return result;
        }
        public int Delete(string pointName)
        {
            int result;
            using (var tagContext = new LogContent())
            {

                var ltags = from t in tagContext.LogTags
                            where t.PointName == pointName
                            select t;
                tagContext.LogTags.RemoveRange(ltags);
                result = tagContext.SaveChanges();
            }
            return result;
        }

        public int Delete(string pointName, DateTime stratTime, DateTime endTime)
        {
            int result;
            using (var tagContext = new LogContent())
            {

                var ltags = from t in tagContext.LogTags
                            where t.PointName == pointName && t.TimeStamp > stratTime && t.TimeStamp < endTime
                            select t;
                tagContext.LogTags.RemoveRange(ltags);
                result = tagContext.SaveChanges();
            }
            return result;
        }

        public int Insert(ITag tag)
        {
            int result;
            using (var tagContext = new LogContent())
            {
                var ltag = Convert(tag);
                tagContext.LogTags.Add(ltag);
                result = tagContext.SaveChanges();
            }
            return result;
        }

        public int Insert(List<ITag> tags)
        {

            int result;
            using (var tagContext = new LogContent())
            {
                List<LogTag> ltags = new List<LogTag>();
                foreach (var tag in tags)
                {
                    ltags.Add(Convert(tag));

                }
                tagContext.LogTags.AddRange(ltags);
                result = tagContext.SaveChanges();
            }
            return result;
        }

        public List<ITag> Select(string pointName)
        {
            List<ITag> result = new List<ITag>();
            using (var tagContext = new LogContent())
            {
                var ltags = from t in tagContext.LogTags
                            where t.PointName == pointName
                            select t;
                foreach (var ltag in ltags)
                {
                    result.Add(Convert(ltag));
                }
            }
            return result;
        }

        public List<ITag> Select(string pointName, DateTime startTime, DateTime endTime)
        {
            List<ITag> result = new List<ITag>();
            using (var tagContext = new LogContent())
            {
                var ltags = from t in tagContext.LogTags
                            where t.PointName == pointName && t.TimeStamp > startTime && t.TimeStamp < endTime
                            select t;
                foreach (var ltag in ltags)
                {
                    result.Add(Convert(ltag));
                }
            }
            return result;
        }
        public int Updata(ITag tag)
        {
            int result;
            using (var tagContext = new LogContent())
            {
                var ltag = tagContext.LogTags.FirstOrDefault(s => s.PointName == tag.Name);
                ltag.Value = tag.Value;
                ltag.Quality = tag.Quality.ToString();
                ltag.ValueType = tag.Type.ToString();
                ltag.TimeStamp = tag.TimeStamp;

                result = tagContext.SaveChanges();
            }
            return result;
        }

        public int Updata(List<ITag> tags)
        {
            int result;
            using (var tagContext = new LogContent())
            {
                foreach (var tag in tags)
                {
                    var ltag = tagContext.LogTags.FirstOrDefault(s => s.PointName == tag.Name);
                    ltag.Value = tag.Value;
                    ltag.Quality = tag.Quality.ToString();
                    ltag.ValueType = tag.Type.ToString();
                    ltag.TimeStamp = tag.TimeStamp;
                }
                result = tagContext.SaveChanges();
            }
            return result;
        }

        public LogTag Convert(ITag tag)
        {
            return new LogTag()
            {
                PointName = tag.Name,
                Quality = tag.Quality.ToString(),
                Value = tag.Value,
                ValueType = tag.Type.ToString(),
                TimeStamp = tag.TimeStamp
            };
        }
        public ITag Convert(LogTag logTag)
        {
            return new Tag()
            {
                Name = logTag.PointName,
                Quality =(QUALITIES)Enum.Parse(typeof(QUALITIES), logTag.Quality),
                Value=logTag.Value,
                Type= (DataType)Enum.Parse(typeof(DataType), logTag.ValueType),
                TimeStamp = logTag.TimeStamp
            };
        }

      
    }
}
