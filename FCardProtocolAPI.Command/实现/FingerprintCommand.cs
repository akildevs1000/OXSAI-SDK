using DoNetDrive.Core.Command;
using DoNetDrive.Protocol.Door.Door8800.SystemParameter.Watch;
using DoNetDrive.Protocol.Door.Door8800.Time;
using DoNetDrive.Protocol.Fingerprint.Data;
using DoNetDrive.Protocol.Fingerprint.Door.Remote;
using DoNetDrive.Protocol.Fingerprint.Person;
using DoNetDrive.Protocol.Fingerprint.SystemParameter;
using DoNetDrive.Protocol.Fingerprint.SystemParameter.ManageMenuPassword;
using DoNetDrive.Protocol.Fingerprint.SystemParameter.OEM;
using DoNetDrive.Protocol.Fingerprint.Transaction;
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
    public class FingerprintCommand : IFingerprintCommand
    {
        private static string[] PersonUploadStatus = new string[] {
            "",
        "上传完毕",
        "特征码无法识别",
        "人员照片不可识别",
        "人员照片或特征码重复"
        };


        /// <summary>
        /// 添加人员
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public async Task<IFcardCommandResult> AddPerson(IFcardCommandParameter parameter)
        {
            try
            {
                var check = CheckPersonnelParameter(parameter, out var commandParameter);

                if (!check.Item1)
                {
                    return new FcardCommandResult
                    {
                        Message = check.Item2,
                        Status = CommandStatus.ParameterError
                    };
                }
                INCommand cmd;
                if (parameter is AddPerson_Parameter)
                {
                    cmd = new AddPerson(parameter.CommandDetail, (AddPerson_Parameter)commandParameter);
                }
                else
                {
                    cmd = new AddPeosonAndImage(parameter.CommandDetail, (AddPersonAndImage_Parameter)commandParameter);
                }
                await parameter.Allocator.AddCommandAsync(cmd);
                var result = cmd.getResult();
                string message;
                if (result is AddPersonAndImage_Result)
                {
                    var r1 = result as AddPersonAndImage_Result;
                    message = $"人员上传{(r1.UserUploadStatus ? "成功" : "失败")}";
                    if (r1.IdDataRepeatUser[0] > 0)
                    {
                        message += "，用户号重复";
                    }
                    if (r1.IdDataUploadStatus[0] > 0)
                    {
                        message += "，识别信息上传状态:" + PersonUploadStatus[r1.IdDataUploadStatus[0]];
                    }
                    return new FcardCommandResult
                    {
                        Status = CommandStatus.Succeed,
                        Data = new
                        {
                            r1.UserUploadStatus,
                            IdDataRepeatUser = r1.IdDataRepeatUser[0],
                            IdDataUploadStatus = r1.IdDataUploadStatus[0],
                        },
                        Message = message
                    };
                }
                else
                {
                    var r2 = result as WritePerson_Result;
                    var UserUploadStatus = r2.FailTotal == 0;
                    message = $"人员上传{(UserUploadStatus ? "成功" : "失败")}";
                    return new FcardCommandResult
                    {
                        Data = new
                        {
                            UserUploadStatus,
                            IdDataRepeatUser = 0,
                            IdDataUploadStatus = 0
                        },
                        Message = message,
                        Status = CommandStatus.Succeed
                    };

                }
            }
            catch (Exception ex)
            {
                return new FcardCommandResult
                {
                    Message = "参数错误，请检查参数",
                    Status = CommandStatus.ParameterError
                };
            }
        }
        /// <summary>
        /// 检查人员参数
        /// </summary>
        /// <param name="iParameter"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private (bool, string) CheckPersonnelParameter(IFcardCommandParameter iParameter, out DoNetDrive.Protocol.Door.Door8800.AbstractParameter parameter)
        {
            (bool, string) result = (false, null);
            parameter = null;
            try
            {
                var p = JsonConvert.DeserializeObject<Models.PersonModel>(iParameter.Data);
                if (p == null)
                {
                    result.Item2 = "人员信息有误请检查";
                    return result;
                }

                if (string.IsNullOrWhiteSpace(p.name))
                {
                    result.Item2 = "缺少人员名称";
                    return result;
                }
                if (!uint.TryParse(p.userCode?.ToString(), out var userCode))
                {
                    result.Item2 = "缺少用户编号/用户编号有误";
                    return result;
                }
                var person = new Person();
                if (!string.IsNullOrWhiteSpace(p.code))
                {
                    if (!int.TryParse(p.code, out _))
                    {
                        result.Item2 = "人员编号必须是数字";
                        return result;
                    }
                    person.PCode = p.code;
                }

                person.PName = p.name;
                person.UserCode = userCode;
                if (!string.IsNullOrWhiteSpace(p.cardData))
                {
                    if (ulong.TryParse(p.cardData, out var cardData))
                    {
                        if (cardData < 0x1 || cardData >= 0xFFFFFFFF)
                        {
                            result.Item2 = "卡号超出取值范围：0x1-0xFFFFFFFF";
                            return result;
                        }
                        person.CardData = cardData;
                    }
                    else
                    {
                        result.Item2 = "卡号必须是十进制数字";
                        return result;
                    }
                }
                if (!string.IsNullOrWhiteSpace(p.password))
                {
                    if (p.password.Length >= 4 && p.password.Length <= 8)
                    {
                        if (!int.TryParse(p.password, out var password))
                        {
                            result.Item2 = "密码是4-8位的数字";
                            return result;
                        }
                        person.Password = p.password;
                    }
                    else
                    {
                        result.Item2 = "密码长度不够：密码是4-8位的数字";
                        return result;
                    }
                }
                if (p.identity != null)
                {
                    if (p.identity > 1 || p.identity < 0)
                    {
                        result.Item2 = "用户身份有误";
                        return result;
                    }
                    person.Identity = (int)p.identity;
                }
                if (p.cardStatus != null)
                {
                    if (p.cardStatus > 3 || p.cardStatus < 0)
                    {
                        result.Item2 = "卡片状态有误";
                        return result;
                    }
                    person.CardStatus = (int)p.cardStatus;
                }
                if (p.cardType != null)
                {
                    if (p.cardType > 1 || p.cardType < 0)
                    {
                        result.Item2 = "卡片状态有误";
                        return result;
                    }
                    person.CardType = (int)p.cardType;
                }
                if (p.enterStatus != null)
                {
                    if (p.enterStatus > 2 || p.enterStatus < 0)
                    {
                        result.Item2 = "出入标记有误";
                        return result;
                    }
                    person.EnterStatus = (int)p.enterStatus;
                }
                if (!string.IsNullOrWhiteSpace(p.expiry))
                {
                    if (DateTime.TryParse(p.expiry, out var expiry))
                    {
                        var time = DateTime.Parse("2089-12-31");
                        if (expiry > time)
                        {
                            expiry = time;
                        }
                        person.Expiry = expiry;
                    }
                    else
                    {
                        result.Item2 = "出入截止日期有误";
                        return result;
                    }
                }
                if (p.openTimes != null)
                {
                    if (p.openTimes > 65535 || p.openTimes < 0)
                    {
                        result.Item2 = "有效次数有误";
                        return result;
                    }
                    person.OpenTimes = (ushort)p.openTimes;
                }
                if (p.timeGroup != null)
                {
                    if (p.timeGroup < 0 || p.timeGroup > 64)
                    {
                        result.Item2 = "开门时段有误";
                        return result;
                    }
                    person.TimeGroup = (int)p.timeGroup;
                }
                for (int i = 0; i < 32; i++)
                {
                    person.SetHolidayValue(i + 1, false);
                }
                person.Job = p.job;
                person.Dept = p.dept;
                IdentificationData[] identifications = null;
                if (!string.IsNullOrWhiteSpace(p.faceImage) || p.fp != null)
                {

                    try
                    {

                        if (!string.IsNullOrWhiteSpace(p.faceImage))
                        {
                            if (!ImageTool.CheckFaceImage(p.faceImage, out byte[] faceData))
                            {
                                result.Item2 = "人员图片超过大小限制：像素尺寸要求 最大 480*640，最小 120*104，大小尺寸，最小 20kb，最大150kb";
                                return result;
                            }
                            faceData = ImageTool.ConvertImage(faceData);
                            identifications = new IdentificationData[1];
                            identifications[0] = new IdentificationData(1, faceData);
                        }
                        else
                        {
                            if (p.fp.Length > 3 || p.fp.Length <= 0)
                            {
                                result.Item2 = "指纹特征码数量错误：上传特征码数量不能低于0或者超过3个";
                                return result;
                            }
                            identifications = new IdentificationData[p.fp.Length];
                            for (int i = 0; i < identifications.Length; i++)
                            {
                                var fpData = Convert.FromBase64String(p.fp[i]);
                                identifications[i] = new IdentificationData(2, i, fpData);
                            }
                        }
                    }
                    catch
                    {
                        result.Item2 = "人员图片有误";
                        return result;
                    }
                    parameter = new AddPersonAndImage_Parameter(person, identifications);
                }
                else
                {
                    parameter = new AddPerson_Parameter(new List<Person>() { person });
                }
                result.Item1 = true;
            }
            catch (Exception ex)
            {
                result.Item2 = "参数异常，请检查参数";
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
            CloseDoor cmd = new(parameter.CommandDetail);
            await parameter.Allocator.AddCommandAsync(cmd);
            return new FcardCommandResult
            {
                Status = CommandStatus.Succeed,
                Message = "关门成功"
            };
        }
        /// <summary>
        /// 删除人员
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public async Task<IFcardCommandResult> DeletePerson(IFcardCommandParameter parameter)
        {
            var data = JObject.Parse(parameter.Data.ToString());
            Dictionary<string, object> d = new Dictionary<string, object>(data.ToObject<IDictionary<string, object>>(), StringComparer.CurrentCultureIgnoreCase);
            if (!d.ContainsKey("userCodeArray"))
            {
                return new FcardCommandResult
                {
                    Status = CommandStatus.ParameterError,
                    Message = "参数格式错误"
                };
            }
            var array = JArray.Parse(d["userCodeArray"].ToString());
            List<Person> personList = new List<Person>();
            foreach (var code in array)
            {
                if (uint.TryParse(code.ToString(), out var uCode))
                    personList.Add(new Person { UserCode = uCode });
            }
            var cmd = new DeletePerson(parameter.CommandDetail, new DeletePerson_Parameter(personList));
            await parameter.Allocator.AddCommandAsync(cmd);
            return new FcardCommandResult
            {
                Status = CommandStatus.Succeed,
                Message = "删除成功"
            };
        }
        /// <summary>
        /// 获取人员详情
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public async Task<IFcardCommandResult> GetPersonDetail(IFcardCommandParameter parameter)
        {
            var data = JObject.Parse(parameter.Data.ToString());
            Dictionary<string, string> d = new Dictionary<string, string>(data.ToObject<IDictionary<string, string>>(), StringComparer.CurrentCultureIgnoreCase);
            if (!d.ContainsKey("usercode") || !uint.TryParse(d["usercode"], out var iUserCode))
            {
                return new FcardCommandResult
                {
                    Status = CommandStatus.ParameterError,
                    Message = "参数格式错误"
                };
            }
            var par = new ReadPersonDetail_Parameter(iUserCode);
            var cmd = new ReadPersonDetail(parameter.CommandDetail, par);
            await parameter.Allocator.AddCommandAsync(cmd);
            var result = cmd.getResult() as ReadPersonDetail_Result;
            if (!result.IsReady)
            {
                return new FcardCommandResult
                {
                    Message = $"没有找到编号为【{iUserCode}】的人员信息",
                    Status = CommandStatus.CommandError
                };
            }
            var p = result.Person;
            var person = new Models.PersonModel
            {
                name = p.PName,
                cardData = p.CardData.ToString(),
                cardStatus = p.CardStatus,
                cardType = p.CardType,
                code = p.PCode,
                dept = p.Dept,
                enterStatus = p.EnterStatus,
                expiry = p.Expiry.ToString("yyyy-MM-dd HH:mm:ss"),
                identity = p.Identity,
                job = p.Job,
                openTimes = p.OpenTimes,
                password = p.Password,
                timeGroup = p.TimeGroup,
                userCode = (int)p.UserCode
            };
            if (result.Person.IsFaceFeatureCode)
            {
                var faceImage = await ReadFileData(parameter, iUserCode);
                if (faceImage != null)
                {
                    person.faceImage = Convert.ToBase64String(faceImage);
                }
            }
            if (result.Person.FingerprintFeatureCodeCout > 0)
            {
                person.fp = new string[result.Person.FingerprintFeatureCodeCout];
                for (int i = 0; i < result.Person.FingerprintFeatureCodeCout; i++)
                {
                    var feature = await ReadFeature(parameter, iUserCode, i);
                    if (feature != null)
                    {
                        person.fp[i] = Convert.ToBase64String(feature);
                    }
                }
            }
            return new FcardCommandResult
            {
                Data = person,
                Message = "查询成功",
                Status = CommandStatus.Succeed
            };

        }
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="iUserCode"></param>
        /// <param name="type"></param>
        /// <param name="serialNumber"></param>
        /// <returns></returns>
        private async Task<byte[]> ReadFileData(IFcardCommandParameter parameter, uint iUserCode)
        {
            DoNetDrive.Protocol.Fingerprint.AdditionalData.ReadFile cmd = new(parameter.CommandDetail,
                new DoNetDrive.Protocol.Fingerprint.AdditionalData.ReadFile_Parameter((int)iUserCode, 1, 1));
            await parameter.Allocator.AddCommandAsync(cmd);
            var result = cmd.getResult() as DoNetDrive.Protocol.Fingerprint.AdditionalData.ReadFile_Result;
            return result.FileDatas;
        }
        /// <summary>
        /// 读取指纹特征码
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="iUserCode"></param>
        /// <param name="serialNumber"></param>
        /// <returns></returns>
        private async Task<byte[]> ReadFeature(IFcardCommandParameter parameter, uint iUserCode, int serialNumber)
        {
            DoNetDrive.Protocol.Fingerprint.AdditionalData.ReadFeatureCode cmd = new(parameter.CommandDetail,
                new DoNetDrive.Protocol.Fingerprint.AdditionalData.ReadFeatureCode_Parameter((int)iUserCode, 2, serialNumber));
            await parameter.Allocator.AddCommandAsync(cmd);
            var result = cmd.getResult() as DoNetDrive.Protocol.Fingerprint.AdditionalData.ReadFeatureCode_Result;
            return result.FileDatas;
        }
        /// <summary>
        /// 获取记录
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public async Task<IFcardCommandResult> GetRecord(IFcardCommandParameter parameter)
        {
            var cmd = new ReadTransactionAndImageDatabase(parameter.CommandDetail, new ReadTransactionAndImageDatabase_Parameter(20, false, null));

            await parameter.Allocator.AddCommandAsync(cmd);
            var result = cmd.getResult() as ReadTransactionAndImageDatabase_Result;
            var tai = new Models.TransactionAndImage();
            tai.Quantity = result.Quantity;
            tai.Readable = result.readable;
            tai.RecordList = new List<Models.FaceTransaction>();
            foreach (var item in result.TransactionList)
            {
                tai.RecordList.Add(new Models.FaceTransaction
                {
                    UserCode = item.UserCode,
                    Accesstype = item.Accesstype,
                    BodyTemperature = ((double)item.BodyTemperature / 10),
                    Photo = item.Photo,
                    RecordDate = item.TransactionDate,
                    RecordImage = item.PhotoDataBuf,
                    RecordNumber = item.SerialNumber,
                    RecordType = item.TransactionType,
                    RecordMsg = MessageType.TransactionCodeNameList[item.TransactionType][item.TransactionCode]
                });
            }
            return new FcardCommandResult
            {
                Data = tai,
                Message = "读取记录成功",
                Status = CommandStatus.Succeed
            };
        }

        /// <summary>
        /// 门常开
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public async Task<IFcardCommandResult> HoldDoor(IFcardCommandParameter parameter)
        {
            HoldDoor cmd = new(parameter.CommandDetail);
            await parameter.Allocator.AddCommandAsync(cmd);
            return new FcardCommandResult
            {
                Status = CommandStatus.Succeed,
                Message = "门常开成功"
            };
        }
        /// <summary>
        /// 远程开门
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public async Task<IFcardCommandResult> OpenDoor(IFcardCommandParameter parameter)
        {
            OpenDoor cmd = new(parameter.CommandDetail);
            await parameter.Allocator.AddCommandAsync(cmd);
            return new FcardCommandResult
            {
                Status = CommandStatus.Succeed,
                Message = "开门成功"
            };
        }
        /// <summary>
        /// 修复记录
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public async Task<IFcardCommandResult> ResetRecord(IFcardCommandParameter parameter)
        {
            var par = new WriteTransactionDatabaseReadIndex_Parameter(e_TransactionDatabaseType.OnCardTransaction, 0);
            var cmd = new WriteTransactionDatabaseReadIndex(parameter.CommandDetail, par);
            await parameter.Allocator.AddCommandAsync(cmd);
            return new FcardCommandResult
            {
                Status = CommandStatus.Succeed,
                Message = "修复记录成功"
            };
        }
        /// <summary>
        /// 读取工作参数
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IFcardCommandResult> GetWorkParam(IFcardCommandParameter parameter)
        {
            Models.WorkParameter work = new Models.WorkParameter();

            var readOEM = new ReadOEM(parameter.CommandDetail);
            await parameter.Allocator.AddCommandAsync(readOEM);
            var oem = readOEM.getResult() as OEM_Result;
            work.Maker = oem.Detail;

            var readDriveLanguage = new ReadDriveLanguage(parameter.CommandDetail);
            await parameter.Allocator.AddCommandAsync(readDriveLanguage);
            var language = readDriveLanguage.getResult() as ReadDriveLanguage_Result;
            work.Language = (byte)language.Language;

            var readDriveVolume = new ReadDriveVolume(parameter.CommandDetail);
            await parameter.Allocator.AddCommandAsync(readDriveVolume);
            var volume = readDriveVolume.getResult() as ReadDriveVolume_Result;
            work.Volume = (byte)volume.Volume;

            var readManageMenuPassword = new ReadManageMenuPassword(parameter.CommandDetail);
            await parameter.Allocator.AddCommandAsync(readManageMenuPassword);
            var password = readManageMenuPassword.getResult() as ReadManageMenuPassword_Result;
            work.MenuPassword = password.Password;

            var readSaveRecordImage = new ReadSaveRecordImage(parameter.CommandDetail);
            await parameter.Allocator.AddCommandAsync(readSaveRecordImage);
            var saveImage = readSaveRecordImage.getResult() as ReadSaveRecordImage_Result;
            work.SavePhoto = (byte)(saveImage.SaveImageSwitch ? 1 : 0);

            var readWatchState = new ReadWatchState(parameter.CommandDetail);
            await parameter.Allocator.AddCommandAsync(readWatchState);
            work.MsgPush = readWatchState.WatchState;

            var readTime = new ReadTime(parameter.CommandDetail);
            await parameter.Allocator.AddCommandAsync(readTime);
            var time = readTime.getResult() as ReadTime_Result;
            work.Time = time.ControllerDate;

            var readLocalIdentity = new ReadLocalIdentity(parameter.CommandDetail);
            await parameter.Allocator.AddCommandAsync(readLocalIdentity);
            var identity = readLocalIdentity.getResult() as ReadLocalIdentity_Result;
            work.Name = identity.LocalName;
            work.Door = identity.InOut;

            return new FcardCommandResult
            {
                Data = work,
                Message = "查询成功",
                Status = CommandStatus.Succeed
            };
        }
        /// <summary>
        /// 设置工作参数
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IFcardCommandResult> SetWorkParam(IFcardCommandParameter parameter)
        {
            var par = JsonConvert.DeserializeObject<Models.WorkParameter>(parameter.Data);
            var check = CheckWorkParameter(par);
            if (!check.Item1)
            {
                return new FcardCommandResult
                {
                    Message = check.Item2,
                    Status = CommandStatus.PasswordError
                };
            }
            if (par.Name != null && par.Door != null)
            {
                WriteLocalIdentity cmd = new WriteLocalIdentity(parameter.CommandDetail, new WriteLocalIdentity_Parameter(1, par.Name, (byte)par.Door));
                await parameter.Allocator.AddCommandAsync(cmd);
            }

            if (par.Maker != null)
            {
                WriteOEM cmd = new WriteOEM(parameter.CommandDetail, new OEM_Parameter(par.Maker));
                await parameter.Allocator.AddCommandAsync(cmd);
            }

            if (par.Language != null)
            {
                WriteDriveLanguage cmd = new WriteDriveLanguage(parameter.CommandDetail, new WriteDriveLanguage_Parameter((int)par.Language));
                await parameter.Allocator.AddCommandAsync(cmd);
            }

            if (par.Volume != null)
            {
                WriteDriveVolume cmd = new WriteDriveVolume(parameter.CommandDetail, new WriteDriveVolume_Parameter((int)par.Volume));
                await parameter.Allocator.AddCommandAsync(cmd);
            }

            if (par.MenuPassword != null)
            {
                WriteManageMenuPassword cmd = new WriteManageMenuPassword(parameter.CommandDetail, new WriteManageMenuPassword_Parameter(par.MenuPassword));
                await parameter.Allocator.AddCommandAsync(cmd);
            }

            if (par.SavePhoto != null)
            {
                WriteSaveRecordImage cmd = new WriteSaveRecordImage(parameter.CommandDetail, new WriteSaveRecordImage_Parameter(par.SavePhoto == 1));
                await parameter.Allocator.AddCommandAsync(cmd);
            }

            if (par.MsgPush != null)
            {
                if (par.MsgPush == 0)
                {
                    CloseWatch cmd = new CloseWatch(parameter.CommandDetail);
                    await parameter.Allocator.AddCommandAsync(cmd);
                }
                else
                {
                    BeginWatch cmd = new BeginWatch(parameter.CommandDetail);
                    await parameter.Allocator.AddCommandAsync(cmd);
                }
            }
            if (par.Time != null)
            {
                WriteCustomTime cmd = new WriteCustomTime(parameter.CommandDetail, new WriteCustomTime_Parameter((DateTime)par.Time));
                await parameter.Allocator.AddCommandAsync(cmd);
            }
            return new FcardCommandResult
            {
                Message = "设置成功",
                Status = CommandStatus.Succeed
            };
        }

        public (bool, string) CheckWorkParameter(Models.WorkParameter parameter)
        {
            (bool, string) result;
            result.Item1 = false;
            result.Item2 = String.Empty;
            if (parameter == null)
            {
                result.Item2 = "参数错误，请检查参数格式";
                return result;
            }
            if (parameter.Name == null &&
                parameter.Door == null &&
                parameter.Maker == null &&
                parameter.Language == null &&
                parameter.Volume == null &&
                parameter.MenuPassword == null &&
                parameter.SavePhoto == null &&
                parameter.MsgPush == null &&
                parameter.Time == null)
            {
                result.Item2 = "参数错误，请检查参数格式";
                return result;
            }
            if (parameter.Maker != null)
            {
                if (parameter.Maker.Manufacturer == null || parameter.Maker.WebAddr == null || parameter.Maker.DeliveryDate == DateTime.MinValue)
                {
                    result.Item2 = "生产制造商信息错误";
                    return result;
                }

            }
            if (parameter.Name != null && parameter.Name.Length > 30)
            {
                result.Item2 = "设备名称信息错误:设备名称不能超过30个文字";
                return result;
            }
            if (parameter.Door != null && (parameter.Door > 1 || parameter.Door < 0))
            {
                result.Item2 = "设备进出方向错误：取值返回0-1";
                return result;
            }
            if ((parameter.Name != null && parameter.Door == null) || (parameter.Name == null && parameter.Door != null))
            {
                result.Item2 = "设备名称或进出方向两者必须同时填写";
                return result;
            }
            if (parameter.Maker != null)
            {
                var maker = parameter.Maker;
                if (maker.Manufacturer != null && maker.Manufacturer.Length > 30)
                {
                    result.Item2 = "生产制造商名称错误：生产制造商名称不能超过30个文字";
                    return result;
                }
                if (maker.WebAddr != null && maker.WebAddr.Length > 60)
                {
                    result.Item2 = "制造商网址错误：制造商网址不能超过60个文字";
                    return result;
                }
            }
            if (parameter.Language != null && (parameter.Language > 16 || parameter.Language < 1))
            {
                result.Item2 = "语言类型错误：取值范围 1-16";
                return result;
            }
            if (parameter.Volume != null && (parameter.Volume > 10 || parameter.Volume < 0))
            {
                result.Item2 = "音量错误：取值范围 0-10";
                return result;
            }
            if (parameter.MenuPassword != null &&
                (parameter.MenuPassword.Length > 8 || parameter.MenuPassword.Length < 4) &&
                int.TryParse(parameter.MenuPassword, out _))
            {
                result.Item2 = "菜单密码错误：仅支持4-8位数字密码";
                return result;
            }
            if (parameter.SavePhoto != null && (parameter.SavePhoto > 1 || parameter.SavePhoto < 0))
            {
                result.Item2 = "现场照片保存开关错误：取值返回0-1";
                return result;
            }
            if (parameter.MsgPush != null && parameter.MsgPush > 1 || parameter.MsgPush < 0)
            {
                result.Item2 = "消息推送开关错误：取值返回0-1";
                return result;
            }
            var maxDateTime = DateTime.Parse("2088-12-30");
            var minDateTime = DateTime.Parse("2000-01-01");
            if (parameter.Time != null && (parameter.Time < minDateTime || parameter.Time > maxDateTime))
            {
                result.Item2 = "日期时间同步错误：日期时间不能小于2000年01月01日并且不能大于2088-12-30";
                return result;
            }
            result.Item1 = true;
            return result;
        }

        public async Task<IFcardCommandResult> CommandTest(IFcardCommandParameter parameter)
        {

            DoNetDrive.Protocol.Fingerprint.AdditionalData.ReadFile cmd = new(parameter.CommandDetail,
                new DoNetDrive.Protocol.Fingerprint.AdditionalData.ReadFile_Parameter(10, 3, 1));

          await  parameter.Allocator.AddCommandAsync(cmd);
            var result = cmd.getResult() as DoNetDrive.Protocol.Fingerprint.AdditionalData.ReadFile_Result;
            return new FcardCommandResult
            {
                Data = result,
                Status = CommandStatus.Succeed
            };


        }
    }
}
