using DataServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer.Serialization;
using System.Globalization;
using ds = DataServer;


namespace DataServer.Points
{
    public class PointsRegister
    {
        /// <summary>
        /// 根据数据类型，直接写入主Mapping
        /// </summary>
        /// <param name="points"></param>
        /// <param name="log"></param>
        public static void Register(PointDeviceCollcet points,ILog log)
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
        public static void Register(PointDeviceCollcet points, IPointMapping<bool> mapping)
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
        public static void Register(PointDeviceCollcet points, IPointMapping<byte> mapping)
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
        public static void Register(PointDeviceCollcet points, IPointMapping<ushort> mapping)
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
        public static void Register(PointDeviceCollcet points, IPointMapping<short> mapping)
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
        public static void Register(PointDeviceCollcet points, IPointMapping<uint> mapping)
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
        public static void Register(PointDeviceCollcet points, IPointMapping<int> mapping)
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
        public static void Register(PointDeviceCollcet points, IPointMapping<float> mapping)
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
        public static void Register(PointDeviceCollcet points, IPointMapping<double> mapping)
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
        public static void Register(PointDeviceCollcet points, IPointMapping<string> mapping)
        {
            foreach (var point in points.StringPoints)
            {
                mapping.Register(point.Name, point);
            }

        }
    }
    /// <summary>
    /// 设备物理点的收集
    /// 目前主要实现由workbook导入的点的收集
    /// </summary>
    public class PointDeviceCollcet
    {

        List<DevicePoint<bool>> boolPoints = new List<DevicePoint<bool>>();
        List<DevicePoint<byte>> bytePoints = new List<DevicePoint<byte>>();
        List<DevicePoint<ushort>> ushortPoints = new List<DevicePoint<ushort>>();
        List<DevicePoint<short>> shortPoints = new List<DevicePoint<short>>();
        List<DevicePoint<int>> intPoints = new List<DevicePoint<int>>();
        List<DevicePoint<uint>> uintPoints = new List<DevicePoint<uint>>();
        List<DevicePoint<float>> floatPoints = new List<DevicePoint<float>>();
        List<DevicePoint<double>> doublePoints = new List<DevicePoint<double>>();
        List<DevicePoint<string>> stringPoints = new List<DevicePoint<string>>();

        public List<DevicePoint<bool>> BoolPoints { get { return boolPoints; } set { boolPoints = value; } }
        public List<DevicePoint<byte>> BytePoints { get { return bytePoints; } set { bytePoints = value; } }

        public List<DevicePoint<ushort>> UshortPoints { get { return ushortPoints; } set { ushortPoints = value; } }
        public List<DevicePoint<short>> ShortPoints { get { return shortPoints; } set { shortPoints = value; } }

        public List<DevicePoint<uint>> UintPoints { get { return uintPoints; } set { uintPoints = value; } }

        public List<DevicePoint<int>> IntPoints { get { return intPoints; } set { intPoints = value; } }


        public List<DevicePoint<float>> FloatPoints { get { return floatPoints; } set { floatPoints = value; } }
        public List<DevicePoint<double>> DoublePoints { get { return doublePoints; } set { doublePoints = value; } }

        public List<DevicePoint<string>> StringPoints { get { return stringPoints; } set { stringPoints = value; } }

       
    }

    public class PointVirtualCollcet
    {
        List<VirtulPoint<bool>> boolPoints = new List<VirtulPoint<bool>>();
        List<VirtulPoint<byte>> bytePoints = new List<VirtulPoint<byte>>();
        List<VirtulPoint<ushort>> ushortPoints = new List<VirtulPoint<ushort>>();
        List<VirtulPoint<short>> shortPoints = new List<VirtulPoint<short>>();
        List<VirtulPoint<int>> intPoints = new List<VirtulPoint<int>>();
        List<VirtulPoint<uint>> uintPoints = new List<VirtulPoint<uint>>();
        List<VirtulPoint<float>> floatPoints = new List<VirtulPoint<float>>();
        List<VirtulPoint<double>> doublePoints = new List<VirtulPoint<double>>();
        List<VirtulPoint<string>> stringPoints = new List<VirtulPoint<string>>();

        public List<VirtulPoint<bool>> BoolPoints { get { return boolPoints; } set { boolPoints = value; } }
        public List<VirtulPoint<byte>> BytePoints { get { return bytePoints; } set { bytePoints = value; } }

        public List<VirtulPoint<ushort>> UshortPoints { get { return ushortPoints; } set { ushortPoints = value; } }
        public List<VirtulPoint<short>> ShortPoints { get { return shortPoints; } set { shortPoints = value; } }

        public List<VirtulPoint<uint>> UintPoints { get { return uintPoints; } set { uintPoints = value; } }

        public List<VirtulPoint<int>> IntPoints { get { return intPoints; } set { intPoints = value; } }


        public List<VirtulPoint<float>> FloatPoints { get { return floatPoints; } set { floatPoints = value; } }
        public List<VirtulPoint<double>> DoublePoints { get { return doublePoints; } set { doublePoints = value; } }

        public List<VirtulPoint<string>> StringPoints { get { return stringPoints; } set { stringPoints = value; } }
    }
    public static class PointsCollcetCreate
    {
        /// <summary>
        /// 根据xmlWorkBook数据创建点集合
        /// </summary>
        /// <param name="workBook"></param>
        /// <returns></returns>
        public static PointDeviceCollcet Create(XMLWorkbook workBook, ILog log)
        {
            PointDeviceCollcet collcet = new PointDeviceCollcet();

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
                            int area;
                            if (int.TryParse(ls[i], out area))
                                deviceAddress.SalveId = area;
                            break;
                        case "address":
                            int address;
                            if (int.TryParse(ls[i], out address))
                                deviceAddress.Address = address;
                            break;
                        case "datatype":
                            dataType = ls[i];
                            break;
                        case "length":
                            int.TryParse(ls[i], out length);
                            break;
                        case "byteorder":
                            int byteorder;
                            if (int.TryParse(ls[i], out byteorder))
                                deviceAddress.ByteOrder = (ByteOrder)byteorder;
                            break;
                    }
                }
                if (pointName != "")
                {
                    if (dataType.ToLower() == ValueType.Bool)
                    {
                        collcet.BoolPoints.Add(new DevicePoint<bool>(pointName, dataType.ToLower(), length, deviceAddress));
                    }
                    if (dataType.ToLower() == ValueType.Bool)
                    {
                        collcet.BoolPoints.Add(new DevicePoint<bool>(pointName, dataType.ToLower(), length, deviceAddress));
                    }
                    if (dataType.ToLower() == ValueType.UInt16)
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
            }
            return collcet;
        }
        /// <summary>
        /// 根据xmlWorkBook数据创建DL645点集合
        /// </summary>
        /// <param name="workBook"></param>
        /// <returns></returns>
        public static PointDeviceCollcet CreateDL645(XMLWorkbook workBook, ILog log)
        {
            PointDeviceCollcet collcet = new PointDeviceCollcet();

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
                            int area;
                            if (int.TryParse(ls[i], out area))
                                deviceAddress.SalveId = int.Parse(ls[i], NumberStyles.HexNumber);
                            break;
                        case "address":
                            int address;
                            if (int.TryParse(ls[i], out address))
                                deviceAddress.Address = int.Parse(ls[i], NumberStyles.HexNumber);
                            break;
                        case "datatype":
                            dataType = ls[i];
                            break;
                        case "length":
                            int.TryParse(ls[i], out length);
                            break;
                        case "byteorder":
                            int byteorder;
                            if (int.TryParse(ls[i], out byteorder))
                                deviceAddress.ByteOrder = (ByteOrder)byteorder;
                            break;
                    }
                }
                if (pointName != "")
                {
                    if (dataType.ToLower() == ValueType.Bool)
                    {
                        collcet.BoolPoints.Add(new DevicePoint<bool>(pointName, dataType.ToLower(), length, deviceAddress));
                    }
                    if (dataType.ToLower() == ValueType.Bool)
                    {
                        collcet.BoolPoints.Add(new DevicePoint<bool>(pointName, dataType.ToLower(), length, deviceAddress));
                    }
                    if (dataType.ToLower() == ValueType.UInt16)
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
            }
            return collcet;
        }
        /// <summary>
        /// 根据XML配置文件创建Modbus点表
        /// 并将点表绑定到通用点表（PointMapping)
        /// </summary>
        /// <param name="workBook"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static PointVirtualCollcet CreateMoudbus(XMLWorkbook workBook, ILog log)
        {
            MappingIndexList mappingIdexList = MappingIndexList.GetInstance();
            var result = new PointVirtualCollcet();
            List<string> colums = workBook.llStrings[0];//第一行列表为索引列数据；
            workBook.llStrings.RemoveAt(0);
            foreach (var ls in workBook.llStrings)
            {
                string addressName = "";
                int address;
                string pointName = "";
                int bindingIndex = 0;
                BindingWay bindingWay = BindingWay.OneWay;

                ByteOrder byteOrder = ByteOrder.None;
                if (ls.Count < colums.Count)
                {
                    continue;
                }
                for (int i = 0; i < colums.Count; i++)
                {

                    switch (colums[i].ToLower())
                    {
                        case "addressname":
                            addressName = ls[i];
                            break;
                        case "pointname":
                            var pointGroup = ls[i].Split(new char[] { '[' });

                            if (pointGroup != null && pointGroup.Length >= 2)
                            {
                                pointName = pointGroup[0];
                                pointGroup[1] = pointGroup[1].Replace("]", "");//string类型处理，去掉‘]’
                                int.TryParse(pointGroup[1], out bindingIndex);
                            }
                            break;
                        case "bindingway":
                            int temp;
                            if (int.TryParse(ls[i], out temp))
                                bindingWay = (BindingWay)temp;
                            break;
                        case "byteorder":
                            int temp1;
                            if (int.TryParse(ls[i], out temp1))
                                byteOrder = (ByteOrder)temp1;
                            break;
                    }
                }
                string type;

                /*
                ===============================
                 1.根据地址名生成相应的Modbus点
                 2.根据配置表绑定到对应的主地址表
                 3.如果绑定成功则添加到list中
                 4。根据不同的绑定类型分为：只读，只写，可读可写
                 ===============================
                */
                if (int.TryParse(addressName, out address) && pointName != "")
                {
                    if (addressName.Substring(0, 1) == "0" || addressName.Substring(0, 1) == "1")
                    {
                        if (mappingIdexList.Find(pointName, out type) && type == ds.ValueType.Bool)
                        {
                            VirtulPoint<bool> boolPoint = new VirtulPoint<bool>(addressName, type) { };
                            var sourcePoint = PointMapping<bool>.GetInstance(log).GetPoint(pointName);
                            if (PointsBinding.BoolBinding(sourcePoint, bindingIndex, boolPoint, bindingWay))
                            {
                                result.BoolPoints.Add(boolPoint);
                            }
                        }
                    }
                    if (addressName.Substring(0, 1) == "3" || addressName.Substring(0, 1) == "4")
                    {
                        if (mappingIdexList.Find(pointName, out type))
                        {
                            VirtulPoint<ushort> ushortPoint1 = new VirtulPoint<ushort>(addressName, ds.ValueType.UInt16);
                            VirtulPoint<ushort> ushortPoint2 = new VirtulPoint<ushort>((address + 1).ToString(), ds.ValueType.UInt16);

                            if (type == ds.ValueType.Byte)
                            {
                                var sourcePoint = PointMapping<byte>.GetInstance(log).GetPoint(pointName);
                                if (PointsBinding.UshortBinding(sourcePoint, bindingIndex, ushortPoint1, bindingWay))
                                {
                                    result.UshortPoints.Add(ushortPoint1);
                                }
                            }
                            if (type == ds.ValueType.Int16)
                            {
                                var sourcePoint = PointMapping<short>.GetInstance(log).GetPoint(pointName);
                                if (PointsBinding.UshortBinding(sourcePoint, bindingIndex, ushortPoint1, bindingWay))
                                {
                                    result.UshortPoints.Add(ushortPoint1);
                                }
                            }
                            if (type == ds.ValueType.UInt16)
                            {
                                var sourcePoint = PointMapping<ushort>.GetInstance(log).GetPoint(pointName);
                                if (PointsBinding.UshortBinding(sourcePoint, bindingIndex, ushortPoint1, bindingWay))
                                {
                                    result.UshortPoints.Add(ushortPoint1);
                                }
                            }
                            if (type == ds.ValueType.Int32)
                            {
                                var sourcePoint = PointMapping<int>.GetInstance(log).GetPoint(pointName);
                                if (PointsBinding.UshortBinding(sourcePoint, bindingIndex, new VirtulPoint<ushort>[] { ushortPoint1, ushortPoint2 }, byteOrder, bindingWay))
                                {
                                    result.UshortPoints.Add(ushortPoint1);
                                    result.UshortPoints.Add(ushortPoint2);
                                }
                            }
                            if (type == ds.ValueType.UInt32)
                            {
                                var sourcePoint = PointMapping<uint>.GetInstance(log).GetPoint(pointName);
                                if (PointsBinding.UshortBinding(sourcePoint, bindingIndex, new VirtulPoint<ushort>[] { ushortPoint1, ushortPoint2 }, byteOrder, bindingWay))
                                {
                                    result.UshortPoints.Add(ushortPoint1);
                                    result.UshortPoints.Add(ushortPoint2);
                                }
                            }
                            if (type == ds.ValueType.Float)
                            {
                                var sourcePoint = PointMapping<float>.GetInstance(log).GetPoint(pointName);
                                if (PointsBinding.UshortBinding(sourcePoint, bindingIndex, new VirtulPoint<ushort>[] { ushortPoint1, ushortPoint2 }, byteOrder, bindingWay))
                                {
                                    result.UshortPoints.Add(ushortPoint1);
                                    result.UshortPoints.Add(ushortPoint2);
                                }
                            }
                        }
                    }
                }
                else
                {
                    log.ErrorLog(string.Format("adderss error or binding point is null!"));
                }

            }
            return result;

        }
    }
}
