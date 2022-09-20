using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCardProtocolAPI.Command
{
    public interface IDoor8900HCommand:IFcardCommand
    {
        /// <summary>
        /// 添加卡号 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task<IFcardCommandResult> AddCard(IFcardCommandParameter parameter);
        /// <summary>
        /// 获取卡号信息
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task<IFcardCommandResult> GetCardInfo(IFcardCommandParameter parameter);
        /// <summary>
        /// 删除卡号
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task<IFcardCommandResult> DeleteCard(IFcardCommandParameter parameter);
    }
}
