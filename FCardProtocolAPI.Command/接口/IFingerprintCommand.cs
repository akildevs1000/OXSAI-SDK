using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCardProtocolAPI.Command
{
    public interface IFingerprintCommand:IFcardCommand
    {
        /// <summary>
        /// 添加人员
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task<IFcardCommandResult> AddPerson(IFcardCommandParameter parameter);
        /// <summary>
        /// 删除人员
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task<IFcardCommandResult> DeletePerson(IFcardCommandParameter parameter);
        /// <summary>
        /// 获取人员详情
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task<IFcardCommandResult> GetPersonDetail(IFcardCommandParameter parameter);
        /// <summary>
        /// 获取工作参数
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task<IFcardCommandResult> GetWorkParam(IFcardCommandParameter parameter);
        /// <summary>
        /// 设置工作参数
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task<IFcardCommandResult> SetWorkParam(IFcardCommandParameter parameter);

        Task<IFcardCommandResult> CommandTest(IFcardCommandParameter parameter);
    }
}
