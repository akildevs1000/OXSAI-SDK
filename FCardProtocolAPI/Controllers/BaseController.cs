using FCardProtocolAPI.Command;
using Microsoft.AspNetCore.Mvc;

namespace FCardProtocolAPI.Controllers
{
    public class BaseController : ControllerBase
    {
        protected IServiceProvider _Provider;
        protected Command.IDoor8900HCommand _Door8900H;
        protected Command.IFingerprintCommand _FingerprintCommand;
        public BaseController(IServiceProvider provider, Command.IDoor8900HCommand door8900H, Command.IFingerprintCommand fingerprint)
        {
            _Provider = provider;
            _FingerprintCommand = fingerprint;
            _Door8900H = door8900H;
        }
        protected string GetDeviceType(string sn)
        {
            var key = sn.Substring(0, 8);
            if (CommandAllocator.DeviceType.ContainsKey(key))
            {
                return CommandAllocator.DeviceType[key];
            }
            return string.Empty;
        }

        /// <summary>
        /// 读取参数内容
        /// </summary>
        /// <returns></returns>
        protected string ReadBody()
        {
            string data = null;
            try
            {
                Request.EnableBuffering();
                using var requestReader = new StreamReader(Request.Body, encoding: System.Text.Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
                data = requestReader.ReadToEndAsync().Result;
            }
            catch
            {
            }
            return data;
        }
        /// <summary>
        /// 获取返回结果实例
        /// </summary>
        /// <param name="message"></param>
        /// <param name="status"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected Command.IFcardCommandResult GetCommandResultInstance(string message, Command.CommandStatus status, object data = null)
        {
            var result = _Provider.GetService(typeof(Command.IFcardCommandResult)) as Command.IFcardCommandResult;
            result.Message = message;
            result.Status = status;
            result.Data = data;
            return result;
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="t"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected async Task<Command.IFcardCommandResult> ExecutiveCommand(Command.IFcardCommand iCommand, Command.IFcardCommandParameter parameter)
        {
            var mt = iCommand.GetType().GetMethod(parameter.Command);
            if (mt == null)
            {
                return GetCommandResultInstance("没有对应的命令", FCardProtocolAPI.Command.CommandStatus.CommandError);
            }
            var command = mt.Invoke(iCommand, new object[] { parameter }) as Task<Command.IFcardCommandResult>;
            return await command;
        }
    }
}
