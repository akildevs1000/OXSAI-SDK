using DoNetDrive.Core.Command;
using DoNetDrive.Protocol.Door.Door8800.Card;
using DoNetDrive.Protocol.Door.Door8800.Door;
using DoNetDrive.Protocol.Door.Door8800.Door.Remote;
using DoNetDrive.Protocol.Door.Door8800.Transaction;
using DoNetDrive.Protocol.Door.Door89H.Data;
using FCardProtocolAPI.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCardProtocolAPI.Command
{
    public class Door8900HCommand : IDoor8900HCommand
    {
      
        /// <summary>
        /// 新增卡号
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public async Task<IFcardCommandResult> AddCard(IFcardCommandParameter parameter)
        {
            var check = CheckParameter(parameter, out var commandParameter);
            if (!check.Item1)
            {
                return new FcardCommandResult
                {
                    Message = check.Item2,
                    Status = CommandStatus.ParameterError
                };
            }
            INCommand cmd;
            if (commandParameter is DoNetDrive.Protocol.Door.Door89H.Card.WriteCardListBySequence_Parameter)
            {
                cmd = new DoNetDrive.Protocol.Door.Door89H.Card.WriteCardListBySequence(parameter.CommandDetail, (DoNetDrive.Protocol.Door.Door89H.Card.WriteCardListBySequence_Parameter)commandParameter);
            }
            else
            {
                cmd = new DoNetDrive.Protocol.Door.Door89H.Card.WriteCardListBySort(parameter.CommandDetail, (DoNetDrive.Protocol.Door.Door89H.Card.WriteCardListBySort_Parameter)commandParameter);
            }
            await parameter.Allocator.AddCommandAsync(cmd);
            var result = cmd.getResult() as WriteCardList_Result;
            return new FcardCommandResult
            {
                Message = result.FailTotal > 0 ? "上传失败" : "上传成功",
                Status = result.FailTotal > 0 ? CommandStatus.CommandError : CommandStatus.Succeed,
                Data = result
            };
        }
        /// <summary>
        /// 检查卡信息
        /// </summary>
        /// <param name="iParameter"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private (bool, string) CheckParameter(IFcardCommandParameter iParameter, out DoNetDrive.Protocol.Door.Door8800.AbstractParameter parameter)
        {
            parameter = null;
            (bool, string) result = (false, null);
            try
            {

                if (string.IsNullOrWhiteSpace(iParameter.Data))
                {
                    result.Item2 = "卡信息错误";
                    return result;
                }

                var addCardModel = JsonConvert.DeserializeObject<Models.AddCardModel>(iParameter.Data);
                List<CardDetail> listDetails = new List<CardDetail>();
                foreach (var item in addCardModel.Cards)
                {
                    if (decimal.TryParse(item.CardData, out var cardData))
                    {
                        if (cardData < 0x01 || cardData >= 0xFFFFFFFFFFFFFFFF)
                        {
                            result.Item2 = "卡号的有效返回是0x01-0xFFFFFFFFFFFFFFFF";
                            return result;
                        }
                    }
                    else
                    {
                        result.Item2 = "卡号只能是整数数字";
                        return result;
                    }
                    var cardInfo = new CardDetail();
                    cardInfo.BigCard.BigValue = cardData;
                    if (!string.IsNullOrWhiteSpace(item.Password))
                    {
                        if (!int.TryParse(item.Password, out var password))
                        {
                            result.Item2 = "密码只能是4-8位的整数数字";
                            return result;
                        }
                        cardInfo.Password = item.Password;
                    }
                    var time = DateTime.Parse("2089-12-31");
                    cardInfo.Expiry = time;
                    if (item.Expiry != null)
                    {
                        if (item.Expiry > time)
                        {
                            item.Expiry = time;
                        }
                        cardInfo.Expiry = (DateTime)item.Expiry;
                    }
                    if (item.TimeGroup != null)
                    {
                        for (int i = 0; i < item.TimeGroup.Length; i++)
                        {
                            cardInfo.SetTimeGroup(i + 1, item.TimeGroup[i]);
                        }
                    }
                    if (item.Doors != null)
                    {
                        cardInfo.SetDoor(1, item.Doors.Door1);
                        cardInfo.SetDoor(2, item.Doors.Door2);
                        cardInfo.SetDoor(3, item.Doors.Door3);
                        cardInfo.SetDoor(4, item.Doors.Door4);
                    }
                    if (item.OpenTimes != null)
                    {
                        if (item.OpenTimes < 0 || item.OpenTimes > 65535)
                        {
                            result.Item2 = "有效次数有误";
                            return result;
                        }
                        cardInfo.OpenTimes = (int)item.OpenTimes;
                    }
                    cardInfo.HolidayUse = false;
                    listDetails.Add(cardInfo);

                }
                if (addCardModel.AreaType == 0)
                {
                    parameter = new DoNetDrive.Protocol.Door.Door89H.Card.WriteCardListBySequence_Parameter(listDetails);
                }
                else
                {
                    //   listDetails = listDetails.OrderBy(a => a.BigCard).ToList();
                    parameter = new DoNetDrive.Protocol.Door.Door89H.Card.WriteCardListBySort_Parameter(listDetails);
                }
                result.Item1 = true;
            }
            catch
            {
                result.Item2 = "数据解析错误";
            }
            return result;
        }
        /// <summary>
        /// 远程关门
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public async Task<IFcardCommandResult> CloseDoor(IFcardCommandParameter parameter)
        {
                CloseDoor cmd = new CloseDoor(parameter.CommandDetail, new Remote_Parameter(GetDoorDetail(parameter)));
                await parameter.Allocator.AddCommandAsync(cmd);
                return new FcardCommandResult()
                {
                    Message = "关门成功",
                    Status = CommandStatus.Succeed
                };           
        }
        /// <summary>
        /// 删除卡号
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IFcardCommandResult> DeleteCard(IFcardCommandParameter parameter)
        {
            var data = JObject.Parse(parameter.Data.ToString());
            Dictionary<string, object> d = new Dictionary<string, object>(data.ToObject<IDictionary<string, object>>(), StringComparer.CurrentCultureIgnoreCase);
            if (!d.ContainsKey("CardArray"))
            {
                return new FcardCommandResult
                {
                    Message = "删除卡参数错误,请将卡号以JSON数组的方式写入到body",
                    Status = CommandStatus.PasswordError
                };
            }
            List<CardDetail> cardDetails = new List<CardDetail>();
            var cards = JArray.Parse(d["CardArray"].ToString());
            foreach (var card in cards)
            {
                if (!decimal.TryParse(card.ToString(), out var bigCard))
                {
                    return new FcardCommandResult
                    {
                        Message = "删除卡,卡号错误:" + card,
                        Status = CommandStatus.PasswordError
                    };
                }
                var carddetail = new CardDetail();
                carddetail.BigCard.BigValue = bigCard;
                cardDetails.Add(carddetail);
            }
            var cmd = new DoNetDrive.Protocol.Door.Door89H.Card.DeleteCard(parameter.CommandDetail, new DoNetDrive.Protocol.Door.Door89H.Card.DeleteCard_Parameter(cardDetails));
            await parameter.Allocator.AddCommandAsync(cmd);
            return new FcardCommandResult
            {
                Message = "删除成功",
                Status = CommandStatus.Succeed
            };
        }
        /// <summary>
        /// 获取卡信息
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<IFcardCommandResult> GetCardInfo(IFcardCommandParameter parameter)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 读取新记录
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public async Task<IFcardCommandResult> GetRecord(IFcardCommandParameter parameter)
        {
            var cmd = new DoNetDrive.Protocol.Door.Door89H.Transaction.ReadTransactionDatabase(parameter.CommandDetail, new ReadTransactionDatabase_Parameter(e_TransactionDatabaseType.OnCardTransaction, 100));
            await parameter.Allocator.AddCommandAsync(cmd);
            var result = cmd.getResult() as ReadTransactionDatabase_Result;
            List<Models.CardRecord> cardList = new List<Models.CardRecord>();
            foreach (var item in result.TransactionList)
            {
                var card = item as DoNetDrive.Protocol.Door.Door89H.Data.CardTransaction;
                cardList.Add(new Models.CardRecord
                {
                    RecordNumber = card.SerialNumber,
                    Accesstype = (byte)(card.IsEnter() ? 1 : 2),
                    CardData = card.BigCard.UInt64Value.ToString(),
                    RecordDate = card.TransactionDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    RecordType = card.TransactionType,
                    RecordMsg = MessageType.CardTransactionCodeList[card.TransactionType][card.TransactionCode]
                });
            }
            return new FcardCommandResult()
            {
                Message = "读取记录成功",
                Status = CommandStatus.Succeed,
                Data = cardList
            };
        }
        /// <summary>
        /// 门常开
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public async Task<IFcardCommandResult> HoldDoor(IFcardCommandParameter parameter)
        {
           
                HoldDoor cmd = new HoldDoor(parameter.CommandDetail, new Remote_Parameter(GetDoorDetail(parameter)));
                await parameter.Allocator.AddCommandAsync(cmd);
                return new FcardCommandResult()
                {
                    Message = "开常开成功",
                    Status = CommandStatus.Succeed
                };
        }
        /// <summary>
        /// 远程开门
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public async Task<IFcardCommandResult> OpenDoor(IFcardCommandParameter parameter)
        {
            OpenDoor cmd = new OpenDoor(parameter.CommandDetail, new Remote_Parameter(GetDoorDetail(parameter)));
            await parameter.Allocator.AddCommandAsync(cmd);
            return new FcardCommandResult()
            {
                Message = "开门成功",
                Status = CommandStatus.Succeed
            };
        }
        /// <summary>
        /// 重置记录
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public async Task<IFcardCommandResult> ResetRecord(IFcardCommandParameter parameter)
        {
            var par = new WriteTransactionDatabaseReadIndex_Parameter(e_TransactionDatabaseType.OnCardTransaction, 0, true);
            var cmd = new WriteTransactionDatabaseReadIndex(parameter.CommandDetail, par);
            await parameter.Allocator.AddCommandAsync(cmd);
            return new FcardCommandResult()
            {
                Message = "重置成功",
                Status = CommandStatus.Succeed
            };
        }
        /// <summary>
        /// 获取门信息
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private DoorDetail<bool> GetDoorDetail(IFcardCommandParameter parameter)
        {
            Models.DoorsModel doors = null;
            if (!string.IsNullOrWhiteSpace(parameter.Data))
            {
                doors = JsonConvert.DeserializeObject<Models.DoorsModel>(parameter.Data);
            }
            if (doors == null)
                doors = Models.DoorsModel.GetInstance();
            DoorDetail<bool> Doors = new DoorDetail<bool>();
            Doors.SetDoor(1, doors.Door1);
            Doors.SetDoor(2, doors.Door2);
            Doors.SetDoor(3, doors.Door3);
            Doors.SetDoor(4, doors.Door4);
            return Doors;
        }
    }
}
