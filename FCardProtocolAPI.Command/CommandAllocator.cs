using DoNetDrive.Core;
using DoNetDrive.Core.Command;
using DoNetDrive.Core.Connector;
using DoNetDrive.Core.Connector.TCPClient;
using DoNetDrive.Core.Connector.TCPServer;
using DoNetDrive.Core.Connector.TCPServer.Client;
using DoNetDrive.Core.Connector.UDP;
using DoNetDrive.Core.Data;
using DoNetDrive.Protocol;
using DoNetDrive.Protocol.Door8800;
using DoNetDrive.Protocol.Fingerprint.AdditionalData;
using DoNetDrive.Protocol.Fingerprint.Data.Transaction;
using DoNetDrive.Protocol.Fingerprint.SystemParameter;
using DoNetDrive.Protocol.Fingerprint.Transaction;
using DoNetDrive.Protocol.Transaction;
using FCardProtocolAPI.Command;
using FCardProtocolAPI.Command.Models;
using FCardProtocolAPI.Common;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace FCardProtocolAPI.Command
{
    public class CommandAllocator
    {
        public static readonly ConnectorAllocator Allocator = ConnectorAllocator.GetAllocator();
        /// <summary>
        /// 设备类型 门禁控制板或人脸指纹机
        /// </summary>
        public static Dictionary<string, string> DeviceType { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// 已注册的设备信息
        /// </summary>
        public static ConcurrentDictionary<string, DevicesInfo> DevicesInfos;
        /// <summary>
        /// 已连接的websocket列表
        /// </summary>
        public static ConcurrentDictionary<string, WebSocket> WebSockets;
        /// <summary>
        /// tcp 客户端列表
        /// </summary>

        public static ConcurrentDictionary<string, TCPServerClientDetail> TCPServerClientList = new ConcurrentDictionary<string, TCPServerClientDetail>();
        /// <summary>
        /// udp 客户端列表
        /// </summary>

        public static ConcurrentDictionary<string, TCPClientDetail> UDPServerClientList = new ConcurrentDictionary<string, TCPClientDetail>();

        static ConcurrentDictionary<int, FaceTransaction> FaceTransactions = new ConcurrentDictionary<int, FaceTransaction>();
        /// <summary>
        /// 设备通讯密码
        /// </summary>
        public static string ConnectionPassword { get; set; }
        /// <summary>
        /// 本地IP
        /// </summary>
        public static string LocalIP { get; set; }
        /// <summary>
        /// udp服务器端口
        /// </summary>
        public static int UDPServerPort { get; set; }
        /// <summary>
        /// tcp服务器端口
        /// </summary>
        public static int TCPServerPort { get; set; }
        /// <summary>
        /// 设备UDP端口
        /// </summary>
        public static int UDPPort { get; set; }
        /// <summary>
        /// 设备TCP端口
        /// </summary>
        public static int TCPPort { get; set; }
        private static int Timeout { get; set; }
        private static int RestartCount { get; set; }

        public static string Door8900HName = nameof(IDoor8900HCommand);
        public static string FingerprintCommandName = nameof(IFingerprintCommand);


        public async static Task Init(IConfiguration configuration)
        {
            //  ServiceProvider = provider;
            WebSockets = new ConcurrentDictionary<string, WebSocket>();
            configuration.GetSection("EquipmentType").Bind(DeviceType);
            var devicesInfoList = new List<DevicesInfo>();
            configuration.GetSection("DevicesInfos").Bind(devicesInfoList);
            DevicesInfos = new ConcurrentDictionary<string, DevicesInfo>(devicesInfoList.ToDictionary(a => a.SN, a => a));
            LocalIP = configuration["LocalIP"];
            UDPServerPort = 9001;            //UDPServerPort = int.Parse(configuration["UDPServerPort"]);

            TCPServerPort = 8000; // int.Parse(configuration["TCPServerPort"]);
            UDPPort = int.Parse(configuration["UDPPort"]);
            TCPPort = int.Parse(configuration["TCPPort"]);
            Timeout = int.Parse(configuration["Timeout"]);
            RestartCount = int.Parse(configuration["RestartCount"]);
            ConnectionPassword = configuration["ConnectionPassword"];

            Allocator.TransactionMessage += MAllocator_TransactionMessage;
            Allocator.ClientOnline += MAllocator_ClientOnline;
            Allocator.ClientOffline += MAllocator_ClientOffline;
            #region Udp服务器绑定
            UDPServerDetail udpDetail = new UDPServerDetail(LocalIP, UDPServerPort);
            await Allocator.OpenConnectorAsync(udpDetail);
            Console.WriteLine("绑定UDP服务器:" + UDPServerPort);
            #endregion

            #region tcp服务器绑定

            var tcp = new TCPServerDetail(LocalIP, TCPServerPort);
            await Allocator.OpenConnectorAsync(tcp);
            Console.WriteLine("绑定TCP服务器:" + TCPServerPort);
            #endregion


        }

        public static INCommandDetail GetCommandDetail(string sn, object userData = null)
        {
            INCommandDetail commandDetail = null;
            if (TCPServerClientList.ContainsKey(sn))
            {
                var tcp = TCPServerClientList[sn];
                commandDetail = GetCommandDetail(tcp, sn);
            }
            else if (UDPServerClientList.ContainsKey(sn))
            {
                var upd = UDPServerClientList[sn];

                commandDetail = GetCommandDetail(upd, sn);
            }
            else if (DevicesInfos.ContainsKey(sn))
            {
                var detail = DevicesInfos[sn];
                var connectType = CommandDetailFactory.ConnectType.UDPClient;
                if (GetDeviceTypeName(sn).Equals(Door8900HName))
                {
                    connectType = CommandDetailFactory.ConnectType.TCPClient;
                }
                commandDetail = GetCommandDetail(connectType, detail.SN, detail.IP, detail.Port);
            }
            if (commandDetail != null)
                commandDetail.UserData = userData;
            return commandDetail;
        }
        public static string GetDeviceTypeName(string sn)
        {
            var key = sn.Substring(0, 8);
            if (DeviceType.ContainsKey(key))
            {
                return DeviceType[key];
            }
            return string.Empty;
        }
        private static INCommandDetail GetCommandDetail(TCPClientDetail clientDetail, string sn)
        {
            if (clientDetail is TCPServerClientDetail)
            {
                var connectType = CommandDetailFactory.ConnectType.TCPServerClient;
                return GetCommandDetail(connectType, sn, clientDetail.GetKey(), 0);
            }
            else
            {
                var connectType = CommandDetailFactory.ConnectType.UDPClient;
                return GetCommandDetail(connectType, sn, clientDetail.Addr, clientDetail.Port);
            }
        }
        /// <summary>
        /// 获取连接对接
        /// </summary>
        /// <param name="connectType"></param>
        /// <param name="sn"></param>
        /// <param name="addr"></param>
        /// <param name="udpPort"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private static INCommandDetail GetCommandDetail(CommandDetailFactory.ConnectType connectType, string sn, string addr, int udpPort)
        {
            //   var password = "FFFFFFFF";
            var protocolType = CommandDetailFactory.ControllerType.Door89H;
            var cmdDtl = CommandDetailFactory.CreateDetail(connectType, addr, udpPort,
                protocolType, sn, ConnectionPassword);

            if (connectType == CommandDetailFactory.ConnectType.UDPClient)
            {
                var dtl = cmdDtl.Connector as TCPClientDetail;
                dtl.LocalAddr = LocalIP;
                dtl.LocalPort = UDPServerPort;
            }
            cmdDtl.Timeout = Timeout;
            cmdDtl.RestartCount = RestartCount;
            return cmdDtl;
        }
        private static void MAllocator_ClientOffline(object sender, ServerEventArgs e)
        {
            var inc = sender as INConnector;
            var connectorType = inc.GetConnectorType();
            var key = inc.GetKey();
            if (connectorType == ConnectorType.TCPServerClient)
            {
                var keys = TCPServerClientList.Keys;
                foreach (var item in keys)
                {
                    if (key == TCPServerClientList[item].GetKey())
                    {
                        TCPServerClientList.Remove(item, out _);
                        break;
                    }
                }
            }
            else
            if (connectorType == ConnectorType.UDPClient)
            {
                var keys = UDPServerClientList.Keys;
                foreach (var item in keys)
                {
                    if (key == UDPServerClientList[item].GetKey())
                    {
                        UDPServerClientList.Remove(item, out _);
                        break;
                    }
                }
            }
            Console.WriteLine(inc.GetConnectorType() + ":客户端离线");

        }

        private static void MAllocator_ClientOnline(object sender, ServerEventArgs e)
        {
            var inc = sender as INConnector;
            var connectorType = inc.GetConnectorType();
            Console.WriteLine(inc.RemoteAddress().ToString() + ":客户端上线");
            var key = inc.GetKey();
            switch (connectorType)
            {
                case ConnectorType.TCPServerClient://tcp 客户端已连接
                case ConnectorType.UDPClient://UDP客户端已连接
                    var fC8800Request = new Door8800RequestHandle(DotNetty.Buffers.UnpooledByteBufferAllocator.Default, RequestHandleFactory);
                    inc.RemoveRequestHandle(typeof(Door8800RequestHandle));//先删除，防止已存在就无法添加。
                    inc.AddRequestHandle(fC8800Request);
                    break;
                default:
                    break;
            }
        }

        private static AbstractTransaction RequestHandleFactory(string sn, byte cmdIndex, byte cmdPar)
        {
            if (cmdIndex >= 1 && cmdIndex <= 4)
            {
                if (GetDeviceTypeName(sn).Equals(Door8900HName))
                {
                    return DoNetDrive.Protocol.Door.Door89H.Transaction.ReadTransactionDatabaseByIndex.NewTransactionTable[cmdIndex]();
                }
                else
                {
                    return ReadTransactionDatabaseByIndex.NewTransactionTable[cmdIndex]();
                }
            }
            if (cmdIndex >= 5 && cmdIndex <= 6)
            {
                return DoNetDrive.Protocol.Door.Door89H.Transaction.ReadTransactionDatabaseByIndex.NewTransactionTable[cmdIndex]();
            }
            if (cmdIndex == 0x22)
            {
                return new DoNetDrive.Protocol.Door.Door8800.Data.Transaction.KeepaliveTransaction();
            }

            if (cmdIndex == 0xA0)
            {
                return new DoNetDrive.Protocol.Door.Door8800.Data.Transaction.ConnectMessageTransaction();
            }
            return null;
        }

        private static void MAllocator_TransactionMessage(INConnectorDetail connector, INData EventData)
        {
            var conn = Allocator.GetConnector(connector);
            if (conn != null)
            {
                if (!conn.IsForciblyConnect())
                {
                    conn.OpenForciblyConnect();
                }
            }
            Door8800Transaction fcTrn = EventData as Door8800Transaction;
            AddClient(conn, fcTrn.SN);
            switch (fcTrn.CmdIndex)
            {
                case 0x01:
                case 0x04:
                    if (WebSockets.Count > 0)
                        CardTransaction(fcTrn);
                    break;
                case 0x02:
                case 0x03:
                    if (WebSockets.Count > 0)
                        DefaultTransaction(fcTrn);
                    break;
                case 0x22:
                case 0xA0:
                    var sndConntmsg = new SendConnectTestResponse(GetCommandDetail(fcTrn.SN));
                    Allocator.AddCommand(sndConntmsg);
                    break;
                default:
                    break;
            }
        }

        private static void CardTransaction(Door8800Transaction transaction)
        {
            var name = GetDeviceTypeName(transaction.SN);
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }
            if (name.Equals(Door8900HName))
            {
                Door89HCardTransaction(transaction);
                return;
            }
            FaceTransaction record;
            if (FaceTransactions.ContainsKey(transaction.EventData.SerialNumber))
            {
                record = FaceTransactions[transaction.EventData.SerialNumber];
            }
            else
            {
                record = new FaceTransaction();
                FaceTransactions.TryAdd(transaction.EventData.SerialNumber, record);
            }
            if (transaction.CmdIndex == 0x01)
            {
                var cardtrn = transaction.EventData as CardTransaction;
                record.RecordNumber = cardtrn.SerialNumber;
                record.Accesstype = cardtrn.Accesstype;
                record.Photo = cardtrn.Photo;
                record.UserCode = cardtrn.UserCode;
                record.RecordDate = cardtrn.TransactionDate;
                record.RecordType = cardtrn.TransactionType;
                record.DeviceID = transaction.SN;
                record.RecordMsg = MessageType.TransactionCodeNameList[cardtrn.TransactionType][cardtrn.TransactionCode];
                if (record.Photo == 0)
                {
                    SendWebSocketMessage(GetResult(Command.CommandStatus.Succeed, record), record.RecordNumber);
                    return;
                }
                var detail = GetCommandDetail(transaction.SN);
                var cmd = new ReadFile(detail,
                    new ReadFile_Parameter(record.RecordNumber, 3, 1));
                detail.CommandCompleteEvent += (s, e) =>
                {
                    var result = e.Result as ReadFile_Result;
                    if (result.FileHandle != 0 && result.Result)
                    {
                        record.RecordImage = result.FileDatas;
                    }
                    SendWebSocketMessage(GetResult(Command.CommandStatus.Succeed, record), record.RecordNumber);
                };
                detail.CommandTimeout += (s, e) =>
                  {
                      SendWebSocketMessage(GetResult(Command.CommandStatus.Succeed, record), record.RecordNumber);
                  };
                Allocator.AddCommand(cmd);
            }
            if (transaction.CmdIndex == 0x04)
            {
                var cardtrn = transaction.EventData as BodyTemperatureTransaction;
                record.BodyTemperature = (double)cardtrn.BodyTemperature / 10;
            }
        }


        private static void Door89HCardTransaction(Door8800Transaction transaction)
        {
            var record = new Models.CardRecord();
            if (transaction.CmdIndex == 1)
            {
                var result = transaction.EventData as DoNetDrive.Protocol.Door.Door89H.Data.CardTransaction;
                record.RecordNumber = result.SerialNumber;
                record.Accesstype = (byte)(result.IsEnter() ? 1 : 2);
                record.CardData = result.BigCard.UInt64Value.ToString();
                record.RecordDate = result.TransactionDate.ToString("yyyy-MM-dd HH:mm:ss");
                record.RecordType = result.TransactionType;
                record.RecordMsg = MessageType.CardTransactionCodeList[result.TransactionType][result.TransactionCode];
                record.Door = result.DoorNum();
                SendWebSocketMessage(GetResult(Command.CommandStatus.Succeed, record), 0);
            }


        }
        private static IFcardCommandResult GetResult(Command.CommandStatus status, object data = null)
        {
            var result = new FcardCommandResult();
            result.Message = "success response";
            result.Status = status;
            result.Data = data;
            return result;
        }
        private static void DefaultTransaction(Door8800Transaction transaction)
        {
            FaceTransaction record = new FaceTransaction
            {
                RecordNumber = transaction.EventData.SerialNumber,
                RecordDate = transaction.EventData.TransactionDate,
                RecordType = transaction.EventData.TransactionType,
                RecordMsg = MessageType.TransactionCodeNameList[transaction.EventData.TransactionType][transaction.EventData.TransactionCode]
            };
            SendWebSocketMessage(GetResult(CommandStatus.Succeed, record), 0);
        }
        private static void SendWebSocketMessage(IFcardCommandResult result, int serialNumber)
        {
            var message = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result));
            var buf = new ArraySegment<byte>(message);
            foreach (var item in WebSockets)
            {
                item.Value.SendAsync(buf, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            if (FaceTransactions.ContainsKey(serialNumber))
            {
                FaceTransactions.Remove(serialNumber, out _);
            }
        }
        /// <summary>
        /// 添加连接客户端
        /// </summary>
        /// <param name="inc"></param>
        /// <param name="sn"></param>
        private static void AddClient(INConnector inc, string sn)
        {
            switch (inc.GetConnectorType())
            {
                case ConnectorType.TCPServerClient://tcp 客户端已连接
                    TCPServerClientDetail clientDetail = inc.GetConnectorDetail() as TCPServerClientDetail;
                    if (!TCPServerClientList.ContainsKey(sn))
                    {
                        TCPServerClientList.TryAdd(sn, clientDetail);
                    }
                    break;
                case ConnectorType.UDPClient:
                    var UdpClientDetail = inc.GetConnectorDetail() as TCPClientDetail;
                    if (!UDPServerClientList.ContainsKey(sn))
                    {
                        UDPServerClientList.TryAdd(sn, UdpClientDetail);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
