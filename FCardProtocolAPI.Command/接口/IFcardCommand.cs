using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCardProtocolAPI.Command
{
    public interface IFcardCommand
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task<IFcardCommandResult> OpenDoor(IFcardCommandParameter parameter);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task<IFcardCommandResult> CloseDoor(IFcardCommandParameter parameter);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task<IFcardCommandResult> HoldDoor(IFcardCommandParameter parameter);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task<IFcardCommandResult> GetRecord(IFcardCommandParameter parameter);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task<IFcardCommandResult> ResetRecord(IFcardCommandParameter parameter);
    }
}
