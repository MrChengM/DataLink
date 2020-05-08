using DataServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer.Serialization;


namespace DataServer
{
    public class PointsRegister
    {
        /// <summary>
        /// 根据数据类型，直接写入主Mapping
        /// </summary>
        /// <param name="points"></param>
        /// <param name="log"></param>
        public static void Register(PointCollcet points,ILog log)
        {
            var boolPointMapping = PointMapping<bool>.GetInstance(log);
            var bytePointMapping = PointMapping<byte>.GetInstance(log);
            var ushortPointMapping = PointMapping<ushort>.GetInstance(log);
            var shortPointMapping = PointMapping<short>.GetInstance(log);
            var uintPointMapping = PointMapping<uint>.GetInstance(log);
            var intPointMapping = PointMapping<int>.GetInstance(log);
            var folatPointMapping = PointMapping<float>.GetInstance(log);
            var doublePointMapping = PointMapping<double>.GetInstance(log);
            var stringPointMapping = PointMapping<string>.GetInstance(log);

            foreach (var point in points.BoolPoints)
            {
                boolPointMapping.Register(point.Name, point);
            }

            foreach(var point in points.BytePoints)
            {
                bytePointMapping.Register(point.Name, point);
            }
            foreach (var point in points.UshortPoints)
            {
                ushortPointMapping.Register(point.Name, point);
            }
            foreach (var point in points.ShortPoints)
            {
                shortPointMapping.Register(point.Name, point);
            }

            foreach (var point in points.IntPoints)
            {
                intPointMapping.Register(point.Name, point);
            }

            foreach (var point in points.UintPoints)
            {
                uintPointMapping.Register(point.Name, point);
            }

            foreach (var point in points.FloatPoints)
            {
                folatPointMapping.Register(point.Name, point);
            }
            foreach (var point in points.DoublePoints)
            {
                doublePointMapping.Register(point.Name, point);
            }
            foreach (var point in points.StringPoints)
            {
                stringPointMapping.Register(point.Name, point);
            }

        }

        /// <summary>
        /// bool型点注册
        /// </summary>
        /// <param name="points"></param>
        /// <param name="mapping"></param>
        public static void Register(PointCollcet points, IPointMapping<bool> mapping)
        {
            foreach (var point in points.BoolPoints)
            {
                mapping.Register(point.Name, point);
            }

        }
        /// <summary>
        /// byte型点注册
        /// </summary>
        /// <param name="points"></param>
        /// <param name="mapping"></param>
        public static void Register(PointCollcet points, IPointMapping<byte> mapping)
        {
            foreach (var point in points.BytePoints)
            {
                mapping.Register(point.Name, point);
            }

        }
        /// <summary>
        /// ushort型点注册
        /// </summary>
        /// <param name="points"></param>
        /// <param name="mapping"></param>
        public static void Register(PointCollcet points, IPointMapping<ushort> mapping)
        {
            foreach (var point in points.UshortPoints)
            {
                mapping.Register(point.Name, point);
            }

        }
        /// <summary>
        /// short型点注册
        /// </summary>
        /// <param name="points"></param>
        /// <param name="mapping"></param>
        public static void Register(PointCollcet points, IPointMapping<short> mapping)
        {
            foreach (var point in points.ShortPoints)
            {
                mapping.Register(point.Name, point);
            }

        }
        /// <summary>
        /// uint型点注册
        /// </summary>
        /// <param name="points"></param>
        /// <param name="mapping"></param>
        public static void Register(PointCollcet points, IPointMapping<uint> mapping)
        {
            foreach (var point in points.UintPoints)
            {
                mapping.Register(point.Name, point);
            }

        }
        /// <summary>
        /// int型点注册
        /// </summary>
        /// <param name="points"></param>
        /// <param name="mapping"></param>
        public static void Register(PointCollcet points, IPointMapping<int> mapping)
        {
            foreach (var point in points.IntPoints)
            {
                mapping.Register(point.Name, point);
            }

        }
        /// <summary>
        /// float型点注册
        /// </summary>
        /// <param name="points"></param>
        /// <param name="boolMaping"></param>
        public static void Register(PointCollcet points, IPointMapping<float> mapping)
        {
            foreach (var point in points.FloatPoints)
            {
                mapping.Register(point.Name, point);
            }

        }
        /// <summary>
        /// double型点注册
        /// </summary>
        /// <param name="points"></param>
        /// <param name="mapping"></param>
        public static void Register(PointCollcet points, IPointMapping<double> mapping)
        {
            foreach (var point in points.DoublePoints)
            {
                mapping.Register(point.Name, point);
            }

        }
        /// <summary>
        /// string型点注册
        /// </summary>
        /// <param name="points"></param>
        /// <param name="mapping"></param>
        public static void Register(PointCollcet points, IPointMapping<string> mapping)
        {
            foreach (var point in points.StringPoints)
            {
                mapping.Register(point.Name, point);
            }

        }
    }
    /// <summary>
    /// 各种数据类型点集合
    /// </summary>
    public class PointCollcet
    {

        List<IPoint<bool>> boolPoints = new List<IPoint<bool>>();
        List<IPoint<byte>> bytePoints = new List<IPoint<byte>>();
        List<IPoint<ushort>> ushortPoints = new List<IPoint<ushort>>();
        List<IPoint<short>> shortPoints = new List<IPoint<short>>();
        List<IPoint<int>> intPoints = new List<IPoint<int>>();
        List<IPoint<uint>> uintPoints = new List<IPoint<uint>>();
        List<IPoint<float>> floatPoints = new List<IPoint<float>>();
        List<IPoint<double>> doublePoints = new List<IPoint<double>>();
        List<IPoint<string>> stringPoints = new List<IPoint<string>>();

        public List<IPoint<bool>> BoolPoints { get { return boolPoints; } set { boolPoints = value; } }
        public List<IPoint<byte>> BytePoints { get { return bytePoints; } set { bytePoints = value; } }

        public List<IPoint<ushort>> UshortPoints { get { return ushortPoints; } set { ushortPoints = value; } }
        public List<IPoint<short>> ShortPoints { get { return shortPoints; } set { shortPoints = value; } }

        public List<IPoint<uint>> UintPoints { get { return uintPoints; } set { uintPoints = value; } }

        public List<IPoint<int>> IntPoints { get { return intPoints; } set { intPoints = value; } }


        public List<IPoint<float>> FloatPoints { get { return floatPoints; } set { floatPoints = value; } }
        public List<IPoint<double>> DoublePoints { get { return doublePoints; } set { doublePoints = value; } }

        public List<IPoint<string>> StringPoints { get { return stringPoints; } set { stringPoints = value; } }

        /// <summary>
        /// 根据xmlWorkBook数据创建点集合
        /// </summary>
        /// <param name="workBook"></param>
        /// <returns></returns>
        public static PointCollcet CreateDevicePoints(XMLWorkbook workBook)
        {
            PointCollcet collcet = new PointCollcet();

            List<string> colums = workBook.llStrings[0];//第一行列表为索引列数据；
            workBook.llStrings.RemoveAt(0);
            foreach (var ls in workBook.llStrings)
            {
                string pointName = "";
                DeviceAddress deviceAddress = new DeviceAddress();
                string dataType = "";
                int length = 0;
                for (int i = 0; i < colums.Count; i++)
                {

                    switch (colums[i].ToLower())
                    {
                        case "tagname":
                            pointName = ls[i];
                            break;
                        case "area":
                            deviceAddress.Area = int.Parse(ls[i]);
                            break;
                        case "address":
                            deviceAddress.Address = int.Parse(ls[i]);
                            break;
                        case "datatype":
                            dataType = ls[i];
                            break;
                        case "length":
                            length = int.Parse(ls[i]);
                            break;
                        case "byteorder":
                            deviceAddress.ByteOrder = (ByteOrder)int.Parse(ls[i]);
                            break;
                    }
                }
                if (dataType.ToLower() == ValueType.Bool)
                {
                    collcet.BoolPoints.Add(new DevicePoint<bool>(pointName, dataType.ToLower(), length, deviceAddress));
                }
                if (dataType.ToLower() == ValueType.Bool)
                {
                    collcet.BoolPoints.Add(new DevicePoint<bool>(pointName, dataType.ToLower(), length, deviceAddress));
                }
                if (dataType.ToLower() ==ValueType.UInt16)
                {
                    collcet.UshortPoints.Add(new DevicePoint<ushort>(pointName, dataType.ToLower(), length, deviceAddress));
                }
                if (dataType.ToLower() == ValueType.Int16)
                {
                    collcet.ShortPoints.Add(new DevicePoint<short>(pointName, dataType.ToLower(), length, deviceAddress));
                }
                if (dataType.ToLower() == ValueType.UInt32)
                {
                    collcet.UintPoints.Add(new DevicePoint<uint>(pointName, dataType.ToLower(), length, deviceAddress));
                }
                if (dataType.ToLower() == ValueType.Int32)
                {
                    collcet.IntPoints.Add(new DevicePoint<int>(pointName, dataType.ToLower(), length, deviceAddress));
                }
                if (dataType.ToLower() == ValueType.Float)
                {
                    collcet.FloatPoints.Add(new DevicePoint<float>(pointName, dataType.ToLower(), length, deviceAddress));
                }
                if (dataType.ToLower() == ValueType.Double)
                {
                    collcet.DoublePoints.Add(new DevicePoint<double>(pointName, dataType.ToLower(), length, deviceAddress));
                }
            }

            return collcet;
        }
    }
}
