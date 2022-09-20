using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCardProtocolAPI.Command
{
    public interface IFcardCommandResult
    {
        /// <summary>
        /// 返回结果
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// 命令
        /// </summary>
    //    public string Command { get; set; }
        /// <summary>
        /// 返回结果状态
        /// </summary>
        public CommandStatus Status { get; set; }
        /// <summary>
        /// 返回消息
        /// </summary>
        public string Message { get; set; }
    }
}
