using DoNetDrive.Core;
using DoNetDrive.Core.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCardProtocolAPI.Command
{
    public interface IFcardCommandParameter
    {
        bool Checked();
        /// <summary>
        /// 命令
        /// </summary>
        public string Command { get; set; }
        /// <summary>
        /// 设备SN
        /// </summary>
        public string Sn { get; set; }
        /// <summary>
        /// 设备IP
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// 设备端口
        /// </summary>
        public int? Port { get; set; }
        /// <summary>
        /// 超时时间
        /// </summary>
        public int? Timeout { get; set; }
        /// <summary>
        /// 命令参数
        /// </summary>
        public string Data { get; set; }
        /// <summary>
        /// 命令详情
        /// </summary>
        public INCommandDetail CommandDetail { get; set; }

        public ConnectorAllocator Allocator { get; set; }
    }
}
