using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer.Permission
{
    public enum ResourceType
    {
        /// <summary>
        /// 最高权限
        /// </summary>
        Sys_Admin = 0x01,
        /// <summary>
        /// 左下菜单栏
        /// </summary>
        Btn_Menu_BL = 0x02,
        /// <summary>
        /// 右上菜单栏
        /// </summary>
        Btn_Menu_TR = 0x03,
        /// <summary>
        /// 导航栏
        /// </summary>
        Btn_Navigation =0x03

    }
}
