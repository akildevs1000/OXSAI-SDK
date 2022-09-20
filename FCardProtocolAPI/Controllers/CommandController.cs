using DoNetDrive.Core;
using FCardProtocolAPI.Command;
using FCardProtocolAPI.Command.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace FCardProtocolAPI.Controllers
{
    //  [Route("api/[controller]")]
    [ApiController]
    public class CommandController : BaseController
    {
        public CommandController(IServiceProvider provider, Command.IDoor8900HCommand door8900H, Command.IFingerprintCommand fingerprint) : base(provider, door8900H, fingerprint)
        {

        }
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <returns></returns>
        [HttpPost("{SN}/{Command}")]
        public async Task<Command.IFcardCommandResult> Command()
        {
            try
            {
                if (!Request.RouteValues.ContainsKey("SN"))
                {
                    return GetCommandResultInstance("缺少设备SN", FCardProtocolAPI.Command.CommandStatus.ParameterError);
                }
                if (!Request.RouteValues.ContainsKey("Command"))
                {
                    return GetCommandResultInstance("缺少请求命令", FCardProtocolAPI.Command.CommandStatus.ParameterError);
                }
                var sn = Request.RouteValues["SN"]?.ToString();
                var commandName = GetDeviceType(sn);
                if (commandName != CommandAllocator.Door8900HName && commandName != CommandAllocator.FingerprintCommandName)
                {
                    return GetCommandResultInstance("没有对应的命令", FCardProtocolAPI.Command.CommandStatus.CommandError);
                }
                var command = Request.RouteValues["Command"]?.ToString();
                var body = ReadBody();
                var commandDetail = CommandAllocator.GetCommandDetail(sn);
                if (commandDetail == null)
                {
                    return GetCommandResultInstance("设备未连接到服务器或者未注册", FCardProtocolAPI.Command.CommandStatus.CommandError);
                }
                var parameter = _Provider.GetService(typeof(Command.IFcardCommandParameter)) as Command.IFcardCommandParameter;
                parameter.Command = command;
                parameter.Sn = sn;
                parameter.Data = body;
                parameter.CommandDetail = commandDetail;
                parameter.Allocator = CommandAllocator.Allocator;
                Command.IFcardCommand iCommand;
                if (commandName == CommandAllocator.Door8900HName)
                {
                    iCommand = _Door8900H;
                }
                else
                {
                    iCommand = _FingerprintCommand;
                }
                return await ExecutiveCommand(iCommand, parameter);
            }
            catch (Exception ex)
            {
                if (ex.Message.Equals("CommandStatus_Timeout"))
                {
                    return GetCommandResultInstance("命令超时", FCardProtocolAPI.Command.CommandStatus.CommonTimeout);
                }
                else
                {
                    return GetCommandResultInstance("命令错误：" + ex.Message, FCardProtocolAPI.Command.CommandStatus.CommandError);
                }
            }
        }
        /// <summary>
        /// websocket 连接
        /// </summary>
        /// <returns></returns>
        [HttpGet("/WebSocket")]
        public async Task WebSocket()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                //获取websocket对象
                var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                var key = HttpContext.Connection.RemoteIpAddress + ":" + HttpContext.Connection.RemotePort;
                CommandAllocator.WebSockets.TryAdd(key, webSocket);
                Console.WriteLine("websocket 客户端连接：" + key);
                while (true)
                {
                    try
                    {
                        byte[] recvBuffer = new byte[1024];
                        var recvAs = new ArraySegment<byte>(recvBuffer);
                        await webSocket.ReceiveAsync(recvAs, CancellationToken.None);
                    }
                    catch
                    {
                        break;
                    }
                }
                CommandAllocator.WebSockets.TryRemove(key, out _);
                Console.WriteLine("websocket 客户端连接关闭：" + key);
            }
            else
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        }
        /// <summary>
        /// 注册设备
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpPost("Register")]
        public Command.IFcardCommandResult Register([FromBody] Command.FcardCommandParameter parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter.Sn))
            {
                return GetCommandResultInstance("设备Sn不能为空", FCardProtocolAPI.Command.CommandStatus.ParameterError);
            }
            if (string.IsNullOrWhiteSpace(parameter.Ip))
            {
                return GetCommandResultInstance("设备Ip不能为空", FCardProtocolAPI.Command.CommandStatus.ParameterError);
            }

            var paht = "Devices.json";
            var jsonStr = System.IO.File.ReadAllText(paht);
            var deviceinfos = JsonConvert.DeserializeObject<FileDevicesInfo>(jsonStr);
            if (parameter.Port == null || parameter.Port == 0)
            {
                var typeName = CommandAllocator.GetDeviceTypeName(parameter.Sn);
                if (typeName.Equals(CommandAllocator.Door8900HName))
                    parameter.Port = CommandAllocator.TCPPort;
                else
                    parameter.Port = CommandAllocator.UDPPort;
            }
            DevicesInfo deviceinfo = new DevicesInfo
            {
                SN = parameter.Sn,
                IP = parameter.Ip,
                Port = (int)parameter.Port
            };
            //判断之前是否已经注册，已经注册就将内容进行替换
            if (CommandAllocator.DevicesInfos.ContainsKey(parameter.Sn))
            {
                CommandAllocator.DevicesInfos[parameter.Sn] = deviceinfo;
                deviceinfos.DevicesInfos.RemoveAll((a) => a.SN.Equals(parameter.Sn));
            }
            else
            {
                CommandAllocator.DevicesInfos.TryAdd(parameter.Sn, deviceinfo);
            }
            if (deviceinfos.DevicesInfos == null)
                deviceinfos.DevicesInfos = new List<DevicesInfo>();
            deviceinfos.DevicesInfos.Add(deviceinfo);
            string json = JsonConvert.SerializeObject(deviceinfos);
            System.IO.File.WriteAllText(paht, json);
            Console.WriteLine("注册设备：" + json);
            return GetCommandResultInstance("设备注册成功", FCardProtocolAPI.Command.CommandStatus.Succeed);
        }
        /// <summary>
        /// 获取设备列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetDevices")]
        public Command.IFcardCommandResult GetDevices()
        {
            var list = new List<object>();
            //获取tcp方式连接的设备
            list.AddRange(CommandAllocator.TCPServerClientList.Select(a => new { SN = a.Key, IP = a.Value.Remote.Addr, Port = a.Value.Remote.Port }));
            //获取UPD方式连接的设备
            list.AddRange(CommandAllocator.UDPServerClientList.Select(a => new { SN = a.Key, IP = a.Value.Addr, Port = a.Value.Port }));
            //获取本地局域网注册的设备
            list.AddRange(CommandAllocator.DevicesInfos.Select(a => new { SN = a.Key, IP = a.Value.IP, Port = a.Value.Port }));
            return GetCommandResultInstance("查询成功", FCardProtocolAPI.Command.CommandStatus.Succeed, list);
        }
    }
}
