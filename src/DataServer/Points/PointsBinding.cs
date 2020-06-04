using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;
using DataServer.Utillity;
namespace DataServer.Points
{

    /// <summary>
    /// 虚拟点绑定到设备点
    /// 主要是虚拟点
    /// </summary>
    public class PointsBinding
    {
        /// <summary>
        /// 16位无符号整形点绑定浮点数据
        /// </summary>
        /// <param name="source">浮点源数据</param>
        /// <param name="sourceindex">源数据点索引</param>
        /// <param name="targets">目标数据，数组长度为2，点长度为1的虚拟点</param>
        /// <param name="bindingWay">绑定方式，默认为单方向</param>
        public static bool UshortBinding(IPoint<float> source, int sourceindex, IPoint<ushort>[] targets,ByteOrder byteOrder=ByteOrder.None,  BindingWay bindingWay = BindingWay.OneWay)
        {
            if (sourceindex >= source.Length)
            {
                return false;
            }
            if (targets.Length != 2)
            {
                return false;
            }
            switch (bindingWay)
            {
                case BindingWay.OneWay:
                    source.PropertyChanged += (s, p) =>
                    {
                        if (int.Parse(p.PropertyName) == sourceindex)
                        {
                            byte[] temp1 = UnsafeNetConvert.FloatToBytes(source[(byte)sourceindex], byteOrder);
                            ushort[] temp2 = (byteOrder == ByteOrder.BigEndian) ? UnsafeNetConvert.BytesToUShorts(temp1, 0, 2, byteOrder) : UnsafeNetConvert.BytesToUShorts(temp1, 0, 2, ByteOrder.None); //判断byteorder是否为无须翻转高低位的大端数据
                            for (int i = 0; i < targets.Length; i++)
                            {
                                targets[i].ValueUpdate(temp2[i], 0);
                            }
                        }
                    };
                    return true;
                case BindingWay.TwoWay:
                    source.PropertyChanged += (s, p) =>
                    {
                        if (int.Parse(p.PropertyName) == sourceindex)
                        {
                            byte[] temp1 = UnsafeNetConvert.FloatToBytes(source[(byte)sourceindex], byteOrder);
                            ushort[] temp2 = (byteOrder == ByteOrder.BigEndian) ? UnsafeNetConvert.BytesToUShorts(temp1, 0, 2, byteOrder) : UnsafeNetConvert.BytesToUShorts(temp1, 0, 2, ByteOrder.None); //判断byteorder是否为无须翻转高低位的大端数据
                            for (int i = 0; i < targets.Length; i++)
                            {
                                targets[i].ValueUpdate(temp2[i], 0);
                            }
                        }
                    };
                    foreach (var target in targets)
                    {
                        target.PropertyChanged += (s, p) =>
                        {
                            var temp1 = (byteOrder == ByteOrder.BigEndian) ? UnsafeNetConvert.UShortsToBytes(new ushort[] { targets[0][0], targets[1][0] }, byteOrder) : UnsafeNetConvert.UShortsToBytes(new ushort[] { targets[0][0], targets[1][0] }, ByteOrder.None);
                            var temp2 = UnsafeNetConvert.BytesToFloat(temp1, 0, byteOrder);
                            source.ValueUpdate(temp2, sourceindex);
                        };
                    }
                    return true;
                case BindingWay.ReverseWay:
                    foreach (var target in targets)
                    {
                        target.PropertyChanged += (s, p) =>
                        {
                            var temp1 = (byteOrder == ByteOrder.BigEndian) ? UnsafeNetConvert.UShortsToBytes(new ushort[] { targets[0][0], targets[1][0] }, byteOrder) : UnsafeNetConvert.UShortsToBytes(new ushort[] { targets[0][0], targets[1][0] }, ByteOrder.None);
                            var temp2 = UnsafeNetConvert.BytesToFloat(temp1, 0, byteOrder);
                            source.ValueUpdate(temp2, sourceindex);
                        };
                    }
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// 16位无符号整形点绑定32整形数据
        /// </summary>
        /// <param name="source">32位整形源数据</param>
        /// <param name="sourceindex">源数据点索引</param>
        /// <param name="targets">目标数据，数组长度为2，点长度为1的虚拟点</param>
        /// <param name="bindingWay">绑定方式，默认为单方向</param>
        public static bool UshortBinding(IPoint<int> source, int sourceindex, IPoint<ushort>[] targets, ByteOrder byterOrder = ByteOrder.None, BindingWay bindingWay = BindingWay.OneWay)
        {
            if (sourceindex >= source.Length)
            {
                return false;
            }
            if (targets.Length != 2)
            {
                return false;
            }
            switch (bindingWay)
            {
                case BindingWay.OneWay:
                    source.PropertyChanged += (s, p) =>
                    {
                        if (int.Parse(p.PropertyName) == sourceindex)
                        {
                            byte[] temp1 = UnsafeNetConvert.IntToBytes(source[(byte)sourceindex], byterOrder);
                            ushort[] temp2 = (byterOrder== ByteOrder.BigEndian)? UnsafeNetConvert.BytesToUShorts(temp1, 0, 2, byterOrder):UnsafeNetConvert.BytesToUShorts(temp1, 0, 2, ByteOrder.None);
                            for (int i = 0; i < targets.Length; i++)
                            {
                                targets[i].ValueUpdate(temp2[i], 0);
                            }
                        }
                    };
                    return true;
                case BindingWay.TwoWay:
                    source.PropertyChanged += (s, p) =>
                    {
                        if (int.Parse(p.PropertyName) == sourceindex)
                        {
                            byte[] temp1 = UnsafeNetConvert.IntToBytes(source[(byte)sourceindex], byterOrder);
                            ushort[] temp2 = (byterOrder == ByteOrder.BigEndian) ? UnsafeNetConvert.BytesToUShorts(temp1, 0, 2, byterOrder) : UnsafeNetConvert.BytesToUShorts(temp1, 0, 2, ByteOrder.None);
                            for (int i = 0; i < targets.Length; i++)
                            {
                                targets[i].ValueUpdate(temp2[i], 0);
                            }
                        }
                    };
                    foreach (var target in targets)
                    {
                        target.PropertyChanged += (s, p) =>
                        {
                            var temp1 = (byterOrder == ByteOrder.BigEndian) ? UnsafeNetConvert.UShortsToBytes(new ushort[] { targets[0][0], targets[1][0] }, byterOrder): UnsafeNetConvert.UShortsToBytes(new ushort[] { targets[0][0], targets[1][0] }, ByteOrder.None);
                            var temp2 = UnsafeNetConvert.BytesToInt(temp1, 0, byterOrder);
                            source.ValueUpdate(temp2, sourceindex);
                        };
                    }
                    return true;
                case BindingWay.ReverseWay:
                    foreach (var target in targets)
                    {
                        target.PropertyChanged += (s, p) =>
                        {
                            var temp1 = (byterOrder == ByteOrder.BigEndian) ? UnsafeNetConvert.UShortsToBytes(new ushort[] { targets[0][0], targets[1][0] }, byterOrder) : UnsafeNetConvert.UShortsToBytes(new ushort[] { targets[0][0], targets[1][0] }, ByteOrder.None);
                            var temp2 = UnsafeNetConvert.BytesToInt(temp1, 0, byterOrder);
                            source.ValueUpdate(temp2, sourceindex);
                        };
                    }
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// 16位无符号整形点绑定32无符号整形数据
        /// </summary>
        /// <param name="source">32位无符号整形源数据</param>
        /// <param name="sourceindex">源数据点索引</param>
        /// <param name="targets">目标数据，数组长度为2，点长度为1的虚拟点</param>
        /// <param name="bindingWay">绑定方式，默认为单方向</param>
        public static bool UshortBinding(IPoint<uint> source, int sourceindex, IPoint<ushort>[] targets, ByteOrder byteOrder = ByteOrder.None, BindingWay bindingWay = BindingWay.OneWay)
        {
            if (sourceindex >= source.Length)
            {
                return false;
            }
            if (targets.Length != 2)
            {
                return false;
            }
            switch (bindingWay)
            {
                case BindingWay.OneWay:
                    source.PropertyChanged += (s, p) =>
                    {
                        if (int.Parse(p.PropertyName) == sourceindex)
                        {
                            byte[] temp1 = UnsafeNetConvert.UIntToBytes(source[(byte)sourceindex], byteOrder);
                            ushort[] temp2 = (byteOrder==ByteOrder.BigEndian)?UnsafeNetConvert.BytesToUShorts(temp1, 0, 2, byteOrder): UnsafeNetConvert.BytesToUShorts(temp1, 0, 2, ByteOrder.None);
                            for (int i = 0; i < targets.Length; i++)
                            {
                                targets[i].ValueUpdate(temp2[i], 0);
                            }
                        }
                    };
                    return true;
                case BindingWay.TwoWay:
                    source.PropertyChanged += (s, p) =>
                    {
                        if (int.Parse(p.PropertyName) == sourceindex)
                        {
                            byte[] temp1 = UnsafeNetConvert.UIntToBytes(source[(byte)sourceindex], byteOrder);
                            ushort[] temp2 = (byteOrder == ByteOrder.BigEndian) ? UnsafeNetConvert.BytesToUShorts(temp1, 0, 2, byteOrder) 
                            : UnsafeNetConvert.BytesToUShorts(temp1, 0, 2, ByteOrder.None);
                            for (int i = 0; i < targets.Length; i++)
                            {
                                targets[i].ValueUpdate(temp2[i], 0);
                            }
                        }
                    };
                    foreach (var target in targets)
                    {
                        target.PropertyChanged += (s, p) =>
                        {
                            var temp1 = (byteOrder == ByteOrder.BigEndian) ? UnsafeNetConvert.UShortsToBytes(new ushort[] { targets[0][0], targets[1][0] }, byteOrder)
                            : UnsafeNetConvert.UShortsToBytes(new ushort[] { targets[0][0], targets[1][0] }, ByteOrder.None);
                            var temp2 = UnsafeNetConvert.BytesToUInt(temp1, 0, byteOrder);
                            source.ValueUpdate(temp2, sourceindex);
                        };
                    }
                    return true;
                case BindingWay.ReverseWay:
                    foreach (var target in targets)
                    {
                        target.PropertyChanged += (s, p) =>
                        {
                            var temp1 = (byteOrder == ByteOrder.BigEndian) ? UnsafeNetConvert.UShortsToBytes(new ushort[] { targets[0][0], targets[1][0] }, byteOrder)
                            : UnsafeNetConvert.UShortsToBytes(new ushort[] { targets[0][0], targets[1][0] }, ByteOrder.None);
                            var temp2 = UnsafeNetConvert.BytesToUInt(temp1, 0, byteOrder);
                            source.ValueUpdate(temp2, sourceindex);
                        };
                    }
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// 16位无符号整形点绑定16位整形数据
        /// </summary>
        /// <param name="source">16位整形源数据</param>
        /// <param name="sourceindex">源数据点索引</param>
        /// <param name="target">目标数据，点长度为1的虚拟点</param>
        /// <param name="bindingWay">绑定方式，默认为单方向</param>
        public static bool UshortBinding(IPoint<short> source, int sourceindex, IPoint<ushort> target,BindingWay bindingWay = BindingWay.OneWay)
        {
            if (sourceindex >= source.Length)
            {
                return false;
            }
            switch (bindingWay)
            {
                case BindingWay.OneWay:
                    source.PropertyChanged += (s, p) =>
                    {
                        if (int.Parse(p.PropertyName) == sourceindex)
                        {
                            byte[] temp1 = UnsafeNetConvert.ShortToBytes(source[(byte)sourceindex], ByteOrder.None);
                            ushort temp2 = UnsafeNetConvert.BytesToUShort(temp1, 0,  ByteOrder.None);
                            target.ValueUpdate(temp2,0);
                        }
                    };
                    return true;
                case BindingWay.TwoWay:
                    source.PropertyChanged += (s, p) =>
                    {
                        if (int.Parse(p.PropertyName) == sourceindex)
                        {
                            byte[] temp1 = UnsafeNetConvert.ShortToBytes(source[(byte)sourceindex], ByteOrder.None);
                            ushort temp2 = UnsafeNetConvert.BytesToUShort(temp1, 0, ByteOrder.None);
                            target.ValueUpdate(temp2, 0);
                        }
                    };
                    target.PropertyChanged += (s, p) =>
                       {
                           var temp1 = UnsafeNetConvert.UShortToBytes(target[0], ByteOrder.None);
                           var temp2 = UnsafeNetConvert.BytesToShort(temp1, 0, ByteOrder.None);
                           source.ValueUpdate(temp2, sourceindex);
                       };
                    return true;
                case BindingWay.ReverseWay:
                    target.PropertyChanged += (s, p) =>
                    {
                        var temp1 = UnsafeNetConvert.UShortToBytes(target[0], ByteOrder.None);
                        var temp2 = UnsafeNetConvert.BytesToShort(temp1, 0, ByteOrder.None);
                        source.ValueUpdate(temp2, sourceindex);
                    };
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// 16位无符号整形点绑定16位无符号整形数据
        /// </summary>
        /// <param name="source">16位无符号整形源数据</param>
        /// <param name="sourceindex">源数据点索引</param>
        /// <param name="target">目标数据，点长度为1的虚拟点</param>
        /// <param name="bindingWay">绑定方式，默认为单方向</param>
        public static bool UshortBinding(IPoint<ushort> source, int sourceindex, IPoint<ushort> target, BindingWay bindingWay = BindingWay.OneWay)
        {
            if (sourceindex >= source.Length)
            {
                return false;
            }
            switch (bindingWay)
            {
                case BindingWay.OneWay:
                    source.PropertyChanged += (s, p) =>
                    {
                        if (int.Parse(p.PropertyName) == sourceindex)
                        {
                            target.ValueUpdate(source[(byte)sourceindex], 0);
                        }
                    };
                    return true;
                case BindingWay.TwoWay:
                    source.PropertyChanged += (s, p) =>
                    {
                        if (int.Parse(p.PropertyName) == sourceindex)
                        {
                            target.ValueUpdate(source[(byte)sourceindex], 0);
                        }
                    };
                    target.PropertyChanged += (s, p) =>
                    {
                        source.ValueUpdate(target[0], sourceindex);
                    };
                    return true;
                case BindingWay.ReverseWay:
                    target.PropertyChanged += (s, p) =>
                    {
                        source.ValueUpdate(target[0], sourceindex);
                    };
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// 16位无符号整形点绑定16位无符号整形数据
        /// </summary>
        /// <param name="source">byte源数据,长度不小于2</param>
        /// <param name="sourceindex">源数据点索引</param>
        /// <param name="target">目标数据，点长度为1的虚拟点</param>
        /// <param name="bindingWay">绑定方式，默认为单方向</param>
        public static bool UshortBinding(IPoint<byte> source, int sourceindex, IPoint<ushort> target, BindingWay bindingWay = BindingWay.OneWay)
        {
            if (sourceindex+1 >= source.Length)
            {
                return false;
            }
            switch (bindingWay)
            {
                case BindingWay.OneWay:
                    source.PropertyChanged += (s, p) =>
                    {
                        if (int.Parse(p.PropertyName) == sourceindex)
                        {
                            var temp = UnsafeNetConvert.BytesToUShort(new byte[] { source[(byte)sourceindex], source[(byte)(sourceindex+1)] },0,ByteOrder.None);
                            target.ValueUpdate(temp, 0);
                        }
                    };
                    return true;
                case BindingWay.TwoWay:
                    source.PropertyChanged += (s, p) =>
                    {
                        if (int.Parse(p.PropertyName) == sourceindex)
                        {
                            var temp = UnsafeNetConvert.BytesToUShort(new byte[] { source[(byte)sourceindex], source[(byte)(sourceindex + 1)] }, 0, ByteOrder.None);
                            target.ValueUpdate(temp, 0);
                        }
                    };
                    target.PropertyChanged += (s, p) =>
                    {
                        var temp = UnsafeNetConvert.UShortToBytes(target[0], ByteOrder.None);
                        source.ValueUpdate(temp[0], sourceindex);
                        source.ValueUpdate(temp[1], sourceindex+1);
                    };
                    return true;
                case BindingWay.ReverseWay:
                    target.PropertyChanged += (s, p) =>
                    {
                        var temp = UnsafeNetConvert.UShortToBytes(target[0], ByteOrder.None);
                        source.ValueUpdate(temp[0], sourceindex);
                        source.ValueUpdate(temp[1], sourceindex + 1);
                    };
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 16位无符号整形点绑定16位无符号整形数据
        /// </summary>
        /// <param name="source">bool源数据</param>
        /// <param name="sourceindex">源数据点索引</param>
        /// <param name="target">目标数据，点长度为1的虚拟点</param>
        /// <param name="bindingWay">绑定方式，默认为单方向</param>
        public static bool BoolBinding(IPoint<bool> source, int sourceindex, IPoint<bool> target, BindingWay bindingWay = BindingWay.OneWay)
        {
            if (sourceindex >= source.Length)
            {
                return false;
            }
            switch (bindingWay)
            {
                case BindingWay.OneWay:
                    source.PropertyChanged += (s, p) =>
                    {
                        if (int.Parse(p.PropertyName) == sourceindex)
                        {
                            target.ValueUpdate(source[(byte)sourceindex],0);
                        }
                    };
                    return true;
                case BindingWay.TwoWay:
                    source.PropertyChanged += (s, p) =>
                    {
                        if (int.Parse(p.PropertyName) == sourceindex)
                        {
                            target.ValueUpdate(source[(byte)sourceindex], 0);
                        }
                    };
                    target.PropertyChanged += (s, p) =>
                    {
                       source.ValueUpdate(target[0],sourceindex) ;
                    };
                    return true;
                case BindingWay.ReverseWay:
                    target.PropertyChanged += (s, p) =>
                    {
                        source.ValueUpdate(target[0], sourceindex);
                    };
                    return true;
                default:
                    return false;
            }
        }
    }

    public enum BindingWay
    {
        OneWay,
        TwoWay,
        ReverseWay,
    }
}
