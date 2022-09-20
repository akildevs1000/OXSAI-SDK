using DoNetDrive.Protocol.Fingerprint.SystemParameter.OEM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCardProtocolAPI.Command.Models
{
    public class WorkParameter
    {
        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 设备进出方向
        /// </summary>
        public byte? Door { get; set; }
        /// <summary>
        /// OEM信息
        /// </summary>
        public OEMDetail Maker { get; set; }
        /// <summary>
        /// 语言类型
        /// </summary>
        public byte? Language { get; set; }
        /// <summary>
        /// 音量
        /// </summary>
        public byte? Volume { get; set; }
        /// <summary>
        /// 语音播报
        /// </summary>
      //  public byte? Speek { get; set; }
        /// <summary>
        /// 菜单管理密码
        /// </summary>
        public string MenuPassword { get; set; }
        /// <summary>
        /// 自定义数据
        /// </summary>
     //   public string CustomData { get; set; }

        /// <summary>
        /// 现场照片保存开关
        /// </summary>
        public byte? SavePhoto { get; set; }
        /// <summary>
        /// 监控状态
        /// </summary>
        public byte? MsgPush { get; set; }
        /// <summary>
        /// 设备时间
        /// </summary>
        public DateTime? Time { get; set; }
    }
}
